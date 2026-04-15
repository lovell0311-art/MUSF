using System;
using ETModel;
using ETModel.EventType;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using System.Collections.Generic;
using System.Linq;
using Aop.Api.Domain;

namespace ETHotfix
{
    [EventMethod("CombatSourceEnterOrSwitchMap")]
    public class CombatSourceEnterOrSwitchMap_AddEnemyToEnemyComponent : ITEventMethodOnRun<CombatSourceEnterOrSwitchMap>
    {
        public void OnRun(CombatSourceEnterOrSwitchMap args)
        {
            if (args.combatSource.Identity != E_Identity.Enemy) return;
            EnemyComponent enemy = args.newMap.GetCustomComponent<EnemyComponent>();
            if (enemy == null) return;
            enemy.AllEnemyDic.TryAdd(args.combatSource.InstanceId, (Enemy)args.combatSource);
        }
    }

    [EventMethod("CombatSourceLeaveMap")]
    public class CombatSourceLeaveMap_RemoveEnemyToEnemyComponent : ITEventMethodOnRun<CombatSourceLeaveMap>
    {
        public void OnRun(CombatSourceLeaveMap args)
        {
            if (args.combatSource.Identity != E_Identity.Enemy) return;
            EnemyComponent enemy = args.leavedMap.GetCustomComponent<EnemyComponent>();
            if (enemy == null) return;
            enemy.AllEnemyDic.Remove(args.combatSource.InstanceId);
        }
    }


    /// <summary>
    /// 单个地图
    /// </summary>
    public static partial class EnemyComponentSystem
    {
        public static void InitMapEnemy(this EnemyComponent b_Component, List<MapSafeAreaInfo> b_EnemyInfos)
        {
            var mMapComponent = b_Component.Parent;
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;

            for (int i = 0, len = b_EnemyInfos.Count; i < len; i++)
            {
                var mEnemyInfo = b_EnemyInfos[i];

                if (mJsonDic.TryGetValue(mEnemyInfo.Index, out var mEnemyConfig))
                {
                    var mEnemy = Root.CreateBuilder.GetInstance<Enemy>(false);
                    mEnemy.Awake(b_Component);

                    //long mEnemyInstanceId = (long)i | (long)mMapComponent.MapId << 24 | (long)mMapComponent.Parent.Parent.SourceId << 32;
                    mEnemy.SetConfig(mEnemyConfig);
                    //mEnemy.SetInstanceId(mEnemyInstanceId);
                    mEnemy.SetInstanceId(mEnemy.Id);
                    mEnemy.AfterAwake();
                    mEnemy.AwakeSkill();
                    mEnemy.DataUpdateSkill();

                    mEnemy.SourcePosX = mEnemyInfo.PositionX;
                    mEnemy.SourcePosY = mEnemyInfo.PositionY;

                    b_Component.AllEnemyDic[mEnemy.InstanceId] = mEnemy;

                    var mFindTheWay = mMapComponent.GetFindTheWay2D(mEnemy.SourcePosX, mEnemy.SourcePosY);
                    if (mFindTheWay == null)
                    {
                        throw new Exception($"怪物出生点为 null MapId:{b_Component.Parent.MapId} {mEnemyInfo.PositionX} {mEnemyInfo.PositionY} id:{mEnemyInfo.Index}");
                    }
                    else if (mFindTheWay.IsStaticObstacle)
                    {
                        Log.Warning($"怪物出生点是障碍物 MapId:{b_Component.Parent.MapId} {mEnemyInfo.PositionX} {mEnemyInfo.PositionY} id:{mEnemyInfo.Index}");
                    }


                    mMapComponent.MoveSendNotice(null, mFindTheWay, mEnemy);
                    //血色城堡，城门周围设计静态障碍
                    if (mMapComponent.MapId == 100 && mEnemy.Config.Id == 197)
                    { 
                        List<int> Y = new List<int>() { 109,110,111,112,113};
                        foreach (var y in Y)
                        {
                            mMapComponent.GetFindTheWay2D(86, y).IsStaticObstacle = true;
                        }
                    }
                }
            }
        }
        public static Enemy InitMapBossEnemy(this EnemyComponent b_Component, List<MapSafeAreaInfo> b_EnemyInfos)
        {
            var mMapComponent = b_Component.Parent;
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;

            Dictionary<int, List<(int, int)>> SourcePoslist = new Dictionary<int, List<(int, int)>>();
            for (int i = 0, len = b_EnemyInfos.Count; i < len; i++)
            {
                var mEnemyInfo = b_EnemyInfos[i];

                if (SourcePoslist.TryGetValue(mEnemyInfo.Index, out var mTemplist) == false)
                {
                    mTemplist = SourcePoslist[mEnemyInfo.Index] = new List<(int, int)>();
                }
                var mdata = (mEnemyInfo.PositionX, mEnemyInfo.PositionY);
                mTemplist.Add(mdata);
            }

            var mIndex = Help_RandomHelper.Range(0, SourcePoslist.Count);
            var mRandomKey = SourcePoslist.Keys.ToList()[mIndex];
            var mRandomValue = SourcePoslist[mRandomKey];
            {
                if (mJsonDic.TryGetValue(mRandomKey, out var mEnemyConfig))
                {
                    var mEnemy = Root.CreateBuilder.GetInstance<Enemy>(false);
                    mEnemy.Awake(b_Component);

                    mEnemy.SetConfig(mEnemyConfig);
                    mEnemy.SetInstanceId(mEnemy.Id);
                    mEnemy.AfterAwake();
                    mEnemy.AwakeSkill();
                    mEnemy.DataUpdateSkill();

                    mEnemy.SourcePoslist = mRandomValue;

                    mIndex = Help_RandomHelper.Range(0, mRandomValue.Count);
                    (int PositionX, int PositionY) mEnemyInfo = mRandomValue[mIndex];
                    mEnemy.SourcePosX = mEnemyInfo.PositionX;
                    mEnemy.SourcePosY = mEnemyInfo.PositionY;

                    b_Component.AllEnemyDic[mEnemy.InstanceId] = mEnemy;

                    var mFindTheWay = mMapComponent.GetFindTheWay2D(mEnemy.SourcePosX, mEnemy.SourcePosY);
                    if (mFindTheWay == null)
                    {
                        throw new Exception($"怪物出生点为 null MapId:{b_Component.Parent.MapId} {mEnemyInfo.PositionX} {mEnemyInfo.PositionY} id:{mRandomKey}");
                    }
                    else if (mFindTheWay.IsStaticObstacle)
                    {
                        Log.Warning($"怪物出生点是障碍物 MapId:{b_Component.Parent.MapId} {mEnemyInfo.PositionX} {mEnemyInfo.PositionY} id:{mRandomKey}");
                    }

                    mMapComponent.MoveSendNotice(null, mFindTheWay, mEnemy);

                    return mEnemy;
                }
            }
            return null;
        }


        public static void InitMapSpecifyEnemy(this EnemyComponent b_Component, List<MapSafeAreaInfo> b_EnemyInfos, int b_monsterId,int Cnt, bool AddAttribute = false)
        {
            var mMapComponent = b_Component.Parent;
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;
            mJsonDic.TryGetValue(b_monsterId, out var mEnemyConfig);
            if (mEnemyConfig == null)
            {
                Log.Error($"创建副本怪物时，找不到怪物配置表");
                return;
            }

            for (int i = 0, len = b_EnemyInfos.Count; i < len; i++)
            {
                if (i == Cnt) break;

                var mEnemyInfo = b_EnemyInfos[i];

                var mEnemy = Root.CreateBuilder.GetInstance<Enemy>(false);
                mEnemy.Awake(b_Component);

                mEnemy.SetConfig(mEnemyConfig);
                mEnemy.AfterAwake();
                mEnemy.SetInstanceId(mEnemy.Id);
                mEnemy.AwakeSkill();
                mEnemy.DataUpdateSkill();
                mEnemy.SourcePosX = mEnemyInfo.PositionX;
                mEnemy.SourcePosY = mEnemyInfo.PositionY;
                if (AddAttribute)
                {
                    //试炼塔怪物增加两个属性
                    mEnemy.GamePropertyDic[E_GameProperty.AttackIgnoreDefenseRate] = 1000;
                    mEnemy.GamePropertyDic[E_GameProperty.IgnoreAbsorbRate] = 1000;
                }
                b_Component.AllEnemyDic[mEnemy.InstanceId] = mEnemy;

                var mFindTheWay = mMapComponent.GetFindTheWay2D(mEnemy.SourcePosX, mEnemy.SourcePosY);

                mMapComponent.MoveSendNotice(null, mFindTheWay, mEnemy);
            }
        }


        public static void SaveMapEnemy(this EnemyComponent b_Component, Dictionary<int, List<MapSafeAreaInfo>> b_EnemyInfos)
        {
            b_Component.EnemyDataDic = new Dictionary<int, List<MapSafeAreaInfo>>(b_EnemyInfos);
        }

        public static void InitMapEnemy(this EnemyComponent b_Component, int b_monsterId, int Level = 1,int Cnt = 50,bool AddAttribute = false)
        {
            if (b_Component.EnemyDataDic.TryGetValue(1, out var mEnemyInfos))
            {
                b_Component.InitMapSpecifyEnemy(mEnemyInfos, b_monsterId, Cnt, AddAttribute);
            }
        }

        public static void InitMapEnemy(this EnemyComponent b_Component, int Level = 1)
        {
            if (b_Component.EnemyDataDic.TryGetValue(Level, out var mEnemyInfos))
            {
                b_Component.InitMapEnemy(mEnemyInfos);
            }
        }


        public static C_FindTheWay2D NearFindTheWay(this MapComponent b_Component, C_FindTheWay2D b_CurrentPoint, C_FindTheWay2D b_TargetPos)
        {
            float distance = MathF.Abs(b_TargetPos.X - b_CurrentPoint.X) + MathF.Abs(b_TargetPos.Y - b_CurrentPoint.Y);

            C_FindTheWay2D result = null;
            // 应该判断距离 距离近者优先
            void CompareH(C_FindTheWay2D findTheWay)
            {
                if (findTheWay.IsObstacle == false)
                {
                    float h = MathF.Abs(b_TargetPos.X - findTheWay.X) + MathF.Abs(b_TargetPos.Y - findTheWay.Y);
                    if (distance > h)
                    {
                        distance = h;
                        result = findTheWay;
                    }
                }
            }
            int left = b_CurrentPoint.X - 1;
            int right = b_CurrentPoint.X + 1;
            int up = b_CurrentPoint.Y + 1;
            int down = b_CurrentPoint.Y - 1;
            // 上下左右
            if (left >= 0)
            {
                C_FindTheWay2D findTheWay = b_Component.GetFindTheWay2D(left, b_CurrentPoint.Y);
                CompareH(findTheWay);
            }
            if (up < b_Component.MapHight)
            {
                C_FindTheWay2D findTheWay = b_Component.GetFindTheWay2D(b_CurrentPoint.X, up);
                CompareH(findTheWay);
            }
            if (right <= b_Component.MapWidth)
            {
                C_FindTheWay2D findTheWay = b_Component.GetFindTheWay2D(right, b_CurrentPoint.Y);
                CompareH(findTheWay);
            }
            if (down >= 0)
            {
                C_FindTheWay2D findTheWay = b_Component.GetFindTheWay2D(b_CurrentPoint.X, down);
                CompareH(findTheWay);
            }

            return result;
        }
    }
}