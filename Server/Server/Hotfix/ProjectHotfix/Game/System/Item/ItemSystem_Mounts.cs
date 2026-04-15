using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    public static partial class ItemSystem
    {
        /// <summary>
        /// 坐骑添加经验
        /// </summary>
        /// <param name="self"></param>
        /// <param name="exp"></param>
        /// <param name="owner"></param>
        /// <returns>true.等级变动，需要更新玩家属性</returns>
        public static bool AddMountsExp(this Item self, int exp, GamePlayer owner)
        {
            switch (self.ConfigID)
            {
                case 260012:
                    {// 黑王马
                        int MountsMaxLevel = 50;
                        return AddDarkHorseExp(self, exp, owner, MountsMaxLevel);
                    }
                case 260015:
                case 260019:
                    {// 天鹰
                        int MountsMaxLevel = 50;
                        return AddDarkHorseExp(self, exp, owner, MountsMaxLevel);
                    }
                default:
                    break;
            }
            return false;
        }

        /// <summary>
        /// 给黑王马添加经验
        /// </summary>
        /// <param name="self"></param>
        /// <param name="exp"></param>
        /// <param name="owner"></param>
        /// <returns>true.等级变动，需要更新玩家属性</returns>
        public static bool AddDarkHorseExp(this Item self, int exp, GamePlayer owner, int b_MountsMaxLevel)
        {
            if (exp <= 0) return false;

            int mountsLevel = self.GetProp(EItemValue.MountsLevel);
            if (mountsLevel < 1)
            {
                mountsLevel = 1;
                self.SetProp(EItemValue.MountsLevel, mountsLevel);
            }
            else if (mountsLevel >= b_MountsMaxLevel)
            {
                // 已到达最高等级
                return false;
            }

            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<Mounts_DarkHorseExpConfigJson>().JsonDic;
            Mounts_DarkHorseExpConfig upExpConfig;

            if (!jsonDic.TryGetValue(mountsLevel, out upExpConfig))
            {
                Log.Error($"黑王马 没找到对应等级的 'Mounts_DarkHorseExpConfig'. mountsLevel={mountsLevel}");
                return false;
            }

            int mountsExp = self.GetProp(EItemValue.MountsExp);
            mountsExp += exp;
            bool levelChanged = false;

            while (mountsExp >= upExpConfig.Exp)
            {
                mountsExp -= upExpConfig.Exp;
                mountsLevel += 1;
                if (mountsLevel >= b_MountsMaxLevel)
                {
                    mountsLevel = b_MountsMaxLevel;
                    mountsExp = 0;
                }
                self.SetProp(EItemValue.MountsLevel, mountsLevel, owner.Player);
                levelChanged = true;
                if (!jsonDic.TryGetValue(mountsLevel, out upExpConfig))
                {
                    Log.Error($"黑王马 没找到对应等级的 'Mounts_DarkHorseExpConfig'. mountsLevel={mountsLevel}");
                    break;
                }
            }
            self.SetProp(EItemValue.MountsExp, mountsExp, owner.Player);
            self.OnlySaveDB();
            return levelChanged;
        }

        /// <summary>
        /// 扣除黑王马百分比的经验
        /// </summary>
        /// <param name="self"></param>
        /// <param name="pct"></param>
        /// <param name="owner"></param>
        /// <returns>true.等级变动，需要更新玩家属性</returns>
        public static bool DecDarkHorseExpPct(this Item self, int pct, GamePlayer owner)
        {
            int mountsLevel = self.GetProp(EItemValue.MountsLevel);
            if (mountsLevel < 1)
            {
                self.SetProp(EItemValue.MountsLevel, 1);
                self.SetProp(EItemValue.MountsExp, 0);
                self.SendAllPropertyData(owner.Player);
                return true;
            }

            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<Mounts_DarkHorseExpConfigJson>().JsonDic;
            Mounts_DarkHorseExpConfig expConfig;

            if (!jsonDic.TryGetValue(mountsLevel, out expConfig))
            {
                Log.Error($"黑王马 没找到对应等级的 'Mounts_DarkHorseExpConfig'. mountsLevel={mountsLevel}");
                return false;
            }

            int decExp = expConfig.Exp * pct / 100;
            int mountsExp = self.GetProp(EItemValue.MountsExp);
            bool levelChanged = false;

            while (mountsExp < decExp)
            {
                decExp -= mountsExp;
                mountsLevel -= 1;
                if (mountsLevel < 1)
                {
                    mountsLevel = 1;
                    mountsExp = 0;
                }
                self.SetProp(EItemValue.MountsLevel, mountsLevel, owner.Player);
                levelChanged = true;
                if (!jsonDic.TryGetValue(mountsLevel, out expConfig))
                {
                    Log.Error($"黑王马 没找到对应等级的 'Mounts_DarkHorseExpConfig'. mountsLevel={mountsLevel}");
                    break;
                }
            }

            self.SetProp(EItemValue.MountsExp, mountsExp, owner.Player);
            self.OnlySaveDB();
            return levelChanged;
        }

    }
}
