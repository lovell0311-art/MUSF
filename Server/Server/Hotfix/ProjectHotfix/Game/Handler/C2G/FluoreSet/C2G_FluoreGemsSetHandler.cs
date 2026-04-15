using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using ETModel.EventType;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_FluoreGemsSetHandler : AMActorRpcHandler<C2G_FluoreGemsSet, G2C_FluoreGemsSet>
    {
        const string ItemEnum = "FluoreSlot";
     
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_FluoreGemsSet b_Request, G2C_FluoreGemsSet b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_FluoreGemsSet b_Request, G2C_FluoreGemsSet b_Response, Action<IMessage> b_Reply)
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
            Item equipItem = backpack.GetItemByUID(b_Request.EquipItemUID);
            Item fluoreGems = backpack.GetItemByUID(b_Request.FluoreGemsItemUID);
            Item createGems = backpack.GetItemByUID(b_Request.CreateGemsItemUID);
            Item mayaGems = backpack.GetItemByUID(b_Request.MayaGemsItemUID);

            //检查材料
            if (equipItem == null || fluoreGems == null || createGems == null || mayaGems == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1702);
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

            //检查装备性质
            int slotCount = equipItem.GetProp(EItemValue.FluoreSlotCount);
            if (!equipItem.IsEquipment() || slotCount <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1705);//请放入正确的装备
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

            //检查宝石
            if (createGems.ConfigID != (int)EItemStrengthen.CREATE_GEMS || mayaGems.ConfigID != (int)EItemStrengthen.MAYA_GEMS || !fluoreGemList.Contains(fluoreGems.ConfigID) )
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1701);//请放入正确的宝石
                b_Reply(b_Response);
                return false;
            }

            int ConfigId = fluoreGems.GetProp(EItemValue.FluoreAttr) / 100;
            var Embedjson = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<FluoreSet_AttrConfigJson>().JsonDic;
            if (Embedjson.TryGetValue(ConfigId, out var Value))
            {
                if (Value.Judgment != 2)
                {
                    if (!equipItem.IsArmor() && Value.Judgment == 0)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1728);//请放入正确的宝石
                        b_Reply(b_Response);
                        return false;
                    }
                    else if (!equipItem.IsWeapon() && Value.Judgment == 1)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1727);//请放入正确的宝石
                        b_Reply(b_Response);
                        return false;
                    }
                }
            }
            //检查是否满插槽
            bool isSlot = false ;
            for (int i = 1; i <= slotCount; i++)
            {
                if (Enum.TryParse(ItemEnum + i, out EItemValue slotEnum))
                {
                    int Id = equipItem.GetProp(slotEnum) /100;
                    if (Id != 0)
                    {
                        if (Id == fluoreGems.GetProp(EItemValue.FluoreAttr)/100)
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1726);//请放入正确的宝石
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    if (Id == 0)
                    {
                        //镶嵌到装备上
                        equipItem.SetProp(slotEnum, fluoreGems.GetProp(EItemValue.FluoreAttr),mPlayer);
                        isSlot = true;
                        break;
                    }
                }
            }
            if (!isSlot)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1710);//镶嵌失败，装备上没有可用插槽
                b_Reply(b_Response);
                return false;
            }

            //宝石消耗掉以后必然有1x1空间，所以不判断空间是否足够，直接成功
            backpack.UseItem(createGems.ItemUID,"荧光宝石镶嵌", 1);
            backpack.UseItem(mayaGems.ItemUID, "荧光宝石镶嵌", 1);
            backpack.UseItem(fluoreGems.ItemUID, "荧光宝石镶嵌", 1);
            EquipmentRelatedSettings.Instance.player = mPlayer;
            EquipmentRelatedSettings.Instance.item = fluoreGems;
            EquipmentRelatedSettings.Instance.TitleCount = 0;
            EquipmentRelatedSettings.Instance.ItemCount = 0;
            Root.EventSystem.OnRun("EquipmentRelatedSettings", EquipmentRelatedSettings.Instance);
            b_Reply(b_Response);
            return true;
        }
    }
}