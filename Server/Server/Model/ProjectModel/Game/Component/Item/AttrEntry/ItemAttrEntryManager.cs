using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 物品属性词条管理
    /// </summary>
    public class ItemAttrEntryManager : TCustomComponent<MainFactory>
    {
        public Dictionary<(int configId, int level), ItemAttrEntry> AllEntry = new Dictionary<(int, int), ItemAttrEntry>();


        public override void Dispose()
        {
            if (IsDisposeable) return;

            AllEntry.Clear();

            base.Dispose();
        }
    }
}
