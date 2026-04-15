using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// 追加属性
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 是否有追加属性
        /// </summary>
        public bool IsHaveOpt => GetProperValue(E_ItemValue.OptValue) != 0;
        /// <summary>
        /// 追加等级
        /// </summary>
        public int OptLev => GetProperValue(E_ItemValue.OptLevel);
        /// <summary>
        /// 获取追加属性
        /// </summary>
        /// <param name="list"></param>
        public void GetAppendAtr(ref List<string> list)
        {
            if (GetProperValue(E_ItemValue.OptValue) is int value && value == 0) return;
            if (GetProperValue(E_ItemValue.OptLevel) is int lev && lev == 0) return;
            if (GetValue() == 0) return;
            list.Add($"<color={ColorTools.LuckyItemColor}>{string.Format(GetAppendStr(), GetValue())}</color>");
            list.Add("");
            //获取属性
            string GetAppendStr() => value switch
            {
                1 => "追加攻击力+{0:G}",
                2 => "追加魔法攻击力+{0:G}",
                3 => "追加防御率+{0:G}",
                4 => "追加防御力+{0:G}",
                5 => "生命自动回复+{0:G}%",
                6 => "攻击力/魔法攻击力/诅咒能力增加+{0:G}",
                7 => "追加防御力+{0:G}",
                _ => string.Empty,
            };
            //获取属性值
            float GetValue() => (value, GetProperValue(E_ItemValue.OptLevel)) switch
            {
                (1, 1) => 4,
                (1, 2) => 8,
                (1, 3) => 12,
                (1, 4) => 16,
                (2, 1) => 4,
                (2, 2) => 8,
                (2, 3) => 12,
                (2, 4) => 16,
                (3, 1) => 4,
                (3, 2) => 8,
                (3, 3) => 12,
                (3, 4) => 16,
                (4, 1) => 4,
                (4, 2) => 8,
                (4, 3) => 12,
                (4, 4) => 16,
                (5, 1) => 1,
                (5, 2) => 2,
                (5, 3) => 3,
                (5, 4) => 4,
                (6, 1) => 4,
                (6, 2) => 8,
                (6, 3) => 12,
                (6, 4) => 16,
                (7, 1) => 5,
                (7, 2) => 10,
                (7, 3) => 15,
                (7, 4) => 20,
                _=>0
            };



        }

    }
}