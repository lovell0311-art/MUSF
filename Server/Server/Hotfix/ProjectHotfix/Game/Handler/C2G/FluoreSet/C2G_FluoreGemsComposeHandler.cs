using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_FluoreGemsComposeHandler : AMActorRpcHandler<C2G_FluoreGemsCompose, G2C_FluoreGemsCompose>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_FluoreGemsCompose b_Request, G2C_FluoreGemsCompose b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_FluoreGemsCompose b_Request, G2C_FluoreGemsCompose b_Response, Action<IMessage> b_Reply)
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
            Item fluoreStone = backpack.GetItemByUID(b_Request.FluoreStoneItemUID);
            Item createGems = backpack.GetItemByUID(b_Request.CreateGemsItemUID);
            Item mayaGems = backpack.GetItemByUID(b_Request.MayaGemsItemUID);
            Item lightGems = backpack.GetItemByUID(b_Request.LightGemsItemUID);

            if (fluoreStone == null || createGems == null || lightGems == null || mayaGems == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1702);   //材料不足
                b_Reply(b_Response);
                return false;
            }

            if (createGems.ConfigID != (int)EItemStrengthen.CREATE_GEMS || mayaGems.ConfigID != (int)EItemStrengthen.MAYA_GEMS || lightGems.ConfigID != (int)EItemStrengthen.LIGHT_STONE)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1701);   //材料不对
                b_Reply(b_Response);
                return false;
            }

            //检查金钱
            if (mPlayer.GetCustomComponent<GamePlayer>().Data.GoldCoin < 1000000)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1704);   //金钱不足
                b_Reply(b_Response);
                return false;
            }

            Dictionary<EItemFluoreGem, EItemFluoreGem> stoneComposeDict = new Dictionary<EItemFluoreGem, EItemFluoreGem>()
            {
                [EItemFluoreGem.FluoreStone_Fire] = EItemFluoreGem.FluoreGem_Fire,
                [EItemFluoreGem.FluoreStone_Ice] = EItemFluoreGem.FluoreGem_Ice,
                [EItemFluoreGem.FluoreStone_Light] = EItemFluoreGem.FluoreGem_Light,
                [EItemFluoreGem.FluoreStone_Soil] = EItemFluoreGem.FluoreGem_Soil,
                [EItemFluoreGem.FluoreStone_Water] = EItemFluoreGem.FluoreGem_Water,
                [EItemFluoreGem.FluoreStone_Wind] = EItemFluoreGem.FluoreGem_Wind
            };

            EItemFluoreGem createrGemID = default;
            if (!stoneComposeDict.TryGetValue((EItemFluoreGem)fluoreStone.ConfigID, out createrGemID))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1703);   //请放入对应荧光石
                b_Reply(b_Response);
                return false;
            }

            //合成对应荧光宝石
            Item fluoreGem = ItemFactory.Create((int)createrGemID, mPlayer.GameAreaId);
            //给荧光宝石随机附加属性            
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mConfigDic = mReadConfigComponent.GetJson<FluoreSet_AttrConfigJson>().JsonDic;
            List<int> weightList = new List<int>();
            List<int> configIDList = new List<int>();
            foreach (var configItem in mConfigDic)
            {
                if (configItem.Value.fluore == fluoreGem.ConfigID)
                {
                    weightList.Add(configItem.Value.weight);
                    configIDList.Add(configItem.Value.Id);
                }
            }
            if (weightList.Count <= 0 )
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1700);   //未找到荧光属性配置
                b_Reply(b_Response);
                return false;
            }
            int randomID = StrengthenItemSystem.StrengthenResult(weightList);
            if (randomID == -1)
                Log.Warning($"C2G_FluoreGemsCompose 荧光宝石合成错误 randomID = -1,createrGemID = {createrGemID}");
            fluoreGem.SetProp(EItemValue.FluoreAttr, configIDList[randomID] * 100 + fluoreGem.GetProp(EItemValue.Level));

            backpack.AddItem(fluoreGem,"荧光宝石合成");
            //宝石消耗掉以后必然有1x1空间，所以不判断空间是否足够，直接成功
            backpack.UseItem(fluoreStone.ItemUID, "荧光宝石合成", 1);
            backpack.UseItem(createGems.ItemUID, "荧光宝石合成", 1);
            backpack.UseItem(mayaGems.ItemUID, "荧光宝石合成", 1);
            backpack.UseItem(lightGems.ItemUID, "荧光宝石合成", 1);

            b_Reply(b_Response);
            return true;
        }
    }
}