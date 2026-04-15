using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using ETModel.Robot;
using System.Collections.Generic;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    [Timer(TimerType.TrialTowerTime)]
    public class TrialTowerTimeCheckTimer : ATimer<TrialTowerCheckComponent>
    {
        public override void Run(TrialTowerCheckComponent self)
        {
            if (self.Parent.Player.OnlineStatus != EOnlineStatus.Online) return;   // 玩家正在进入游戏 或 正在下线

            var gamePlayer = self.Parent;
            var mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(self.Parent.Player.SourceGameAreaId);
            BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
            
            if (gamePlayer.IsDeath)
            {
                var mTransferPointId = 400;
                var mJsonMapDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;
                if (mJsonMapDic.TryGetValue(mTransferPointId, out var _TransferPointConfig) != false)
                {
                    if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(_TransferPointConfig.MapId, out var mTargetMapComponent) != false)
                    {
                        if (mTargetMapComponent.TransferPointFindTheWayDic.TryGetValue(mTransferPointId, out var mTransferPointlist) != false)
                        {
                            if (mTransferPointlist != null || mTransferPointlist.Count > 0)
                            {
                                var mPlayerUnitData = gamePlayer.UnitData;
                                if (mPlayerUnitData != null)
                                {
                                    var mRandomIndex = Help_RandomHelper.Range(0, mTransferPointlist.Count);
                                    var mTransferPoint = mTransferPointlist[mRandomIndex];

                                    DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                                    DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, gamePlayer.Player.GameAreaId);
                                    // 切换地图
                                    mTargetMapComponent.Switch(gamePlayer, mTransferPoint.X, mTransferPoint.Y);
                                    dBProxy.Save(mPlayerUnitData).Coroutine();

                                    RebirthHelper.Rebirth(gamePlayer,
                                                          gamePlayer.CurrentMap,
                                                          gamePlayer.UnitData.X,
                                                          gamePlayer.UnitData.Y,
                                                          "试炼塔死亡复活"
                                                          , reEnterMap: false);
                                }
                            }
                        }
                    }
                }

                if (batteCopyManagerCpt.TrialTowerList.TryGetValue(gamePlayer.Player.GameUserId, out var mapComponent))
                {
                    mapComponent.Dispose();
                    batteCopyManagerCpt.TrialTowerList.Remove(gamePlayer.Player.GameUserId);
                }
                gamePlayer.RemoveCustomComponent<TrialTowerCheckComponent>();
                return;
            }
            if (gamePlayer.CurrentMap != null && gamePlayer.CurrentMap.MapId == 111)
            {
                var Enemy = gamePlayer.CurrentMap.GetCustomComponent<EnemyComponent>();
                if (Enemy != null && Enemy.AllEnemyDic != null)
                {
                    if (Enemy.AllEnemyDic.Count <= 0 && gamePlayer.ISNo == false)
                    {
                        //通知奖励并告知可以进入下一层
                        C2G_NotificationReward c2G_NotificationReward = new C2G_NotificationReward();
                        c2G_NotificationReward.Cnt = gamePlayer.CopyCount;
                        gamePlayer.Player.Send(c2G_NotificationReward);
                        gamePlayer.ISNo = true;
                    }
                }
            }
        }
    }

    [EventMethod(typeof(TrialTowerCheckComponent), EventSystemType.INIT)]
    public class TrialTowerCheckComponentEventOnInit : ITEventMethodOnInit<TrialTowerCheckComponent>
    {
        public void OnInit(TrialTowerCheckComponent b_Component)
        {
            // 10秒循环检查，如果不需要那么准确。间隔可以改长
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000, TimerType.TrialTowerTime, b_Component);
        }
    }

    [EventMethod(typeof(TrialTowerCheckComponent), EventSystemType.DISPOSE)]
    public class TrialTowerCheckComponentEventOnDispose : ITEventMethodOnDispose<TrialTowerCheckComponent>
    {
        public override void OnDispose(TrialTowerCheckComponent b_Component)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
        }
    }
}
