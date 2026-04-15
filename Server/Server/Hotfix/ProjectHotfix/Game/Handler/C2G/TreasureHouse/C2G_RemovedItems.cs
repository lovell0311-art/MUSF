using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using System.Xml.Linq;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_RemovedItemsHandler : AMActorRpcHandler<C2G_RemovedItems, G2C_RemovedItems>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_RemovedItems b_Request, G2C_RemovedItems b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_RemovedItems b_Request, G2C_RemovedItems b_Response, Action<IMessage> b_Reply)
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
            var TH = mPlayer.GetCustomComponent<TreasureHouseComponent>();
            if (TH == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                b_Reply(b_Response);
                return false;
            }
            var ItemInfo = TH.GetItem(b_Request.Uid);
            if (ItemInfo == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3307);
                b_Reply(b_Response);
                return false;
            }
            if (!TH.Dle(b_Request.Uid))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3307);
                b_Reply(b_Response);
                return false;
            }
            TH.SortFill();
            G2M_DleTreasureHouseItemInfo g2M_DleTreasureHouseItemInfo = new G2M_DleTreasureHouseItemInfo
            {
                Uid = b_Request.Uid,
                UserID = mPlayer.GameUserId,
                Game = mPlayer.GameUserId,
                Crystal = mPlayer.Data.YuanbaoCoin,
                MaxTyp = ItemInfo.MaxType,
                MinTyp = ItemInfo.MinType,
            };
            IResponse Message = await TH.SendGM(g2M_DleTreasureHouseItemInfo);
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
                M2G_DleTreasureHouseItemInfo mg = Message as M2G_DleTreasureHouseItemInfo;
                MailInfo mailinfo = new MailInfo
                {
                    MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                    MailName = "藏宝阁",
                    MailAcceptanceTime = Help_TimeHelper.GetNowSecond(),
                    MailContent = "玩家物品下架邮件",
                    MailState = 0,
                    ReceiveOrNot = 0,
                    MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000
                };
                MailItem mailItem = new MailItem
                {
                    ItemConfigID = mg.ConfigID,
                    AreaId = mg.AreaId,
                    ItemID = mg.Uid,
                    CreateAttr = new ItemCreateAttr() { Quantity = mg.Cnt}
                };
                mailinfo.MailEnclosure.Add(mailItem);
                MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mAreaId).Coroutine();

                b_Reply(b_Response);
                return true;
            }
        }
    }
}