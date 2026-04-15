using ETModel;
using System;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Net;
using ETModel.Robot;
using TencentCloud.Gse.V20191112.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_EnterTheTestTowerHandler : AMActorRpcHandler<C2G_EnterTheTestTower, G2C_EnterTheTestTower>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_EnterTheTestTower b_Request, G2C_EnterTheTestTower b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_EnterTheTestTower b_Request, G2C_EnterTheTestTower b_Response, Action<IMessage> b_Reply)
        {
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
            var Title = mPlayer.GetCustomComponent<PlayerTitle>();
            if (Title == null)
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
            if(gamePlayer.Data.Level < 10) 
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(408);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            var PlayerShopInfo = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (PlayerShopInfo != null)
            {
                if (!PlayerShopInfo.GetPlayerShopState(DeviationType.MaxMonthlyCard))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(408);
                    b_Reply(b_Response);
                    return true;
                }
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

            if (mConfigDic.TryGetValue(battleCopyData.TrialTowerNum + 1, out var value))
            {
                int Ceng = battleCopyData.TrialTowerNum +1;
                int Expend = value.Expend;
                if (!Title.CheckTitle(60012))
                {
                    if (Ceng >= 4)
                    {
                        Ceng -= 2;
                        if (mConfigDic.TryGetValue(Ceng, out var value1))
                            Expend = value1.Expend;
                    }
                    else
                        Expend = 0;
                    
                }
                if (Expend != 0 && Expend <= mPlayer.Data.YuanbaoCoin)
                {
                   
                    gamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -Expend, "试炼塔消耗");
                    mWriteDataComponent.Save(gamePlayer.Data, dBProxy).Coroutine();
                    G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                    G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                    mBattleKVData.Key = (int)E_GameProperty.YuanbaoCoin;
                    mBattleKVData.Value = gamePlayer.GetNumerial(E_GameProperty.YuanbaoCoin);
                    mChangeValue_notice.Info.Add(mBattleKVData);
                    mPlayer.Send(mChangeValue_notice);
                }
                else if (Expend != 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3312);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("魔晶不足!");
                    b_Reply(b_Response);
                    return false;
                }
                battleCopyData.TrialTowerNum++;
                mWriteDataComponent.Save(battleCopyData, dBProxy).Coroutine();

            }
            if (batteCopyManagerCpt.TrialTowerList.TryGetValue(mPlayer.GameUserId, out var mapComponent))
            {
                if (mapComponent.Id == gamePlayer.CurrentMap.Id)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2103);
                    b_Reply(b_Response);
                    return false;
                }
                mapComponent.Dispose();
                batteCopyManagerCpt.TrialTowerList.Remove(mPlayer.GameUserId);
            }
           
            MapManageComponent mapManageComponent = mServerArea.GetCustomComponent<MapManageComponent>();
            var map = mapManageComponent.Copy(111);
            var mEnemy = mReadConfigComponent.GetJson<TrialTower_MonsterConfigJson>().JsonDic;
            if (mEnemy.TryGetValue(1, out var EnemyInfo))
            {
                map.GetCustomComponent<EnemyComponent>().InitMapEnemy(EnemyInfo.MobId, 1, EnemyInfo.Number, true);
            }
            batteCopyManagerCpt.TrialTowerList.Add(mPlayer.GameUserId, map);
            gamePlayer.CopyCount = 1;
            var mFindTheWaySource = gamePlayer.CurrentMap.GetFindTheWay2D(gamePlayer.UnitData.X, gamePlayer.UnitData.Y);
            if (mFindTheWaySource == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(509);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            int mTransferPointId = 10500;
            var mJsonMapDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;
            if (mJsonMapDic.TryGetValue(mTransferPointId, out var _TransferPointConfig) == false)
            {
                // 传送点不存在
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(513);
                b_Reply(b_Response);
                return false;
            }

            if (map.TransferPointFindTheWayDic.TryGetValue(mTransferPointId, out var mTransferPointlist) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(517);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("传送点初始化异常!");
                b_Reply(b_Response);
                return false;
            }
            if (mTransferPointlist == null || mTransferPointlist.Count == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(517);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("传送点初始化异常!");
                b_Reply(b_Response);
                return false;
            }

            var mRandomIndex = Help_RandomHelper.Range(0, mTransferPointlist.Count);
            var mTransferPoint = mTransferPointlist[mRandomIndex];

            // 公告移动信息
            map.MoveSendNotice(mFindTheWaySource, mTransferPoint, gamePlayer);

            gamePlayer.AddCustomComponent<TrialTowerCheckComponent>();
           
            b_Reply(b_Response);
            return true;
        }
    }
}
