using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Net.Sockets;
using UnityEngine.Profiling;

namespace ETModel
{
	[ObjectSystem]
	public class SessionAwakeSystem : AwakeSystem<Session, AChannel>
	{
		public override void Awake(Session self, AChannel b)
		{
			self.Awake(b);
		}
	}


    /// <summary>
    /// 会话实体
    /// </summary>
    public sealed class Session : Entity
	{
        //远程调用ID
        private static int RpcId { get; set; }
        //通道
		private AChannel channel;
        //请求回调
		private readonly Dictionary<int, Action<IResponse>> requestCallback = new Dictionary<int, Action<IResponse>>();
		private readonly byte[] opcodeBytes = new byte[2];

		public OpcodeTypeComponent opcodeTypeComponent;
        private long selfId = 0; // 自身id,用于记录。Dispose不会清零
        public NetworkComponent Network
		{
			get
			{
				return this.GetParent<NetworkComponent>();
			}
		}

		public int Error
		{
			get
			{
				return this.channel.Error;
			}
			set
			{
				this.channel.Error = value;
			}
		}

		public void Awake(AChannel aChannel)
		{
			//缓存通道
			this.channel = aChannel;
			this.requestCallback.Clear();
			this.selfId = this.Id;
			ClientNetTrace.Append($"SessionAwake session={this.Id} remote={this.channel.RemoteAddress}");
            //错误回调
			channel.ErrorCallback += this.OnError;
			//注册通道读取消息的回调 由通道内部触发
			channel.ReadCallback += this.OnRead;
            opcodeTypeComponent = this.Network.Entity.GetComponent<OpcodeTypeComponent>();
        }


		//释放
		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			this.Network.Remove(this.Id);

			base.Dispose();
			
			foreach (Action<IResponse> action in this.requestCallback.Values.ToArray())
			{
				action.Invoke(new ResponseMessage { Error = this.Error });
			}

			int error = this.channel.Error;
			if (this.channel.Error != 0)
			{
				Log.Trace($"session dispose: {this.Id} ErrorCode: {error}, please see ErrorCode.cs!");
			}

			this.channel.Dispose();
			
			this.requestCallback.Clear();
		
        }

		public void Start()
		{
			//内部进行连接服务器和接收来自服务器的消息.
			this.channel.Start();
		}

		public IPEndPoint RemoteAddress
		{
			get
			{
				return this.channel.RemoteAddress;
			}
		}

		public ChannelType ChannelType
		{
			get
			{
				return this.channel.ChannelType;
			}
		}

		public MemoryStream Stream
		{
			get
			{
				return this.channel.Stream;
			}
		}

        public void OnError(AChannel c, int e)
        {
            ClientNetTrace.Append($"SessionOnError session={this.selfId} remote={c.RemoteAddress} error={e}");
            this.Network.Remove(this.selfId);
        }

        //当读取的时候
        public void OnRead(MemoryStream memoryStream)
		{
			try
			{
				ClientNetTrace.Append($"SessionOnRead session={this.Id} remote={this.RemoteAddress} length={memoryStream.Length}");
				this.Run(memoryStream);
			}
			catch (Exception e)
			{
				ClientNetTrace.Append($"SessionOnReadException session={this.Id} type={e.GetType().Name} message={e.Message}");
				Log.Error(e);
			}
		}

		/// <summary>
		/// 读取消息
		/// </summary>
		/// <param name="memoryStream">包体数据</param>
		private void Run(MemoryStream memoryStream)
		{
			memoryStream.Seek(Packet.MessageIndex, SeekOrigin.Begin);
			//读取操作码 操作码也是属于包体的一部分
			ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.OpcodeIndex);
			ClientNetTrace.Append($"SessionRun session={this.Id} opcode={opcode} remote={this.RemoteAddress}");
			
#if !SERVER
            //如果是热更模块的操作码 >10000以上的
			if (OpcodeHelper.IsClientHotfixMessage(opcode))
			{
				SessionCallbackComponent callbackComponent = this.GetComponent<SessionCallbackComponent>();
				ClientNetTrace.Append($"SessionRunHotfix session={this.Id} opcode={opcode} hasCallback={callbackComponent != null}");
                //提交到热更层Session对象,在其内部注册的方法去处理
               // Profiler.BeginSample("Session.Run.SessionCallbackComponent"+opcode);
                callbackComponent.MessageCallback.Invoke(this, opcode, memoryStream);
              //  Profiler.EndSample();
                return;
			}
#endif
			
			object message;
			try
			{
              //  Profiler.BeginSample("this.Network.Entity.GetComponent<OpcodeTypeComponent>()");
                //通过操作码 获取到实例
               // OpcodeTypeComponent opcodeTypeComponent = this.Network.Entity.GetComponent<OpcodeTypeComponent>();
              //  Profiler.EndSample();
                object instance = opcodeTypeComponent.GetInstance(opcode);
                //反序列化 将内存流剩下的数据反序列化成proto实体
				message = this.Network.MessagePacker.DeserializeFrom(instance, memoryStream);

				//需要调试日志的信息 就打印出来
				if (OpcodeHelper.IsNeedDebugLogMessage(opcode))
				{
					//Log.Msg(message);
				}
               
            }
			catch (Exception e)
			{
				ClientNetTrace.Append($"SessionDeserializeException session={this.Id} opcode={opcode} type={e.GetType().Name} message={e.Message}");
				// 出现任何消息解析异常都要断开Session，防止客户端伪造消息
				Log.Error($"opcode: {opcode} {this.Network.Count} {e}, ip: {this.RemoteAddress}");
				this.Error = ErrorCode.ERR_PacketParserError;
				this.Network.Remove(this.Id);
				return;
			}

			//如果消息不是响应类型的消息 进行派发处理
			if (!(message is IResponse response))
			{
				ClientNetTrace.Append($"SessionDispatch session={this.Id} opcode={opcode} type={message?.GetType().Name ?? "null"}");
				//Log.DebugRed($"派发消息：opcode：{opcode} message：{message}");
				//进行派发处理
			//	Profiler.BeginSample("this.Network.MessageDispatcher.Dispatch");
				this.Network.MessageDispatcher.Dispatch(this, opcode, message);
			//	Profiler.EndSample();

                return;
			}

			//如果是响应类型的消息
			//从requestCallback缓存取出要执行的方法 将message压入到方法中 然后执行该方法 这涉及到ETTask的封装 等待了解
			Action<IResponse> action;
			if (!this.requestCallback.TryGetValue(response.RpcId, out action))
			{
				ClientNetTrace.Append($"SessionRpcMissing session={this.Id} opcode={opcode} rpc={response.RpcId} error={response.Error}");
				throw new Exception($"not found rpc, response message: {StringHelper.MessageToStr(response)}");
			}
			ClientNetTrace.Append($"SessionRpcResponse session={this.Id} opcode={opcode} rpc={response.RpcId} error={response.Error}");
			this.requestCallback.Remove(response.RpcId);

           // Profiler.BeginSample("action.response");
            action(response);
           // Profiler.EndSample();
        }

        /// <summary>
        /// 对外的发送请求接口
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
		public ETTask<IResponse> Call(IRequest request)
		{
			//RpcId逻辑自增 保证唯一性
			int rpcId = ++RpcId;
			ClientNetTrace.Append($"SessionCallRegister session={this.Id} request={request.GetType().Name} rpc={rpcId} remote={this.RemoteAddress}");
			//这里使用了ETTask 暂时只需要知道先声明ETTaskCompletionSource对象tcs
			//然后当tcs这个对象调用了SetResult,就会返回上一层await等待的地方,继续执行后面的语句
			var tcs = new ETTaskCompletionSource<IResponse>();
			//思路是先以key和value存储到字典里,当收到响应的时候,因为请求和响应的RPCID是一致的,所以可以通过RPCID取到Value
			//因为Value存储的是委托,所以收到响应的时候,调用一下即可执行这内部的tcs.SetResult(response);
			this.requestCallback[rpcId] = (response) =>
			{
				try
				{
					ClientNetTrace.Append($"SessionCallCallback session={this.Id} request={request.GetType().Name} rpc={rpcId} response={response?.GetType().Name ?? "null"} error={response?.Error ?? -1}");
					if (ErrorCode.IsRpcNeedThrowException(response.Error))
					{
						throw new RpcException(response.Error, response.Message);
					}
					//设置结果 即可返回await等待的地方
					tcs.SetResult(response);
				}
				catch (Exception e)
				{
					tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
				}
			};
			//设置RpcId 然后发送调用请求的方法
			request.RpcId = rpcId;
			ClientNetTrace.Append($"SessionCallSend session={this.Id} request={request.GetType().Name} rpc={rpcId} remote={this.RemoteAddress}");
			this.Send(request);
			return tcs.Task;
		}

		public ETTask<IResponse> Call(IRequest request, CancellationToken cancellationToken)
		{
			int rpcId = ++RpcId;
			var tcs = new ETTaskCompletionSource<IResponse>();

			this.requestCallback[rpcId] = (response) =>
			{
				try
				{
					if (ErrorCode.IsRpcNeedThrowException(response.Error))
					{
						throw new RpcException(response.Error, response.Message);
					}

					tcs.SetResult(response);
				}
				catch (Exception e)
				{
					tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName}", e));
				}
			};

			cancellationToken.Register(() => this.requestCallback.Remove(rpcId));
			request.RpcId = rpcId;

            Log.DebugPurple($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ffff")}  == {request.RpcId}  {request.ToString()}");
            this.Send(request);
			return tcs.Task;
		}

        //答复
		public void Reply(IResponse message)
		{
			if (this.IsDisposed)
			{
				throw new Exception("session已经被Dispose了");
			}

			this.Send(message);
		}

		/// <summary>
		/// 发送逻辑1-取到协议的操作码
		/// </summary>
		/// <param name="message"></param>
		public void Send(IMessage message)
		{
		//	OpcodeTypeComponent opcodeTypeComponent = this.Network.Entity.GetComponent<OpcodeTypeComponent>();
			//获取到协议号(操作码)
			ushort opcode = opcodeTypeComponent.GetOpcode(message.GetType());
			
			Send(opcode, message);
		}
		
		/// <summary>
		/// 发送逻辑2-将操作码和协议都写入到内存中
		/// </summary>
		/// <param name="opcode"></param>
		/// <param name="message"></param>
		public void Send(ushort opcode, object message)
		{
			if (this.IsDisposed)
			{
				throw new Exception("session已经被Dispose了");
			}
			//需要打印消息 就打印出来
			if (OpcodeHelper.IsNeedDebugLogMessage(opcode) )
			{
#if !SERVER
				if (OpcodeHelper.IsClientHotfixMessage(opcode))
				{
				}
				else
#endif
                {
					
				}
			}
			
			MemoryStream stream = this.Stream;//将数据写入到内存流中
			stream.Seek(Packet.MessageIndex, SeekOrigin.Begin);//设置写入的位置为2 因为留2个字节写入操作码
			stream.SetLength(Packet.MessageIndex);//写入的长度 2 tcp
			this.Network.MessagePacker.SerializeTo(message, stream);//开始写入proto序列化后的数组
			stream.Seek(0, SeekOrigin.Begin);//重设写入位置为0
			
			opcodeBytes.WriteTo(0, opcode);//将操作码opcode转化为opcodeBytes 操作码也就是协议号 
			Array.Copy(opcodeBytes, 0, stream.GetBuffer(), 0, opcodeBytes.Length);//将操作码复制到内存流中

#if SERVER
			// 如果是allserver，内部消息不走网络，直接转给session,方便调试时看到整体堆栈
			if (this.Network.AppType == AppType.AllServer)
			{
				Session session = this.Network.Entity.GetComponent<NetInnerComponent>().Get(this.RemoteAddress);
				session.Run(stream);
				return;
			}
#endif
			this.Send(stream);//调用下一个发送接口
		}

		/// <summary>
		/// 发送逻辑3-调用通道的发送接口
		/// </summary>
		/// <param name="stream"></param>
		public void Send(MemoryStream stream)
		{

			channel.Send(stream);
		}
	}
}
