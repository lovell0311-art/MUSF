using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETHotfix.ItemUpdateProp
{
    /// <summary>
    /// 二代半(2.5代)翅膀属性更新方法 B
    /// </summary>
    [ItemUpdateProp]
    public class Wing_SecondGeneration_B : Default
    {

        /// <summary>
        /// 更新 佩戴/使用 需求
        /// </summary>
        /// <param name="item">更新的物品</param>
        public override void UpdateRequire(Item item)
        {
            int level = item.GetProp(EItemValue.Level);

            item.SetProp(EItemValue.RequireLevel, item.ConfigData.ReqLvl);
        }


    }
}
