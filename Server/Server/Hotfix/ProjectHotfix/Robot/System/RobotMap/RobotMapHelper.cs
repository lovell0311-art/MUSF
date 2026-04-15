using ETModel;
using ETModel.Robot;
using ETModel.Robot.EventType;
using System.Threading.Tasks;
using CustomFrameWork;
using UnityEngine;

namespace ETHotfix.Robot
{
    public static class RobotMapHelper
    {
        public static async Task MapChangeTo(Scene clientScene,int mapId,int posX,int posY)
        {
            Log.Debug($"[{clientScene.Name}] 开始切换地图,MapId={mapId}");
            int oldMapId = 0;
            //clientScene.RemoveComponent<AIComponent>();
            RobotMapComponent robotMap = clientScene.GetComponent<RobotMapComponent>();
            Unit localUnit = clientScene.GetComponent<PlayerComponent>().LocalUnit;
            if (localUnit.CurrentMap != null)
            {
                // 已经进入地图了
                oldMapId = robotMap.CurrentMap.MapId;
                robotMap.CurrentMap.UnitLeave(localUnit);
            }

            robotMap.CurrentMap?.Dispose();
            Map currentMap = await robotMap.Load(mapId);
            currentMap.AddComponent<ClientSceneComponent, Scene>(clientScene);
            currentMap.UnitEnter(localUnit,new Vector2Int(posX, posY));


            clientScene.GetComponent<ObjectWait>().Notify(new ETModel.Robot.WaitType.Wait_MapChangeFinish());
            Log.Debug($"[{clientScene.Name}] 切换地图完成,MapId={mapId}");


            MapChangeFinish args = MapChangeFinish.Instance;
            args.ClientScene = clientScene;
            args.OldMapId = oldMapId;
            args.NewMapId = robotMap.CurrentMap.MapId;
            Game.EventSystem.Run("MapChangeFinish", args);
        }
    }
}
