using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.IO;
using CustomFrameWork;

namespace ETModel
{
    /// <summary>
    /// 等待重连组件
    /// </summary>
    public sealed class RTChannel : AChannel
    {
        public CircularBuffer recvBuffer = new CircularBuffer();
        public CircularBuffer sendBuffer = new CircularBuffer();

        private readonly MemoryStream memoryStream;


        private readonly byte[] packetSizeCache;


        public RTChannel(TChannel channel,TService service) : base(service, ChannelType.Accept)
        {
            // 记录现场
            this.Id = channel.Id;
            this.RemoteAddress = channel.RemoteAddress;

            channel.TransferBufferTo(this);

            int packetSize = service.PacketSizeLength;
            this.packetSizeCache = new byte[packetSize];
            this.memoryStream = service.MemoryStreamManager.GetStream("message", ushort.MaxValue);

        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();

            this.memoryStream.Dispose();
        }

        private TService GetService()
        {
            return (TService)this.Service;
        }

        public override MemoryStream Stream
        {
            get
            {
                return this.memoryStream;
            }
        }

        public override void Start()
        {

        }

        public override void Send(MemoryStream stream)
        {
            if (this.IsDisposed)
            {
                throw new Exception("RTChannel已经被Dispose, 不能发送消息");
            }

            switch (this.GetService().PacketSizeLength)
            {
                case Packet.PacketSizeLength4:
                    if (stream.Length > ushort.MaxValue * 16 * 4)
                    {
                        throw new Exception($"send packet too large: {stream.Length}");
                    }
                    this.packetSizeCache.WriteTo(0, (int)stream.Length);
                    break;
                case Packet.PacketSizeLength2:
                    if (stream.Length > ushort.MaxValue)
                    {
                        throw new Exception($"send packet too large: {stream.Length}");
                    }
                    this.packetSizeCache.WriteTo(0, (ushort)stream.Length);
                    break;
                default:
                    throw new Exception("packet size must be 2 or 4!");
            }

            this.sendBuffer.Write(this.packetSizeCache, 0, this.packetSizeCache.Length);
            this.sendBuffer.Write(stream);
        }
    }
}