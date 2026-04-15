using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
	[BsonIgnoreExtraElements]
	public class ItemsBoxStatus:IDisposable
	{
		[BsonElement("PageHeight")]
		protected int m_PageHeight = 0;//默认为0，只有一页
		[BsonElement("Width")]
		protected int m_Width;
		[BsonElement("ItemBoxStatus")]
		protected List<char> m_ItemBoxStatus_v = new List<char>();
		#region public
		public void Init(int width, int capacity)
		{
			SetWidth(width);
			SetSize(capacity);
		}

        public void SetPageHeight(int height) { m_PageHeight = height; }
		public int GetPageHeight() { return m_PageHeight; }
		public int GetWidth() { return m_Width; }
		public int GetSize() { return m_ItemBoxStatus_v.Count; }
		public List<char> GetItemBoxList()
		{
			return m_ItemBoxStatus_v;
		}
		public void Debug()
		{
			if (m_Width == 0)
			{
				Console.Write("[empty]\n");
				return;
			}
			int _width = 0;

			for (int i = 0; i < m_Width; ++i) Console.Write("==");
			Console.Write("\n");
			for (int i = 0; i < m_ItemBoxStatus_v.Count; i++)
			{
				char an = m_ItemBoxStatus_v[i];
				Console.Write(an > 0x00 ? "■" : "□");

				++_width;
				if (_width == m_Width)
				{
					Console.Write("\n");
					_width = 0;
				}
			}
			Console.Write("\n");
			for (int i = 0; i < m_Width; ++i) Console.Write("==");
			Console.Write("\n");
		}
		public bool CheckStatus(int width, int height, int x, int y)
		{
			//Log.Debug($"检查背包，将要加入的物品属性：width={width}  height={height}  x={x}  y={y}");
			//Log.Debug($"当前背包长宽：width:{GetWidth()} size:{GetSize()}");
			//Debug();
			if (m_PageHeight > 0)
			{
				// 是否在同一页
				if ((int)(y / m_PageHeight) != (int)((y + height - 1) / m_PageHeight))
				{
					// 不在同一页
					return false;
				}
			}
			int _id = 0;
			for (int height_val = 0; height_val < height; ++height_val)
			{
				for (int width_val = 0; width_val < width; ++width_val)
				{
					if ((x + width_val) >= m_Width)
					{
						// 超出宽度
						return false;
					}
					_id = POINT_TO_ID(x + width_val, y + height_val, m_Width);
					if (_id < 0 || _id >= (int)m_ItemBoxStatus_v.Count)
					{ return false; }
					if (m_ItemBoxStatus_v[_id] > 0)
					{ return false; }
				}
			}
			//Log.Debug($"检验通过，可以添加到背包");
			return true;
		}

		public bool CheckStatus(int width, int height,ref int _x,ref int _y)
		{
			for (int i = 0; i < m_ItemBoxStatus_v.Count; i++)
			{
				char an = m_ItemBoxStatus_v[i];
                if (an == 0x00)
                {
                    if (CheckStatus(width,height,i))
                    {
						ID_TO_POINT(ref _x, ref _y, m_Width, i);
						return true;
                    }
				}
			}
			return false;
		}
		/// <summary>
		/// 添加物品
		/// </summary>
		/// <param name="width">物品宽</param>
		/// <param name="height">物品高</param>
		/// <param name="x">坐标x</param>
		/// <param name="y">坐标y</param>
		/// <returns></returns>
		public bool AddItem(int width, int height, int x, int y)
		{
			if (!CheckStatus(width, height, x, y))
				return false;

			int _id = 0;
			for (int height_val = 0; height_val < height; ++height_val)
			{
				for (int width_val = 0; width_val < width; ++width_val)
				{
					_id = POINT_TO_ID(x + width_val, y + height_val, m_Width);
					if (_id < 0 || _id >= m_ItemBoxStatus_v.Count)
						return false;
					m_ItemBoxStatus_v[_id] = (char)0x01;
				}
			}
			//Log.Debug("=================添加物品成功，当前背包状态：");
			//Debug();
			return true;
		}

		public bool AddItem(int width, int height,ref int posX,ref int posY)
		{
            for (int i = 0; i < m_ItemBoxStatus_v.Count; i++)
            {
                if (m_ItemBoxStatus_v[i] != (char)0x01)
                {
                    if (AddItem(width,height,i))
                    {
						ID_TO_POINT(ref posX, ref posY, m_Width, i);
						return true;
                    }
                }
			}
			return false;
		}
		/// <summary>
		/// 移除物品
		/// </summary>
		/// <param name="width">物品宽</param>
		/// <param name="height">物品高</param>
		/// <param name="x">坐标x</param>
		/// <param name="y">坐标y</param>
		public void RemoveItem(int width, int height, int x, int y)
		{
			int _id = 0;
			for (int height_val = 0; height_val < height; ++height_val)
			{
				for (int width_val = 0; width_val < width; ++width_val)
				{
					_id = POINT_TO_ID(x + width_val, y + height_val, m_Width);
					if (_id < 0 || _id >= (int)m_ItemBoxStatus_v.Count)
						continue;
					m_ItemBoxStatus_v[_id] = (char)0x00;
				}
			}
			//Log.Debug("=================物品删除成功，当前背包状态：");
			//Debug();
		}

		public bool IncreaseSize(int val)
		{
			if (val <= 0)
			{
				return false;
			}
			if (m_ItemBoxStatus_v.Count == 0)
			{
				SetSize(val);
				return true;
			}
			int total_size = val;
			total_size += m_ItemBoxStatus_v.Count;
			m_ItemBoxStatus_v.Capacity = total_size;
			m_ItemBoxStatus_v.AddRange(Enumerable.Repeat((char)0x00, val));

			//uint space_size = sizeof(m_ItemBoxStatus_v[0]) * m_ItemBoxStatus_v.Count;
			//int space_size = m_ItemBoxStatus_v.Count;
			//char status_buff = new char[space_size];
			//if (!status_buff)
			//{
			//	return false;
			//}
			//// 记录状态
			//memcpy(status_buff, &m_ItemBoxStatus_v[0], space_size);
			//m_ItemBoxStatus_v.resize(m_ItemBoxStatus_v.size() + val);
			//Clear();
			//// 恢复状态
			//memcpy(&m_ItemBoxStatus_v[0], status_buff, space_size);
			return true;
		}
		public void Dispose()
		{
			m_PageHeight = 0;
			m_Width = 0;
			m_ItemBoxStatus_v.Clear();
		}

		public ItemsBoxStatus Clone()
		{
			ItemsBoxStatus result = new ItemsBoxStatus();
			result.m_ItemBoxStatus_v = new List<char>(this.m_ItemBoxStatus_v);
			result.m_PageHeight = this.m_PageHeight;
			result.m_Width = this.m_Width;
			return result;
		}
		#endregion

		#region protected
		protected int POINT_TO_ID(int _x, int _y, int _w)
		{
			return (_x) + (_y) * (_w);
		}
		protected void ID_TO_POINT(ref int _x, ref int _y, int _w, int _id)
		{
			_x = (_id) % (_w);
			_y = (_id) / (_w);
		}
		protected bool CheckStatus(int width, int height, int id)
		{
			if (id < 0 || id >= (int)m_ItemBoxStatus_v.Count)
				return false;
			int _x = 0;
			int _y = 0;

			ID_TO_POINT(ref _x, ref _y, m_Width, id);
			return CheckStatus(width, height, _x, _y);
		}
		

		protected bool AddItem(int width, int height, int id)
		{
			if (id < 0 || id >= m_ItemBoxStatus_v.Count)
				return false;
			int _x = 0;
			int _y = 0;
			ID_TO_POINT(ref _x, ref _y, m_Width, id);
			return AddItem(width, height, _x, _y);
		}

		protected void RemoveItem(int width, int height, int id)
		{
			if (id < 0 || id >= m_ItemBoxStatus_v.Count)
				return;
			int _x = 0;
			int _y = 0;
			ID_TO_POINT(ref _x, ref _y, m_Width, id);
			RemoveItem(width, height, _x, _y);

		}

		/// <summary>
		/// 设置格子宽度
		/// </summary>
		/// <param name="val"></param>
		protected void SetWidth(int val)
		{
			m_Width = val;
			Clear();
		}

		/// <summary>
		/// 设置格子总容量
		/// </summary>
		/// <param name="val"></param>
		protected void SetSize(int val)
		{
			m_ItemBoxStatus_v.Capacity = val;
			Clear();
		}

		protected void Clear()
		{
			for (int i = 0; i < m_ItemBoxStatus_v.Capacity; i++)
            {
				if (i < m_ItemBoxStatus_v.Count)
				{
					m_ItemBoxStatus_v[i] = (char)0x00;
				}
				else {
					m_ItemBoxStatus_v.Add((char)0x00);
				}
				
			}
		}
		#endregion

		#region lock
		/// <summary>
		/// 用来锁定存物品的格子
		/// </summary>
		public class Lock : IDisposable
		{
			private ItemsBoxStatus Box = null;
			private int PosX = 0;
			private int PosY = 0;
			private int Width = 0;
			private int Height = 0;


			public Lock(ItemsBoxStatus box,int posX,int posY,int width,int height)
			{
				Box = box;
				PosX = posX;
				PosY = posY;
				Width = width;
				Height = height;
            }

			~Lock()
            {
                Dispose();
            }

            public void Dispose()
            {
                if (Box != null)
                {
                    UnLock();
                }
            }

			/// <summary>
			/// 解锁
			/// </summary>
			public void UnLock()
            {
				if (Box == null) throw new Exception("没有锁定格子");
				Box.RemoveItem(Width, Height, PosX, PosY);
				Box = null;
			}
		}

        /// <summary>
        /// 锁定网格
        /// </summary>
        /// <example>
        /// using(ItemsBoxStatus.LockGird(1,1,0,0))
        /// {
        ///		会中断的代码
        ///		await ....
        /// }
        /// </example>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>

        public Lock LockGrid(int width, int height, int x, int y)
        {
			if(AddItem(width,height,x,y))
            {
				return new Lock(this, x, y, width, height);
            }
			return null;
        }

		/// <summary>
		/// 锁列表。使用时务必加 using
		/// </summary>
		public class LockList : List<Lock> , IDisposable
        {
            public static LockList Create()
            {
                return MonoPool<LockList>.Instance.Fetch();
            }

            public void Dispose()
            {
				foreach(var v in this)
                {
					v.Dispose();
                }
                this.Clear();
                MonoPool<LockList>.Instance.Recycle(this);
            }
        }
        #endregion
    }
}
