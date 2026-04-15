using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 物品套装管理
    /// </summary>
    public class ItemSetManager : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// k ItemConfigId v 套装选择器
        /// </summary>
        public Dictionary<int,RandomSelector<int>> ItemSetSelector = new Dictionary<int, RandomSelector<int>>();

        /// <summary>
        /// k ExtraEntryId v AttrEntryLevel
        /// </summary>
        public Dictionary<int, RandomSelector<int>> ExtraEntryLevelSelector = new Dictionary<int, RandomSelector<int>>();


        public override void Dispose()
        {
            if (IsDisposeable) return;
            base.Dispose();

            ItemSetSelector.Clear();
            ExtraEntryLevelSelector.Clear();
        }
    }
}
