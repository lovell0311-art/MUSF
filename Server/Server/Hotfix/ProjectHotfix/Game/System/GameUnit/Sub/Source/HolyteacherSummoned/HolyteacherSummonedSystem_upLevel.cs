using System;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    public static partial class HolyteacherSummonedSystem
    {
        /// <summary>
        /// 获取经验
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_AddExprience"></param>
        public static void AddExprience(this HolyteacherSummoned b_Component, int b_AddExprience)
        {
            if (b_Component.GamePlayer.GetNumerial(E_GameProperty.Property_Command) < 185 + b_Component.GetNumerial(E_GameProperty.Level) * 15)
            {
                return;
            }

            b_Component.Item.AddMountsExp(b_AddExprience, b_Component.GamePlayer);
        }
    }
}