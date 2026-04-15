using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;


namespace ETHotfix
{
    public static class GameTaskConfigSystem
    {
        /// <summary>
        /// 允许这个职业
        /// </summary>
        /// <param name="occupation">职业</param>
        /// <param name="occupationLevel">转职等级</param>
        /// <returns></returns>
        public static bool AllowedOccupation(this GameTaskConfig self,E_GameOccupation occupation, int occupationLevel = 0)
        {
            switch (occupation)
            {
                case E_GameOccupation.Spell: return (self.Spell != -1 && self.Spell <= occupationLevel);
                case E_GameOccupation.Swordsman: return (self.Swordsman != -1 && self.Swordsman <= occupationLevel);
                case E_GameOccupation.Archer: return (self.Archer != -1 && self.Archer <= occupationLevel);
                case E_GameOccupation.Spellsword: return (self.Spellsword != -1 && self.Spellsword <= occupationLevel);
                case E_GameOccupation.Holyteacher: return (self.Holyteacher != -1 && self.Holyteacher <= occupationLevel);
                case E_GameOccupation.SummonWarlock: return (self.SummonWarlock != -1 && self.SummonWarlock <= occupationLevel);
                case E_GameOccupation.Combat: return (self.Combat != -1 && self.Combat <= occupationLevel);
                case E_GameOccupation.GrowLancer: return (self.GrowLancer != -1 && self.GrowLancer <= occupationLevel);
                default: break;
            }
            return false;
        }

        public static bool CanReceive(this GameTaskConfig self,GamePlayer gamePlayer)
        {
            if (!self.AllowedOccupation((E_GameOccupation)gamePlayer.Data.PlayerTypeId, gamePlayer.Data.OccupationLevel))
            {
                return false;
            }
            if (gamePlayer.Data.Level < self.ReqLevelMin)
            {
                return false;
            }
            if (self.ReqLevelMax > 0 /* 最高等级大于 0 时才生效 */
                && gamePlayer.Data.Level > self.ReqLevelMax)
            {
                return false;
            }
            return true;
        }

    }
}
