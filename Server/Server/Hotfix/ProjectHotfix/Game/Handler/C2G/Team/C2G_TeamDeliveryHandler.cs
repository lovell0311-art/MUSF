using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_TeamDeliveryHandler : AMActorRpcHandler<C2G_TeamDeliveryRequest, G2C_TeamDeliveryResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_TeamDeliveryRequest b_Request, G2C_TeamDeliveryResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_TeamDeliveryRequest b_Request, G2C_TeamDeliveryResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();

            var mPkNumber = mGamePlayer.GetNumerial(E_GameProperty.PkNumber);
            if (mPkNumber > 43200)
            {// 极恶
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(519);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }
            else if (mPkNumber > 21600)
            {// 红 
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(519);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }
            else if (mPkNumber > 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(519);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("红名状态,不能传送!");
                b_Reply(b_Response);
                return false;
            }


            var mTeamComponent = mPlayer.GetCustomComponent<TeamComponent>();
            if (mTeamComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1206);
                b_Reply(b_Response);
                return true;
            }

            TeamManageComponent mTeamManageComponent = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
            var mDic = mTeamManageComponent.GetAllByTeamID(mTeamComponent.TeamID);
            if (mDic == null
                || mDic.ContainsKey(mGamePlayer.InstanceId) == false
                || mDic.TryGetValue(b_Request.GameUserId, out var FriendPlayer) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1206);
                b_Reply(b_Response);
                return true;
            }

            var FriendMapInfo = FriendPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer == null && FriendMapInfo == null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_AccountNotExists;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置信息异常!");
                b_Reply(b_Response);
                return false;
            }

            if (mGamePlayer.IsDeath)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_DBProxyNotFoundError;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("请等待复活后尝试攻击!");
                b_Reply(b_Response);
                return false;
            }
            //if (mGamePlayer.IsAttacking)
            //{
            //    b_Response.Error = ErrorCodeHotfix.ERR_DBProxyNotFoundError;
            //    b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("正在攻击!");
            //    b_Reply(b_Response);
            //    return false;
            //}
            //else
            if (mGamePlayer.IsCanOperation() == false)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_DBProxyNotFoundError;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("异常状态,不能移动!");
                b_Reply(b_Response);
                return false;
            }

            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(FriendMapInfo.UnitData.Index, out var MapInfoConfig) == false)
            {
                // 传送点不存在
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(513);
                b_Reply(b_Response);
                return false;
            }

            if (mGamePlayer.Data.Level < MapInfoConfig.GotoMapByLevel)
            {
                // 进入等级不足
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(514);
                b_Reply(b_Response);
                return false;
            }
            if (MapInfoConfig.Id == 102 || MapInfoConfig.Id == 112)//古战场特殊处理
            {
                if (mPlayer.Data.YuanbaoCoin >= 1)
                {
                    mGamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -1, "古战场进入扣费");
                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                    dBProxy.Save(mGamePlayer.Player.Data).Coroutine();
                    G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                    G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                    mBattleKVData.Key = (int)E_GameProperty.YuanbaoCoin;
                    mBattleKVData.Value = mGamePlayer.GetNumerial(E_GameProperty.YuanbaoCoin);
                    mChangeValue_notice.Info.Add(mBattleKVData);
                    mPlayer.Send(mChangeValue_notice);
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                    b_Reply(b_Response);
                    return false;
                }
            }

            if (MapInfoConfig.Id == 10)
            {
                if (mPlayer.GetCustomComponent<EquipmentComponent>().GetEquipItemByPosition(EquipPosition.Wing) == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2213);
                    b_Reply(b_Response);
                    return false;
                }
            }

            List<int> ints = new List<int>() { 103, 104, 105, 106, 107, 108, 109 };
            if (ints.Contains(MapInfoConfig.Id) || MapInfoConfig.IsCopyMap == 1)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(513);
                b_Reply(b_Response);
                return false;
            }
            if (MapInfoConfig.Id == 17)
            {
                var Title = mPlayer.GetCustomComponent<PlayerTitle>();
                if (Title.CheckTitle(60002) && Title.CheckTitle(60001))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2213);
                    b_Reply(b_Response);
                    return false;
                }
            }
            /*if (mGamePlayer.Data.GoldCoin < MapInfoConfig.)//Map_TransferPointConfigJson可以检查金币
            {
                // 金币不足
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(515);
                b_Reply(b_Response);
                return false;
            }*/

            /*if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mGamePlayer.UnitData.Index, out var mapComponent) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                b_Reply(b_Response);
                return false;
            }*/
            MapComponent mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, mPlayer.GameUserId);
            if (mapComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }
            
            if (mapComponent.TryGetPosX(mGamePlayer.UnitData.X) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                b_Reply(b_Response);
                return false;
            }
            var mMapCellSource = mapComponent.GetFindTheWay2D(mGamePlayer);
            if (mMapCellSource == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                b_Reply(b_Response);
                return false;
            }
            MapComponent mapComponent2 = Help_MapHelper.GetMapByMapId(mServerArea, FriendMapInfo.UnitData.Index, FriendMapInfo.Player.GameUserId);
            if (mapComponent2 == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }
            var City = mServerArea.GetCustomComponent<CitySiegeActivities>();
            if (City != null && City.GetSate())
            {
                if (4 == mapComponent2.MapId)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2710);
                    b_Reply(b_Response);
                    return false;
                }
            }
            /*if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(FriendMapInfo.UnitData.Index, out var mapComponent2) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(416);
                b_Reply(b_Response);
                return false;
            }*/

            if (mapComponent2.TryGetPosX(FriendMapInfo.UnitData.X) == false)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_DBProxyNotFoundError;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不可行走!X");
                b_Reply(b_Response);
                return false;
            }
            var mMapCellTarget = mapComponent2.GetFindTheWay2D(FriendMapInfo);
            if (mMapCellTarget == null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_DBProxyNotFoundError;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不可行走!Y");
                b_Reply(b_Response);
                return false;
            }
            //在副本时退出副本
            if (BatteCopyManagerComponent.BattleCopyMapIDList.Contains(mGamePlayer.UnitData.Index))
            {
                Log.PLog("RunCopyLog", "TeamDelivery");
                bool result = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().PlayerExitCopyHandler(mGamePlayer, mServerArea);
            }
            mGamePlayer.UnitData.Angle = FriendMapInfo.UnitData.Angle;
            // 公告移动信息
            mapComponent.MoveSendNotice(mMapCellSource, mMapCellTarget, mGamePlayer,false);

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(mPlayer.GameAreaId);
            mWriteDataComponent.Save(mGamePlayer.UnitData, dBProxy2).Coroutine();


            //无敌buf

            var b_BattleComponent = mapComponent.GetCustomComponent<BattleComponent>();
            Func<CombatSource.BattleSyncTimerTask> mCreateFunc = () =>
            {
                var mBattleSyncTimer = CustomFrameWork.Root.CreateBuilder.GetInstance<CombatSource.BattleSyncTimerTask>();
                mBattleSyncTimer.TaskType = CombatSource.E_SyncTimerTaskType.DeathRemoveTask;
                mBattleSyncTimer.CombatRoundId = IdGeneraterNew.Instance.GenerateId();
                mBattleSyncTimer.SyncWaitTime = 5000;
                mBattleSyncTimer.NextWaitTime = b_BattleComponent.CurrentTimeTick + mBattleSyncTimer.SyncWaitTime;

                mBattleSyncTimer.DisposeAction = (b_CombatSource, b_BattleComponent, b_TimerTask) =>
                {
                    if (b_CombatSource.IsDisposeable) return;

                    b_CombatSource.RemoveHealthState(E_BattleSkillStats.WuDi, b_BattleComponent);
                    b_CombatSource.UpdateHealthState();
                };
                mBattleSyncTimer.SyncAction = (b_CombatSource, b_BattleComponent, b_TimerTask, b_TimeTick) =>
                {
                    if (b_CombatSource.IsDisposeable) return CombatSource.E_SyncTimerTaskResult.Dispose;

                    if (b_CombatSource.HealthStatsDic.TryGetValue(E_BattleSkillStats.WuDi, out var hp_Curse) == false)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }

                    if (b_TimerTask.NextWaitTime == hp_Curse.ContinueTimeMax)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }
                    if (b_TimeTick > hp_Curse.ContinueTimeMax)
                    {
                        return CombatSource.E_SyncTimerTaskResult.Dispose;
                    }
                    else
                    {
                        b_TimerTask.NextWaitTime = hp_Curse.ContinueTimeMax;

                        b_CombatSource.AddTask(b_TimerTask);
                    }
                    return CombatSource.E_SyncTimerTaskResult.NextRound;
                };

                return mBattleSyncTimer;
            };

            // 移除进入场景用的无敌buff
            mGamePlayer.RemoveCustomComponent<BuffWuDiForEnterMap>();

            mGamePlayer.AddHealthState(E_BattleSkillStats.WuDi, 0, 5000, 0, mCreateFunc, b_BattleComponent);
            mGamePlayer.UpdateHealthState();

            b_Reply(b_Response);
            return true;

        }
    }
}