using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ETModel
{
    public class CircularBuffer: Stream
    {
        public int ChunkSize = 8192;

        private readonly Queue<byte[]> bufferQueue = new Queue<byte[]>();

        private readonly Queue<byte[]> bufferCache = new Queue<byte[]>();

        public int LastIndex { get; set; }

        public int FirstIndex { get; set; }
		
        private byte[] lastBuffer;

	    public CircularBuffer()
	    {
		    this.AddLast();
	    }

        public override long Length
        {
            get
            {
                int c = 0;
                if (this.bufferQueue.Count == 0)
                {
                    c = 0;
                }
                else
                {
                    c = (this.bufferQueue.Count - 1) * ChunkSize + this.LastIndex - this.FirstIndex;
                }
                if (c < 0)
                {
                    Log.Error("CircularBuffer count < 0: {0}, {1}, {2}".Fmt(this.bufferQueue.Count, this.LastIndex, this.FirstIndex));
                }
                return c;
            }
        }

        public void AddLast()
        {
            byte[] buffer;
            if (this.bufferCache.Count > 0)
            {
                buffer = this.bufferCache.Dequeue();
            }
            else
            {
                buffer = new byte[ChunkSize];
            }
            this.bufferQueue.Enqueue(buffer);
            this.lastBuffer = buffer;
        }

        public void RemoveFirst()
        {
            this.bufferCache.Enqueue(bufferQueue.Dequeue());
        }

        public byte[] First
        {
            get
            {
                if (this.bufferQueue.Count == 0)
                {
                    this.AddLast();
                }
                return this.bufferQueue.Peek();
            }
        }

        public byte[] Last
        {
            get
            {
                if (this.bufferQueue.Count == 0)
                {
                    this.AddLast();
                }
                return this.lastBuffer;
            }
        }

		/// <summary>
		/// 从CircularBuffer读到stream中
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public async Task ReadAsync(Stream stream)
	    {
		    long buffLength = this.Length;
			int sendSize = this.ChunkSize - this.FirstIndex;
		    if (sendSize > buffLength)
		    {
			    sendSize = (int)buffLength;
		    }
			
		    await stream.WriteAsync(this.First, this.FirstIndex, sendSize);
		    
		    this.FirstIndex += sendSize;
		    if (this.FirstIndex == this.ChunkSize)
		    {
			    this.FirstIndex = 0;
			    this.RemoveFirst();
		    }
		}

	    // 从CircularBuffer读到stream
	    public void Read(Stream stream, int count)
	    {
		    if (count > this.Length)
		    {
			    throw new Exception($"bufferList length < count, {Length} {count}");
		    }

		    int alreadyCopyCount = 0;
		    while (alreadyCopyCount < count)
		    {
			    int n = count - alreadyCopyCount;
			    if (ChunkSize - this.FirstIndex > n)
			    {
				    stream.Write(this.First, this.FirstIndex, n);
				    this.FirstIndex += n;
				    alreadyCopyCount += n;
			    }
			    else
			    {
				    stream.Write(this.First, this.FirstIndex, ChunkSize - this.FirstIndex);
				    alreadyCopyCount += ChunkSize - this.FirstIndex;
				    this.FirstIndex = 0;
				    this.RemoveFirst();
			    }
		    }
	    }
	    
	    /// <summary>从stream写入CircularBuffer(效率低，懒得改了，能不用就不用)</summary>
	    public void Write(Stream stream)
		{
			int count = (int)(stream.Length - stream.Position);
			
			int alreadyCopyCount = 0;
			while (alreadyCopyCount < count)
			{
				if (this.LastIndex == ChunkSize)
				{
					this.AddLast();
					this.LastIndex = 0;
				}

				int n = count - alreadyCopyCount;
				if (ChunkSize - this.LastIndex > n)
				{
					stream.Read(this.lastBuffer, this.LastIndex, n);
					this.LastIndex += count - alreadyCopyCount;
					alreadyCopyCount += n;
				}
				else
				{
					stream.Read(this.lastBuffer, this.LastIndex, ChunkSize - this.LastIndex);
					alreadyCopyCount += ChunkSize - this.LastIndex;
					this.LastIndex = ChunkSize;
				}
			}
		}
	    

	    /// <summary>
		///  从stream写入CircularBuffer
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public async Task<int> WriteAsync(Stream stream)
	    {
		    int size = this.ChunkSize - this.LastIndex;
		    
		    int n = await stream.ReadAsync(this.Last, this.LastIndex, size);

		    if (n == 0)
		    {
			    return 0;
		    }

		    this.LastIndex += n;

		    if (this.LastIndex == this.ChunkSize)
		    {
			    this.AddLast();
			    this.LastIndex = 0;
		    }

		    return n;
	    }

	    // 把CircularBuffer中数据写入buffer
        public override int Read(byte[] buffer, int offset, int count)
        {
	        if (buffer.Length < offset + count)
	        {
		        throw new Exception($"bufferList length < coutn, buffer length: {buffer.Length} {offset} {count}");
	        }

	        long length = this.Length;
			if (length < count)
            {
	            count = (int)length;
            }

            int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                int n = count - alreadyCopyCount;
				if (ChunkSize - this.FirstIndex > n)
                {
                    unsafe
                    {
                        fixed (byte* pBuffer = buffer, pFirst = First)
                        {
                            Buffer.MemoryCopy(pFirst + this.FirstIndex, pBuffer + alreadyCopyCount + offset, ChunkSize, n);
                        }
                    }
                    this.FirstIndex += n;
                    alreadyCopyCount += n;
                }
                else
                {
                    unsafe
                    {
                        fixed (byte* pBuffer = buffer, pFirst = First)
                        {
                            Buffer.MemoryCopy(pFirst + this.FirstIndex, pBuffer + alreadyCopyCount + offset, ChunkSize, ChunkSize - this.FirstIndex);
                        }
                    }
                    alreadyCopyCount += ChunkSize - this.FirstIndex;
                    this.FirstIndex = 0;
                    this.RemoveFirst();
                }
            }

	        return count;
        }

	    // 把buffer写入CircularBuffer中
        public override void Write(byte[] buffer, int offset, int count)
        {
	        int alreadyCopyCount = 0;
            while (alreadyCopyCount < count)
            {
                if (this.LastIndex == ChunkSize)
                {
                    this.AddLast();
                    this.LastIndex = 0;
                }

                int n = count - alreadyCopyCount;
                if (ChunkSize - this.LastIndex > n)
                {
                    unsafe
                    {
						fixed(byte* pBuffer = buffer,pLastBuffer = lastBuffer)
                        {
							Buffer.MemoryCopy(pBuffer + alreadyCopyCount + offset, pLastBuffer + LastIndex, ChunkSize, n);
                        }
                    }
                    this.LastIndex += count - alreadyCopyCount;
                    alreadyCopyCount += n;
                }
                else
                {
                    unsafe
                    {
                        fixed (byte* pBuffer = buffer, pLastBuffer = lastBuffer)
                        {
                            Buffer.MemoryCopy(pBuffer + alreadyCopyCount + offset, pLastBuffer + LastIndex, ChunkSize, ChunkSize - this.LastIndex);
                        }
                    }
                    alreadyCopyCount += ChunkSize - this.LastIndex;
                    this.LastIndex = ChunkSize;
                }
            }
        }

		/// <summary>
		/// 将其他 CircularBuffer 中的数据移到自身
		/// </summary>
		/// <param name="buffer"></param>
		public void TransferFrom(CircularBuffer buffer)
        {
            while (buffer.Length > 0)
            {
                int sendSize = buffer.ChunkSize - buffer.FirstIndex;
                if (sendSize > buffer.Length)
                {
                    sendSize = (int)buffer.Length;
                }
                this.Write(buffer.First, buffer.FirstIndex, sendSize);
				//第一个索引+=传输的字节数
				buffer.FirstIndex += sendSize;
                //如果等于ChunkSize
                if (buffer.FirstIndex == buffer.ChunkSize)
                {
					buffer.FirstIndex = 0;
					//移除掉第一个(也就是已发送的数据)
					buffer.RemoveFirst();
                }
            }
        }

	    public override void Flush()
	    {
		    throw new NotImplementedException();
		}

	    public override long Seek(long offset, SeekOrigin origin)
	    {
			throw new NotImplementedException();
	    }

	    public override void SetLength(long value)
	    {
		    throw new NotImplementedException();
		}

	    public override bool CanRead
	    {
		    get
		    {
			    return true;
		    }
	    }

	    public override bool CanSeek
	    {
		    get
		    {
			    return false;
		    }
	    }

	    public override bool CanWrite
	    {
		    get
		    {
			    return true;
		    }
	    }

	    public override long Position { get; set; }
    }
}