
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 装备强化+11~+12
    /// </summary>
    [SynthesisRule(typeof(InEquipmentNumber1))]
    public class InEquipmentNumber1 : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response,int mClientFinalR)
        {
            await InEquipmentNumber.Run(synthesis, itemList, config, b_Response,1,mClientFinalR);
        }
    }

    public static class InEquipmentNumber
    {
        public static async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int LimitCnt,int mClientFinalR)
        {
            Item NeedItem1 = null;  //合成主要材料1
            List<Item> addRateItemList = new List<Item>();  //附加成功率的物品
            int curSuccessRate = config.BaseSuccessRate;
            bool haveProtect = false;

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
            bool haveOtherItem = true;
            DBPetsData dBPetsData = null;//宠物
            bool IsPets = false;
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManagerComponent.GetZoneDB(DBType.Core, synthesis.Parent.GameAreaId);
            //检查是否有主要物品
            for (int i = itemList.Count - 1; i >= 0; i--)
            {
                if (itemList[i].data.ConfigID / 10000 == 35)
                {

                    var PetList = await dBProxy2.Query<DBPetsData>(p => p.GameUserId == 0 && p.PetsId == itemList[i].ItemUID && p.IsDisabled != 1);
                    if (PetList != null && PetList.Count > 0)
                    {
                        dBPetsData = PetList[0] as DBPetsData;
                    }
                    IsPets = true;
                }
                int Cnt = itemList[i].data.ExcellentEntry.Count;
                //排除配置表中已判断的物品
                if (Cnt == LimitCnt)
                {
                    NeedItem1 = itemList[i];
                    haveOtherItem = false;
                    break;
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
            //增加成功率加成判定
            for (int i = 0; i < itemList.Count; i++)
            {
                switch (itemList[i].ConfigID)
                {
                    case (int)EItemStrengthen.PROTECT_RULE:
                        haveProtect = true;
                        break;
                    default:
                        break;
                }
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
                Log.Warning($"方法: InEquipmentNumber1 前后概率不一致C:{mClientFinalR}S:{curSuccessRate}SMax:{config.MaxSuccessRate}");
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
                //强化成功
                var excAttrEntryManager = Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
                if (IsPets)
                {
                    if (excAttrEntryManager.TryGetPetsAttrEntry(out var Info))
                    {
                        var newSelector = new RandomSelector<int>(Info);
                        for (int i = Info.Count; i > 0; --i)
                        {
                            if (newSelector.TryGetValueAndRemove(out var entryId))
                            {
                                if (NeedItem1.data.ExcellentEntry.Contains(entryId) == false)
                                {
                                    NeedItem1.data.ExcellentEntry.Add(entryId);
                                    List<int> SkillId = Help_JsonSerializeHelper.DeSerialize<List<int>>(dBPetsData.Excellent);
                                    SkillId.Add(entryId);
                                    synthesis.Parent.PLog($"宠物强化增加卓越词条{entryId}");
                                    dBPetsData.Excellent = Help_JsonSerializeHelper.Serialize(SkillId);
                                    await dBProxy2.Save(dBPetsData);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    
                    if (excAttrEntryManager.TryGetSelectorByItem(NeedItem1, out var selector))
                    {
                        var newSelector = new RandomSelector<int>(selector);
                        int ExcellentCnt = 0;
                        while (ExcellentCnt < 6)
                        {
                            if (newSelector.TryGetValueAndRemove(out var entryId))
                            {
                                if (!NeedItem1.data.ExcellentEntry.Contains(entryId))
                                {
                                    NeedItem1.data.ExcellentEntry.Add(entryId);
                                    break;
                                }
                            }
                            ExcellentCnt++;
                        }
                    }
                }
                if (!synthesis.AddResultItem(NeedItem1))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                    return;
                }
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(NeedItem1));
                b_Response.Result = true;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(4, NeedItem1, true).Coroutine();
            }
            else
            {
                if (haveProtect)
                {
                    //保护装备
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
                else
                {
                    Random r = new Random();
                    int num = r.Next(1, 11);
                    if (num != 1)
                    {
                        if (!synthesis.AddResultItem(NeedItem1))
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                            b_Response.Message = "合成物品异常，无法进入缓存空间";//一般不会报，若报此异常为服务端问题，需排查
                            return;
                        }
                        b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(NeedItem1));
                    }
                }
                b_Response.Result = false;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(4, NeedItem1, false).Coroutine();
            }

            return;
        }
    }
}