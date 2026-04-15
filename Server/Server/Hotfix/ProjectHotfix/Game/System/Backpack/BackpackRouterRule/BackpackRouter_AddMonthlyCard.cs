using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using ETModel;

namespace ETHotfix
{
    [BackpackRouter(320317)]    // 小月卡
    public class BackpackRouter_AddMinMonthlyCard : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            int durTime = (int)(item.GetProp(EItemValue.ValidTime) - Help_TimeHelper.GetNowSecond());
            if (durTime <= 0)
            {
                item.Delete($"进入背包({log})");
                return;
            }
            var PlayerShop = backpack.mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            PlayerShop.SetPlayerShopState(DeviationType.MinMonthlyCard, true, (uint)(durTime * 1000));
            PlayerShop.SendPlayerShopState();
            var equipComponent = backpack.mPlayer.GetCustomComponent<EquipmentComponent>();
            equipComponent?.ApplyEquipProp();
            item.Delete($"进入背包({log})");
        }
    }

    [BackpackRouter(320168)]    // 大月卡
    public class BackpackRouter_AddMaxMonthlyCard : IBackpackRouterHandler
    {
        public void Enter(BackpackComponent backpack, Item item, int posX, int posY, string log)
        {
            int durTime = (int)(item.GetProp(EItemValue.ValidTime) - Help_TimeHelper.GetNowSecond());
            if (durTime <= 0)
            {
                item.Dispose();
                return;
            }
            var PlayerShop = backpack.mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            PlayerShop.SetPlayerShopState(DeviationType.MaxMonthlyCard, true, (uint)(durTime * 1000));
            //PlayerShop.HeBingMonthlyCard(true, (uint)(durTime * 1000));
            PlayerShop.SendPlayerShopState();
            var equipComponent = backpack.mPlayer.GetCustomComponent<EquipmentComponent>();
            equipComponent?.ApplyEquipProp();
            item.Dispose();
        }
    }

}
