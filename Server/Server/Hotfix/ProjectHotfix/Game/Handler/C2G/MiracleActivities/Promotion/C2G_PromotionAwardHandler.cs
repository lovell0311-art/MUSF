using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PromotionAwardHandler : AMActorRpcHandler<C2G_PromotionAward, G2C_PromotionAward>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PromotionAward b_Request, G2C_PromotionAward b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PromotionAward b_Request, G2C_PromotionAward b_Response, Action<IMessage> b_Reply)
        {
            b_Reply(b_Response);
            return true;
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_AccountNotExists;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }

            G2M_ClaimVerification g2M_ClaimVerification = new G2M_ClaimVerification();
            g2M_ClaimVerification.AppendData = b_Request.AppendData;
            g2M_ClaimVerification.UserID = mPlayer.UserId;
            g2M_ClaimVerification.Type = b_Request.Type;
            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
            Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
            IResponse Message = await loginCenterSession.Call(g2M_ClaimVerification);
            //IResponse Message = await mPlayer.GetSessionMGMT().Call(g2M_ClaimVerification);
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;//Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2502);
                b_Reply(b_Response);
                return false;
            }
            if (b_Request.Type == 1)
            {
                var Title = mPlayer.GetCustomComponent<PlayerTitle>();
                if (Title != null)
                {
                    if (Title.CheckTitle(60007))
                    {
                        Title.AddTitle(60007,1);
                        Title.SendTitle();
                    }
                }
            }
            else if (b_Request.Type == 2)
            {
                var bk = mPlayer.GetCustomComponent<BackpackComponent>();
                if (bk != null)
                {
                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                    itemCreateAttr.Quantity = 1;
                    itemCreateAttr.CustomAttrMethod.Add("ItemAddExcAttr");
                    List<Item> mDropItem = ItemFactory.CreateMany(340001, mPlayer.GameAreaId, itemCreateAttr);
                    if (mDropItem != null && mDropItem.Count == 1)
                    {
                        if (!bk.AddItem(mDropItem[0],"邀请码领取奖励"))
                        {
                            MailInfo mailinfo = new MailInfo();
                            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                            mailinfo.MailName = "邀请码领取奖励";
                            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                            mailinfo.MailContent = "恭喜玩家获得奖励道具";
                            mailinfo.MailState = 0;
                            mailinfo.ReceiveOrNot = 0;
                            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                            MailItem mailItem = new MailItem
                            {
                                ItemConfigID = 340001,
                                ItemID = 0,
                                CreateAttr = itemCreateAttr
                            };
                            mailinfo.MailEnclosure.Add(mailItem);
                            MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mPlayer.GameAreaId).Coroutine();

                        }
                    }
                }
            }
            else if (b_Request.Type == 3)
            {
                var mGamePlayer = mPlayer.GetCustomComponent <PlayerShopMallComponent>();
                mGamePlayer?.SetMiracleCoin(200, "邀请码奖励");
            }
            b_Reply(b_Response);
            return true;

        }
    }
}