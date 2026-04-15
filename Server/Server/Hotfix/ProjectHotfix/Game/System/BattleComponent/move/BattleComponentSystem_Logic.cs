using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using CustomFrameWork;
using System.Linq;

namespace ETHotfix
{

    public static partial class BattleComponentSystem
    {
        private static bool Logic(this BattleComponent b_Component, GamePlayer b_CombatSource)
        {
            //if (b_CombatSource.IsAttacking) return true;
            //// 检查状态 合不合适战斗
            //b_Component.CombatStateUpdate(b_CombatSource);

            //if (b_CombatSource.IsAttacking) return true;
            //// 更新下战斗目标 优先前方目标
            //b_Component.WarEnemyTargetUpdate(b_CombatSource);

            //if (b_CombatSource.IsAttacking) return true;
            //// 是否处于眩晕,压制 等异常状态中
            //if (b_CombatSource.IsCanOperation() == false) return true;

            //if (b_CombatSource.Enemy != null)
            //{

            //    //b_CombatSource.Attack(b_BattleHero.Enemy, b_Component);
            //}



            return true;
        }
        private static bool Logic(this BattleComponent b_Component, Enemy b_CombatSource, long b_CurrentTime)
        {
            if (b_CombatSource.IsAttacking) return true;
            // 检查状态 合不合适战斗
            b_Component.CombatStateUpdate(b_CombatSource);

            if (b_CombatSource.MoveRestTime > b_CurrentTime)
            {
                return true;
            }

            if (b_CombatSource.IsAttacking) return true;
            // 是否处于眩晕,压制 等异常状态中
            if (b_CombatSource.IsCanOperation() == false) return true;

            if (b_CombatSource.IsAttacking) return true;
            if (b_CombatSource.IsCheckState(1)) return false;//不攻击的怪
            // 更新下战斗目标 优先前方目标
            b_Component.WarEnemyTargetUpdate(b_CombatSource);

            if (b_CombatSource.TargetEnemy != null)
            {
                bool UseSkill()
                {
                    var mRandomWeight = Help_RandomHelper.Range(0, b_CombatSource.Config.AttackTypeDicSum);
                    var mDropWeight = 0;

                    var mWeightKeyArray = b_CombatSource.Config.AttackTypeDic.Keys.ToArray();
                    for (int i = 0, len = mWeightKeyArray.Length; i < len; i++)
                    {
                        var mWeightKey = mWeightKeyArray[i];
                        var mWeight = b_CombatSource.Config.AttackTypeDic[mWeightKey];

                        mDropWeight += mWeight;
                        if (mRandomWeight <= mDropWeight)
                        {
                            if (b_CombatSource.SkillGroup.TryGetValue(mWeightKey, out var mSingleSkill))
                            {
                                if (b_CombatSource.TargetEnemy.InstanceId == b_CombatSource.InstanceId) return false;

                                if (mSingleSkill.NextAttackTime > b_Component.CurrentTimeTick)
                                {
                                    return false;
                                }

                                var mAttackTargetCell = b_Component.Parent.GetFindTheWay2D(b_CombatSource.TargetEnemy);
                                if (mAttackTargetCell == null || mAttackTargetCell.IsSafeArea)
                                {
                                    return false;
                                }

                                var mTargetEnemy = mSingleSkill.FindTarget(b_CombatSource, b_CombatSource.TargetEnemy, mAttackTargetCell, b_Component, null);
                                if (mTargetEnemy == null)
                                {
                                    return false;
                                }
                                //if (mTargetEnemy.InstanceId != b_CombatSource.TargetEnemy.InstanceId)
                                //{
                                //    mAttackTargetCell = b_Component.Parent.GetFindTheWay2D(mTargetEnemy);
                                //}

                                var mTryUseResult = mSingleSkill.TryUse(b_CombatSource, mTargetEnemy, mAttackTargetCell, b_Component, null);
                                if (mTryUseResult == false)
                                {// 转移动
                                    return false;
                                }
                                var mUseResult = mSingleSkill.UseSkill(b_CombatSource, mTargetEnemy, b_Component);
                                if (mUseResult)
                                {// 使用技能成功

                                    var mCoolTime = mSingleSkill.GetCoolTime(b_CombatSource);
                                    if (b_CombatSource.Config.AtSpeed > mCoolTime)
                                    {
                                        mCoolTime = b_CombatSource.Config.AtSpeed;
                                    }
                                    mSingleSkill.NextAttackTime = b_Component.CurrentTimeTick + mCoolTime;

                                    return true;
                                }
                                break;
                            }
                        }
                    }
                    return false;
                }

                bool mUseSkillResult = false;
                if (b_CombatSource.Config.Monster_Type == 0)
                {
                    if (b_CombatSource.InCasting == false && b_CombatSource.Config.AttackTypeDic.Count != 0)
                        mUseSkillResult = UseSkill();
                    else if (b_CombatSource.Enemy != null && mUseSkillResult == false)
                    {
                        b_CombatSource.Attack(b_CombatSource.Enemy, b_Component);
                        return true;
                    }

                    return mUseSkillResult;
                }
                else
                {
                    if (b_CombatSource.InCasting == false && b_CombatSource.Config.AttackTypeDic.Count != 0)
                        mUseSkillResult = UseSkill();
                    if (b_CombatSource.Enemy != null && mUseSkillResult == false)
                    {
                        b_CombatSource.Attack(b_CombatSource.Enemy, b_Component);
                        return true;
                    }
                }
            }

            return false;
        }
        private static bool Logic(this BattleComponent b_Component, Summoned b_CombatSource, long b_CurrentTime)
        {
            if (b_CombatSource.IsAttacking) return true;
            // 检查状态 合不合适战斗
            b_Component.CombatStateUpdate(b_CombatSource);

            if (b_CombatSource.MoveRestTime > b_CurrentTime)
            {
                return true;
            }

            if (b_CombatSource.IsAttacking) return true;
            // 是否处于眩晕,压制 等异常状态中
            if (b_CombatSource.IsCanOperation() == false) return true;

            if (b_CombatSource.IsAttacking) return true;
            // 更新下战斗目标 优先前方目标
            b_Component.WarEnemyTargetUpdate(b_CombatSource);

            if (b_CombatSource.TargetEnemy != null)
            {
                bool UseSkill()
                {
                    var mRandomWeight = Help_RandomHelper.Range(0, b_CombatSource.Config.AttackTypeDicSum);
                    var mDropWeight = 0;

                    var mWeightKeyArray = b_CombatSource.Config.AttackTypeDic.Keys.ToArray();
                    for (int i = 0, len = mWeightKeyArray.Length; i < len; i++)
                    {
                        var mWeightKey = mWeightKeyArray[i];
                        var mWeight = b_CombatSource.Config.AttackTypeDic[mWeightKey];

                        mDropWeight += mWeight;
                        if (mRandomWeight <= mDropWeight)
                        {
                            if (b_CombatSource.SkillGroup.TryGetValue(mWeightKey, out var mSingleSkill))
                            {
                                if (b_CombatSource.TargetEnemy.InstanceId == b_CombatSource.GamePlayer.InstanceId) return false;

                                if (mSingleSkill.NextAttackTime > b_Component.CurrentTimeTick)
                                {
                                    return false;
                                }

                                var mAttackTargetCell = b_Component.Parent.GetFindTheWay2D(b_CombatSource.TargetEnemy);
                                if (mAttackTargetCell == null || mAttackTargetCell.IsSafeArea)
                                {
                                    return false;
                                }

                                var mTargetEnemy = mSingleSkill.FindTarget(b_CombatSource, b_CombatSource.TargetEnemy, mAttackTargetCell, b_Component, null);
                                if (mTargetEnemy == null)
                                {
                                    return false;
                                }
                                //if (mTargetEnemy.InstanceId != b_CombatSource.TargetEnemy.InstanceId)
                                //{
                                //    mAttackTargetCell = b_Component.Parent.GetFindTheWay2D(mTargetEnemy);
                                //}

                                var mTryUseResult = mSingleSkill.TryUse(b_CombatSource, mTargetEnemy, mAttackTargetCell, b_Component, null);
                                if (mTryUseResult == false)
                                {// 转移动
                                    return false;
                                }
                                var mUseResult = mSingleSkill.UseSkill(b_CombatSource, mTargetEnemy, b_Component);
                                if (mUseResult)
                                {// 使用技能成功

                                    var mCoolTime = mSingleSkill.GetCoolTime(b_CombatSource);
                                    if (b_CombatSource.Config.AtSpeed > mCoolTime)
                                    {
                                        mCoolTime = b_CombatSource.Config.AtSpeed;
                                    }
                                    mSingleSkill.NextAttackTime = b_Component.CurrentTimeTick + mCoolTime;

                                    return true;
                                }
                                break;
                            }
                        }
                    }
                    return false;
                }

                bool mUseSkillResult = false;
                if (b_CombatSource.Config.Monster_Type == 0)
                {
                    if (b_CombatSource.InCasting == false && b_CombatSource.Config.AttackTypeDic.Count != 0)
                        mUseSkillResult = UseSkill();
                    else if (b_CombatSource.Enemy != null && mUseSkillResult == false)
                    {
                        b_CombatSource.Attack(b_CombatSource.Enemy, b_Component);
                        return true;
                    }
                    return mUseSkillResult;
                }
                else
                {
                    if (b_CombatSource.InCasting == false && b_CombatSource.Config.AttackTypeDic.Count != 0)
                        mUseSkillResult = UseSkill();
                    if (b_CombatSource.Enemy != null && mUseSkillResult == false)
                    {
                        b_CombatSource.Attack(b_CombatSource.Enemy, b_Component);
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool Logic(this BattleComponent b_Component, Pets b_CombatSource, long b_CurrentTime)
        {
            if (b_CombatSource.IsAttacking) return true;
            // 检查状态 合不合适战斗
            b_Component.CombatStateUpdate(b_CombatSource);

            if (b_CombatSource.MoveRestTime > b_CurrentTime)
            {
                return true;
            }

            if (b_CombatSource.IsAttacking) return true;
            // 是否处于眩晕,压制 等异常状态中
            if (b_CombatSource.IsCanOperation() == false) return true;

            if (b_CombatSource.IsAttacking) return true;
            // 更新下战斗目标 优先前方目标
            b_Component.WarEnemyTargetUpdate(b_CombatSource);
            
            bool UseSkill()
            {
                if (b_CombatSource.TargetEnemy != null && b_CombatSource.SkillCurrent != null)
                {
                    if (b_CombatSource.TargetEnemy.InstanceId == b_CombatSource.GamePlayer.InstanceId) return false;
                    //
                    var mAttackTargetCell = b_Component.Parent.GetFindTheWay2D(b_CombatSource.TargetEnemy);
                    if (mAttackTargetCell == null || mAttackTargetCell.IsSafeArea)
                    {
                        return false;
                    }
                    
                    var mSingleSkill = b_CombatSource.SkillCurrent;

                    if (mSingleSkill.NextAttackTime > b_Component.CurrentTimeTick)
                    {
                        return false;
                    }
                    var mTargetEnemy = mSingleSkill.FindTarget(b_CombatSource, b_CombatSource.TargetEnemy, mAttackTargetCell, b_Component, null);
                    if (mTargetEnemy == null)
                    {
                        return false;
                    }
                   
                    var mTryUseResult = mSingleSkill.TryUse(b_CombatSource, mTargetEnemy, mAttackTargetCell, b_Component, null);
                    if (mTryUseResult)
                    {
                        var mUseResult = mSingleSkill.UseSkill(b_CombatSource, mTargetEnemy, b_Component);
                        if (mUseResult)
                        {
                            mSingleSkill.NextAttackTime = b_Component.CurrentTimeTick + mSingleSkill.GetCoolTime(b_CombatSource);

                            b_CombatSource.dBPetsData.PetsMP -= mSingleSkill.MP;
                            if (b_CombatSource.dBPetsData.PetsMP <= 0) b_CombatSource.dBPetsData.PetsMP = 0;
                            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_CombatSource.GamePlayer.Player.GameAreaId);
                            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)b_CombatSource.GamePlayer.Player.GameAreaId);
                            mWriteDataComponent.Save(b_CombatSource.dBPetsData, dBProxy).Coroutine();
                            return true;
                        }
                    }
                }
                return false;
            }
            if (b_CombatSource.Enemy != null && !UseSkill())
            {
                if(b_CombatSource.dBPetsData.UseSkillID == 0)
                    b_CombatSource.Attack(b_CombatSource.Enemy, b_Component);
                return true;
            }
            return false;
        }

        private static bool Logic(this BattleComponent b_Component, HolyteacherSummoned b_CombatSource, long b_CurrentTime)
        {
            if (b_CombatSource.IsAttacking) return true;
            // 检查状态 合不合适战斗
            b_Component.CombatStateUpdate(b_CombatSource);

            if (b_CombatSource.MoveRestTime > b_CurrentTime)
            {
                return true;
            }

            if (b_CombatSource.IsAttacking) return true;
            // 是否处于眩晕,压制 等异常状态中
            if (b_CombatSource.IsCanOperation() == false) return true;

            if (b_CombatSource.IsAttacking) return true;
            // 更新下战斗目标 优先前方目标
            b_Component.WarEnemyTargetUpdate(b_CombatSource);

            if (b_CombatSource.TargetEnemy != null)
            {
                bool UseSkill()
                {
                    var mRandomWeight = Help_RandomHelper.Range(0, b_CombatSource.Config.AttackTypeDicSum);
                    var mDropWeight = 0;

                    var mWeightKeyArray = b_CombatSource.Config.AttackTypeDic.Keys.ToArray();
                    for (int i = 0, len = mWeightKeyArray.Length; i < len; i++)
                    {
                        var mWeightKey = mWeightKeyArray[i];
                        var mWeight = b_CombatSource.Config.AttackTypeDic[mWeightKey];

                        mDropWeight += mWeight;
                        if (mRandomWeight <= mDropWeight)
                        {
                            if (b_CombatSource.SkillGroup.TryGetValue(mWeightKey, out var mSingleSkill))
                            {
                                if (b_CombatSource.TargetEnemy.InstanceId == b_CombatSource.GamePlayer.InstanceId) return false;

                                if (mSingleSkill.NextAttackTime > b_Component.CurrentTimeTick)
                                {
                                    return false;
                                }

                                var mAttackTargetCell = b_Component.Parent.GetFindTheWay2D(b_CombatSource.TargetEnemy);
                                if (mAttackTargetCell == null || mAttackTargetCell.IsSafeArea)
                                {
                                    return false;
                                }

                                var mTargetEnemy = mSingleSkill.FindTarget(b_CombatSource, b_CombatSource.TargetEnemy, mAttackTargetCell, b_Component, null);
                                if (mTargetEnemy == null)
                                {
                                    return false;
                                }
                                //if (mTargetEnemy.InstanceId != b_CombatSource.TargetEnemy.InstanceId)
                                //{
                                //    mAttackTargetCell = b_Component.Parent.GetFindTheWay2D(mTargetEnemy);
                                //}

                                var mTryUseResult = mSingleSkill.TryUse(b_CombatSource, mTargetEnemy, mAttackTargetCell, b_Component, null);
                                if (mTryUseResult == false)
                                {// 转移动
                                    return false;
                                }
                                var mUseResult = mSingleSkill.UseSkill(b_CombatSource, mTargetEnemy, b_Component);
                                if (mUseResult)
                                {// 使用技能成功

                                    var mCoolTime = mSingleSkill.GetCoolTime(b_CombatSource);
                                    if (b_CombatSource.Config.AtSpeed > mCoolTime)
                                    {
                                        mCoolTime = b_CombatSource.Config.AtSpeed;
                                    }
                                    mSingleSkill.NextAttackTime = b_Component.CurrentTimeTick + mCoolTime;

                                    return true;
                                }
                                break;
                            }
                        }
                    }
                    return false;
                }

                bool mUseSkillResult = false;
                if (b_CombatSource.Config.Monster_Type == 0)
                {
                    if (b_CombatSource.InCasting == false && b_CombatSource.Config.AttackTypeDic.Count != 0)
                        mUseSkillResult = UseSkill();
                    //else if (b_CombatSource.Enemy != null && mUseSkillResult == false)
                    //{
                    //    b_CombatSource.Attack(b_CombatSource.Enemy, b_Component);
                    //    return true;
                    //}
                    return mUseSkillResult;
                }
                else
                {
                    if (b_CombatSource.InCasting == false && b_CombatSource.Config.AttackTypeDic.Count != 0)
                        mUseSkillResult = UseSkill();
                    //if (b_CombatSource.Enemy != null && mUseSkillResult == false)
                    //{
                    //    b_CombatSource.Attack(b_CombatSource.Enemy, b_Component);
                    //    return true;
                    //}
                }
            }

            return false;
        }
    }
}
