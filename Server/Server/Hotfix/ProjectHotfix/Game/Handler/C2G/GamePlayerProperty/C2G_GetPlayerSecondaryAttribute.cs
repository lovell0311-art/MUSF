using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetPlayerSecondaryAttributeHandler : AMActorRpcHandler<C2G_GetPlayerSecondaryAttribute, G2C_GetPlayerSecondaryAttribute>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetPlayerSecondaryAttribute b_Request, G2C_GetPlayerSecondaryAttribute b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetPlayerSecondaryAttribute b_Request, G2C_GetPlayerSecondaryAttribute b_Response, Action<IMessage> b_Reply)
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

            //卓越一击
            AddPropertyData(E_GameProperty.ExcellentAttackRate, mDatalist);
            AddPropertyData(E_GameProperty.ExcellentAttackHurtValueIncrease, mDatalist);
            //幸运一击
            AddPropertyData(E_GameProperty.LucklyAttackRate, mDatalist);
            AddPropertyData(E_GameProperty.LucklyAttackHurtValueIncrease, mDatalist);
            //三倍伤害
            AddPropertyData(E_GameProperty.InjuryValueRate_3, mDatalist);
            //双倍伤害
            AddPropertyData(E_GameProperty.InjuryValueRate_2, mDatalist);  
            //无视防御
            AddPropertyData(E_GameProperty.AttackIgnoreDefenseRate, mDatalist);
            //无视吸收
            AddPropertyData(E_GameProperty.AttackIgnoreAbsorbRate, mDatalist);
            AddPropertyData(E_GameProperty.IgnoreAbsorbRate, mDatalist);
            //伤害提高
            AddPropertyData(E_GameProperty.InjuryValueRate_Increase, mDatalist);
            //伤害减少
            AddPropertyData(E_GameProperty.InjuryValueRate_Reduce, mDatalist);
            //伤害反射
            AddPropertyData(E_GameProperty.BackInjuryRate, mDatalist);
            //伤害吸收
            AddPropertyData(E_GameProperty.HurtValueAbsorbRate, mDatalist);

            AddPropertyData(E_GameProperty.DefenseRate, mDatalist);
            AddPropertyData(E_GameProperty.PVPDefenseRate, mDatalist);
            AddPropertyData(E_GameProperty.ReboundRate, mDatalist);
            AddPropertyData(E_GameProperty.InjuryValue_Reduce, mDatalist);
            AddPropertyData(E_GameProperty.ReplyHp, mDatalist);
            AddPropertyData(E_GameProperty.KillEnemyReplyHpRate, mDatalist);
            AddPropertyData(E_GameProperty.ReplyAllHpRate, mDatalist);
            AddPropertyData(E_GameProperty.ReplyMp, mDatalist);
            AddPropertyData(E_GameProperty.KillEnemyReplyMpRate, mDatalist);
            AddPropertyData(E_GameProperty.ReplyAllMpRate, mDatalist);
            AddPropertyData(E_GameProperty.MpConsumeRate_Reduce, mDatalist);
            AddPropertyData(E_GameProperty.ReplyAG, mDatalist);
            AddPropertyData(E_GameProperty.AgConsumeRate_Reduce, mDatalist);
            AddPropertyData(E_GameProperty.ReplySD, mDatalist);
            AddPropertyData(E_GameProperty.KillEnemyReplySDRate, mDatalist);
            AddPropertyData(E_GameProperty.ReplyAllSdRate, mDatalist);
            AddPropertyData(E_GameProperty.HitSdRate, mDatalist);
            AddPropertyData(E_GameProperty.AttackSdRate, mDatalist);
            AddPropertyData(E_GameProperty.SDAttackIgnoreRate, mDatalist);
            AddPropertyData(E_GameProperty.SDAttackIgnoreRate, mDatalist);
            AddPropertyData(E_GameProperty.ShacklesRate, mDatalist);
            AddPropertyData(E_GameProperty.ShacklesResistanceRate, mDatalist);
            AddPropertyData(E_GameProperty.ReallyDefense, mDatalist);
            AddPropertyData(E_GameProperty.DefenseShieldRate, mDatalist);
            AddPropertyData(E_GameProperty.AddGoldCoinRate_Increase, mDatalist);

            AddPropertyData(E_GameProperty.DamageAbsPct_Guard, mDatalist);
            AddPropertyData(E_GameProperty.DamageAbsPct_Wing, mDatalist);
            AddPropertyData(E_GameProperty.DamageAbsPct_Mounts, mDatalist);
            AddPropertyData(E_GameProperty.DamageAbsPct_Pets, mDatalist);
            AddPropertyData(E_GameProperty.DisregardHarmReductionPct, mDatalist); 
            b_Response.Info.AddRange(mDatalist);
            b_Reply(b_Response);
            return true;
        }
    }
}