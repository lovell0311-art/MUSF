using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Mrs.V20200910.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class GM2C_PayReturnRequestHandler : AMRpcHandler<GM2C_PayReturnRequest, C2GM_PayReturnResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, GM2C_PayReturnRequest b_Request, C2GM_PayReturnResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = b_Request.MAreaId;
            long AppOrdefId = b_Request.AppOrderId;
            long gameUserId = b_Request.GameUserID;
            //int Areaid = mAreaId << 16;
            //Areaid += 1;//大区没有0线,加以为一线，取ServerArea信息
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(mAreaId);
            if (mServerArea == null)
            {
                b_Reply(b_Response);
                return true;
            }
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, gameUserId))
            {
                PlayerManageComponent playerManage = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();
                Player player = playerManage.Get(mServerArea.GameAreaId, gameUserId);
                if (player == null)
                {
                    // 玩家已经下线
                    // 无需判断 Online 状态，Get 取出的玩家时有已经判断了
                    b_Reply(b_Response);
                    return false;
                }
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mServerArea.GameAreaId);
                if (dBProxy2 == null)
                {
                    b_Reply(b_Response);
                    return false;
                }
                var payInfo = await dBProxy2.Query<DBPlayerPayOrderInfo>(P => P.App_Ordef_id == AppOrdefId);
                if (payInfo == null)
                {
                    b_Reply(b_Response);
                    return false;
                }
                DBPlayerPayOrderInfo dBPlayerPayOrderInfo = payInfo[0] as DBPlayerPayOrderInfo;
                if (dBPlayerPayOrderInfo.Success || dBPlayerPayOrderInfo.Effective == false)
                {
                    b_Reply(b_Response);
                    return false;
                }

                int money = dBPlayerPayOrderInfo.Money;

                var mGamePlayer = player.GetCustomComponent<GamePlayer>();
                var PlayerShopInfo = player.GetCustomComponent<PlayerShopMallComponent>();
                var Bk = player.GetCustomComponent<BackpackComponent>();
                switch ((PlayerShopQuotaType)dBPlayerPayOrderInfo.Product_id)
                {
                    case PlayerShopQuotaType.FirstTopUp:
                        {
                            //PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            //PlayerShopInfo.SetPageRecharge((int)DeviationType.FirstCharge6, money);
                            PlayerShopInfo.FirstReceiveItem(DeviationType.FirstCharge6);
                        }
                        break;
                    case PlayerShopQuotaType.SecondTopUp:
                        {
                            //PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            //PlayerShopInfo.SetPageRecharge((int)DeviationType.FirstCharge38, money);
                            PlayerShopInfo.FirstReceiveItem(DeviationType.FirstCharge38);
                        }
                        break;
                    case PlayerShopQuotaType.ThirdTopUp:
                        {
                            //PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            //PlayerShopInfo.SetPageRecharge((int)DeviationType.FirstCharge68, money);
                            PlayerShopInfo.FirstReceiveItem(DeviationType.FirstCharge68);
                        }
                        break;
                    case PlayerShopQuotaType.FourthlyTopUp:
                        {
                            //PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            //layerShopInfo.SetPageRecharge((int)DeviationType.FirstCharge198, money);
                            PlayerShopInfo.FirstReceiveItem(DeviationType.FirstCharge198);
                        }
                        break;
                    case PlayerShopQuotaType.FifthTopUp:
                        {
                            //PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            //PlayerShopInfo.SetPageRecharge((int)DeviationType.FirstCharge288, money);
                            PlayerShopInfo.FirstReceiveItem(DeviationType.FirstCharge288);
                        }
                        break;
                    case PlayerShopQuotaType.OneTimeRecharge:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            for (DeviationType i = DeviationType.FirstCharge6; i <= DeviationType.FirstCharge288; ++i)
                            {
                                if (PlayerShopInfo.GetRechargeState((int)i))
                                {
                                    PlayerShopInfo.SetPageRecharge((int)i, money);
                                    PlayerShopInfo.FirstReceiveItem(i);
                                }
                            }
                        }
                        break;
                    case PlayerShopQuotaType.SevenDaysTopUp:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
                            if (ActivitiesInfo != null && ActivitiesInfo.GetActivitState(4))
                            {
                                PlayerShopInfo.SetSevenDay(money, out int Day);
                                if (Day != 0)
                                {
                                    PlayerShopInfo.SevenDayReceiveItem(Day);
                                }
                            }
                            else
                            {
                                b_Reply(b_Response);
                                return true;
                            }
                        }
                        break;
                    case PlayerShopQuotaType.AwardFlag:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
                            if (ActivitiesInfo != null && ActivitiesInfo.GetActivitState(3))
                            {
                                ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                itemCreateAttr.Quantity = 1;

                                itemCreateAttr.CustomAttrMethod.Add("ItemRandAddExcAttr_3_6");

                                Item item = ItemFactory.Create(340001, player.GameAreaId, itemCreateAttr);
                                if (!Bk.AddItem(item, "限时充值奖励旗帜"))
                                {
                                    MailInfo mailinfo = new MailInfo();
                                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
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
                                    MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                                }
                            }
                            else
                            {
                                b_Reply(b_Response);
                                return true;
                            }
                        }
                        break;
                    case PlayerShopQuotaType.TransformationRing:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
                            if (ActivitiesInfo != null && ActivitiesInfo.GetActivitState(5))
                            {
                                ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                itemCreateAttr.Quantity = 1;

                                Item item = ItemFactory.Create(260017, player.GameAreaId, itemCreateAttr);
                                if (!Bk.AddItem(item, "六一充值奖励"))
                                {
                                    MailInfo mailinfo = new MailInfo();
                                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
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
                                    MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                                }
                            }
                            else
                            {
                                b_Reply(b_Response);
                                return true;
                            }
                        }
                        break;
                    case PlayerShopQuotaType.PhoenixMount:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
                            if (ActivitiesInfo != null && ActivitiesInfo.GetActivitState(5))
                            {
                                ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                itemCreateAttr.Quantity = 1;

                                itemCreateAttr.CustomAttrMethod.Add("ItemRandAddExcAttr_3_6");
                                Item item = ItemFactory.Create(240019, player.GameAreaId, itemCreateAttr);
                                if (!Bk.AddItem(item, "六一充值奖励"))
                                {
                                    MailInfo mailinfo = new MailInfo();
                                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
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
                                    MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                                }
                            }
                            else
                            {
                                b_Reply(b_Response);
                                return true;
                            }
                        }
                        break;
                    case PlayerShopQuotaType.StoreRechargeI:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                        }
                        break;
                    case PlayerShopQuotaType.StoreRechargeII:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                        }
                        break;
                    case PlayerShopQuotaType.StoreRechargeIII:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                        }
                        break;
                    case PlayerShopQuotaType.StoreRechargeIV:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                        }
                        break;
                    case PlayerShopQuotaType.StoreRechargeV:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                        }
                        break;
                    case PlayerShopQuotaType.StoreRechargeVI:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                        }
                        break;
                    case PlayerShopQuotaType.StoreRechargeVII:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                        }
                        break;
                    case PlayerShopQuotaType.StoreRechargeVIII:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                        }
                        break;
                    case PlayerShopQuotaType.LevelTopUpI:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelTopUp_TypeConfigJson>().JsonDic;
                            foreach (var item in JsonDic)
                            {
                                if (item.Value.Level == 220 && mGamePlayer.Data.PlayerTypeId == item.Value.RoleType)
                                {
                                    int ItmeId = 0;
                                    switch (mGamePlayer.Data.PlayerTypeId)
                                    {
                                        case 1: ItmeId = 80006; break;
                                        case 2: ItmeId = 10016; break;
                                        case 3: ItmeId = 40007; break;
                                        case 4: ItmeId = 80006; break;
                                        case 5: ItmeId = 100002; break;
                                        case 6: ItmeId = 80029; break;
                                    }
                                    if (ItmeId != 0)
                                    {
                                        ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                        itemCreateAttr.Quantity = item.Value.Quantity;
                                        itemCreateAttr.IsBind = item.Value.IsBind;
                                        itemCreateAttr.Level = item.Value.EnhanceLevel;
                                        itemCreateAttr.HaveSkill = item.Value.HasSkill == 1;
                                        itemCreateAttr.HaveLucky = item.Value.HasLucky == 1;
                                        itemCreateAttr.CustomAttrMethod.Add(item.Value.CustomAttrMathod);
                                        {
                                            MailInfo mailinfo = new MailInfo();
                                            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                                            mailinfo.MailName = "等级充值奖励";
                                            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                                            mailinfo.MailContent = "恭喜玩家获得以下奖励道具";
                                            mailinfo.MailState = 0;
                                            mailinfo.ReceiveOrNot = 0;
                                            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                                            MailItem mailItem = new MailItem();
                                            mailItem.ItemConfigID = ItmeId;
                                            mailItem.ItemID = 0;
                                            mailItem.CreateAttr = itemCreateAttr;
                                            mailinfo.MailEnclosure.Add(mailItem);
                                            MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case PlayerShopQuotaType.LevelTopUpIII:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelTopUp_TypeConfigJson>().JsonDic;
                            foreach (var item in JsonDic)
                            {
                                if (item.Value.Level == 280 && mGamePlayer.Data.PlayerTypeId == item.Value.RoleType)
                                {
                                    int ItmeId = 0;
                                    switch (mGamePlayer.Data.PlayerTypeId)
                                    {
                                        case 5: ItmeId = 320298; break;
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 6: ItmeId = 320297; break;
                                    }
                                    if (ItmeId != 0)
                                    {
                                        ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                        itemCreateAttr.Quantity = item.Value.Quantity;
                                        itemCreateAttr.IsBind = item.Value.IsBind;
                                        itemCreateAttr.Level = item.Value.EnhanceLevel;
                                        itemCreateAttr.HaveSkill = item.Value.HasSkill == 1;
                                        itemCreateAttr.HaveLucky = item.Value.HasLucky == 1;
                                        itemCreateAttr.CustomAttrMethod.Add(item.Value.CustomAttrMathod);
                                        {
                                            MailInfo mailinfo = new MailInfo();
                                            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                                            mailinfo.MailName = "等级充值奖励";
                                            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                                            mailinfo.MailContent = "恭喜玩家获得以下奖励道具";
                                            mailinfo.MailState = 0;
                                            mailinfo.ReceiveOrNot = 0;
                                            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                                            MailItem mailItem = new MailItem();
                                            mailItem.ItemConfigID = ItmeId;
                                            mailItem.ItemID = 0;
                                            mailItem.CreateAttr = itemCreateAttr;
                                            mailinfo.MailEnclosure.Add(mailItem);
                                            MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case PlayerShopQuotaType.LevelTopUpII:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelTopUp_TypeConfigJson>().JsonDic;
                            MailInfo mailinfo = new MailInfo();
                            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                            mailinfo.MailName = "等级充值奖励";
                            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                            mailinfo.MailContent = "恭喜玩家获得以下奖励道具";
                            mailinfo.MailState = 0;
                            mailinfo.ReceiveOrNot = 0;
                            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                            foreach (var item in JsonDic)
                            {
                                if (item.Value.Level == 250)
                                {
                                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                    itemCreateAttr.Quantity = item.Value.Quantity;
                                    itemCreateAttr.IsBind = item.Value.IsBind;
                                    itemCreateAttr.Level = item.Value.EnhanceLevel;
                                    itemCreateAttr.HaveSkill = item.Value.HasSkill == 1;
                                    itemCreateAttr.HaveLucky = item.Value.HasLucky == 1;
                                    itemCreateAttr.CustomAttrMethod.Add(item.Value.CustomAttrMathod);
                                    MailItem mailItem = new MailItem();
                                    mailItem.ItemConfigID = item.Value.ConfigId;
                                    mailItem.ItemID = 0;
                                    mailItem.CreateAttr = itemCreateAttr;
                                    mailinfo.MailEnclosure.Add(mailItem);
                                }
                            }
                            MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                        }
                        break;
                    case PlayerShopQuotaType.LevelTopUpIV:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelTopUp_TypeConfigJson>().JsonDic;
                            MailInfo mailinfo = new MailInfo();
                            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                            mailinfo.MailName = "等级充值奖励";
                            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                            mailinfo.MailContent = "恭喜玩家获得以下奖励道具";
                            mailinfo.MailState = 0;
                            mailinfo.ReceiveOrNot = 0;
                            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                            foreach (var item in JsonDic)
                            {
                                if (item.Value.Level == 300)
                                {
                                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                    itemCreateAttr.Quantity = item.Value.Quantity;
                                    itemCreateAttr.IsBind = item.Value.IsBind;
                                    itemCreateAttr.Level = item.Value.EnhanceLevel;
                                    itemCreateAttr.HaveSkill = item.Value.HasSkill == 1;
                                    itemCreateAttr.HaveLucky = item.Value.HasLucky == 1;
                                    itemCreateAttr.CustomAttrMethod.Add(item.Value.CustomAttrMathod);
                                    MailItem mailItem = new MailItem();
                                    mailItem.ItemConfigID = item.Value.ConfigId;
                                    mailItem.ItemID = 0;
                                    mailItem.CreateAttr = itemCreateAttr;
                                    mailinfo.MailEnclosure.Add(mailItem);
                                }
                            }
                            MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                        }
                        break;
                    case PlayerShopQuotaType.LevelTopUpV:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelTopUp_TypeConfigJson>().JsonDic;
                            MailInfo mailinfo = new MailInfo();
                            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                            mailinfo.MailName = "等级充值奖励";
                            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                            mailinfo.MailContent = "恭喜玩家获得以下奖励道具";
                            mailinfo.MailState = 0;
                            mailinfo.ReceiveOrNot = 0;
                            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                            foreach (var item in JsonDic)
                            {
                                if (item.Value.Level == 320)
                                {
                                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                    itemCreateAttr.Quantity = item.Value.Quantity;
                                    itemCreateAttr.IsBind = item.Value.IsBind;
                                    itemCreateAttr.Level = item.Value.EnhanceLevel;
                                    itemCreateAttr.HaveSkill = item.Value.HasSkill == 1;
                                    itemCreateAttr.HaveLucky = item.Value.HasLucky == 1;
                                    itemCreateAttr.CustomAttrMethod.Add(item.Value.CustomAttrMathod);
                                    MailItem mailItem = new MailItem();
                                    mailItem.ItemConfigID = item.Value.ConfigId;
                                    mailItem.ItemID = 0;
                                    mailItem.CreateAttr = itemCreateAttr;
                                    mailinfo.MailEnclosure.Add(mailItem);
                                }
                            }
                            MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                        }
                        break;
                    case PlayerShopQuotaType.LevelTopUpVI:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelTopUp_TypeConfigJson>().JsonDic;
                            MailInfo mailinfo = new MailInfo();
                            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                            mailinfo.MailName = "等级充值奖励";
                            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                            mailinfo.MailContent = "恭喜玩家获得以下奖励道具";
                            mailinfo.MailState = 0;
                            mailinfo.ReceiveOrNot = 0;
                            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                            foreach (var item in JsonDic)
                            {
                                if (item.Value.Level == 350)
                                {
                                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                    itemCreateAttr.Quantity = item.Value.Quantity;
                                    itemCreateAttr.IsBind = item.Value.IsBind;
                                    itemCreateAttr.Level = item.Value.EnhanceLevel;
                                    itemCreateAttr.HaveSkill = item.Value.HasSkill == 1;
                                    itemCreateAttr.HaveLucky = item.Value.HasLucky == 1;
                                    itemCreateAttr.CustomAttrMethod.Add(item.Value.CustomAttrMathod);
                                    MailItem mailItem = new MailItem();
                                    mailItem.ItemConfigID = item.Value.ConfigId;
                                    mailItem.ItemID = 0;
                                    mailItem.CreateAttr = itemCreateAttr;
                                    mailinfo.MailEnclosure.Add(mailItem);
                                }
                            }
                            MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                        }
                        break;
                    case PlayerShopQuotaType.LevelTopUpVII:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<LevelTopUp_TypeConfigJson>().JsonDic;
                            MailInfo mailinfo = new MailInfo();
                            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                            mailinfo.MailName = "等级充值奖励";
                            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                            mailinfo.MailContent = "恭喜玩家获得以下奖励道具";
                            mailinfo.MailState = 0;
                            mailinfo.ReceiveOrNot = 0;
                            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                            foreach (var item in JsonDic)
                            {
                                if (item.Value.Level == 400)
                                {
                                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                    itemCreateAttr.Quantity = item.Value.Quantity;
                                    itemCreateAttr.IsBind = item.Value.IsBind;
                                    itemCreateAttr.Level = item.Value.EnhanceLevel;
                                    itemCreateAttr.HaveSkill = item.Value.HasSkill == 1;
                                    itemCreateAttr.HaveLucky = item.Value.HasLucky == 1;
                                    itemCreateAttr.CustomAttrMethod.Add(item.Value.CustomAttrMathod);
                                    MailItem mailItem = new MailItem();
                                    mailItem.ItemConfigID = item.Value.ConfigId;
                                    mailItem.ItemID = 0;
                                    mailItem.CreateAttr = itemCreateAttr;
                                    mailinfo.MailEnclosure.Add(mailItem);
                                }
                            }
                            MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                        }
                        break;
                    case PlayerShopQuotaType.ActiveTopUpI:
                        {
                            PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                            var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
                            if (ActivitiesInfo != null && ActivitiesInfo.GetActivitState(6))
                            {
                                MailInfo mailinfo = new MailInfo();
                                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                                mailinfo.MailName = "限时充值奖励";
                                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                                mailinfo.MailContent = "恭喜玩家获得奖励道具";
                                mailinfo.MailState = 0;
                                mailinfo.ReceiveOrNot = 0;
                                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;

                                ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                                itemCreateAttr.Quantity = 1;
                                itemCreateAttr.CustomAttrMethod.Add("ItemRandAddExcAttr_3_6");

                                MailItem mailItem = new MailItem
                                {
                                    ItemConfigID = 340006,
                                    ItemID = 0,
                                    CreateAttr = itemCreateAttr
                                };
                                MailItem mailItem2 = new MailItem
                                {
                                    ItemConfigID = 240019,
                                    ItemID = 0,
                                    CreateAttr = itemCreateAttr
                                };
                                mailinfo.MailEnclosure.Add(mailItem);
                                mailinfo.MailEnclosure.Add(mailItem2);
                                MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();

                            }
                            else
                            {
                                b_Reply(b_Response);
                                return true;
                            }
                        }
                        break;
                    case PlayerShopQuotaType.SpecifiedAmount:
                        PlayerShopInfo.SetAccumulatedRecharge(money, $"{b_Request.LogInfo} Page:{dBPlayerPayOrderInfo.Product_id}");
                        break;
                    default:
                        break;
                }

                if (PlayerShopInfo.GetPlayerShopState(DeviationType.NoRecharge))
                {
                    PlayerShopInfo.dBPlayerShopMall.RechargeStatus -= 1;
                }
                var PlayerTitle = player.GetCustomComponent<PlayerTitle>();
                if (player.Data.AccumulatedRecharge >= 688)
                {
                    if (PlayerTitle.CheckTitle(60006))
                    {
                        PlayerTitle.AddTitle(60006, 1);
                        PlayerTitle.SendTitle();
                        MailInfo mailinfo = new MailInfo();
                        mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                        mailinfo.MailName = "充值奖励";
                        mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                        mailinfo.MailContent = "恭喜玩家获得充值称号《精灵之魂》";
                        mailinfo.MailState = 0;
                        mailinfo.ReceiveOrNot = 1;
                        mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                        MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                    }
                }
                //else if (player.Data.AccumulatedRecharge >= 298 && player.Data.AccumulatedRecharge < 688)//国庆修改成16888过后恢复
                //{
                //    string Msg = "恭喜玩家获得充值称号《勇者之名》";
                //    if (PlayerTitle.CheckTitle(60006))
                //    {
                //        PlayerTitle.AddTitle(60006, 1);
                //        Msg = "恭喜玩家获得充值称号《勇者之名》《天使之翼》";
                //    }
                //    if (PlayerTitle.CheckTitle(60019))
                //    {
                //        PlayerTitle.AddTitle(60019,1);
                //        PlayerTitle.SendTitle();
                //        MailInfo mailinfo = new MailInfo();
                //        mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                //        mailinfo.MailName = "充值奖励";
                //        mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                //        mailinfo.MailContent = Msg;
                //        mailinfo.MailState = 0;
                //        mailinfo.ReceiveOrNot = 1;
                //        mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                //        MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                //    }
                //}
                //else if (player.Data.AccumulatedRecharge >= 688 && player.Data.AccumulatedRecharge < 996)//国庆修改成16888过后恢复
                //{
                //    string Msg = "恭喜玩家获得充值称号《精灵之魂》";
                //    bool IsTitle = false;
                //    if (PlayerTitle.CheckTitle(60006))
                //    {
                //        PlayerTitle.AddTitle(60006, 1);
                //        Msg = "恭喜玩家获得充值称号《精灵之魂》《勇者之名》";
                //        IsTitle = true;
                //    }
                //    if (PlayerTitle.CheckTitle(60019))
                //    {
                //        PlayerTitle.AddTitle(60019, 1);
                //        if(IsTitle)
                //            Msg = "恭喜玩家获得充值称号《精灵之魂》《勇者之名》《天使之翼》";
                //        else
                //            Msg = "恭喜玩家获得充值称号《精灵之魂》《天使之翼》";
                //    }
                //    if (PlayerTitle.CheckTitle(60020))
                //    {
                //        PlayerTitle.AddTitle(60020, 1);
                //        PlayerTitle.SendTitle();
                //        MailInfo mailinfo = new MailInfo();
                //        mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                //        mailinfo.MailName = "充值奖励";
                //        mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                //        mailinfo.MailContent = Msg;
                //        mailinfo.MailState = 0;
                //        mailinfo.ReceiveOrNot = 1;
                //        mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                //        MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                //    }
                //}
                //else if(player.Data.AccumulatedRecharge >= 996)
                //{
                //    string Msg = "恭喜玩家获得充值称号《奇迹行者》";
                //    if (PlayerTitle.CheckTitle(60020))
                //    {
                //        PlayerTitle.AddTitle(60020, 1);
                //        Msg = "恭喜玩家获得充值称号《精灵之魂》《奇迹行者》";
                //    }
                //    if (PlayerTitle.CheckTitle(60021))
                //    {
                //        PlayerTitle.AddTitle(60021, 1);
                //        PlayerTitle.SendTitle();
                //        MailInfo mailinfo = new MailInfo();
                //        mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                //        mailinfo.MailName = "充值奖励";
                //        mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                //        mailinfo.MailContent = Msg;
                //        mailinfo.MailState = 0;
                //        mailinfo.ReceiveOrNot = 1;
                //        mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                //        MailSystem.SendMail(player.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                //    }
                //}
                PlayerShopInfo.SendPlayerShopState();

                dBPlayerPayOrderInfo.Success = true;
                await dBProxy2.Save(dBPlayerPayOrderInfo);

                // 重新统计充值金额
                await player.GetCustomComponent<CumulativeRechargeComponent>().Load();
            }

            b_Reply(b_Response);
            return true;

        }
    }
}