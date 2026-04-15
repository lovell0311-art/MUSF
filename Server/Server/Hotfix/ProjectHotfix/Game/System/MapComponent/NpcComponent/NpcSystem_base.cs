using System;
using System.Threading.Tasks;
using ETModel;

namespace ETHotfix
{



    public static partial class NpcSystem
    {
        public static void SetConfig(this GameNpc b_Component, Npc_InfoConfig b_Config)
        {
            b_Component.Config = b_Config;
        }

        public static void SetShopComponent(this GameNpc b_Component, NpcShopComponent shopComponent)
        {
            b_Component.ShopComponent = shopComponent;
        }

        public static void AfterAwake(this GameNpc b_Component)
        {
          
        }
        public static void DataUpdate(GameNpc b_Component)
        {
           


        }
    }
}