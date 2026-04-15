using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Mrs.V20200910.Models;


namespace ETHotfix
{
    [EventMethod(typeof(NewYearAnimalActivity), EventSystemType.INIT)]
    public class NewYearAnimalActivityEventOnInit : ITEventMethodOnInit<NewYearAnimalActivity>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(NewYearAnimalActivity b_Component)
        {
            b_Component.OnInit();
        }
    }
    public static partial class NewYearAnimalActivityComponentSystem
    {
        public static void OnInit(this NewYearAnimalActivity b_Component)
        {
            var Json = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Kill_RewardConfigJson>().JsonDic;
            int ActivityID = Json[1].ActivityID;
            foreach (var item in Json)
            {
                if (b_Component.Mob_LIst == null || b_Component.MobCnt == null)
                {
                    b_Component.Mob_LIst = new Dictionary<int, List<Enemy>>();
                    b_Component.MobCnt = new Dictionary<int, int>();
                }
                if (b_Component.Mob_LIst.ContainsKey(item.Key) == false)
                {
                    List<Enemy> enemies = new List<Enemy>();    
                    b_Component.Mob_LIst.Add(item.Key, enemies);
                    b_Component.MobCnt.Add(item.Key, 0);
                }
            }
            b_Component.ActivitID = ActivityID;
            b_Component.IsBrushMonster = true;
            var Json2 = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Activity_InfoConfigJson>().JsonDic;
            if (Json2 != null)
            {
                b_Component.Parent.GetCustomComponent<ActivitiesComponent>().CheckActivitTime(Json2[ActivityID].OpenTime, Json2[ActivityID].EndTime, ActivityID);
            }
        }



        public static void UpdateNewYearAnimalActivity(this NewYearAnimalActivity b_Component, C_ServerArea ServerInfo)
        {
            if (b_Component.Parent.GetCustomComponent<ActivitiesComponent>().Activities.TryGetValue(b_Component.ActivitID, out StructActivit Info) == false) return;
            if (!Info.IsOpen) return;
            if (!b_Component.IsBrushMonster) return;
            //只在1线和2线刷
            if(ServerInfo.GameAreaId != 1|| ServerInfo.GameAreaId!=2) return;
            //int wk = Convert.ToInt32(DateTime.Now.DayOfWeek);
            //if (wk != 6 && wk != 7) return;
            {
                var Json = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Kill_RewardConfigJson>().JsonDic;
                List<int> ints = new List<int>();
                int Index = 0;
                foreach (var ActivityInfo in Json)
                {
                    ints.Clear();
                    foreach (var Tiem in ActivityInfo.Value.BrushTimeDic)
                    {
                        if (DateTime.Now.Hour == Tiem.Key && DateTime.Now.Minute == Tiem.Value)
                        {
                            if (b_Component.RecycleTime == (0, 0))
                            {
                                int H = DateTime.Now.Hour + ActivityInfo.Value.ExistenceTime[0];
                                int M = DateTime.Now.Minute + ActivityInfo.Value.ExistenceTime[1];
                                if (M >= 60) { H++; M -= 60; }
                                if (H >= 24) { H -= 24; }
                                b_Component.RecycleTime = (H, M);
                            }

                            //Log.PLog("ShuaGuai", $"UP b_Component.RecycleTime.Item1:{b_Component.RecycleTime.Item1}-{DateTime.Now.Hour} b_Component.RecycleTime.Item2:{b_Component.RecycleTime.Item2}-{DateTime.Now.Minute}");
                            for (int i = 0; i < ActivityInfo.Value.MiracleCoin;)
                            {
                                Index = Help_RandomHelper.Range(1, 100);
                                if (ints.Contains(Index) == false)
                                {
                                    switch (ActivityInfo.Key)
                                    {
                                        case 1:
                                            var Json2 = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Yong_ShuaGuaiConfigJson>().JsonDic;
                                            //Log.PLog("ShuaGuai",$"Yong MapID:{ActivityInfo.Key} MobID:{ActivityInfo.Value.ModID} X:{Json2[Index].PosX} Y:{Json2[Index].PosY}");
                                            b_Component.CreateMonster(ServerInfo, ActivityInfo.Key, ActivityInfo.Value.ModID, Json2[Index].PosX, Json2[Index].PosY);
                                            ints.Add(Index);
                                            i++;
                                            break;
                                        case 2:
                                            var Json3 = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<XianLin_ShuaGuaiConfigJson>().JsonDic;
                                            //Log.PLog("ShuaGuai", $"Xian MapID:{ActivityInfo.Key} MobID:{ActivityInfo.Value.ModID} X:{Json3[Index].PosX} Y:{Json3[Index].PosY}");
                                            b_Component.CreateMonster(ServerInfo, ActivityInfo.Key, ActivityInfo.Value.ModID, Json3[Index].PosX, Json3[Index].PosY);
                                            ints.Add(Index);
                                            i++;
                                            break;
                                        case 3:
                                            var Json4 = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<HuanShuYuan_ShaGuaiConfigJson>().JsonDic;
                                            //Log.PLog("ShuaGuai", $"Huan MapID:{ActivityInfo.Key} MobID:{ActivityInfo.Value.ModID} X:{Json4[Index].PosX} Y:{Json4[Index].PosY}");
                                            b_Component.CreateMonster(ServerInfo, ActivityInfo.Key, ActivityInfo.Value.ModID, Json4[Index].PosX, Json4[Index].PosY);
                                            ints.Add(Index);
                                            i++;
                                            break;
                                    }


                                }
                            }
                            b_Component.IsBrushMonster = false;
                        }
                    }
                }
            }
        }

        public static void CreateMonster(this NewYearAnimalActivity b_Component, C_ServerArea ServerInfo, int MapID, int EnemyID, int PositionX, int PositionY)
        {
            MapComponent mapComponent = ServerInfo.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(MapID);
            EnemyComponent b_EnemyComponent = mapComponent.GetCustomComponent<EnemyComponent>();
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;

            if (mJsonDic.TryGetValue(EnemyID, out var mEnemyConfig))
            {
                var mEnemy = Root.CreateBuilder.GetInstance<Enemy>(false);
                mEnemy.Awake(b_Component);

                mEnemy.SetConfig(mEnemyConfig);
                mEnemy.SetInstanceId(mEnemy.Id);
                mEnemy.AfterAwake();
                mEnemy.AwakeSkill();
                mEnemy.DataUpdateSkill();

                mEnemy.SourcePosX = PositionX;
                mEnemy.SourcePosY = PositionY;

                if (b_Component.Mob_LIst.ContainsKey(MapID) == false)
                {
                    List<Enemy> enemies = new List<Enemy>();
                    b_Component.Mob_LIst.Add(MapID, enemies);
                }
                b_Component.Mob_LIst[MapID].Add(mEnemy);
                b_Component.MobCnt[MapID]++;

                var mFindTheWay = mapComponent.GetFindTheWay2D(mEnemy.SourcePosX, mEnemy.SourcePosY);
                if (mFindTheWay.IsStaticObstacle)
                {
                    Log.Warning($"怪物出生点是障碍物 MapId:{MapID} X:{PositionX} Y:{PositionY} id:{EnemyID}");
                }

                mapComponent.MoveSendNotice(null, mFindTheWay, mEnemy);
            }
        }

        public static void RecycleMonster(this NewYearAnimalActivity b_Component, C_ServerArea ServerInfo)
        {
            if (b_Component.Mob_LIst == null) return;
            if (b_Component.RecycleTime.Item1 == DateTime.Now.Hour && b_Component.RecycleTime.Item2 == DateTime.Now.Minute)
            {
                foreach (var Mob in b_Component.Mob_LIst)
                {
                    for (int i=0;i<Mob.Value.Count;++i)
                    {
                        Mob.Value[i].CurrentMap?.Leave(Mob.Value[i]);
                        Mob.Value[i].Dispose();
                        b_Component.MobCnt[Mob.Key]--;
                    }
                }
                b_Component.Mob_LIst.Clear();
                b_Component.IsBrushMonster = true;
                b_Component.RecycleTime = (0, 0);
            }
        }

        public static void ActiveFall(this NewYearAnimalActivity b_Component, C_ServerArea ServerInfo)
        {
            if (b_Component.Mob_LIst != null)
            {
                foreach (var Mob in b_Component.Mob_LIst)
                {
                    using ListComponent<Enemy> delList = ListComponent<Enemy>.Create();
                    for (int i = 0; i < Mob.Value.Count;++i)
                    {
                        if (Mob.Value[i].IsDeath)
                        {
                            delList.Add(Mob.Value[i]);
                            b_Component.MobCnt[Mob.Key]--;
                            Mob.Value[i].CurrentMap?.Leave(Mob.Value[i]);
                            Mob.Value[i].Dispose();
                        }
                    }
                    foreach (Enemy enemy in delList)
                    {
                        Mob.Value.Remove(enemy);
                    }
                }
            }
        }
    }
}

