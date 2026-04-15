using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Hcm.V20181106.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class M2G_ItemeXpireHandler : AMHandler<M2G_ItemeXpire>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, M2G_ItemeXpire b_Request)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request);
            }
        }
        protected override async Task<bool> Run(M2G_ItemeXpire b_Request)
        {
            var mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(b_Request.MAreaId, b_Request.GameUserID);
            if (mPlayer != null)
            {
                var maill = mPlayer.GetCustomComponent<PlayerMailComponent>();
                if (maill != null)
                {
                    maill.mailInfos.Clear();
                    maill.ServerMail.Clear();
                    DataCacheManageComponent mDataCacheComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
                    if (mDataCacheComponent.Remove<DBMailData>())
                        await maill.PlayerLoadMail(b_Request.MAreaId);
                }

                var TH = mPlayer.GetCustomComponent<TreasureHouseComponent>();
                if (TH != null)
                {
                    if (TH.Dle(b_Request.ItemUid))
                    {
                        Log.PLog($"角色藏宝阁物品到期下架 ItemUid:{b_Request.ItemUid}");
                    }
                    else
                    {
                        Log.PLog($"角色藏宝阁物品到期下架失败 ItemUid:{b_Request.ItemUid}");
                    }
                }
            }
            return true;
        }
    }
}