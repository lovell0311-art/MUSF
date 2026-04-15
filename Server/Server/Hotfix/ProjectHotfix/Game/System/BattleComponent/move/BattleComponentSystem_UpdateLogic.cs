using ETModel;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CustomFrameWork;
using CustomFrameWork.Component;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    public static partial class BattleComponentSystem
    {
        public static void UpdateLogic(this BattleComponent b_Component, GamePlayer b_CombatSource, MapComponent b_MapComponent)
        {

        }
        public static void LateUpdateLogic(this BattleComponent b_Component, GamePlayer b_CombatSource, MapComponent b_MapComponent)
        {
            long mCurrentTime = b_Component.CurrentTimeTick;

            if (b_CombatSource.Player.OnlineStatus == EOnlineStatus.Offline) return;

            // 本单位阵亡了?
            if (b_CombatSource.IsDeath)
            {
                //如果在副本 死亡直接退出房间 回到 ？
                if (BatteCopyManagerComponent.BattleCopyMapIDList.Contains(b_MapComponent.MapId))
                {
                    C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_CombatSource.Player.SourceGameAreaId);
                    if (mServerArea == null)
                    {
                        Log.Error("数据库异常 找不到区分 无法复活");
                        return;
                    }

                    //mServerArea.GetCustomComponent<BatteCopyManagerComponent>().PlayerDeathHandler(b_CombatSource, mServerArea);
                    if (b_CombatSource.CopyLiveCnt <= 0 || b_CombatSource.DeathSleepTime <= mCurrentTime)
                    {
                        b_CombatSource.CopyLiveCnt = 0;

                        mServerArea.GetCustomComponent<BatteCopyManagerComponent>().PlayerDeathHandlerAsync(b_CombatSource, mServerArea).Coroutine();
                        RebirthHelper.Rebirth(
                            b_CombatSource,
                            b_CombatSource.CurrentMap,
                            b_CombatSource.UnitData.X,
                            b_CombatSource.UnitData.Y,
                            "副本死亡复活"
                            , reEnterMap: false);
                        return;
                    }
                }
                if (b_CombatSource.GetCustomComponent<RebirthComponent>() == null)
                {
                    // 添加复活组件
                    b_CombatSource.AddCustomComponent<RebirthComponent>();
                    // 怪物需要将自己移出地图
                    // 让玩家在地图中带着，保留玩家视野
                    //b_CombatSource.CurrentMap?.Leave(b_CombatSource);
                }
            }

            if (b_CombatSource.IsDeath == false) b_CombatSource.RunSyncTaskAction(mCurrentTime, b_Component);
        }
        public static void UpdateLogic(this BattleComponent b_Component, Enemy b_CombatSource, long b_CurrentTime, MapComponent b_MapComponent)
        {
            // 本单位阵亡了?
            if (b_CombatSource.IsDeath) return;
            // 本单位已经在攻击中?
            if (b_CombatSource.IsAttacking) return;

            if (b_CombatSource.IsCheckState(2)) return;


            if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
            {

            }
            else
            {
                if (b_Component.Logic(b_CombatSource, b_CurrentTime)) return;

                if (b_CombatSource.IsAttacking) return;
                if (b_CombatSource.IsCanMove() == false) return;

                if (!b_CombatSource.IsCheckState(1))//不攻击的怪
                    b_Component.WarEnemyTargetUpdate(b_CombatSource);

                if (b_CombatSource.TargetEnemy != null)
                {// 朝着目标走
                    if (b_CombatSource.Enemy != null)
                    {// 是否脱战了
                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                        return;
                    }
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                        return;
                    }

                    var mMapComponent = b_MapComponent;

                    C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                    C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(b_CombatSource.TargetEnemy);
                    if (mEndPos.IsSafeArea) return;

                    List<C_FindTheWay2D> mFindPoslist = mMapComponent.FindPathComponent.AStarFindTheWay(mStartPos, mEndPos, b_CombatSource.Config.Ran2, b_CombatSource.Config.Ran2);
                    if (mFindPoslist != null && mFindPoslist.Count > 1)
                    {
                        var mNextNodeTemp = mFindPoslist[0];
                        if (mNextNodeTemp.X == b_CombatSource.UnitData.X && mNextNodeTemp.Y == b_CombatSource.UnitData.Y)
                        {
                            b_CombatSource.Pathlist = new List<C_FindTheWay2D>();
                            for (int i = 1; i < mFindPoslist.Count; i++)
                            {
                                b_CombatSource.Pathlist.Add(mFindPoslist[i]);
                            }
                        }
                        else
                        {
                            b_CombatSource.Pathlist = mFindPoslist;
                        }
                    }

                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                    }
                }
                else if (b_CombatSource.TargetEnemy == null)
                {
                    if (b_CombatSource.MoveSleepTime > b_CurrentTime)
                    {// 移动静默中
                        return;
                    }

                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                        return;
                    }

                    var mMapComponent = b_MapComponent;

                    int mIntervalRadius = b_CombatSource.Config.Ran2;
                    int mOffsetX = Help_RandomHelper.Range(0, mIntervalRadius) - b_CombatSource.Config.Ran;
                    int mOffsetY = Help_RandomHelper.Range(0, mIntervalRadius) - b_CombatSource.Config.Ran;

                    int mEndPosIndexX = b_CombatSource.SourcePosX + mOffsetX;
                    int mEndPosIndexY = b_CombatSource.SourcePosY + mOffsetY;
                    if (mEndPosIndexX < 0) mEndPosIndexX = 0;
                    else if (mEndPosIndexX >= b_MapComponent.MapWidth) mEndPosIndexX = b_MapComponent.MapWidth - 1;
                    if (mEndPosIndexY < 0) mEndPosIndexY = 0;
                    else if (mEndPosIndexY >= b_MapComponent.MapHight) mEndPosIndexY = b_MapComponent.MapHight - 1;

                    if (b_CombatSource.UnitData.X == mEndPosIndexX && b_CombatSource.UnitData.Y == mEndPosIndexY)
                    {// 移动静默1-4秒
                        b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                        return;
                    }
                    else
                    {
                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(mEndPosIndexX, mEndPosIndexY);
                        if (mEndPos.IsSafeArea) return;

                        List<C_FindTheWay2D> mFindPoslist = mMapComponent.FindPathComponent.AStarFindTheWay(mStartPos, mEndPos, Math.Abs(mOffsetX), Math.Abs(mOffsetY));
                        if (mFindPoslist != null && mFindPoslist.Count > 1)
                        {
                            var mNextNodeTemp = mFindPoslist[0];
                            if (mNextNodeTemp.X == b_CombatSource.UnitData.X && mNextNodeTemp.Y == b_CombatSource.UnitData.Y)
                            {
                                b_CombatSource.Pathlist = new List<C_FindTheWay2D>();
                                for (int i = 1; i < mFindPoslist.Count; i++)
                                {
                                    b_CombatSource.Pathlist.Add(mFindPoslist[i]);
                                }
                            }
                            else
                            {
                                b_CombatSource.Pathlist = mFindPoslist;
                            }
                        }
                    }

                    C_FindTheWay2D mNextNode;
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        mNextNode = b_CombatSource.Pathlist[0];

                        if (mNextNode.IsObstacle || mNextNode.IsSafeArea)
                        {// 将要移动的目标点变成了障碍物 则重新寻路
                            b_CombatSource.Pathlist = null;
                            return;
                        }
                    }
                    else
                    {
                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(mEndPosIndexX, mEndPosIndexY);
                        if (mEndPos.IsSafeArea) return;
                        mNextNode = mMapComponent.NearFindTheWay(mStartPos, mEndPos);
                        if (mNextNode == null)
                        {
                            b_CombatSource.Pathlist = null;
                            return;
                        }

                        if (mNextNode.IsObstacle)
                        {// 将要移动的目标点变成了障碍物 则重新寻路
                            return;
                        }

                        b_CombatSource.Pathlist = new List<C_FindTheWay2D>() { mNextNode };
                    }
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                    }
                }
            }
        }
        public static void LateUpdateLogic(this BattleComponent b_Component, Enemy b_CombatSource, long b_CurrentTime, MapComponent b_MapComponent)
        {
            // 本单位阵亡了?
            if (b_CombatSource.IsDeath)
            {
                b_CombatSource.AddCustomComponent<RebirthComponent>();
                b_CombatSource.CurrentMap?.Leave(b_CombatSource);
                return;
            }

            if (b_CombatSource.IsDeath == false) b_CombatSource.RunSyncTaskAction(b_CurrentTime, b_Component);

            // 本单位已经在攻击中?
            if (b_CombatSource.IsAttacking) return;

            if (b_CombatSource.IsCheckState(2)) return;

            if (b_CombatSource.IsCanMove() == false) return;

            if (b_CombatSource.MoveSleepTime > b_CurrentTime)
            {
                return;
            }
            if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
            {
                if (b_CombatSource.MoveNeedTime <= b_CurrentTime)
                {
                    C_FindTheWay2D mStartPos = b_MapComponent.GetFindTheWay2D(b_CombatSource);
                    C_FindTheWay2D mNextNode = b_CombatSource.Pathlist[0];

                    if (mNextNode.IsSafeArea)
                    {
                        b_CombatSource.Pathlist = null;

                        b_CombatSource.MoveSleepTime = b_CurrentTime;
                        b_CombatSource.MoveRestTime = b_CurrentTime;
                        return;
                    }

                    if (mNextNode.IsObstacle == false)
                    {
                        b_CombatSource.Pathlist.Remove(mNextNode);

                        var mTargetEnemy = b_CombatSource.TargetEnemy;
                        if (mTargetEnemy != null)
                        {
                            if (mTargetEnemy.IsDisposeable || mTargetEnemy.IsDeath)
                            {
                                if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                            }
                            else if (b_MapComponent.MapId != mTargetEnemy.UnitData.Index)
                            {
                                if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                b_CombatSource.MoveRestTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                            }
                            else
                            {
                                var mDistance = Vector2.Distance(mNextNode.Vector2Pos, b_MapComponent.GetFindTheWay2D(mTargetEnemy).Vector2Pos);
                                if (10 * mDistance <= b_CombatSource.Config.AR)
                                {// 是否脱战了
                                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                    b_CombatSource.MoveRestTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                                }
                            }
                        }
                        else if (b_CombatSource.Pathlist.Count == 0)
                        {
                            b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                        }

                        b_MapComponent.MoveSendNotice(mStartPos, mNextNode, b_CombatSource);
                    }
                    else
                    {
                        b_CombatSource.Pathlist = null;

                        b_CombatSource.MoveSleepTime = b_CurrentTime;
                        b_CombatSource.MoveRestTime = b_CurrentTime;
                    }
                }
            }
        }

        public static void UpdateLogic(this BattleComponent b_Component, Summoned b_CombatSource, long b_CurrentTime, MapComponent b_MapComponent)
        {
            if (b_CombatSource.GamePlayer.UnitData.Index != b_MapComponent.MapId) return;
            // 本单位阵亡了?
            if (b_CombatSource.IsDeath) return;
            // 本单位已经在攻击中?
            if (b_CombatSource.IsAttacking) return;

            if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
            {

            }
            else
            {
                var mFindTheWay = b_MapComponent.GetFindTheWay2D(b_CombatSource);
                if (mFindTheWay == null)
                {
                    return;
                }

                if (mFindTheWay.IsSafeArea == false)
                {
                    if (b_Component.Logic(b_CombatSource, b_CurrentTime)) return;
                }

                if (b_CombatSource.IsAttacking) return;
                if (b_CombatSource.IsCanMove() == false) return;

                if (mFindTheWay != null && mFindTheWay.IsSafeArea == false)
                {
                    b_Component.WarEnemyTargetUpdate(b_CombatSource);
                    if (b_CombatSource.TargetEnemy != null)
                    {// 朝着目标走
                        if (b_CombatSource.Enemy != null)
                        {// 是否脱战了
                         //LogToolComponent.Error($"1 {b_CombatSource.Config.Name} {b_CombatSource.InstanceId} {10 * Vector2.Distance(new Vector2(b_CombatSource.UnitData.X, b_CombatSource.UnitData.Y), new Vector2(mTargetEnemy.UnitData.X, mTargetEnemy.UnitData.Y))}", false);

                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                            return;
                        }
                        if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                        {
                            b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                            return;
                        }

                        var mMapComponent = b_MapComponent;

                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(b_CombatSource.TargetEnemy);

                        List<C_FindTheWay2D> mFindPoslist = mMapComponent.FindPathComponent.AStarFindTheWay(mStartPos, mEndPos, b_CombatSource.Config.Ran2, b_CombatSource.Config.Ran2);
                        if (mFindPoslist != null && mFindPoslist.Count > 1)
                        {
                            var mNextNodeTemp = mFindPoslist[0];
                            if (mNextNodeTemp.X == b_CombatSource.UnitData.X && mNextNodeTemp.Y == b_CombatSource.UnitData.Y)
                            {
                                b_CombatSource.Pathlist = new List<C_FindTheWay2D>();
                                for (int i = 1; i < mFindPoslist.Count; i++)
                                {
                                    b_CombatSource.Pathlist.Add(mFindPoslist[i]);
                                }
                            }
                            else
                            {
                                b_CombatSource.Pathlist = mFindPoslist;
                            }
                        }

                        if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                        {// 在这里直接跳跃到下一格 防止前端因时间差异停顿
                            var mNextNode = mFindPoslist[0];
                            b_CombatSource.Pathlist.Remove(mNextNode);

                            var mTargetEnemy = b_CombatSource.TargetEnemy;
                            if (mTargetEnemy != null)
                            {
                                var mDistance = Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, b_Component.Parent.GetFindTheWay2D(mTargetEnemy).Vector2Pos);
                                if (10 * mDistance <= b_CombatSource.Config.AR)
                                {// 是否脱战了
                                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                }
                            }
                            else if (b_CombatSource.Pathlist.Count == 0)
                            {
                                b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                            }
                            else
                            {
                                b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                            }

                            b_MapComponent.MoveSendNotice(mStartPos, mNextNode, b_CombatSource);
                        }
                    }
                }
                else if (b_CombatSource.TargetEnemy != null)
                {
                    b_CombatSource.TargetEnemy = null;
                    if (b_CombatSource.Enemy != null)
                    {
                        b_CombatSource.Enemy = null;
                    }
                }
                if (b_CombatSource.TargetEnemy == null)
                {
                    if (b_CombatSource.MoveSleepTime > b_CurrentTime)
                    {// 移动静默中
                        return;
                    }

                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                        return;
                    }

                    var mMapComponent = b_MapComponent;

                    int mIntervalRadius = b_CombatSource.Config.Ran2;
                    int mOffsetX = Help_RandomHelper.Range(0, mIntervalRadius) - b_CombatSource.Config.Ran;
                    int mOffsetY = Help_RandomHelper.Range(0, mIntervalRadius) - b_CombatSource.Config.Ran;

                    int mEndPosIndexX = b_CombatSource.GamePlayer.UnitData.X + mOffsetX;
                    int mEndPosIndexY = b_CombatSource.GamePlayer.UnitData.Y + mOffsetY;
                    if (mEndPosIndexX < 0) mEndPosIndexX = 0;
                    else if (mEndPosIndexX >= b_MapComponent.MapWidth) mEndPosIndexX = b_MapComponent.MapWidth - 1;
                    if (mEndPosIndexY < 0) mEndPosIndexY = 0;
                    else if (mEndPosIndexY >= b_MapComponent.MapHight) mEndPosIndexY = b_MapComponent.MapHight - 1;

                    if (b_CombatSource.UnitData.X == mEndPosIndexX && b_CombatSource.UnitData.Y == mEndPosIndexY)
                    {// 移动静默1-4秒
                        b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                        return;
                    }
                    else
                    {
                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(mEndPosIndexX, mEndPosIndexY);
                        List<C_FindTheWay2D> mFindPoslist = mMapComponent.FindPathComponent.AStarFindTheWay(mStartPos, mEndPos, Math.Abs(mOffsetX), Math.Abs(mOffsetY));
                        if (mFindPoslist != null && mFindPoslist.Count > 1)
                        {
                            var mNextNodeTemp = mFindPoslist[0];
                            if (mNextNodeTemp.X == b_CombatSource.UnitData.X && mNextNodeTemp.Y == b_CombatSource.UnitData.Y)
                            {
                                b_CombatSource.Pathlist = new List<C_FindTheWay2D>();
                                for (int i = 1; i < mFindPoslist.Count; i++)
                                {
                                    b_CombatSource.Pathlist.Add(mFindPoslist[i]);
                                }
                            }
                            else
                            {
                                b_CombatSource.Pathlist = mFindPoslist;
                            }
                        }
                    }

                    C_FindTheWay2D mNextNode;
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        mNextNode = b_CombatSource.Pathlist[0];

                        if (mNextNode.IsObstacle)
                        {// 将要移动的目标点变成了障碍物 则重新寻路
                            b_CombatSource.Pathlist = null;
                            return;
                        }
                    }
                    else
                    {
                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(mEndPosIndexX, mEndPosIndexY);
                        mNextNode = mMapComponent.NearFindTheWay(mStartPos, mEndPos);
                        if (mNextNode == null)
                        {
                            b_CombatSource.Pathlist = null;
                            return;
                        }

                        if (mNextNode.IsObstacle)
                        {// 将要移动的目标点变成了障碍物 则重新寻路
                            return;
                        }

                        b_CombatSource.Pathlist = new List<C_FindTheWay2D>() { mNextNode };
                    }
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {//这里可以
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                    }
                }
            }
        }
        public static void LateUpdateLogic(this BattleComponent b_Component, Summoned b_CombatSource, long b_CurrentTime, MapComponent b_MapComponent)
        {
            // 本单位阵亡了?
            if (b_CombatSource.IsDeath)
            {
                b_CombatSource.IsReallyDeath = true;
                return;
            }
            // 每个单位是独立的，不需要判断所属玩家和自己是不是同一张地图
            // 判断地图建议用唯一id,而不是configId
            //if (b_CombatSource.GamePlayer.UnitData.Index != b_MapComponent.MapId) return;

            if (b_CombatSource.IsDeath == false) b_CombatSource.RunSyncTaskAction(b_CurrentTime, b_Component);

            // 本单位已经在攻击中?
            if (b_CombatSource.IsAttacking) return;

            if (b_CombatSource.IsCanMove() == false) return;

            //if (b_CombatSource.GamePlayer.UnitData.Index != b_MapComponent.MapId) return;

            GamePlayer gamePlayer = b_CombatSource.GamePlayer;
            {// 距离过远拉回
                // 切换地图移到 CombatSourceEnterOrSwitchMap_PetsSwitchMap
                // 不需要关心地图是否变动
                // 直接对比平方，不需要开方
                if ((b_CombatSource.Position - gamePlayer.Position).sqrMagnitude > 12 * 12)
                {
                    // list 使用对象池
                    using (ListComponent<C_FindTheWay2D> posList = ListComponent<C_FindTheWay2D>.Create())
                    {
                        MapComponent currentMap = gamePlayer.CurrentMap;
                        posList.Add(currentMap.GetFindTheWay2D(gamePlayer));

                        int mRepelValue = 1;
                        var mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x + mRepelValue, gamePlayer.Position.y + mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x + mRepelValue, gamePlayer.Position.y - mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x - mRepelValue, gamePlayer.Position.y + mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x - mRepelValue, gamePlayer.Position.y - mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        var mRandomIndex = Help_RandomHelper.Range(0, posList.Count);
                        var mRandomResult = posList[mRandomIndex];

                        b_CombatSource.Pathlist = null;
                        b_CombatSource.Move(mRandomResult.X, mRandomResult.Y, false);
                    }
                }
            }

            if (b_CombatSource.MoveSleepTime > b_CurrentTime)
            {
                return;
            }
            if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
            {
                if (b_CombatSource.MoveNeedTime <= b_CurrentTime)
                {
                    C_FindTheWay2D mStartPos = b_MapComponent.GetFindTheWay2D(b_CombatSource);
                    C_FindTheWay2D mNextNode = b_CombatSource.Pathlist[0];

                    if (mNextNode.IsObstacle == false)
                    {
                        b_CombatSource.Pathlist.Remove(mNextNode);

                        var mTargetEnemy = b_CombatSource.TargetEnemy;
                        if (mTargetEnemy != null)
                        {
                            if (mTargetEnemy.IsDisposeable || mTargetEnemy.IsDeath)
                            {
                                if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                            }
                            else if (b_MapComponent.MapId != mTargetEnemy.UnitData.Index)
                            {
                                if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                b_CombatSource.MoveRestTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                            }
                            else
                            {
                                var mDistance = Vector2.Distance(mNextNode.Vector2Pos, b_Component.Parent.GetFindTheWay2D(mTargetEnemy).Vector2Pos);
                                if (10 * mDistance <= b_CombatSource.Config.AR)
                                {// 是否脱战了
                                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                    b_CombatSource.MoveRestTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                                }
                            }
                        }
                        else if (b_CombatSource.Pathlist.Count == 0)
                        {
                            b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                        }

                        b_MapComponent.MoveSendNotice(mStartPos, mNextNode, b_CombatSource);
                    }
                    else
                    {
                        b_CombatSource.Pathlist = null;

                        b_CombatSource.MoveSleepTime = b_CurrentTime;
                        b_CombatSource.MoveRestTime = b_CurrentTime;
                    }
                }
            }
        }

        public static void UpdateLogic(this BattleComponent b_Component, Pets b_CombatSource, long b_CurrentTime, MapComponent b_MapComponent)
        {
            if (b_CombatSource.GamePlayer.UnitData.Index != b_MapComponent.MapId) return;
            // 本单位阵亡了?
            if (b_CombatSource.IsDeath) return;
            // 本单位已经在攻击中?
            if (b_CombatSource.IsAttacking) return;

            if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
            {

            }
            else
            {
                var mFindTheWay = b_MapComponent.GetFindTheWay2D(b_CombatSource);
                if (mFindTheWay == null)
                {
                    return;
                }

                if (mFindTheWay.IsSafeArea == false)
                {
                    if (b_Component.Logic(b_CombatSource, b_CurrentTime)) return;
                }

                if (b_CombatSource.IsAttacking) return;
                if (b_CombatSource.IsCanMove() == false) return;

                if (mFindTheWay != null && mFindTheWay.IsSafeArea == false)
                {
                    b_Component.WarEnemyTargetUpdate(b_CombatSource);
                    if (b_CombatSource.TargetEnemy != null)
                    {// 朝着目标走
                        if (b_CombatSource.Enemy != null)
                        {// 是否脱战了
                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                            return;
                        }
                        if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                        {
                            b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                            return;
                        }

                        var mMapComponent = b_MapComponent;

                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(b_CombatSource.TargetEnemy);

                        List<C_FindTheWay2D> mFindPoslist = mMapComponent.FindPathComponent.AStarFindTheWay(mStartPos, mEndPos, 6, 6);//b_CombatSource.Config.Ran, b_CombatSource.Config.Ran);
                        if (mFindPoslist != null && mFindPoslist.Count > 1)
                        {
                            var mNextNodeTemp = mFindPoslist[0];
                            if (mNextNodeTemp.X == b_CombatSource.UnitData.X && mNextNodeTemp.Y == b_CombatSource.UnitData.Y)
                            {
                                b_CombatSource.Pathlist = new List<C_FindTheWay2D>();
                                for (int i = 1; i < mFindPoslist.Count; i++)
                                {
                                    b_CombatSource.Pathlist.Add(mFindPoslist[i]);
                                }
                            }
                            else
                            {
                                b_CombatSource.Pathlist = mFindPoslist;
                            }
                        }

                        if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                        {
                            b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                        }
                    }
                }
                else if (b_CombatSource.TargetEnemy != null)
                {
                    b_CombatSource.TargetEnemy = null;
                    if (b_CombatSource.Enemy != null)
                    {
                        b_CombatSource.Enemy = null;
                    }
                }

                if (b_CombatSource.TargetEnemy == null)
                {
                    if (b_CombatSource.MoveSleepTime > b_CurrentTime)
                    {// 移动静默中
                        return;
                    }
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                        return;
                    }
                    int mEndPosIndexX = 0;
                    int mEndPosIndexY = 0;
                    var mMapComponent = b_MapComponent;

                    /*if (Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, b_Component.Parent.GetFindTheWay2D(b_CombatSource.GamePlayer).Vector2Pos) > 5)
                    {
                        mEndPosIndexX = b_CombatSource.GamePlayer.UnitData.X;
                        mEndPosIndexY = b_CombatSource.GamePlayer.UnitData.Y;
                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(b_CombatSource.GamePlayer);

                        List<C_FindTheWay2D> mFindPoslist = mMapComponent.FindPathComponent.AStarFindTheWay(mStartPos, mEndPos, b_CombatSource.Config.Ran, b_CombatSource.Config.Ran);
                        if (mFindPoslist != null && mFindPoslist.Count > 1)
                        {
                            var mNextNodeTemp = mFindPoslist[0];
                            if (mNextNodeTemp.X == b_CombatSource.UnitData.X && mNextNodeTemp.Y == b_CombatSource.UnitData.Y)
                            {
                                b_CombatSource.Pathlist = new List<C_FindTheWay2D>();
                                for (int i = 1; i < mFindPoslist.Count; i++)
                                {
                                    b_CombatSource.Pathlist.Add(mFindPoslist[i]);
                                }
                            }
                            else
                            {
                                b_CombatSource.Pathlist = mFindPoslist;
                            }
                        }
                    }
                    else*/
                    {
                        int mIntervalRadius = b_CombatSource.Config.Ran2;
                        int mOffsetX = Help_RandomHelper.Range(2, mIntervalRadius) - b_CombatSource.Config.Ran;
                        int mOffsetY = Help_RandomHelper.Range(2, mIntervalRadius) - b_CombatSource.Config.Ran;
                        if (mOffsetX == 0 && mOffsetY == 0)
                        {
                            mOffsetX = mIntervalRadius;
                        }
                        int Value_A = b_CombatSource.GamePlayer.UnitData.X;
                        int Value_B = b_CombatSource.GamePlayer.UnitData.Y;

                        mEndPosIndexX = Value_A + mOffsetX;
                        mEndPosIndexY = Value_B + mOffsetY;
                        if (mEndPosIndexX < 0) mEndPosIndexX = 0;
                        else if (mEndPosIndexX >= b_MapComponent.MapWidth) mEndPosIndexX = b_MapComponent.MapWidth - 1;
                        if (mEndPosIndexY < 0) mEndPosIndexY = 0;
                        else if (mEndPosIndexY >= b_MapComponent.MapHight) mEndPosIndexY = b_MapComponent.MapHight - 1;

                        //int Value_C = RandomHelper.RandomNumber(0, b_CombatSource.Config.Ran);
                        //Vector2 mSelfPos1 = new Vector2(b_CombatSource.UnitData.X, b_CombatSource.UnitData.Y);
                        //Vector2 mTargetPos2 = new Vector2(Value_A, Value_B);
                        //if (Vector2.Distance(mSelfPos1, mTargetPos2) < 2 && Value_C > 6)
                        if (b_CombatSource.UnitData.X == mEndPosIndexX && b_CombatSource.UnitData.Y == mEndPosIndexY)
                        {// 移动静默1-4秒
                            b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                            return;
                        }
                        else
                        {
                            C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                            C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(mEndPosIndexX, mEndPosIndexY);

                            List<C_FindTheWay2D> mFindPoslist = mMapComponent.FindPathComponent.AStarFindTheWay(mStartPos, mEndPos, b_CombatSource.Config.Ran, b_CombatSource.Config.Ran);
                            if (mFindPoslist != null && mFindPoslist.Count > 1)
                            {
                                var mNextNodeTemp = mFindPoslist[0];
                                if (mNextNodeTemp.X == b_CombatSource.UnitData.X && mNextNodeTemp.Y == b_CombatSource.UnitData.Y)
                                {
                                    b_CombatSource.Pathlist = new List<C_FindTheWay2D>();
                                    for (int i = 1; i < mFindPoslist.Count; i++)
                                    {
                                        b_CombatSource.Pathlist.Add(mFindPoslist[i]);
                                    }
                                }
                                else
                                {
                                    b_CombatSource.Pathlist = mFindPoslist;
                                }
                            }
                        }
                    }

                    C_FindTheWay2D mNextNode;
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        mNextNode = b_CombatSource.Pathlist[0];

                        if (mNextNode.IsObstacle)
                        {// 将要移动的目标点变成了障碍物 则重新寻路
                            b_CombatSource.Pathlist = null;
                            return;
                        }
                    }
                    else
                    {
                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(mEndPosIndexX, mEndPosIndexY);
                        mNextNode = mMapComponent.NearFindTheWay(mStartPos, mEndPos);
                        if (mNextNode == null)
                        {
                            b_CombatSource.Pathlist = null;
                            return;
                        }

                        if (mNextNode.IsObstacle)
                        {// 将要移动的目标点变成了障碍物 则重新寻路
                            return;
                        }

                        b_CombatSource.Pathlist = new List<C_FindTheWay2D>() { mNextNode };
                    }
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                    }

                }

            }
        }
        public static void LateUpdateLogic(this BattleComponent b_Component, Pets b_CombatSource, long b_CurrentTime, MapComponent b_MapComponent)
        {
            // 每个单位是独立的，不需要判断所属玩家和自己是不是同一张地图
            // 判断地图建议用唯一id,而不是configId
            //if (b_CombatSource.GamePlayer.UnitData.Index != b_MapComponent.MapId) return;

            // 本单位阵亡了?
            if (b_CombatSource.IsDeath)
            {
                // 离开地图后，Update会停止
                b_CombatSource.AddCustomComponent<RebirthComponent>();
                b_CombatSource.CurrentMap?.Leave(b_CombatSource);
                return;
            }

            if (b_CombatSource.IsDeath == false) b_CombatSource.RunSyncTaskAction(b_CurrentTime, b_Component);//自动回复

            GamePlayer gamePlayer = b_CombatSource.GamePlayer;
            if (gamePlayer.CurrentMap != null)
            {
                // 距离过远拉回
                // 切换地图移到 CombatSourceEnterOrSwitchMap_PetsSwitchMap
                // 不需要关心地图是否变动
                // 直接对比平方，不需要开方
                if ((b_CombatSource.Position - gamePlayer.Position).sqrMagnitude > b_CombatSource.Config.Ran * b_CombatSource.Config.Ran)
                {
                    // list 使用对象池
                    using (ListComponent<C_FindTheWay2D> posList = ListComponent<C_FindTheWay2D>.Create())
                    {
                        MapComponent currentMap = gamePlayer.CurrentMap;
                        posList.Add(currentMap.GetFindTheWay2D(gamePlayer));

                        int mRepelValue = 1;
                        var mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x + mRepelValue, gamePlayer.Position.y + mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x + mRepelValue, gamePlayer.Position.y - mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x - mRepelValue, gamePlayer.Position.y + mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x - mRepelValue, gamePlayer.Position.y - mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        var mRandomIndex = Help_RandomHelper.Range(0, posList.Count);
                        var mRandomResult = posList[mRandomIndex];

                        b_CombatSource.Pathlist = null;
                        b_CombatSource.Move(mRandomResult.X, mRandomResult.Y, false);
                    }
                }
            }
           //宠物自动回血
            var ShopInfo = b_CombatSource.GamePlayer?.Player.GetCustomComponent<PlayerShopMallComponent>();
            if (ShopInfo != null && ShopInfo.GetPlayerShopState(DeviationType.MaxMonthlyCard))
            {
                if (b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX) * 0.4 >= b_CombatSource.dBPetsData.PetsHP)
                {
                    b_CombatSource.dBPetsData.PetsHP = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                }
                else if (b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP_MAX) * 0.4 >= b_CombatSource.dBPetsData.PetsMP)
                {
                    b_CombatSource.dBPetsData.PetsMP = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                }
            }
            else
            {
                if (b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX) * 0.4 >= b_CombatSource.dBPetsData.PetsHP)
                {
                    int[] ItemID = new[] { 310002, 310003, 310004 };
                    b_CombatSource.PetsUseItem(ItemID, 0);//保留b_BeAttacker.GamePlayer.PetsUseItem(ItemID);
                }
                else if (b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP_MAX) * 0.4 >= b_CombatSource.dBPetsData.PetsMP)
                {
                    int[] ItemID = new[] { 310005, 310006, 310007 };
                    b_CombatSource.PetsUseItem(ItemID, 1);
                }
            }
            // 本单位已经在攻击中?
            if (b_CombatSource.IsAttacking) return;

            if (b_CombatSource.IsCanMove() == false) return;

            if (b_CombatSource.MoveSleepTime > b_CurrentTime)
            {
                return;
            }
            if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
            {
                if (b_CombatSource.MoveNeedTime <= b_CurrentTime)
                {
                    C_FindTheWay2D mStartPos = b_MapComponent.GetFindTheWay2D(b_CombatSource);
                    C_FindTheWay2D mNextNode = b_CombatSource.Pathlist[0];

                    if (mNextNode.IsObstacle == false)
                    {
                        b_CombatSource.Pathlist.Remove(mNextNode);
                        if (b_CombatSource.Pathlist.Count == 0)
                        {
                            b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                        }

                        var mTargetEnemy = b_CombatSource.TargetEnemy;
                        if (mTargetEnemy != null)
                        {
                            if (mTargetEnemy.IsDisposeable || mTargetEnemy.IsDeath)
                            {
                                if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                            }
                            else if (b_MapComponent.MapId != mTargetEnemy.UnitData.Index)
                            {
                                if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                b_CombatSource.MoveRestTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                            }
                            else
                            {
                                var mDistance = Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, b_Component.Parent.GetFindTheWay2D(mTargetEnemy).Vector2Pos);
                                if (10 * mDistance <= b_CombatSource.Config.AttackDistance)
                                {// 是否脱战了
                                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                    b_CombatSource.MoveRestTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                                }
                            }
                        }
                        else if (b_CombatSource.Pathlist.Count == 0)
                        {
                            b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                        }

                        b_MapComponent.MoveSendNotice(mStartPos, mNextNode, b_CombatSource);
                    }
                    else
                    {
                        b_CombatSource.Pathlist = null;

                        b_CombatSource.MoveSleepTime = b_CurrentTime;
                        b_CombatSource.MoveRestTime = b_CurrentTime;
                    }
                }
            }
        }


        public static void UpdateLogic(this BattleComponent b_Component, HolyteacherSummoned b_CombatSource, long b_CurrentTime, MapComponent b_MapComponent)
        {
            if (b_CombatSource.GamePlayer.UnitData.Index != b_MapComponent.MapId) return;
            // 本单位阵亡了?
            if (b_CombatSource.IsDeath) return;
            // 本单位已经在攻击中?
            if (b_CombatSource.IsAttacking) return;

            if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
            {

            }
            else
            {
                var mFindTheWay = b_MapComponent.GetFindTheWay2D(b_CombatSource);
                if (mFindTheWay == null)
                {
                    return;
                }

                if (mFindTheWay.IsSafeArea == false && false)
                {
                    if (b_Component.Logic(b_CombatSource, b_CurrentTime)) return;
                }

                if (b_CombatSource.IsAttacking) return;
                if (b_CombatSource.IsCanMove() == false) return;

                if (mFindTheWay != null && mFindTheWay.IsSafeArea == false)
                {
                    //b_Component.WarEnemyTargetUpdate(b_CombatSource);
                    if (b_CombatSource.TargetEnemy != null)
                    {// 朝着目标走
                        if (b_CombatSource.Enemy != null)
                        {// 是否脱战了
                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                            return;
                        }
                        if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                        {
                            b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                            return;
                        }

                        var mMapComponent = b_MapComponent;

                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(b_CombatSource.TargetEnemy);

                        List<C_FindTheWay2D> mFindPoslist = mMapComponent.FindPathComponent.AStarFindTheWay(mStartPos, mEndPos, b_CombatSource.Config.Ran2, b_CombatSource.Config.Ran2);
                        if (mFindPoslist != null && mFindPoslist.Count > 1)
                        {
                            var mNextNodeTemp = mFindPoslist[0];
                            if (mNextNodeTemp.X == b_CombatSource.UnitData.X && mNextNodeTemp.Y == b_CombatSource.UnitData.Y)
                            {
                                b_CombatSource.Pathlist = new List<C_FindTheWay2D>();
                                for (int i = 1; i < mFindPoslist.Count; i++)
                                {
                                    b_CombatSource.Pathlist.Add(mFindPoslist[i]);
                                }
                            }
                            else
                            {
                                b_CombatSource.Pathlist = mFindPoslist;
                            }
                        }

                        if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                        {// 在这里直接跳跃到下一格 防止前端因时间差异停顿
                            var mNextNode = mFindPoslist[0];
                            b_CombatSource.Pathlist.Remove(mNextNode);

                            var mTargetEnemy = b_CombatSource.TargetEnemy;
                            if (mTargetEnemy != null)
                            {
                                var mDistance = Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, b_Component.Parent.GetFindTheWay2D(mTargetEnemy).Vector2Pos);
                                if (10 * mDistance <= b_CombatSource.Config.AR)
                                {// 是否脱战了
                                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                }
                            }
                            else if (b_CombatSource.Pathlist.Count == 0)
                            {
                                b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                            }
                            else
                            {
                                b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                            }

                            b_MapComponent.MoveSendNotice(mStartPos, mNextNode, b_CombatSource);
                        }
                    }
                }
                else if (b_CombatSource.TargetEnemy != null)
                {
                    b_CombatSource.TargetEnemy = null;
                    if (b_CombatSource.Enemy != null)
                    {
                        b_CombatSource.Enemy = null;
                    }
                }
                if (b_CombatSource.TargetEnemy == null)
                {
                    if (b_CombatSource.MoveSleepTime > b_CurrentTime)
                    {// 移动静默中
                        return;
                    }

                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                        return;
                    }

                    var mMapComponent = b_MapComponent;

                    int mIntervalRadius = b_CombatSource.Config.Ran2;
                    int mOffsetX = Help_RandomHelper.Range(0, mIntervalRadius) - b_CombatSource.Config.Ran;
                    int mOffsetY = Help_RandomHelper.Range(0, mIntervalRadius) - b_CombatSource.Config.Ran;

                    int mEndPosIndexX = b_CombatSource.GamePlayer.UnitData.X + mOffsetX;
                    int mEndPosIndexY = b_CombatSource.GamePlayer.UnitData.Y + mOffsetY;
                    if (mEndPosIndexX < 0) mEndPosIndexX = 0;
                    else if (mEndPosIndexX >= b_MapComponent.MapWidth) mEndPosIndexX = b_MapComponent.MapWidth - 1;
                    if (mEndPosIndexY < 0) mEndPosIndexY = 0;
                    else if (mEndPosIndexY >= b_MapComponent.MapHight) mEndPosIndexY = b_MapComponent.MapHight - 1;

                    if (b_CombatSource.UnitData.X == mEndPosIndexX && b_CombatSource.UnitData.Y == mEndPosIndexY)
                    {// 移动静默1-4秒
                        b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                        return;
                    }
                    else
                    {
                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(mEndPosIndexX, mEndPosIndexY);
                        List<C_FindTheWay2D> mFindPoslist = mMapComponent.FindPathComponent.AStarFindTheWay(mStartPos, mEndPos, Math.Abs(mOffsetX), Math.Abs(mOffsetY));
                        if (mFindPoslist != null && mFindPoslist.Count > 1)
                        {
                            var mNextNodeTemp = mFindPoslist[0];
                            if (mNextNodeTemp.X == b_CombatSource.UnitData.X && mNextNodeTemp.Y == b_CombatSource.UnitData.Y)
                            {
                                b_CombatSource.Pathlist = new List<C_FindTheWay2D>();
                                for (int i = 1; i < mFindPoslist.Count; i++)
                                {
                                    b_CombatSource.Pathlist.Add(mFindPoslist[i]);
                                }
                            }
                            else
                            {
                                b_CombatSource.Pathlist = mFindPoslist;
                            }
                        }
                    }

                    C_FindTheWay2D mNextNode;
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {
                        mNextNode = b_CombatSource.Pathlist[0];

                        if (mNextNode.IsObstacle)
                        {// 将要移动的目标点变成了障碍物 则重新寻路
                            b_CombatSource.Pathlist = null;
                            return;
                        }
                    }
                    else
                    {
                        C_FindTheWay2D mStartPos = mMapComponent.GetFindTheWay2D(b_CombatSource);
                        C_FindTheWay2D mEndPos = mMapComponent.GetFindTheWay2D(mEndPosIndexX, mEndPosIndexY);
                        mNextNode = mMapComponent.NearFindTheWay(mStartPos, mEndPos);
                        if (mNextNode == null)
                        {
                            b_CombatSource.Pathlist = null;
                            return;
                        }

                        if (mNextNode.IsObstacle)
                        {// 将要移动的目标点变成了障碍物 则重新寻路
                            return;
                        }

                        b_CombatSource.Pathlist = new List<C_FindTheWay2D>() { mNextNode };
                    }
                    if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
                    {//这里可以
                        b_CombatSource.MoveNeedTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                    }
                }
            }
        }
        public static void LateUpdateLogic(this BattleComponent b_Component, HolyteacherSummoned b_CombatSource, long b_CurrentTime, MapComponent b_MapComponent)
        {
            // 本单位阵亡了?
            if (b_CombatSource.IsDeath)
            {
                b_CombatSource.CurrentMap?.Leave(b_CombatSource);
                return;
            }

            //if (b_CombatSource.IsDeath == false) b_CombatSource.RunSyncTaskAction(b_CurrentTime, b_Component);//自动回复

            GamePlayer gamePlayer = b_CombatSource.GamePlayer;
            if (gamePlayer.CurrentMap != null)
            {
                // 距离过远拉回
                // 切换地图移到 CombatSourceEnterOrSwitchMap_PetsSwitchMap
                // 不需要关心地图是否变动
                // 直接对比平方，不需要开方
                if ((b_CombatSource.Position - gamePlayer.Position).sqrMagnitude > b_CombatSource.Config.Ran * b_CombatSource.Config.Ran)
                {
                    // list 使用对象池
                    using (ListComponent<C_FindTheWay2D> posList = ListComponent<C_FindTheWay2D>.Create())
                    {
                        MapComponent currentMap = gamePlayer.CurrentMap;
                        posList.Add(currentMap.GetFindTheWay2D(gamePlayer));

                        int mRepelValue = 1;
                        var mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x + mRepelValue, gamePlayer.Position.y + mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x + mRepelValue, gamePlayer.Position.y - mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x - mRepelValue, gamePlayer.Position.y + mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        mCurrentTemp = currentMap.GetFindTheWay2D(gamePlayer.Position.x - mRepelValue, gamePlayer.Position.y - mRepelValue);
                        if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false) posList.Add(mCurrentTemp);

                        var mRandomIndex = Help_RandomHelper.Range(0, posList.Count);
                        var mRandomResult = posList[mRandomIndex];

                        b_CombatSource.Pathlist = null;
                        b_CombatSource.Move(mRandomResult.X, mRandomResult.Y, false);
                    }
                }
            }

            // 本单位已经在攻击中?
            if (b_CombatSource.IsAttacking) return;

            if (b_CombatSource.IsCanMove() == false) return;

            if (b_CombatSource.MoveSleepTime > b_CurrentTime)
            {
                return;
            }
            if (b_CombatSource.Pathlist != null && b_CombatSource.Pathlist.Count > 0)
            {
                if (b_CombatSource.MoveNeedTime <= b_CurrentTime)
                {
                    C_FindTheWay2D mStartPos = b_MapComponent.GetFindTheWay2D(b_CombatSource);
                    C_FindTheWay2D mNextNode = b_CombatSource.Pathlist[0];

                    if (mNextNode.IsObstacle == false)
                    {
                        b_CombatSource.Pathlist.Remove(mNextNode);

                        var mTargetEnemy = b_CombatSource.TargetEnemy;
                        if (mTargetEnemy != null)
                        {
                            if (mTargetEnemy.IsDisposeable || mTargetEnemy.IsDeath)
                            {
                                if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                            }
                            else if (b_MapComponent.MapId != mTargetEnemy.UnitData.Index)
                            {
                                if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                                b_CombatSource.MoveRestTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                            }
                            else
                            {
                                var mDistance = Vector2.Distance(mNextNode.Vector2Pos, b_Component.Parent.GetFindTheWay2D(mTargetEnemy).Vector2Pos);
                                if (10 * mDistance <= b_CombatSource.Config.AR)
                                {// 是否脱战了
                                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;

                                    b_CombatSource.MoveRestTime = b_CurrentTime + b_CombatSource.Config.MoSpeed;
                                }
                            }
                        }
                        else if (b_CombatSource.Pathlist.Count == 0)
                        {
                            b_CombatSource.MoveSleepTime = b_CurrentTime + Help_RandomHelper.Range(1, 4) * 1000;
                        }

                        b_MapComponent.MoveSendNotice(mStartPos, mNextNode, b_CombatSource);
                    }
                    else
                    {
                        b_CombatSource.Pathlist = null;

                        b_CombatSource.MoveSleepTime = b_CurrentTime;
                        b_CombatSource.MoveRestTime = b_CurrentTime;
                    }
                }
            }
        }
    }
}
