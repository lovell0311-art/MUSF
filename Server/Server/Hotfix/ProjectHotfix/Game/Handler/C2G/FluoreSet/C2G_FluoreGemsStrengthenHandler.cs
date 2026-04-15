using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_FluoreGemsStrengthenHandler : AMActorRpcHandler<C2G_FluoreGemsStrengthen, G2C_FluoreGemsStrengthen>
    {

        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_FluoreGemsStrengthen b_Request, G2C_FluoreGemsStrengthen b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_FluoreGemsStrengthen b_Request, G2C_FluoreGemsStrengthen b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                b_Reply(b_Response);
                return false;
            }
            BackpackComponent backpack = mPlayer.GetCustomComponent<BackpackComponent>();
            Item mainFluoreGems = backpack.GetItemByUID(b_Request.MainFluoreGemsItemUID);
            Item fluoreGems = backpack.GetItemByUID(b_Request.FluoreGemsItemUID);
            Item lightGemsRune = backpack.GetItemByUID(b_Request.LightGemsRuneItemUID);

            //检查材料
            if (mainFluoreGems == null || fluoreGems == null || lightGemsRune == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1702);//材料不足
                b_Reply(b_Response);
                return false;
            }

            //检查金钱
            if (mPlayer.GetCustomComponent<GamePlayer>().Data.GoldCoin < 1000000)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1704);//金币不足
                b_Reply(b_Response);
                return false;
            }

            List<int> fluoreGemList = new List<int>() {
                (int)EItemFluoreGem.FluoreGem_Fire,
                (int)EItemFluoreGem.FluoreGem_Ice,
                (int)EItemFluoreGem.FluoreGem_Light,
                (int)EItemFluoreGem.FluoreGem_Soil,
                (int)EItemFluoreGem.FluoreGem_Water,
                (int)EItemFluoreGem.FluoreGem_Wind
            };

            if (!fluoreGemList.Contains(mainFluoreGems.ConfigID) || !fluoreGemList.Contains(fluoreGems.ConfigID) 
                || lightGemsRune.ConfigID != (int)EItemStrengthen.LIGHT_STONE_STRENGTHEN_RUNE)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1701);//请放入正确的宝石
                b_Reply(b_Response);
                return false;
            }

            //检查材料性质
            if (mainFluoreGems.GetProp(EItemValue.Level) != fluoreGems.GetProp(EItemValue.Level))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1711);//荧光宝石不是同级材料
                b_Reply(b_Response);
                return false;
            }
            int fluoreAttr = mainFluoreGems.GetProp(EItemValue.FluoreAttr);
            if (fluoreAttr == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1712);//荧光宝石属性异常
                b_Reply(b_Response);
                return false;
            }
            //强化属性
            int fluoreAttrID = fluoreAttr / 100;
            int fluoreLevel = fluoreAttr % 100;
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mConfigDic = mReadConfigComponent.GetJson<FluoreSet_StrengthenConfigJson>().JsonDic;
            if (!mConfigDic.TryGetValue(fluoreLevel+1,out var config))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1713);//已满级无法强化
                b_Reply(b_Response);
                return false;
            }

            //判断强化是否成功
            if (StrengthenItemSystem.StrengthenResult(config.SuccessRate))
            {
                fluoreLevel++;
                mainFluoreGems.SetProp(EItemValue.Level, fluoreLevel, mPlayer);
                mainFluoreGems.SetProp(EItemValue.FluoreAttr, fluoreAttrID * 100 + fluoreLevel, mPlayer);
            }
            else {
                b_Response.Message = "强化失败";
            }

            //成功与否都消耗材料
            backpack.UseItem(fluoreGems.ItemUID,"荧光宝石强化", 1);
            backpack.UseItem(lightGemsRune.ItemUID, "荧光宝石强化", 1);

            b_Reply(b_Response);
            return true;
        }
    }
}