using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using static ETModel.MapManageComponent;

namespace ETHotfix
{
    [EventMethod(typeof(MapManageComponent), EventSystemType.INIT)]
    public class MapManageComponentEventOnInit : ITEventMethodOnInit<MapManageComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(MapManageComponent b_Component)
        {
            b_Component.OnInit();
        }
    }

    /// <summary>
    /// 全部的地图
    /// </summary>
    public static partial class MapManageComponentSystem
    {
        public static void OnInit(this MapManageComponent b_Component)
        {
            ReadConfigComponent mReadConfigComponent = CustomFrameWork.Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mMapConfigs = mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.Values.ToArray();
            //for (int i = 0, len = 1; i < len; i++)
            for (int i = 0, len = mMapConfigs.Length; i < len; i++)
            {
                var mMapConfig = mMapConfigs[i];

                try
                {
                    string configStr = File.ReadAllText($"../{mMapConfig.TerrainPath}");
                    MapInfo mMapInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<MapInfo>(configStr);

                    string configStr2 = File.ReadAllText($"../{mMapConfig.SafeAreaPath}");
                    List<MapSafeAreaInfo> mMapSafeAreaInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapSafeAreaInfo>>(configStr2);
                    
                    string path = $"../{mMapConfig.SpawnPath}";
                    string configStr5 = "[]";
                    if (File.Exists(path)) configStr5 = File.ReadAllText(path);
                    List<MapSafeAreaInfo> mMapSpawnAreaInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapSafeAreaInfo>>(configStr5);

                    string configStr7 = File.ReadAllText($"../{mMapConfig.TransferPoint}");
                    List<MapDeliveryAreaInfo> mMapTransferPointAreaInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapDeliveryAreaInfo>>(configStr7);

                    string configStr4 = File.ReadAllText($"../{mMapConfig.NpcPath}");
                    List<MapSafeAreaInfo> mNpcPathData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapSafeAreaInfo>>(configStr4);

                    if (mMapConfig.IsCopyMap == 0)
                    {
                        MapComponent mMapComponent = b_Component.keyValuePairs[mMapConfig.Id] = Root.CreateBuilder.GetInstance<MapComponent>(false);
                        mMapComponent.Awake(b_Component);
                        mMapComponent.MapId = mMapConfig.Id;

                        mMapComponent.OnInit();
                        mMapComponent.InitMap(mMapInfo);
                        mMapComponent.InitMapSafeArea(mMapSafeAreaInfo);
                        mMapComponent.InitMapSpawnSafeArea(mMapSpawnAreaInfo);
                        mMapComponent.InitMapTransferPoint(mMapTransferPointAreaInfo);

                        if (mMapConfig.MonsterPathDic.TryGetValue(0, out var mMonsterPath))
                        {
                            string configStr3 = File.ReadAllText($"../{mMonsterPath}");
                            List<MapSafeAreaInfo> mEnemyPathData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapSafeAreaInfo>>(configStr3);
                            var mEnemyComponent = mMapComponent.AddCustomComponent<EnemyComponent>();
                            mEnemyComponent.InitMapEnemy(mEnemyPathData);
                        }
                        if (mMapConfig.MonsterPathDic.TryGetValue(100, out var mMonsterPathBoss))
                        {
                            if (b_Component.Parent.GameAreaRouteId == 1)
                            {
                                string configStr3 = File.ReadAllText($"../{mMonsterPathBoss}");
                                List<MapSafeAreaInfo> mEnemyPathData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapSafeAreaInfo>>(configStr3);
                                var mEnemyComponent = mMapComponent.AddCustomComponent<EnemyComponent>();
                                mEnemyComponent.InitMapBossEnemy(mEnemyPathData);
                            }
                        }
                        if (mMapConfig.MonsterPathDic.TryGetValue(200, out var mMonsterPathHuangJin))
                        {
                           
                            {
                                string configStr3 = File.ReadAllText($"../{mMonsterPathHuangJin}");
                                List<MapSafeAreaInfo> mEnemyPathData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapSafeAreaInfo>>(configStr3);
                                var mEnemyComponent = mMapComponent.AddCustomComponent<EnemyComponent>();
                                mEnemyComponent.InitMapBossEnemy(mEnemyPathData);

                                if (MapEnemySpawnPathCacheDic.TryGetValue(mMapConfig.Id, out var mTempDic) == false)
                                {
                                    mTempDic = MapEnemySpawnPathCacheDic[mMapConfig.Id] = new Dictionary<int, List<MapSafeAreaInfo>>();
                                }
                                mTempDic[200] = mEnemyPathData;
                            }
                        }
                        var ServerInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>();
                        var info = ServerInfo?.GetStartUpInfo(OptionComponent.Options.AppType, OptionComponent.Options.AppId);
                        if (info.IsVIP == 1)
                        {
                            if (mMapConfig.MonsterPathDic.TryGetValue(300, out var mVipMonsterBoss))
                            {
                                    string configStr6 = File.ReadAllText($"../{mVipMonsterBoss}");
                                    List<MapSafeAreaInfo> mEnemyPathData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapSafeAreaInfo>>(configStr6);
                                    var mEnemyComponent = mMapComponent.AddCustomComponent<EnemyComponent>();
                                    mEnemyComponent.InitMapBossEnemy(mEnemyPathData);
                            }
                        }
                        var mNpcComponent = mMapComponent.AddCustomComponent<NpcComponent>();
                        mNpcComponent.InitMapNpc(mNpcPathData);
                    }
                    else if (mMapConfig.IsCopyMap == 1)
                    {
                        MapInfoCacheDic[mMapConfig.Id] = mMapInfo;
                        MapSafeAreaInfoCacheDic[mMapConfig.Id] = mMapSafeAreaInfo;
                        MapSpawnPathCacheDic[mMapConfig.Id] = mMapSpawnAreaInfo;
                        MapTransferPointPathCacheDic[mMapConfig.Id] = mMapTransferPointAreaInfo;
                        MapNpcPathCacheDic[mMapConfig.Id] = mNpcPathData;

                        /*if (mMapConfig.MonsterPathDic.TryGetValue(0, out var mMonsterPath))
                        {
                            string configStr3 = File.ReadAllText($"../{mMonsterPath}");
                            List<MapSafeAreaInfo> mEnemyPathData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapSafeAreaInfo>>(configStr3);
                            if (MapEnemySpawnPathCacheDic.TryGetValue(mMapConfig.Id, out var mTempDic) == false)
                            {
                                mTempDic = MapEnemySpawnPathCacheDic[mMapConfig.Id] = new Dictionary<int, List<MapSafeAreaInfo>>();
                            }
                            mTempDic[0] = mEnemyPathData;
                        }*/


                        if (mMapConfig.MonsterPathDic.Count > 0)
                        {
                            var mKey = mMapConfig.MonsterPathDic.Keys.ToArray();
                            for (int j = 1, jlen = mKey.Length; j <= jlen; j++)
                            {
                                if (mMapConfig.MonsterPathDic.TryGetValue(j, out var mMonsterPath))
                                {
                                    string configStr3 = File.ReadAllText($"../{mMonsterPath}");
                                    List<MapSafeAreaInfo> mEnemyPathData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<MapSafeAreaInfo>>(configStr3);
                                    if (MapEnemySpawnPathCacheDic.TryGetValue(mMapConfig.Id, out var mTempDic) == false)
                                    {
                                        mTempDic = MapEnemySpawnPathCacheDic[mMapConfig.Id] = new Dictionary<int, List<MapSafeAreaInfo>>();
                                    }
                                    mTempDic[j] = mEnemyPathData;
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new Exception($"load config file fail, path: {mMapConfig} {e}");
                }
            }

            Log.Console("地图初始化完成");
        }

        public static MapComponent GetMapByMapIndex(this MapManageComponent b_Component, int b_MapIndex)
        {
            if (b_Component.keyValuePairs.TryGetValue(b_MapIndex, out var mMap))
            {
                return mMap;
            }
            return null;
        }

        //public static bool SendNotice(this MapManageComponent b_Component, CombatSource b_Target, IActorMessage b_ActorMessage)
        //{
        //    var mapComponent = b_Component.GetMapByMapIndex(b_Target.UnitData.Index);
        //    if (mapComponent.FindTheWayDic.TryGetValue(b_Target.UnitData.X, out var mTempCellDic))
        //    {
        //        if (mTempCellDic.TryGetValue(b_Target.UnitData.Y, out var mMapCellSource))
        //        {
        //            mapComponent.SendNotice(mMapCellSource, b_ActorMessage);
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public static MapComponent Copy(this MapManageComponent b_Component, int b_MapId)
        {
            ReadConfigComponent mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mMapConfigs = mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic;
            if (mMapConfigs.TryGetValue(b_MapId, out var mMapConfig))
            {
                MapComponent mMapComponent = Root.CreateBuilder.GetInstance<MapComponent>(false);
                mMapComponent.Awake(b_Component);
                mMapComponent.MapId = mMapConfig.Id;

                mMapComponent.OnInit();
                mMapComponent.InitMap(MapInfoCacheDic[b_MapId]);
                mMapComponent.InitMapSafeArea(MapSafeAreaInfoCacheDic[b_MapId]);
                mMapComponent.InitMapSpawnSafeArea(MapSpawnPathCacheDic[b_MapId]);
                mMapComponent.InitMapTransferPoint(MapTransferPointPathCacheDic[b_MapId]);

                var mNpcComponent = mMapComponent.AddCustomComponent<NpcComponent>();
                mNpcComponent.InitMapNpc(MapNpcPathCacheDic[b_MapId]);

                if (MapEnemySpawnPathCacheDic.TryGetValue(mMapConfig.Id, out var mTempDic) == false)
                {
                    mTempDic = MapEnemySpawnPathCacheDic[mMapConfig.Id] = new Dictionary<int, List<MapSafeAreaInfo>>();
                }

                var mEnemyComponent = mMapComponent.AddCustomComponent<EnemyComponent>();
                mEnemyComponent.SaveMapEnemy(mTempDic);

                return mMapComponent;
            }
            return null;
        }

    }
}