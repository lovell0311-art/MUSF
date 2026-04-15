using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using TencentCloud.Hcm.V20181106.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PetsPackBackRequestHandler : AMActorRpcHandler<C2G_PetsPackBackRequest, G2C_PetsPackBackResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PetsPackBackRequest b_Request, G2C_PetsPackBackResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PetsPackBackRequest b_Request, G2C_PetsPackBackResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }

            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                b_Reply(b_Response);
                return false;
            }

            var Bk = mPlayer.GetCustomComponent<BackpackComponent>();
            if (Bk == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                b_Reply(b_Response);
                return false;
            }
            if (Bk.GetItemByUID(b_Request.PetsID) != null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1724);
                b_Reply(b_Response);
                return false;
            }
            var EC = mPlayer.GetCustomComponent<EquipmentComponent>();
            var PetItem = EC.GetEquipItemByPosition(EquipPosition.Pet);
            if (PetItem != null && PetItem.ItemUID == b_Request.PetsID)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1724);
                b_Reply(b_Response);
                return false;
            }
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
            
            if (dBProxy != null)//查询是否重复宠物 
            {
                var INfo = await dBProxy.Query<DBItemData>(P => P.Id == b_Request.PetsID);
                if (INfo != null && INfo.Count > 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1724);
                    b_Reply(b_Response);
                    return false;
                }
            }
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();
            var dataCache = mDataCacheManageComponent.Get<DBPetsData>();

            var Shop = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (Shop == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.Data.YuanbaoCoin < 1)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                b_Reply(b_Response);
                return false;
            }

            if (mGamePlayer.Pets != null && mGamePlayer.Pets.dBPetsData.PetsId == b_Request.PetsID)
            {
                var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
                if (mJsonDic.TryGetValue(mGamePlayer.Pets.dBPetsData.ConfigID, out var mPetJson) == false)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                    b_Reply(b_Response);
                    return false;
                }
                ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                itemCreateAttr.Quantity = 1;
                if (mGamePlayer.Pets.dBPetsData.PetsTrialTime != 0)
                {
                    itemCreateAttr.IsBind = 2;
                    string StrTime = (mGamePlayer.Pets.dBPetsData.PetsTrialTime - Help_TimeHelper.GetNowSecond()).ToString();
                    itemCreateAttr.ValidTime = int.Parse(StrTime);
                }

                Item item = ItemFactory.Create(mPetJson.BeakId, mPlayer.GameAreaId, itemCreateAttr);
                if (item == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                    b_Reply(b_Response);
                    return false;
                }
                int posX = 0, posY = 0;
                if (!Bk.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y, ref posX, ref posY))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                    b_Reply(b_Response);
                    return false;
                }

                Pets pets = mGamePlayer.Pets;
                if (mGamePlayer.PetsList.ContainsKey(pets.dBPetsData.PetsId))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1606);
                    b_Reply(b_Response);
                    return false;
                }
                pets.IsAttacking = false;
                pets.dBPetsData.PetsUseState = 0;
                pets.Pathlist = null;
                pets.dBPetsData.GameUserId = 0;



                pets.CurrentMap?.Leave(pets);
                mGamePlayer.Pets = null;
                var equipComponent = mPlayer.GetCustomComponent<EquipmentComponent>();
                if (equipComponent != null)
                {
                    equipComponent.ApplyEquipProp();
                }

                pets.dBPetsData.PetsId = item.ItemUID;

                if (dataCache != null)
                {
                    dataCache.DataRemove(pets.dBPetsData.Id);
                }
                pets.dBPetsData.IsDisabled = 1;
                await dBProxy.Save(pets.dBPetsData);

                item.data.ExcellentEntry = new HashSet<int>(pets.dBPetsData.ExcellentId);
                item.SetProp(EItemValue.Level, pets.dBPetsData.EnhanceLv);
                item.SetProp(EItemValue.LuckyEquip,1);
                Bk.AddItem(item, "宠物回收进背包");

                pets.Dispose();

                Shop.SetPlayerYuanBao(-1, "宠物回收扣除1魔晶");
                b_Reply(b_Response);
                return true;
            }
            else
            {
                if (mGamePlayer.PetsList.TryGetValue(b_Request.PetsID, out Pets petsInfo))
                {
                    var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
                    if (mJsonDic.TryGetValue(petsInfo.dBPetsData.ConfigID, out var mPetJson) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                        b_Reply(b_Response);
                        return true;
                    }
                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                    itemCreateAttr.Quantity = 1;

                    if (petsInfo.dBPetsData.PetsTrialTime != 0)
                        itemCreateAttr.IsBind = 2;

                    Item item = ItemFactory.Create(mPetJson.BeakId, mPlayer.GameAreaId, itemCreateAttr);
                    if (item == null)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                        b_Reply(b_Response);
                        return false;
                    }
                    int posX = 0, posY = 0;
                    if (!Bk.mItemBox.CheckStatus(item.ConfigData.X, item.ConfigData.Y, ref posX, ref posY))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(700);
                        b_Reply(b_Response);
                        return false;
                    }

                    petsInfo.dBPetsData.PetsId = item.ItemUID;
                    petsInfo.dBPetsData.GameUserId = 0;

                    if (dataCache != null)
                    {
                        dataCache.DataRemove(petsInfo.dBPetsData.Id);
                    }

                    petsInfo.dBPetsData.IsDisabled = 1;
                    await dBProxy.Save(petsInfo.dBPetsData);
                    item.data.ExcellentEntry = new HashSet<int>(petsInfo.dBPetsData.ExcellentId);
                    item.SetProp(EItemValue.Level, petsInfo.dBPetsData.EnhanceLv);
                    item.SetProp(EItemValue.LuckyEquip, 1);
                    Bk.AddItem(item, "宠物回收进背包");

                    mGamePlayer.PetsList.Remove(b_Request.PetsID);
                    petsInfo.Dispose();
                    //Shop.SetPlayerYuanBao(-1, "宠物回收扣除1魔晶");
                    b_Reply(b_Response);
                    return true;
                }
            }
            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1604);
            b_Reply(b_Response);
            return false;
        }
    }
}