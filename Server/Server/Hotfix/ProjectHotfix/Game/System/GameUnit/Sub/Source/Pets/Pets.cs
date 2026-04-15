using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Mrs.V20200910.Models;

namespace ETHotfix
{
    public static partial class PetsSystem
    {
        /// <summary>
        /// 卓越曾加值
        /// </summary>
        /// <param name="b_Component"></param>
        public static void AddValueExcellent(this Pets b_Component)
        {
            if (b_Component != null)
            {
                List<int> IDList = b_Component.dBPetsData.ExcellentId;
                var excellent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_ExcConfigJson>().JsonDic;
                foreach (int i in IDList)
                {
                    if (excellent.ContainsKey(i) != false)
                    {
                        Excellent Type = (Excellent)excellent[i].PropId;
                        int Value_A = excellent[i].Value0;
                        switch (Type)
                        {
                            case Excellent.EHP:
                                b_Component.GamePropertyDic[E_GameProperty.ExcellentAttackRate] += Value_A * 100;
                                break;
                            /*case Excellent.API:
                                int Value_B = b_Component.dBPetsData.PetsLevel / Value_A;
                                b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] += Value_B;
                                b_Component.GamePropertyDic[E_GameProperty.MinAtteck] += Value_B;
                                break;*/
                            case Excellent.APIP:
                                b_Component.GamePropertyDic[E_GameProperty.AttackBonus] += Value_A;
                                break;
                            case Excellent.ASI:
                                b_Component.GamePropertyDic[E_GameProperty.AttackSpeed] += Value_A;
                                break;
                            case Excellent.HGWKM:
                                b_Component.GamePropertyDic[E_GameProperty.KillMonsterReplyHp_8] += 1;
                                break;
                            case Excellent.MVGWKM:
                                b_Component.GamePropertyDic[E_GameProperty.KillMonsterReplyMp_8] += 1;
                                break;
                            case Excellent.MHI:
                                b_Component.GamePropertyDic[E_GameProperty.HealthBonus] += Value_A;

                                break;
                            case Excellent.MMVI:
                                b_Component.GamePropertyDic[E_GameProperty.MagicBonus] += Value_A;
                                break;
                            case Excellent.DR:
                                b_Component.GamePropertyDic[E_GameProperty.InjuryValueRate_Reduce] += Value_A * 100;
                                break;
                            case Excellent.IR:
                                b_Component.GamePropertyDic[E_GameProperty.BackInjuryRate] += Value_A;
                                break;
                            case Excellent.DSR:
                                b_Component.GamePropertyDic[E_GameProperty.DefenseRate] += Value_A;
                                break;
                            case Excellent.GGWKMI:
                                b_Component.GamePropertyDic[E_GameProperty.AddGoldCoinRate_Increase] += Value_A;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 卓越动态值更新
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Type"></param>
        /// <param name="Value_A"></param>
        public static void UpdataExcellentValue(this Pets b_Component ,int OldLv=0)
        {

            if (b_Component != null)
            {
                List<int> IDList = b_Component.dBPetsData.ExcellentId;
                var excellent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_ExcConfigJson>().JsonDic;
                foreach (int i in IDList)
                {
                    if (excellent.ContainsKey(i) != false)
                    {
                        Excellent Type = (Excellent)excellent[i].PropId;
                        int Value_A = excellent[i].Value0;
                        switch (Type)
                        {
                            case Excellent.API:
                                if (OldLv != 0)
                                {
                                    int Value_C = OldLv / Value_A;
                                    b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] -= Value_C;
                                    b_Component.GamePropertyDic[E_GameProperty.MinAtteck] -= Value_C;
                                }
                                int Value_B = b_Component.dBPetsData.PetsLevel / Value_A;
                                b_Component.GamePropertyDic[E_GameProperty.MaxAtteck] += Value_B;
                                b_Component.GamePropertyDic[E_GameProperty.MinAtteck] += Value_B;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}