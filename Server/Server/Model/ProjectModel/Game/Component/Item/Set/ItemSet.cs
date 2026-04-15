using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;

namespace ETModel
{
    /// <summary>物品套装属性</summary>
    public class ItemSet : ADataContext
    {
        public int ConfigId;
        /// <summary>拥有这个套装的物品数</summary>
        public Dictionary<int,int> ItemId2Count = new Dictionary<int,int>();
        /// <summary>套装配置</summary>
        public SetItem_TypeConfig Config;
        /// <summary>有效的属性</summary>
        public List<int> ValidAttrEntryId = new List<int>();

        public HashSet<long> __HaveItem = new HashSet<long>();
        /// <summary>需要更新属性词条</summary>
        public bool NeedUpdate = false;

        public override void Dispose()
        {
            if (IsDisposeable) return;

            ConfigId = 0;
            ItemId2Count.Clear();
            Config = null;
            ValidAttrEntryId.Clear();
            __HaveItem.Clear();
            NeedUpdate = false;

            base.Dispose();
        }
    }
}
