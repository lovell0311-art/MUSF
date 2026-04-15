
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Component;
using NLog.Fluent;
using TencentCloud.Mrs.V20200910.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_RenewItemRequestHandler : AMActorRpcHandler<C2G_RenewItemRequest, G2C_RenewItemRequest>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_RenewItemRequest b_Request, G2C_RenewItemRequest b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_RenewItemRequest b_Request, G2C_RenewItemRequest b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家不存在!");
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
            BackpackComponent backpack = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpack == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(702);
                b_Reply(b_Response);
                return true;
            }
            //找到物品
            if (backpack.mItemDict.TryGetValue(b_Request.ItemUUID, out Item curItem) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                b_Reply(b_Response);
                return true;
            }
            var itemConfig = curItem.ConfigData;
            int MoJin = 100;
            if (itemConfig.Id == 300001 || itemConfig.Id == 300002)
                MoJin = 36;
            else if (itemConfig.Id == 300004 || itemConfig.Id == 300003)
                MoJin = 56;
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                b_Reply(b_Response);
                return true;
            }
            if (mPlayer.Data.YuanbaoCoin < MoJin)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                b_Reply(b_Response);
                return false;
            }
            var OldTime = backpack.mItemDict[b_Request.ItemUUID].GetProp(EItemValue.ValidTime);
            OldTime += 15 * 24 * 60 * 60;
            backpack.mItemDict[b_Request.ItemUUID].SetProp(EItemValue.ValidTime, OldTime, mPlayer);
            mPlayer.GetCustomComponent<GamePlayer>().UpdateCoin(E_GameProperty.YuanbaoCoin, -MoJin, $"续费{b_Request.ItemUUID}");
            G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
            G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
            mBattleKVData.Key = (int)E_GameProperty.YuanbaoCoin;
            mBattleKVData.Value = mPlayer.GetCustomComponent<GamePlayer>().GetNumerial(E_GameProperty.YuanbaoCoin);
            mChangeValue_notice.Info.Add(mBattleKVData);
            mPlayer.Send(mChangeValue_notice);
            backpack.mItemDict[b_Request.ItemUUID].SendAllPropertyData(mPlayer);
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
            mWriteDataComponent.Save(mPlayer.Data, dBProxy).Coroutine();

            b_Reply(b_Response);
            return true;
        }
    }
}