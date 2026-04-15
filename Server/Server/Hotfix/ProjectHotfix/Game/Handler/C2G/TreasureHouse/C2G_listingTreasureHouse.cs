using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Xml.Linq;
using TencentCloud.Ecm.V20190719.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_listingTreasureHouseHandler : AMActorRpcHandler<C2G_listingTreasureHouse, G2C_listingTreasureHouse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_listingTreasureHouse b_Request, G2C_listingTreasureHouse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_listingTreasureHouse b_Request, G2C_listingTreasureHouse b_Response, Action<IMessage> b_Reply)
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
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (b_Request.ItemPrice < 2)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3306);
                b_Reply(b_Response);
                return false;
            }
            var PlayerShop = mPlayer.GetCustomComponent<PlayerShopMallComponent>();
            if (PlayerShop == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            if (!PlayerShop.GetPlayerShopState(DeviationType.MaxMonthlyCard))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2202);
                b_Reply(b_Response);
                return false;
            }
            var backpackComponent = mPlayer.GetCustomComponent<BackpackComponent>();
            //从背包拿出物品
            var item = backpackComponent.GetItemByUID(b_Request.ItemUUID);
            if (item != null)
            {
                // TODO 物品状态限制 - 仓库
                if (item.GetProp(EItemValue.IsBind) == 2 || item.GetProp(EItemValue.IsBind) == 1)
                {
                    // 绑定角色的物品无法移到仓库
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3300);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsTask) != 0)
                {
                    // 任务物品无法移到仓库
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3301);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsUsing) != 0)
                {
                    // 使用中的物品无法移到仓库
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3302);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.IsLocking) != 0)
                {
                    // 锁定的物品无法移到仓库
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3303);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.GetProp(EItemValue.ValidTime) != 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3310);
                    b_Reply(b_Response);
                    return false;
                }
                if (item.ConfigData.Sell != 1)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3304);
                    b_Reply(b_Response);
                    return false;
                }
                int MaxType = 0;//1武器，2防具，3坐骑/旗帜，4翅膀/时装，5宠物/宠物装备，6材料/其他
                int MinType = 0;
                if ((item.IsWeapon() || item.IsArmor() || item.Type == EItemType.Necklace ||
                item.Type == EItemType.Rings || item.Type == EItemType.Arrow)
                 && item.data.ExcellentEntry.Count < 1 && !item.HaveSetOption())
                {
                    //if (!item.IsSetEquip() && !item.IsSocketEquip())
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3306);
                        b_Reply(b_Response);
                        return false;
                    }
                }
                else if (item.IsWeapon() || item.Type == EItemType.Arrow)
                {
                    MaxType = 1;
                    if (item.Type == EItemType.Flag)
                    {
                        if (item.data.ExcellentEntry.Count >= 1)
                            MaxType = 6;
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3306);
                            b_Reply(b_Response);
                            return false;
                        }
                    }
                    if(item.Type == EItemType.Necklace)
                        MaxType = 2;

                    MinType = (int)item.Type; 
                }
                else if (item.IsArmor() || item.Type == EItemType.Necklace || item.Type == EItemType.Rings)
                { MaxType = 2; MinType = (int)item.Type; }
                else if (item.Type == EItemType.Mounts)
                    MaxType = 3;
                else if (item.IsWing() || item.IsFirstWing() || item.IsSecondWing() || item.IsThirdWing() || item.IsFourthWing())
                    MaxType = 4;
                else if (item.Type == EItemType.Pets)
                    MaxType = 5;
                else
                    MaxType = 7;

                if (MaxType == 0)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3306);
                    b_Reply(b_Response);
                    return false;
                }

                backpackComponent.PutItOnTheShelf(item,"上架藏宝阁");
                ItemFactory.DleData(b_Request.ItemUUID, mPlayer);

                var TH = mPlayer.GetCustomComponent<TreasureHouseComponent>();
                if (TH.AddItem(item))
                {
                    int optLv = item.HaveSetOption() && item.HaveEnableSocket() ? 3 : 0;
                        optLv = item.HaveEnableSocket() ? 2 :( item.HaveSetOption() ? 1 : 0 );

                    G2M_AddTreasureHouseItemInfo g2M_AddTreasureHouseItemInfo = new G2M_AddTreasureHouseItemInfo();
                    string Class = Help_JsonSerializeHelper.Serialize(item.ConfigData.UseRole);
                    GMTreasureHouseItemInfo gMTreasureHouseItemInfo = new GMTreasureHouseItemInfo()
                    {
                        Uid = item.ItemUID,
                        UserID = mPlayer.GameUserId,
                        Name = item.GetClientName(),
                        Class = Class,
                        Excellent = item.data.ExcellentEntry.Count,
                        Enhance = item.GetProp(EItemValue.Level),
                        Readdition = optLv,
                        Price = b_Request.ItemPrice,
                        AreaId = item.data.GameAreaId,
                        ConfigID = item.data.ConfigID,
                        Cnt = item.GetProp(EItemValue.Quantity)
                    };
                    g2M_AddTreasureHouseItemInfo.AppendData = b_Request.AppendData;
                    g2M_AddTreasureHouseItemInfo.MaxType = MaxType;
                    g2M_AddTreasureHouseItemInfo.MinType = MinType;
                    g2M_AddTreasureHouseItemInfo.ItemInfo = gMTreasureHouseItemInfo;
                    IResponse Message = await TH.SendGM(g2M_AddTreasureHouseItemInfo);
                    if (Message == null)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                        b_Reply(b_Response);
                        return false;
                    }
                    else if (Message.Error != 0)
                    {
                        b_Response.Error = Message.Error;
                        b_Reply(b_Response);
                        return false;
                    }
                    else
                    {
                        M2G_AddTreasureHouseItemInfo info = Message as M2G_AddTreasureHouseItemInfo;
                        TH.AddItemLoadDB(info.DBid).Coroutine();

                        mPlayer.GetCustomComponent<GamePlayer>().SendItem(7,item).Coroutine();
                        b_Reply(b_Response);
                        return true;
                    }
                }
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(807);
                b_Reply(b_Response);
                return false;
            }
            b_Reply(b_Response);
            return true;
        }
    }
}