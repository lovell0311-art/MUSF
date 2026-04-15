using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetBattleCopyInfoRequestHandler : AMActorRpcHandler<C2G_GetBattleCopyInfoRequest, G2C_GetBattleCopyInfoRequest>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetBattleCopyInfoRequest b_Request, G2C_GetBattleCopyInfoRequest b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_GetBattleCopyInfoRequest b_Request, G2C_GetBattleCopyInfoRequest b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }

            BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
            if (batteCopyManagerCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2601);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有找到副本管理组件");
                b_Reply(b_Response);
                return false;
            }

            b_Response.BatteCopyStates = new RepeatedField<BattleCopyState>();
            foreach (var data in batteCopyManagerCpt.battleCopyMap)
            {
                b_Response.BatteCopyStates.Add(new BattleCopyState() 
                {
                    State = (int)data.Value.copyState,
                    PrepareTimer =data.Value.prepareTimer,
                    StartTimer =data.Value.startTimer,
                    EndTimer =data.Value.endTimer,
                });
            }

            b_Response.NumMaxs = new RepeatedField<int>();
            Dictionary<int, BattleCopy_ConditionConfig> mJsonConditionDic = 
                Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleCopy_ConditionConfigJson>().JsonDic;
            foreach (var data in mJsonConditionDic)
            {
                b_Response.NumMaxs.Add(data.Value.Challenge);
            }

            long userId = b_Request.ActorId;
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }
            DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();

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
                battleCopyData.updateTime = Help_TimeHelper.GetCurrenTimeStamp();
            }

            b_Response.Numbers = new RepeatedField<int>();

            b_Response.Numbers.Add(battleCopyData.demonSquaeNum);
            b_Response.Numbers.Add(battleCopyData.redCastleNum);

            b_Reply(b_Response);
            return true;
        }
    }
}
