using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using System.Linq;

namespace ETHotfix
{
    [FriendOf(typeof(CheckWingComponent))]
    [Timer(TimerType.CheckWing)]
    public class CheckWingTimer : ATimer<CheckWingComponent>
    {
        public override void Run(CheckWingComponent self)
        {
            GamePlayer gamePlayer = self.Parent;
            if(gamePlayer.CurrentMap == null || !ConstMapId.GetListValue().Contains(gamePlayer.CurrentMap.MapId)) 
            {
                gamePlayer.RemoveCustomComponent<CheckWingComponent>();
                return;
            }
            void Exit()
            {

                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(gamePlayer.Player.SourceGameAreaId);
                Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<CreateRole_InfoConfigJson>().JsonDic.TryGetValue(gamePlayer.Data.PlayerTypeId, out var mJson);
                MapComponent map = mServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(mJson.InitMap);
                var mSafeAreaValues = map.SpawnSafeFindTheWayDic.Values.ToArray();
                if (mSafeAreaValues.Length > 0)
                {
                    int mRandomIndex = Help_RandomHelper.Range(0, mSafeAreaValues.Length);
                    var mRandemValueDic = mSafeAreaValues[mRandomIndex];

                    var mSafeAreaDicKeys = mRandemValueDic.Keys.ToArray();
                    int mRandomKeyIndex = Help_RandomHelper.Range(0, mSafeAreaDicKeys.Length);
                    var mRandemKeyValue = mSafeAreaDicKeys[mRandomKeyIndex];
                    var mRandemValue = mRandemValueDic[mRandemKeyValue];

                    var mFindTheWay2D = mRandemValue;

                    if (mFindTheWay2D.Map.MapId != mJson.InitMap)
                    {
                        gamePlayer.Player.PLog($"获取出生地图异常 {mFindTheWay2D.Map.MapId}:{mJson.InitMap}");
                    }
                    gamePlayer.SwitchMap(map, mFindTheWay2D.X, mFindTheWay2D.Y);
                }
            }
            if (self.equipmentCom.GetEquipItemByPosition(EquipPosition.Wing) == null && ConstMapId.TianKongZhiCheng == gamePlayer.CurrentMap.MapId)
            {
                Exit();
            }
            if (ConstMapId.GetListValue().Contains(gamePlayer.CurrentMap.MapId) && self.equipmentCom.GetEquipItemByPosition(EquipPosition.Guard) == null)
            {
                Exit();
            }
        }
    }

    [FriendOf(typeof(CheckWingComponent))]
    [EventMethod(typeof(CheckWingComponent), EventSystemType.INIT)]
    public class CheckWingComponentEventOnInit : ITEventMethodOnInit<CheckWingComponent>
    {
        public void OnInit(CheckWingComponent self)
        {
            GamePlayer gamePlayer = self.Parent;
            self.equipmentCom = gamePlayer.Player.GetCustomComponent<EquipmentComponent>();
            self.timerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000 * 10, TimerType.CheckWing, self);
        }
    }

    [FriendOf(typeof(CheckWingComponent))]
    [EventMethod(typeof(CheckWingComponent), EventSystemType.DISPOSE)]
    public class CheckWingComponentEventOnDispose : ITEventMethodOnDispose<CheckWingComponent>
    {
        public override void OnDispose(CheckWingComponent self)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref self.timerId);
            self.equipmentCom = null;
        }
    }

    [EventMethod("CombatSourceEnterOrSwitchMap")]
    public class CombatSourceEnterOrSwitchMap_AddCheckWingComponentHelper : ITEventMethodOnRun<ETModel.EventType.CombatSourceEnterOrSwitchMap>
    {
        public void OnRun(ETModel.EventType.CombatSourceEnterOrSwitchMap args)
        {
            if (args.combatSource.Identity != E_Identity.Hero) return;
            if (!ConstMapId.GetListValue().Contains(args.newMap.MapId)) return;

            // 进入天空之城
            // 定时检查有没有翅膀
            ((GamePlayer)args.combatSource).AddCustomComponent<CheckWingComponent>();
        }
    }

    [EventMethod("CombatSourceLeaveMap")]
    public class CombatSourceLeaveMap_AddCheckWingComponentHelper : ITEventMethodOnRun<ETModel.EventType.CombatSourceLeaveMap>
    {
        public void OnRun(ETModel.EventType.CombatSourceLeaveMap args)
        {
            if (args.combatSource.Identity != E_Identity.Hero) return;
            if (!ConstMapId.GetListValue().Contains(args.leavedMap.MapId)) return;

            // 离开天空之城
            // 移除翅膀检查组件
            ((GamePlayer)args.combatSource).RemoveCustomComponent<CheckWingComponent>();
        }
    }

    [FriendOf(typeof(CheckWingComponent))]
    public static class CheckWingComponentSystem
    {

    }
}
