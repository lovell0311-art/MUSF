using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 使用天鹰
    /// </summary>
    [ItemUseRule(typeof(UseItemTianYing))]
    public class UseItemTianYing : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            return;
            var mGamePlayer = b_Player.GetCustomComponent<GamePlayer>();

            if (!b_Item.CanUse(mGamePlayer))
            {
                b_Response.Error = 2302;
                return;
            }

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mGamePlayer.UnitData.Index, b_Player.GameUserId);
            if (mMapComponent == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            if (mGamePlayer.HolyteacherSummoned != null && mGamePlayer.HolyteacherSummoned.IsDeath == false)
            {
                mGamePlayer.HolyteacherSummoned.IsDeath = true;

                G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                mAttackResultNotice.AttackTarget = mGamePlayer.HolyteacherSummoned.InstanceId;
                mAttackResultNotice.HpValue = 0;

                mMapComponent.SendNotice(mGamePlayer.HolyteacherSummoned, mAttackResultNotice);
            }
            else
            {
                var mTargetId = 546;
                var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Enemy_InfoConfigJson>().JsonDic;

                if (mJsonDic.TryGetValue(mTargetId, out var mEnemyConfig) == false)
                {
                    b_Response.Error = 99;
                    //b_Attacker.CombatRoundId = b_BattleComponent.WaitSync(mAttackTime, b_Attacker.InstanceId, mBeAttacker.InstanceId, (b_CombatRoundId, b_AttackerId, b_BeAttackerId) => { if (b_Attacker.CombatRoundId == b_CombatRoundId) b_Attacker.IsAttacking = false; }, () => { b_Attacker.CombatRoundId = 0; });
                    return;
                }

                var mSummoned = Root.CreateBuilder.GetInstance<HolyteacherSummoned>(false);
                mSummoned.Awake(null);
                mSummoned.GamePlayer = mGamePlayer;
                mSummoned.SetConfig(mEnemyConfig, b_Item);
                mSummoned.SetInstanceId(mSummoned.Id);
                mSummoned.AfterAwake();
                mSummoned.AwakeSkill();
                mSummoned.DataUpdateSkill();


                List<C_FindTheWay2D> mMapFieldDic = new List<C_FindTheWay2D>();

                int mRepelValue = 1;
                var mCurrentTemp = mMapComponent.GetFindTheWay2D(mGamePlayer.UnitData.X + mRepelValue, mGamePlayer.UnitData.Y + mRepelValue);
                if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false && mMapFieldDic.Contains(mCurrentTemp) == false) mMapFieldDic.Add(mCurrentTemp);

                mCurrentTemp = mMapComponent.GetFindTheWay2D(mGamePlayer.UnitData.X + mRepelValue, mGamePlayer.UnitData.Y - mRepelValue);
                if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false && mMapFieldDic.Contains(mCurrentTemp) == false) mMapFieldDic.Add(mCurrentTemp);

                mCurrentTemp = mMapComponent.GetFindTheWay2D(mGamePlayer.UnitData.X - mRepelValue, mGamePlayer.UnitData.Y + mRepelValue);
                if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false && mMapFieldDic.Contains(mCurrentTemp) == false) mMapFieldDic.Add(mCurrentTemp);

                mCurrentTemp = mMapComponent.GetFindTheWay2D(mGamePlayer.UnitData.X - mRepelValue, mGamePlayer.UnitData.Y - mRepelValue);
                if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false && mMapFieldDic.Contains(mCurrentTemp) == false) mMapFieldDic.Add(mCurrentTemp);

                if (mMapFieldDic.Count > 0)
                {
                    var mRandomIndex = Help_RandomHelper.Range(0, mMapFieldDic.Count);
                    var mRandomResult = mMapFieldDic[mRandomIndex];

                    mMapComponent.MoveSendNotice(null, mRandomResult, mSummoned);
                    mGamePlayer.HolyteacherSummoned = mSummoned;
                }
                else
                {
                    var mRandomResult = mMapComponent.GetFindTheWay2D(mGamePlayer);

                    mMapComponent.MoveSendNotice(null, mRandomResult, mSummoned);
                    mGamePlayer.HolyteacherSummoned = mSummoned;
                }
            }

            b_Response.Error = 0;
            return;
        }
    }
}