using CustomFrameWork;
using CustomFrameWork.Component;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;

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

    public sealed class Session : Entity
    {
        private static int RpcId { get; set; } = 10000;
        private AChannel channel;
        public AChannel Channel { get => channel; }

        private readonly Dictionary<int, Action<IResponse>> requestCallback = new Dictionary<int, Action<IResponse>>();

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
            this.channel = aChannel;
            this.requestCallback.Clear();
            this.selfId = this.Id;
            channel.ErrorCallback += this.OnError;
            channel.ReadCallback += this.OnRead;
            channel.SessionId = this.Id;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            this.Network.Remove(this.Id);

            base.Dispose();

            this.channel.Dispose();


            var actionList = this.requestCallback.Values.ToArray();
            this.requestCallback.Clear();

            foreach (Action<IResponse> action in actionList)
            {
                action.Invoke(new ErrorResponse { Error = this.Error });
            }

            // 打印Session断开原因
            Log.Info($"#Session# ({selfId}) {Log.GetStacksInfo(1)}");

            //int error = this.channel.Error;
            //if (this.channel.Error != 0)
            //{
            //	Log.Trace($"session dispose: {this.Id} ErrorCode: {error}, please see ErrorCode.cs!");
            //}


        }

        public void Start()
        {
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

        public void OnRead(MemoryStream memoryStream)
        {
            try
            {
                this.Run(memoryStream);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }

        public void OnError(AChannel c, int e)
        {

            this.Network.Remove(this.selfId);
        }

        private void Run(MemoryStream memoryStream)
        {
            memoryStream.Seek(Packet.MessageIndex, SeekOrigin.Begin);
            ushort opcode = BitConverter.ToUInt16(memoryStream.GetBuffer(), Packet.OpcodeIndex);

            if (opcode == OuterOpcode.T2T_Ping)
            {
                var mPingComponent = Root.MainFactory.GetCustomComponent<PingComponent>();
                if (mPingComponent != null)
                {
                    mPingComponent.ReceiveTickDic[this.Id] = Help_TimeHelper.GetNowSecond();
                    return;
                }
            }

#if !SERVER
			if (OpcodeHelper.IsClientHotfixMessage(opcode))
			{
				this.GetComponent<SessionCallbackComponent>().MessageCallback.Invoke(this, opcode, memoryStream);
				return;
			}
#endif
            Type type = null;
            object message = null;
            try
            {
                type = OpcodeTypeComponent.Instance.GetType(opcode);
                message = MessageSerializeHelper.DeserializeFrom(opcode, type, memoryStream);

                //if (OpcodeHelper.IsNeedDebugLogMessage(opcode))
                if (opcode == ETHotfix.HotfixBattleOpcode.G2C_MovePosResponse
                    || opcode == ETHotfix.HotfixBattleOpcode.C2G_MovePosRequest
                    || opcode == ETHotfix.HotfixBattleOpcode.G2C_MovePos_notice
                    || opcode < 2000)
                {

                }
                else
                {
                    Log.Trace().Log("Reveice({session}):  {type}   Data:{@data}",
                        this.Id,
                        message,
                        message);
                }
            }
            catch (Exception e)
            {
                // 出现任何消息解析异常都要断开Session，防止客户端伪造消息
                Log.Error($"opcode: {opcode} {this.Network.Count} {e}, ip: {this.RemoteAddress}");
                this.Error = ErrorCode.ERR_PacketParserError;
                this.Network.Remove(this.Id);
                return;
            }

            if (!(message is IResponse response))
            {
                this.Network.MessageDispatcher.Dispatch(this, opcode, message);
                return;
            }

            Action<IResponse> action;
            if (this.requestCallback.TryGetValue(response.RpcId, out action))
            {
                this.requestCallback.Remove(response.RpcId);
                action(response);
            }
        }

        public Task<IResponse> CallWithoutException(IRequest request)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                if (response is ErrorResponse errorResponse)
                {
                    tcs.SetException(new Exception($"session close, errorcode: {errorResponse.Error} {errorResponse.Message}"));
                    return;
                }
                tcs.SetResult(response);
            };

            request.RpcId = rpcId;
            this.Send(request);
            return tcs.Task;
        }

        public Task<IResponse> CallWithoutException(IRequest request, CancellationToken cancellationToken)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                if (response is ErrorResponse errorResponse)
                {
                    tcs.SetException(new Exception($"session close, errorcode: {errorResponse.Error} {errorResponse.Message}"));
                    return;
                }
                tcs.SetResult(response);
            };

            cancellationToken.Register(() => this.requestCallback.Remove(rpcId));

            request.RpcId = rpcId;
            this.Send(request);
            return tcs.Task;
        }

        public async Task<IResponse> CallWithoutException(IRequest request, ETCancellationToken cancellationToken)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                if (response is ErrorResponse errorResponse)
                {
                    tcs.SetException(new Exception($"session close, errorcode: {errorResponse.Error} {errorResponse.Message}"));
                    return;
                }
                tcs.SetResult(response);
            };

            void CancelAction()
            {
                if (this.requestCallback.Remove(rpcId))
                {
                    tcs.SetResult(new ErrorResponse { Error = ErrorCode.ERR_CallCancel });
                }
            }

            IResponse ret = null;
            try
            {
                cancellationToken?.Add(CancelAction);
                request.RpcId = rpcId;
                this.Send(request);
                ret = await tcs.Task;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
            return ret;
        }

        public Task<IResponse> Call(IRequest request)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();
            this.requestCallback[rpcId] = (response) =>
            {
                if (ErrorCode.IsRpcNeedThrowException(response.Error))
                {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName} {response.Error}"));
                    return;
                }

                tcs.SetResult(response);
            };

            request.RpcId = rpcId;
            this.Send(request);
            return tcs.Task;
        }

        public Task<IResponse> Call(IRequest request, CancellationToken cancellationToken)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                if (ErrorCode.IsRpcNeedThrowException(response.Error))
                {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName} {response.Error}"));
                    return;
                }
                tcs.SetResult(response);
            };

            cancellationToken.Register(() => this.requestCallback.Remove(rpcId));

            request.RpcId = rpcId;
            this.Send(request);
            return tcs.Task;
        }

        public async Task<IResponse> Call(IRequest request, ETCancellationToken cancellationToken)
        {
            int rpcId = ++RpcId;
            var tcs = new TaskCompletionSource<IResponse>();

            this.requestCallback[rpcId] = (response) =>
            {
                if (ErrorCode.IsRpcNeedThrowException(response.Error))
                {
                    tcs.SetException(new Exception($"Rpc Error: {request.GetType().FullName} {response.Error}"));
                    return;
                }
                tcs.SetResult(response);
            };

            void CancelAction()
            {
                if (this.requestCallback.Remove(rpcId))
                {
                    tcs.SetResult(new ErrorResponse { Error = ErrorCode.ERR_CallCancel });
                }
            }

            IResponse ret = null;
            try
            {
                cancellationToken?.Add(CancelAction);
                request.RpcId = rpcId;
                this.Send(request);
                ret = await tcs.Task;
            }
            finally
            {
                cancellationToken?.Remove(CancelAction);
            }
            return ret;
        }

        public void Reply(IResponse message)
        {
            if (this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }

            this.Send(message);
        }

        public void Send(IMessage message)
        {
            if (message is IResponse response && message.GetType().Name == "R2C_RegisterOrLoginResponse")
            {
                Log.Info($"#SessionSend# session={this.Id} remote={this.RemoteAddress} type={message.GetType().Name} rpc={response.RpcId} error={response.Error}");
            }

            ushort opcode = OpcodeTypeComponent.Instance.GetOpcode(message.GetType());

            Send(opcode, message);
        }

        public void Send(ushort opcode, object message)
        {
            if (this.IsDisposed)
            {
                throw new Exception("session已经被Dispose了");
            }

            MemoryStream stream = this.Stream;

            stream.Seek(Packet.MessageIndex, SeekOrigin.Begin);
            stream.SetLength(Packet.MessageIndex);
            MessageSerializeHelper.SerializeTo(opcode, message, stream);
            stream.Seek(0, SeekOrigin.Begin);

            stream.GetBuffer().WriteTo(0, opcode);



            if (OpcodeHelper.IsNeedDebugLogMessage(opcode))
            {
#if !SERVER
				if (OpcodeHelper.IsClientHotfixMessage(opcode))
				{
				}
				else
#endif
                if (opcode == OuterOpcode.T2T_Ping
                   || opcode == ETHotfix.HotfixBattleOpcode.G2C_MovePosResponse
                   || opcode == ETHotfix.HotfixBattleOpcode.C2G_MovePosRequest
                   || opcode == ETHotfix.HotfixBattleOpcode.G2C_MovePos_notice
                   || opcode < 2000)
                {

                }
                else
                {
                    Log.Trace().Log("Send({session}): {type}   Data:[{size}]{@data}",
                        this.Id,
                        message,
                        stream.Length,
                        message);
                }
            }

#if SERVER
            // 如果是allserver，内部消息不走网络，直接转给session,方便调试时看到整体堆栈
            if (this.Network.AppType == AppType.AllServer)
            {
                Session session = this.Network.Entity.GetComponent<NetInnerComponent>().Get(this.RemoteAddress);
                session.Run(stream);
                return;
            }
#endif

            this.Send(stream);
        }

        public void Send(MemoryStream stream)
        {
            channel.Send(stream);
        }
    }
}
