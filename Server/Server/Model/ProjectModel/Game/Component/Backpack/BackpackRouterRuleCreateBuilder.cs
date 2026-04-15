using System;
using System.Collections.Generic;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETModel
{
    public class BackpackRouterAttribute : BaseAttribute
    {
        public int ItemConfigId = 0;
        public BackpackRouterAttribute(int itemConfigId) 
        {
            ItemConfigId = itemConfigId;
        }
    }

    public interface IBackpackRouterHandler
    {
        /// <summary>
        /// 物品即将进入背包,阻止物品进入。调用 item.Dispose()
        /// </summary>
        /// <param name="backpack"></param>
        /// <param name="item"></param>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <param name="log"></param>
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log);
    }


    // 背包路由
    public sealed class BackpackRouterRuleCreateBuilder : TCustomComponent<MainFactory>
    {
        public Dictionary<int, IBackpackRouterHandler> RouterRuleDict = new Dictionary<int, IBackpackRouterHandler>();


        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }

            RouterRuleDict.Clear();

            base.Dispose();
        }
    }
}