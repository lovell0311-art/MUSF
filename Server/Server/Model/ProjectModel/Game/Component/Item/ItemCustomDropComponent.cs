using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETModel
{
    /// <summary>
    /// 自定义物品掉落
    /// </summary>
    public class ItemCustomDropComponent : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 可以掉落的物品
        /// k monsterId , (k dropTypeId , v CustomItemDrop_InfoConfig.Id)
        /// </summary>
        public Dictionary<int, Dictionary<int, RandomSelector<int>>> ItemDict = new Dictionary<int, Dictionary<int, RandomSelector<int>>>();

        /// <summary>
        /// 掉落类型选择器
        /// </summary>
        public RandomSelector<int> DropTypeSelector = new RandomSelector<int>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            ItemDict.Clear();

            base.Dispose();
        }
    }
}
