using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [BackpackRouter(320501)]    
    [BackpackRouter(320502)]   
    [BackpackRouter(320503)]    
    [BackpackRouter(320504)]    
    [BackpackRouter(320505)]    
    [BackpackRouter(320506)]    
    [BackpackRouter(320507)]    
    [BackpackRouter(320508)]    
    //[BackpackRouter(320509)]    
    //[BackpackRouter(320510)]    
    //[BackpackRouter(320511)]    
    //[BackpackRouter(320512)]
    //[BackpackRouter(320513)]
    //[BackpackRouter(320519)]
    //[BackpackRouter(320520)]
    //[BackpackRouter(320521)]
    public class BackpackRouter_AddPlayerTitle : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            int titleId = item.ConfigID % 320500 + 60000;
            var Info = backpack.Parent.GetCustomComponent<PlayerTitle>();
            if (Info != null) 
            {
                int titleType = 0;
                if (item.GetProp(EItemValue.IsBind) == 1)
                {
                    titleType = 1;
                }
                if (item.GetProp(EItemValue.IsBind) == 2)
                {
                    titleType = 0;
                }
                Info.AddTitle(titleId, titleType, item.GetProp(EItemValue.ValidTime));
                Info.SendTitle();
            }
            item.Delete($"进入背包({log})");
        }
    }
}
