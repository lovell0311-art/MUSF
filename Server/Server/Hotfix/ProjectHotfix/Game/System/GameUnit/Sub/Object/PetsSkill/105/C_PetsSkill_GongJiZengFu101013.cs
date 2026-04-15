
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using CustomFrameWork;
using ETModel;
using UnityEngine;
using static ETModel.CombatSource;

namespace ETHotfix
{
    /// <summary>
    /// 月神祝福
    /// </summary>
    [HeroSkillAttribute(BindId = (int)E_Pets_SKILL_Id.GongJiZengFu)]
    public partial class C_PetsSkill_GongJiZengFu101013 : C_HeroSkillSource
    {
        public override void AfterAwake()
        { // 只调用一次  
            IsDataHasError = false;
            DataUpdate();
        }
        public override void DataUpdate()
        {  //数据变化 更新变更数据 
            if (IsDataHasError) return;
            IsDataHasError = true;

            if (!(Config is Pets_SkillConfig mConfig))
            {
                return;
            }

            MP = mConfig.ConsumeDic[1];
            if (mConfig.ConsumeDic.TryGetValue(2, out var mAG))
            {
                AG = mAG;
            }

            CoolTime = mConfig.CoolTime;




            NextAttackTime = 0;
            IsDataHasError = false;
        }
    }

    /// <summary>
    /// 月神祝福
    /// </summary>
    public partial class C_PetsSkill_GongJiZengFu101013
    {
        public bool TryUseByUseStandard(CombatSource b_Attacker, IActorResponse b_Response)
        {
            if (!(Config is Pets_SkillConfig mConfig))
            {
                //b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(425);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("技能配置数据异常!");
                return false;
            }

            var mGamePlayer = b_Attacker as Pets;
            if (mGamePlayer != null)
            {
                var mKeys = mConfig.UseStandardDic.Keys.ToArray();
                for (int i = 0, len = mKeys.Length; i < len; i++)
                {
                    int key = mKeys[i];
                    int value = mConfig.UseStandardDic[key];

                    switch (key)
                    {
                        case 1:
                            {  // 等级
                                if (mGamePlayer.dBPetsData.PetsLevel < value)
                                {
                                    //b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(408);
                                    return false;
                                }
                            }
                            break;
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                        /*case 6:
                            {
                                var mPropertyValue = mGamePlayer.GetNumerial((E_GameProperty)(key - 1));
                                if (mPropertyValue < value)
                                {
                                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(409);
                                    return false;
                                }
                            }
                            break;*/
                        default:
                            break;
                    }
                }
            }
            return true;
        }
        public override bool TryUse(CombatSource b_Attacker, CombatSource b_BeAttacker, C_FindTheWay2D b_Cell, BattleComponent b_BattleComponent, IActorResponse b_Response)
        {
            if (!(Config is Pets_SkillConfig mConfig))
            {
                return false;
            }

            if (mConfig.skillType == 2) return true;

            return TryUseByUseStandard(b_Attacker, b_Response);
        }
        public override bool UseSkill(CombatSource b_Attacker, CombatSource b_BeAttacker, BattleComponent b_BattleComponent)
        {
            if (!(Config is Pets_SkillConfig mConfig))
            {
                return false;
            }

            int ValueA = (int)(b_Attacker.GamePropertyDic[E_GameProperty.MaxMagicAtteck] * mConfig.OtherDataDic[2] * 0.01f);
            int ValueB = (int)(b_Attacker.GamePropertyDic[E_GameProperty.MinMagicAtteck] * mConfig.OtherDataDic[2] * 0.01f);
            b_Attacker.GamePropertyDic[E_GameProperty.MinMagicAtteck] += ValueA;
            b_Attacker.GamePropertyDic[E_GameProperty.MaxMagicAtteck] += ValueB;

            b_Attacker.ChangeNumerialType(E_GameProperty.MinMagicAtteck);
            b_Attacker.ChangeNumerialType(E_GameProperty.MaxMagicAtteck);
            return true;
        }
    }
}
