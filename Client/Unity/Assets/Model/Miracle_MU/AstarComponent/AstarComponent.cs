using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LitJson;
using System.Linq;
using System;
using UnityEngine.UIElements;

namespace ETModel
{
    public class IInt
    {
        public int num;
    }
    [ObjectSystem]
    public class AstarComponentAwake : AwakeSystem<AstarComponent>
    {
        public override void Awake(AstarComponent self)
        {
            self.Awake();
        }
    }
    [ObjectSystem]
    public class AstarComponentUpdate : UpdateSystem<AstarComponent>
    {
        public override void Update(AstarComponent self)
        {
            self.Update();
        }
    }
    public class AstarComponent : Component
    {
        public static AstarComponent Instance { get; private set; }
        public AstarNode[][] Nodes { get { return nodes; } set { nodes = value; } }
        public int Width { get { return this.width; } }
        public int Height { get { return this.height; } }

        private AstarNode[][] nodes;
        private int width;
        private int height;
        public string curScenenodesName;//当前场景名字
        public AstarSceneData CurSceneAstarData;//当前场景的格子

        public  Dictionary<string, AstarSceneData> sceneNodes = new Dictionary<string, AstarSceneData>();

        private readonly Queue<AstarPathfinder> findQueue = new Queue<AstarPathfinder>();
        private readonly List<AstarPathfinder> findingList = new List<AstarPathfinder>();

        private const string ABNAME = "navgriddata";
        private const string ASTAR_PREFAB_NAME = "NavGridData";
        private const float GRIDSIZE = 2;
        //场景的格子 字典 AstarNodeJosnData
        private readonly Dictionary<string, AstarNodeJosnData> SceneGridDic = new Dictionary<string, AstarNodeJosnData>();

        public Action<int, long> AreaAdd;//区域添加 实体 <区域ID,实体ID>
        public Action<int, long> AreaRemove;//区域移除 实体
        public Action<int> AreaClear;//清除区域内的实体 <区域id>

        public Func<long> GetLocaRoleUUID;

        public void Awake()
        {
            Instance = this;
            AssetBundleComponent.Instance.LoadBundle(ABNAME.StringToAB());
            GameObject data = AssetBundleComponent.Instance.GetAsset(ABNAME.StringToAB(), ASTAR_PREFAB_NAME) as GameObject;
            if (data == null)
            {
                // Older bundles may still expose the prefab under the lowercase bundle name.
                data = AssetBundleComponent.Instance.GetAsset(ABNAME.StringToAB(), ABNAME) as GameObject;
            }

            if (data == null)
            {
                Log.Error($"AstarComponent.Awake failed: prefab not found bundle={ABNAME.StringToAB()} expected={ASTAR_PREFAB_NAME}");
                return;
            }

            ReferenceCollector monoReference = data.GetReferenceCollector();
            if (monoReference == null)
            {
                Log.Error($"AstarComponent.Awake failed: ReferenceCollector missing on {data.name}");
                return;
            }
            var total = monoReference.GetKVAssets(MonoReferenceType.TextAsset);
            size.num = total.Length;
            for (int i = 0; i < total.Length; i++)
            {
                var item = total[i];
                string str = (item.Value as TextAsset).text;
                AstarNodeJosnData josnData = JsonMapper.ToObject<AstarNodeJosnData>(str);
                SceneGridDic[item.Key] = josnData;
             //   SceneGridDic[item.Key] = str;
               // StartTask(item.Key, str, i);
            }
            AssetBundleComponent.Instance.UnloadBundle(ABNAME.StringToAB());
        }
        public static IInt size = new IInt();
        bool startTask = false;

      
        //idx为0，预热函数
        async void StartTask(string key, string str, int idx)
        {
            while (idx != 0 && !startTask)
            {
                await Task.Delay(1);
            }
            Task t = Task.Run(() =>
            {
                startTask = true;
              //  AstarSceneData sceneData = LoadNavGridData(key, str);
                AstarSceneData sceneData = LoadNavGridData(key);
                lock (this.sceneNodes)
                {
                    this.sceneNodes.Add(key, sceneData);
                }
                lock (size)
                {
                    size.num--;
                }
            });
        }
        public void Update()
        {
            int i = 0;
            while (i < findingList.Count)
            {
                if (findingList[i].IsDone)
                {
                    findingList[i].NotifyComplete();
                    findingList[i].Dispose();
                    findingList.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }

            //只查找一个 finder
            if (findingList.Count > 0)
            {
                return;
            }

            if (findQueue.Count > 0)
            {
                AstarPathfinder job = findQueue.Dequeue();
                
                job.FindPah();
                this.findingList.Add(job);
                /*   this.findingList.Add(job);
                   Thread jobThread = new Thread(job.FindPah);
                   jobThread.Start();*/

            }

        }

     

        /// <summary>
        /// 加载地图的格子数据
        /// </summary>
        /// <param name="sceneName"></param>
        public void LoadSceneNodes(string sceneName,string lastSceneName)
        {
          
            if (curScenenodesName == sceneName)
            {
              // Log.DebugBrown($"当前 {curScenenodesName}  场景的格子已经加载 this.width：{this.width}  this.height：{this.height}  this.nodes：{this.nodes.Length} ");
                return;
            }
            if (sceneNodes.ContainsKey(lastSceneName))
            {
            
                sceneNodes.Remove(lastSceneName);
            }
        
            if (this.sceneNodes.TryGetValue(sceneName, out AstarSceneData sceneData))
            {
             
                this.width = sceneData.Width;
                this.height = sceneData.Hieght;
                this.Nodes = sceneData.Nodes;
                this.curScenenodesName = sceneName;
                this.CurSceneAstarData = sceneData;
                //Log.DebugRed($"{sceneName} -》导航数据加载完成");
            }
            else
            {
                 sceneData = LoadNavGridData(sceneName);
                this.width = sceneData.Width;
                this.height = sceneData.Hieght;
                this.Nodes = sceneData.Nodes;
                this.curScenenodesName = sceneName;
                this.CurSceneAstarData = sceneData;
                    sceneNodes[sceneName] = sceneData;
            }
        }




        private AstarSceneData LoadNavGridData(string key)
        {
            try
            {
                // AstarNodeJosnData data = JsonMapper.ToObject<AstarNodeJosnData>(text);
                
                AstarNodeJosnData data = SceneGridDic[key];
                AstarSceneData sceneData = new AstarSceneData();
                
                    sceneData.SceneName = key;
                    sceneData.Width = data.Width;
                    sceneData.Hieght = data.Height;
                    sceneData.Nodes = new AstarNode[data.Width][];
                    sceneData.Id = GetMapID(key);
               
                for (int i = 0; i < sceneData.Nodes.Length; i++)
                {
                    sceneData.Nodes[i] = new AstarNode[data.Height];
                }

                /*int mAreaHeight = 12;
                int mAreaWidth = 12;
                int mRowUnitCount = (data.Width / mAreaWidth + 1);*/

                int index = 0;
                //列
                for (int y = 0; y < data.Height; y++)
                {
                    //行
                    for (int x = 0; x < data.Width; x++)
                    {
                        int type = data.SceneInfos[index];
                        var mNodes = sceneData.Nodes[x][y] = new AstarNode() { x = x, z = y, isWalkable = type == 0, Map = sceneData };

                        /*int mAreaPosX = (mNodes.x / mAreaWidth);
                        int mAreaPosY = (mNodes.z / mAreaHeight);
                        int areaIndex = mAreaPosX + mAreaPosY * mRowUnitCount;
                        mNodes.AreaIndex = areaIndex;
                        mNodes.AreaPosX = mAreaPosX;
                        mNodes.AreaPosY = mAreaPosY;

                        if (sceneData.MapCellFieldDic.TryGetValue(mAreaPosX, out var mapCellFieldDic) == false)
                        {
                            mapCellFieldDic = sceneData.MapCellFieldDic[mAreaPosX] = new Dictionary<int, MapCellFieldComponent>();
                        }
                        if (mapCellFieldDic.TryGetValue(mAreaPosY, out var mCurrentCellField) == false)
                        {
                            mCurrentCellField = mapCellFieldDic[mAreaPosY] = new MapCellFieldComponent();

                            mCurrentCellField.AreaPosX = mAreaPosX;
                            mCurrentCellField.AreaPosY = mAreaPosY;
                            mCurrentCellField.AreaIndex = areaIndex;
                            mCurrentCellField.AroundFieldDic = new Dictionary<int, MapCellFieldComponent>();
                            mCurrentCellField.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                        }
                        if (sceneData.MapCellFieldDic.TryGetValue(mCurrentCellField.AreaPosX - 1, out var mapCellAreaDicLeft))
                        {
                            if (mapCellAreaDicLeft.TryGetValue(mCurrentCellField.AreaPosY - 1, out var mLeftUpNode))
                            {
                                mCurrentCellField.AroundFieldDic[mLeftUpNode.AreaIndex] = mLeftUpNode;
                                mLeftUpNode.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                            }
                            if (mapCellAreaDicLeft.TryGetValue(mCurrentCellField.AreaPosY, out var mLeftMiddleNode))
                            {
                                mCurrentCellField.AroundFieldDic[mLeftMiddleNode.AreaIndex] = mLeftMiddleNode;
                                mLeftMiddleNode.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                            }
                        }
                        if (sceneData.MapCellFieldDic.TryGetValue(mCurrentCellField.AreaPosX, out var mapCellAreaDicCenter))
                        {
                            if (mapCellAreaDicCenter.TryGetValue(mCurrentCellField.AreaPosY - 1, out var mUpMiddleNode))
                            {
                                mCurrentCellField.AroundFieldDic[mUpMiddleNode.AreaIndex] = mUpMiddleNode;

                                mUpMiddleNode.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                            }
                        }
                        if (sceneData.MapCellFieldDic.TryGetValue(mCurrentCellField.AreaPosX + 1, out var mapCellAreaDicRight))
                        {
                            if (mapCellAreaDicRight.TryGetValue(mCurrentCellField.AreaPosY - 1, out var mRightUpNode))
                            {
                                mCurrentCellField.AroundFieldDic[mRightUpNode.AreaIndex] = mRightUpNode;

                                mRightUpNode.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                            }
                        }*/

                        index++;
                    }
                }
                /*var mRowFieldDiclist = sceneData.MapCellFieldDic.Values.ToArray();
                for (int i = 0, len = mRowFieldDiclist.Length; i < len; i++)
                {
                    var mRowFieldDic = mRowFieldDiclist[i];

                    var mFieldDiclist = mRowFieldDic.Values.ToArray();
                    for (int j = 0, jlen = mFieldDiclist.Length; j < jlen; j++)
                    {
                        var mCurrentCellField = mFieldDiclist[j];
                        mCurrentCellField.AroundField = mCurrentCellField.AroundFieldDic.Keys.ToList();
                        mCurrentCellField.AroundFieldArray = mCurrentCellField.AroundFieldDic.Values.ToArray();
                    }
                }*/
                return sceneData;
            }
            catch (System.Exception e)
            {
                Log.Error(e.ToString());
            }
            return null;

            int GetMapID(string mapname) 
            {
               /* IConfig[] allinfo= ConfigComponent.Instance.GetAll(typeof(Map_InfoConfig));
                foreach (Map_InfoConfig info in allinfo.Cast<Map_InfoConfig>())
                {
                    if (info.TerrainPath.Contains(mapname))
                    {
                        return (int)info.Id;
                    }
                }*/
                return 0;
            }
        }

        public AstarNode GetNode(int x, int y)
        {
            AstarNode node = null;
            if (x < this.Width && x >= 0 &&
                y >= 0 && y < this.Height)
            {
                node = Nodes[x][y];
            }

            return node;
        }
        /// <summary>
        /// 获取下一个格子
        /// </summary>
        /// <param name="curnode">当前所在格子</param>
        /// <param name="dir">方向</param>
        /// <returns></returns>
        public AstarNode GetNextNode(AstarNode curnode, Vector2 dir)
        {
            if (curnode == null || this.Nodes == null || this.Width <= 0 || this.Height <= 0)
            {
                return null;
            }
            int x = (int)(curnode.x + dir.x);
            int y = (int)(curnode.z + dir.y);
            return GetNode(x, y);
        }

        public AstarNode GetNodeVector(float x, float z)
        {
            int gridX = Mathf.CeilToInt(x / GRIDSIZE);
            int gridY = Mathf.CeilToInt(z / GRIDSIZE);
            return GetNode(gridX, gridY);
        }
        public AstarNode GetNodeVector(Vector3 vector)
        {
            int gridX = Mathf.CeilToInt(vector.x / GRIDSIZE);
            int gridY = Mathf.CeilToInt(vector.z / GRIDSIZE);
            return GetNode(gridX, gridY);
        }

        public AstarNode GetNearestWalkableNode(float x, float z, int searchRadius = 12)
        {
            int gridX = Mathf.CeilToInt(x / GRIDSIZE);
            int gridY = Mathf.CeilToInt(z / GRIDSIZE);
            return GetNearestWalkableNode(gridX, gridY, searchRadius);
        }

        public AstarNode GetNearestWalkableNode(int x, int y, int searchRadius = 12)
        {
            if (this.Nodes == null || this.Width <= 0 || this.Height <= 0)
            {
                return null;
            }

            int centerX = Mathf.Clamp(x, 0, this.Width - 1);
            int centerY = Mathf.Clamp(y, 0, this.Height - 1);

            AstarNode centerNode = GetNode(centerX, centerY);
            if (centerNode != null && centerNode.isWalkable)
            {
                return centerNode;
            }

            for (int radius = 1; radius <= searchRadius; ++radius)
            {
                AstarNode nearestNode = null;
                int nearestDistance = int.MaxValue;

                for (int dx = -radius; dx <= radius; ++dx)
                {
                    for (int dy = -radius; dy <= radius; ++dy)
                    {
                        if (Mathf.Abs(dx) != radius && Mathf.Abs(dy) != radius)
                        {
                            continue;
                        }

                        AstarNode candidate = GetNode(centerX + dx, centerY + dy);
                        if (candidate == null || candidate.isWalkable == false)
                        {
                            continue;
                        }

                        int distance = dx * dx + dy * dy;
                        if (distance < nearestDistance)
                        {
                            nearestDistance = distance;
                            nearestNode = candidate;
                        }
                    }
                }

                if (nearestNode != null)
                {
                    return nearestNode;
                }
            }

            return null;
        }
        /// <summary>
        /// 是否可行走
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool IsWalkable(float x, float z)
        {
            AstarNode astarNode = GetNodeVector(x, z);
            if (astarNode == null) return false;

            return astarNode.isWalkable;
        }
        /// <summary>
        /// 修改可行走区域
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="isWask"></param>
        public void ChangeAstarNodeState(int x, int z, bool isWask)
        {
            AstarNode astarNode = GetNode(x, z);
            
            if (astarNode == null) return;
            astarNode.isWalkable = isWask;
        }
        /// <summary>
        /// 格子坐标转成 三维坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector3Int GetVectory3(int x, int y)
        {

            return new Vector3Int((int)(x * GRIDSIZE), 0, (int)(y * GRIDSIZE));
        }
        public Vector3Int GetVectory3(AstarNode node)
        {

            return new Vector3Int((int)(node.x * GRIDSIZE), 0, (int)(node.z * GRIDSIZE));
        }
        public Vector3Int GetNextVectory3(int x, int y)
        {
            return new Vector3Int((int)((x * GRIDSIZE) + (GRIDSIZE / 2)), 0, (int)((y * GRIDSIZE) + (GRIDSIZE / 2)));
        }
        /// <summary>
        /// 格子坐标 转为二维坐标
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Vector2 GetVectory2(int x, int y)
        {
            Vector2 vector = new Vector2(x * GRIDSIZE, y * GRIDSIZE);
            return vector;
        }
        /// <summary>
        /// 查找路劲
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">目标点</param>
        /// <param name="callback">回调函数</param>
        public void FindPath(Vector3 start, Vector3 end, AstarFindCallback callback)
        {
            AstarPathfinder finder = ComponentFactory.Create<AstarPathfinder, AstarNode, AstarNode, AstarFindCallback>(GetNodeVector(start.x, start.z), GetNodeVector(end.x, end.z), callback);
            this.findQueue.Clear();
            this.findQueue.Enqueue(finder);
        }
        /// <summary>
        /// 查找路径点
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="callback"></param>
        public void FindPath(AstarNode start, AstarNode end, AstarFindCallback callback)
        {
            AstarPathfinder finder = ComponentFactory.Create<AstarPathfinder, AstarNode, AstarNode, AstarFindCallback>(start, end, callback);
            this.findQueue.Clear();
            this.findQueue.Enqueue(finder);
        }

        /// <summary>
        /// 两点之间是否有障碍物
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>true 有障碍物</returns>
        public  bool IsHaveObstacle(AstarNode from, AstarNode to)
        {
            var gridList=GridHelp.GetTouchedPosBetweenTwoPoint(from,to);
            for (int i = 0, length=gridList.Count; i < length; i++)
            {
                var grid = gridList[i];
                if (GetNode(grid.x, grid.y).isWalkable == false)
                {
                    //有障碍物
                    return true;
                }
            }
            return false;
        }
    }
}
