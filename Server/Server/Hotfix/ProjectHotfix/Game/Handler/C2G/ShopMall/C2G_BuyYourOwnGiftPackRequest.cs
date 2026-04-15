using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;
using System.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_BuyYourOwnGiftPackRequestHandler : AMActorRpcHandler<C2G_BuyYourOwnGiftPackRequest, G2C_BuyYourOwnGiftPackResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_BuyYourOwnGiftPackRequest b_Request, G2C_BuyYourOwnGiftPackResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_BuyYourOwnGiftPackRequest b_Request, G2C_BuyYourOwnGiftPackResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
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
            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (gameplayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDic = readConfig.GetJson<ValueGift_TypeConfigJson>().JsonDic;
            if (jsonDic.TryGetValue(b_Request.MaxType,out var Info ))
            {
                if (Info.Money > gameplayer.Player.Data.YuanbaoCoin)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                    b_Reply(b_Response);
                    return false;
                }
                MailInfo mailInfo = new MailInfo();
                mailInfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                mailInfo.MailName = "礼包购买通知";
                mailInfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                mailInfo.MailContent = "超值礼包";
                mailInfo.MailState = 0;
                mailInfo.ReceiveOrNot = 0;
                mailInfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
               
                var ItemjsonDic = readConfig.GetJson<ValueGift_ItemInfoConfigJson>().JsonDic;
                List<MailItem> ItemList = new List<MailItem>();
                foreach (var item in ItemjsonDic)
                {
                    if (item.Value.TypeId == b_Request.MaxType && item.Value.Id == b_Request.MinType)
                    {
                        MailItem iteminfo = new MailItem();
                        iteminfo.ItemConfigID = item.Value.ItemId;
                        iteminfo.ItemID = 0;
                        ItemCreateAttr CreateAttr = new ItemCreateAttr();
                        CreateAttr.Level = item.Value.Level;
                        CreateAttr.Quantity = item.Value.Quantity;
                        CreateAttr.HaveSkill = item.Value.HasSkill == 1;
                        CreateAttr.HaveLucky = item.Value.HasLucky == 1;
                        CreateAttr.SetId = item.Value.SetId;
                        CreateAttr.IsBind = item.Value.IsBind;
                        CreateAttr.ValidTime = item.Value.ItemExpirationTime;
                        if(!string.IsNullOrEmpty(item.Value.CustomAttrMathod))
                            CreateAttr.CustomAttrMethod.AddRange(item.Value.CustomAttrMathod.Split(','));

                        iteminfo.CreateAttr = CreateAttr;

                        ItemList.Add(iteminfo);
                    }
                }
                //最多6个道具
                foreach (var item in ItemList)
                {
                    mailInfo.MailEnclosure.Add(item);
                    if (mailInfo.MailEnclosure.Count >= 6) break;
                }
                await MailSystem.SendMail(mPlayer.GameUserId, mailInfo, mPlayer.GameAreaId, "购买获取");

                PlayerShop.SetPlayerYuanBao(-Info.Money, "购买超值礼包消耗");
                b_Reply(b_Response);
                return true;
            }
            b_Response.Error = 2402;
            b_Reply(b_Response);
            return false;
        }
    }
}