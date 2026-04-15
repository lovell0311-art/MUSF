using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    /// <summary>
    /// 物品数值计算
    /// </summary>
    public static class ItemSystem_Numerial
    {
        #region 耐久

        /// <summary>
        /// 普通武器耐久下降
        /// </summary>
        /// <param name="self"></param>
        /// <param name="defence"></param>
        /// <param name="owner"></param>
        /// <returns>返回 true 时，需要调用 EquipmentComponent.ApplyEquipProp() 来重新计算装备属性</returns>
        public static bool NormalWeaponDurabilityDown(this Item self, int defence, GamePlayer owner)
        {
            if (self.GetProp(EItemValue.Durability) <= 0)
            {
                return false;
            }

            int damageMin = self.GetProp(EItemValue.DamageMin);
            int plusDamage = 0; // 追加伤害
            self.AppendAttr(ref plusDamage, EItemOpt.ATTACK);

            if (damageMin == 0)
            {
                Log.Warning($"Damage is 0,self.ConfigID = {self.ConfigID}");
                return false;
            }
            int div = damageMin + damageMin / 2 + plusDamage; //4

            if (div == 0)
            {
                return false;
            }

            int decreaseDur = (defence * 2) / div; //5

            self.data.DurabilitySmall += decreaseDur;
            self.OnlySaveDB();

            int iBaseDurSmall = 564;
            int mpsDownDur1 = owner.GetNumerial(E_GameProperty.MpsDownDur1);
            if(mpsDownDur1 > 0)
            {
                // 大师降低耐久
                iBaseDurSmall += (int)(iBaseDurSmall * mpsDownDur1 / 100f);
            }

            if(self.data.DurabilitySmall > iBaseDurSmall)
            {
                self.data.DurabilitySmall = 0;
                self.SetProp(EItemValue.Durability, self.GetProp(EItemValue.Durability) - 1);
                self.UpdateProp();
                self.OnlySaveDB();
                self.SendAllPropertyData(owner.Player);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 弓类武器耐久下降
        /// </summary>
        /// <param name="self"></param>
        /// <param name="defence"></param>
        /// <param name="owner"></param>
        /// <returns>返回 true 时，需要调用 EquipmentComponent.ApplyEquipProp() 来重新计算装备属性</returns>
        public static bool BowWeaponDurabilityDown(this Item self, int defence, GamePlayer owner)
        {
            if (self.GetProp(EItemValue.Durability) <= 0)
            {
                return false;
            }

            int damageMin = self.GetProp(EItemValue.DamageMin);
            int plusDamage = 0; // 追加伤害
            self.AppendAttr(ref plusDamage, EItemOpt.ATTACK);

            if (damageMin == 0)
            {
                Log.Warning($"Damage is 0,self.ConfigID = {self.ConfigID}");
                return false;
            }
            int div = damageMin + damageMin / 2 + plusDamage; //4

            if (div == 0)
            {
                return false;
            }

            int decreaseDur = (defence * 2) / div; //5

            self.data.DurabilitySmall += decreaseDur;
            self.OnlySaveDB();

            int iBaseDurSmall = 780;
            int mpsDownDur1 = owner.GetNumerial(E_GameProperty.MpsDownDur1);
            if (mpsDownDur1 > 0)
            {
                // 大师降低耐久
                iBaseDurSmall += (int)(iBaseDurSmall * mpsDownDur1 / 100f);
            }

            if (self.data.DurabilitySmall > iBaseDurSmall)
            {
                self.data.DurabilitySmall = 0;
                self.SetProp(EItemValue.Durability, self.GetProp(EItemValue.Durability) - 1);
                self.UpdateProp();
                self.OnlySaveDB();
                self.SendAllPropertyData(owner.Player);
                return true;
            }
            return false;
        }



        /// <summary>
        /// 法杖类武器耐久下降
        /// </summary>
        /// <param name="self"></param>
        /// <param name="defence"></param>
        /// <param name="owner"></param>
        /// <returns>返回 true 时，需要调用 EquipmentComponent.ApplyEquipProp() 来重新计算装备属性</returns>
        public static bool StaffWeaponDurabilityDown(this Item self,int defence, GamePlayer owner)
        {
            if (self.GetProp(EItemValue.Durability) <= 0)
            {
                return false;
            }


            int magic = self.GetProp(EItemValue.MagicDamage) / 2 + self.GetProp(EItemValue.Level) * 2;
            int plusmagic = 0;
            self.AppendAttr(ref plusmagic, EItemOpt.MAGIC_ATTACK);

            int div = magic + magic / 3 + plusmagic;
            if (div == 0)
            {
                Log.Warning($"Damage is 0,self.ConfigID = {self.ConfigID}");
                return false;
            }

            int DecreaseDur = defence / div;

            self.data.DurabilitySmall += DecreaseDur;
            self.OnlySaveDB();

            int iBaseDurSmall = 1050;
            int mpsDownDur1 = owner.GetNumerial(E_GameProperty.MpsDownDur1);
            if (mpsDownDur1 > 0)
            {
                // 大师降低耐久
                iBaseDurSmall += (int)(iBaseDurSmall * mpsDownDur1 / 100f);
            }

            if (self.data.DurabilitySmall > iBaseDurSmall)
            {
                self.data.DurabilitySmall = 0;
                self.SetProp(EItemValue.Durability, self.GetProp(EItemValue.Durability) - 1);
                self.UpdateProp();
                self.OnlySaveDB();
                self.SendAllPropertyData(owner.Player);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 防具类耐久下降
        /// </summary>
        /// <param name="self"></param>
        /// <param name="damagemin"></param>
        /// <param name="owner"></param>
        /// <returns>返回 true 时，需要调用 EquipmentComponent.ApplyEquipProp() 来重新计算装备属性</returns>
        public static bool ArmorDurabilityDown(this Item self, int damageMin, GamePlayer owner)
        {
            if (self.GetProp(EItemValue.Durability) <= 0)
            {
                return false;
            }


            int def = self.GetProp(EItemValue.Defense);
            int plusdef = 0;
            int decreaseDur;

            if (def == 0)
            {
                Log.Warning($"This is def 0,self.ConfigID = {self.ConfigID}");
                return false;
            }

            int reqValue = 0;
            if(self.Type == EItemType.Shields)
            {
                self.AppendAttr(ref plusdef, EItemOpt.DEFENCE_2);
                decreaseDur = damageMin / (def * 5 + plusdef);
            }
            else if(self.ConfigData.UseRole.TryGetValue((int)E_GameOccupation.Spell,out reqValue) && reqValue >= 0)
            {
                // 法师
                self.AppendAttr(ref plusdef, EItemOpt.DEFENCE);
                decreaseDur = damageMin / (def * 3 + plusdef);
            }
            else if (self.ConfigData.UseRole.TryGetValue((int)E_GameOccupation.Swordsman, out reqValue) && reqValue >= 0)
            {
                // 剑士
                self.AppendAttr(ref plusdef, EItemOpt.DEFENCE);
                decreaseDur = damageMin / (def * 3 + plusdef);
            }
            else if (self.ConfigData.UseRole.TryGetValue((int)E_GameOccupation.Archer, out reqValue) && reqValue >= 0)
            {
                // 弓箭手
                self.AppendAttr(ref plusdef, EItemOpt.DEFENCE);
                decreaseDur = damageMin / (def * 2 + plusdef);
            }
            else if (self.ConfigData.UseRole.TryGetValue((int)E_GameOccupation.Spellsword, out reqValue) && reqValue >= 0)
            {
                // 魔剑士
                self.AppendAttr(ref plusdef, EItemOpt.DEFENCE);
                decreaseDur = damageMin / (def * 7 + plusdef);
            }
            else if (self.ConfigData.UseRole.TryGetValue((int)E_GameOccupation.Holyteacher, out reqValue) && reqValue >= 0)
            {
                // 圣导师
                self.AppendAttr(ref plusdef, EItemOpt.DEFENCE);
                decreaseDur = damageMin / (def * 6 + plusdef);
            }
            else if (self.ConfigData.UseRole.TryGetValue((int)E_GameOccupation.SummonWarlock, out reqValue) && reqValue >= 0)
            {
                // 召唤术师
                self.AppendAttr(ref plusdef, EItemOpt.DEFENCE);
                decreaseDur = damageMin / (def * 3 + plusdef);
            }
            else if (self.ConfigData.UseRole.TryGetValue((int)E_GameOccupation.Combat, out reqValue) && reqValue >= 0)
            {
                // 格斗家
                self.AppendAttr(ref plusdef, EItemOpt.DEFENCE);
                decreaseDur = damageMin / (def * 6 + plusdef);
            }
            else
            {
                // 其他职业装备 用平均值
                self.AppendAttr(ref plusdef, EItemOpt.DEFENCE);
                decreaseDur = damageMin / (def * 5 + plusdef);
            }

            self.data.DurabilitySmall += decreaseDur;
            self.OnlySaveDB();

            int iBaseDurSmall = 69;
            int mpsDownDur1 = owner.GetNumerial(E_GameProperty.MpsDownDur1);
            if (mpsDownDur1 > 0)
            {
                // 大师降低耐久
                iBaseDurSmall += (int)(iBaseDurSmall * mpsDownDur1 / 100f);
            }
            if (self.data.DurabilitySmall > iBaseDurSmall)
            {
                self.data.DurabilitySmall = 0;
                self.SetProp(EItemValue.Durability, self.GetProp(EItemValue.Durability) - 1);
                self.UpdateProp();
                self.OnlySaveDB();
                self.SendAllPropertyData(owner.Player);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 耐久降低，用于翅膀、戒指、项链
        /// </summary>
        /// <param name="self"></param>
        /// <param name="dur"></param>
        /// <param name="owner"></param>
        /// <returns>返回 true 时，需要调用 EquipmentComponent.ApplyEquipProp() 来重新计算装备属性</returns>
        public static bool DurabilityDown(this Item self,int dur, GamePlayer owner)
        {
            if (self.GetProp(EItemValue.Durability) <= 0)
            {
                return false;
            }
            self.data.DurabilitySmall += dur;
            self.OnlySaveDB();

            int iBaseDurSmall = 564;
            int mpsDownDur2 = owner.GetNumerial(E_GameProperty.MpsDownDur2);
            if (mpsDownDur2 > 0)
            {
                // 大师降低耐久
                iBaseDurSmall += (int)(iBaseDurSmall * mpsDownDur2 / 100f);
            }
            if (self.data.DurabilitySmall > iBaseDurSmall)
            {
                self.data.DurabilitySmall = 0;
                self.SetProp(EItemValue.Durability, self.GetProp(EItemValue.Durability) - 1);
                self.UpdateProp();
                self.OnlySaveDB();
                self.SendAllPropertyData(owner.Player);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 守护收到伤害，耐久下降
        /// </summary>
        /// <param name="self"></param>
        /// <param name="damage"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static bool SpriteDamage(this Item self,int damage, GamePlayer owner)
        {
            return false;
            if (self.GetProp(EItemValue.Durability) <= 0) return false;
            int fN = 10;
            int fdamage = damage * 100;
            int iBaseDurSmall = 100;

            switch (self.ConfigID)
            {
                case 300003:    /* 强化恶魔 */
                case 300004:    /* 强化天使 */
                case 300006:    /* 熊猫 */
                case 300007:    /* 幼龙骨架 */
                case 300008:    /* 独角兽 */
                    fN += 10;
                    break;
            }

            switch (self.ConfigID)
            {
                case 300001:    /* 守护天使 */
                    {
                        int mpsDownDur3 = owner.GetNumerial(E_GameProperty.MpsDownDur3);
                        if (mpsDownDur3 > 0)
                        {
                            // 大师降低耐久
                            iBaseDurSmall += (int)(iBaseDurSmall * mpsDownDur3 / 100f);
                        }
                        fdamage = (int)(fdamage * 3 / 10f);
                        fdamage /= fN;
                    }
                    break;
                case 300002:    /* 小恶魔 */
                    {
                        int mpsDownDur3 = owner.GetNumerial(E_GameProperty.MpsDownDur3);
                        if (mpsDownDur3 > 0)
                        {
                            // 大师降低耐久
                            iBaseDurSmall += (int)(iBaseDurSmall * mpsDownDur3 / 100f);
                        }
                        fdamage = (int)(fdamage * 2 / 10f);
                        fdamage /= fN;
                    }
                    break;
                case 300005:    /* 鲁道夫 */
                    {
                        fdamage = (int)(fdamage * 3 / 10f);
                        fdamage /= fN;
                    }
                    break;
                case 300003:    /* 强化恶魔 */
                    {
                        fdamage = (int)(fdamage * 3 / 10f);
                        fdamage /= fN;
                    }
                    break;
                case 300004:    /* 强化天使 */
                case 300006:    /* 熊猫 */
                case 300007:    /* 幼龙骨架 */
                case 300008:    /* 独角兽 */
                    {
                        fdamage = (int)(fdamage * 2 / 10f);
                        fdamage /= fN;
                    }
                    break;
                default:
                    {
                        fdamage = 0;
                    }
                    break;
            }
            if(fdamage > 0)
            {
                self.data.DurabilitySmall += fdamage;
                self.OnlySaveDB();

                if (self.data.DurabilitySmall >= iBaseDurSmall)
                {
                    int dur = self.GetProp(EItemValue.Durability) - self.data.DurabilitySmall / iBaseDurSmall;
                    if (dur < 0) dur = 0;
                    self.SetProp(EItemValue.Durability, dur, owner.Player);
                    self.data.DurabilitySmall = self.data.DurabilitySmall % iBaseDurSmall;
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// 坐骑收到伤害，耐久下降
        /// </summary>
        /// <param name="self"></param>
        /// <param name="damage"></param>
        /// <param name="owner"></param>
        /// <returns></returns>
        public static bool MountsDamage(this Item self,int damage,GamePlayer owner)
        {
            return false;
            if (self.GetProp(EItemValue.Durability) <= 0) return false;
            int iBaseDurSmall;
            int dur;
            int mpsPetDurDownSpeed;
            int mpsDownDur3;
            switch (self.ConfigID)
            {
                case 260012:
                    // 黑王马
                    dur = (int)(damage * 2 / 10f);
                    dur += 1;
                    self.data.DurabilitySmall += dur;
                    self.OnlySaveDB();

                    iBaseDurSmall = 1500;
                    mpsPetDurDownSpeed = owner.GetNumerial(E_GameProperty.MpsPetDurDownSpeed);
                    if (mpsPetDurDownSpeed > 0)
                    {
                        // 大师降低耐久
                        iBaseDurSmall += (int)(iBaseDurSmall * mpsPetDurDownSpeed / 100f);
                    }
                    if (self.data.DurabilitySmall > iBaseDurSmall)
                    {
                        self.data.DurabilitySmall = 0;
                        self.SetProp(EItemValue.Durability, self.GetProp(EItemValue.Durability) - 1);
                        if(self.GetProp(EItemValue.Durability) == 0)
                        {
                            // TODO 坐骑死亡，扣除 10% 经验
                            int oldLevel = self.GetProp(EItemValue.MountsLevel);
                            int oldExp = self.GetProp(EItemValue.MountsExp);
                            self.DecDarkHorseExpPct(10, owner);
                            Log.PLog("Item", $"a:{owner.Player.UserId} r:{owner.Player.GameUserId} 坐骑死亡惩罚 ({self.ToLogString()}) " +
                                $"level:{oldLevel}=>{self.GetProp(EItemValue.MountsLevel)} exp:{oldExp}=>{self.GetProp(EItemValue.MountsLevel)}");
                        }
                        self.UpdateProp();
                        self.OnlySaveDB();
                        self.SendAllPropertyData(owner.Player);
                        return true;
                    }
                    break;
                case 260013:
                    // 彩云兽
                    dur = (int)(damage * 100 / 10f);
                    self.data.DurabilitySmall += dur;
                    self.OnlySaveDB();

                    iBaseDurSmall = 100;
                    mpsDownDur3 = owner.GetNumerial(E_GameProperty.MpsDownDur3);
                    if (mpsDownDur3 > 0)
                    {
                        // 大师降低耐久
                        iBaseDurSmall += (int)(iBaseDurSmall * mpsDownDur3 / 100f);
                    }
                    if (self.data.DurabilitySmall > iBaseDurSmall)
                    {
                        self.data.DurabilitySmall = 0;
                        self.SetProp(EItemValue.Durability, self.GetProp(EItemValue.Durability) - 1);
                        self.UpdateProp();
                        self.OnlySaveDB();
                        self.SendAllPropertyData(owner.Player);
                        return true;
                    }
                    break;
                case 260008:
                case 260009:
                case 260010:
                case 260011:
                    // 炎狼兽
                    dur = (int)(damage * 2 / 10f);
                    dur /= 10;
                    dur += 1;
                    self.data.DurabilitySmall += dur;
                    self.OnlySaveDB();

                    iBaseDurSmall = 150;
                    mpsDownDur3 = owner.GetNumerial(E_GameProperty.MpsDownDur3);
                    if (mpsDownDur3 > 0)
                    {
                        // 大师降低耐久
                        iBaseDurSmall += (int)(iBaseDurSmall * mpsDownDur3 / 100f);
                    }
                    if (self.data.DurabilitySmall > iBaseDurSmall)
                    {
                        self.data.DurabilitySmall = 0;
                        self.SetProp(EItemValue.Durability, self.GetProp(EItemValue.Durability) - 1);
                        self.UpdateProp();
                        self.OnlySaveDB();
                        self.SendAllPropertyData(owner.Player);
                        return true;
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        #endregion


        private static void AppendAttr(this Item self,ref int value,EItemOpt appendId)
        {
            switch ((EItemOpt)self.GetProp(EItemValue.OptValue))
            {
                case EItemOpt.ATTACK:
                    value += self.GetProp(EItemValue.OptLevel) * 4;
                    break;
                case EItemOpt.MAGIC_ATTACK:
                    value += self.GetProp(EItemValue.OptLevel) * 4;
                    break;
                case EItemOpt.DEFENCE_RATE:
                    value += self.GetProp(EItemValue.OptLevel) * 4;
                    break;
                case EItemOpt.DEFENCE:
                    value += self.GetProp(EItemValue.OptLevel) * 4;
                    break;
                case EItemOpt.HP_AUTO_RECOVERY:
                    value += self.GetProp(EItemValue.OptLevel);
                    break;
                case EItemOpt.ALL_ATTACK:
                    value += self.GetProp(EItemValue.OptLevel) * 4;
                    break;
                case EItemOpt.DEFENCE_2:
                    value += self.GetProp(EItemValue.OptLevel) * 5;
                    break;
            }
        }



    }
}
