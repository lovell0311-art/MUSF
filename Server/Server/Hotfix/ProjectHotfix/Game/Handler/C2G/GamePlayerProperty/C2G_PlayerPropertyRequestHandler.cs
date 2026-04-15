using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Gse.V20191112.Models;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PlayerPropertyRequestHandler : AMActorRpcHandler<C2G_PlayerPropertyRequest, G2C_PlayerPropertyResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PlayerPropertyRequest b_Request, G2C_PlayerPropertyResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PlayerPropertyRequest b_Request, G2C_PlayerPropertyResponse b_Response, Action<IMessage> b_Reply)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mServerArea.GameAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
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

            if (b_Request.SelectId == 0)
            {
                var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();

                void AddPropertyData(E_GameProperty b_GameProperty, List<G2C_BattleKVData> b_Datalist)
                {
                    var mValue = mGamePlayer.GetNumerialFunc(b_GameProperty);
                    //if (mValue > 0)
                    {
                        G2C_BattleKVData mChildData = new G2C_BattleKVData();
                        mChildData.Key = (int)b_GameProperty;
                        mChildData.Value = mValue;
                        b_Datalist.Add(mChildData);
                    }
                }

                List<G2C_BattleKVData> mDatalist = new List<G2C_BattleKVData>();

                AddPropertyData(E_GameProperty.FreePoint, mDatalist);

                // 基本属性
                AddPropertyData(E_GameProperty.Property_Strength, mDatalist);
                AddPropertyData(E_GameProperty.Property_Willpower, mDatalist);
                AddPropertyData(E_GameProperty.Property_Agility, mDatalist);
                AddPropertyData(E_GameProperty.Property_BoneGas, mDatalist);
                AddPropertyData(E_GameProperty.Property_Command, mDatalist);
                // 二级属性
                AddPropertyData(E_GameProperty.MinAtteck, mDatalist);
                AddPropertyData(E_GameProperty.MaxAtteck, mDatalist);
                AddPropertyData(E_GameProperty.AtteckSuccessRate, mDatalist);
                AddPropertyData(E_GameProperty.PVPAtteckSuccessRate, mDatalist);

                AddPropertyData(E_GameProperty.Defense, mDatalist);
                AddPropertyData(E_GameProperty.AttackSpeed, mDatalist);
                AddPropertyData(E_GameProperty.DefenseRate, mDatalist);
                AddPropertyData(E_GameProperty.PVPDefenseRate, mDatalist);

                AddPropertyData(E_GameProperty.PROP_HP, mDatalist);
                AddPropertyData(E_GameProperty.PROP_HP_MAX, mDatalist);
                AddPropertyData(E_GameProperty.PROP_MP, mDatalist);
                AddPropertyData(E_GameProperty.PROP_MP_MAX, mDatalist);

                switch ((E_GameOccupation)mGamePlayer.Data.PlayerTypeId)
                {
                    case E_GameOccupation.Spell:
                        {
                            AddPropertyData(E_GameProperty.MinMagicAtteck, mDatalist);
                            AddPropertyData(E_GameProperty.MaxMagicAtteck, mDatalist);
                            AddPropertyData(E_GameProperty.MagicRate_Increase, mDatalist);
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            AddPropertyData(E_GameProperty.SkillAddition, mDatalist);
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            AddPropertyData(E_GameProperty.MinMagicAtteck, mDatalist);
                            AddPropertyData(E_GameProperty.MaxMagicAtteck, mDatalist);
                            AddPropertyData(E_GameProperty.MagicRate_Increase, mDatalist);
                            AddPropertyData(E_GameProperty.SkillAddition, mDatalist);
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            AddPropertyData(E_GameProperty.SkillAddition, mDatalist);
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            AddPropertyData(E_GameProperty.MinMagicAtteck, mDatalist);
                            AddPropertyData(E_GameProperty.MaxMagicAtteck, mDatalist);
                            AddPropertyData(E_GameProperty.MagicRate_Increase, mDatalist);
                            AddPropertyData(E_GameProperty.MinDamnationAtteck, mDatalist);
                            AddPropertyData(E_GameProperty.MaxDamnationAtteck, mDatalist);
                            AddPropertyData(E_GameProperty.DamnationRate_Increase, mDatalist);
                        }
                        break;
                    case E_GameOccupation.Combat:
                        {
                            AddPropertyData(E_GameProperty.AdvanceAttackPower, mDatalist);
                            AddPropertyData(E_GameProperty.RangeAttack, mDatalist);
                            AddPropertyData(E_GameProperty.SacredBeast, mDatalist);                            
                        }
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            AddPropertyData(E_GameProperty.DreamRiderPenalize, mDatalist);
                            AddPropertyData(E_GameProperty.DreamRiderIrritate, mDatalist);
                        }
                        break;
                    default:
                        break;
                }

                AddPropertyData(E_GameProperty.LucklyAttackRate, mDatalist);
                AddPropertyData(E_GameProperty.ExcellentAttackRate, mDatalist);
                AddPropertyData(E_GameProperty.InjuryValueRate_2, mDatalist);
                AddPropertyData(E_GameProperty.InjuryValueRate_3, mDatalist);
                AddPropertyData(E_GameProperty.AttackIgnoreDefenseRate, mDatalist);
                AddPropertyData(E_GameProperty.ReboundRate, mDatalist);
                // TODO 技能攻击力增加量
                //AddPropertyData(E_GameProperty.SkillAddition, mDatalist);

                AddPropertyData(E_GameProperty.LucklyAttackHurtValueIncrease, mDatalist);
                AddPropertyData(E_GameProperty.ExcellentAttackHurtValueIncrease, mDatalist);

                AddPropertyData(E_GameProperty.InjuryValueRate_Increase, mDatalist);
                AddPropertyData(E_GameProperty.InjuryValueRate_Reduce, mDatalist);
                AddPropertyData(E_GameProperty.InjuryValue_Reduce, mDatalist);
                AddPropertyData(E_GameProperty.BackInjuryRate, mDatalist);
                AddPropertyData(E_GameProperty.HurtValueAbsorbRate, mDatalist);

                AddPropertyData(E_GameProperty.ReplyHp, mDatalist);
                AddPropertyData(E_GameProperty.KillEnemyReplyHpRate, mDatalist);
                AddPropertyData(E_GameProperty.ReplyAllHpRate, mDatalist);
                // TODO 生命力吸收量
                //AddPropertyData(E_GameProperty.HpAbsorbRate, mDatalist);
                AddPropertyData(E_GameProperty.ReplyMp, mDatalist);
                AddPropertyData(E_GameProperty.KillEnemyReplyMpRate, mDatalist);
                AddPropertyData(E_GameProperty.ReplyAllMpRate, mDatalist);
                AddPropertyData(E_GameProperty.MpConsumeRate_Reduce, mDatalist);
                AddPropertyData(E_GameProperty.ReplyAG, mDatalist);
                AddPropertyData(E_GameProperty.AgConsumeRate_Reduce, mDatalist);
                AddPropertyData(E_GameProperty.ReplySD, mDatalist);
                AddPropertyData(E_GameProperty.KillEnemyReplySDRate, mDatalist);
                AddPropertyData(E_GameProperty.ReplyAllSdRate, mDatalist);
                // TODO SD吸收量
                //AddPropertyData(E_GameProperty.SdAbsorbRate, mDatalist);
                AddPropertyData(E_GameProperty.HitSdRate, mDatalist);
                AddPropertyData(E_GameProperty.AttackSdRate, mDatalist);
                AddPropertyData(E_GameProperty.SDAttackIgnoreRate, mDatalist);
                AddPropertyData(E_GameProperty.SDAttackIgnoreRate, mDatalist);
                AddPropertyData(E_GameProperty.ShacklesRate, mDatalist);
                AddPropertyData(E_GameProperty.ShacklesResistanceRate, mDatalist);
                AddPropertyData(E_GameProperty.ReallyDefense, mDatalist);
                // TODO 盾牌伤害吸收量
                //AddPropertyData(E_GameProperty.ShieldHurtAbsorb, mDatalist);
                AddPropertyData(E_GameProperty.DefenseShieldRate, mDatalist);
                AddPropertyData(E_GameProperty.AddGoldCoinRate_Increase, mDatalist);
                             
                b_Response.Info.AddRange(mDatalist);
                b_Response.ReincarnateCnt = mGamePlayer.Data.ReincarnateCnt;
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(99);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("666666!");
                b_Reply(b_Response);
                return false;
            }

            b_Reply(b_Response);
            return true;
        }
    }
}