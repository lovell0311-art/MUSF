using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.IO;
using System.Runtime.InteropServices;

#if SERVER
using CustomFrameWork;
#endif

namespace ETModel
{
	public static class KcpProtocalType
	{
		public const byte SYN = 1;
		public const byte ACK = 2;
		public const byte FIN = 3;
		public const byte RCT = 4;  // 重连
        public const byte RACK = 5;  // 重连ACK
        public const byte MSG = 6;
        public const byte PING = 7; // ping
		public const byte NRCT = 8;	// 需要重新连接

        public const byte MSG_V1 = 16; // v1 版本MSG
    }

    public sealed class KService : AService
    {
        public static KService Instance { get; private set; }

        private uint IdGenerater = 1000;

		// KService创建的时间
		public long StartTime;
		// 当前时间 - KService创建的时间
		public uint TimeNow { get; private set; }

		private uint msgLastTime;

		private Socket socket;

		private readonly Dictionary<long, KChannel> localConnChannels = new Dictionary<long, KChannel>();
		
		private readonly byte[] cache = new byte[8192];

		private readonly Queue<long> removedChannels = new Queue<long>();
		
		public RecyclableMemoryStreamManager MemoryStreamManager = new RecyclableMemoryStreamManager();

		#region 连接相关
		// 记录等待连接的channel，10秒后或者第一个消息过来才会从这个dict中删除
		private readonly Dictionary<uint, KChannel> waitConnectChannels = new Dictionary<uint, KChannel>();
		#endregion

		#region 定时器相关
		// 下帧要更新的channel
		private readonly HashSet<long> updateChannels = new HashSet<long>();
		// 下次时间更新的channel
		private readonly MultiMap<long, long> timeId = new MultiMap<long, long>();
		private readonly List<long> timeOutTime = new List<long>();
		// 记录最小时间，不用每次都去MultiMap取第一个值
		private long minTime;
		#endregion


		private EndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 0);

		public KService(IPEndPoint ipEndPoint, Action<AChannel> acceptCallback)
		{
			this.AcceptCallback += acceptCallback;
			
			this.StartTime = TimeHelper.ClientNow();
			this.TimeNow = (uint)(TimeHelper.ClientNow() - this.StartTime);
			this.msgLastTime = TimeNow;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			//this.socket.Blocking = false;
			this.socket.Bind(ipEndPoint);
#if SERVER
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				const uint IOC_IN = 0x80000000;
				const uint IOC_VENDOR = 0x18000000;
				uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
				this.socket.IOControl((int)SIO_UDP_CONNRESET, new[] { Convert.ToByte(false) }, null);
			}
#endif
            Instance = this;
        }

        public KService()
		{
			this.StartTime = TimeHelper.ClientNow();
			this.TimeNow = (uint)(TimeHelper.ClientNow() - this.StartTime);
			this.msgLastTime = TimeNow;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			//this.socket.Blocking = false;
			this.socket.Bind(new IPEndPoint(IPAddress.Any, 0));
#if SERVER
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				const uint IOC_IN = 0x80000000;
				const uint IOC_VENDOR = 0x18000000;
				uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;
				this.socket.IOControl((int)SIO_UDP_CONNRESET, new[] { Convert.ToByte(false) }, null);
			}
#endif
            Instance = this;
        }

        public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			foreach (KeyValuePair<long, KChannel> keyValuePair in this.localConnChannels)
			{
				keyValuePair.Value.Dispose();
			}

			this.socket.Close();
			this.socket = null;
            Instance = null;
        }

		public void Recv()
		{
			if (this.socket == null)
			{
				return;
			}

			while (socket != null && this.socket.Available > 0)
			{
				int messageLength = 0;
				try
				{
					messageLength = this.socket.ReceiveFrom(this.cache, ref this.ipEndPoint);
				}
				catch (Exception e)
				{
					Log.Error(e);
					continue;
				}

				// 长度小于1，不是正常的消息
				if (messageLength < 1)
				{
					continue;
				}
				// accept
				byte flag = this.cache[0];
				ClientNetTrace.Append($"KServiceRecv flag={flag} len={messageLength} from={this.ipEndPoint}");
				
				// conn从1000开始，如果为1，2，3则是特殊包
				uint remoteConn = 0;
				uint localConn = 0;
				KChannel kChannel = null;
				KcpVersion version = KcpVersion.None;
				switch (flag)
				{
					case KcpProtocalType.SYN:  // accept
						// 长度!=5，不是accpet消息
						if (messageLength != 5 && messageLength != 7)
						{
							break;
						}

						IPEndPoint acceptIpEndPoint = (IPEndPoint)this.ipEndPoint;
						this.ipEndPoint = new IPEndPoint(0, 0);

						remoteConn = BitConverter.ToUInt32(this.cache, 1);
						if(messageLength >= 7)
						{
                            version = (KcpVersion)BitConverter.ToUInt16(this.cache, 5);
                        }
						// 如果已经收到连接，则忽略
						if (this.waitConnectChannels.TryGetValue(remoteConn, out kChannel))
						{
							break;
						}

						localConn = ++this.IdGenerater;
						kChannel = new KChannel(localConn, remoteConn, this.socket, acceptIpEndPoint, this);
                        kChannel.Version = version;

                        this.localConnChannels[kChannel.LocalConn] = kChannel;
						this.waitConnectChannels[remoteConn] = kChannel;

						this.OnAccept(kChannel);

						break;
					case KcpProtocalType.ACK:  // connect返回
						// 长度!=9，不是connect消息
						if (messageLength != 9)
						{
							break;
						}
						remoteConn = BitConverter.ToUInt32(this.cache, 1);
						localConn = BitConverter.ToUInt32(this.cache, 5);

                        kChannel = this.GetKChannel(localConn);
						ClientNetTrace.Append($"KServiceRecvAck local={localConn} remote={remoteConn} found={kChannel != null}");
						if (kChannel != null)
						{
							kChannel.HandleConnnect(remoteConn);
						}
						break;
					case KcpProtocalType.FIN:  // 断开
						// 长度!=13，不是DisConnect消息
						if (messageLength != 13)
						{
							break;
						}

						remoteConn = BitConverter.ToUInt32(this.cache, 1);
						localConn = BitConverter.ToUInt32(this.cache, 5);

                        // 处理chanel
                        kChannel = this.GetKChannel(localConn);
						if (kChannel != null)
						{
							// 校验remoteConn，防止第三方攻击
							if (kChannel.RemoteConn == remoteConn)
							{
								kChannel.Disconnect(ErrorCode.ERR_PeerDisconnect);
							}
						}
						break;
					case KcpProtocalType.RCT:  // 重连消息
                        // 长度!=9，不是重连消息
                        if (messageLength != 9)
                        {
                            break;
                        }
                        IPEndPoint rctAcceptIpEndPoint = (IPEndPoint)this.ipEndPoint;
                        this.ipEndPoint = new IPEndPoint(0, 0);

                        remoteConn = BitConverter.ToUInt32(this.cache, 1);
                        localConn = BitConverter.ToUInt32(this.cache, 5);

                        kChannel = this.GetKChannel(localConn);
						if (kChannel != null)
                        {
                            // 校验remoteConn，防止第三方攻击
                            if (kChannel.RemoteConn == remoteConn)
                            {
								kChannel.RemoteEndPoint = rctAcceptIpEndPoint;
								kChannel.ReAccept();
							}
                        }
                        break;
                    case KcpProtocalType.RACK:  // 重连返回
                        // 长度!=9，不是reconnect消息
                        if (messageLength != 9)
                        {
                            break;
                        }
                        remoteConn = BitConverter.ToUInt32(this.cache, 1);
                        localConn = BitConverter.ToUInt32(this.cache, 5);

                        kChannel = this.GetKChannel(localConn);
                        if (kChannel != null)
                        {
                            if (kChannel.RemoteConn == remoteConn)
                            {
								// 进行三次握手 Client RCT -> Server RACK -> Client RACK
								switch(kChannel.ChannelType)
                                {
									case ChannelType.Accept:
										// 服务端
                                        kChannel.HandleAccept();
                                        break;
                                    case ChannelType.Connect:
                                        // 客户端
                                        kChannel.HandleReconnect();
										break;
                                }
                            }
                        }
                        break;
                    case KcpProtocalType.MSG:  // 正常消息
											   // 长度<9，不是Msg消息
                        if (messageLength < 9)
						{
							break;
						}
						// 处理chanel
						remoteConn = BitConverter.ToUInt32(this.cache, 1);
						localConn = BitConverter.ToUInt32(this.cache, 5);

						this.waitConnectChannels.Remove(remoteConn);

                        kChannel = this.GetKChannel(localConn);
						ClientNetTrace.Append($"KServiceRecvMsg local={localConn} remote={remoteConn} found={kChannel != null} reconnecting={(kChannel != null ? kChannel.Reconnecting.ToString() : "null")}");
						if (kChannel != null)
						{
                            if (kChannel.Reconnecting == KChannel.ReconnectStatue.None)
							{
                                // 校验remoteConn，防止第三方攻击
                                if (kChannel.RemoteConn == remoteConn)
                                {
                                    kChannel.HandleRecv(this.cache, 5, messageLength - 5);
									if(msgLastTime != TimeNow)
									{
										msgLastTime = TimeNow;
                                        if (!kChannel.RemoteEndPoint.Equals((IPEndPoint)this.ipEndPoint))
                                        {
                                            // 网络地址变了，告诉客户端，需要重连
                                            kChannel.NeedReconnect((IPEndPoint)this.ipEndPoint);
                                        }
                                    }
                                    
                                }
                            }
						}
						else
						{
                            if (msgLastTime != TimeNow)
							{
								msgLastTime = TimeNow;
                                // 连接已断开，告诉客户端重连
                                cache.WriteTo(0, KcpProtocalType.FIN);
                                cache.WriteTo(1, localConn);
                                cache.WriteTo(5, remoteConn);
                                cache.WriteTo(9, (uint)SocketError.Shutdown);
                                this.socket.SendTo(cache, 0, 13, SocketFlags.None, (IPEndPoint)ipEndPoint);
                            }
                                
                        }
						break;
                    case KcpProtocalType.MSG_V1:  // 正常消息
                                               // 长度<11，不是Msg消息
                        {
                            if (messageLength < 11)
                            {
                                break;
                            }
                            // 处理chanel
                            remoteConn = BitConverter.ToUInt32(this.cache, 1);
                            ushort checksum = BitConverter.ToUInt16(this.cache, 5);
                            localConn = BitConverter.ToUInt32(this.cache, 7);
							this.cache.WriteTo(5, (ushort)0);
							ushort actualChecksum = Checksum(this.cache, 0, messageLength);

                            if (actualChecksum != checksum)
							{
								// 数据被修改
								ClientNetTrace.Append($"KServiceRecvMsgV1ChecksumMismatch local={localConn} remote={remoteConn} expected={checksum} actual={actualChecksum} len={messageLength}");
								break;
							}

                            this.waitConnectChannels.Remove(remoteConn);

                            kChannel = this.GetKChannel(localConn);
							ClientNetTrace.Append($"KServiceRecvMsgV1 local={localConn} remote={remoteConn} checksum={checksum} found={kChannel != null} reconnecting={(kChannel != null ? kChannel.Reconnecting.ToString() : "null")}");
                            if (kChannel != null)
                            {
                                if (kChannel.Reconnecting == KChannel.ReconnectStatue.None)
                                {
                                    // 校验remoteConn，防止第三方攻击
                                    if (kChannel.RemoteConn == remoteConn)
                                    {
                                        kChannel.HandleRecv(this.cache, 7, messageLength - 7);
                                        if (msgLastTime != TimeNow)
                                        {
                                            msgLastTime = TimeNow;
                                            if (!kChannel.RemoteEndPoint.Equals((IPEndPoint)this.ipEndPoint))
                                            {
                                                // 网络地址变了，告诉客户端，需要重连
                                                kChannel.NeedReconnect((IPEndPoint)this.ipEndPoint);
                                            }
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (msgLastTime != TimeNow)
                                {
                                    msgLastTime = TimeNow;
                                    // 连接已断开，告诉客户端重连
                                    cache.WriteTo(0, KcpProtocalType.FIN);
                                    cache.WriteTo(1, localConn);
                                    cache.WriteTo(5, remoteConn);
                                    cache.WriteTo(9, (uint)SocketError.Shutdown);
                                    this.socket.SendTo(cache, 0, 13, SocketFlags.None, (IPEndPoint)ipEndPoint);
                                }

                            }
                        }
                        break;
                    case KcpProtocalType.PING:  // Ping
                        // 长度<9，不是ping消息
                        if (messageLength < 9)
                        {
                            break;
                        }
                        // 处理chanel
                        remoteConn = BitConverter.ToUInt32(this.cache, 1);
                        localConn = BitConverter.ToUInt32(this.cache, 5);

                        this.waitConnectChannels.Remove(remoteConn);

                        kChannel = this.GetKChannel(localConn);
						ClientNetTrace.Append($"KServiceRecvPing local={localConn} remote={remoteConn} found={kChannel != null}");
                        if (kChannel != null && kChannel.Reconnecting == KChannel.ReconnectStatue.None)
                        {
                            // 校验remoteConn，防止第三方攻击
                            if (kChannel.RemoteConn == remoteConn)
                            {
                                kChannel.HandlePing();
                                if (!kChannel.RemoteEndPoint.Equals((IPEndPoint)this.ipEndPoint))
                                {
                                    // 网络地址变了，告诉客户端，需要重连
                                    kChannel.NeedReconnect((IPEndPoint)this.ipEndPoint);
                                }
                            }
                        }
                        break;
                    case KcpProtocalType.NRCT:  // 需要重连
                        // 长度<9，不是nrct消息
                        if (messageLength < 9)
                        {
                            break;
                        }
                        // 处理chanel
                        remoteConn = BitConverter.ToUInt32(this.cache, 1);
                        localConn = BitConverter.ToUInt32(this.cache, 5);


                        kChannel = this.GetKChannel(localConn);
                        if (kChannel != null && kChannel.Reconnecting == KChannel.ReconnectStatue.None)
                        {
                            // 校验remoteConn，防止第三方攻击
                            if (kChannel.RemoteConn == remoteConn)
                            {
								kChannel.StartReconnect();
                            }
                        }
                        break;
                }
            }
		}

		public KChannel GetKChannel(long id)
		{
			AChannel aChannel = this.GetChannel(id);
			if (aChannel == null)
			{
				return null;
			}

			return (KChannel)aChannel;
		}

		public override AChannel GetChannel(long id)
		{
			if (this.removedChannels.Contains(id))
			{
				return null;
			}
			KChannel channel;
			this.localConnChannels.TryGetValue(id, out channel);
			return channel;
		}

        public static void Output(IntPtr bytes, int count, IntPtr kcp, IntPtr user)
        {
            if(!KChannel.KcpPtrChannels.TryGetValue(kcp, out KChannel kChannel))
            {
                Log.Error($"not found kchannel, {(uint)kcp}");
                return;
            }
            kChannel.Output(bytes, count);
        }

        public override AChannel ConnectChannel(IPEndPoint remoteEndPoint)
		{
			uint localConn = (uint)RandomHelper.RandomNumber(1000, int.MaxValue);
			KChannel oldChannel;
			if (this.localConnChannels.TryGetValue(localConn, out oldChannel))
			{
				this.localConnChannels.Remove(oldChannel.LocalConn);
				oldChannel.Dispose();
			}

			KChannel channel = new KChannel(localConn, this.socket, remoteEndPoint, this);
			this.localConnChannels[channel.LocalConn] = channel;
			return channel;
		}

		public override AChannel ConnectChannel(string address)
		{
			IPEndPoint ipEndPoint2 = NetworkHelper.ToIPEndPoint(address);
			return this.ConnectChannel(ipEndPoint2);
		}

		public override void Remove(long id)
		{
			KChannel channel;
			if (!this.localConnChannels.TryGetValue(id, out channel))
			{
				return;
			}
			if (channel == null)
			{
				return;
			}
			this.removedChannels.Enqueue(id);

			// 删除channel时检查等待连接状态的字典是否要清除
			KChannel waitConnectChannel;
			if (this.waitConnectChannels.TryGetValue(channel.RemoteConn, out waitConnectChannel))
			{
				if (waitConnectChannel.LocalConn == channel.LocalConn)
				{
					this.waitConnectChannels.Remove(channel.RemoteConn);
				}
			}
		}

#if !SERVER
		// 客户端channel很少,直接每帧update所有channel即可,这样可以消除TimerOut方法的gc
		public void AddToUpdateNextTime(long time, long id)
		{
		}
		
		public override void Update()
		{
			this.TimeNow = (uint) (TimeHelper.ClientNow() - this.StartTime);

			this.Recv();

			foreach (var kv in this.localConnChannels)
			{
				kv.Value.Update();
			}
			
			while (this.removedChannels.Count > 0)
			{
				long id = this.removedChannels.Dequeue();
				KChannel channel;
				if (!this.localConnChannels.TryGetValue(id, out channel))
				{
					continue;
				}
				this.localConnChannels.Remove(id);
				channel.Dispose();
			}
		}
#else
		// 服务端需要看channel的update时间是否已到
		public void AddToUpdateNextTime(long time, long id)
		{
			if (time == 0)
			{	
				this.updateChannels.Add(id);
				return;
			}
			if (time < this.minTime)
			{
				this.minTime = time;
			}
			this.timeId.Add(time, id);
		}

		public override void Update()
		{
			this.TimeNow = (uint)(TimeHelper.ClientNow() - this.StartTime);
			
			this.Recv();
			
			this.TimerOut();

			foreach (long id in updateChannels)
			{
				KChannel kChannel = this.GetKChannel(id);
				if (kChannel == null)
				{
					continue;
				}
				if (kChannel.Id == 0)
				{
					continue;
				}
				kChannel.Update();
			}
			this.updateChannels.Clear();

			while (this.removedChannels.Count > 0)
			{
				long id = this.removedChannels.Dequeue();
				KChannel channel;
				if (!this.localConnChannels.TryGetValue(id, out channel))
				{
					continue;
				}
				this.localConnChannels.Remove(id);
				channel.Dispose();
			}
		}

		// 计算到期需要update的channel
		private void TimerOut()
		{
			if (this.timeId.Count == 0)
			{
				return;
			}

			uint timeNow = this.TimeNow;

			if (timeNow < this.minTime)
			{
				return;
			}
			this.timeOutTime.Clear();

			foreach (KeyValuePair<long, List<long>> kv in this.timeId.GetDictionary())
			{
				long k = kv.Key;
				if (k > timeNow)
				{
					minTime = k;
					break;
				}
				this.timeOutTime.Add(k);
			}
			foreach (long k in this.timeOutTime)
			{
				foreach (long v in this.timeId[k])
				{
					this.updateChannels.Add(v);
				}

				this.timeId.Remove(k);
			}
		}
#endif

        public static unsafe ushort Checksum(byte[] data, int offset, int count)
        {
            uint checksum = 0;
            int size = count;
            fixed (byte* p = data)
            {
                byte* p2 = p;
                p2 += offset;
                while (size > 3)
                {
                    checksum += *(uint*)p2;
                    size -= 4;
                    p2 += 4;
                }
                if (size == 3)
                {
                    checksum += *(ushort*)p2;
                    p2 += 2;
                    checksum += *p2;
                }
                else if (size == 2)
                {
                    checksum += *(ushort*)p2;
                }
                else if (size == 1)
                {
                    checksum += *p2;

                }
                while ((checksum >> 16) > 0)
                {
                    checksum = (checksum & 0xffff) + (checksum >> 16);
                }
            }
            return (ushort)checksum;
        }
    }
}
