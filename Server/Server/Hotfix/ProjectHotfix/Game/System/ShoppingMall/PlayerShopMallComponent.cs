using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using Org.BouncyCastle.Crypto;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Mrs.V20200910.Models;
using TencentCloud.Scf.V20180416.Models;
using TencentCloud.Vod.V20180717.Models;

namespace ETHotfix
{
    /// <summary>
    /// 充值类型以及金额 单位：元
    /// </summary>
    public static class PlayerShopQuota
    {
        public const int Amount         = 0;
        public const int AmountI        = 10;
        public const int AmountII       = 20;
        public const int AmountIII      = 20;
        public const int AmountIV       = 36;
        public const int AmountV        = 68;
        public const int AmountVI       = 68;
        public const int AmountVII      = 96;
        public const int AmountVIII     = 128;
        public const int AmountIX       = 168;
        public const int AmountX        = 328;
        public const int AmountXI       = 368;
        public const int AmountXII      = 588;
        public const int AmountXIII     = 648;
        public const int AmountXIV      = 668;
        public const int AmountXV       = 688;
        public const int AmountXVI      = 888;
        public const int AmountXVII     = 20000;
        public const int AmountXVIII    = 1288;
        public const int AmountXIX      = 1688;
        public const int AmountXX       = 2000;
        public const int AmountXXI      = 2998;
        //public const int AmountXXII   = ;


        public static int GetPayValue(PlayerShopQuotaType playerShopQuotaType)
        {
            switch (playerShopQuotaType)
            {
                case PlayerShopQuotaType.FirstTopUp: return AmountI;
                case PlayerShopQuotaType.SecondTopUp: return AmountII;
                case PlayerShopQuotaType.ThirdTopUp: return AmountII;
                case PlayerShopQuotaType.FourthlyTopUp: return AmountIV;
                case PlayerShopQuotaType.FifthTopUp: return AmountV;
                case PlayerShopQuotaType.SevenDaysTopUp: return AmountVI;
                case PlayerShopQuotaType.AwardFlag: return AmountXII;
                case PlayerShopQuotaType.TransformationRing: return AmountXV;
                case PlayerShopQuotaType.PhoenixMount: return AmountXVI;
                case PlayerShopQuotaType.StoreRechargeI: return AmountI;
                case PlayerShopQuotaType.StoreRechargeII: return AmountIII;
                case PlayerShopQuotaType.StoreRechargeIII: return AmountVI;
                case PlayerShopQuotaType.StoreRechargeIV: return AmountVIII;
                case PlayerShopQuotaType.StoreRechargeV: return AmountX;
                case PlayerShopQuotaType.StoreRechargeVI: return AmountXIII;
                case PlayerShopQuotaType.StoreRechargeVII: return AmountXVII;
                case PlayerShopQuotaType.StoreRechargeVIII: return AmountXXI;
                //case PlayerShopQuotaType.OneTimeRecharge: return Amount;
                //case PlayerShopQuotaType.LevelTopUpI: return AmountIX;
                //case PlayerShopQuotaType.LevelTopUpII: return AmountXI;
                //case PlayerShopQuotaType.LevelTopUpIII: return AmountXIV;
                //case PlayerShopQuotaType.LevelTopUpIV: return AmountXVII;
                //case PlayerShopQuotaType.LevelTopUpV: return AmountXVIII;
                //case PlayerShopQuotaType.LevelTopUpVI: return AmountXIX;
                //case PlayerShopQuotaType.LevelTopUpVII: return AmountXX;
                //case PlayerShopQuotaType.ActiveTopUpI: return AmountXX;
                default:
                    break;
            }
            return -1;
        }

    }
    public static class PlayerShopMallSystemComponent
    {
        /// <summary>
        /// 加载账号充值信息，没有创建一个
        /// </summary>
        /// <param name="b_Component"></param>
        public static async Task<bool> PlayerLoadShopMall(this PlayerShopMallComponent b_Component)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Component.mPlayer.AddCustomComponent<DataCacheManageComponent>();
            var mDataCache = mDataCacheComponent.Get<DBPlayerShopMall>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, b_Component.mPlayer.GameAreaId);
            if (mDataCache == null)
            {
                mDataCache = await mDataCacheComponent.Add<DBPlayerShopMall>(dBProxy2, p => p.GameUserID == b_Component.mPlayer.GameUserId);
            }

            var ShopInfo = mDataCache.DataQuery(p => p.GameUserID == b_Component.mPlayer.GameUserId);
            if (ShopInfo.Count >= 1)
            {
                b_Component.dBPlayerShopMall = ShopInfo[0];
                if (b_Component.GetPlayerShopState(DeviationType.MaxMonthlyCard))
                {
                    if (Help_TimeHelper.GetNow() > b_Component.dBPlayerShopMall.InSituCd)
                        b_Component.dBPlayerShopMall.InSituCd = 0;
                }
                //b_Component.dBPlayerShopMall.SevenRecharge = Help_JsonSerializeHelper.DeSerialize<Dictionary<(string,int), (int, bool)>>(b_Component.dBPlayerShopMall.StrSevenRecharge);
                if (b_Component.dBPlayerShopMall.StrSevenRecharge != "")
                {
                    b_Component.dBPlayerShopMall.SevenRecharge = b_Component.dBPlayerShopMall.StrSevenRecharge.Split(';')
                    .Select(x => x.Split(','))
                    .ToDictionary(
                    x => (x[0], int.Parse(x[1])),
                    x => (int.Parse(x[2]), bool.Parse(x[3]))
                    );
                    var Info = b_Component.dBPlayerShopMall.SevenRecharge;
                    if (Info != null && Info.Count > 0)
                    {
                        G2C_SevenDaysToRechargeMessage g2C_SevenDaysToRechargeMessage = new G2C_SevenDaysToRechargeMessage();
                        g2C_SevenDaysToRechargeMessage.Info = b_Component.dBPlayerShopMall.StrSevenRecharge;
                        b_Component.mPlayer.Send(g2C_SevenDaysToRechargeMessage);
                    }
                }
                b_Component.dBPlayerShopMall.RechargeRecordList =
                    Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(b_Component.dBPlayerShopMall.RechargeRecord) ??
                    new Dictionary<int, int>();
                if (b_Component.SyncFirstChargeStateFromRecord())
                {
                    b_Component.SetPlayerShopDB();
                }
                //if (b_Component.GetPlayerShopState(DeviationType.MaxMonthlyCard))
                //{
                //    if (b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic.ContainsKey(E_GameProperty.ExperienceBonus))
                //        b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExperienceBonus] += 20;
                //    else
                //    {
                //        b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExperienceBonus] = 0;
                //        b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExperienceBonus] += 20;
                //    }

                //    if (b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic.ContainsKey(E_GameProperty.ExplosionRate))
                //        b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExplosionRate] += 20;
                //    else
                //    {
                //        b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExplosionRate] = 0;
                //        b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExplosionRate] += 20;
                //    }

                //    if (b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic.ContainsKey(E_GameProperty.GoldCoinMarkup))
                //        b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.GoldCoinMarkup] += 20;
                //    else
                //    {
                //        b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.GoldCoinMarkup] = 0;
                //        b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.GoldCoinMarkup] += 20;
                //    }
                //    //b_Component.SetMonthlyCardStatus(true);
                //}
            }
            else if (ShopInfo.Count == 0)
            {
                DBPlayerShopMall dBPlayerShopMall = new DBPlayerShopMall();
                dBPlayerShopMall.Id = IdGeneraterNew.Instance.GenerateUnitId(b_Component.mPlayer.GameAreaId);
                dBPlayerShopMall.GameUserID = b_Component.mPlayer.GameUserId;
                b_Component.dBPlayerShopMall = dBPlayerShopMall;
                mDataCache.DataAdd(dBPlayerShopMall);
                await dBProxy2.Save(dBPlayerShopMall);
            }
            b_Component.SendPlayerShopState();
           
            return true;
        }
        public static async Task<bool> SetPayInfo(this PlayerShopMallComponent b_Component, long appOrderId, int money, long XYId)
        {
            //int mAreaId = b_Component.Parent.GameAreaId;
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Component.Parent.SourceGameAreaId);
            if (mServerArea == null)
            {
                Log.Warning($"充值未到账登录加载时异常UserId:{b_Component.Parent.UserId}");
            }
            var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, OptionComponent.Options.AppId);
            Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
            // 充值，订单处理，调用统一接口
            GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
            gM2C_PayReturnMessage.MAreaId = b_Component.Parent.SourceGameAreaId;
            gM2C_PayReturnMessage.AppOrderId = appOrderId;
            gM2C_PayReturnMessage.GameUserID = b_Component.Parent.GameUserId;
            gM2C_PayReturnMessage.LogInfo = "充值未到账登录补充";
            gameSession.Call(gM2C_PayReturnMessage).Coroutine();
            return true;
        }
        public static int GetOneTimeRechargeValue(this PlayerShopMallComponent b_Component)
        {
            int Value = 0;
            for (DeviationType i = DeviationType.FirstCharge6; i <= DeviationType.FirstCharge288; ++i)
            {
                if (b_Component.GetRechargeState((int)i))
                {
                    switch (i)
                    {
                        case DeviationType.FirstCharge6: Value += 6; break;
                        case DeviationType.FirstCharge38: Value += 16; break;
                        case DeviationType.FirstCharge68: Value += 36; break;
                        case DeviationType.FirstCharge198: Value += 66; break;
                        case DeviationType.FirstCharge288: Value += 96; break;
                    }
                }
            }
            return Value;
        }
        public static async Task UpdateTitle(this PlayerShopMallComponent b_Component, int Value)
        {
            var player = b_Component.Parent;
            var PlayerTitle = player.GetCustomComponent<PlayerTitle>();
            if (player.Data.AccumulatedRecharge >= 99 && player.Data.AccumulatedRecharge < 298)
            {
                if (PlayerTitle.CheckTitle(60006))
                {
                    PlayerTitle.AddTitle(60006, 1);
                    PlayerTitle.SendTitle();
                    MailInfo mailinfo = new MailInfo();
                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                    mailinfo.MailName = "充值奖励";
                    mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                    mailinfo.MailContent = "恭喜玩家获得充值称号《天使之翼》";
                    mailinfo.MailState = 0;
                    mailinfo.ReceiveOrNot = 1;
                    mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    MailSystem.SendMail(player.GameUserId, mailinfo, player.GameAreaId).Coroutine();
                }
            }
            else if (player.Data.AccumulatedRecharge >= 298 && player.Data.AccumulatedRecharge < 688)//国庆修改成16888过后恢复
            {
                string Msg = "恭喜玩家获得充值称号《勇者之名》";
                if (PlayerTitle.CheckTitle(60006))
                {
                    PlayerTitle.AddTitle(60006, 1);
                    Msg = "恭喜玩家获得充值称号《勇者之名》《天使之翼》";
                }
                if (PlayerTitle.CheckTitle(60019))
                {
                    PlayerTitle.AddTitle(60019, 1);
                    PlayerTitle.SendTitle();
                    MailInfo mailinfo = new MailInfo();
                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                    mailinfo.MailName = "充值奖励";
                    mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                    mailinfo.MailContent = Msg;
                    mailinfo.MailState = 0;
                    mailinfo.ReceiveOrNot = 1;
                    mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    MailSystem.SendMail(player.GameUserId, mailinfo, player.GameAreaId).Coroutine();
                }
            }
            else if (player.Data.AccumulatedRecharge >= 688 && player.Data.AccumulatedRecharge < 996)//国庆修改成16888过后恢复
            {
                string Msg = "恭喜玩家获得充值称号《精灵之魂》";
                bool IsTitle = false;
                if (PlayerTitle.CheckTitle(60006))
                {
                    PlayerTitle.AddTitle(60006, 1);
                    Msg = "恭喜玩家获得充值称号《精灵之魂》《勇者之名》";
                    IsTitle = true;
                }
                if (PlayerTitle.CheckTitle(60019))
                {
                    PlayerTitle.AddTitle(60019, 1);
                    if (IsTitle)
                        Msg = "恭喜玩家获得充值称号《精灵之魂》《勇者之名》《天使之翼》";
                    else
                        Msg = "恭喜玩家获得充值称号《精灵之魂》《天使之翼》";
                }
                if (PlayerTitle.CheckTitle(60020))
                {
                    PlayerTitle.AddTitle(60020, 1);
                    PlayerTitle.SendTitle();
                    MailInfo mailinfo = new MailInfo();
                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                    mailinfo.MailName = "充值奖励";
                    mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                    mailinfo.MailContent = Msg;
                    mailinfo.MailState = 0;
                    mailinfo.ReceiveOrNot = 1;
                    mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    MailSystem.SendMail(player.GameUserId, mailinfo, player.GameAreaId).Coroutine();
                }
            }
            else if (player.Data.AccumulatedRecharge >= 996)
            {
                if (PlayerTitle.CheckTitle(60021))
                {
                    PlayerTitle.AddTitle(60021, 1);
                    PlayerTitle.SendTitle();
                    MailInfo mailinfo = new MailInfo();
                    mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
                    mailinfo.MailName = "充值奖励";
                    mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                    mailinfo.MailContent = "恭喜玩家获得充值称号《奇迹行者》";
                    mailinfo.MailState = 0;
                    mailinfo.ReceiveOrNot = 1;
                    mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    MailSystem.SendMail(player.GameUserId, mailinfo, player.GameAreaId).Coroutine();
                }
            }

        }
        /// <summary>
        /// 累计充值
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Value"></param>
        /// <param name="log">什么原因，详细，一定要详细</param>
        /// <returns></returns>
        public static bool SetAccumulatedRecharge(this PlayerShopMallComponent b_Component, int Value, string log)
        {
            b_Component.dBPlayerShopMall.AccumulatedRecharge += Value;
            b_Component.Parent.Data.AccumulatedRecharge += Value;
            //b_Component.Parent.Data.PayTitle += Value;

            if (b_Component.Parent.Data.WeeklyTotalPayTiem <= Help_TimeHelper.GetNowSecond())
                b_Component.Parent.Data.WeeklyTotalPay = 0;
            b_Component.Parent.Data.WeeklyTotalPay += Value;

            b_Component.SetPlayerYuanBao(Value, log);
            return true;
        }
        /// <summary>
        /// 获取首充每档的状态
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool GetRechargeState(this PlayerShopMallComponent b_Component, int Type)
        {
            b_Component.dBPlayerShopMall.RechargeRecordList ??= new Dictionary<int, int>();

            if (b_Component.dBPlayerShopMall.RechargeRecordList.ContainsKey(Type))
            {
                return false;
            }
            if (IsFirstChargeType(Type) && (b_Component.dBPlayerShopMall.RechargeStatus & Type) == Type)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 首充
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Page"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool SetPageRecharge(this PlayerShopMallComponent b_Component, int Page, int Value)
        {
            if (Page < 0) return false;
            if (b_Component == null) return false;
            if (b_Component.dBPlayerShopMall.RechargeRecordList == null)
                b_Component.dBPlayerShopMall.RechargeRecordList = new Dictionary<int, int>();

            if (b_Component.dBPlayerShopMall.RechargeRecordList.ContainsKey(Page))
            {
                b_Component.dBPlayerShopMall.RechargeRecordList[Page] += Value;
            }
            else
            {
                b_Component.dBPlayerShopMall.RechargeRecordList.Add(Page, Value);
            }
            b_Component.dBPlayerShopMall.RechargeRecord = Help_JsonSerializeHelper.Serialize(b_Component.dBPlayerShopMall.RechargeRecordList);
            b_Component.TryMarkFirstChargeState(Page);

            //b_Component.SetPlayerShopDB();
            return true;
        }

        private static bool SyncFirstChargeStateFromRecord(this PlayerShopMallComponent b_Component)
        {
            if (b_Component?.dBPlayerShopMall?.RechargeRecordList == null)
            {
                return false;
            }

            bool modified = false;
            foreach (int type in b_Component.dBPlayerShopMall.RechargeRecordList.Keys)
            {
                if (!IsFirstChargeType(type))
                {
                    continue;
                }

                if ((b_Component.dBPlayerShopMall.RechargeStatus & type) == type)
                {
                    continue;
                }

                b_Component.dBPlayerShopMall.RechargeStatus |= type;
                modified = true;
            }

            return modified;
        }

        private static void TryMarkFirstChargeState(this PlayerShopMallComponent b_Component, int type)
        {
            if (!IsFirstChargeType(type))
            {
                return;
            }

            b_Component.dBPlayerShopMall.RechargeStatus |= type;
        }

        private static bool IsFirstChargeType(int type)
        {
            switch ((DeviationType)type)
            {
                case DeviationType.FirstCharge6:
                case DeviationType.FirstCharge38:
                case DeviationType.FirstCharge68:
                case DeviationType.FirstCharge198:
                case DeviationType.FirstCharge288:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 7天充值
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool SetSevenDay(this PlayerShopMallComponent b_Component, int Value, out int CntDay)
        {
            if (b_Component == null)
            {
                CntDay = 0;
                return false;
            }
            if (b_Component.dBPlayerShopMall.SevenRecharge == null)
                b_Component.dBPlayerShopMall.SevenRecharge = new Dictionary<(string, int), (int, bool)>();

            int mState = 0;
            DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            string Time = dateTime.ToString();
            int Day = b_Component.dBPlayerShopMall.SevenRecharge.Count;
            if (b_Component.dBPlayerShopMall.SevenRecharge.TryGetValue((Time, Day), out (int, bool) Info) != false)
            {
                (int, bool) SetValue;
                (string, int) SetKey = (Time, Day);

                SetValue.Item1 = Info.Item1 + Value;
                SetValue.Item2 = Info.Item2;
                b_Component.dBPlayerShopMall.SevenRecharge.Remove(SetKey);
                b_Component.dBPlayerShopMall.SevenRecharge.Add(SetKey, SetValue);
                mState = SetKey.Item2;

            }
            else
            {
                ++Day;
                b_Component.dBPlayerShopMall.SevenRecharge.Add((Time, Day), (Value, false));
                mState = Day;
            }
            //b_Component.dBPlayerShopMall.StrSevenRecharge = Help_JsonSerializeHelper.Serialize(b_Component.dBPlayerShopMall.SevenRecharge);
            b_Component.dBPlayerShopMall.StrSevenRecharge = string.Join(";", b_Component.dBPlayerShopMall.SevenRecharge.Select(x => $"{x.Key.Item1},{x.Key.Item2},{x.Value.Item1},{x.Value.Item2}"));
            G2C_SevenDaysToRechargeMessage g2C_SevenDaysToRechargeMessage = new G2C_SevenDaysToRechargeMessage();
            g2C_SevenDaysToRechargeMessage.Info = b_Component.dBPlayerShopMall.StrSevenRecharge;
            b_Component.mPlayer.Send(g2C_SevenDaysToRechargeMessage);
            b_Component.SetPlayerShopDB();
            CntDay = mState;
            return true;
        }
        public static bool SevenReceive(this PlayerShopMallComponent b_Component, int Day = 0)
        {
            if (b_Component == null) return false;
            if (b_Component.dBPlayerShopMall.SevenRecharge == null) return false;

            foreach (var Info in b_Component.dBPlayerShopMall.SevenRecharge)
            {
                (string, int) DayInfo = Info.Key;
                if (DayInfo.Item2 == Day)
                {
                    if (!Info.Value.Item2)
                    {
                        var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<SevenDay_RewardPropsConfigJson>().JsonDic;
                        if (JsonDic != null)
                        {
                            //if (JsonDic[Day].Limit <= Info.Value.Item1)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
        public static bool SetSevenReceiveState(this PlayerShopMallComponent b_Component, int Day = 0)
        {
            if (b_Component == null) return false;
            if (b_Component.dBPlayerShopMall.SevenRecharge == null) return false;

            foreach (var Info in b_Component.dBPlayerShopMall.SevenRecharge)
            {
                (string, int) DayInfo = Info.Key;
                if (DayInfo.Item2 == Day)
                {
                    if (!Info.Value.Item2)
                    {
                        (int, bool) SetValue;
                        SetValue.Item1 = Info.Value.Item1;
                        SetValue.Item2 = true;
                        b_Component.dBPlayerShopMall.SevenRecharge.Remove(DayInfo);
                        b_Component.dBPlayerShopMall.SevenRecharge.Add(DayInfo, SetValue);
                        b_Component.dBPlayerShopMall.StrSevenRecharge = string.Join(";", b_Component.dBPlayerShopMall.SevenRecharge.Select(x => $"{x.Key.Item1},{x.Key.Item2},{x.Value.Item1},{x.Value.Item2}"));

                        G2C_SevenDaysToRechargeMessage g2C_SevenDaysToRechargeMessage = new G2C_SevenDaysToRechargeMessage();
                        g2C_SevenDaysToRechargeMessage.Info = b_Component.dBPlayerShopMall.StrSevenRecharge;
                        b_Component.mPlayer.Send(g2C_SevenDaysToRechargeMessage);
                        b_Component.SetPlayerShopDB();
                        return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 当前元宝
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Value"></param>
        /// <param name="log">什么原因，详细，一定要详细</param>
        /// <returns></returns>
        public static bool SetPlayerYuanBao(this PlayerShopMallComponent b_Component, int Value, string log)
        {
            var gameplayer = b_Component.Parent.GetCustomComponent<GamePlayer>();

            //var mSourceValue = gameplayer.Player.Data.YuanbaoCoin;
            //gameplayer.Player.Data.YuanbaoCoin += Value;
            gameplayer.UpdateCoin(E_GameProperty.YuanbaoCoin, Value, log);

            //Log.PLog($"账号id {gameplayer.Player.UserId}  角色id:{gameplayer.Player.GameUserId} - PlayerShopMallComponent - 元宝收入: {mSourceValue} - {Value} - {gameplayer.Player.Data.YuanbaoCoin}");

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)b_Component.mPlayer.GameAreaId);
            mWriteDataComponent.Save(gameplayer.Player.Data, dBProxy).Coroutine();

            b_Component.SetPlayerShopDB();
            b_Component.SendPlayerShopState();
            return true;
        }
        public static bool HeBingMonthlyCard(this PlayerShopMallComponent b_Component,bool Modify = false, uint MonDuration = 2592000000)
        {
            bool Min = b_Component.SetPlayerShopState(DeviationType.MinMonthlyCard, Modify, MonDuration);
            if (Min)
            {
                Log.PLog($"购买合并后的特权卡设置小特权卡权限出错");
            }
            bool Max = b_Component.SetPlayerShopState(DeviationType.MaxMonthlyCard, Modify, MonDuration);
            if (Max)
            {
                Log.PLog($"购买合并后的特权卡设置大特权卡权限出错");
            }
            return Max && Min;
        }
        /// <summary>
        /// 修改充值状态
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="type"></param>
        /// <param name="Modify"></param>
        /// <returns></returns>
        public static bool SetPlayerShopState(this PlayerShopMallComponent b_Component, DeviationType type, bool Modify = false, uint MonDuration = 2592000000)
        {
            if (type < DeviationType.NoRecharge || type > DeviationType.MaxMonthlyCard) return false;

            if (type == DeviationType.MinMonthlyCard)
            {
                if (Modify)
                {
                    if ((b_Component.dBPlayerShopMall.RechargeStatus & (int)type) != (int)type)
                        b_Component.dBPlayerShopMall.RechargeStatus += (int)type;

                    if (Help_TimeHelper.GetNow() < b_Component.dBPlayerShopMall.MinMCEndTime)
                        b_Component.dBPlayerShopMall.MinMCEndTime += MonDuration;
                    else
                        b_Component.dBPlayerShopMall.MinMCEndTime = Help_TimeHelper.GetNow() + MonDuration;// 2592000000;
                    //b_Component.SetMonthlyCardStatus(true);
                }
                else
                {
                    if ((b_Component.dBPlayerShopMall.RechargeStatus & (int)type) == (int)type)
                        b_Component.dBPlayerShopMall.RechargeStatus -= (int)type;

                    b_Component.dBPlayerShopMall.MinMCEndTime = 0;
                    b_Component.dBPlayerShopMall.InSituCd = 0;
                    //b_Component.SetMonthlyCardStatus();
                }
            }
            else if (type == DeviationType.MaxMonthlyCard)
            {
                if (Modify)
                {
                    if ((b_Component.dBPlayerShopMall.RechargeStatus & (int)type) != (int)type)
                        b_Component.dBPlayerShopMall.RechargeStatus += (int)type;

                    if (Help_TimeHelper.GetNow() < b_Component.dBPlayerShopMall.MaxMCEndTime)
                        b_Component.dBPlayerShopMall.MaxMCEndTime += MonDuration;
                    else
                        b_Component.dBPlayerShopMall.MaxMCEndTime = Help_TimeHelper.GetNow() + MonDuration;// 2592000000;

                    b_Component.SetMonthlyCardStatus(true);
                }
                else
                {
                    if ((b_Component.dBPlayerShopMall.RechargeStatus & (int)type) == (int)type)
                        b_Component.dBPlayerShopMall.RechargeStatus -= (int)type;

                    b_Component.dBPlayerShopMall.MaxMCEndTime = 0;
                    b_Component.SetMonthlyCardStatus();
                }
            }
            else
            {
                b_Component.dBPlayerShopMall.RechargeStatus += (int)type;
            }

            b_Component.SetPlayerShopDB();
            return true;
        }
        public static bool CheckValue(this PlayerShopMallComponent b_Component, int Type, int Value)
        {
            if (b_Component == null) return false;
            if (b_Component.dBPlayerShopMall.RechargeRecordList == null) return false;
            if (b_Component.dBPlayerShopMall.RechargeRecordList.TryGetValue(Type, out int OldValue))
            {
                if (OldValue >= Value) return true;
            }
            return false;
        }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool GetPlayerShopState(this PlayerShopMallComponent b_Component, DeviationType type)
        {
            if (type == DeviationType.MinMonthlyCard)
            {
                var Title = b_Component.mPlayer.GetCustomComponent<PlayerTitle>();
                if (Title != null && !Title.CheckTitle(60012)) return true;
                if (b_Component.dBPlayerShopMall.MinMCEndTime != 0 && Help_TimeHelper.GetNow() >= b_Component.dBPlayerShopMall.MinMCEndTime)
                {
                    b_Component.SetPlayerShopState(DeviationType.MinMonthlyCard);
                }
            }
            else if (type == DeviationType.MaxMonthlyCard)
            {
                var Title = b_Component.mPlayer.GetCustomComponent<PlayerTitle>();
                if (Title != null && !Title.CheckTitle(60012)) return true;
                if (b_Component.dBPlayerShopMall.MaxMCEndTime != 0 && Help_TimeHelper.GetNow() >= b_Component.dBPlayerShopMall.MaxMCEndTime)
                {
                    b_Component.SetPlayerShopState(DeviationType.MaxMonthlyCard);
                }
            }

            return (b_Component.dBPlayerShopMall.RechargeStatus & (int)type) == (int)type;
        }
        /// <summary>
        /// 设置月卡的属性， AddorDle= false默认删除属性
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="AddorDle"></param>
        /// <returns></returns>
        public static bool SetMonthlyCardStatus(this PlayerShopMallComponent b_Component, bool AddorDle = false)
        {
            var equipComponent = b_Component.mPlayer.GetCustomComponent<EquipmentComponent>();
            if (equipComponent != null)
            {
                equipComponent.ApplyEquipProp();
            }
            /*if (AddorDle)
            {
                if ((b_Component.dBPlayerShopMall.RechargeStatus & (int)DeviationType.MaxMonthlyCard) == (int)DeviationType.MaxMonthlyCard)
                    return true;

                if (b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic.ContainsKey(E_GameProperty.ExperienceBonus))
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExperienceBonus] += 20;
                else
                {
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExperienceBonus] = 0;
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExperienceBonus] += 20;
                }

                if (b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic.ContainsKey(E_GameProperty.ExplosionRate))
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExplosionRate] += 20;
                else
                {
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExplosionRate] = 0;
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExplosionRate] += 20;
                }

                if (b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic.ContainsKey(E_GameProperty.GoldCoinMarkup))
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.GoldCoinMarkup] += 20;
                else
                {
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.GoldCoinMarkup] = 0;
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.GoldCoinMarkup] += 20;
                }
            }
            else
            {
                if (b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic.ContainsKey(E_GameProperty.ExperienceBonus))
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExperienceBonus] -= 20;
                else
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExperienceBonus] = 0;

                if (b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic.ContainsKey(E_GameProperty.ExplosionRate))
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExplosionRate] -= 20;
                else
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.ExplosionRate] = 0;

                if (b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic.ContainsKey(E_GameProperty.GoldCoinMarkup))
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.GoldCoinMarkup] -= 20;
                else
                    b_Component.Parent.GetCustomComponent<GamePlayer>().GamePropertyDic[E_GameProperty.GoldCoinMarkup] = 0;
            }*/
            return true;
        }
        /// <summary>
        /// Set奇迹币 
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool SetMiracleCoin(this PlayerShopMallComponent b_Component, int Value, string log)
        {
            var gameplayer = b_Component.Parent.GetCustomComponent<GamePlayer>();

            //var mSourceValue = gameplayer.Data.MiracleCoin;

            //gameplayer.Data.MiracleCoin += Value;
            gameplayer.UpdateCoin(E_GameProperty.MiracleCoin, Value, log);

            //Log.PLog($"账号id {gameplayer.Player.UserId}  角色id:{gameplayer.Player.GameUserId} - PlayerShopMallComponent - 奇迹币收入: {mSourceValue} - {Value} - {gameplayer.Data.MiracleCoin}");

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)b_Component.mPlayer.GameAreaId);
            mWriteDataComponent.Save(gameplayer.Data, dBProxy).Coroutine();

            //b_Component.SetPlayerShopDB();
            b_Component.SendPlayerShopState();
            return true;
        }

        public static void SendPlayerShopState(this PlayerShopMallComponent b_Component)
        {
            var gameplayer = b_Component.Parent.GetCustomComponent<GamePlayer>();
            G2C_PlayerShopInfoUpdataMessage g2C_PlayerShopInfoUpdataMessage = new G2C_PlayerShopInfoUpdataMessage();
            g2C_PlayerShopInfoUpdataMessage.CurrentYuanbao = gameplayer.Player.Data.YuanbaoCoin;
            g2C_PlayerShopInfoUpdataMessage.MiracleCoin = gameplayer.Data.MiracleCoin;
            g2C_PlayerShopInfoUpdataMessage.RechargeStatus = b_Component.dBPlayerShopMall.RechargeStatus;
            g2C_PlayerShopInfoUpdataMessage.AccumulatedRecharge = b_Component.dBPlayerShopMall.AccumulatedRecharge;
            g2C_PlayerShopInfoUpdataMessage.MaxMCEndTime = b_Component.dBPlayerShopMall.MaxMCEndTime;
            g2C_PlayerShopInfoUpdataMessage.MinMCEndTime = b_Component.dBPlayerShopMall.MinMCEndTime;
            g2C_PlayerShopInfoUpdataMessage.InSituCd = b_Component.dBPlayerShopMall.InSituCd;
            g2C_PlayerShopInfoUpdataMessage.RechargeRecord = b_Component.dBPlayerShopMall.RechargeRecord;

            b_Component.mPlayer.Send(g2C_PlayerShopInfoUpdataMessage);
        }
        public static void SetPlayerShopDB(this PlayerShopMallComponent b_Component)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.mPlayer.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)b_Component.mPlayer.GameAreaId);
            mWriteDataComponent.Save(b_Component.dBPlayerShopMall, dBProxy).Coroutine();
        }
        /// <summary>
        /// 首充发放物品
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        public static bool FirstReceiveItem(this PlayerShopMallComponent b_Component, DeviationType Type)
        {
            var mPlayer = b_Component.Parent;
            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            var Bk = mPlayer.GetCustomComponent<BackpackComponent>();
            bool AddItem(int ItemId, ItemCreateAttr CreateAttr)
            {
                Item mDropItem = ItemFactory.Create(ItemId, mPlayer.GameAreaId, CreateAttr);
                if (Bk.AddItem(mDropItem, "充值获取") == false)
                {
                    Log.PLog("ShopItem", $"角色:{mPlayer.GameUserId}道具ID:{ItemId}数量:{CreateAttr.Quantity} 领取失败 背包不足！！！");
                    return false;
                }
                else
                {
                    Log.PLog("ShopItem", $"角色:{mPlayer.GameUserId}道具ID:{ItemId}数量:{CreateAttr.Quantity} 领取成功");
                    return true;
                }
            }

            MailInfo mailinfo = new MailInfo();
            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
            mailinfo.MailName = "首充奖励";
            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
            mailinfo.MailContent = "首充奖励由于背包不足，通过邮件发放。";
            mailinfo.MailState = 0;
            mailinfo.ReceiveOrNot = 0;
            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;

            switch (Type)
            {
                case DeviationType.FirstCharge6:
                    {
                        b_Component.SetPlayerShopState(DeviationType.MaxMonthlyCard, true);
                        b_Component.SendPlayerShopState();
                        gameplayer.BuyVipSetDrop();
                        Log.PLog("PlayerShop", $"角色ID:{mPlayer.GameUserId} 名称:{gameplayer.Data.NickName} 领取首充10元奖励，月卡到期时间:{b_Component.dBPlayerShopMall.MaxMCEndTime}");
                        return true;
                    }
                    return false;
                case DeviationType.FirstCharge38:
                    {
                        ItemCreateAttr CreateAttr1 = new ItemCreateAttr();
                        CreateAttr1.Quantity = 1;
                        if (!AddItem(300001, CreateAttr1))
                        {
                            MailItem mailItem = new MailItem
                            {
                                ItemConfigID = 300001,
                                ItemID = 0,
                                CreateAttr = CreateAttr1
                            };
                            mailinfo.MailEnclosure.Add(mailItem);
                            MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mPlayer.GameAreaId).Coroutine();
                        }
                    }
                    return false;
                case DeviationType.FirstCharge68:
                    {
                        ItemCreateAttr CreateAttr1 = new ItemCreateAttr();
                        CreateAttr1.Quantity = 1;
                        if (!AddItem(300002, CreateAttr1))
                        {
                            MailItem mailItem = new MailItem
                            {
                                ItemConfigID = 300002,
                                ItemID = 0,
                                CreateAttr = CreateAttr1
                            };
                            mailinfo.MailEnclosure.Add(mailItem);
                            MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mPlayer.GameAreaId).Coroutine();
                        }
                    }
                    return false;
                case DeviationType.FirstCharge198:
                    {
                        int ItemID = 0;
                        ItemCreateAttr CreateAttr1 = new ItemCreateAttr
                        {
                            Quantity = 1,
                            Level = 7,
                        };
                        CreateAttr1.CustomAttrMethod.Add("ItemRandAddExcAttr_3");
                        switch ((E_GameOccupation)gameplayer.Data.PlayerTypeId)
                        {
                            case E_GameOccupation.Spell: ItemID = 80005; break;
                            case E_GameOccupation.Swordsman: ItemID = 10014; break;
                            case E_GameOccupation.Archer: ItemID = 50005; break;
                            case E_GameOccupation.Spellsword: ItemID = 80005; break;
                            case E_GameOccupation.Holyteacher: ItemID = 100000; break;
                            case E_GameOccupation.SummonWarlock: ItemID = 80028; break;
                            case E_GameOccupation.Combat: ItemID = 120001; break;
                            case E_GameOccupation.GrowLancer: ItemID = 70013; break;
                            default: break;
                        }
                        if (ItemID == 0) return false;

                        if (!AddItem(ItemID, CreateAttr1))
                        {
                            MailItem mailItem = new MailItem
                            {
                                ItemConfigID = ItemID,
                                ItemID = 0,
                                CreateAttr = CreateAttr1
                            };
                            mailinfo.MailEnclosure.Add(mailItem);
                            MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mPlayer.GameAreaId).Coroutine();
                        }
                    }
                    return false;
                case DeviationType.FirstCharge288:
                    {
                        ItemCreateAttr CreateAttr1 = new ItemCreateAttr();
                        CreateAttr1.Quantity = 1;
                        int ItemID = 0;
                        switch ((E_GameOccupation)gameplayer.Data.PlayerTypeId)
                        {
                            case E_GameOccupation.Combat:
                            case E_GameOccupation.Spell: ItemID = 220059; break;
                            case E_GameOccupation.Spellsword:
                            case E_GameOccupation.Swordsman: ItemID = 220058; break;
                            case E_GameOccupation.Archer: ItemID = 220060; break;
                            case E_GameOccupation.SummonWarlock:
                            case E_GameOccupation.GrowLancer: ItemID = 220057; break;
                            case E_GameOccupation.Holyteacher: ItemID = 220056; break;
                            default: break;
                        }
                        if (ItemID == 0) return false;

                        if (!AddItem(ItemID, CreateAttr1))
                        {
                            MailItem mailItem = new MailItem { ItemConfigID = ItemID, ItemID = 0, CreateAttr = CreateAttr1 };
                            mailinfo.MailEnclosure.Add(mailItem);
                            MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mPlayer.GameAreaId).Coroutine();
                        }
                    }
                    return false;
                default:
                    break;
            }
            return false;
        }
        public static bool NationalDayClaim(this PlayerShopMallComponent b_Component, int Day)
        {
            var mPlayer = b_Component.Parent;

            if (b_Component == null) return false;
            if (b_Component.dBPlayerShopMall.SevenRecharge == null) return false;

            foreach (var Info in b_Component.dBPlayerShopMall.SevenRecharge)
            {
                (string, int) DayInfo = Info.Key;
                if (DayInfo.Item2 == Day)
                {
                    if (!Info.Value.Item2)
                    {
                        var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<SevenDay_RewardPropsConfigJson>().JsonDic;
                        if (JsonDic != null)
                        {
                            if (JsonDic[Day] != null && JsonDic.ContainsKey(Day))
                            {
                                b_Component.SetMiracleCoin(JsonDic[Day].MagicCrystal, "国庆签到奖励");
                                //gameplayer.UpdateCoin(E_GameProperty.MiracleCoin, JsonDic[Day].MagicCrystal, "国庆签到奖励");
                                b_Component.SetSevenReceiveState(Day);
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static bool SevenDayReceiveItem(this PlayerShopMallComponent b_Component, int Day)
        {
            var mPlayer = b_Component.Parent;
            var gameplayer = mPlayer.GetCustomComponent<GamePlayer>();
            var Bk = mPlayer.GetCustomComponent<BackpackComponent>();
            if (b_Component.SevenReceive(Day))
            {
                var JsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<SevenDay_RewardPropsConfigJson>().JsonDic;
                if (JsonDic != null && JsonDic.ContainsKey(Day))
                {
                    ItemCreateAttr CreateAttr1 = new ItemCreateAttr();
                    CreateAttr1.Quantity = 1;
                    CreateAttr1.IsBind = 1;
                    int ItemId = 0;//JsonDic[Day].ItemID;
                    Item mDropItem = ItemFactory.Create(ItemId, mPlayer.GameAreaId, CreateAttr1);
                    if (!Bk.AddItem(mDropItem, "充值获取"))
                    {
                        Log.PLog("ShopItem", $"角色:{mPlayer.GameUserId}道具ID:{ItemId}数量:{CreateAttr1.Quantity} 领取失败 背包不足！！！");
                        MailInfo mailinfo = new MailInfo();
                        mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                        mailinfo.MailName = "七天充值奖励";
                        mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                        mailinfo.MailContent = "七天充值奖励由于背包不足，通过邮件发放。";
                        mailinfo.MailState = 0;
                        mailinfo.ReceiveOrNot = 0;
                        mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                        MailItem mailItem = new MailItem
                        {
                            ItemConfigID = ItemId,
                            ItemID = 0,
                            CreateAttr = CreateAttr1
                        };
                        mailinfo.MailEnclosure.Add(mailItem);
                        MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mPlayer.GameAreaId).Coroutine();
                    }
                    else
                    {
                        Log.PLog("ShopItem", $"角色:{mPlayer.GameUserId}道具ID:{ItemId}数量:{CreateAttr1.Quantity} 领取成功");
                    }
                    b_Component.SetSevenReceiveState(Day);
                    return true;
                }
            }
            return false;
        }

        private static async Task CreatePayOrderHy(this PlayerShopMallComponent b_Component, int money, DBAccountInfo dbLoginInfo)
        {
            Player player = b_Component.Parent;

            DBPlayerPayOrderInfo mPlayerPayOrderInfo = new DBPlayerPayOrderInfo();
            mPlayerPayOrderInfo.Id = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
            mPlayerPayOrderInfo.App_Ordef_id = mPlayerPayOrderInfo.Id;
            mPlayerPayOrderInfo.GUid = player.UserId;
            mPlayerPayOrderInfo.Rid = player.GameUserId;
            mPlayerPayOrderInfo.Money = money;
            mPlayerPayOrderInfo.Time = Help_TimeHelper.GetNowSecond();
            mPlayerPayOrderInfo.RName = player.GetCustomComponent<GamePlayer>().Data.NickName;
            mPlayerPayOrderInfo.Effective = true;
            mPlayerPayOrderInfo.ChannelId = dbLoginInfo.ChannelId;
            mPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
            mPlayerPayOrderInfo.Success = true;
            mPlayerPayOrderInfo.TradePlatform = TradePlatform.Hy;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, player.GameAreaId);
            await dBProxy2.Save(mPlayerPayOrderInfo);

            //GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
            //gM2C_PayReturnMessage.MAreaId = player.GameAreaId;
            //gM2C_PayReturnMessage.AppOrderId = mPlayerPayOrderInfo.Id;
            //gM2C_PayReturnMessage.GameUserID = player.GameUserId;
            //gM2C_PayReturnMessage.LogInfo = "好易第三方充值";
            //var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, OptionComponent.Options.AppId);
            //Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
            //IResponse response2 = await gameSession.Call(gM2C_PayReturnMessage);
            //C2GM_PayReturnResponse game2gm_SendMail = response2 as C2GM_PayReturnResponse;
            //if (game2gm_SendMail == null)
            //{
            //    Log.Debug($"在线充值返回失败 角色:{player.GameUserId}");
            //    return false;
            //}
            //return true;
        }

        public static async Task<bool> ThreePayInfo(this PlayerShopMallComponent b_Component, int money, DBAccountInfo dbLoginInfo)
        {
            Player player = b_Component.Parent;
            GamePlayer gamePlayer = player.GetCustomComponent<GamePlayer>();
            int mAreaId = player.GameAreaId;
            //C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(player.SourceGameAreaId);
            //if (mServerArea == null)
            //{
            //    Log.Debug($"充值到账加载异常UserId:{player.UserId}");
            //}
            // 创建完成订单，用于记录和统计
           await b_Component.CreatePayOrderHy(money, dbLoginInfo);

            int oldHyPaytotal = player.Data.HyPaytotal;
            player.Data.HyPaytotal += money;//第三方才有值，其他默认为零
            player.PLog($"HyPaytotal 变动 {player.Data.HyPaytotal - oldHyPaytotal} {oldHyPaytotal} => {player.Data.HyPaytotal}");

            b_Component.SetAccumulatedRecharge(money, $"游戏第三方充值");
            if (b_Component.GetPlayerShopState(DeviationType.NoRecharge))
            {
                b_Component.dBPlayerShopMall.RechargeStatus -= 1;
            }
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)player.GameAreaId);
            mWriteDataComponent.Save(player.Data, dBProxy).Coroutine();
            return true;
        }
        public static async Task<bool> GMPayInfo(this PlayerShopMallComponent b_Component, int money)
        {
            // 创建完成订单，用于记录和统计
            DBPlayerPayOrderInfo payOrderInfo = await b_Component.CreatePayOrderGM(money);

            // 充值，订单处理，调用统一接口
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Component.Parent.SourceGameAreaId);
            if (mServerArea == null)
            {
                Log.Debug($"充值未到账登录加载时异常UserId:{b_Component.Parent.UserId}");
            }
            var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, OptionComponent.Options.AppId);
            Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
            GM2C_PayReturnRequest gM2C_PayReturnMessage = new GM2C_PayReturnRequest();
            gM2C_PayReturnMessage.MAreaId = b_Component.Parent.SourceGameAreaId;
            gM2C_PayReturnMessage.AppOrderId = payOrderInfo.App_Ordef_id;
            gM2C_PayReturnMessage.GameUserID = b_Component.Parent.GameUserId;
            gM2C_PayReturnMessage.LogInfo = "GM 道具充值";
            gameSession.Call(gM2C_PayReturnMessage).Coroutine();
            return true;
        }
        private static async Task<DBPlayerPayOrderInfo> CreatePayOrderGM(this PlayerShopMallComponent b_Component, int money)
        {
            Player player = b_Component.Parent;

            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, DBProxyComponent.CommonDBId);
            DBAccountInfo dbLoginInfo = null;
            if (mDBProxy != null)
            {
                var list = await mDBProxy.Query<DBAccountInfo>(p => p.Id == player.UserId);
                if (list.Count > 0)
                {
                    dbLoginInfo = list[0] as DBAccountInfo;
                }
            }
            if (dbLoginInfo == null)
            {
                Log.Error($"a:{player.UserId} r:{player.GameUserId} GM充值创建订单失败，money={money}");
            }

            DBPlayerPayOrderInfo mPlayerPayOrderInfo = new DBPlayerPayOrderInfo();
            mPlayerPayOrderInfo.Id = IdGeneraterNew.Instance.GenerateUnitId(player.GameAreaId);
            mPlayerPayOrderInfo.App_Ordef_id = mPlayerPayOrderInfo.Id;
            mPlayerPayOrderInfo.GUid = player.UserId;
            mPlayerPayOrderInfo.Rid = player.GameUserId;
            mPlayerPayOrderInfo.Money = money;
            mPlayerPayOrderInfo.Time = Help_TimeHelper.GetNowSecond();
            mPlayerPayOrderInfo.RName = player.GetCustomComponent<GamePlayer>().Data.NickName;
            mPlayerPayOrderInfo.Effective = true;
            mPlayerPayOrderInfo.ChannelId = dbLoginInfo.ChannelId;
            mPlayerPayOrderInfo.SuccessTime = Help_TimeHelper.GetNowSecond();
            mPlayerPayOrderInfo.Success = false;
            mPlayerPayOrderInfo.TradePlatform = TradePlatform.GM;
            mPlayerPayOrderInfo.Product_id = (int)PlayerShopQuotaType.SpecifiedAmount;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, player.GameAreaId);
            await dBProxy2.Save(mPlayerPayOrderInfo);

            return mPlayerPayOrderInfo;
        }
    }
}
