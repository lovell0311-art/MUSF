using ETModel;
using NLog.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using TencentCloud.Bri.V20190328.Models;
using CustomFrameWork;
using UnityEngine;
using Log = CustomFrameWork.Log;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace ETHotfix
{
    /// <summary>
    /// 单个地图
    /// </summary>
    [FriendOf(typeof(DBPlayerUnitData))]
    [FriendOf(typeof(CombatSource))]
    [FriendOf(typeof(C_FindTheWay2D))]
    public static class MapComponentSystem_Notice
    {
        // 没有给自身发送
        // 没有位移设置

        public static void MoveSendNotice(this MapComponent b_Component, C_FindTheWay2D b_StartFindTheWay, C_FindTheWay2D b_TargetFindTheWay, CombatSource b_CombatSource, bool b_NeedMove = true,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            if (b_TargetFindTheWay == null)
            {
                // 离开地图
                b_StartFindTheWay.Map.Leave(b_CombatSource,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
                return;
            }
            if (b_CombatSource.CurrentCell == null)
            {
                // 进入地图
                if (b_CombatSource.Identity == E_Identity.Hero)
                {
                    if (((GamePlayer)b_CombatSource).Player.OnlineStatus == EOnlineStatus.Offline)
                    {
                        // 正在下线，禁止进入地图
                        return;
                    }
                }
                b_TargetFindTheWay.Map.Enter(b_CombatSource, b_TargetFindTheWay.X, b_TargetFindTheWay.Y, false,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
                return;
            }

            if (b_TargetFindTheWay.Map.Id == b_CombatSource.CurrentMap.Id)
            {
                // 移动
                b_TargetFindTheWay.Map.Move(b_CombatSource, b_TargetFindTheWay.X, b_TargetFindTheWay.Y, b_NeedMove);
            }
            else
            {
                // 切换地图
                b_TargetFindTheWay.Map.Switch(b_CombatSource, b_TargetFindTheWay.X, b_TargetFindTheWay.Y,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }


        }

        public static void MoveSendNotice(this MapComponent b_Component, C_FindTheWay2D b_StartFindTheWay, C_FindTheWay2D b_TargetFindTheWay, GameNpc b_CombatSource, bool b_NeedMove = true)
        {
            List<int> leaveAroundField = null;
            List<int> currentAroundField = null;
            List<int> intoAroundField = null;

            MapCellAreaComponent mSourceCellField = null;
            MapCellAreaComponent mTargetCellField = null;

            if (b_StartFindTheWay != null)
            {
                //b_StartFindTheWay.IsObstacle = false;
                mSourceCellField = b_StartFindTheWay.Map.GetMapCellField(b_StartFindTheWay);
            }
            //b_TargetFindTheWay.IsObstacle = true;
            mTargetCellField = b_TargetFindTheWay.Map.GetMapCellField(b_TargetFindTheWay);
            if (mTargetCellField == null)
            {
                return;
            }

            //b_CombatSource.UnitData.Index = b_TargetFindTheWay.Map.MapId;
            //b_CombatSource.UnitData.X = b_TargetFindTheWay.X;
            //b_CombatSource.UnitData.Y = b_TargetFindTheWay.Y;
            //if (b_NeedMove) b_CombatSource.MoveNeedTime = b_TargetFindTheWay.Map.GetCustomComponent<BattleComponent>().CurrentTimeTick + b_CombatSource.Config.MoSpeed;

            if (b_StartFindTheWay == null)
            {// 可能是传送
                leaveAroundField = null;
                currentAroundField = null;
                intoAroundField = mTargetCellField.AroundField;

                mTargetCellField.FieldNpcDic[b_CombatSource.Id] = b_CombatSource;
            }
            else if (b_StartFindTheWay.Map.Id != b_TargetFindTheWay.Map.Id)
            {// 不是一个地图 可能是传送
                leaveAroundField = mSourceCellField.AroundField;
                currentAroundField = null;
                intoAroundField = mTargetCellField.AroundField;

                if (mSourceCellField.Id != mTargetCellField.Id)
                {
                    if (mSourceCellField.FieldNpcDic.ContainsKey(b_CombatSource.Id))
                        mSourceCellField.FieldNpcDic.Remove(b_CombatSource.Id);

                    mTargetCellField.FieldNpcDic[b_CombatSource.Id] = b_CombatSource;
                }
                else
                {
                    if (mTargetCellField.FieldNpcDic.ContainsKey(b_CombatSource.Id) == false)
                    {
                        mTargetCellField.FieldNpcDic[b_CombatSource.Id] = b_CombatSource;
                    }
                }
            }
            else
            {
                var aroundFieldSource = mSourceCellField.AroundField;
                var targetFieldSource = mTargetCellField.AroundField;

                leaveAroundField = aroundFieldSource.Except(targetFieldSource).ToList();
                currentAroundField = aroundFieldSource.Intersect(targetFieldSource).ToList();
                intoAroundField = targetFieldSource.Except(aroundFieldSource).ToList();

                if (mSourceCellField.Id != mTargetCellField.Id)
                {
                    if (mSourceCellField.FieldNpcDic.ContainsKey(b_CombatSource.Id))
                        mSourceCellField.FieldNpcDic.Remove(b_CombatSource.Id);

                    mTargetCellField.FieldNpcDic[b_CombatSource.Id] = b_CombatSource;
                }
                else
                {
                    if (mTargetCellField.FieldNpcDic.ContainsKey(b_CombatSource.Id) == false)
                    {
                        mTargetCellField.FieldNpcDic[b_CombatSource.Id] = b_CombatSource;
                    }
                }
            }

            if (leaveAroundField != null && leaveAroundField.Count > 0)
            {// 告诉 离开了
                if (mSourceCellField != null)
                {
                    G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                    mMoveMessageNotice.X = b_StartFindTheWay.X;
                    mMoveMessageNotice.Y = b_StartFindTheWay.Y;
                    mMoveMessageNotice.GameUserId = b_CombatSource.Id;
                    mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                    mMoveMessageNotice.ViewId = 2;
                    mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                    for (int i = 0, len = leaveAroundField.Count; i < len; i++)
                    {
                        int mleaveAroundFieldIndex = leaveAroundField[i];

                        if (mSourceCellField.AroundFieldDic.TryGetValue(mleaveAroundFieldIndex, out var mTemp))
                        {
                            mTemp.RadioBroadcast(mMoveMessageNotice);
                        }
                    }
                }
            }
            if (currentAroundField != null && currentAroundField.Count > 0)
            {// 告诉 移动了
                if (mTargetCellField != null)
                {
                    G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                    mMoveMessageNotice.X = b_TargetFindTheWay.X;
                    mMoveMessageNotice.Y = b_TargetFindTheWay.Y;
                    mMoveMessageNotice.GameUserId = b_CombatSource.Id;
                    mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                    mMoveMessageNotice.ViewId = 0;
                    mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                    for (int i = 0, len = currentAroundField.Count; i < len; i++)
                    {
                        int mcurrentAroundFieldIndex = currentAroundField[i];

                        if (mTargetCellField.AroundFieldDic.TryGetValue(mcurrentAroundFieldIndex, out var mTemp))
                        {
                            mTemp.RadioBroadcast(mMoveMessageNotice);
                        }
                    }
                }
            }
            if (intoAroundField != null && intoAroundField.Count > 0)
            {// 告诉 进来了  通知该玩家 这区域中原本有什么
                if (mTargetCellField != null)
                {
                    G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                    mMoveMessageNotice.X = b_TargetFindTheWay.X;
                    mMoveMessageNotice.Y = b_TargetFindTheWay.Y;
                    mMoveMessageNotice.GameUserId = b_CombatSource.Id;
                    mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                    mMoveMessageNotice.ViewId = 1;
                    mMoveMessageNotice.MapId = b_TargetFindTheWay.Map.MapId;
                    if (b_StartFindTheWay != null)
                    {
                        mMoveMessageNotice.SourceX = b_StartFindTheWay.X;
                        mMoveMessageNotice.SourceY = b_StartFindTheWay.Y;
                    }
                    else
                    {
                        mMoveMessageNotice.SourceX = b_TargetFindTheWay.X;
                        mMoveMessageNotice.SourceY = b_TargetFindTheWay.Y;
                    }
                    mMoveMessageNotice.ModelId = b_CombatSource.Config.Id;
                    mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;

                    for (int i = 0, len = intoAroundField.Count; i < len; i++)
                    {
                        int mintoAroundFieldIndex = intoAroundField[i];

                        if (mTargetCellField.AroundFieldDic.TryGetValue(mintoAroundFieldIndex, out var mTemp))
                        {
                            mTemp.RadioBroadcast(mMoveMessageNotice);
                        }
                    }
                }
            }
        }


        public static void QuitMap(this MapComponent b_Component, C_FindTheWay2D b_StartFindTheWay, CombatSource b_CombatSource,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            b_StartFindTheWay.Map.Leave(b_CombatSource,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
        }

        public static void QuitMap(this MapComponent b_Component, C_FindTheWay2D b_StartFindTheWay, GameNpc b_CombatSource)
        {
            if (b_StartFindTheWay != null)
            {
                // 地图分域
                var mCellFieldTemp = b_StartFindTheWay.Map.GetMapCellField(b_StartFindTheWay);
                if (mCellFieldTemp != null)
                {
                    if (mCellFieldTemp.FieldNpcDic.ContainsKey(b_CombatSource.Id))
                    {
                        mCellFieldTemp.FieldNpcDic.Remove(b_CombatSource.Id);
                    }
                }
            }
        }




        public static void Enter(this MapComponent b_Component, CombatSource b_CombatSource, int b_PosX, int b_PosY, bool notEvent = false,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            if (b_CombatSource.CurrentCell != null)
            {
                throw new Exception($"重复进入地图 instanceId:{b_CombatSource.InstanceId} oldMapId:{b_CombatSource.CurrentMap.MapId} newMapId:{b_Component.MapId}");
            }
            C_FindTheWay2D targetPos = b_Component.GetFindTheWay2D(b_PosX, b_PosY);
            if (targetPos == null)
            {
                throw new Exception($"无法进入目标点 instanceId:{b_CombatSource.InstanceId} mapId:{b_Component.MapId} x:{b_PosX} y:{b_PosY}");
            }
            MapCellAreaComponent targetCell = targetPos.MapField;


            List<int> intoAroundField = targetCell.AroundField;

            targetCell.AddCombatSource(b_CombatSource);
            targetPos.AddCombatSource(b_CombatSource);

            if (b_CombatSource.OpenObstacle)
            {
                // 启用碰撞
                targetPos.obstacleCount++;
            }

            b_CombatSource.currentCell = targetCell;
            b_CombatSource.UnitData.index = b_Component.MapId;
            b_CombatSource.UnitData.x = targetPos.X;
            b_CombatSource.UnitData.y = targetPos.Y;
            b_CombatSource.position.x = targetPos.X;
            b_CombatSource.position.y = targetPos.Y;


            switch (b_CombatSource.Identity)
            {
                case E_Identity.Hero:
                    ((GamePlayer)b_CombatSource).Player.Send(new G2C_MovePos_notice()
                    {
                        UnitType = (int)E_Identity.Hero,
                        GameUserId = b_CombatSource.InstanceId,
                        MapId = b_CombatSource.UnitData.Index,
                        X = b_CombatSource.UnitData.X,
                        Y = b_CombatSource.UnitData.Y,
                        Angle = b_CombatSource.UnitData.Angle,
                        IsNeedMove = 0,
                        Title = ((GamePlayer)b_CombatSource).Data.Title,
                        WallTitle = ((GamePlayer)b_CombatSource).Data.WallTile,
                        ReincarnateCnt = ((GamePlayer)b_CombatSource).Data.ReincarnateCnt
                    });
                    CellChanged((GamePlayer)b_CombatSource, null, null, null, null, intoAroundField);
                    b_CombatSource.AddCustomComponent<UpdateFrameComponent>();
                    break;
                case E_Identity.Enemy:
                    {
                        CellChanged((Enemy)b_CombatSource, null, null, null, null, intoAroundField);
                        if (targetCell.obServerCount > 0)
                        {
                            b_CombatSource.AddCustomComponent<UpdateFrameComponent>();
                        }
                    }
                    break;
                case E_Identity.Pet:
                    {
                        CellChanged((Pets)b_CombatSource, null, null, null, null, intoAroundField);
                        b_CombatSource.AddCustomComponent<UpdateFrameComponent>();
                    }
                    break;
                case E_Identity.Summoned:
                    CellChanged((Summoned)b_CombatSource, null, null, null, null, intoAroundField);
                    b_CombatSource.AddCustomComponent<UpdateFrameComponent>();
                    break;
                case E_Identity.HolyteacherSummoned:
                    CellChanged((HolyteacherSummoned)b_CombatSource, null, null, null, null, intoAroundField);
                    b_CombatSource.AddCustomComponent<UpdateFrameComponent>();
                    break;
            }


            if (b_CombatSource.Identity == E_Identity.Hero)
            {
                Log.PLog("Map", $"a:{((GamePlayer)b_CombatSource).Player.UserId} r:{b_CombatSource.InstanceId} 进入地图 {b_Component.Id}:{b_Component.MapId}",
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }
            else if (b_CombatSource.Identity == E_Identity.Pet)
            {
                Log.PLog("Map", $"pets:{b_CombatSource.InstanceId} 进入地图 {b_Component.Id}:{b_Component.MapId}",
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }
            else if(b_CombatSource.Identity == E_Identity.Summoned)
            {
                Log.PLog("Map", $"summoned:{b_CombatSource.InstanceId} 进入地图 {b_Component.Id}:{b_Component.MapId}",
                     callerLineNumber,
                     callerMemberName,
                     callerFilePath);
            }

            if(notEvent == false)
            {
                // 发布 CombatSourceEnterOrSwitchMap 事件
                ETModel.EventType.CombatSourceEnterOrSwitchMap.Instance.combatSource = b_CombatSource;
                ETModel.EventType.CombatSourceEnterOrSwitchMap.Instance.oldMap = null;
                ETModel.EventType.CombatSourceEnterOrSwitchMap.Instance.newMap = b_Component;
                Root.EventSystem.OnRun("CombatSourceEnterOrSwitchMap", ETModel.EventType.CombatSourceEnterOrSwitchMap.Instance);
            }
        }

        public static void Enter(this MapComponent b_Component, CombatSource b_CombatSource, Vector2Int b_Pos, bool notEvent = false,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            Enter(b_Component, b_CombatSource, b_Pos.x, b_Pos.y, notEvent,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
        }

        public static void Move(this MapComponent b_Component, CombatSource b_CombatSource, int b_PosX, int b_PosY, bool b_NeedMove = true)
        {
            if (b_CombatSource.CurrentCell == null)
            {
                throw new Exception($"还未进入地图 instanceId:{b_CombatSource.InstanceId}");
            }
            if (b_CombatSource.CurrentMap.Id != b_Component.Id)
            {
                throw new Exception($"操作的不是同一张地图 instanceId:{b_CombatSource.InstanceId} map.MapId:{b_Component.MapId}:{b_CombatSource.CurrentMap.MapId} map.Id:{b_Component.Id}:{b_CombatSource.CurrentMap.Id}");
            }
            C_FindTheWay2D targetPos = b_Component.GetFindTheWay2D(b_PosX, b_PosY);
            if (targetPos == null)
            {
                throw new Exception($"无法进入目标点 instanceId:{b_CombatSource.InstanceId} mapId:{b_Component.MapId} x:{b_PosX} y:{b_PosY}");
            }

            C_FindTheWay2D startPos = b_Component.GetFindTheWay2D(b_CombatSource.UnitData.X, b_CombatSource.UnitData.Y);

            MapCellAreaComponent sourceCell = startPos.MapField;
            MapCellAreaComponent targetCell = targetPos.MapField;

            var aroundFieldSource = sourceCell.AroundField;
            var targetFieldSource = targetCell.AroundField;

            CombatSource.leaveAroundField.Clear();
            CombatSource.currentAroundField.Clear();
            CombatSource.intoAroundField.Clear();

            foreach (int v in aroundFieldSource.Except(targetFieldSource)) CombatSource.leaveAroundField.Add(v);
            foreach (int v in aroundFieldSource.Intersect(targetFieldSource)) CombatSource.currentAroundField.Add(v);
            foreach (int v in targetFieldSource.Except(aroundFieldSource)) CombatSource.intoAroundField.Add(v);


            if (sourceCell.Id != targetCell.Id)
            {
                sourceCell.RemoveCombatSource(b_CombatSource);
                targetCell.AddCombatSource(b_CombatSource);

                b_CombatSource.currentCell = targetCell;
            }

            if (startPos.Id != targetPos.Id)
            {
                startPos.RemoveCombatSource(b_CombatSource);
                targetPos.AddCombatSource(b_CombatSource);

                if (b_CombatSource.OpenObstacle)
                {
                    // 启用碰撞
                    startPos.obstacleCount--;
                    targetPos.obstacleCount++;
                }

                b_CombatSource.UnitData.x = targetPos.X;
                b_CombatSource.UnitData.y = targetPos.Y;
                b_CombatSource.position.x = targetPos.X;
                b_CombatSource.position.y = targetPos.Y;
            }



            switch (b_CombatSource.Identity)
            {
                case E_Identity.Hero:
                    if (b_NeedMove == false)
                    {
                        // 同地图传送
                        ((GamePlayer)b_CombatSource).Player.Send(new G2C_MovePos_notice()
                        {
                            UnitType = (int)E_Identity.Hero,
                            GameUserId = b_CombatSource.InstanceId,
                            MapId = b_CombatSource.UnitData.Index,
                            X = b_CombatSource.UnitData.X,
                            Y = b_CombatSource.UnitData.Y,
                            Angle = b_CombatSource.UnitData.Angle,
                            IsNeedMove = 1
                        });
                    }
                    CellChanged((GamePlayer)b_CombatSource, startPos, sourceCell, CombatSource.leaveAroundField, CombatSource.currentAroundField, CombatSource.intoAroundField, b_NeedMove);
                    break;
                case E_Identity.Enemy:
                    CellChanged((Enemy)b_CombatSource, startPos, sourceCell, CombatSource.leaveAroundField, CombatSource.currentAroundField, CombatSource.intoAroundField, b_NeedMove);
                    break;
                case E_Identity.Pet:
                    CellChanged((Pets)b_CombatSource, startPos, sourceCell, CombatSource.leaveAroundField, CombatSource.currentAroundField, CombatSource.intoAroundField, b_NeedMove);
                    break;
                case E_Identity.Summoned:
                    CellChanged((Summoned)b_CombatSource, startPos, sourceCell, CombatSource.leaveAroundField, CombatSource.currentAroundField, CombatSource.intoAroundField, b_NeedMove);
                    break;
                case E_Identity.HolyteacherSummoned:
                    CellChanged((HolyteacherSummoned)b_CombatSource, startPos, sourceCell, CombatSource.leaveAroundField, CombatSource.currentAroundField, CombatSource.intoAroundField, b_NeedMove);
                    break;
            }

            if (sourceCell.Id != targetCell.Id)
            {
                // TODO 检查是否要添加或移除 UpdateFrameComponent
                if (sourceCell.obServerCount > 0)
                {
                    if (targetCell.obServerCount <= 0)
                    {
                        b_CombatSource.RemoveCustomComponent<UpdateFrameComponent>();
                    }
                }
                else if (targetCell.obServerCount > 0)
                {
                    b_CombatSource.AddCustomComponent<UpdateFrameComponent>();
                }
            }
        }

        public static void Move(this MapComponent b_Component, CombatSource b_CombatSource, Vector2Int b_Pos, bool b_NeedMove = true)
        {
            Move(b_Component, b_CombatSource, b_Pos.x, b_Pos.y, b_NeedMove);
        }

        public static void Leave(this MapComponent b_Component, CombatSource b_CombatSource,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            if (b_CombatSource.CurrentCell == null)
            {
                // 还未进入地图
                return;
            }
            if (b_CombatSource.CurrentMap.Id != b_Component.Id)
            {
                throw new Exception($"操作的不是同一张地图 instanceId:{b_CombatSource.InstanceId} map.MapId:{b_Component.MapId}:{b_CombatSource.CurrentMap.MapId} map.Id:{b_Component.Id}:{b_CombatSource.CurrentMap.Id}");
            }

            C_FindTheWay2D startPos = b_Component.GetFindTheWay2D(b_CombatSource.UnitData.X, b_CombatSource.UnitData.Y);

            MapCellAreaComponent sourceCell = startPos.MapField;

            List<int> leaveAroundField = sourceCell.AroundField;

            sourceCell.RemoveCombatSource(b_CombatSource);
            startPos.RemoveCombatSource(b_CombatSource);

            if (b_CombatSource.OpenObstacle)
            {
                // 启用碰撞
                startPos.obstacleCount--;
            }

            b_CombatSource.currentCell = null;
            b_CombatSource.position.x = 0;
            b_CombatSource.position.y = 0;

            switch (b_CombatSource.Identity)
            {
                case E_Identity.Hero:
                    CellChanged((GamePlayer)b_CombatSource, startPos, sourceCell, leaveAroundField, null, null);
                    b_CombatSource.RemoveCustomComponent<UpdateFrameComponent>();
                    break;
                case E_Identity.Enemy:
                    CellChanged((Enemy)b_CombatSource, startPos, sourceCell, leaveAroundField, null, null);
                    if (sourceCell.obServerCount > 0)
                    {
                        b_CombatSource.RemoveCustomComponent<UpdateFrameComponent>();
                    }
                    break;
                case E_Identity.Pet:
                    CellChanged((Pets)b_CombatSource, startPos, sourceCell, leaveAroundField, null, null);
                    b_CombatSource.RemoveCustomComponent<UpdateFrameComponent>();
                    break;
                case E_Identity.Summoned:
                    CellChanged((Summoned)b_CombatSource, startPos, sourceCell, leaveAroundField, null, null);
                    b_CombatSource.RemoveCustomComponent<UpdateFrameComponent>();
                    break;
                case E_Identity.HolyteacherSummoned:
                    CellChanged((HolyteacherSummoned)b_CombatSource, startPos, sourceCell, leaveAroundField, null, null);
                    b_CombatSource.RemoveCustomComponent<UpdateFrameComponent>();
                    break;
            }

            if (b_CombatSource.Identity == E_Identity.Hero)
            {
                Log.PLog("Map", $"a:{((GamePlayer)b_CombatSource).Player.UserId} r:{b_CombatSource.InstanceId} 离开地图 {b_Component.Id}:{b_Component.MapId}",
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }
            else if (b_CombatSource.Identity == E_Identity.Pet)
            {
                Log.PLog("Map", $"pets:{b_CombatSource.InstanceId} 离开地图 {b_Component.Id}:{b_Component.MapId}",
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }
            else if (b_CombatSource.Identity == E_Identity.Summoned)
            {
                Log.PLog("Map", $"summoned:{b_CombatSource.InstanceId} 离开地图 {b_Component.Id}:{b_Component.MapId}",
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            }

            // 发布 CombatSourceLeaveMap 事件
            ETModel.EventType.CombatSourceLeaveMap.Instance.combatSource = b_CombatSource;
            ETModel.EventType.CombatSourceLeaveMap.Instance.leavedMap = b_Component;
            Root.EventSystem.OnRun("CombatSourceLeaveMap", ETModel.EventType.CombatSourceLeaveMap.Instance);
        }

        public static void Switch(this MapComponent b_Component, CombatSource b_CombatSource, int b_PosX, int b_PosY,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            if (b_CombatSource.CurrentCell == null)
            {
                throw new Exception($"还未进入地图 instanceId:{b_CombatSource.InstanceId}");
            }
            C_FindTheWay2D targetPos = b_Component.GetFindTheWay2D(b_PosX, b_PosY);
            if (targetPos == null)
            {
                throw new Exception($"无法进入目标点 instanceId:{b_CombatSource.InstanceId} mapId:{b_Component.MapId} x:{b_PosX} y:{b_PosY}");
            }

            MapComponent oldMap = b_CombatSource.CurrentMap;

            oldMap.Leave(b_CombatSource,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
            b_Component.Enter(b_CombatSource, b_PosX, b_PosY, true,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);

            if (b_CombatSource.Identity == E_Identity.Hero)
            {
                Log.PLog("Map", $"a:{((GamePlayer)b_CombatSource).Player.UserId} r:{b_CombatSource.InstanceId} 切换地图 {oldMap.Id}:{oldMap.MapId} => {b_Component.Id}:{b_Component.MapId}",
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);

                // 发布 CombatSourceEnterOrSwitchMap 事件
                ETModel.EventType.CombatSourceEnterOrSwitchMap.Instance.combatSource = b_CombatSource;
                ETModel.EventType.CombatSourceEnterOrSwitchMap.Instance.oldMap = oldMap;
                ETModel.EventType.CombatSourceEnterOrSwitchMap.Instance.newMap = b_Component;
                Root.EventSystem.OnRun("CombatSourceEnterOrSwitchMap", ETModel.EventType.CombatSourceEnterOrSwitchMap.Instance);
            }
        }

        public static void Switch(this MapComponent b_Component, CombatSource b_CombatSource, Vector2Int b_Pos,
            [CallerLineNumber] int callerLineNumber = 0,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "")
        {
            Switch(b_Component, b_CombatSource, b_Pos.x, b_Pos.y,
                    callerLineNumber,
                    callerMemberName,
                    callerFilePath);
        }

        #region Add and Remove CombatSource
        private static void AddCombatSource(this MapCellAreaComponent b_Component, CombatSource b_CombatSource)
        {
            switch (b_CombatSource.Identity)
            {
                case E_Identity.Hero:
                    b_Component.FieldPlayerDic.Add(b_CombatSource.InstanceId, (GamePlayer)b_CombatSource);
                    break;
                case E_Identity.Enemy:
                    b_Component.FieldEnemyDic.Add(b_CombatSource.InstanceId, (Enemy)b_CombatSource);
                    break;
                case E_Identity.Pet:
                    b_Component.FieldPetsDic.Add(b_CombatSource.InstanceId, (Pets)b_CombatSource);
                    break;
                case E_Identity.Summoned:
                    b_Component.FieldSummonedDic.Add(b_CombatSource.InstanceId, (Summoned)b_CombatSource);
                    break;
                case E_Identity.HolyteacherSummoned:
                    b_Component.FieldHolyteacherSummonedDic.Add(b_CombatSource.InstanceId, (HolyteacherSummoned)b_CombatSource);
                    break;
                default:
                    throw new Exception($"无法添加未知类型 {b_CombatSource.Identity}");
            }
        }

        private static void AddCombatSource(this C_FindTheWay2D b_Component, CombatSource b_CombatSource)
        {
            switch (b_CombatSource.Identity)
            {
                case E_Identity.Hero:
                    b_Component.FieldPlayerDic.Add(b_CombatSource.InstanceId, (GamePlayer)b_CombatSource);
                    break;
                case E_Identity.Enemy:
                    b_Component.FieldEnemyDic.Add(b_CombatSource.InstanceId, (Enemy)b_CombatSource);
                    break;
                case E_Identity.Pet:
                    b_Component.FieldPetsDic.Add(b_CombatSource.InstanceId, (Pets)b_CombatSource);
                    break;
                case E_Identity.Summoned:
                    b_Component.FieldSummonedDic.Add(b_CombatSource.InstanceId, (Summoned)b_CombatSource);
                    break;
                case E_Identity.HolyteacherSummoned:
                    //b_Component.FieldHolyteacherSummonedDic.Add(b_CombatSource.InstanceId, (HolyteacherSummoned)b_CombatSource);
                    break;
                default:
                    throw new Exception($"无法添加未知类型 {b_CombatSource.Identity}");
            }
        }

        private static void RemoveCombatSource(this MapCellAreaComponent b_Component, CombatSource b_CombatSource)
        {
            switch (b_CombatSource.Identity)
            {
                case E_Identity.Hero:
                    b_Component.FieldPlayerDic.Remove(b_CombatSource.InstanceId);
                    break;
                case E_Identity.Enemy:
                    b_Component.FieldEnemyDic.Remove(b_CombatSource.InstanceId);
                    break;
                case E_Identity.Pet:
                    b_Component.FieldPetsDic.Remove(b_CombatSource.InstanceId);
                    break;
                case E_Identity.Summoned:
                    b_Component.FieldSummonedDic.Remove(b_CombatSource.InstanceId);
                    break;
                case E_Identity.HolyteacherSummoned:
                    b_Component.FieldHolyteacherSummonedDic.Remove(b_CombatSource.InstanceId);
                    break;
                default:
                    throw new Exception($"无法添加未知类型 {b_CombatSource.Identity}");
            }
        }

        private static void RemoveCombatSource(this C_FindTheWay2D b_Component, CombatSource b_CombatSource)
        {
            switch (b_CombatSource.Identity)
            {
                case E_Identity.Hero:
                    b_Component.FieldPlayerDic.Remove(b_CombatSource.InstanceId);
                    break;
                case E_Identity.Enemy:
                    b_Component.FieldEnemyDic.Remove(b_CombatSource.InstanceId);
                    break;
                case E_Identity.Pet:
                    b_Component.FieldPetsDic.Remove(b_CombatSource.InstanceId);
                    break;
                case E_Identity.Summoned:
                    b_Component.FieldSummonedDic.Remove(b_CombatSource.InstanceId);
                    break;
                case E_Identity.HolyteacherSummoned:
                    //b_Component.FieldHolyteacherSummonedDic.Remove(b_CombatSource.InstanceId);
                    break;
                default:
                    throw new Exception($"无法添加未知类型 {b_CombatSource.Identity}");
            }
        }
        #endregion

        #region Cell 变动
        private static void CellChanged(GamePlayer b_CombatSource, C_FindTheWay2D oldPos, MapCellAreaComponent oldCell,
            List<int> leaveAroundField, List<int> currentAroundField, List<int> intoAroundField, bool b_NeedMove = true)
        {
            if (leaveAroundField != null && leaveAroundField.Count > 0)
            {// 告诉 离开了
                G2C_BattlePickUpDropItem_notice itemLeaveNotice = new G2C_BattlePickUpDropItem_notice();
                G2C_UnitLeaveView_notice unitLeaveViewNotice = new G2C_UnitLeaveView_notice();
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = oldPos.X;
                mMoveMessageNotice.Y = oldPos.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.ViewId = 2;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;

                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                using RepeatedFieldComponent<long> leavedItem = RepeatedFieldComponent<long>.Create();
                using RepeatedFieldComponent<long> leavedUnit = RepeatedFieldComponent<long>.Create();
                for (int i = 0, len = leaveAroundField.Count; i < len; i++)
                {
                    int mleaveAroundFieldIndex = leaveAroundField[i];

                    if (oldCell.AroundFieldDic.TryGetValue(mleaveAroundFieldIndex, out var cell))
                    {
                        cellList.Add(cell);
                        // 添加离开视野的物品
                        foreach (long itemInstanceId in cell.MapItemRes.Keys)
                        {
                            leavedItem.Add(itemInstanceId);
                        }
                        // 观察者 -1
                        ModifyObServerCount(cell, -1);

                        leavedUnit.AddRange(cell.FieldPlayerDic.Keys);
                        leavedUnit.AddRange(cell.FieldNpcDic.Keys);
                        leavedUnit.AddRange(cell.FieldHolyteacherSummonedDic.Keys);
                        leavedUnit.AddRange(cell.FieldEnemyDic.Keys);
                        leavedUnit.AddRange(cell.FieldSummonedDic.Keys);
                        leavedUnit.AddRange(cell.FieldPetsDic.Keys);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice, b_CombatSource);


                if (leavedItem.Count != 0)
                {
                    itemLeaveNotice.InstanceId = leavedItem;

                    b_CombatSource.Player.Send(itemLeaveNotice);
                }
                if (leavedUnit.Count != 0)
                {
                    unitLeaveViewNotice.LeavedUnitId = leavedUnit;
                    b_CombatSource.Player.Send(unitLeaveViewNotice);
                }
            }
            if (currentAroundField != null && currentAroundField.Count > 0)
            {// 告诉 移动了
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.ViewId = 0;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                //mMoveMessageNotice.Title = b_CombatSource.Data.Title;
                //mMoveMessageNotice.WallTitle = b_CombatSource.Data.WallTile;
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = currentAroundField.Count; i < len; i++)
                {
                    int mcurrentAroundFieldIndex = currentAroundField[i];

                    if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mcurrentAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice, b_CombatSource);
            }
            if (intoAroundField != null && intoAroundField.Count > 0)
            {// 告诉 进来了  通知该玩家 这区域中原本有什么
                int mapId = b_CombatSource.UnitData.Index;
                void GoToTargetCellField(MapCellAreaComponent b_MapCellComponent)
                {
                    var battleComponent = b_MapCellComponent.Parent.GetCustomComponent<BattleComponent>();
                    {
                        foreach (var mPlayer in b_MapCellComponent.FieldPlayerDic.Values)
                        {
                            //                             if (mPlayer.InstanceId == b_CombatSource.InstanceId || mPlayer.IsDisposeable || mPlayer.UnitData == null || mPlayer.Data == null)
                            //                             {
                            //                                 continue;
                            //                             }
                            if (mPlayer.InstanceId == b_CombatSource.InstanceId) continue;
                            G2C_MovePos_notice mMoveNoticeTemp = new G2C_MovePos_notice();
                            mMoveNoticeTemp.X = mPlayer.UnitData.X;
                            mMoveNoticeTemp.Y = mPlayer.UnitData.Y;
                            mMoveNoticeTemp.GameUserId = mPlayer.InstanceId;
                            mMoveNoticeTemp.ViewId = 1;
                            mMoveNoticeTemp.MapId = mapId;
                            mMoveNoticeTemp.SourceX = mPlayer.UnitData.X;
                            mMoveNoticeTemp.SourceY = mPlayer.UnitData.Y;
                            mMoveNoticeTemp.NickName = mPlayer.Data.NickName;
                            mMoveNoticeTemp.ModelId = mPlayer.Data.PlayerTypeId;
                            mMoveNoticeTemp.OccupationLevel = mPlayer.Data.OccupationLevel;
                            mMoveNoticeTemp.HpValue = mPlayer.GetNumerialFunc(E_GameProperty.PROP_HP);
                            mMoveNoticeTemp.HpMaxValue = mPlayer.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                            mMoveNoticeTemp.PkNumber = mPlayer.UnitData.PkPoint;
                            mMoveNoticeTemp.Title = mPlayer.Data.Title;
                            mMoveNoticeTemp.WallTitle = mPlayer.Data.WallTile;
                            mMoveNoticeTemp.ReincarnateCnt = mPlayer.Data.ReincarnateCnt;
                            b_CombatSource.Player.Send(mMoveNoticeTemp);

                            var mlist = mPlayer.HealthStatsDic.Keys.ToArray();
                            for (int i = 0, len = mlist.Length; i < len; i++)
                            {
                                var mkey = mlist[i];
                              
                                switch (mkey)
                                {
                                    case E_BattleSkillStats.FangYuHuZhao:
                                        {
                                            if (mPlayer.HealthStatsDic.TryGetValue(mkey, out var mHealthStats))
                                            {
                                                if (mHealthStats.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                                {
                                                    if (mTempBufferData.CacheData.TryGetValue(0, out var mTempBufferValue))
                                                    {
                                                        if (mTempBufferValue > 0)
                                                        {
                                                            G2C_AttackBuffer_notice mAttackBufferNotice = new G2C_AttackBuffer_notice();
                                                            mAttackBufferNotice.AttackTarget = mPlayer.InstanceId;
                                                            mAttackBufferNotice.BufferId = (long)mkey | (long)0 << 16;
                                                            mAttackBufferNotice.Ticks = mHealthStats.ContinueTimeMax - battleComponent.CurrentTimeTick;

                                                            b_CombatSource.Player.Send(mAttackBufferNotice);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            if (mPlayer.HealthStatsDic.TryGetValue(mkey, out var mHealthStats))
                                            {
                                                G2C_AttackBuffer_notice mAttackBufferNotice = new G2C_AttackBuffer_notice();
                                                mAttackBufferNotice.AttackTarget = mPlayer.InstanceId;
                                                mAttackBufferNotice.BufferId = (long)mkey | (long)0 << 16;
                                                mAttackBufferNotice.Ticks = mHealthStats.ContinueTimeMax - battleComponent.CurrentTimeTick;

                                                b_CombatSource.Player.Send(mAttackBufferNotice);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    {
                        foreach (var mSummoned in b_MapCellComponent.FieldSummonedDic.Values)
                        {
                            //                             if (mSummoned.IsDisposeable == true || mSummoned.IsDeath == true || mSummoned.GamePlayer.InstanceId == b_CombatSource.InstanceId)
                            //                             {
                            //                                 continue;
                            //                             }

                            G2C_MovePos_notice mMoveNoticeTemp = new G2C_MovePos_notice();
                            mMoveNoticeTemp.X = mSummoned.UnitData.X;
                            mMoveNoticeTemp.Y = mSummoned.UnitData.Y;
                            mMoveNoticeTemp.UnitType = (int)mSummoned.Identity;
                            mMoveNoticeTemp.GameUserId = mSummoned.InstanceId;
                            mMoveNoticeTemp.OwnerGameUserId = mSummoned.GamePlayer.InstanceId;
                            mMoveNoticeTemp.MapId = mapId;
                            mMoveNoticeTemp.ViewId = 1;
                            mMoveNoticeTemp.SourceX = mSummoned.UnitData.X;
                            mMoveNoticeTemp.SourceY = mSummoned.UnitData.Y;
                            mMoveNoticeTemp.ModelId = mSummoned.Config.Id;
                            mMoveNoticeTemp.HpValue = mSummoned.GetNumerialFunc(E_GameProperty.PROP_HP);
                            mMoveNoticeTemp.HpMaxValue = mSummoned.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);

                            b_CombatSource.Player.Send(mMoveNoticeTemp);
                        }
                    }
                    {
                        foreach (var mPet in b_MapCellComponent.FieldPetsDic.Values)
                        {
                            //                             if (mPet.IsDisposeable == true || mPet.IsDeath == true || mPet.GamePlayer.InstanceId == b_CombatSource.InstanceId)
                            //                             {
                            //                                 continue;
                            //                             }

                            G2C_MovePos_notice mMoveNoticeTemp = new G2C_MovePos_notice();
                            mMoveNoticeTemp.X = mPet.UnitData.X;
                            mMoveNoticeTemp.Y = mPet.UnitData.Y;
                            mMoveNoticeTemp.UnitType = (int)mPet.Identity;
                            mMoveNoticeTemp.GameUserId = mPet.InstanceId;
                            mMoveNoticeTemp.MapId = mapId;
                            mMoveNoticeTemp.ViewId = 1;
                            mMoveNoticeTemp.SourceX = mPet.UnitData.X;
                            mMoveNoticeTemp.SourceY = mPet.UnitData.Y;

                            mMoveNoticeTemp.NickName = mPet.dBPetsData.PetsName;
                            mMoveNoticeTemp.OwnerGameUserId = mPet.GamePlayer.InstanceId;
                            mMoveNoticeTemp.ModelId = mPet.Config.Id;
                            mMoveNoticeTemp.HpValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                            mMoveNoticeTemp.HpMaxValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                            mMoveNoticeTemp.IsNeedMove = b_NeedMove ? 0 : 1;

                            b_CombatSource.Player.Send(mMoveNoticeTemp);

                            var mlist = mPet.HealthStatsDic.Keys.ToArray();
                            for (int i = 0, len = mlist.Length; i < len; i++)
                            {
                                var mkey = mlist[i];

                                switch (mkey)
                                {
                                    case E_BattleSkillStats.FangYuHuZhao:
                                        {
                                            if (mPet.HealthStatsDic.TryGetValue(mkey, out var mHealthStats))
                                            {
                                                if (mHealthStats.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                                {
                                                    if (mTempBufferData.CacheData.TryGetValue(0, out var mTempBufferValue))
                                                    {
                                                        if (mTempBufferValue > 0)
                                                        {
                                                            G2C_AttackBuffer_notice mAttackBufferNotice = new G2C_AttackBuffer_notice();
                                                            mAttackBufferNotice.AttackTarget = mPet.InstanceId;
                                                            mAttackBufferNotice.BufferId = (long)mkey | (long)0 << 16;
                                                            mAttackBufferNotice.Ticks = mHealthStats.ContinueTimeMax - battleComponent.CurrentTimeTick;

                                                            b_CombatSource.Player.Send(mAttackBufferNotice);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        break;
                                    default:
                                        {
                                            if (mPet.HealthStatsDic.TryGetValue(mkey, out var mHealthStats))
                                            {
                                                G2C_AttackBuffer_notice mAttackBufferNotice = new G2C_AttackBuffer_notice();
                                                mAttackBufferNotice.AttackTarget = mPet.InstanceId;
                                                mAttackBufferNotice.BufferId = (long)mkey | (long)0 << 16;
                                                mAttackBufferNotice.Ticks = mHealthStats.ContinueTimeMax - battleComponent.CurrentTimeTick;

                                                b_CombatSource.Player.Send(mAttackBufferNotice);
                                            }
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    {
                        foreach (var mEnemy in b_MapCellComponent.FieldEnemyDic.Values)
                        {

                            //Log.PLog("ShuaGuai", $"MovePos Mobid:{mEnemy.Config.Id} UUID:{mEnemy.InstanceId} X:{mEnemy.UnitData.X}Y:{mEnemy.UnitData.Y}");
                            G2C_MovePos_notice mMoveNoticeTemp = new G2C_MovePos_notice();
                            mMoveNoticeTemp.X = mEnemy.UnitData.X;
                            mMoveNoticeTemp.Y = mEnemy.UnitData.Y;
                            mMoveNoticeTemp.UnitType = (int)mEnemy.Identity;
                            mMoveNoticeTemp.GameUserId = mEnemy.InstanceId;
                            mMoveNoticeTemp.MapId = mapId;
                            mMoveNoticeTemp.ViewId = 1;
                            mMoveNoticeTemp.SourceX = mEnemy.UnitData.X;
                            mMoveNoticeTemp.SourceY = mEnemy.UnitData.Y;
                            mMoveNoticeTemp.ModelId = mEnemy.Config.Id;
                            mMoveNoticeTemp.HpValue = mEnemy.GetNumerialFunc(E_GameProperty.PROP_HP);
                            mMoveNoticeTemp.HpMaxValue = mEnemy.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);

                            //CustomFrameWork.Component.LogToolComponent.FileSimpleLog("移动", $" 玩家移动: {b_CombatSource.InstanceId} 进入  视野添加 {mEnemy.InstanceId}", false);

                            b_CombatSource.Player.Send(mMoveNoticeTemp);
                        }
                    }
                    {
                        foreach (var mNpc in b_MapCellComponent.FieldNpcDic.Values)
                        {

                            G2C_MovePos_notice mMoveNoticeTemp = new G2C_MovePos_notice();
                            mMoveNoticeTemp.X = mNpc.X;
                            mMoveNoticeTemp.Y = mNpc.Y;
                            mMoveNoticeTemp.UnitType = (int)mNpc.Identity;
                            mMoveNoticeTemp.GameUserId = mNpc.Id;
                            mMoveNoticeTemp.MapId = mapId;
                            mMoveNoticeTemp.ViewId = 1;
                            mMoveNoticeTemp.SourceX = mNpc.X;
                            mMoveNoticeTemp.SourceY = mNpc.Y;
                            mMoveNoticeTemp.ModelId = mNpc.Config.Id;
                            mMoveNoticeTemp.Angle = mNpc.Angle;

                            b_CombatSource.Player.Send(mMoveNoticeTemp);
                        }
                    }

                    if (b_MapCellComponent.MapItemRes.Count > 0)
                    {
                        G2C_ItemDrop_notice mItemDrop_notice = new G2C_ItemDrop_notice();
                        foreach (var mapItem in b_MapCellComponent.MapItemRes.Values)
                        {
                            mItemDrop_notice.Info.Add(mapItem.ToMessage());
                        }
                        b_CombatSource.Player.Send(mItemDrop_notice);
                    }

                    if (b_MapCellComponent.MapStallDic.Count > 0)
                    {
                        G2C_BaiTanInstance_notice mBaiTanInstance_notice = null;

                        var mMapStallArray = b_MapCellComponent.MapStallDic.Values.ToArray();
                        for (int j = 0, jlen = mMapStallArray.Length; j < jlen; j++)
                        {
                            var mMapItemRes = mMapStallArray[j];

                            if (mMapItemRes.IsStalling)
                            {
                                if (mBaiTanInstance_notice == null)
                                {
                                    mBaiTanInstance_notice = new G2C_BaiTanInstance_notice();
                                }
                                C2G_BaiTanInfoMessage mBaiTanInfoMessage = new C2G_BaiTanInfoMessage();
                                mBaiTanInfoMessage.BaiTanInstanceId = mMapItemRes.StallId;
                                mBaiTanInfoMessage.BaiTanName = mMapItemRes.StallName;
                                mBaiTanInstance_notice.Prop.Add(mBaiTanInfoMessage);
                            }
                        }
                        if (mBaiTanInstance_notice != null)
                        {
                            b_CombatSource.Player.Send(mBaiTanInstance_notice);
                        }
                    }
                }

                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.ViewId = 1;
                mMoveMessageNotice.MapId = mapId;
                if (oldPos != null)
                {
                    mMoveMessageNotice.SourceX = oldPos.X;
                    mMoveMessageNotice.SourceY = oldPos.Y;
                }
                else
                {
                    mMoveMessageNotice.SourceX = b_CombatSource.UnitData.X;
                    mMoveMessageNotice.SourceY = b_CombatSource.UnitData.Y;
                }

                mMoveMessageNotice.NickName = b_CombatSource.Data.NickName;
                mMoveMessageNotice.ModelId = b_CombatSource.Data.PlayerTypeId;
                mMoveMessageNotice.OccupationLevel = b_CombatSource.Data.OccupationLevel;
                mMoveMessageNotice.HpValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                mMoveMessageNotice.HpMaxValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                mMoveMessageNotice.PkNumber = b_CombatSource.UnitData.PkPoint;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                mMoveMessageNotice.Title = b_CombatSource.Data.Title;
                mMoveMessageNotice.WallTitle = b_CombatSource.Data.WallTile;
                mMoveMessageNotice.ReincarnateCnt = b_CombatSource.Data.ReincarnateCnt;
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = intoAroundField.Count; i < len; i++)
                {
                    int mintoAroundFieldIndex = intoAroundField[i];

                    if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mintoAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);

                        GoToTargetCellField(mTemp);

                        // 观察者 +1
                        ModifyObServerCount(mTemp, 1);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice, b_CombatSource);
            }
        }

        private static void CellChanged(Enemy b_CombatSource, C_FindTheWay2D oldPos, MapCellAreaComponent oldCell,
            List<int> leaveAroundField, List<int> currentAroundField, List<int> intoAroundField, bool b_NeedMove = true)
        {
            if (leaveAroundField != null && leaveAroundField.Count > 0)
            {// 告诉 离开了
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = oldPos.X;
                mMoveMessageNotice.Y = oldPos.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 2;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = leaveAroundField.Count; i < len; i++)
                {
                    int mleaveAroundFieldIndex = leaveAroundField[i];

                    if (oldCell.AroundFieldDic.TryGetValue(mleaveAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
            if (currentAroundField != null && currentAroundField.Count > 0)
            {// 告诉 移动了
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 0;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = currentAroundField.Count; i < len; i++)
                {
                    int mcurrentAroundFieldIndex = currentAroundField[i];

                    if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mcurrentAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
            if (intoAroundField != null && intoAroundField.Count > 0)
            {// 告诉 进来了  通知该玩家 这区域中原本有什么
                {
                    G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                    mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                    mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                    mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                    mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                    mMoveMessageNotice.ViewId = 1;
                    mMoveMessageNotice.MapId = b_CombatSource.UnitData.Index;
                    if (oldPos != null)
                    {
                        mMoveMessageNotice.SourceX = oldPos.X;
                        mMoveMessageNotice.SourceY = oldPos.Y;
                    }
                    else
                    {
                        mMoveMessageNotice.SourceX = b_CombatSource.UnitData.X;
                        mMoveMessageNotice.SourceY = b_CombatSource.UnitData.Y;
                    }
                    mMoveMessageNotice.HpValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                    mMoveMessageNotice.HpMaxValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    mMoveMessageNotice.ModelId = b_CombatSource.Config.Id;
                    mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;

                    using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                    for (int i = 0, len = intoAroundField.Count; i < len; i++)
                    {
                        int mintoAroundFieldIndex = intoAroundField[i];

                        if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mintoAroundFieldIndex, out var mTemp))
                        {
                            cellList.Add(mTemp);
                        }
                    }
                    cellList.RadioBroadcast(mMoveMessageNotice);
                }
            }
        }

        private static void CellChanged(Pets b_CombatSource, C_FindTheWay2D oldPos, MapCellAreaComponent oldCell,
           List<int> leaveAroundField, List<int> currentAroundField, List<int> intoAroundField, bool b_NeedMove = true)
        {
            if (leaveAroundField != null && leaveAroundField.Count > 0)
            {// 告诉 离开了
                //Log.Debug($"宠物通知离开{b_CombatSource.InstanceId}");
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = oldPos.X;
                mMoveMessageNotice.Y = oldPos.Y;
                mMoveMessageNotice.OwnerGameUserId = b_CombatSource.GamePlayer.InstanceId;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 2;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                //Log.Debug($"宠物离开 {b_StartFindTheWay.Map.Id} 进入{b_TargetFindTheWay.Map.Id}");
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = leaveAroundField.Count; i < len; i++)
                {
                    int mleaveAroundFieldIndex = leaveAroundField[i];

                    if (oldCell.AroundFieldDic.TryGetValue(mleaveAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                        ModifyObServerCount(mTemp, -1);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
            if (currentAroundField != null && currentAroundField.Count > 0)
            {// 告诉 移动了
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                mMoveMessageNotice.OwnerGameUserId = b_CombatSource.GamePlayer.InstanceId;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 0;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = currentAroundField.Count; i < len; i++)
                {
                    int mcurrentAroundFieldIndex = currentAroundField[i];

                    if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mcurrentAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
            if (intoAroundField != null && intoAroundField.Count > 0)
            {// 告诉 进来了  通知该玩家 这区域中原本有什么
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 1;
                mMoveMessageNotice.MapId = b_CombatSource.UnitData.Index;
                if (oldPos != null)
                {
                    mMoveMessageNotice.SourceX = oldPos.X;
                    mMoveMessageNotice.SourceY = oldPos.Y;
                }
                else
                {
                    mMoveMessageNotice.SourceX = b_CombatSource.UnitData.X;
                    mMoveMessageNotice.SourceY = b_CombatSource.UnitData.Y;
                }

                mMoveMessageNotice.NickName = b_CombatSource.dBPetsData.PetsName;
                mMoveMessageNotice.OwnerGameUserId = b_CombatSource.GamePlayer.InstanceId;
                mMoveMessageNotice.ModelId = b_CombatSource.dBPetsData.ConfigID;
                mMoveMessageNotice.HpValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                mMoveMessageNotice.HpMaxValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                mMoveMessageNotice.MpValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP);
                mMoveMessageNotice.MpMaxValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;

                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = intoAroundField.Count; i < len; i++)
                {
                    int mintoAroundFieldIndex = intoAroundField[i];

                    if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mintoAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                        ModifyObServerCount(mTemp, 1);
                    }
                }

                cellList.RadioBroadcast(mMoveMessageNotice);

                var battleComponent = b_CombatSource.CurrentCell.Parent.GetCustomComponent<BattleComponent>();
                var mlist = b_CombatSource.HealthStatsDic.Keys.ToArray();
                for (int i = 0, len = mlist.Length; i < len; i++)
                {
                    var mkey = mlist[i];

                    switch (mkey)
                    {
                        case E_BattleSkillStats.FangYuHuZhao:
                            {
                                if (b_CombatSource.HealthStatsDic.TryGetValue(mkey, out var mHealthStats))
                                {
                                    if (mHealthStats.CacheDatas.TryGetValue(0, out var mTempBufferData))
                                    {
                                        if (mTempBufferData.CacheData.TryGetValue(0, out var mTempBufferValue))
                                        {
                                            if (mTempBufferValue > 0)
                                            {
                                                G2C_AttackBuffer_notice mAttackBufferNotice = new G2C_AttackBuffer_notice();
                                                mAttackBufferNotice.AttackTarget = b_CombatSource.InstanceId;
                                                mAttackBufferNotice.BufferId = (long)mkey | (long)0 << 16;
                                                mAttackBufferNotice.Ticks = mHealthStats.ContinueTimeMax - battleComponent.CurrentTimeTick;

                                                for (int j = 0, jlen = cellList.Count; j < jlen; j++)
                                                {
                                                    cellList[j].RadioBroadcast(mAttackBufferNotice);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            {
                                if (b_CombatSource.HealthStatsDic.TryGetValue(mkey, out var mHealthStats))
                                {
                                    G2C_AttackBuffer_notice mAttackBufferNotice = new G2C_AttackBuffer_notice();
                                    mAttackBufferNotice.AttackTarget = b_CombatSource.InstanceId;
                                    mAttackBufferNotice.BufferId = (long)mkey | (long)0 << 16;
                                    mAttackBufferNotice.Ticks = mHealthStats.ContinueTimeMax - battleComponent.CurrentTimeTick;

                                    for (int j = 0, jlen = cellList.Count; j < jlen; j++)
                                    {
                                        cellList[j].RadioBroadcast(mAttackBufferNotice);
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }
        private static void CellChanged(Summoned b_CombatSource, C_FindTheWay2D oldPos, MapCellAreaComponent oldCell,
          List<int> leaveAroundField, List<int> currentAroundField, List<int> intoAroundField, bool b_NeedMove = true)
        {
            if (leaveAroundField != null && leaveAroundField.Count > 0)
            {// 告诉 离开了
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = oldPos.X;
                mMoveMessageNotice.Y = oldPos.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 2;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = leaveAroundField.Count; i < len; i++)
                {
                    int mleaveAroundFieldIndex = leaveAroundField[i];

                    if (oldCell.AroundFieldDic.TryGetValue(mleaveAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                        ModifyObServerCount(mTemp, -1);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
            if (currentAroundField != null && currentAroundField.Count > 0)
            {// 告诉 移动了
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 0;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = currentAroundField.Count; i < len; i++)
                {
                    int mcurrentAroundFieldIndex = currentAroundField[i];

                    if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mcurrentAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
            if (intoAroundField != null && intoAroundField.Count > 0)
            {// 告诉 进来了  通知该玩家 这区域中原本有什么
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.OwnerGameUserId = b_CombatSource.GamePlayer.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 1;
                mMoveMessageNotice.MapId = b_CombatSource.UnitData.Index;
                if (oldPos != null)
                {
                    mMoveMessageNotice.SourceX = oldPos.X;
                    mMoveMessageNotice.SourceY = oldPos.Y;
                }
                else
                {
                    mMoveMessageNotice.SourceX = b_CombatSource.UnitData.X;
                    mMoveMessageNotice.SourceY = b_CombatSource.UnitData.Y;
                }
                mMoveMessageNotice.HpValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP);
                mMoveMessageNotice.HpMaxValue = b_CombatSource.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                mMoveMessageNotice.ModelId = b_CombatSource.Config.Id;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;

                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = intoAroundField.Count; i < len; i++)
                {
                    int mintoAroundFieldIndex = intoAroundField[i];

                    if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mintoAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                        ModifyObServerCount(mTemp, 1);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
        }
        private static void CellChanged(HolyteacherSummoned b_CombatSource, C_FindTheWay2D oldPos, MapCellAreaComponent oldCell,
           List<int> leaveAroundField, List<int> currentAroundField, List<int> intoAroundField, bool b_NeedMove = true)
        {
            if (leaveAroundField != null && leaveAroundField.Count > 0)
            {// 告诉 离开了
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = oldPos.X;
                mMoveMessageNotice.Y = oldPos.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 2;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = leaveAroundField.Count; i < len; i++)
                {
                    int mleaveAroundFieldIndex = leaveAroundField[i];

                    if (oldCell.AroundFieldDic.TryGetValue(mleaveAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                        ModifyObServerCount(mTemp, -1);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
            if (currentAroundField != null && currentAroundField.Count > 0)
            {// 告诉 移动了
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 0;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;
                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = currentAroundField.Count; i < len; i++)
                {
                    int mcurrentAroundFieldIndex = currentAroundField[i];

                    if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mcurrentAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
            if (intoAroundField != null && intoAroundField.Count > 0)
            {// 告诉 进来了  通知该玩家 这区域中原本有什么
                G2C_MovePos_notice mMoveMessageNotice = new G2C_MovePos_notice();
                mMoveMessageNotice.X = b_CombatSource.UnitData.X;
                mMoveMessageNotice.Y = b_CombatSource.UnitData.Y;
                mMoveMessageNotice.GameUserId = b_CombatSource.InstanceId;
                mMoveMessageNotice.OwnerGameUserId = b_CombatSource.GamePlayer.InstanceId;
                mMoveMessageNotice.UnitType = (int)b_CombatSource.Identity;
                mMoveMessageNotice.ViewId = 1;
                mMoveMessageNotice.MapId = b_CombatSource.UnitData.Index;
                if (oldPos != null)
                {
                    mMoveMessageNotice.SourceX = oldPos.X;
                    mMoveMessageNotice.SourceY = oldPos.Y;
                }
                else
                {
                    mMoveMessageNotice.SourceX = b_CombatSource.UnitData.X;
                    mMoveMessageNotice.SourceY = b_CombatSource.UnitData.Y;
                }
                mMoveMessageNotice.ModelId = b_CombatSource.Config.Id;
                mMoveMessageNotice.IsNeedMove = b_NeedMove ? 0 : 1;

                using ListComponent<MapCellAreaComponent> cellList = ListComponent<MapCellAreaComponent>.Create();
                for (int i = 0, len = intoAroundField.Count; i < len; i++)
                {
                    int mintoAroundFieldIndex = intoAroundField[i];

                    if (b_CombatSource.CurrentCell.AroundFieldDic.TryGetValue(mintoAroundFieldIndex, out var mTemp))
                    {
                        cellList.Add(mTemp);
                        ModifyObServerCount(mTemp, 1);
                    }
                }
                cellList.RadioBroadcast(mMoveMessageNotice);
            }
        }

        public static void ModifyObServerCount(this MapCellAreaComponent cell, int val)
        {
            if (val > 0)
            {
                // 观察者进入
                if (cell.obServerCount <= 0)
                {
                    foreach (var unit in cell.FieldEnemyDic.Values)
                    {
                        unit.AddCustomComponent<UpdateFrameComponent>();
                    }
                }
                cell.obServerCount += val;
            }
            else if (val < 0)
            {
                // 观察者离开
                cell.obServerCount += val;
                if (cell.obServerCount <= 0)
                {
                    foreach (var unit in cell.FieldEnemyDic.Values)
                    {
                        unit.RemoveCustomComponent<UpdateFrameComponent>();
                    }
                }

            }
            if (cell.obServerCount < 0)
            {
                Log.Error("cell.obServerCount < 0");
            }
        }
        #endregion

        /// <summary>
        /// 地图分发消息
        /// </summary>
        public static void SendNotice(this MapComponent b_Component, C_FindTheWay2D b_StartFindTheWay, IActorMessage b_ActorMessage)
        {
            var mSourceCellField = b_StartFindTheWay.Map.GetMapCellField(b_StartFindTheWay);
            if (mSourceCellField != null)
            {
                mSourceCellField.RadioBroadcastByAroundCellField(b_ActorMessage);
            }
        }
        public static void SendNotice(this MapComponent b_Component, CombatSource b_CombatSource, IActorMessage b_ActorMessage)
        {
            if (b_CombatSource == null || b_CombatSource.IsDisposeable || b_CombatSource.UnitData == null) return;

            var mSourceCellField = b_Component.GetMapCellField(b_CombatSource);
            if (mSourceCellField != null)
            {
                mSourceCellField.RadioBroadcastByAroundCellField(b_ActorMessage);
            }
        }

        public static async Task SendNoticeByServer(this MapComponent b_Component, IActorMessage b_ActorMessage)
        {
            await Task.Delay(100);

            var mServerArea = b_Component.Parent.Parent;

            for (int i = 0, len = mServerArea.VirtualIdlist.Count; i < len; i++)
            {
                var mSourceGameAreaId = mServerArea.VirtualIdlist[i];
                var mGameAreaId = mSourceGameAreaId >> 16;
                var mPlayerDic = CustomFrameWork.Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllByZone(mGameAreaId);

                if (mPlayerDic == null || mPlayerDic.Count <= 0) continue;

                var mPlayerlist = mPlayerDic.Values.ToArray();
                for (int j = 0, jlen = mPlayerlist.Length; j < jlen; j++)
                {
                    var mPlayer = mPlayerlist[j];
                    if (mPlayer.IsDisposeable) continue;
                    if (mPlayer.SourceGameAreaId != mSourceGameAreaId) continue;

                    mPlayer.Send(b_ActorMessage);
                }
            }
        }
        public static void SendNoticeByMap(this MapComponent b_Component, IActorMessage b_ActorMessage)
        {
            for (int i = 0, len = b_Component.MapCellFieldlist.Count; i < len; i++)
            {
                var mSourceCellField = b_Component.MapCellFieldlist[i];
                if (mSourceCellField != null)
                {
                    mSourceCellField.RadioBroadcast(b_ActorMessage);
                }
            }
        }
    }
}