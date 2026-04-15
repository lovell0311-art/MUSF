
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
    /// 装备强化1
    /// </summary>
    [SynthesisRule(typeof(EquipmentStrengthenSynthesis1))]
    public class EquipmentStrengthenSynthesis1 : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            await EquipmentStrengthenSynthesis1_9.Run(synthesis, itemList, config, b_Response, 0,mClientFinalR);
        }

        public static class EquipmentStrengthenSynthesis1_9
        {
            public static async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int needLevel, int mClientFinalR)
            {
                Item NeedItem1 = null;  //合成主要材料1
                List<Item> addRateItemList = new List<Item>();  //附加成功率的物品
                int curSuccessRate = config.BaseSuccessRate;
                bool haveProtect = true ;

                //检查金币
                if (!synthesis.CheckGold((int)config.NeedGold))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                    return;
                }
                //检查物品
                if (!synthesis.CheckItem(config.NeedItemsDic, itemList))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                    b_Response.Message = "物品种类或数量不正确";
                    return;
                }
                bool haveOtherItem = false;
                int targetCount = 0;
                //检查是否有主要物品
                for (int i = itemList.Count - 1; i >= 0; i--)
                {
                    int optLevel = itemList[i].GetProp(EItemValue.OptLevel);
                    int level = itemList[i].GetProp(EItemValue.Level);
                    //排除配置表中已判断的物品
                    if (config.NeedItemsDic.ContainsKey(itemList[i].ConfigID) || itemList[i].ConfigID == (int)EItemStrengthen.PROTECT_RULE || itemList[i].ConfigID == (int)EItemStrengthen.LUKCY_RULE || itemList[i].ConfigID == (int)EItemStrengthen.LUKCY_RULE_10)
                    {
                        continue;
                    }
                    if (!itemList[i].CanStrengthen())
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                        b_Response.Message = "只能强化武器、防具和翅膀";
                        return;
                    }
                    if (level != needLevel)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                        b_Response.Message = "只能强化武器、防具和翅膀";
                        return;
                    }
                    targetCount++;
                    NeedItem1 = itemList[i];
                    itemList.RemoveAt(i);
                    if (targetCount > 1)
                    {
                        haveOtherItem = true;
                    }
                }
                if (haveOtherItem)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                    b_Response.Message = "物品种类不正确";
                    return;
                }
                if (NeedItem1 == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                    b_Response.Message = "装备要求等级或追加不足";
                    return;
                }

                //幸运装备加成功率
                if (NeedItem1.IsLuckyEquip())
                {
                    curSuccessRate += 25;
                }

                //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
                //消耗金币
                if (!synthesis.UseGold((int)config.NeedGold))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                    return;
                }
                //消耗物品(配置表所需物品)
                if (!synthesis.UseAllItem(config.NeedItemsDic, itemList, config.Info))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1719);
                    return;
                }
                for (int i = itemList.Count - 1; i >= 0; i--)
                {
                    if (itemList[i] == null)
                    {
                        itemList.RemoveAt(i);   //消耗物品以后清理所有空对象
                    }
                }
                for (int i = 0; i < addRateItemList.Count; i++)
                {
                    synthesis.UseItem(addRateItemList[i].ItemUID, config.Info, addRateItemList[i].GetProp(EItemValue.Quantity));
                }
                //synthesis.UseItem(NeedItem1.ItemUID, config.Info, NeedItem1.GetProp(EItemValue.Quantity));
                if (mClientFinalR != curSuccessRate && config.MaxSuccessRate > curSuccessRate)
                {
                    Log.Warning($"方法: EquipmentStrengthenSynthesis1 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
                }
                //限制最大成功率
                if (curSuccessRate > config.MaxSuccessRate)
                {
                    curSuccessRate = config.MaxSuccessRate;
                }
                //消耗完毕,清空缓存空间
                //synthesis.BackAllItemToBackpack(config.Info);
                synthesis.mItemDict.Clear();
                //判断是否合成成功
                if (StrengthenItemSystem.StrengthenResult(curSuccessRate))
                {
                    ETModel.EventType.EquipmentRelatedSettings.Instance.player = synthesis.Parent;
                    ETModel.EventType.EquipmentRelatedSettings.Instance.item = NeedItem1;
                    ETModel.EventType.EquipmentRelatedSettings.Instance.TitleCount = 0;
                    ETModel.EventType.EquipmentRelatedSettings.Instance.ItemCount = 0;
                    Root.EventSystem.OnRun("EquipmentRelatedSettings", ETModel.EventType.EquipmentRelatedSettings.Instance);
                    //强化成功
                    NeedItem1.SetProp(EItemValue.Level, needLevel + 1);
                    if (!synthesis.AddResultItem(NeedItem1))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                        b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                        return;
                    }
                    b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(NeedItem1));
                    b_Response.Result = true;
                    synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(2, NeedItem1, true).Coroutine();
                }
                else
                {
                    synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(2, NeedItem1, false).Coroutine();
                    if (haveProtect)
                    {
                        //保护装备，但是等级清零
                        if (needLevel <= 9)
                        {
                            needLevel--;
                            if (needLevel <= 0) needLevel = 0;
                            NeedItem1.SetProp(EItemValue.Level, needLevel);
                        }
                        //else if (needLevel >= 7)
                        //    NeedItem1.SetProp(EItemValue.Level, 0);

                        if (!synthesis.AddResultItem(NeedItem1))
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                            b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                            return;
                        }
                        b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(NeedItem1));
                        b_Response.Result = false;
                        b_Response.Message = "合成失败，装备保护启动，保留装备但等级清零";

                    }
                    b_Response.Result = false;
                }

                return;
            }
        }

    }
}