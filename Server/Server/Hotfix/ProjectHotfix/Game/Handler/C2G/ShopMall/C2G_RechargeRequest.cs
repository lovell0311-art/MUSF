using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;
using System.Linq;
using TencentCloud.Hcm.V20181106.Models;
using System.Net;
using MongoDB.Bson;
using System.Net.Http;
using System.Text;
using TencentCloud.Es.V20180416.Models;

namespace ETHotfix
{
#if DEVELOP
    [MessageHandler(AppType.Game)]
    public class C2G_RechargeRequestHandler : AMActorRpcHandler<C2G_RechargeRequest, G2C_RechargeResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_RechargeRequest b_Request, G2C_RechargeResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_RechargeRequest b_Request, G2C_RechargeResponse b_Response, Action<IMessage> b_Reply)
        {
            //b_Reply(b_Response);
            //return true;
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

            var Bk = mPlayer.GetCustomComponent<BackpackComponent>();
            if (Bk == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2200);
                b_Reply(b_Response);
                return false;
            }
            //调用收费接口，接收返回
            int Value = PlayerShopQuota.GetPayValue((PlayerShopQuotaType)b_Request.Page);
            //if (Value == 0 && (PlayerShopQuotaType)b_Request.Page == PlayerShopQuotaType.OneTimeRecharge)
            //{
            //    Value = PlayerShop.GetOneTimeRechargeValue();
            //}

            Log.PLog("PlayerShop", $"名称:{mPlayer.GetCustomComponent<GamePlayer>().Data.NickName} UserID:{mPlayer.UserId} 总元宝:{mPlayer.GetCustomComponent<GamePlayer>().Player.Data.YuanbaoCoin} 获得元宝:{Value} 充值");
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            DBAccountInfo dbLoginInfo = null;
            if (mDBProxy != null)
            {
                var list = await mDBProxy.Query<DBAccountInfo>(p => p.Id == mPlayer.UserId);
                if (list.Count > 0)
                {
                    dbLoginInfo = list[0] as DBAccountInfo;
                }
            }

            long ordefId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
            DBPlayerPayOrderInfo mPlayerPayOrderInfo = new DBPlayerPayOrderInfo
            {
                Id = ordefId,
                App_Ordef_id = ordefId,
                Gid = 0,
                Uid = 0,
                GUid = mPlayer.UserId,
                Rid = mPlayer.GameUserId,
                Product_id = b_Request.Page,
                Money = Value,
                SuccessTime = Help_TimeHelper.GetNowSecond(),
                Time = Help_TimeHelper.GetNowSecond(),
                RName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName,
                Effective = true,
                ChannelId = dbLoginInfo.ChannelId,
                TradePlatform = TradePlatform.GM,
                StatisticalAmount = true
            };

            var mDBProxy2 = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);
            await mDBProxy2.Save(mPlayerPayOrderInfo);
            var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, OptionComponent.Options.AppId);
            Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
            // 充值，订单处理，调用统一接口
            GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
            gM2C_PayReturnMessage.MAreaId = mPlayer.SourceGameAreaId;
            gM2C_PayReturnMessage.AppOrderId = ordefId;
            gM2C_PayReturnMessage.GameUserID = mPlayer.GameUserId;
            gM2C_PayReturnMessage.LogInfo = "GM内部充值消息";
            gameSession.Call(gM2C_PayReturnMessage).Coroutine();
            b_Reply(b_Response);
            return false;
            
            switch ((PlayerShopQuotaType)b_Request.Page)
            {
                case PlayerShopQuotaType.FirstTopUp:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        PlayerShop.SetPageRecharge((int)DeviationType.FirstCharge6, Value);
                        PlayerShop.FirstReceiveItem(DeviationType.FirstCharge6);
                    }
                    break;
                case PlayerShopQuotaType.SecondTopUp:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        PlayerShop.SetPageRecharge((int)DeviationType.FirstCharge38, Value);
                        PlayerShop.FirstReceiveItem(DeviationType.FirstCharge38);
                    }
                    break;
                case PlayerShopQuotaType.ThirdTopUp:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        PlayerShop.SetPageRecharge((int)DeviationType.FirstCharge68, Value);
                        PlayerShop.FirstReceiveItem(DeviationType.FirstCharge68);
                    }
                    break;
                case PlayerShopQuotaType.FourthlyTopUp: 
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        PlayerShop.SetPageRecharge((int)DeviationType.FirstCharge198, Value);
                        PlayerShop.FirstReceiveItem(DeviationType.FirstCharge198);
                    }
                    break;
                case PlayerShopQuotaType.FifthTopUp: 
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        PlayerShop.SetPageRecharge((int)DeviationType.FirstCharge288, Value);
                        PlayerShop.FirstReceiveItem(DeviationType.FirstCharge288);
                    }
                    break;
                case PlayerShopQuotaType.OneTimeRecharge:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        for (DeviationType i = DeviationType.FirstCharge6; i <= DeviationType.FirstCharge288; ++i)
                        {
                            if (PlayerShop.GetRechargeState((int)i))
                            {
                                PlayerShop.SetPageRecharge((int)i, Value);
                                PlayerShop.FirstReceiveItem(i);
                            }
                        }
                    }
                    break;
                case PlayerShopQuotaType.SevenDaysTopUp:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
                        if (ActivitiesInfo != null && ActivitiesInfo.GetActivitState(4))
                        {
                            PlayerShop.SetSevenDay(Value, out int Day);
                            if (Day != 0)
                            {
                                PlayerShop.SevenDayReceiveItem(Day);
                            }
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                            b_Reply(b_Response);
                            return true;
                        }
                    }
                    break;
                case PlayerShopQuotaType.AwardFlag: 
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
                        if (ActivitiesInfo != null && ActivitiesInfo.GetActivitState(3))
                        {
                            ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                            itemCreateAttr.Quantity = 1;
                            itemCreateAttr.IsBind = 1;
                            itemCreateAttr.CustomAttrMethod.Add("ItemRandAddExcAttr_3_6");

                            Item item = ItemFactory.Create(340001, mPlayer.GameAreaId, itemCreateAttr);
                            if (!Bk.AddItem(item, "限时充值奖励旗帜"))
                            {
                                MailInfo mailinfo = new MailInfo();
                                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                                mailinfo.MailName = "限时充值奖励";
                                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                                mailinfo.MailContent = "恭喜玩家获得永久旗帜";
                                mailinfo.MailState = 0;
                                mailinfo.ReceiveOrNot = 0;
                                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                                MailItem mailItem = new MailItem();
                                mailItem.ItemConfigID = 340001;
                                mailItem.ItemID = 0;
                                mailItem.CreateAttr = itemCreateAttr;
                                mailinfo.MailEnclosure.Add(mailItem);
                                MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mAreaId).Coroutine();
                            }
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                            b_Reply(b_Response);
                            return true;
                        }
                    }
                    break;
                case PlayerShopQuotaType.TransformationRing:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
                        if (ActivitiesInfo != null && ActivitiesInfo.GetActivitState(5))
                        {
                            ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                            itemCreateAttr.Quantity = 1;
                            itemCreateAttr.IsBind = 1;
                            Item item = ItemFactory.Create(260017, mPlayer.GameAreaId, itemCreateAttr);
                            if (!Bk.AddItem(item, "六一充值奖励"))
                            {
                                MailInfo mailinfo = new MailInfo();
                                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                                mailinfo.MailName = "五一充值奖励";
                                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                                mailinfo.MailContent = "恭喜玩家获得五一永久道具奖励";
                                mailinfo.MailState = 0;
                                mailinfo.ReceiveOrNot = 0;
                                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                                MailItem mailItem = new MailItem();
                                mailItem.ItemConfigID = 260017;
                                mailItem.ItemID = 0;
                                mailItem.CreateAttr = itemCreateAttr;
                                mailinfo.MailEnclosure.Add(mailItem);
                                MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mAreaId).Coroutine();
                            }
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                            b_Reply(b_Response);
                            return true;
                        }
                    }
                    break;
                case PlayerShopQuotaType.PhoenixMount:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                        var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
                        if (ActivitiesInfo != null && ActivitiesInfo.GetActivitState(5))
                        {
                            ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                            itemCreateAttr.Quantity = 1;
                            itemCreateAttr.IsBind = 1;
                            itemCreateAttr.CustomAttrMethod.Add("ItemRandAddExcAttr_3_6");
                            Item item = ItemFactory.Create(240019, mPlayer.GameAreaId, itemCreateAttr);
                            if (!Bk.AddItem(item, "六一充值奖励"))
                            {
                                MailInfo mailinfo = new MailInfo();
                                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                                mailinfo.MailName = "五一充值奖励";
                                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                                mailinfo.MailContent = "恭喜玩家获得五一永久道具奖励";
                                mailinfo.MailState = 0;
                                mailinfo.ReceiveOrNot = 0;
                                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                                MailItem mailItem = new MailItem();
                                mailItem.ItemConfigID = 240019;
                                mailItem.ItemID = 0;
                                mailItem.CreateAttr = itemCreateAttr;
                                mailinfo.MailEnclosure.Add(mailItem);
                                MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mAreaId).Coroutine();
                            }
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                            b_Reply(b_Response);
                            return true;
                        }
                    }
                    break;
                case PlayerShopQuotaType.StoreRechargeI:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                    }
                    break;
                case PlayerShopQuotaType.StoreRechargeII:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                    }
                    break;
                case PlayerShopQuotaType.StoreRechargeIII:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                    }
                    break;
                case PlayerShopQuotaType.StoreRechargeIV:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                    }
                    break;
                case PlayerShopQuotaType.StoreRechargeV:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                    }
                    break;
                case PlayerShopQuotaType.StoreRechargeVI:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                    }
                    break;
                case PlayerShopQuotaType.StoreRechargeVII:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                    }
                    break;
                case PlayerShopQuotaType.StoreRechargeVIII:
                    {
                        PlayerShop.SetAccumulatedRecharge(Value, $"游戏内部GM Page:{b_Request.Page}");
                    }
                    break;
                default:
                    break;
            }

            if (PlayerShop.GetPlayerShopState(DeviationType.NoRecharge))
            {
                PlayerShop.dBPlayerShopMall.RechargeStatus -= 1;
            }

            if (mPlayer.Data.PayTitle >= 1688 && mPlayer.Data.PayTitle < 20000)
            {
                if (mPlayer.GetCustomComponent<PlayerTitle>().CheckTitle(60006))
                {
                    mPlayer.GetCustomComponent<PlayerTitle>().AddTitle(60006, 1);
                    mPlayer.GetCustomComponent<PlayerTitle>().SendTitle();
                    MailInfo mailinfo = new MailInfo();
                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                    mailinfo.MailName = "充值奖励";
                    mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                    mailinfo.MailContent = "恭喜玩家获得充值称号《天使之翼》";
                    mailinfo.MailState = 0;
                    mailinfo.ReceiveOrNot = 1;
                    mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mAreaId).Coroutine();
                }
            }
            else if (mPlayer.Data.PayTitle >= 20000)
            {
                if (mPlayer.GetCustomComponent<PlayerTitle>().CheckTitle(60005))
                {
                    mPlayer.GetCustomComponent<PlayerTitle>().AddTitle(60005, 1);
                    mPlayer.GetCustomComponent<PlayerTitle>().SendTitle();
                    MailInfo mailinfo = new MailInfo();
                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                    mailinfo.MailName = "充值奖励";
                    mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                    mailinfo.MailContent = "恭喜玩家获得充值称号《主宰无双》";
                    mailinfo.MailState = 0;
                    mailinfo.ReceiveOrNot = 1;
                    mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mAreaId).Coroutine();
                }
            }
            
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            dbLoginInfo = null;
            if (mDBProxy != null)
            {
                {
                    var list = await mDBProxy.Query<DBAccountInfo>(p => p.Id == mPlayer.UserId);
                    if (list.Count > 0) 
                    {
                        dbLoginInfo = list[0] as DBAccountInfo;
                    }
                }
            }
            if (dbLoginInfo != null)
            {
                G2M_EnterRankingRequest g2M_EnterRankingRequest = new G2M_EnterRankingRequest();//充值排行
                g2M_EnterRankingRequest.AppendData = mAreaId;
                g2M_EnterRankingRequest.UserID = mPlayer.UserId;
                g2M_EnterRankingRequest.XyUserId = dbLoginInfo.XYAccountNumber == null ? "" : dbLoginInfo.XYAccountNumber;
                g2M_EnterRankingRequest.RankType = 0;
                g2M_EnterRankingRequest.ValueA = mPlayer.Data.WeeklyTotalPay;//PlayerShop.dBPlayerShopMall.AccumulatedRecharge;
                g2M_EnterRankingRequest.PlayerName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
                g2M_EnterRankingRequest.Level = mPlayer.GetCustomComponent<GamePlayer>().Data.Level;

                IResponse mResult = await mPlayer.GetSessionMGMT().Call(g2M_EnterRankingRequest);
                if (mResult != null && mResult.Error == 0)
                {
                    M2G_EnterRankingResponse m2G_EnterRankingResponse = mResult as M2G_EnterRankingResponse;
                    mPlayer.Data.WeeklyTotalPayTiem = m2G_EnterRankingResponse.EndTime;
                }
            }
            else
            {
                Log.Debug($"充值成功，数据错误没将进入排行uID:{mPlayer.UserId}");
            }
            b_Reply(b_Response);
            return true;
        }
    }
#endif
}