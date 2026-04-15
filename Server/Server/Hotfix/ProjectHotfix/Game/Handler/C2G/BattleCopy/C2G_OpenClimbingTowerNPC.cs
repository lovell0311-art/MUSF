using ETModel;
using System;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Net;
using ETModel.Robot;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenClimbingTowerNPCHandler : AMActorRpcHandler<C2G_OpenClimbingTowerNPC, G2C_OpenClimbingTowerNPC>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenClimbingTowerNPC b_Request, G2C_OpenClimbingTowerNPC b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_OpenClimbingTowerNPC b_Request, G2C_OpenClimbingTowerNPC b_Response, Action<IMessage> b_Reply)
        {
            b_Reply(b_Response);
            return true;
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }
            GamePlayer gamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (gamePlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            var Title = mPlayer.GetCustomComponent<PlayerTitle>();
            if (Title == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
            DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
            if (batteCopyManagerCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2600);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有找到副本管理组件");
                b_Reply(b_Response);
                return false;
            }
            var mBattleCopyData = mDataCacheManageComponent.Get<DBBattleCopyData>();
            if (mBattleCopyData == null)
            {
                mBattleCopyData = await mDataCacheManageComponent.Add<DBBattleCopyData>(dBProxy, p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
            }
            var mDatalist2 = mBattleCopyData.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
            if (mDatalist2.Count == 0)
            {
                DBBattleCopyData bBattleCopyData = new DBBattleCopyData()
                {
                    Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                    GameUserId = mPlayer.GameUserId,
                    GameAreaId = mPlayer.GameAreaId,
                    demonSquaeNum = batteCopyManagerCpt.demonSquaeNum,
                    redCastleNum = batteCopyManagerCpt.redCastleNum,
                    TrialTowerNum = 0,
                    updateTime = Help_TimeHelper.GetCurrenTimeStamp(),
                };
                bool mSaveResult = await dBProxy.Save(bBattleCopyData);
                if (mSaveResult == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1513);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("保存数据失败!");
                    b_Reply(b_Response);
                    return false;
                }
                mBattleCopyData.DataAdd(bBattleCopyData);
            }

            mDatalist2 = mBattleCopyData.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.GameAreaId == mPlayer.GameAreaId);
            if (mDatalist2.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1500);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏异常!");
                b_Reply(b_Response);
                return false;
            }
            DBBattleCopyData battleCopyData = mDatalist2[0];
            DateTime UpdateDate = Help_TimeHelper.ConvertStringToDateTime(battleCopyData.updateTime);
            DateTime CurrentDate = DateTime.Now;
            if (UpdateDate.Day != CurrentDate.Day)
            {
                battleCopyData.demonSquaeNum = batteCopyManagerCpt.demonSquaeNum;
                battleCopyData.redCastleNum = batteCopyManagerCpt.redCastleNum;
                battleCopyData.TrialTowerNum = 0;
                battleCopyData.updateTime = Help_TimeHelper.GetCurrenTimeStamp();
            }
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mConfigDic = mReadConfigComponent.GetJson<TrialTower_ExpendConfigJson>().JsonDic;
            if(mConfigDic.TryGetValue(battleCopyData.TrialTowerNum+1,out var value))
            {
               
            }
            b_Reply(b_Response);
            return true;
        }
    }
}
