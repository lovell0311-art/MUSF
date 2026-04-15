using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CustomFrameWork;
using ETModel;
using UnityEngine;

namespace ETHotfix
{

    public static partial class BattleComponentSystem
    {
        /// <summary>
        /// 状态更新
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_CombatSource"></param>
        private static void CombatStateUpdate(this BattleComponent b_Component, CombatSource b_CombatSource)
        {

        }
        /// <summary>
        /// 目标检测 检测当战斗目标
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_CombatSource"></param>
        private static void WarEnemyTargetUpdate(this BattleComponent b_Component, Enemy b_CombatSource)
        {
            MapComponent mMapComponent = b_Component.Parent as MapComponent;

            {
                var mFindTheWay = mMapComponent.GetFindTheWay2D(b_CombatSource);
                if (mFindTheWay == null)
                {
                    b_CombatSource.Enemy = null;
                    b_CombatSource.TargetEnemy = null;
                    return;
                }
                if (mFindTheWay.IsSafeArea)
                {
                    b_CombatSource.Enemy = null;
                    b_CombatSource.TargetEnemy = null;
                    return;
                }
            }

            if (b_CombatSource.Enemy != null)
            {
                if (b_CombatSource.Enemy.IsDisposeable)
                {
                    b_CombatSource.Enemy = null;
                }
                else if (b_CombatSource.Enemy.IsDeath)
                {
                    b_CombatSource.Enemy = null;
                }
                else if (b_CombatSource.Enemy.UnitData.Index != mMapComponent.MapId)
                {
                    b_CombatSource.Enemy = null;
                }
                else
                {
                    Vector2 mTargetEnemyPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.Enemy).Vector2Pos;
                    if (Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource.SourcePosX, b_CombatSource.SourcePosY).Vector2Pos, mTargetEnemyPoint) > b_CombatSource.Config.Ran2)
                    {// 是否脱战了
                        b_CombatSource.Enemy = null;
                    }
                    else if (10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, mTargetEnemyPoint) > b_CombatSource.Config.AR)
                    {// 攻击范围内
                        b_CombatSource.Enemy = null;
                    }
                    if (b_CombatSource.Enemy != null && (b_CombatSource.Position - b_CombatSource.Enemy.Position).Length() * 10 > b_CombatSource.Config.AR)
                    {
                        b_CombatSource.Enemy = null;
                        b_CombatSource.TargetEnemy = null;
                    }
                }
            }

            if (b_CombatSource.Enemy == null)
            {
                if (b_CombatSource.TargetEnemy != null)
                {
                    if (b_CombatSource.TargetEnemy.IsDisposeable)
                    {
                        b_CombatSource.TargetEnemy = null;
                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else if (b_CombatSource.TargetEnemy.IsDeath)
                    {
                        b_CombatSource.TargetEnemy = null;
                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else if (b_CombatSource.TargetEnemy.UnitData.Index != mMapComponent.MapId)
                    {
                        b_CombatSource.TargetEnemy = null;

                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else
                    {
                        Vector2 mTargetEnemyPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.TargetEnemy).Vector2Pos;
                        if (Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource.SourcePosX, b_CombatSource.SourcePosY).Vector2Pos, mTargetEnemyPoint) > b_CombatSource.Config.Ran2)
                        {// 是否脱战了
                            b_CombatSource.TargetEnemy = null;

                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                        }
                        float distance = 10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, mTargetEnemyPoint);
                        if (distance > b_CombatSource.Config.VR)
                        {
                            b_CombatSource.TargetEnemy = null;

                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                        }
                    }
                }
                if (b_CombatSource.TargetEnemy == null)
                {
                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;

                    var mCenterMapCellField = b_Component.Parent.GetMapCellField(b_CombatSource);
                    if (mCenterMapCellField != null)
                    {
                        float distance = b_CombatSource.Config.VR;
                        CombatSource mTargetGamePlayer = null;
                        Vector2 mCurrnetPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos;
                        Vector2 mSourcePoint = new Vector2(b_CombatSource.SourcePosX, b_CombatSource.SourcePosY);

                        var mMapCellFieldlist = mCenterMapCellField.AroundFieldDic.Values.ToArray();
                        for (int i = 0, len = mMapCellFieldlist.Length; i < len; i++)
                        {
                            var mMapCellField = mMapCellFieldlist[i];

                            if (mMapCellField.FieldPlayerDic.Count > 0)
                            {
                                var mFieldPlayerlist = mMapCellField.FieldPlayerDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldPlayerlist.Length; j < jlen; j++)
                                {
                                    var mFieldPlayer = mFieldPlayerlist[j];
                                    if (mFieldPlayer == null) continue;
                                    if (mFieldPlayer.IsDisposeable) continue;
                                    if (mFieldPlayer.IsDeath) continue;
                                    if (b_CombatSource.CreatePlayerId != 0 && mFieldPlayer.InstanceId != b_CombatSource.CreatePlayerId) continue;
                                    //if (b_CombatSource.Config.Monster_Type == 6)
                                        //if (!mFieldPlayer.Data.SpecialTitle) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayer);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    Vector2 mFieldPlayerPoint = mFindTheWay.Vector2Pos;

                                    if (Vector2.Distance(mSourcePoint, mFieldPlayerPoint) > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldPlayerPoint);
                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldPlayer;
                                    }
                                }
                            }
                            if (mMapCellField.FieldPetsDic.Count > 0)
                            {
                                var mFieldPetslist = mMapCellField.FieldPetsDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldPetslist.Length; j < jlen; j++)
                                {
                                    var mFieldPlayerPets = mFieldPetslist[j];
                                    if (mFieldPlayerPets == null) continue;
                                    if (mFieldPlayerPets.IsDisposeable || mFieldPlayerPets.GamePlayer.IsDisposeable) continue;
                                    if (mFieldPlayerPets.IsDeath || mFieldPlayerPets.GamePlayer.IsDeath) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayerPets);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    Vector2 mFieldPetsPoint = mFindTheWay.Vector2Pos;

                                    if (Vector2.Distance(mSourcePoint, mFieldPetsPoint) > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldPetsPoint);
                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldPlayerPets;
                                    }
                                }
                            }
                            if (mMapCellField.FieldSummonedDic.Count > 0)
                            {
                                var mFieldSummonedlist = mMapCellField.FieldSummonedDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldSummonedlist.Length; j < jlen; j++)
                                {
                                    var mFieldPlayerSummoned = mFieldSummonedlist[j];
                                    if (mFieldPlayerSummoned == null) continue;
                                    if (mFieldPlayerSummoned.IsDisposeable || mFieldPlayerSummoned.GamePlayer.IsDisposeable) continue;
                                    if (mFieldPlayerSummoned.IsDeath || mFieldPlayerSummoned.GamePlayer.IsDeath) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayerSummoned);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    Vector2 mFieldSummonedPoint = mFindTheWay.Vector2Pos;

                                    if (Vector2.Distance(mSourcePoint, mFieldSummonedPoint) > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldSummonedPoint);
                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldPlayerSummoned;
                                    }
                                }
                            }
                        }

                        if (mTargetGamePlayer != null)
                        {
                            b_CombatSource.TargetEnemy = mTargetGamePlayer;
                        }
                    }
                }
                if (b_CombatSource.TargetEnemy != null && b_CombatSource.TargetEnemy.IsDisposeable == false && b_CombatSource.TargetEnemy.IsDeath == false)
                {
                    if (10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, b_Component.Parent.GetFindTheWay2D(b_CombatSource.TargetEnemy).Vector2Pos) <= b_CombatSource.Config.AR)
                    {
                        b_CombatSource.Enemy = b_CombatSource.TargetEnemy;
                    }
                }
            }
        }
        private static void WarEnemyTargetUpdate(this BattleComponent b_Component, Summoned b_CombatSource)
        {
            {
                var mFindTheWay = b_Component.Parent.GetFindTheWay2D(b_CombatSource);
                if (mFindTheWay == null)
                {
                    b_CombatSource.Enemy = null;
                    b_CombatSource.TargetEnemy = null;
                    return;
                }
                if (mFindTheWay.IsSafeArea)
                {
                    b_CombatSource.Enemy = null;
                    b_CombatSource.TargetEnemy = null;
                    return;
                }
            }
            if (b_CombatSource.UnitData.Index != b_CombatSource.GamePlayer.UnitData.Index) return;

            var mTempEnemy = b_CombatSource.GamePlayer.Enemy;
            if (mTempEnemy != null && mTempEnemy.IsDisposeable == false && mTempEnemy.IsDeath == false && mTempEnemy.Identity == E_Identity.Hero)
            {
                var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mTempEnemy);
                if (mFindTheWay != null)
                {
                    if (mFindTheWay.IsSafeArea)
                    {
                        b_CombatSource.TargetEnemy = null;
                    }
                    else
                    {
                        b_CombatSource.TargetEnemy = mTempEnemy;

                        var mDistance = 10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, mFindTheWay.Vector2Pos);
                        if (mDistance <= b_CombatSource.Config.AR)
                        {
                            b_CombatSource.Enemy = b_CombatSource.TargetEnemy;
                        }
                        return;
                    }
                }
            }

            MapComponent mMapComponent = b_Component.Parent;
            if (b_CombatSource.Enemy != null)
            {
                if (b_CombatSource.Enemy.IsDisposeable)
                {
                    b_CombatSource.Enemy = null;
                }
                else if (b_CombatSource.Enemy.IsDeath)
                {
                    b_CombatSource.Enemy = null;
                }
                else if (b_CombatSource.Enemy.UnitData.Index != mMapComponent.MapId)
                {
                    b_CombatSource.Enemy = null;
                }
                else
                {
                    Vector2 mTargetEnemyPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.Enemy).Vector2Pos;
                    if (Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource.GamePlayer).Vector2Pos, mTargetEnemyPoint) > b_CombatSource.Config.Ran2)
                    {// 是否脱战了
                        b_CombatSource.Enemy = null;
                    }
                    else if (10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, mTargetEnemyPoint) > b_CombatSource.Config.AR)
                    {// 攻击范围内
                        b_CombatSource.Enemy = null;
                    }
                }
            }

            if (b_CombatSource.Enemy == null)
            {
                if (b_CombatSource.TargetEnemy != null)
                {
                    if (b_CombatSource.TargetEnemy.IsDisposeable)
                    {
                        b_CombatSource.TargetEnemy = null;
                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else if (b_CombatSource.TargetEnemy.IsDeath)
                    {
                        b_CombatSource.TargetEnemy = null;
                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else if (b_CombatSource.TargetEnemy.UnitData.Index != mMapComponent.MapId)
                    {
                        b_CombatSource.TargetEnemy = null;

                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else
                    {
                        Vector2 mTargetEnemyPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.TargetEnemy).Vector2Pos;
                        if (Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource.GamePlayer).Vector2Pos, mTargetEnemyPoint) > b_CombatSource.Config.Ran2)
                        {// 是否脱战了
                            b_CombatSource.TargetEnemy = null;

                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                        }
                        float distance = 10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, mTargetEnemyPoint);
                        if (distance > b_CombatSource.Config.VR)
                        {
                            b_CombatSource.TargetEnemy = null;

                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                        }
                    }
                }
                if (b_CombatSource.TargetEnemy == null)
                {
                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;

                    var mCenterMapCellField = b_Component.Parent.GetMapCellField(b_CombatSource);
                    if (mCenterMapCellField != null)
                    {
                        E_PKModel mCurrnetPKModel = b_CombatSource.PKModel();

                        bool mIsHasTeam = false;
                        Dictionary<long, Player> mDic = null;
                        if (mCurrnetPKModel == E_PKModel.Friend)
                        {
                            var mTeamComponent = b_CombatSource.GamePlayer.Player.GetCustomComponent<TeamComponent>();
                            if (mTeamComponent != null)
                            {
                                TeamManageComponent mTeamManageComponent = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                                mDic = mTeamManageComponent.GetAllByTeamID(mTeamComponent.TeamID);
                                mIsHasTeam = mDic != null && mDic.ContainsKey(b_CombatSource.GamePlayer.InstanceId);
                            }
                        }
                        var mAttackerFanJiIdlist = b_CombatSource.GetFanJiIdlist();

                        float distance = b_CombatSource.Config.VR, distance2 = b_CombatSource.Config.Ran2;
                        CombatSource mTargetGamePlayer = null, mTargetGamePlayer2 = null;
                        Vector2 mCurrnetPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos;
                        Vector2 mSourcePoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.GamePlayer).Vector2Pos;
                        var mMapCellFieldlist = mCenterMapCellField.AroundFieldArray;
                        for (int i = 0, len = mMapCellFieldlist.Length; i < len; i++)
                        {
                            var mMapCellField = mMapCellFieldlist[i];

                            if (mMapCellField.FieldEnemyDic.Count > 0)
                            {
                                var mFieldEnemylist = mMapCellField.FieldEnemyDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldEnemylist.Length; j < jlen; j++)
                                {
                                    var mFieldEnemy = mFieldEnemylist[j];
                                    if (mFieldEnemy == null) continue;
                                    if (mFieldEnemy.IsDisposeable) continue;
                                    if (mFieldEnemy.IsDeath) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldEnemy);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    Vector2 mFieldEnemyPoint = mFindTheWay.Vector2Pos;

                                    float mSourceDistance = Vector2.Distance(mSourcePoint, mFieldEnemyPoint);
                                    if (mSourceDistance > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }
                                    if (mSourceDistance < distance2)
                                    {
                                        distance2 = mSourceDistance;
                                        mTargetGamePlayer2 = mFieldEnemy;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldEnemyPoint);
                                    if (mTempDistance > b_CombatSource.Config.VR)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldEnemy;
                                    }
                                }
                            }

                            if (mMapCellField.FieldPlayerDic.Count > 0)
                            {
                                var mFieldPlayerlist = mMapCellField.FieldPlayerDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldPlayerlist.Length; j < jlen; j++)
                                {
                                    var mFieldPlayer = mFieldPlayerlist[j];
                                    if (mFieldPlayer == null) continue;
                                    if (mFieldPlayer.IsDisposeable) continue;
                                    if (mFieldPlayer.IsDeath) continue;
                                    if (mFieldPlayer.InstanceId == b_CombatSource.GamePlayer.InstanceId) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayer);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    var mTryUseResult = b_CombatSource.TryUseByPlayerKilling(mFieldPlayer, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                                    if (mTryUseResult == false) continue;

                                    Vector2 mFieldPlayerPoint = mFindTheWay.Vector2Pos;

                                    float mSourceDistance = Vector2.Distance(mSourcePoint, mFieldPlayerPoint);
                                    if (mSourceDistance > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }
                                    if (mSourceDistance < distance2)
                                    {
                                        distance2 = mSourceDistance;
                                        mTargetGamePlayer2 = mFieldPlayer;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldPlayerPoint);
                                    if (mTempDistance > b_CombatSource.Config.VR)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldPlayer;
                                    }
                                }
                            }
                            if (mMapCellField.FieldPetsDic.Count > 0)
                            {
                                var mFieldPetslist = mMapCellField.FieldPetsDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldPetslist.Length; j < jlen; j++)
                                {
                                    var mFieldPlayerPets = mFieldPetslist[j];
                                    if (mFieldPlayerPets == null) continue;
                                    if (mFieldPlayerPets.IsDisposeable || mFieldPlayerPets.GamePlayer.IsDisposeable) continue;
                                    if (mFieldPlayerPets.IsDeath || mFieldPlayerPets.GamePlayer.IsDeath) continue;
                                    if (mFieldPlayerPets.GamePlayer.InstanceId == b_CombatSource.GamePlayer.InstanceId) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayerPets);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    var mTryUseResult = b_CombatSource.TryUseByPlayerKilling(mFieldPlayerPets, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                                    if (mTryUseResult == false) continue;

                                    Vector2 mFieldPetsPoint = mFindTheWay.Vector2Pos;

                                    float mSourceDistance = Vector2.Distance(mSourcePoint, mFieldPetsPoint);
                                    if (mSourceDistance > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }
                                    if (mSourceDistance < distance2)
                                    {
                                        distance2 = mSourceDistance;
                                        mTargetGamePlayer2 = mFieldPlayerPets;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldPetsPoint);
                                    if (mTempDistance > b_CombatSource.Config.VR)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldPlayerPets;
                                    }
                                }
                            }
                            if (mMapCellField.FieldSummonedDic.Count > 0)
                            {
                                var mFieldSummonedlist = mMapCellField.FieldSummonedDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldSummonedlist.Length; j < jlen; j++)
                                {
                                    var mFieldPlayerSummoned = mFieldSummonedlist[j];
                                    if (mFieldPlayerSummoned == null) continue;
                                    if (mFieldPlayerSummoned.IsDisposeable || mFieldPlayerSummoned.GamePlayer.IsDisposeable) continue;
                                    if (mFieldPlayerSummoned.IsDeath || mFieldPlayerSummoned.GamePlayer.IsDeath) continue;
                                    if (mFieldPlayerSummoned.GamePlayer.InstanceId == b_CombatSource.GamePlayer.InstanceId) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayerSummoned);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    var mTryUseResult = b_CombatSource.TryUseByPlayerKilling(mFieldPlayerSummoned, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                                    if (mTryUseResult == false) continue;

                                    Vector2 mFieldSummonedPoint = mFindTheWay.Vector2Pos;

                                    float mSourceDistance = Vector2.Distance(mSourcePoint, mFieldSummonedPoint);
                                    if (mSourceDistance > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }
                                    if (mSourceDistance < distance2)
                                    {
                                        distance2 = mSourceDistance;
                                        mTargetGamePlayer2 = mFieldPlayerSummoned;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldSummonedPoint);
                                    if (mTempDistance > b_CombatSource.Config.VR)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldPlayerSummoned;
                                    }
                                }
                            }
                        }

                        if (mTargetGamePlayer != null)
                        {
                            b_CombatSource.TargetEnemy = mTargetGamePlayer;
                        }
                        else if (mTargetGamePlayer2 != null)
                        {
                            b_CombatSource.TargetEnemy = mTargetGamePlayer2;
                        }
                    }
                }

                mTempEnemy = b_CombatSource.TargetEnemy;
                if (mTempEnemy != null && mTempEnemy.IsDisposeable == false && mTempEnemy.IsDeath == false)
                {
                    var mDistance = 10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, b_Component.Parent.GetFindTheWay2D(mTempEnemy).Vector2Pos);
                    if (mDistance <= b_CombatSource.Config.AR)
                    {
                        b_CombatSource.Enemy = b_CombatSource.TargetEnemy;
                    }
                }
            }
        }
        private static void WarEnemyTargetUpdate(this BattleComponent b_Component, Pets b_CombatSource)
        {
            {
                var mFindTheWay = b_Component.Parent.GetFindTheWay2D(b_CombatSource);
                if (mFindTheWay == null)
                {
                    b_CombatSource.Enemy = null;
                    b_CombatSource.TargetEnemy = null;
                    return;
                }
                if (mFindTheWay.IsSafeArea)
                {
                    b_CombatSource.Enemy = null;
                    b_CombatSource.TargetEnemy = null;
                    return;
                }
            }

            if (b_CombatSource.UnitData.Index != b_CombatSource.GamePlayer.UnitData.Index) return;

            //var mTempEnemy = b_CombatSource.GamePlayer.Enemy;//屏蔽宠物PVP
            //if (mTempEnemy != null && mTempEnemy.IsDisposeable == false && mTempEnemy.IsDeath == false && mTempEnemy.Identity == E_Identity.Hero)
            //{
            //    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mTempEnemy);
            //    if (mFindTheWay != null)
            //    {
            //        if (mFindTheWay.IsSafeArea)
            //        {
            //            b_CombatSource.TargetEnemy = null;
            //        }
            //        else
            //        {
            //            b_CombatSource.TargetEnemy = mTempEnemy;

            //            var mDistance = 10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, mFindTheWay.Vector2Pos);
            //            if (mDistance <= b_CombatSource.Config.AttackDistance)
            //            {
            //                b_CombatSource.Enemy = b_CombatSource.TargetEnemy;
            //            }
            //            return;
            //        }
            //    }
            //}

            MapComponent mMapComponent = b_Component.Parent as MapComponent;
            if (b_CombatSource.Enemy != null)
            {
                if (b_CombatSource.Enemy.IsDisposeable)
                {
                    b_CombatSource.Enemy = null;
                }
                else if (b_CombatSource.Enemy.IsDeath)
                {
                    b_CombatSource.Enemy = null;
                }
                else if (b_CombatSource.Enemy.UnitData.Index != mMapComponent.MapId)
                {
                    b_CombatSource.Enemy = null;
                }
                else
                {
                    Vector2 mCurrnetPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos;
                    Vector2 mTargetEnemyPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.Enemy).Vector2Pos;
                    if (Vector2.Distance(mCurrnetPoint, mTargetEnemyPoint) > b_CombatSource.Config.Ran)
                    {// 是否脱战了
                        b_CombatSource.Enemy = null;
                    }
                    else if (10 * Vector2.Distance(mCurrnetPoint, mTargetEnemyPoint) > b_CombatSource.Config.AttackDistance)
                    {// 攻击范围内
                        b_CombatSource.Enemy = null;
                    }
                }
            }

            if (b_CombatSource.Enemy == null)
            {
                if (b_CombatSource.TargetEnemy != null)
                {
                    if (b_CombatSource.TargetEnemy.IsDisposeable)
                    {
                        b_CombatSource.TargetEnemy = null;
                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else if (b_CombatSource.TargetEnemy.IsDeath)
                    {
                        b_CombatSource.TargetEnemy = null;
                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else if (b_CombatSource.TargetEnemy.UnitData.Index != mMapComponent.MapId)
                    {
                        b_CombatSource.TargetEnemy = null;

                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else
                    {
                        Vector2 mCurrnetPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos;
                        Vector2 mTargetEnemyPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.TargetEnemy).Vector2Pos;
                        if (Vector2.Distance(mCurrnetPoint, mTargetEnemyPoint) > b_CombatSource.Config.Ran)
                        {// 是否脱战了
                            b_CombatSource.TargetEnemy = null;

                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                        }
                        else if (10 * Vector2.Distance(mCurrnetPoint, mTargetEnemyPoint) > b_CombatSource.Config.AttackDistance)
                        {// 攻击范围内
                            b_CombatSource.TargetEnemy = null;

                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                        }
                    }
                }
                if (b_CombatSource.TargetEnemy == null)
                {
                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;

                    var mCenterMapCellField = b_Component.Parent.GetMapCellField(b_CombatSource);
                    if (mCenterMapCellField != null)
                    {
                        E_PKModel mCurrnetPKModel = b_CombatSource.PKModel();

                        bool mIsHasTeam = false;
                        Dictionary<long, Player> mDic = null;
                        if (mCurrnetPKModel == E_PKModel.Friend)
                        {
                            var mTeamComponent = b_CombatSource.GamePlayer.Player.GetCustomComponent<TeamComponent>();
                            if (mTeamComponent != null)
                            {
                                TeamManageComponent mTeamManageComponent = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                                mDic = mTeamManageComponent.GetAllByTeamID(mTeamComponent.TeamID);
                                mIsHasTeam = mDic != null && mDic.ContainsKey(b_CombatSource.GamePlayer.InstanceId);
                            }
                        }
                        var mAttackerFanJiIdlist = b_CombatSource.GetFanJiIdlist();

                        float distance = b_CombatSource.Config.VR;
                        CombatSource mTargetGamePlayer = null;
                        Vector2 mCurrnetPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos;
                        Vector2 mSourcePoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.GamePlayer).Vector2Pos;
                        var mMapCellFieldlist = mCenterMapCellField.AroundFieldDic.Values.ToArray();
                        for (int i = 0, len = mMapCellFieldlist.Length; i < len; i++)
                        {
                            var mMapCellField = mMapCellFieldlist[i];

                            if (mMapCellField.FieldEnemyDic.Count > 0)
                            {
                                var mFieldEnemylist = mMapCellField.FieldEnemyDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldEnemylist.Length; j < jlen; j++)
                                {
                                    var mFieldEnemy = mFieldEnemylist[j];
                                    if (mFieldEnemy == null) continue;
                                    if (mFieldEnemy.IsDisposeable) continue;
                                    if (mFieldEnemy.IsDeath) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldEnemy);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    Vector2 mFieldEnemyPoint = mFindTheWay.Vector2Pos;

                                    if (Vector2.Distance(mSourcePoint, mFieldEnemyPoint) > b_CombatSource.Config.Ran)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldEnemyPoint);
                                    if (mTempDistance > b_CombatSource.Config.VR)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldEnemy;
                                    }
                                }
                            }
                            //屏蔽宠物PVP
                            //if (mMapCellField.FieldPlayerDic.Count > 0)
                            //{
                            //    var mFieldPlayerlist = mMapCellField.FieldPlayerDic.Values.ToArray();
                            //    for (int j = 0, jlen = mFieldPlayerlist.Length; j < jlen; j++)
                            //    {
                            //        var mFieldPlayer = mFieldPlayerlist[j];
                            //        if (mFieldPlayer == null) continue;
                            //        if (mFieldPlayer.IsDisposeable) continue;
                            //        if (mFieldPlayer.IsDeath) continue;
                            //        if (mFieldPlayer.InstanceId == b_CombatSource.GamePlayer.InstanceId) continue;

                            //        var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayer);
                            //        if (mFindTheWay == null) continue;
                            //        if (mFindTheWay.IsSafeArea) continue;

                            //        var mTryUseResult = b_CombatSource.TryUseByPlayerKilling(mFieldPlayer, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                            //        if (mTryUseResult == false) continue;

                            //        Vector2 mFieldPlayerPoint = mFindTheWay.Vector2Pos;

                            //        if (Vector2.Distance(mSourcePoint, mFieldPlayerPoint) > b_CombatSource.Config.Ran)
                            //        {// 是否脱战了
                            //            continue;
                            //        }

                            //        float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldPlayerPoint);
                            //        if (mTempDistance > b_CombatSource.Config.VR)
                            //        {// 是否脱战了
                            //            continue;
                            //        }

                            //        if (mTempDistance < distance)
                            //        {
                            //            distance = mTempDistance;
                            //            mTargetGamePlayer = mFieldPlayer;
                            //        }
                            //    }
                            //}
                            //if (mMapCellField.FieldPetsDic.Count > 0)
                            //{
                            //    var mFieldPetslist = mMapCellField.FieldPetsDic.Values.ToArray();

                            //    for (int j = 0, jlen = mFieldPetslist.Length; j < jlen; j++)
                            //    {
                            //        var mFieldPlayerPets = mFieldPetslist[j];
                            //        if (mFieldPlayerPets == null) continue;
                            //        if (mFieldPlayerPets.IsDisposeable || mFieldPlayerPets.GamePlayer.IsDisposeable) continue;
                            //        if (mFieldPlayerPets.IsDeath || mFieldPlayerPets.GamePlayer.IsDeath) continue;
                            //        if (mFieldPlayerPets.GamePlayer.InstanceId == b_CombatSource.GamePlayer.InstanceId) continue;

                            //        var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayerPets);
                            //        if (mFindTheWay == null) continue;
                            //        if (mFindTheWay.IsSafeArea) continue;

                            //        var mTryUseResult = b_CombatSource.TryUseByPlayerKilling(mFieldPlayerPets, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                            //        if (mTryUseResult == false) continue;

                            //        Vector2 mFieldPetsPoint = mFindTheWay.Vector2Pos;

                            //        if (Vector2.Distance(mCurrnetPoint, mFieldPetsPoint) > b_CombatSource.Config.Ran)
                            //        {// 是否脱战了
                            //            continue;
                            //        }

                            //        float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldPetsPoint);
                            //        if (mTempDistance > b_CombatSource.Config.VR)
                            //        {// 是否脱战了
                            //            continue;
                            //        }

                            //        if (mTempDistance < distance)
                            //        {
                            //            distance = mTempDistance;
                            //            mTargetGamePlayer = mFieldPlayerPets;
                            //        }
                            //    }
                            //}
                            //if (mMapCellField.FieldSummonedDic.Count > 0)
                            //{
                            //    var mFieldSummonedlist = mMapCellField.FieldSummonedDic.Values.ToArray();
                            //    for (int j = 0, jlen = mFieldSummonedlist.Length; j < jlen; j++)
                            //    {
                            //        var mFieldPlayerSummoned = mFieldSummonedlist[j];
                            //        if (mFieldPlayerSummoned == null) continue;
                            //        if (mFieldPlayerSummoned.IsDisposeable || mFieldPlayerSummoned.GamePlayer.IsDisposeable) continue;
                            //        if (mFieldPlayerSummoned.IsDeath || mFieldPlayerSummoned.GamePlayer.IsDeath) continue;
                            //        if (mFieldPlayerSummoned.GamePlayer.InstanceId == b_CombatSource.GamePlayer.InstanceId) continue;

                            //        var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayerSummoned);
                            //        if (mFindTheWay == null) continue;
                            //        if (mFindTheWay.IsSafeArea) continue;

                            //        var mTryUseResult = b_CombatSource.TryUseByPlayerKilling(mFieldPlayerSummoned, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                            //        if (mTryUseResult == false) continue;

                            //        Vector2 mFieldSummonedPoint = mFindTheWay.Vector2Pos;

                            //        if (Vector2.Distance(mCurrnetPoint, mFieldSummonedPoint) > b_CombatSource.Config.Ran)
                            //        {// 是否脱战了
                            //            continue;
                            //        }

                            //        float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldSummonedPoint);
                            //        if (mTempDistance > b_CombatSource.Config.VR)
                            //        {// 是否脱战了
                            //            continue;
                            //        }

                            //        if (mTempDistance < distance)
                            //        {
                            //            distance = mTempDistance;
                            //            mTargetGamePlayer = mFieldPlayerSummoned;
                            //        }
                            //    }
                            //}
                        }

                        if (mTargetGamePlayer != null)
                        {
                            b_CombatSource.TargetEnemy = mTargetGamePlayer;
                        }
                    }
                }

                var mTempEnemy = b_CombatSource.TargetEnemy;
                if (mTempEnemy != null && mTempEnemy.IsDisposeable == false && mTempEnemy.IsDeath == false)
                {
                    var mDistance = 10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, b_Component.Parent.GetFindTheWay2D(mTempEnemy).Vector2Pos);
                    if (mDistance <= b_CombatSource.Config.AttackDistance)
                    {
                        b_CombatSource.Enemy = b_CombatSource.TargetEnemy;
                    }
                }
            }
        }


        private static void WarEnemyTargetUpdate(this BattleComponent b_Component, HolyteacherSummoned b_CombatSource)
        {
            {// 安全区 或者不再当前地图
                var mFindTheWay = b_Component.Parent.GetFindTheWay2D(b_CombatSource);
                if (mFindTheWay == null)
                {
                    b_CombatSource.Enemy = null;
                    b_CombatSource.TargetEnemy = null;
                    return;
                }
                if (mFindTheWay.IsSafeArea)
                {
                    b_CombatSource.Enemy = null;
                    b_CombatSource.TargetEnemy = null;
                    return;
                }
            }

            // 玩家的目标覆盖当前目标 如果玩家当前的目标为玩家时
            var mTempEnemy = b_CombatSource.GamePlayer.Enemy;
            if (mTempEnemy != null && mTempEnemy.IsDisposeable == false && mTempEnemy.IsDeath == false && mTempEnemy.Identity == E_Identity.Hero)
            {
                var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mTempEnemy);
                if (mFindTheWay != null)
                {
                    if (mFindTheWay.IsSafeArea)
                    {
                        b_CombatSource.TargetEnemy = null;
                    }
                    else
                    {
                        b_CombatSource.TargetEnemy = mTempEnemy;

                        var mDistance = 10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, mFindTheWay.Vector2Pos);
                        if (mDistance <= b_CombatSource.Config.AR)
                        {
                            b_CombatSource.Enemy = b_CombatSource.TargetEnemy;
                        }
                        return;
                    }
                }
            }

            MapComponent mMapComponent = b_Component.Parent;
            if (b_CombatSource.Enemy != null)
            {
                if (b_CombatSource.Enemy.IsDisposeable)
                {
                    b_CombatSource.Enemy = null;
                }
                else if (b_CombatSource.Enemy.IsDeath)
                {
                    b_CombatSource.Enemy = null;
                }
                else if (b_CombatSource.Enemy.UnitData.Index != mMapComponent.MapId)
                {
                    b_CombatSource.Enemy = null;
                }
                else
                {
                    Vector2 mTargetEnemyPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.Enemy).Vector2Pos;
                    if (Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource.GamePlayer).Vector2Pos, mTargetEnemyPoint) > b_CombatSource.Config.Ran2)
                    {// 是否脱战了
                        b_CombatSource.Enemy = null;
                    }
                    else if (10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, mTargetEnemyPoint) > b_CombatSource.Config.AR)
                    {// 攻击范围内
                        b_CombatSource.Enemy = null;
                    }
                }
            }

            if (b_CombatSource.Enemy == null)
            {
                if (b_CombatSource.TargetEnemy != null)
                {
                    if (b_CombatSource.TargetEnemy.IsDisposeable)
                    {
                        b_CombatSource.TargetEnemy = null;
                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else if (b_CombatSource.TargetEnemy.IsDeath)
                    {
                        b_CombatSource.TargetEnemy = null;
                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else if (b_CombatSource.TargetEnemy.UnitData.Index != mMapComponent.MapId)
                    {
                        b_CombatSource.TargetEnemy = null;

                        if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                    }
                    else
                    {
                        Vector2 mTargetEnemyPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.TargetEnemy).Vector2Pos;
                        if (Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource.GamePlayer).Vector2Pos, mTargetEnemyPoint) > b_CombatSource.Config.Ran2)
                        {// 是否脱战了
                            b_CombatSource.TargetEnemy = null;

                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                        }
                        float distance = 10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, mTargetEnemyPoint);
                        if (distance > b_CombatSource.Config.VR)
                        {
                            b_CombatSource.TargetEnemy = null;

                            if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;
                        }
                    }
                }
                if (b_CombatSource.TargetEnemy == null)
                {
                    if (b_CombatSource.Pathlist != null) b_CombatSource.Pathlist = null;

                    var mCenterMapCellField = b_Component.Parent.GetMapCellField(b_CombatSource);
                    if (mCenterMapCellField != null)
                    {
                        E_PKModel mCurrnetPKModel = b_CombatSource.PKModel();

                        bool mIsHasTeam = false;
                        Dictionary<long, Player> mDic = null;
                        if (mCurrnetPKModel == E_PKModel.Friend)
                        {
                            var mTeamComponent = b_CombatSource.GamePlayer.Player.GetCustomComponent<TeamComponent>();
                            if (mTeamComponent != null)
                            {
                                TeamManageComponent mTeamManageComponent = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                                mDic = mTeamManageComponent.GetAllByTeamID(mTeamComponent.TeamID);
                                mIsHasTeam = mDic != null && mDic.ContainsKey(b_CombatSource.GamePlayer.InstanceId);
                            }
                        }
                        var mAttackerFanJiIdlist = b_CombatSource.GetFanJiIdlist();

                        float distance = b_CombatSource.Config.VR, distance2 = b_CombatSource.Config.Ran2;
                        CombatSource mTargetGamePlayer = null, mTargetGamePlayer2 = null;
                        Vector2 mCurrnetPoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos;
                        Vector2 mSourcePoint = b_Component.Parent.GetFindTheWay2D(b_CombatSource.GamePlayer).Vector2Pos;

                        var mMapCellFieldlist = mCenterMapCellField.AroundFieldArray;
                        for (int i = 0, len = mMapCellFieldlist.Length; i < len; i++)
                        {
                            var mMapCellField = mMapCellFieldlist[i];

                            if (mMapCellField.FieldEnemyDic.Count > 0)
                            {
                                var mFieldEnemylist = mMapCellField.FieldEnemyDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldEnemylist.Length; j < jlen; j++)
                                {
                                    var mFieldEnemy = mFieldEnemylist[j];
                                    if (mFieldEnemy == null) continue;
                                    if (mFieldEnemy.IsDisposeable) continue;
                                    if (mFieldEnemy.IsDeath) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldEnemy);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    Vector2 mFieldEnemyPoint = mFindTheWay.Vector2Pos;

                                    float mSourceDistance = Vector2.Distance(mSourcePoint, mFieldEnemyPoint);
                                    if (mSourceDistance > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }
                                    if (mSourceDistance < distance2)
                                    {
                                        distance2 = mSourceDistance;
                                        mTargetGamePlayer2 = mFieldEnemy;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldEnemyPoint);
                                    if (mTempDistance > b_CombatSource.Config.VR)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldEnemy;
                                    }
                                }
                            }

                            if (mMapCellField.FieldPlayerDic.Count > 0)
                            {
                                var mFieldPlayerlist = mMapCellField.FieldPlayerDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldPlayerlist.Length; j < jlen; j++)
                                {
                                    var mFieldPlayer = mFieldPlayerlist[j];
                                    if (mFieldPlayer == null) continue;
                                    if (mFieldPlayer.IsDisposeable) continue;
                                    if (mFieldPlayer.IsDeath) continue;
                                    if (mFieldPlayer.InstanceId == b_CombatSource.GamePlayer.InstanceId) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayer);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    var mTryUseResult = b_CombatSource.TryUseByPlayerKilling(mFieldPlayer, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                                    if (mTryUseResult == false) continue;

                                    Vector2 mFieldPlayerPoint = mFindTheWay.Vector2Pos;

                                    float mSourceDistance = Vector2.Distance(mSourcePoint, mFieldPlayerPoint);
                                    if (mSourceDistance > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }
                                    if (mSourceDistance < distance2)
                                    {
                                        distance2 = mSourceDistance;
                                        mTargetGamePlayer2 = mFieldPlayer;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldPlayerPoint);
                                    if (mTempDistance > b_CombatSource.Config.VR)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldPlayer;
                                    }
                                }
                            }
                            if (mMapCellField.FieldPetsDic.Count > 0)
                            {
                                var mFieldPetslist = mMapCellField.FieldPetsDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldPetslist.Length; j < jlen; j++)
                                {
                                    var mFieldPlayerPets = mFieldPetslist[j];
                                    if (mFieldPlayerPets == null) continue;
                                    if (mFieldPlayerPets.IsDisposeable || mFieldPlayerPets.GamePlayer.IsDisposeable) continue;
                                    if (mFieldPlayerPets.IsDeath || mFieldPlayerPets.GamePlayer.IsDeath) continue;
                                    if (mFieldPlayerPets.GamePlayer.InstanceId == b_CombatSource.GamePlayer.InstanceId) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayerPets);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    var mTryUseResult = b_CombatSource.TryUseByPlayerKilling(mFieldPlayerPets, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                                    if (mTryUseResult == false) continue;

                                    Vector2 mFieldPetsPoint = mFindTheWay.Vector2Pos;

                                    float mSourceDistance = Vector2.Distance(mSourcePoint, mFieldPetsPoint);
                                    if (mSourceDistance > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }
                                    if (mSourceDistance < distance2)
                                    {
                                        distance2 = mSourceDistance;
                                        mTargetGamePlayer2 = mFieldPlayerPets;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldPetsPoint);
                                    if (mTempDistance > b_CombatSource.Config.VR)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldPlayerPets;
                                    }
                                }
                            }
                            if (mMapCellField.FieldSummonedDic.Count > 0)
                            {
                                var mFieldSummonedlist = mMapCellField.FieldSummonedDic.Values.ToArray();
                                for (int j = 0, jlen = mFieldSummonedlist.Length; j < jlen; j++)
                                {
                                    var mFieldPlayerSummoned = mFieldSummonedlist[j];
                                    if (mFieldPlayerSummoned == null) continue;
                                    if (mFieldPlayerSummoned.IsDisposeable || mFieldPlayerSummoned.GamePlayer.IsDisposeable) continue;
                                    if (mFieldPlayerSummoned.IsDeath || mFieldPlayerSummoned.GamePlayer.IsDeath) continue;
                                    if (mFieldPlayerSummoned.GamePlayer.InstanceId == b_CombatSource.GamePlayer.InstanceId) continue;

                                    var mFindTheWay = b_Component.Parent.GetFindTheWay2D(mFieldPlayerSummoned);
                                    if (mFindTheWay == null) continue;
                                    if (mFindTheWay.IsSafeArea) continue;

                                    var mTryUseResult = b_CombatSource.TryUseByPlayerKilling(mFieldPlayerSummoned, mAttackerFanJiIdlist, mCurrnetPKModel, mIsHasTeam, mDic);
                                    if (mTryUseResult == false) continue;

                                    Vector2 mFieldSummonedPoint = mFindTheWay.Vector2Pos;

                                    float mSourceDistance = Vector2.Distance(mSourcePoint, mFieldSummonedPoint);
                                    if (mSourceDistance > b_CombatSource.Config.Ran2)
                                    {// 是否脱战了
                                        continue;
                                    }
                                    if (mSourceDistance < distance2)
                                    {
                                        distance2 = mSourceDistance;
                                        mTargetGamePlayer2 = mFieldPlayerSummoned;
                                    }

                                    float mTempDistance = 10 * Vector2.Distance(mCurrnetPoint, mFieldSummonedPoint);
                                    if (mTempDistance > b_CombatSource.Config.VR)
                                    {// 是否脱战了
                                        continue;
                                    }

                                    if (mTempDistance < distance)
                                    {
                                        distance = mTempDistance;
                                        mTargetGamePlayer = mFieldPlayerSummoned;
                                    }
                                }
                            }
                        }

                        if (mTargetGamePlayer != null)
                        {
                            b_CombatSource.TargetEnemy = mTargetGamePlayer;
                        }
                        else if (mTargetGamePlayer2 != null)
                        {
                            b_CombatSource.TargetEnemy = mTargetGamePlayer2;
                        }
                    }
                }

                mTempEnemy = b_CombatSource.TargetEnemy;
                if (mTempEnemy != null && mTempEnemy.IsDisposeable == false && mTempEnemy.IsDeath == false)
                {
                    var mDistance = 10 * Vector2.Distance(b_Component.Parent.GetFindTheWay2D(b_CombatSource).Vector2Pos, b_Component.Parent.GetFindTheWay2D(mTempEnemy).Vector2Pos);
                    if (mDistance <= b_CombatSource.Config.AR)
                    {
                        b_CombatSource.Enemy = b_CombatSource.TargetEnemy;
                    }
                }
            }
        }

    }
}
