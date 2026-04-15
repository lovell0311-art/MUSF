
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_TackOutGoldInWarehouseHandler : AMActorRpcHandler<C2G_TackOutGoldInWarehouse, G2C_TackOutGoldInWarehouse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_TackOutGoldInWarehouse b_Request, G2C_TackOutGoldInWarehouse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_TackOutGoldInWarehouse b_Request, G2C_TackOutGoldInWarehouse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }
            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            WarehouseComponent warehouse = mPlayer.GetCustomComponent<WarehouseComponent>();
            //移出金币到背包
            if (warehouse.DeductCoin((int)b_Request.Gold, $"转出到 角色:{mGamePlayer.Data.NickName}"))
            {
                //mGamePlayer.Data.GoldCoin += (int)b_Request.Gold;
                mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, (int)b_Request.Gold, $"转出到 角色:{mGamePlayer.Data.NickName}");

                //保存数据库
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                mWriteDataComponent.Save(mGamePlayer.Data, dBProxy2).Coroutine();

                //推送给玩家
                mPlayer.Send(new G2C_BackpackGoldChange_notice()
                {
                    GameUserId = mPlayer.GameUserId,
                    Gold = mGamePlayer.Data.GoldCoin
                });
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1902);//error:仓库金币不足
                b_Reply(b_Response);
                return false;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}