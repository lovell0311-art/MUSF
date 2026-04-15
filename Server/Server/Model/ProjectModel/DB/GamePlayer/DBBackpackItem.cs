using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class DBBackpackItem : DBBase
    {
        /// <summary>
        /// 所属玩家，对应DBGamePlayer的ID，Player的GameUserId
        /// </summary>
        public long GameUserId { get; set; }
        //public DBItemData ItemData { get; set; }
        //public BackpackComponent backpackComponent;
        public int PageHeight;
        public int Width;
        public int Capacity;
        public int GameAreaId;

        public int IsDispose{ get; set; }   //1代表已删除
    }
}
