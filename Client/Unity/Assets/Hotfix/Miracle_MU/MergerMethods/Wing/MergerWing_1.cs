using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using MongoDB.Bson.Serialization.Serializers;
using ILRuntime.Runtime;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 一代翅膀合成
    /// </summary>
    [MergerSystem(301)]
    public class MergerWing_1 : MergerMethod
    {
        /// <summary>
        /// 玛雅武器
        /// </summary>
        readonly Dictionary<long, string> MayEquipDic = new Dictionary<long, string>
        {
            { 30005,"玛雅龙斧"},
            { 40006,"玛雅神弓"},
            { 80007,"玛雅雷杖"},
        };
        /// <summary>
        /// 翅膀合成保护符咒
        /// </summary>
        readonly Dictionary<long, string> HeChengFuChouDic = new Dictionary<long, string>
        {
            { 320037,"恶魔之翼"},//恶魔之翼合成符咒
            { 320038,"天使之翼"},
            { 320039,"精灵之翼"},
            { 320040,"灾难之翼"},
        };
        public override bool CanUserThisMergerMethod()
        {
            IsCanMerger = true;
            Money = 1_000_000;
            SuccessRate = 0;
            MaxSuccessRate = 100; 
            FailedDelete = true;
            mergerMethodId = 10004;
            mergerMethod = "GenerationWingSynthesis";
            ///是否拥有 玛雅武器、玛雅之石 这两件 必要物品
            (bool IsHaveMayEquip, bool IsHaveMayaStone) MustItem;
            Log.DebugGreen($"一代翅膀合成：{CheckItems.Count}");
            //标题
            AddTextTitle("一代翅膀合成");

            ///必须材料
            AddMustItemInfoText(isHave:MustItem.IsHaveMayEquip = IsHaveMayaEquip(out string item), str: item);
            AddMustItemInfoText("玛雅之石 x1", MustItem.IsHaveMayaStone = IsHaveItem(itemConfigId:GemItemConfigId.MAYA_GEMS.ToInt64()));

            ///辅助材料
            AddSubItemInfoText("祝福宝石(+5%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.BLESSING_GEMS.ToInt64(), (long)100000,5, false));
            AddSubItemInfoText("灵魂宝石(+3%)\t\txN", IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), (long)100000, 3, false));
            //AddSubItemInfoText("玛雅之石(+2%) 可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), (long)100000, 2, false));
            AddSubItemInfoText("低级魔晶石(+5%) 可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.LOW_LEVEL_MOJING_STONE.ToInt64(), (long)100000, addSuccessrateValue: 5, IsMust: false));
            AddSubItemInfoText("生命宝石(+6%)  可选 xN", IsHaveItem(itemConfigId: GemItemConfigId.ANIMA_GEMS.ToInt64(), (long)100000, 6, false));
          //  AddSubItemInfoText("保护符咒\t\tx1", IsHaveItem(itemConfigId: 320318));
            AddSubItemInfoText("幸运符咒(+10%) 可选 可选 x1 或者 幸运符咒(+1~10%) 可选 x1~10", IsHaveItem(itemConfigId: 320400, addSuccessrateValue: 10, IsMust: false) || IsHaveLuckFuZhou(addSuccessrateValue: 1, IsMust: false));
            AddSubItemInfoText("合成装备保护符咒(只保留武器)\t\tx1", IsHaveItem(itemConfigId: 320318));
            //AddSubItemInfoText("祝福之石(+5%) 可选 x1", IsHaveItem(itemConfigId:GemItemConfigId.BLESSING_GEMS.ToInt64(),addSuccessrateValue:5,IsMust:false));
            //AddSubItemInfoText("灵魂之石(+3%) 可选 x1", IsHaveItem(itemConfigId: GemItemConfigId.SOUL_GEMS.ToInt64(), addSuccessrateValue: 3, IsMust: false));
            //AddSubItemInfoText("玛雅之石(+2%) 可选 x1", IsHaveItem(itemConfigId: GemItemConfigId.MAYA_GEMS.ToInt64(), addSuccessrateValue: 2, IsMust: false));
            //AddSubItemInfoText("低级魔晶石(+5%) 可选 x1", IsHaveMOJING_STONE(itemConfigId: GemItemConfigId.LOW_LEVEL_MOJING_STONE.ToInt64(), addSuccessrateValue: 5, IsMust: false));
            IsHaveHeChengFuChou();

            IsCanMerger = MustItem==(true,true);
            return CheckItemCount();

            ///是否有玛雅武器（+4以上追加4以上）x1
            bool IsHaveMayaEquip(out string str) 
            {
                bool isHave = false;
                str = $"玛雅物品 +4以上 追4以上 x1";
                for (int i = CheckItems.Count - 1; i >= 0; i--)
                {
                    var item = CheckItems[i];
                    if (MayEquipDic.ContainsKey(item.ConfigId))
                    {
                        if (item.GetProperValue(E_ItemValue.Level) >= 4&&item.OptLev>=1)
                        {
                            SuccessRate += CheckItems[i].GetProperValue(E_ItemValue.Level) * 2 + CheckItems[i].OptLev * 3 + CheckItems[i].ExecllentEntryDic.Count * 7;//卓越属性;
                            str = $"{MayEquipDic[item.ConfigId]} +4以上 追4以上 x1";
                            CheckItems.RemoveAt(i);
                            isHave = true;
                            break;
                        }
                    }
                }
                IsCanMerger = isHave;
                
                return isHave;
            }

            ///是否有翅膀合成符咒
            void IsHaveHeChengFuChou() 
            {
                for (int i = 0,length= HeChengFuChouDic.Count; i < length; i++)
                {
                    AddSubItemInfoText($"{HeChengFuChouDic.ElementAt(i).Value}合成符咒 可选 x1", IsHaveItem(itemConfigId: HeChengFuChouDic.ElementAt(i).Key, addSuccessrateValue: 0, IsMust: false));
                    if (IsHaveItem(itemConfigId: HeChengFuChouDic.ElementAt(i).Key, addSuccessrateValue: 100, IsMust: false))
                    {
                        AddTextTitle($"一代翅膀({HeChengFuChouDic.ElementAt(i).Value})合成");
                        break;
                    }
                }
            }
        }
       
    }
}