using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SellingItemToNPCShopHandler : AMActorRpcHandler<C2G_SellingItemToNPCShop, G2C_SellingItemToNPCShop>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SellingItemToNPCShop b_Request, G2C_SellingItemToNPCShop b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SellingItemToNPCShop b_Request, G2C_SellingItemToNPCShop b_Response, Action<IMessage> b_Reply)
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

            var mPkNumber = mGamePlayer.GetNumerial(E_GameProperty.PkNumber);
            if (mPkNumber > 43200)
            {// 极恶
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1804);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }
            else if (mPkNumber > 21600)
            {// 红 
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1804);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }
            else if (mPkNumber > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1804);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }


            //可以不检测是否商店存在，直接检测物品是否可卖
            BackpackComponent backpack = mPlayer.GetCustomComponent<BackpackComponent>();
            Item item = backpack.GetItemByUID(b_Request.ItemUUID);
            if (item != null)
            {
                // TODO 物品状态限制 - 出售
//                 if (item.GetProp(EItemValue.IsTask) != 0)
//                 {
//                     // 任务物品无法出售
//                     b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3116);
//                     b_Reply(b_Response);
//                     return false;
//                 }
                if (item.GetProp(EItemValue.IsUsing) != 0)
                {
                    // 使用中的物品无法出售
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3117);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsLocking) != 0)
                {
                    // 锁定的物品无法出售
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3118);
                    b_Reply(b_Response);
                    return false;
                }

                if (item.GetProp(EItemValue.SellMoney) > 0)
                {
                    int sellMoney = item.GetProp(EItemValue.SellMoney);
                    //移除背包
                    backpack.DeleteItem(item, "出售物品给NPC");

                    //卖给商店直接销毁Item
                    //var PlayerData = mPlayer.GetCustomComponent<GamePlayer>().Data;
                    //PlayerData.GoldCoin += item.GetProp(EItemValue.SellMoney);
                 
                    mGamePlayer.UpdateCoin(E_GameProperty.GoldCoin, sellMoney, "出售物品给NPC");

                    //保存数据库
                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
                    mWriteDataComponent.Save(mGamePlayer.Data, dBProxy2).Coroutine();

                    var notice = new G2C_BackpackGoldChange_notice() { GameUserId = mPlayer.GameUserId, Gold = mGamePlayer.Data.GoldCoin };
                    mPlayer.Send(notice);

                    item.DisposeDB("出售物品给NPC");
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1802);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("物品不可卖!");
                    b_Reply(b_Response);
                    return false;
                }
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(813);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包找不到物品!");
                b_Reply(b_Response);
                return false;
            }
            b_Reply(b_Response);
            return true;
        }
    }
}