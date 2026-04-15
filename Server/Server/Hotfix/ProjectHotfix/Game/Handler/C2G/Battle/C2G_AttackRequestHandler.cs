using ETModel;
using System;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using UnityEngine;
using System.Collections.Generic;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_AttackRequestHandler : AMActorRpcHandler<C2G_AttackRequest, G2C_AttackResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AttackRequest b_Request, G2C_AttackResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AttackRequest b_Request, G2C_AttackResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer.IsDeath)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(400);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("请等待复活后尝试攻击!");
                b_Reply(b_Response);
                return false;
            }
            mPlayer.ClientTime.ClientTime = b_Request.Tick;
            #region 攻击条件判断

            //if (mGamePlayer.IsAttacking)
            //{
            //    b_Response.Tick = mGamePlayer.AttackTime;

            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(401);
            //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("正在攻击!");
            //    b_Reply(b_Response);
            //    return false;
            //}
            //else
            if (mGamePlayer.IsCanOperation() == false)
            {
                b_Response.Tick = mGamePlayer.AttackTime;

                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(402);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("异常状态,不能攻击!");
                b_Reply(b_Response);
                return false;
            }

            /*var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            if (mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(mData.Index, out var mMapConfig) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(516);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }*/

            MapComponent mapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, mPlayer.GameUserId);
            if (mapComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(518);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                b_Reply(b_Response);
                return false;
            }
            /*if (mMapConfig.IsCopyMap == 0)
            {
                if (mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(mData.Index, out mapComponent) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(516);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                    b_Reply(b_Response);
                    return false;
                }
            }
            else
            {
                mapComponent = mServerArea.GetCustomComponent<BatteCopyManagerComponent>().GetRoomMapComponent(mData.Index, mPlayer.GameUserId);
                if (mapComponent == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(518);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("地图数据异常!");
                    b_Reply(b_Response);
                    return false;
                }
            }*/
            var mFindTheWay = mapComponent.GetFindTheWay2D(mGamePlayer);
            if (mFindTheWay.IsSafeArea)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(421);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("安全区内不能攻击!");
                b_Reply(b_Response);
                return false;
            }

            if (mapComponent.TryGetPosX(b_Request.PosX) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(417);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常x,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            var mAttackTargetCell = mapComponent.GetFindTheWay2D(b_Request.PosX, b_Request.PosY);
            if (mAttackTargetCell == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(418);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置数据异常y,不可行走!");
                b_Reply(b_Response);
                return false;
            }
            if (mAttackTargetCell.IsSafeArea)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(421);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("安全区内不能攻击!");
                b_Reply(b_Response);
                return false;
            }
            if (mAttackTargetCell.IsStaticObstacle)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(440);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("安全区内不能攻击!");
                b_Reply(b_Response);
                return false;
            }

            var mMapCellField = mapComponent.GetMapCellField(mAttackTargetCell);
            if (mMapCellField == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(423);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置区域数据异常x,不能攻击!");
                b_Reply(b_Response);
                return false;
            }
            #endregion

            if (b_Request.AttackType == 0)
            {
                if (mGamePlayer.InstanceId == b_Request.GameUserId)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不能攻击友方单位!");
                    b_Reply(b_Response);
                    return false;
                }

                Vector2 mSelfPos = mFindTheWay.Vector2Pos;
                Vector2 mTargetPos = mAttackTargetCell.Vector2Pos;
                if (Vector2.Distance(mSelfPos, mTargetPos) > mGamePlayer.GetNumerial(E_GameProperty.AttackDistance))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(411);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标超出攻击范围");
                    b_Reply(b_Response);
                    return false;
                }

                switch (mGamePlayer.Identity)
                {
                    case E_Identity.Hero:
                        {
                            switch ((E_GameOccupation)mGamePlayer.Data.PlayerTypeId)
                            {
                                case E_GameOccupation.Archer:
                                    {
                                        var mEquipmentComponent = mGamePlayer.Player.GetCustomComponent<EquipmentComponent>();
                                        if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Weapon, out var mWeaponEquipment))
                                        {
                                            if (mWeaponEquipment.Type == EItemType.Bows)
                                            {
                                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldEquipment))
                                                {
                                                    if (mShieldEquipment.Type == EItemType.Arrow)
                                                    {

                                                    }
                                                    else if (mShieldEquipment.ConfigID == 40019)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if (mGamePlayer.HealthStatsDic.ContainsKey(E_BattleSkillStats.WuYingJian216) == false)
                                                        {
                                                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(441);
                                                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                                                            return false;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (mGamePlayer.HealthStatsDic.ContainsKey(E_BattleSkillStats.WuYingJian216) == false)
                                                    {
                                                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(441);
                                                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                                                        return false;
                                                    }
                                                }
                                            }
                                            else if (mWeaponEquipment.Type == EItemType.Crossbows)
                                            {
                                                if (mEquipmentComponent.EquipPartItemDict.TryGetValue((int)EquipPosition.Shield, out var mShieldEquipment))
                                                {
                                                    if (mShieldEquipment.Type == EItemType.Arrow)
                                                    {

                                                    }
                                                    else if (mShieldEquipment.ConfigID == 50012)
                                                    {

                                                    }
                                                    else
                                                    {
                                                        if (mGamePlayer.HealthStatsDic.ContainsKey(E_BattleSkillStats.WuYingJian216) == false)
                                                        {
                                                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(441);
                                                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                                                            return false;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (mGamePlayer.HealthStatsDic.ContainsKey(E_BattleSkillStats.WuYingJian216) == false)
                                                    {
                                                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(441);
                                                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                                                        return false;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                        break;
                    default:
                        break;
                }

                if (false)
                {// 遍历
                    bool mFindResult = false;
                    var mBattleComponent = mapComponent.GetCustomComponent<BattleComponent>();
                    for (int i = 0, len = mMapCellField.AroundField.Count; i < len; i++)
                    {
                        var mAroundFieldIndex = mMapCellField.AroundField[i];

                        if (mMapCellField.AroundFieldDic.TryGetValue(mAroundFieldIndex, out var mCellField) == false) continue;

                        if (mCellField.FieldEnemyDic.TryGetValue(b_Request.GameUserId, out var mTargetEnemy))
                        {
                            mFindResult = true;
                            if (mTargetEnemy.IsDeath || mTargetEnemy.IsDisposeable)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(404);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                                b_Reply(b_Response);
                                return false;
                            }
                            else
                                mGamePlayer.Attack(mTargetEnemy, b_Request.Tick, mBattleComponent);
                            break;
                        }
                        else if (mCellField.FieldPlayerDic.TryGetValue(b_Request.GameUserId, out var mTargetPlayer))
                        {
                            mFindResult = true;
                            if (mTargetPlayer.IsDeath || mTargetPlayer.IsDisposeable)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(404);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                                b_Reply(b_Response);
                                return false;
                            }
                            else
                            {
                                var mTryUseResult = mGamePlayer.TryUseByPlayerKilling(mTargetPlayer, b_Response);
                                if (mTryUseResult == false)
                                {
                                    b_Reply(b_Response);
                                    return false;
                                }
                                mGamePlayer.Attack(mTargetPlayer, b_Request.Tick, mBattleComponent);
                            }
                            break;
                        }
                        else if (mCellField.FieldPetsDic.TryGetValue(b_Request.GameUserId, out var mTargetPets))
                        {
                            mFindResult = true;
                            if (mTargetPets.IsDeath || mTargetPets.IsDisposeable)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(404);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                                b_Reply(b_Response);
                                return false;
                            }
                            else if (mTargetPets.GamePlayer.InstanceId == mGamePlayer.InstanceId)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不能攻击友方单位!");
                                b_Reply(b_Response);
                                return false;
                            }
                            else
                            {
                                var mTryUseResult = mGamePlayer.TryUseByPlayerKilling(mTargetPets, b_Response);
                                if (mTryUseResult == false)
                                {
                                    b_Reply(b_Response);
                                    return false;
                                }
                                mGamePlayer.Attack(mTargetPets, b_Request.Tick, mBattleComponent);
                            }
                            break;
                        }
                        else if (mCellField.FieldSummonedDic.TryGetValue(b_Request.GameUserId, out var mTargetSummoned))
                        {
                            mFindResult = true;
                            if (mTargetSummoned.IsDeath || mTargetSummoned.IsDisposeable)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(404);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                                b_Reply(b_Response);
                                return false;
                            }
                            else if (mTargetSummoned.GamePlayer.InstanceId == mGamePlayer.InstanceId)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不能攻击友方单位!");
                                b_Reply(b_Response);
                                return false;
                            }
                            else
                            {
                                var mTryUseResult = mGamePlayer.TryUseByPlayerKilling(mTargetSummoned, b_Response);
                                if (mTryUseResult == false)
                                {
                                    b_Reply(b_Response);
                                    return false;
                                }
                                mGamePlayer.Attack(mTargetSummoned, b_Request.Tick, mBattleComponent);
                            }
                            break;
                        }
                    }

                    if (mFindResult == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(405);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("无效目标,不能攻击!");
                        b_Reply(b_Response);
                        return false;
                    }
                }
                else
                {
                    var mBattleComponent = mapComponent.GetCustomComponent<BattleComponent>();

                    if (mMapCellField.FieldEnemyDic.TryGetValue(b_Request.GameUserId, out var mTargetEnemy))
                    {
                        if (mTargetEnemy.IsDeath || mTargetEnemy.IsDisposeable)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(404);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                            b_Reply(b_Response);
                            return false;
                        }

                        var mWaitTaskTime = mGamePlayer.GetAttackSpeed();
                        if (mGamePlayer.NextAttackTime > mPlayer.ClientTime.ClientTime)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(401);
                            b_Reply(b_Response);
                            return false;
                        }

                        mGamePlayer.Attack(mTargetEnemy, b_Request.Tick, mBattleComponent);
                        mGamePlayer.NextAttackTime = mPlayer.ClientTime.ClientTime + mWaitTaskTime;
                    }
                    else if (mMapCellField.FieldPlayerDic.TryGetValue(b_Request.GameUserId, out var mTargetPlayer))
                    {
                        if (mTargetPlayer.IsDeath || mTargetPlayer.IsDisposeable)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(404);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                            b_Reply(b_Response);
                            return false;
                        }
                        else
                        {
                            var mTryUseResult = mGamePlayer.TryUseByPlayerKilling(mTargetPlayer, b_Response);
                            if (mTryUseResult == false)
                            {
                                b_Reply(b_Response);
                                return false;
                            }

                            var mWaitTaskTime = mGamePlayer.GetAttackSpeed();
                            if (mGamePlayer.NextAttackTime > mPlayer.ClientTime.ClientTime)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(401);
                                b_Reply(b_Response);
                                return false;
                            }

                            mGamePlayer.Attack(mTargetPlayer, b_Request.Tick, mBattleComponent);
                            mGamePlayer.NextAttackTime = mPlayer.ClientTime.ClientTime + mWaitTaskTime;
                        }
                    }
                    else if (mMapCellField.FieldPetsDic.TryGetValue(b_Request.GameUserId, out var mTargetPets))
                    {
                        if (mTargetPets.IsDeath || mTargetPets.IsDisposeable)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(404);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                            b_Reply(b_Response);
                            return false;
                        }
                        else if (mTargetPets.GamePlayer.InstanceId == mGamePlayer.InstanceId)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不能攻击友方单位!");
                            b_Reply(b_Response);
                            return false;
                        }
                        else
                        {
                            var mTryUseResult = mGamePlayer.TryUseByPlayerKilling(mTargetPets, b_Response);
                            if (mTryUseResult == false)
                            {
                                b_Reply(b_Response);
                                return false;
                            }

                            var mWaitTaskTime = mGamePlayer.GetAttackSpeed();
                            if (mGamePlayer.NextAttackTime > mPlayer.ClientTime.ClientTime)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(401);
                                b_Reply(b_Response);
                                return false;
                            }

                            mGamePlayer.Attack(mTargetPets, b_Request.Tick, mBattleComponent);
                            mGamePlayer.NextAttackTime = mPlayer.ClientTime.ClientTime + mWaitTaskTime;
                        }
                    }
                    else if (mMapCellField.FieldSummonedDic.TryGetValue(b_Request.GameUserId, out var mTargetSummoned))
                    {
                        if (mTargetSummoned.IsDeath || mTargetSummoned.IsDisposeable)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(404);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标已阵亡,不能攻击!");
                            b_Reply(b_Response);
                            return false;
                        }
                        else if (mTargetSummoned.GamePlayer.InstanceId == mGamePlayer.InstanceId)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不能攻击友方单位!");
                            b_Reply(b_Response);
                            return false;
                        }
                        else
                        {
                            var mTryUseResult = mGamePlayer.TryUseByPlayerKilling(mTargetSummoned, b_Response);
                            if (mTryUseResult == false)
                            {
                                b_Reply(b_Response);
                                return false;
                            }

                            var mWaitTaskTime = mGamePlayer.GetAttackSpeed();
                            if (mGamePlayer.NextAttackTime > mPlayer.ClientTime.ClientTime)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(401);
                                b_Reply(b_Response);
                                return false;
                            }

                            mGamePlayer.Attack(mTargetSummoned, b_Request.Tick, mBattleComponent);
                            mGamePlayer.NextAttackTime = mPlayer.ClientTime.ClientTime + mWaitTaskTime;
                        }
                    }
                    else
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(405);
                        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("无效目标,不能攻击!");
                        b_Reply(b_Response);
                        return false;
                    }
                }
            }
            else
            {//技能
                int mUseSkillId = (int)b_Request.AttackType;
                if (mGamePlayer.SkillGroup.TryGetValue(mUseSkillId, out var mSingleSkill) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(406);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("技能没有学习,此次攻击无效!");
                    b_Reply(b_Response);
                    return false;
                }

                switch ((E_GameOccupation)mGamePlayer.Data.PlayerTypeId)
                {
                    case E_GameOccupation.Spell:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SpellConfigJson>().JsonDic;
                            if (mJsonDic.ContainsKey(mUseSkillId) == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(407);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前职业没有此技能!");
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SwordsmanConfigJson>().JsonDic;
                            if (mJsonDic.ContainsKey(mUseSkillId) == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(407);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前职业没有此技能!");
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_ArcherConfigJson>().JsonDic;
                            if (mJsonDic.ContainsKey(mUseSkillId) == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(407);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前职业没有此技能!");
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SpellswordConfigJson>().JsonDic;
                            if (mJsonDic.ContainsKey(mUseSkillId) == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(407);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前职业没有此技能!");
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_HolyteacherConfigJson>().JsonDic;
                            if (mJsonDic.ContainsKey(mUseSkillId) == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(407);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前职业没有此技能!");
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SummonWarlockConfigJson>().JsonDic;
                            if (mJsonDic.ContainsKey(mUseSkillId) == false)
                            {
                                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(407);
                                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前职业没有此技能!");
                                b_Reply(b_Response);
                                return false;
                            }
                        }
                        break;
                    case E_GameOccupation.Combat:
                        break;
                    case E_GameOccupation.GrowLancer:
                        break;
                    default:
                        break;
                }

                var mBattleComponent = mapComponent.GetCustomComponent<BattleComponent>();

                if (mSingleSkill.NextAttackTime > mBattleComponent.CurrentTimeTick)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(442);
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("技能冷却,无法使用!");
                    b_Reply(b_Response);
                    return false;
                }

                var mTargetEnemy = mSingleSkill.FindTarget(mGamePlayer, b_Request.GameUserId, mAttackTargetCell, mBattleComponent, b_Response);
                if (mTargetEnemy == null)
                {
                    if (b_Response.Error == 0)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                    }
                    b_Reply(b_Response);
                    return false;
                }
                var mTryUseResult = mSingleSkill.TryUse(mGamePlayer, mTargetEnemy, mAttackTargetCell, mBattleComponent, b_Response);
                if (mTryUseResult == false)
                {
                    if (b_Response.Error == 0)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                    }
                    //b_Response.Error = ErrorCodeHotfix.ERR_DBProxyNotFoundError;
                    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage(mTryUseResult.Item2);
                    b_Reply(b_Response);
                    return false;
                }

                if (mGamePlayer.NextAttackTime > mPlayer.ClientTime.ClientTime)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(401);
                    b_Reply(b_Response);
                    return false;
                }
                mGamePlayer.NextAttackTime = mPlayer.ClientTime.ClientTime;

                var mUseResult = mSingleSkill.UseSkill(mGamePlayer, mTargetEnemy, mAttackTargetCell, mBattleComponent);
                if (mUseResult)
                {
                    var mMpConsumeRate = mGamePlayer.GetNumerial(E_GameProperty.MpConsumeRate_Reduce);
                    int mp = (int)(mSingleSkill.MP * (100 - mMpConsumeRate) / 100f);

                    mGamePlayer.UnitData.Mp -= mp;
                    mGamePlayer.UnitData.AG -= (mSingleSkill.AG - mSingleSkill.AG * mGamePlayer.GetNumerial(E_GameProperty.EmbedAGDecrease));
                    if (mGamePlayer.UnitData.Mp < 0) mGamePlayer.UnitData.Mp = 0;
                    if (mGamePlayer.UnitData.AG < 0) mGamePlayer.UnitData.AG = 0;
                    G2C_ChangeValue_notice b_ChangeValue_notice = new G2C_ChangeValue_notice();
                    G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                    mBattleKVData.Key = (int)E_GameProperty.PROP_AG;
                    mBattleKVData.Value = mGamePlayer.GetNumerial(E_GameProperty.PROP_AG);
                    G2C_BattleKVData mBattleKVData1 = new G2C_BattleKVData();
                    mBattleKVData1.Key = (int)E_GameProperty.PROP_MP;
                    mBattleKVData1.Value = mGamePlayer.GetNumerial(E_GameProperty.PROP_MP);
                    b_ChangeValue_notice.GameUserId = mPlayer.GameUserId;
                    b_ChangeValue_notice.Info.Add(mBattleKVData);
                    b_ChangeValue_notice.Info.Add(mBattleKVData1);
                    mPlayer.Send(b_ChangeValue_notice);
                    // 技能冷却
                    if (mSingleSkill.CoolTime > 0)
                    {
                        var mCoolTime = mSingleSkill.GetCoolTime(mGamePlayer);

                        mSingleSkill.NextAttackTime = mBattleComponent.CurrentTimeTick + mCoolTime;
                    }

                    bool isTriggerLianji = false;
                    C_HeroSkillSource mDoubleHitSkill = null;
                    switch ((E_GameOccupation)mGamePlayer.Data.PlayerTypeId)
                    {
                        case E_GameOccupation.None:
                            break;
                        case E_GameOccupation.Spell:
                            {
                                if (mGamePlayer.Data.OccupationLevel >= 2 &&
                                    mGamePlayer.SkillGroup.TryGetValue(27, out mDoubleHitSkill))
                                {
                                    isTriggerLianji = true;
                                }
                            }
                            break;
                        case E_GameOccupation.Swordsman:
                            {
                                if (mGamePlayer.Data.OccupationLevel >= 2 &&
                                    mGamePlayer.SkillGroup.TryGetValue(122, out mDoubleHitSkill))
                                {
                                    isTriggerLianji = true;
                                }
                            }
                            break;
                        case E_GameOccupation.Archer:
                            {
                                if (mGamePlayer.Data.OccupationLevel >= 2 &&
                                 mGamePlayer.SkillGroup.TryGetValue(220, out mDoubleHitSkill))
                                {
                                    isTriggerLianji = true;
                                }
                            }
                            break;
                        case E_GameOccupation.Spellsword:
                            {
                                if (mGamePlayer.Data.OccupationLevel >= 2 &&
                                 mGamePlayer.SkillGroup.TryGetValue(329, out mDoubleHitSkill))
                                {
                                    isTriggerLianji = true;
                                }
                            }
                            break;
                        case E_GameOccupation.Holyteacher:
                            {
                                if (mGamePlayer.Data.OccupationLevel >= 2 &&
                                 mGamePlayer.SkillGroup.TryGetValue(417, out mDoubleHitSkill))
                                {
                                    isTriggerLianji = true;
                                }
                            }
                            break;
                        case E_GameOccupation.SummonWarlock:
                            {
                                if (mGamePlayer.Data.OccupationLevel >= 2 &&
                                 mGamePlayer.SkillGroup.TryGetValue(525, out mDoubleHitSkill))
                                {
                                    isTriggerLianji = true;
                                }
                            }
                            break;
                        case E_GameOccupation.Combat:
                            break;
                        case E_GameOccupation.GrowLancer:
                            break;
                        default:
                            break;
                    }
                    if (isTriggerLianji && mDoubleHitSkill != null)
                    {
                        // 连击
                        var mLastUseSkillTime = mBattleComponent.CurrentTimeTick;
                        if (mGamePlayer.LastUseSkillTime + 2000 < mLastUseSkillTime)
                        {
                            mGamePlayer.doubleHitId = 0;
                            mGamePlayer.doubleHitId2 = 0;
                        }

                        switch (mSingleSkill.Id)
                        {
                            case 102:
                            case 110:
                                {
                                    mGamePlayer.doubleHitId = 0;
                                    mGamePlayer.doubleHitId2 = 0;
                                }
                                break;
                            default:
                                {
                                    if (mGamePlayer.doubleHitId == 0)
                                    {
                                        mGamePlayer.doubleHitId = mSingleSkill.Id;
                                        //if (mSingleSkill.Id == 103
                                        // || mSingleSkill.Id == 104
                                        // || mSingleSkill.Id == 105
                                        // || mSingleSkill.Id == 106
                                        // || mSingleSkill.Id == 107)
                                        //{
                                        //    mGamePlayer.doubleHitId = mSingleSkill.Id;
                                        //}
                                        //else
                                        //{
                                        //    mGamePlayer.doubleHitId = 0;
                                        //    mGamePlayer.doubleHitId2 = 0;
                                        //}
                                    }
                                    else if (mGamePlayer.doubleHitId2 == 0)
                                    {
                                        //if (mSingleSkill.Id == 108
                                        //  || mSingleSkill.Id == 111
                                        //  || mSingleSkill.Id == 113
                                        //  || mSingleSkill.Id == 116
                                        //  || mSingleSkill.Id == 117
                                        //  || mSingleSkill.Id == 119
                                        //  || mSingleSkill.Id == 120)
                                        if (mSingleSkill.Id != mGamePlayer.doubleHitId)
                                        {
                                            mGamePlayer.doubleHitId2 = mSingleSkill.Id;
                                        }
                                        else
                                        {
                                            mGamePlayer.doubleHitId = 0;
                                            mGamePlayer.doubleHitId2 = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (mGamePlayer.doubleHitId2 != mSingleSkill.Id && mGamePlayer.doubleHitId != mSingleSkill.Id)
                                        {
                                            //if (mSingleSkill.Id == 108
                                            //    || mSingleSkill.Id == 111
                                            //    || mSingleSkill.Id == 113
                                            //    || mSingleSkill.Id == 116
                                            //    || mSingleSkill.Id == 117
                                            //    || mSingleSkill.Id == 119
                                            //    || mSingleSkill.Id == 120)
                                            {
                                                mTryUseResult = mDoubleHitSkill.TryUse(mGamePlayer, mTargetEnemy, mAttackTargetCell, mBattleComponent, b_Response);
                                                if (mTryUseResult)
                                                {
                                                    mDoubleHitSkill.UseSkill(mGamePlayer, mTargetEnemy, mAttackTargetCell, mBattleComponent);
                                                }
                                            }
                                        }
                                        mGamePlayer.doubleHitId = 0;
                                        mGamePlayer.doubleHitId2 = 0;
                                    }
                                }
                                break;
                        }

                        mGamePlayer.LastUseSkillTime = mLastUseSkillTime;
                    }
                }
            }
            //b_Response.Tick = mGamePlayer.NextAttackTime;
            b_Reply(b_Response);
            return true;
        }
    }
}