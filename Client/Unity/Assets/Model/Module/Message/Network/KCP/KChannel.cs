using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

#if SERVER
using CustomFrameWork;
#endif

namespace ETModel
{
	public enum KcpVersion : ushort
	{
		None = 0,
		V1 = 1,
	}

	public struct WaitSendBuffer
	{
		public byte[] Bytes;
		public int Length;

		public WaitSendBuffer(byte[] bytes, int length)
		{
			this.Bytes = bytes;
			this.Length = length;
		}
	}

	public class KChannel : AChannel
	{
		public static readonly Dictionary<IntPtr, KChannel> KcpPtrChannels = new Dictionary<IntPtr, KChannel>();
        public const uint SERVER_RECV_TIMEOUT = 1000 * 30;
        public const uint CLIENT_RECV_TIMEOUT = 1000 * 10;
		public const uint RECONNECT_TIMEOUT = 1000 * 20;
		public enum ReconnectStatue
        { 
			None,
			Reconnecting,
			Accepting,
		}

		private Socket socket;

		private IntPtr kcp;

		private readonly Queue<WaitSendBuffer> sendBuffer = new Queue<WaitSendBuffer>();

		private bool isConnected;
		/// <summary>
		/// 正在重连...
		/// </summary>
		private ReconnectStatue reconnecting = ReconnectStatue.None;
        public ReconnectStatue Reconnecting { get { return reconnecting; } }

		public IPEndPoint RemoteEndPoint;

		public KcpVersion Version;

        private uint lastRecvTime;
        private uint lastPingTime;
		private uint lastRecvPingTime;
		
		private readonly uint createTime;
		private uint reconnectTime;

		public uint RemoteConn { get; private set; }

		public uint SendBytesCount { get; private set; }
        public uint SendPackCount { get; private set; }

		public long LastRecvTimestamp => lastRecvTime + this.GetService().StartTime;
		public long LastPingTimestamp => lastPingTime + this.GetService().StartTime;
        public long LastRecvPingTimestamp => lastRecvPingTime + this.GetService().StartTime;


		private readonly MemoryStream memoryStream;
		private readonly object kcpSyncRoot = new object();
		private byte[] transportBuffer = new byte[512];
		private const int KcpSegmentOverhead = 24;

		// accept
		public KChannel(uint localConn, uint remoteConn, Socket socket, IPEndPoint remoteEndPoint, KService kService) : base(kService, ChannelType.Accept)
		{
			this.memoryStream = this.GetService().MemoryStreamManager.GetStream("message", ushort.MaxValue);

			this.SendBytesCount = 0;
			this.SendPackCount = 0;
            this.LocalConn = localConn;
			this.RemoteConn = remoteConn;
		    this.RemoteAddress = remoteEndPoint;
            this.RemoteEndPoint = remoteEndPoint;
			this.socket = socket;
			this.kcp = Kcp.KcpCreate(this.RemoteConn, new IntPtr(this.LocalConn));
			KcpPtrChannels.Add(this.kcp, this);

			SetOutput();
			Kcp.KcpNodelay(this.kcp,  1, 10, 1, 1);
			Kcp.KcpWndsize(this.kcp, 2048, 2048);
			Kcp.KcpSetmtu(this.kcp, 470);
			this.lastRecvTime = kService.TimeNow;
            this.lastPingTime = kService.TimeNow;
            this.lastRecvPingTime = kService.TimeNow;
			this.createTime = kService.TimeNow;
			this.Version = KcpVersion.None;
            this.Accept();
		}

		// connect
		public KChannel(uint localConn, Socket socket, IPEndPoint remoteEndPoint, KService kService) : base(kService, ChannelType.Connect)
		{
			this.memoryStream = this.GetService().MemoryStreamManager.GetStream("message", ushort.MaxValue);

			this.SendBytesCount = 0;
			this.SendPackCount = 0;
            this.LocalConn = localConn;
			this.socket = socket;
		    this.RemoteAddress = remoteEndPoint;
			this.RemoteEndPoint = remoteEndPoint;
			this.lastRecvTime = kService.TimeNow;
            this.lastPingTime = kService.TimeNow;
            this.lastRecvPingTime = kService.TimeNow;
            this.createTime = kService.TimeNow;
			this.Version = KcpVersion.V1;
            this.Connect();
        }

        public uint LocalConn
		{
			get
			{
				return (uint)this.Id;
			}
			set
			{
				this.Id = value;
			}
		}

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();

			try
			{
				if (this.Error == ErrorCode.ERR_Success)
				{
					for (int i = 0; i < 4; i++)
					{
						this.Disconnect();
					}
				}
			}
			catch (Exception)
			{
				// ignored
			}

			if (this.kcp != IntPtr.Zero)
			{
                KcpPtrChannels.Remove(this.kcp);
                Kcp.KcpRelease(this.kcp);
				this.kcp = IntPtr.Zero;
			}
			this.socket = null;
			this.memoryStream.Dispose();
		}

		public override MemoryStream Stream
		{
			get
			{
				return this.memoryStream;
			}
		}

		private byte[] GetTransportBuffer(int requiredLength)
		{
			if (this.transportBuffer.Length < requiredLength)
			{
				Array.Resize(ref this.transportBuffer, requiredLength);
			}

			return this.transportBuffer;
		}

		private int NormalizeRecvSegments(byte[] data, int offset, int length, uint conv)
		{
			if (data == null || length < KcpSegmentOverhead)
			{
				return 0;
			}

			int rewrittenSegments = 0;
			int cursor = 0;
			while (cursor + KcpSegmentOverhead <= length)
			{
				data.WriteTo(offset + cursor, conv);
				++rewrittenSegments;

				uint segmentLength = BitConverter.ToUInt32(data, offset + cursor + 20);
				long nextCursor = (long)cursor + KcpSegmentOverhead + segmentLength;
				if (nextCursor >= length)
				{
					return rewrittenSegments;
				}

				cursor = (int)nextCursor;
			}

			return rewrittenSegments;
		}

		public void Disconnect(int error)
		{
			this.OnError(error);
		}

		private KService GetService()
		{
			return (KService)this.Service;
		}

		public void HandleConnnect(uint remoteConn)
		{
			if (this.isConnected)
			{
				return;
			}

			this.RemoteConn = remoteConn;

			this.kcp = Kcp.KcpCreate(this.RemoteConn, new IntPtr(this.LocalConn));
            KcpPtrChannels.Add(this.kcp, this);
            SetOutput();
            Kcp.KcpNodelay(this.kcp, 1, 10, 1, 1);
			Kcp.KcpWndsize(this.kcp, 1024, 1024);
			Kcp.KcpSetmtu(this.kcp, 470);

			this.isConnected = true;
			this.lastRecvTime = this.GetService().TimeNow;
			ClientNetTrace.Append($"KChannelHandleConnect local={this.LocalConn} remote={this.RemoteConn} conv={this.RemoteConn} version={this.Version}");

			HandleSend();
		}

		public void Accept()
		{
			if (this.socket == null)
			{
				return;
			}
			
			uint timeNow = this.GetService().TimeNow;

			try
			{
				lock (this.kcpSyncRoot)
				{
					byte[] buffer = this.GetTransportBuffer(9);
					buffer.WriteTo(0, KcpProtocalType.ACK);
					buffer.WriteTo(1, LocalConn);
					buffer.WriteTo(5, RemoteConn);
					this.socket.SendTo(buffer, 0, 9, SocketFlags.None, RemoteEndPoint);
					this.SendBytesCount += 9;
					this.SendPackCount += 1;
				}
                // 200毫秒后再次update发送connect请求
                this.GetService().AddToUpdateNextTime(timeNow + 200, this.Id);
			}
			catch(SocketException e)
            {
				Log.Error(e);
                this.OnError(e.ErrorCode);
            }
            catch (Exception e)
			{
				Log.Error(e);
				this.OnError((int)SocketError.SocketError);
			}
		}


        public void HandleReconnect()
        {
            if (this.socket == null)
            {
                return;
            }
            try
            {
				lock (this.kcpSyncRoot)
				{
					byte[] buffer = this.GetTransportBuffer(9);
					buffer.WriteTo(0, KcpProtocalType.RACK);
					buffer.WriteTo(1, LocalConn);
					buffer.WriteTo(5, RemoteConn);
					this.socket.SendTo(buffer, 0, 9, SocketFlags.None, RemoteEndPoint);
					this.SendBytesCount += 9;
					this.SendPackCount += 1;
				}
            }
            catch (SocketException e)
            {
                // 网络中断，等待网络恢复后重连
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError((int)SocketError.SocketError);
            }


            if (this.reconnecting == ReconnectStatue.None)
            {
                return;
            }

            ReconnectComplete();
        }

		public void HandleAccept()
        {
            if (this.reconnecting == ReconnectStatue.None)
            {
                return;
            }

            ReconnectComplete();
        }

        public void ReAccept()
        {
            if (this.socket == null)
            {
                return;
            }

            uint timeNow = this.GetService().TimeNow;

            try
            {
				if(this.reconnecting != ReconnectStatue.Accepting)
                {
                    this.reconnecting = ReconnectStatue.Accepting;
					this.reconnectTime = timeNow;
                }
				lock (this.kcpSyncRoot)
				{
					byte[] buffer = this.GetTransportBuffer(9);
					buffer.WriteTo(0, KcpProtocalType.RACK);
					buffer.WriteTo(1, LocalConn);
					buffer.WriteTo(5, RemoteConn);
					this.socket.SendTo(buffer, 0, 9, SocketFlags.None, RemoteEndPoint);
					this.SendBytesCount += 9;
					this.SendPackCount += 1;
				}

                // 200毫秒后再次update发送connect请求
                this.GetService().AddToUpdateNextTime(timeNow + 200, this.Id);
            }
            catch (SocketException e)
            {
                // 网络中断，等待网络恢复后重连
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError((int)SocketError.SocketError);
            }
        }

		/// <summary>
		/// 发送请求连接消息
		/// </summary>
		private void Connect()
		{
			try
			{
				uint timeNow = this.GetService().TimeNow;
				
				this.lastRecvTime = timeNow;
				
				lock (this.kcpSyncRoot)
				{
					byte[] buffer = this.GetTransportBuffer(7);
					buffer.WriteTo(0, KcpProtocalType.SYN);
					buffer.WriteTo(1, this.LocalConn);
					buffer.WriteTo(5, (ushort)this.Version);
					this.socket.SendTo(buffer, 0, 7, SocketFlags.None, RemoteEndPoint);
					this.SendBytesCount += 7;
					this.SendPackCount += 1;
				}

                // 200毫秒后再次update发送connect请求
                this.GetService().AddToUpdateNextTime(timeNow + 300, this.Id);
			}
            catch (SocketException e)
            {
                Log.Error(e);
                this.OnError(e.ErrorCode);
            }
            catch (Exception e)
			{
				Log.Error(e);
				this.OnError((int)SocketError.SocketError);
			}
		}

		private void Reconnect()
        {
            try
            {
                uint timeNow = this.GetService().TimeNow;

                this.lastRecvTime = timeNow;

				lock (this.kcpSyncRoot)
				{
					byte[] buffer = this.GetTransportBuffer(9);
					buffer.WriteTo(0, KcpProtocalType.RCT);
					buffer.WriteTo(1, this.LocalConn);
					buffer.WriteTo(5, this.RemoteConn);
					this.socket.SendTo(buffer, 0, 9, SocketFlags.None, RemoteEndPoint);
					this.SendBytesCount += 9;
					this.SendPackCount += 1;
				}

                // 200毫秒后再次update发送reconnect请求
                this.GetService().AddToUpdateNextTime(timeNow + 300, this.Id);
            }
            catch (SocketException e)
            {
                // 网络中断，进行重连
                this.GetService().AddToUpdateNextTime(this.GetService().TimeNow + 300, this.Id);
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError((int)SocketError.SocketError);
            }
        }

		public void StartReconnect()
        {
			this.reconnectTime = this.GetService().TimeNow;

			switch (ChannelType)
            {
				case ChannelType.Accept:
                    // 服务器
                    // 等待客户端重连
                    reconnecting = ReconnectStatue.Reconnecting;
                    break;
                case ChannelType.Connect:
                    // 客户端
                    // 向服务器请求重连
                    reconnecting = ReconnectStatue.Reconnecting;
					Reconnect();
                    break;
            }

        }

		/// <summary>
		///  重连成功
		/// </summary>
		private void ReconnectComplete()
        {
            if (this.reconnecting == ReconnectStatue.None)
            {
				return;
            }
            this.lastRecvTime = this.GetService().TimeNow;
            this.reconnecting = ReconnectStatue.None;
            HandleSend();
        }

		/// <summary>
		/// 告诉客户端，需要重连
		/// </summary>
		/// <param name="toIpEndPoint"></param>
		public void NeedReconnect(IPEndPoint toIpEndPoint)
        {
            try
            {
				lock (this.kcpSyncRoot)
				{
					byte[] buffer = this.GetTransportBuffer(9);
					buffer.WriteTo(0, KcpProtocalType.NRCT);
					buffer.WriteTo(1, this.LocalConn);
					buffer.WriteTo(5, this.RemoteConn);
					this.socket.SendTo(buffer, 0, 9, SocketFlags.None, toIpEndPoint);
					this.SendBytesCount += 9;
					this.SendPackCount += 1;
				}
            }
            catch (SocketException e)
            {
                // 网络中断，进行重连
            }
            catch (Exception e)
            {
                Log.Error(e);
                this.OnError((int)SocketError.SocketError);
            }
        }

		private void Disconnect()
		{
			if (this.socket == null)
			{
				return;
			}
			try
			{
				lock (this.kcpSyncRoot)
				{
					byte[] buffer = this.GetTransportBuffer(13);
					buffer.WriteTo(0, KcpProtocalType.FIN);
					buffer.WriteTo(1, this.LocalConn);
					buffer.WriteTo(5, this.RemoteConn);
					buffer.WriteTo(9, (uint)this.Error);
					this.socket.SendTo(buffer, 0, 13, SocketFlags.None, RemoteEndPoint);
					this.SendBytesCount += 13;
					this.SendPackCount += 1;
				}
            }
            catch (SocketException e)
            {
                Log.Error(e);
                this.OnError(e.ErrorCode);
            }
            catch (Exception e)
			{
				Log.Error(e);
				this.OnError((int)SocketError.SocketError);
			}
		}

		public void Ping()
        {
            try
            {
				this.lastPingTime = this.GetService().TimeNow;
				lock (this.kcpSyncRoot)
				{
					byte[] buffer = this.GetTransportBuffer(9);
					buffer.WriteTo(0, KcpProtocalType.PING);
					buffer.WriteTo(1, this.LocalConn);
					buffer.WriteTo(5, this.RemoteConn);
					this.socket.SendTo(buffer, 0, 9, SocketFlags.None, RemoteEndPoint);
					this.SendBytesCount += 9;
					this.SendPackCount += 1;
				}
            }
            catch (SocketException e)
            {
                // 网络异常，开始尝试重连
                StartReconnect();
            }
			catch (Exception e)
            {
                Log.Error(e);
                this.OnError((int)SocketError.SocketError);
            }
        }

		public void Update()
		{
			if (this.IsDisposed)
			{
				return;
			}

			uint timeNow = this.GetService().TimeNow;
			
			// 如果还没连接上，发送连接请求
			if (!this.isConnected)
			{
				// 10秒没连接上则报错
				if (timeNow - this.createTime > 5 * 1000)
				{
					this.OnError((int)SocketError.TimedOut);
					return;
				}
				
				if (timeNow - this.lastRecvTime < 500)
				{
					return;
				}

				switch (ChannelType)
				{
					case ChannelType.Accept:
						this.Accept();
						break;
					case ChannelType.Connect:
						this.Connect();
						break;
				}
				
				return;
			}
			else if(this.reconnecting != ReconnectStatue.None)
            {
                // 正在重连中...
                // 20秒没重连接上则报错
                if (timeNow - this.reconnectTime > RECONNECT_TIMEOUT)
                {
                    this.OnError((int)SocketError.TimedOut);
                    return;
                }

                if (timeNow - this.lastRecvTime < 500)
                {
                    return;
                }
                switch (ChannelType)
                {
                    case ChannelType.Accept:
						if(this.reconnecting == ReconnectStatue.Accepting)
                        {
                            this.ReAccept();
                        }
                        break;
                    case ChannelType.Connect:
						this.Reconnect();
                        break;
                }
				return;
            }

            if (timeNow - this.lastPingTime > 5000)
            {
				Ping();
            }

			//超时断开连接
#if SERVER
			if (timeNow - this.lastRecvTime > SERVER_RECV_TIMEOUT)
			{
				if(timeNow - this.lastRecvPingTime > SERVER_RECV_TIMEOUT)
				{
                    this.OnError((int)SocketError.TimedOut);
                    return;
                }
			}
#else
			if (timeNow - this.lastRecvTime > CLIENT_RECV_TIMEOUT)
			{
				if(timeNow - this.lastRecvPingTime > SERVER_RECV_TIMEOUT)
				{
					StartReconnect();
					return;
				}
			}
#endif

            try
            {
				lock (this.kcpSyncRoot)
				{
					Kcp.KcpUpdate(this.kcp, timeNow);

					if (this.kcp != IntPtr.Zero)
					{
						uint nextUpdateTime = Kcp.KcpCheck(this.kcp, timeNow);
						this.GetService().AddToUpdateNextTime(nextUpdateTime, this.Id);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
				this.OnError((int)SocketError.SocketError);
				return;
			}
		}

		private void HandleSend()
		{
			lock (this.kcpSyncRoot)
			{
				while (true)
				{
					if (this.sendBuffer.Count <= 0)
					{
						break;
					}

					WaitSendBuffer buffer = this.sendBuffer.Dequeue();
					this.KcpSendNoLock(buffer.Bytes, buffer.Length);
				}
			}
		}

		public void HandleRecv(byte[] date, int offset, int length)
		{
			if (this.IsDisposed)
			{
				return;
			}

            this.isConnected = true;
			ClientNetTrace.Append($"KChannelHandleRecvEnter local={this.LocalConn} remote={this.RemoteConn} offset={offset} length={length} reconnecting={this.reconnecting}");

			try
			{
				lock (this.kcpSyncRoot)
				{
					uint conv = Kcp.KcpGetconv(this.kcp);
					uint packetConv = length >= 4 ? BitConverter.ToUInt32(date, offset) : 0;
					int rewrittenSegments = this.NormalizeRecvSegments(date, offset, length, conv);
					ClientNetTrace.Append($"KChannelHandleRecvConv local={this.LocalConn} remote={this.RemoteConn} conv={conv} packetConv={packetConv} rewritten={rewrittenSegments}");
					int inputRet = Kcp.KcpInput(this.kcp, date, offset, length);
					ClientNetTrace.Append($"KChannelHandleRecvInput local={this.LocalConn} remote={this.RemoteConn} ret={inputRet}");
					this.GetService().AddToUpdateNextTime(0, this.Id);

					while (true)
					{
						if (this.IsDisposed)
						{
							return;
						}

						int n = Kcp.KcpPeeksize(this.kcp);
						ClientNetTrace.Append($"KChannelHandleRecvPeek local={this.LocalConn} remote={this.RemoteConn} peek={n}");
						if (n < 0)
						{
							return;
						}
						if (n == 0)
						{
							ClientNetTrace.Append($"KChannelHandleRecvNetworkReset local={this.LocalConn} remote={this.RemoteConn}");
							this.OnError((int)SocketError.NetworkReset);
							return;
						}

						byte[] buffer = this.memoryStream.GetBuffer();
						this.memoryStream.SetLength(n);
						this.memoryStream.Seek(0, SeekOrigin.Begin);
						int count = Kcp.KcpRecv(this.kcp, buffer, ushort.MaxValue);
						ClientNetTrace.Append($"KChannelHandleRecvRead local={this.LocalConn} remote={this.RemoteConn} peek={n} count={count}");
						if (n != count)
						{
							return;
						}
						if (count <= 0)
						{
							return;
						}

						this.lastRecvTime = this.GetService().TimeNow;
						ClientNetTrace.Append($"KChannelHandleRecvDispatch local={this.LocalConn} remote={this.RemoteConn} count={count}");
						this.OnRead(this.memoryStream);
					}
				}
			}
			catch (Exception e)
			{
				ClientNetTrace.Append($"KChannelHandleRecvException local={this.LocalConn} remote={this.RemoteConn} type={e.GetType().Name} message={e.Message}");
				Log.Error(e);
				this.OnError((int)SocketError.SocketError);
			}
		}

		public void HandlePing()
        {
            if (this.IsDisposed)
            {
                return;
            }

			this.lastRecvPingTime = this.GetService().TimeNow;
        }

		public int KcpPeekSize()
		{
			lock (this.kcpSyncRoot)
			{
				return Kcp.KcpPeeksize(this.kcp);
			}
		}

        public override void Start()
		{
		}

		public void Output(IntPtr bytes, int count)
		{
			if (this.IsDisposed)
			{
				return;
			}
			if(this.reconnecting != ReconnectStatue.None)
            {
				return;
            }
			try
			{
				if (count == 0)
				{
					Log.Error($"output 0");
					return;
				}
				lock (this.kcpSyncRoot)
				{
					switch (this.Version)
					{
						case KcpVersion.None:
							{
								int sendLength = count + 5;
								byte[] buffer = this.GetTransportBuffer(sendLength);
								buffer.WriteTo(0, KcpProtocalType.MSG);
								// 每个消息头部写下该channel的id;
								buffer.WriteTo(1, this.LocalConn);
								Marshal.Copy(bytes, buffer, 5, count);
								this.socket.SendTo(buffer, 0, sendLength, SocketFlags.None, this.RemoteEndPoint);
								this.SendBytesCount += (uint)sendLength;
								this.SendPackCount += 1;
							}
							break;
						case KcpVersion.V1:
							{
								int sendLength = count + 7;
								byte[] buffer = this.GetTransportBuffer(sendLength);
								buffer.WriteTo(0, KcpProtocalType.MSG_V1);
								// 每个消息头部写下该channel的id;
								buffer.WriteTo(1, this.LocalConn);
								// checksum 让接收端校验数据是否被修改
								buffer.WriteTo(5, (ushort)0);
								Marshal.Copy(bytes, buffer, 7, count);
								ushort checksum = KService.Checksum(buffer, 0, sendLength);
								buffer.WriteTo(5, checksum);

								this.socket.SendTo(buffer, 0, sendLength, SocketFlags.None, this.RemoteEndPoint);
								this.SendBytesCount += (uint)sendLength;
								this.SendPackCount += 1;
							}
							break;
					}
				}
            }
            catch (SocketException e)
            {
				// 网络异常，开始尝试重连
				StartReconnect();
            }
			catch (Exception e)
			{
				Log.Error(e);
				this.OnError((int)SocketError.SocketError);
			}
		}
		
#if !ENABLE_IL2CPP
		private KcpOutput kcpOutput;
#endif

		public void SetOutput()
		{
#if ENABLE_IL2CPP
			Kcp.KcpSetoutput(this.kcp, KcpOutput);
#else
			// 跟上一行一样写法，pc跟linux会出错, 保存防止被GC
			kcpOutput = KcpOutput;
			Kcp.KcpSetoutput(this.kcp, kcpOutput);
#endif
		}


#if ENABLE_IL2CPP
		[AOT.MonoPInvokeCallback(typeof(KcpOutput))]
#endif
		public static int KcpOutput(IntPtr bytes, int len, IntPtr kcp, IntPtr user)
        {
            KService.Output(bytes, len, kcp, user);
            return len;
        }

        private void KcpSendNoLock(byte[] buffers, int length)
		{
			if (this.IsDisposed)
			{
				return;
			}
			Kcp.KcpSend(this.kcp, buffers, length);

			this.GetService().AddToUpdateNextTime(0, this.Id);
		}

		private void Send(byte[] buffer, int index, int length)
		{
			lock (this.kcpSyncRoot)
			{
				if (isConnected && reconnecting == ReconnectStatue.None)
				{
					this.KcpSendNoLock(buffer, length);
					return;
				}

				byte[] pendingBuffer = new byte[length];
				Buffer.BlockCopy(buffer, index, pendingBuffer, 0, length);
				this.sendBuffer.Enqueue(new WaitSendBuffer(pendingBuffer, length));
			}
		}

		public override void Send(MemoryStream stream)
		{
			lock (this.kcpSyncRoot)
			{
				if (this.kcp != IntPtr.Zero)
				{
					// 检查等待发送的消息，如果超出两倍窗口大小，应该断开连接
					if (Kcp.KcpWaitsnd(this.kcp) > 1024 * 2)
					{
						this.OnError((int)SocketError.MessageSize);
						return;
					}
				}

				ushort size = (ushort)(stream.Length - stream.Position);
				if (this.isConnected && reconnecting == ReconnectStatue.None)
				{
					this.KcpSendNoLock(stream.GetBuffer(), size);
					return;
				}

				byte[] bytes = new byte[size];
				Buffer.BlockCopy(stream.GetBuffer(), (int)stream.Position, bytes, 0, size);

				this.sendBuffer.Enqueue(new WaitSendBuffer(bytes, size));
			}
		}
	}
}
