using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using ETModel;
using Org.BouncyCastle.Tls.Crypto;

namespace ETHotfix
{
    public static partial class CombatSourceSystem
    {

        public static bool IsHitPvE(this CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent, bool b_CanDodge = false)
        {
            var mLastAttackTime = Help_TimeHelper.GetNowSecond();
            b_Attacker.LastAttackTime = mLastAttackTime;
            b_BeAttacker.LastAttackTime = mLastAttackTime;

            if (b_BeAttacker.HealthStatsDic.ContainsKey(E_BattleSkillStats.WuDi))
            {
                return false;
            }

            if (b_CanDodge)
            {
                float mHitRateTemp = b_BeAttacker.GetNumerialFunc(E_GameProperty.DefenseRate) / (b_Attacker.GetNumerialFunc(E_GameProperty.AtteckSuccessRate) * 1f);
                int mHitRate = (int)((1 - mHitRateTemp) * 100);
                if (mHitRate < 5) mHitRate = 5;
                if (mHitRate >= 100) return true;

                int mRandomValue = Help_RandomHelper.Range(0, 100);
                if (mRandomValue > mHitRate)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool PetsIsHitPvP(this Pets b_Attacker, GamePlayer b_BeAttacker, BattleComponent b_BattleComponent, bool b_CanDodge = false)
        {
            var mLastAttackTime = Help_TimeHelper.GetNowSecond();
            b_Attacker.LastAttackTime = mLastAttackTime;
            b_BeAttacker.LastAttackTime = mLastAttackTime;

            if (b_BeAttacker.HealthStatsDic.ContainsKey(E_BattleSkillStats.WuDi))
            {
                return false;
            }

            if (b_CanDodge)
            {
                float mHitRateF = (b_Attacker.GetNumerial(E_GameProperty.PVPAtteckSuccessRate) * 1f / (b_Attacker.GetNumerial(E_GameProperty.PVPAtteckSuccessRate) + b_BeAttacker.GetNumerial(E_GameProperty.PVPDefenseRate))) * (b_Attacker.dBPetsData.PetsLevel * 1f / (b_Attacker.dBPetsData.PetsLevel + b_BeAttacker.Data.Level));
                int mHitRate = (int)(mHitRateF * 1000);

                /*if (b_Attacker.Data.Level - b_BeAttacker.Data.Level >= 300)
                {
                    mHitRate -= 150;
                }
                else if (b_Attacker.Data.Level - b_BeAttacker.Data.Level >= 200)
                {
                    mHitRate -= 100;
                }
                else if (b_Attacker.Data.Level - b_BeAttacker.Data.Level >= 100)
                {
                    mHitRate -= 50;
                }*/

                if (mHitRate < 50) mHitRate = 50;
                if (mHitRate >= 1000) return true;

                int mRandomValue = Help_RandomHelper.Range(0, 1000);
                if (mRandomValue > mHitRate)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool IsHitPvP(this GamePlayer b_Attacker, GamePlayer b_BeAttacker, BattleComponent b_BattleComponent, bool b_CanDodge = false)
        {
            var mLastAttackTime = Help_TimeHelper.GetNowSecond();
            b_Attacker.LastAttackTime = mLastAttackTime;
            b_BeAttacker.LastAttackTime = mLastAttackTime;

            if (b_BeAttacker.HealthStatsDic.ContainsKey(E_BattleSkillStats.WuDi))
            {
                return false;
            }

            if (b_CanDodge)
            {
                float mHitRateF = (b_Attacker.GetNumerial(E_GameProperty.PVPAtteckSuccessRate) * 1f / (b_Attacker.GetNumerial(E_GameProperty.PVPAtteckSuccessRate) + b_BeAttacker.GetNumerial(E_GameProperty.PVPDefenseRate))) * (b_Attacker.Data.Level * 1f / (b_Attacker.Data.Level + b_BeAttacker.Data.Level));
                int mHitRate = (int)(mHitRateF * 1000);

                if (b_Attacker.Data.Level - b_BeAttacker.Data.Level >= 300)
                {
                    mHitRate -= 150;
                }
                else if (b_Attacker.Data.Level - b_BeAttacker.Data.Level >= 200)
                {
                    mHitRate -= 100;
                }
                else if (b_Attacker.Data.Level - b_BeAttacker.Data.Level >= 100)
                {
                    mHitRate -= 50;
                }

                if (mHitRate < 50) mHitRate = 50;
                if (mHitRate >= 1000) return true;

                int mRandomValue = Help_RandomHelper.Range(0, 1000);
                if (mRandomValue > mHitRate)
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsHitPvP(this CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent, bool b_CanDodge = false)
        {
            return (b_Attacker as GamePlayer).IsHitPvP(b_BeAttacker as GamePlayer, b_BattleComponent, b_CanDodge);
        }
        public static bool PetsIsHitPvP(this CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent, bool b_CanDodge = false)
        {
            return (b_Attacker as Pets).PetsIsHitPvP(b_BeAttacker as GamePlayer, b_BattleComponent, b_CanDodge);
        }

        public static bool TryUseByPlayerKilling(this CombatSource b_Attacker, long b_BeAttackerId, int b_BeAttackerLevel, long b_BeAttackWarAllianceID, IActorResponse b_Response)
        {
            if (b_BeAttackerLevel <= 50)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(439);
                return false;
            }

            var mCurrnetPKModel = b_Attacker.PKModel();
            if (mCurrnetPKModel == E_PKModel.AllBoddy)
            {
                return true;
            }

            var mBeAttackerId = b_BeAttackerId;
            if (b_Attacker.GetFanJiIdlist().TryGetValue(mBeAttackerId, out var mAttackerAttackTime) == false || mAttackerAttackTime + 60 < Help_TimeHelper.GetNowSecond())
            {
                if (mCurrnetPKModel == E_PKModel.Peace)
                {
                    if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(437);
                    return false;
                }

                if (mCurrnetPKModel == E_PKModel.Friend)
                {
                    long mAttackerId = 0;
                    TeamComponent mTeamComponent = null;
                    PlayerWarAllianceComponent mPlayerWarAllianceComponent = null;
                    switch (b_Attacker.Identity)
                    {
                        case E_Identity.Hero:
                            {
                                mAttackerId = b_Attacker.InstanceId;
                                mTeamComponent = (b_Attacker as GamePlayer).Player.GetCustomComponent<TeamComponent>();
                                mPlayerWarAllianceComponent = (b_Attacker as GamePlayer).Player.GetCustomComponent<PlayerWarAllianceComponent>();
                            }
                            break;
                        case E_Identity.Pet:
                            {
                                var mGamePlayer = (b_Attacker as Pets).GamePlayer;
                                mAttackerId = mGamePlayer.InstanceId;
                                mTeamComponent = mGamePlayer.Player.GetCustomComponent<TeamComponent>();
                                mPlayerWarAllianceComponent = mGamePlayer.Player.GetCustomComponent<PlayerWarAllianceComponent>();
                            }
                            break;
                        case E_Identity.Summoned:
                            {
                                var mGamePlayer = (b_Attacker as Summoned).GamePlayer;
                                mAttackerId = mGamePlayer.InstanceId;
                                mTeamComponent = mGamePlayer.Player.GetCustomComponent<TeamComponent>();
                                mPlayerWarAllianceComponent = mGamePlayer.Player.GetCustomComponent<PlayerWarAllianceComponent>();
                            }
                            break;
                        //case E_Identity.HolyteacherSummoned:
                        //    {
                        //        var mGamePlayer = (b_Attacker as HolyteacherSummoned).GamePlayer;
                        //        mAttackerId = mGamePlayer.InstanceId;
                        //        mTeamComponent = mGamePlayer.Player.GetCustomComponent<TeamComponent>();
                        //    }
                        //    break;
                        default:
                            break;
                    }

                    if (mTeamComponent != null)
                    {
                        TeamManageComponent mTeamManageComponent = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
                        var mDic = mTeamManageComponent.GetAllByTeamID(mTeamComponent.TeamID);
                        if (mDic != null && mDic.ContainsKey(mAttackerId) && mDic.ContainsKey(mBeAttackerId))
                        {
                            if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                            return false;
                        }
                    }
                    if (mPlayerWarAllianceComponent != null)
                    {
                        if (mPlayerWarAllianceComponent.WarAllianceID == b_BeAttackWarAllianceID && b_BeAttackWarAllianceID != 0&& mPlayerWarAllianceComponent.WarAllianceID!=0)
                        {
                            if (b_Response != null) b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(434);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static bool TryUseByPlayerKilling(this CombatSource b_Attacker, long b_BeAttackerId, int b_BeAttackerLevel, long b_BeAttackWarAllianceID, Dictionary<long, long> b_AttackerFanJiIdlist, E_PKModel b_PKModel, bool b_HasTeam, Dictionary<long, Player> b_TeamComponent)
        {
            if (b_BeAttackerLevel <= 50) return false;

            if (b_PKModel == E_PKModel.AllBoddy)
            {
                return true;
            }

            var mBeAttackerId = b_BeAttackerId;
            if (b_AttackerFanJiIdlist.TryGetValue(mBeAttackerId, out var mAttackerAttackTime) == false || mAttackerAttackTime + 60 < Help_TimeHelper.GetNowSecond())
            {
                if (b_PKModel == E_PKModel.Peace)
                {
                    return false;
                }

                if (b_HasTeam && b_TeamComponent.ContainsKey(mBeAttackerId))
                {
                    return false;
                }

                switch (b_Attacker.Identity)
                {
                    case E_Identity.Hero:
                        {
                            var mPlayerWarAllianceComponent = (b_Attacker as GamePlayer).Player.GetCustomComponent<PlayerWarAllianceComponent>();
                            if (mPlayerWarAllianceComponent.WarAllianceID == 0) return true;
                            if (mPlayerWarAllianceComponent.WarAllianceID == b_BeAttackWarAllianceID)
                            {
                                return false;
                            }
                        }
                        break;
                    case E_Identity.Pet:
                        {
                            var mGamePlayer = (b_Attacker as Pets).GamePlayer;
                            var mPlayerWarAllianceComponent = mGamePlayer.Player.GetCustomComponent<PlayerWarAllianceComponent>();

                            if (mPlayerWarAllianceComponent.WarAllianceID == b_BeAttackWarAllianceID)
                            {
                                return false;
                            }
                        }
                        break;
                    case E_Identity.Summoned:
                        {
                            var mGamePlayer = (b_Attacker as Summoned).GamePlayer;
                            var mPlayerWarAllianceComponent = mGamePlayer.Player.GetCustomComponent<PlayerWarAllianceComponent>();
                            if (mPlayerWarAllianceComponent.WarAllianceID == 0) return true;
                            if (mPlayerWarAllianceComponent.WarAllianceID == b_BeAttackWarAllianceID)
                            {
                                return false;
                            }
                        }
                        break;
                    //case E_Identity.HolyteacherSummoned:
                    //    {
                    //        var mGamePlayer = (b_Attacker as HolyteacherSummoned).GamePlayer;
                    //        mAttackerId = mGamePlayer.InstanceId;
                    //        mTeamComponent = mGamePlayer.Player.GetCustomComponent<TeamComponent>();
                    //    }
                    //    break;
                    default:
                        break;
                }
            }
            return true;
        }
        public static bool TryUseByPlayerKilling(this CombatSource b_Attacker, GamePlayer b_BeAttacker, Dictionary<long, long> b_AttackerFanJiIdlist, E_PKModel b_PKModel, bool b_HasTeam, Dictionary<long, Player> b_TeamComponent)
        {
            var mWarAllianceId = b_BeAttacker.Data.WarAllianceID;
            var mBeAttackerId = b_BeAttacker.InstanceId;
            var mBeAttackerLevel = b_BeAttacker.Data.Level;

            return b_Attacker.TryUseByPlayerKilling(mBeAttackerId, mBeAttackerLevel, mWarAllianceId, b_AttackerFanJiIdlist, b_PKModel, b_HasTeam, b_TeamComponent);
        }
        public static bool TryUseByPlayerKilling(this CombatSource b_Attacker, Summoned b_BeAttacker, Dictionary<long, long> b_AttackerFanJiIdlist, E_PKModel b_PKModel, bool b_HasTeam, Dictionary<long, Player> b_TeamComponent)
        {
            if (b_BeAttacker.GamePlayer.IsDisposeable) return false;

            var mWarAllianceId = b_BeAttacker.GamePlayer.Data.WarAllianceID;
            var mBeAttackerId = b_BeAttacker.GamePlayer.InstanceId;
            var mBeAttackerLevel = b_BeAttacker.GamePlayer.Data.Level;

            return b_Attacker.TryUseByPlayerKilling(mBeAttackerId, mBeAttackerLevel, mWarAllianceId, b_AttackerFanJiIdlist, b_PKModel, b_HasTeam, b_TeamComponent);
        }
        public static bool TryUseByPlayerKilling(this CombatSource b_Attacker, Pets b_BeAttacker, Dictionary<long, long> b_AttackerFanJiIdlist, E_PKModel b_PKModel, bool b_HasTeam, Dictionary<long, Player> b_TeamComponent)
        {
            if (b_BeAttacker.GamePlayer.IsDisposeable) return false;

            var mWarAllianceId = b_BeAttacker.GamePlayer.Data.WarAllianceID;
            var mBeAttackerId = b_BeAttacker.GamePlayer.InstanceId;
            var mBeAttackerLevel = b_BeAttacker.GamePlayer.Data.Level;

            return b_Attacker.TryUseByPlayerKilling(mBeAttackerId, mBeAttackerLevel, mWarAllianceId, b_AttackerFanJiIdlist, b_PKModel, b_HasTeam, b_TeamComponent);
        }
        public static bool TryUseByPlayerKilling(this CombatSource b_Attacker, GamePlayer b_BeAttacker, IActorResponse b_Response)
        {
            var mWarAllianceId = b_BeAttacker.Data.WarAllianceID;
            var mBeAttackerId = b_BeAttacker.InstanceId;
            var mBeAttackerLevel = b_BeAttacker.Data.Level;

            return b_Attacker.TryUseByPlayerKilling(mBeAttackerId, mBeAttackerLevel, mWarAllianceId, b_Response);
        }
        public static bool TryUseByPlayerKilling(this CombatSource b_Attacker, Summoned b_BeAttacker, IActorResponse b_Response)
        {
            if (b_BeAttacker.GamePlayer.IsDisposeable) return false;

            var mWarAllianceId = b_BeAttacker.GamePlayer.Data.WarAllianceID;
            var mBeAttackerId = b_BeAttacker.GamePlayer.InstanceId;
            var mBeAttackerLevel = b_BeAttacker.GamePlayer.Data.Level;

            return b_Attacker.TryUseByPlayerKilling(mBeAttackerId, mBeAttackerLevel, mWarAllianceId, b_Response);
        }
        public static bool TryUseByPlayerKilling(this CombatSource b_Attacker, Pets b_BeAttacker, IActorResponse b_Response)
        {
            if (b_BeAttacker.GamePlayer.IsDisposeable) return false;

            var mWarAllianceId = b_BeAttacker.GamePlayer.Data.WarAllianceID;
            var mBeAttackerId = b_BeAttacker.GamePlayer.InstanceId;
            var mBeAttackerLevel = b_BeAttacker.GamePlayer.Data.Level;
           
            return b_Attacker.TryUseByPlayerKilling(mBeAttackerId, mBeAttackerLevel, mWarAllianceId, b_Response);
        }

        public static int GetSkillUpValue(this GamePlayer self, int m_SkillId)
        {
            if (self == null) return 0;

            DataCacheManageComponent mDataCacheManageComponent = self.Player.GetCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
            if (mDataCache != null)
            {
                var mData = mDataCache.OnlyOne();
                if (mData != null)
                {
                    if (mData.SkillUpInfo.TryGetValue(m_SkillId, out int level))
                        return level;
                    else
                        return 0 ;
                }
            }
            return 0;
        }
    }
}