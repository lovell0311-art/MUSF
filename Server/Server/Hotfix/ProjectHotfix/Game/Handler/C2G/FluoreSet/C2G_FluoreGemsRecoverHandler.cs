using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_FluoreGemsRecoverHandler : AMActorRpcHandler<C2G_FluoreGemsRecover, G2C_FluoreGemsRecover>
    {
        const string ItemEnum = "FluoreSlot";
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_FluoreGemsRecover b_Request, G2C_FluoreGemsRecover b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_FluoreGemsRecover b_Request, G2C_FluoreGemsRecover b_Response, Action<IMessage> b_Reply)
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
            Item guardianGems = backpack.GetItemByUID(b_Request.GuardianGemsItemUID);
            Item recycledGems = backpack.GetItemByUID(b_Request.RecycledGemsItemUID);
            Item mayaGems = backpack.GetItemByUID(b_Request.MayaGemsItemUID);

            //检查材料
            if (equipItem == null || guardianGems == null || recycledGems == null || mayaGems == null || mayaGems.GetProp(EItemValue.Quantity) < 5)
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

            if (b_Request.MountID > slotCount)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1706);//装备该位置无插槽
                b_Reply(b_Response);
                return false;
            }

            //检查宝石
            if (guardianGems.ConfigID != (int)EItemStrengthen.GUARDIAN_GEMS || mayaGems.ConfigID != (int)EItemStrengthen.MAYA_GEMS || recycledGems.ConfigID != (int)EItemStrengthen.RECYCLED_GEMS)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1701);//请放入正确的宝石
                b_Reply(b_Response);
                return false;
            }

            //抽取宝石
            int fluoreAttr = 0 ;
            if (Enum.TryParse(ItemEnum + b_Request.MountID, out EItemValue slotEnum))
            {
                if (equipItem.GetProp(slotEnum) > 0)
                {
                    //抽取后生成荧光宝石属性
                    fluoreAttr = equipItem.GetProp(slotEnum);
                }
                else {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1707);//插槽里没有荧光宝石，
                    b_Reply(b_Response);
                    return false;
                }
            }
            else {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1708);//插槽不满足抽取条件，
                b_Reply(b_Response);
                return false;
            }

            if (fluoreAttr == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1707);//插槽里没有荧光宝石，
                b_Reply(b_Response);
                return false;
            }

            int fluoreAttrID = fluoreAttr / 100;
            int fluoreLevel = fluoreAttr % 100;
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mConfigDic = mReadConfigComponent.GetJson<FluoreSet_AttrConfigJson>().JsonDic;
            if (!mConfigDic.TryGetValue(fluoreAttrID,out var config))
            {
                Log.Error("FluoreSet_AttrConfig配置表找不到荧光属性ID：" + fluoreAttrID);
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1709);//配置表找不到荧光属性
                b_Reply(b_Response);
                return false;
            }
            int fluoreID = config.fluore;
            Item fluoreItem = ItemFactory.Create(fluoreID, mPlayer.GameAreaId);
            fluoreItem.SetProp(EItemValue.FluoreAttr, fluoreAttr);
            fluoreItem.SetProp(EItemValue.Level, fluoreLevel);

            //判断宝石是否能加入到背包
            if (!backpack.AddItem(fluoreItem,"荧光宝石提取"))
            {
                fluoreItem.Dispose();
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1011);//背包空间不足
                b_Reply(b_Response);
                return false;
            }

            equipItem.SetProp(slotEnum, 0, mPlayer);
            backpack.UseItem(guardianGems.ItemUID, "荧光宝石提取", 1);
            backpack.UseItem(recycledGems.ItemUID, "荧光宝石提取", 1);
            backpack.UseItem(mayaGems.ItemUID, "荧光宝石提取", 5);
            


            b_Reply(b_Response);
            return true;
        }
    }
}