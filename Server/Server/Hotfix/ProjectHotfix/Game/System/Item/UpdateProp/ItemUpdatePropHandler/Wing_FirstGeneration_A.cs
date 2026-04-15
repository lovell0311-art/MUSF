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
    /// 一代翅膀属性更新方法 A
    /// </summary>
    [ItemUpdateProp]
    public class Wing_FirstGeneration_A : Default
    {

        /// <summary>
        /// 更新 佩戴/使用 需求
        /// </summary>
        /// <param name="item">更新的物品</param>
        public override void UpdateRequire(Item item)
        {
            int level = item.GetProp(EItemValue.Level);

            item.SetProp(EItemValue.RequireLevel, level * 4 + item.ConfigData.ReqLvl);
        }


    }
}
