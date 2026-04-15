using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using System.Net;

namespace ETHotfix
{
    [Timer(TimerType.AncientBattlefieldCheck)]
    public class AncientBattlefieldCheckTimer : ATimer<AncientBattlefieldCheckComponent>
    {
        public override void Run(AncientBattlefieldCheckComponent self)
        {
            if (self.Parent.OnlineStatus != EOnlineStatus.Online) return;   // 玩家正在进入游戏 或 正在下线

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(self.Parent.SourceGameAreaId);
            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();
            int mapId = gamePlayer.UnitData.Index;

            if(mapId == ConstMapId.GuZhanChang || mapId == 112)
            {
                MapComponent map = Help_MapHelper.GetMapByMapId(mServerArea, gamePlayer.UnitData.Index, gamePlayer.InstanceId);
                if(map == null)
                {
                    Log.Error("GamePlayer 生命周期错乱");
                    // 将这个组件移除掉，防止继续报错
                    self.Parent.RemoveCustomComponent<AncientBattlefieldCheckComponent>();
                    return;
                }

                //古战场权限检查,最后1分钟检查
                var Exittime = gamePlayer.Data.GuZhanChangExittime;
                if (Exittime <= Help_TimeHelper.GetNowSecond() + 60)
                {
                    if (self.Parent.Data.YuanbaoCoin < 1)
                    {
                        var mMapCellSource = map.GetFindTheWay2D(gamePlayer.UnitData.X, gamePlayer.UnitData.Y);
                        var mMapCellTarget = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(ConstMapId.YongZheDaLu).GetFindTheWay2D(187, 200);

                        //map.QuitMap(mMapCellSource, gamePlayer);
                        // 公告移动信息
                        map.MoveSendNotice(mMapCellSource, mMapCellTarget, gamePlayer);

                        DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)self.Parent.GameAreaId);
                        var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(self.Parent.GameAreaId);
                        mWriteDataComponent.Save(gamePlayer.UnitData, dBProxy2).Coroutine();

                        // 直接移除这个组件，不用等待下次检查有没有在古战场地图
                        self.Parent.RemoveCustomComponent<AncientBattlefieldCheckComponent>();
                    }
                    else
                    {
                        gamePlayer.Data.GuZhanChangExittime = Help_TimeHelper.GetNowSecond() + 3600;
                        gamePlayer.UpdateCoin(E_GameProperty.YuanbaoCoin,-1,"古战场续费");
                        DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                        DBProxyComponent dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, self.Parent.GameAreaId);
                        dBProxy.Save(gamePlayer.Player.Data).Coroutine();
                        G2C_ChangeValue_notice mChangeValue_notice = new G2C_ChangeValue_notice();
                        G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                        mBattleKVData.Key = (int)E_GameProperty.YuanbaoCoin;
                        mBattleKVData.Value = gamePlayer.GetNumerial(E_GameProperty.YuanbaoCoin);
                        mChangeValue_notice.Info.Add(mBattleKVData);
                        self.Parent.Send(mChangeValue_notice);
                    }
                }
            }
            else
            {
                // 玩家离开 古战场(102)
                self.Parent.RemoveCustomComponent<AncientBattlefieldCheckComponent>();
            }
        }
    }

    [EventMethod(typeof(AncientBattlefieldCheckComponent), EventSystemType.INIT)]
    public class AncientBattlefieldCheckComponentEventOnInit : ITEventMethodOnInit<AncientBattlefieldCheckComponent>
    {
        public void OnInit(AncientBattlefieldCheckComponent b_Component)
        {
            // 10秒循环检查，如果不需要那么准确。间隔可以改长
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000 * 10, TimerType.AncientBattlefieldCheck, b_Component);
        }
    }

    [EventMethod(typeof(AncientBattlefieldCheckComponent), EventSystemType.DISPOSE)]
    public class AncientBattlefieldCheckComponentEventOnDispose : ITEventMethodOnDispose<AncientBattlefieldCheckComponent>
    {
        public override void OnDispose(AncientBattlefieldCheckComponent b_Component)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
        }
    }


    [EventMethod("CombatSourceEnterOrSwitchMap")]
    public class CombatSourceEnterOrSwitchMap_AddCheckComponentHelper : ITEventMethodOnRun<ETModel.EventType.CombatSourceEnterOrSwitchMap>
    {
        public void OnRun(ETModel.EventType.CombatSourceEnterOrSwitchMap args)
        {
            if (args.combatSource.Identity != E_Identity.Hero) return;
            if (args.newMap.MapId != ConstMapId.GuZhanChang && args.newMap.MapId != 112) return;

            // 进入古战场
            // 定时检查有没有魔晶
            ((GamePlayer)args.combatSource).Data.GuZhanChangExittime = Help_TimeHelper.GetNowSecond() + 3600;
            ((GamePlayer)args.combatSource).Player.AddCustomComponent<AncientBattlefieldCheckComponent>();
        }
    }
}
