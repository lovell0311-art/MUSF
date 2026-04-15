using System;
using System.Threading.Tasks;
using CustomFrameWork;
using ETModel;

namespace ETHotfix
{

    public static partial class C_HeroSkillSourceSystem
    {
        public static CombatSource FindTargetByBeAttackerId(this C_HeroSkillSource b_Component, CombatSource b_Attacker, long b_BeAttackerId, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            var mCellField = b_BattleComponent.Parent.GetMapCellField(b_Cell);

            if (mCellField.FieldPlayerDic.TryGetValue(b_BeAttackerId, out var mTargetPlayer))
            {
                if (mTargetPlayer.IsDeath || mTargetPlayer.IsDisposeable)
                {
                    if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(413);
                    return null;
                }

                if (b_Attacker.Identity != E_Identity.Enemy)
                {
                    if (mTargetPlayer.InstanceId == b_Attacker.GetPlayerInstance())
                    {
                        if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                        return null;
                    }
                    var mTryUseResult = b_Attacker.TryUseByPlayerKilling(mTargetPlayer, b_Response);
                    if (mTryUseResult == false)
                    {
                        return null;
                    }
                }
                
                return mTargetPlayer;
            }
            else if (b_Attacker.Identity != E_Identity.Enemy && mCellField.FieldEnemyDic.TryGetValue(b_BeAttackerId, out var mTargetEnemy))
            {
                if (mTargetEnemy.IsDeath || mTargetEnemy.IsDisposeable)
                {
                    if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(413);
                    return null;
                }
                return mTargetEnemy;
            }
            else if (mCellField.FieldPetsDic.TryGetValue(b_BeAttackerId, out var mTargetPet))
            {
                if (mTargetPet.IsDeath || mTargetPet.IsDisposeable)
                {
                    if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(413);
                    return null;
                }

                if (b_Attacker.Identity != E_Identity.Enemy)
                {
                    if (mTargetPet.GamePlayer.InstanceId == b_Attacker.GetPlayerInstance())
                    {
                        if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                        return null;
                    }
                    var mTryUseResult = b_Attacker.TryUseByPlayerKilling(mTargetPet, b_Response);
                    if (mTryUseResult == false)
                    {
                        return null;
                    }
                }

                return mTargetPet;
            }
            else if (mCellField.FieldSummonedDic.TryGetValue(b_BeAttackerId, out var mTargetSummoned))
            {
                if (mTargetSummoned.IsDeath || mTargetSummoned.IsDisposeable)
                {
                    if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(413);
                    return null;
                }

                if (b_Attacker.Identity != E_Identity.Enemy)
                {
                    if (mTargetSummoned.GamePlayer.InstanceId == b_Attacker.GetPlayerInstance())
                    {
                        if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                        return null;
                    }
                    var mTryUseResult = b_Attacker.TryUseByPlayerKilling(mTargetSummoned, b_Response);
                    if (mTryUseResult == false)
                    {
                        return null;
                    }
                }
                return mTargetSummoned;
            }
            else
            {
                if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(414);
                return null;
            }
        }

    }
}