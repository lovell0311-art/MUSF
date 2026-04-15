using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Baseic;
using CustomFrameWork;

namespace ETModel
{
    /// <summary>
    /// 宝箱掉落组管理
    /// </summary>
    public class TreasureChestManager : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 物品选择器
        /// </summary>
        public Dictionary<int,RandomSelector<int>> ItemSelector = new Dictionary<int,RandomSelector<int>>();
        /// <summary>
        /// 特殊掉落物品选择器
        /// </summary>
        public Dictionary<int, RandomSelector<int>> SpecialDrop = new Dictionary<int, RandomSelector<int>>();
        public override void Dispose()
        {
            if (IsDisposeable) return;

            ItemSelector.Clear();
            SpecialDrop.Clear();

            base.Dispose();
        }
    }
}
