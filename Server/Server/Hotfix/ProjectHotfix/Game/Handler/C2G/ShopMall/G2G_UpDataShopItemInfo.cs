using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class G2G_UpDataShopItemInfoHandler : AMHandler<G2G_UpDataShopItemInfo>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, G2G_UpDataShopItemInfo b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(G2G_UpDataShopItemInfo b_Request)
        {
            var ShopInfo = Root.MainFactory.GetCustomComponent<ShopMallComponent>();

            if (ShopInfo.ShopItemDic != null &&
                ShopInfo.ShopItemDic.TryGetValue((ShopMallType)b_Request.ShopType, out var valuePairs))
            {
                if (valuePairs.TryGetValue((ItemSortType)b_Request.Type, out var valuePairs1))
                {
                    if (valuePairs1.TryGetValue(b_Request.ShopID, out var shopItem))
                    {
                        if (shopItem.Id == b_Request.ItemId)
                            ShopInfo.ShopItemDic[(ShopMallType)b_Request.ShopType][(ItemSortType)b_Request.Type][b_Request.ShopID].limitCnt = b_Request.ItemCnt;
                    }
                }
            }

            return true;
        }
    }
}