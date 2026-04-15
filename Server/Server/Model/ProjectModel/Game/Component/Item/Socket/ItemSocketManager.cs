using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 镶嵌物品管理
    /// </summary>
    public class ItemSocketManager : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 默认掉落孔数选择器
        /// </summary>
        public RandomSelector<int> DropHoleCountSelector = new RandomSelector<int>(); 


        public override void Dispose()
        {
            if (IsDisposeable) return;
            base.Dispose();
        }
    }
}
