using System.Collections.Generic;
using ETModel;
using ILRuntime.Runtime;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 二代翅膀合成
    /// </summary>
    [MergerSystem(302)]
    public class MergerWing_2 : MergerMethod
    {
        /// <summary>
        /// 二代翅膀合成
        /// </summary>
        Dictionary<long, string> Wing_1_Dic = new Dictionary<long, string>()
        {
          { 220057,"灾难之翼"},
          { 220058,"恶魔之翼"},
          { 220059,"天使之翼"},
          { 220060,"精灵之翼"},
        };
        /// <summary>
        /// 翅膀合成保护符咒
        /// </summary>
        readonly Dictionary<long, string> HeChengFuChouDic = new Dictionary<long, string>
        {
            { 320042,"飞龙之翼"},
            { 320043,"魔魂之翼"},
            { 320044,"圣灵之翼"},
            { 320045,"绝望之翼"},
            { 320046,"暗黑之翼"},
            { 320371,"武者披风"},
            { 320372,"极限披风"},

        };
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 5_000_0;
            SuccessRate = 0;
            MaxSuccessRate = 90;
            FailedDelete = true;
            mergerMethodId = 10005;
            mergerMethod = "SecondWingSynthesis";
            (bool IsHaveWing_1, bool IsHaveLuoKeZhiYu,bool IsHaveMayaStone,bool IsZhuoYue) MustItem;
            Log.DebugGreen($"二代翅膀合成：{CheckItems.Count}");
            //标题
            AddTextTitle("二代翅膀合成");
            ///必须材料
            AddMustItemInfoText("一代翅膀\t\tx1", MustItem.IsHaveWing_1 = IsHaveWing_1()); 
            AddMustItemInfoText("洛克之羽\t\tx1", MustItem.IsHaveLuoKeZhiYu = IsHaveItem(itemConfigId: 320297));
            AddMustItemInfoText("玛雅之石\t\tx1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64()));
            AddMustItemInfoText("卓越品质武器/防具装备 (+4以上/追4以上) x1", MustItem.IsZhuoYue = IsHaveExcellenceItem());
            ///辅助材料
            //AddSubItemInfoText("中级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            ///辅助材料
            AddSubItemInfoText("祝福宝石(+5%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), (long)100000, 5, false));
            AddSubItemInfoText("灵魂宝石(+3%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), (long)100000, 3, false));
            AddSubItemInfoText("中级魔晶石(+5%) 可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.MIDDLE_LEVEL_MOJING_STONE.ToInt64(), (long)100000, 5, false));
            //AddSubItemInfoText("玛雅之石(+2%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), (long)100000, 2, false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
            // AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            IsHaveHeChengFuChou();
             IsCanMerger = MustItem == (true, true,true,true);
            return CheckItemCount();

            ///是否有一代翅膀
            bool IsHaveWing_1()
            {
                bool isHave = false;
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    var item = CheckItems[i];
                    if (Wing_1_Dic.ContainsKey(item.ConfigId))
                    {
                        SuccessRate = item.GetProperValue(E_ItemValue.Level) * 2 + item.OptLev * 3;
                        CheckItems.RemoveAt(i);
                            isHave = true;
                            break;
                    }
                }
                IsCanMerger = isHave;
                return isHave;
            }
            //是否有卓越品质武器/防具装备 (+4以上/追4以上)
            bool IsHaveExcellenceItem() 
            {
                bool isHave = false;
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    var item = CheckItems[i];
                    if (item.ItemType <= (int)E_ItemType.Boots && item.IsHaveExecllentEntry&&item.GetProperValue(E_ItemValue.Level)>=4&& item.OptLev >= 1)
                    {
                        SuccessRate += CheckItems[i].GetProperValue(E_ItemValue.Level) * 2 + CheckItems[i].OptLev * 3 + CheckItems[i].ExecllentEntryDic.Count * 7;//卓越属性;
                        CheckItems.RemoveAt(i);
                        isHave = true;
                        break;
                    }
                }
                return isHave;
            }  
            ///是否有翅膀合成符咒
            void IsHaveHeChengFuChou()
            {
                for (int i = 0, length = HeChengFuChouDic.Count; i < length; i++)
                {
                    AddSubItemInfoText($"{HeChengFuChouDic.ElementAt(i).Value}合成符咒 可选 x1", IsHaveItem(itemConfigId: HeChengFuChouDic.ElementAt(i).Key, addSuccessrateValue: 0, IsMust: false));
                    if (IsHaveItem(itemConfigId: HeChengFuChouDic.ElementAt(i).Key, addSuccessrateValue: 0, IsMust: false))
                    {
                        AddTextTitle($"二代翅膀({HeChengFuChouDic.ElementAt(i).Value})合成");
                        break;
                    }
                }
            }
        }
    }
}
