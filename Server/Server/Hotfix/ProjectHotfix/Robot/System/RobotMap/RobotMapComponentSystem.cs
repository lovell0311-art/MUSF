using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using ETModel;
using ETModel.Robot;
using CustomFrameWork;
using CustomFrameWork.Component;
using static ETModel.Robot.RobotMapComponent;

namespace ETHotfix.Robot
{
    [ObjectSystem]
    public class RobotMapComponentDestroySystem : DestroySystem<RobotMapComponent>
    {
        public override void Destroy(RobotMapComponent self)
        {
            if(self.CurrentMap != null)
            {
                self.CurrentMap.Dispose();
                self.CurrentMap = null;
            }
        }
    }

    public static partial class RobotMapComponentSystem
    {
        public static async Task<Map> Load(this RobotMapComponent self,int mapId)
        {
            ReadConfigComponent mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if(!mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(mapId,out Map_InfoConfig mapConfig))
            {
                return null;
            }


            Map map = ComponentFactory.Create<Map>();
            map.MapId = mapId;
            map.Astar = await ThreadPoolComponent.Instance.WaitTaskAsync(() =>
            {
                string configStr = File.ReadAllText($"../{mapConfig.TerrainPath}");
                MapInfo mapInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<MapInfo>(configStr);

                AstarFindPath astar = new AstarFindPath();
                astar.Init(mapInfo.width, mapInfo.height);
                byte[,] maze = new byte[mapInfo.width, mapInfo.height];
                int[] sceneInfos = mapInfo.SceneInfos;
                for (int i = 0, length = sceneInfos.Length; i < length; ++i)
                {
                    maze[i % mapInfo.width, i / mapInfo.width] = (byte)sceneInfos[i];
                }
                astar.Maze = maze;
                return astar;
            });
            map.AddComponent<MapNpcInfoComponent>().AllNpcSpawnPoint = await ThreadPoolComponent.Instance.WaitTaskAsync(() =>
            {
                string configStr = File.ReadAllText($"../{mapConfig.NpcPath}");
                List<NpcInfo> spawnPoint = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NpcInfo>>(configStr);
                return spawnPoint;
            });

            map._SafeArea = await ThreadPoolComponent.Instance.WaitTaskAsync(() =>
            {

                byte[,] ret = new byte[map.Astar.Wight, map.Astar.Height];
                string configStr = File.ReadAllText($"../{mapConfig.SafeAreaPath}");
                List<NpcInfo> pointList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<NpcInfo>>(configStr);
                foreach(NpcInfo point in pointList)
                {
                    if(point.PositionX >= map.Astar.Wight ||
                    point.PositionY >= map.Astar.Height ||
                    point.PositionX < 0 ||
                    point.PositionY < 0)
                    {
                        continue;
                    }
                    ret[point.PositionX, point.PositionY] = 1;
                }
                return ret;
            });

            self.CurrentMap = map;
            return map;
        }



    }
}
