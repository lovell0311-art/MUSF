using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_ClearingGiftKitHandler : AMActorRpcHandler<C2G_ClearingGiftKit, G2C_ClearingGiftKit>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_ClearingGiftKit b_Request, G2C_ClearingGiftKit b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }


        protected override async Task<bool> Run(C2G_ClearingGiftKit b_Request, G2C_ClearingGiftKit b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player player = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (player == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(102);
                b_Reply(b_Response);
                return false;
            }
            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var ItemList = readConfig.GetJson<SelectionGiftBundle_ItemInfoConfigJson>().JsonDic;
            var ItemMoney = readConfig.GetJson<SelectionGiftBundle_TypeConfigJson>().JsonDic;
            if (ItemList == null || ItemList.Count <= 0 || ItemMoney == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3400);
                b_Reply(b_Response);
                return false;
            }
            if (ItemMoney.TryGetValue(b_Request.MaxType,out var Money ) == false)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3400);
                b_Reply(b_Response);
                return false;
            }

            if (Money.Money > player.Data.YuanbaoCoin)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2205);
                b_Reply(b_Response);
                return false;
            }
            ////通过GameUserId取余计算出一个礼包Id
            ////判定当前账号下那个角色购买过那个类型的礼包
            //int ReceivedId = (int)(player.GameUserId % 1000000000) + b_Request.MaxType;
            //if (player.Data.ReceivedCRGiftId.Contains(ReceivedId))
            //{
            //    // 重复领取充值礼包
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3402);
            //    b_Reply(b_Response);
            //    return false;
            //}
            //player.Data.ReceivedCRGiftId.Add(ReceivedId);

            var gameplayer = player.GetCustomComponent<GamePlayer>();
            gameplayer.UpdateCoin(E_GameProperty.YuanbaoCoin, -Money.Money, "购买开荒礼包");
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(player.GameAreaId);
            mWriteDataComponent.Save(gameplayer.Player.Data, dBProxy).Coroutine();

            MailInfo mailinfo = new MailInfo();
            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
            mailinfo.MailName = "开荒礼包";
            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
            mailinfo.MailContent = "恭喜玩家获得开荒礼包";
            mailinfo.MailState = 0;
            mailinfo.ReceiveOrNot = 0;
            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
            foreach (var info in ItemList)
            {
                if (info.Value.TypeId == b_Request.MaxType)
                {
                    if (b_Request.MinType == info.Value.RoleType)
                    {
                        ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                        itemCreateAttr.Quantity = info.Value.Quantity;
                        itemCreateAttr.IsBind = info.Value.IsBind;
                        itemCreateAttr.Level = info.Value.Level;
                        itemCreateAttr.OptLevel = info.Value.OptLevel;
                        itemCreateAttr.HaveSkill = info.Value.HasSkill != 0;
                        itemCreateAttr.SetId = info.Value.SetId;
                        itemCreateAttr.OptionExcellent = new List<int>(info.Value.OptionExcellent);
                        if (!string.IsNullOrEmpty(info.Value.CustomAttrMathod))
                            itemCreateAttr.CustomAttrMethod.AddRange(info.Value.CustomAttrMathod.Split(','));

                        MailItem mailItem = new MailItem();
                        mailItem.ItemConfigID = info.Value.ItemId;
                        mailItem.ItemID = 0;
                        mailItem.CreateAttr = itemCreateAttr;
                        mailinfo.MailEnclosure.Add(mailItem);
                    }
                }
            }
            MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
           
            b_Reply(b_Response);
            return true;
        }
    }
}