
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
    /// 一般合成-恶魔广场通行证
    /// </summary>
    [SynthesisRule(typeof(EmoGuangChangSynthesis))]
    public class EmoGuangChangSynthesis : C_SynthesisRule<SynthesisComponent, List<Item>, Synthesis_InfoConfig, G2C_ItemsSynthesis,int>
    {
        public const int EMoTongXingZheng = 320099;   //恶魔通行证
        public const int EMoZhiYao = 320098;    //恶魔之钥
        public const int EMoZhiYan = 320097;    //恶魔之眼
        public override async Task Run(SynthesisComponent synthesis, List<Item> itemList, Synthesis_InfoConfig config, G2C_ItemsSynthesis b_Response, int mClientFinalR)
        {
            Log.Debug("进入果实合成逻辑");
            //检查物品
            if (!synthesis.CheckItem(config.NeedItemsDic,itemList))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                return;
            }
            Item emozhiyao = null;
            Item emozhiyan = null;
            for (int i = 0; i < itemList.Count; i++)
            {
                if (itemList[i].ConfigID == EMoZhiYao)
                {
                    emozhiyao = itemList[i];
                }
                if (itemList[i].ConfigID == EMoZhiYan)
                {
                    emozhiyan = itemList[i];
                }
            }
            if (emozhiyao == null || emozhiyan == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                return;
            }
            if (emozhiyao.GetProp(EItemValue.Level) != emozhiyan.GetProp(EItemValue.Level))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1716);
                return;
            }
            //=================判定逻辑结束，开始消耗逻辑(所有异常放到上面报，下面默认判断为合成逻辑成功)==================
            int itemLevel = emozhiyao.GetProp(EItemValue.Level);

            //消耗金币
            if (!synthesis.UseGold((int)config.NeedGold))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(712);
                return;
            }
            //消耗物品
            if (!synthesis.UseAllItem(config.NeedItemsDic, itemList, config.Info))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1719);
                return;
            }
            synthesis.BackAllItemToBackpack(config.Info);
            if (mClientFinalR != config.BaseSuccessRate)
            {
                Log.Warning($"方法: EmoGuangChangSynthesis 前后概率不一致C:{mClientFinalR}S:{config.BaseSuccessRate}SMax:{config.MaxSuccessRate}");
            }
            //生成合成物品
            Item resultItem = ItemFactory.Create(EMoTongXingZheng, synthesis.mPlayer.GameAreaId);
            //判断是否合成成功
            if (StrengthenItemSystem.StrengthenResult(config.BaseSuccessRate))
            {
                //生成合成物品
                //Item resultItem = ItemFactory.Create(EMoTongXingZheng, synthesis.mPlayer.GameAreaId);
                
                if (!synthesis.AddResultItem(resultItem))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1717);
                    b_Response.Message = "合成物品异常，无法进入缓存空间";
                    return;
                }
                resultItem.SetProp(EItemValue.Level, itemLevel);
                b_Response.AddedItem.Add(synthesis.Item2BackpackStatusData(resultItem));
                b_Response.Result = true;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, true).Coroutine();
            }
            else {
                b_Response.Result = false;
                synthesis.Parent?.GetCustomComponent<GamePlayer>()?.SendItem(3, resultItem, false).Coroutine();
            }

            return;
        }
    }
}