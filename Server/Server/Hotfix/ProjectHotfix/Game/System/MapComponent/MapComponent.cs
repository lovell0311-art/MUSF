using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static ETHotfix.MapManageComponentSystem;
using static ETModel.MapManageComponent;

namespace ETHotfix
{
    [EventMethod("NSMapComponent.Destory")]
    public class NSMapComponentDestory : ITEventMethodOnRun<ETModel.EventType.NSMapComponent.Destory>
    {
        public void OnRun(ETModel.EventType.NSMapComponent.Destory args)
        {
            MapComponent map = args.self;
            Log.Info($"开始清理地图 Unit  mapId:{map.MapId} id:{map.Id}");
            // map.Dispose() 清理地图
            CombatSourceRecycleComponent.Instance.ClearCombatSource();

            foreach (var cell in map.MapCellFieldlist)
            {
                if(cell.FieldEnemyDic.Count != 0)
                {
                    Enemy[] allUnit = cell.FieldEnemyDic.Values.ToArray();
                    foreach (Enemy unit in allUnit)
                    {
                        map.Leave(unit);
                        unit.Dispose();
                    }
                }
                if(cell.FieldSummonedDic.Count != 0)
                {
                    Summoned[] allUnit = cell.FieldSummonedDic.Values.ToArray();
                    foreach (Summoned unit in allUnit)
                    {
                        // 玩家所属单位，只将其移除地图
                        map.Leave(unit);
                    }
                }
                if (cell.FieldHolyteacherSummonedDic.Count != 0)
                {
                    HolyteacherSummoned[] allUnit = cell.FieldHolyteacherSummonedDic.Values.ToArray();
                    foreach (HolyteacherSummoned unit in allUnit)
                    {
                        // 玩家所属单位，只将其移除地图
                        map.Leave(unit);
                    }
                }
                if (cell.FieldPetsDic.Count != 0)
                {
                    Pets[] allUnit = cell.FieldPetsDic.Values.ToArray();
                    foreach (Pets unit in allUnit)
                    {
                        // 玩家所属单位，只将其移除地图
                        map.Leave(unit);
                    }
                }
            }

            MapEntity[] allEntities = args.self.AllEntities.Values.ToArray();
            foreach (MapEntity entity in allEntities)
            {
                entity.Dispose();
            }
            args.self.AllEntities.Clear();
            Log.Info($"开始清理地图 Unit 完成  mapId:{map.MapId} id:{map.Id}");
        }
    }


    /// <summary>
    /// 单个地图
    /// </summary>
    public static partial class MapComponentSystem
    {

        public static void OnInit(this MapComponent b_Component)
        {
            b_Component.AddCustomComponent<BattleComponent>();
        }



        /// <summary>
        /// 设置障碍物
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_CombatUnit"></param>
        /// <param name="b_IsObstacle"></param>
        /// <returns></returns>
        public static C_FindTheWay2D SetStaticObstacle(this MapComponent b_Component, C_FindTheWay2D b_CombatUnit, bool b_IsObstacle = true)
        {
            //if (b_IsObstacle)
            //    b_Component.FindTheEnemyAndUsDic[b_CombatUnit.X][b_CombatUnit.Y] = b_CombatUnit;
            //else
            //    b_Component.FindTheEnemyAndUsDic[b_CombatUnit.X][b_CombatUnit.Y] = null;
            b_CombatUnit.IsStaticObstacle = b_IsObstacle;
            return b_CombatUnit;
        }
        public static void InitMap(this MapComponent b_Component, MapInfo b_MapInfo)
        {
            b_Component.FindPathComponent = b_Component.GetCustomComponent<FindPathComponent>();
            if (b_Component.FindPathComponent == null)
            {
                b_Component.FindPathComponent = b_Component.AddCustomComponent<FindPathComponent>();
                // 初始化地形信息 从0开始  Max-1结束
                b_Component.FindTheWayDic = b_Component.FindPathComponent.PathFindingCenterInit(b_MapInfo.width, b_MapInfo.height, new Vector3(0, 0, 0), 1.0f, 1.0f, true);
            }
            b_Component.MapWidth = b_MapInfo.width;
            b_Component.MapHight = b_MapInfo.height;

            Log.Debug(b_MapInfo.SceneInfoSize.ToString());
            Log.Debug(b_MapInfo.SceneInfos.Count.ToString());

            int mAreaHeight = 12;
            int mAreaWidth = 12;
            int mRowUnitCount = (b_MapInfo.width / mAreaWidth + 1);
            for (int y = 0; y < b_MapInfo.height; y++)
            {
                for (int x = 0; x < b_MapInfo.width; x++)
                {
                    int index = (y * b_MapInfo.width) + x;

                    int mValue = b_MapInfo.SceneInfos[index];

                    C_FindTheWay2D mFindTheWay = b_Component.GetFindTheWay2D(x, y);
                    if (mFindTheWay == null) continue;

                    mFindTheWay.Map = b_Component;

                    if (mValue == 1)
                    {
                        mFindTheWay.IsStaticObstacle = true;
                    }

                    int mAreaPosX = (mFindTheWay.X / mAreaWidth);
                    int mAreaPosY = (mFindTheWay.Y / mAreaHeight);
                    int areaIndex = mAreaPosX + mAreaPosY * mRowUnitCount;
                    mFindTheWay.AreaIndex = areaIndex;
                    mFindTheWay.AreaPosX = mAreaPosX;
                    mFindTheWay.AreaPosY = mAreaPosY;

                    if (b_Component.MapCellFieldDic.TryGetValue(mAreaPosX, out var mapCellAreaDic) == false)
                    {
                        mapCellAreaDic = b_Component.MapCellFieldDic[mAreaPosX] = new Dictionary<int, MapCellAreaComponent>();
                    }
                    if (mapCellAreaDic.TryGetValue(mAreaPosY, out var mCurrentCellField) == false)
                    {
                        mCurrentCellField = mapCellAreaDic[mAreaPosY] = Root.CreateBuilder.GetInstance<MapCellAreaComponent>(false);
                        mCurrentCellField.Awake(b_Component);
                        mCurrentCellField.OnInit();

                        mCurrentCellField.AreaPosX = mAreaPosX;
                        mCurrentCellField.AreaPosY = mAreaPosY;
                        mCurrentCellField.AreaIndex = areaIndex;
                        mCurrentCellField.AroundFieldDic = new Dictionary<int, MapCellAreaComponent>();
                        mCurrentCellField.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                    }
                    mFindTheWay.MapField = mCurrentCellField;

                    if (b_Component.MapCellFieldDic.TryGetValue(mCurrentCellField.AreaPosX - 1, out var mapCellAreaDicLeft))
                    {
                        if (mapCellAreaDicLeft.TryGetValue(mCurrentCellField.AreaPosY - 1, out var mLeftUpNode))
                        {
                            mCurrentCellField.LeftUpNode = mLeftUpNode;
                            mCurrentCellField.AroundFieldDic[mLeftUpNode.AreaIndex] = mLeftUpNode;

                            mLeftUpNode.RightDownNode = mCurrentCellField;
                            mLeftUpNode.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                        }
                        if (mapCellAreaDicLeft.TryGetValue(mCurrentCellField.AreaPosY, out var mLeftMiddleNode))
                        {
                            mCurrentCellField.LeftMiddleNode = mLeftMiddleNode;
                            mCurrentCellField.AroundFieldDic[mLeftMiddleNode.AreaIndex] = mLeftMiddleNode;

                            mLeftMiddleNode.RightMiddleNode = mCurrentCellField;
                            mLeftMiddleNode.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                        }
                    }
                    if (b_Component.MapCellFieldDic.TryGetValue(mCurrentCellField.AreaPosX, out var mapCellAreaDicCenter))
                    {
                        if (mapCellAreaDicCenter.TryGetValue(mCurrentCellField.AreaPosY - 1, out var mUpMiddleNode))
                        {
                            mCurrentCellField.UpMiddleNode = mUpMiddleNode;
                            mCurrentCellField.AroundFieldDic[mUpMiddleNode.AreaIndex] = mUpMiddleNode;

                            mUpMiddleNode.DownMiddleNode = mCurrentCellField;
                            mUpMiddleNode.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                        }
                    }
                    if (b_Component.MapCellFieldDic.TryGetValue(mCurrentCellField.AreaPosX + 1, out var mapCellAreaDicRight))
                    {
                        if (mapCellAreaDicRight.TryGetValue(mCurrentCellField.AreaPosY - 1, out var mRightUpNode))
                        {
                            mCurrentCellField.RightUpNode = mRightUpNode;
                            mCurrentCellField.AroundFieldDic[mRightUpNode.AreaIndex] = mRightUpNode;

                            mRightUpNode.LeftDownNode = mCurrentCellField;
                            mRightUpNode.AroundFieldDic[mCurrentCellField.AreaIndex] = mCurrentCellField;
                        }
                    }
                }
            }

            var mRowFieldDiclist = b_Component.MapCellFieldDic.Values.ToArray();
            for (int i = 0, len = mRowFieldDiclist.Length; i < len; i++)
            {
                var mRowFieldDic = mRowFieldDiclist[i];

                var mFieldDiclist = mRowFieldDic.Values.ToArray();
                for (int j = 0, jlen = mFieldDiclist.Length; j < jlen; j++)
                {
                    var mCurrentCellField = mFieldDiclist[j];

                    b_Component.MapCellFieldlist.Add(mCurrentCellField);

                    mCurrentCellField.AroundField = mCurrentCellField.AroundFieldDic.Keys.ToList();
                    mCurrentCellField.AroundFieldArray = mCurrentCellField.AroundFieldDic.Values.ToArray();
                }
            }
        }
        public static void InitMapSafeArea(this MapComponent b_Component, List<MapSafeAreaInfo> b_MapSafeAreaInfos)
        {
            for (int i = 0, len = b_MapSafeAreaInfos.Count; i < len; i++)
            {
                var mSafeAreaInfo = b_MapSafeAreaInfos[i];

                var Temp = b_Component.GetFindTheWay2D(mSafeAreaInfo.PositionX, mSafeAreaInfo.PositionY);
                if (Temp != null && Temp.IsStaticObstacle == false)
                {
                    Temp.IsSafeArea = true;
                    if (b_Component.SafeFindTheWayDic.TryGetValue(mSafeAreaInfo.PositionX, out var mSafeAreaDic) == false)
                    {
                        b_Component.SafeFindTheWayDic[mSafeAreaInfo.PositionX] = mSafeAreaDic = new Dictionary<int, C_FindTheWay2D>();
                    }
                    if (mSafeAreaDic.ContainsKey(mSafeAreaInfo.PositionY) == false)
                    {
                        mSafeAreaDic[mSafeAreaInfo.PositionY] = Temp;
                    }
                }
            }
        }
        public static void InitMapSpawnSafeArea(this MapComponent b_Component, List<MapSafeAreaInfo> b_MapSafeAreaInfos)
        {
            for (int i = 0, len = b_MapSafeAreaInfos.Count; i < len; i++)
            {
                var mSafeAreaInfo = b_MapSafeAreaInfos[i];

                var Temp = b_Component.GetFindTheWay2D(mSafeAreaInfo.PositionX, mSafeAreaInfo.PositionY);
                if (Temp != null && Temp.IsStaticObstacle == false)
                {
                    if (b_Component.SpawnSafeFindTheWayDic.TryGetValue(mSafeAreaInfo.PositionX, out var mSafeAreaDic) == false)
                    {
                        b_Component.SpawnSafeFindTheWayDic[mSafeAreaInfo.PositionX] = mSafeAreaDic = new Dictionary<int, C_FindTheWay2D>();
                    }
                    if (mSafeAreaDic.ContainsKey(mSafeAreaInfo.PositionY) == false)
                    {
                        mSafeAreaDic[mSafeAreaInfo.PositionY] = Temp;
                    }
                }
            }
        }

        public static void InitMapTransferPoint(this MapComponent b_Component, List<MapDeliveryAreaInfo> b_MapSafeAreaInfos)
        {
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Map_TransferPointConfigJson>().JsonDic;

            for (int i = 0, len = b_MapSafeAreaInfos.Count; i < len; i++)
            {
                var mTransferPoint = b_MapSafeAreaInfos[i];

                if (mJsonDic.ContainsKey(mTransferPoint.Index) == false)
                {
                    continue;
                }

                var Temp = b_Component.GetFindTheWay2D(mTransferPoint.PositionX, mTransferPoint.PositionY);
                if (Temp != null && Temp.IsStaticObstacle == false)
                {
                    Temp.TransferPoint = mTransferPoint.Index;

                    if (b_Component.TransferPointFindTheWayDic.TryGetValue(mTransferPoint.Index, out var mTransferPointlist) == false)
                    {
                        b_Component.TransferPointFindTheWayDic[mTransferPoint.Index] = mTransferPointlist = new List<C_FindTheWay2D>();
                    }
                    if (mTransferPointlist.Contains(Temp) == false)
                    {
                        mTransferPointlist.Add(Temp);
                    }

                    //if (b_Component.SpawnSafeFindTheWayDic.TryGetValue(mTransferPoint.PositionX, out var mSafeAreaDic))
                    //{
                    //    if (mSafeAreaDic.ContainsKey(mTransferPoint.PositionY))
                    //    {
                    //        LogToolComponent.SimpleError($"{mTransferPoint.PositionX} {mTransferPoint.PositionY}", false);
                    //    }
                    //}
                }
            }
        }

        public static bool TryGetPosX(this MapComponent b_Component, int b_PosX)
        {
            if (b_PosX < 0) return false;
            if (b_PosX >= b_Component.MapWidth) return false;
            return true;
        }
        public static bool TryGetPosY(this MapComponent b_Component, int b_PosY)
        {
            if (b_PosY < 0) return false;
            if (b_PosY >= b_Component.MapHight) return false;
            return true;
        }
        public static C_FindTheWay2D GetFindTheWay2D(this MapComponent b_Component, int b_X, int b_Y)
        {
            if (b_X < 0) return null;
            if (b_Y < 0) return null;
            if (b_X >= b_Component.MapWidth) return null;
            if (b_Y >= b_Component.MapHight) return null;

            return b_Component.FindTheWayDic[b_X, b_Y];
        }

        public static C_FindTheWay2D GetFindTheWay2D(this MapComponent b_Component, CombatSource b_CombatSource)
        {
            if (b_CombatSource.UnitData.Index != b_Component.MapId) return null;

            int mPosX = b_CombatSource.UnitData.X;
            int mPosY = b_CombatSource.UnitData.Y;

            if (mPosX < 0) return null;
            if (mPosY < 0) return null;
            if (mPosX >= b_Component.MapWidth) return null;
            if (mPosY >= b_Component.MapHight) return null;

            return b_Component.FindTheWayDic[mPosX, mPosY];
        }
        public static MapCellAreaComponent GetMapCellFieldByPos(this MapComponent b_Component, int b_X, int b_Y)
        {
            var mFindTheWay = b_Component.GetFindTheWay2D(b_X, b_Y);
            if (mFindTheWay != null)
            {
                return b_Component.GetMapCellField(mFindTheWay);
            }
            return null;
        }
        public static MapCellAreaComponent GetMapCellField(this MapComponent b_Component, C_FindTheWay2D b_FindTheWay2D)
        {
            if (b_Component.MapCellFieldDic.TryGetValue(b_FindTheWay2D.AreaPosX, out var mRowMapCellFieldDic))
            {
                if (mRowMapCellFieldDic.TryGetValue(b_FindTheWay2D.AreaPosY, out var mCenterMapCellField))
                {
                    return mCenterMapCellField;
                }
            }
            return null;
        }
        public static MapCellAreaComponent GetMapCellField(this MapComponent b_Component, CombatSource b_CombatSource)
        {
            C_FindTheWay2D mFindTheWay = b_Component.GetFindTheWay2D(b_CombatSource);
            if (mFindTheWay != null)
            {
                return b_Component.GetMapCellField(mFindTheWay);
            }
            return null;
        }

    }

}