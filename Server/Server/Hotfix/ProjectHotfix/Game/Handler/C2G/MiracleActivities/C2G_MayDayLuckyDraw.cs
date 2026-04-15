using Aop.Api.Domain;
using CommandLine;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Ame.V20190916.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_MayDayLuckyDrawHandler : AMActorRpcHandler<C2G_MayDayLuckyDraw, G2C_MayDayLuckyDraw>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_MayDayLuckyDraw b_Request, G2C_MayDayLuckyDraw b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_MayDayLuckyDraw b_Request, G2C_MayDayLuckyDraw b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
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
            var ActivitiesInfo = mServerArea.GetCustomComponent<ActivitiesComponent>();
            if (!ActivitiesInfo.GetActivitState(14))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2400);
                b_Reply(b_Response);
                return false;
            }
            /*每日抽奖
            var PlayerActivitiesInfo = mPlayer.GetCustomComponent<PlayerActivitComponent>();
            var PlayerActivity = PlayerActivitiesInfo?.GetMiracleActivities(14);
            if (PlayerActivity != null)
            {
                long Time = Help_TimeHelper.GetNowSecond();
                DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                dateTime = dateTime.ToUniversalTime(); 
                //秒
                long timestamp = (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

                if (PlayerActivity.Value64A <= Time)
                {
                    PlayerActivity.Value64A = timestamp + 86400;

                    var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<GoldLottery_ItemInfoConfigJson>().JsonDic;
                    RandomSelector<int> randomItem = new RandomSelector<int>();
                    foreach (var info in mJsonDic)
                    {
                        randomItem.Add(info.Value.Id, info.Value.Weight);
                    }
                    randomItem.TryGetValue(out int configId);
                    if (mJsonDic.TryGetValue(configId, out var ItemInfo))
                    {
                        ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                        itemCreateAttr.Level = mJsonDic[configId].Level;
                        itemCreateAttr.Quantity = mJsonDic[configId].Quantity;
                        itemCreateAttr.HaveSkill = mJsonDic[configId].HasSkill != 0;
                        itemCreateAttr.HaveLucky = mJsonDic[configId].HasLucky != 0;
                        itemCreateAttr.IsBind = mJsonDic[configId].IsBind;
                        if (!string.IsNullOrEmpty(mJsonDic[configId].CustomAttrMathod))
                            itemCreateAttr.CustomAttrMethod.AddRange(mJsonDic[configId].CustomAttrMathod.Split(','));

                        MailInfo mailinfo = new MailInfo();
                        mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                        mailinfo.MailName = "五一抽奖";
                        mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                        mailinfo.MailContent = "恭喜玩家获得奖励物品";
                        mailinfo.MailState = 0;
                        mailinfo.ReceiveOrNot = 0;
                        mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                        MailItem mailItem = new MailItem();
                        mailItem.ItemConfigID = mJsonDic[configId].ItemId;
                        mailItem.ItemID = 0;
                        mailItem.CreateAttr = itemCreateAttr;
                        mailinfo.MailEnclosure.Add(mailItem);
                        MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                    }
                    PlayerActivity.Value32A = 1;
                    PlayerActivitiesInfo.SetPlayerActivit(14, PlayerActivity);
                    PlayerActivitiesInfo.DBPlayerActivit(14);

                    b_Response.Config = configId;
                }
                else
                    b_Response.Error = 911;
               
            } */
            //道具抽奖
            {
                var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<GoldLottery_ItemInfoConfigJson>().JsonDic;
                if (mJsonDic.TryGetValue(200,out var Info))//200在配置表中是需要消耗的道具
                {
                    var knapsack = mPlayer.GetCustomComponent<BackpackComponent>();
                    var ItemLists = knapsack.GetAllItemByConfigID(Info.ItemId);
                    if (ItemLists != null && ItemLists.Count > 0)
                    {
                        RandomSelector<int> randomItem = new RandomSelector<int>();
                        foreach (var info in mJsonDic)
                        {
                            if (info.Value.Id == 200) continue;
                            randomItem.Add(info.Value.Id, info.Value.Weight);
                        }
                        randomItem.TryGetValue(out int Id);
                        //100在配置表中默认谢谢参与不产出道具
                        if (mJsonDic.TryGetValue(Id, out var ItemInfo) && Id != 100)
                        {
                            ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                            itemCreateAttr.Level = ItemInfo.Level;
                            itemCreateAttr.Quantity = ItemInfo.Quantity;
                            itemCreateAttr.HaveSkill = ItemInfo.HasSkill != 0;
                            itemCreateAttr.HaveLucky = ItemInfo.HasLucky != 0;
                            if (!string.IsNullOrEmpty(ItemInfo.CustomAttrMathod))
                                itemCreateAttr.CustomAttrMethod.AddRange(ItemInfo.CustomAttrMathod.Split(','));

                            MailInfo mailinfo = new MailInfo();
                            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                            mailinfo.MailName = "每日抽奖";
                            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                            mailinfo.MailContent = "恭喜玩家获得奖励物品";
                            mailinfo.MailState = 0;
                            mailinfo.ReceiveOrNot = 0;
                            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                            MailItem mailItem = new MailItem();
                            mailItem.ItemConfigID = ItemInfo.ItemId;
                            mailItem.ItemID = 0;
                            mailItem.CreateAttr = itemCreateAttr;
                            mailinfo.MailEnclosure.Add(mailItem);
                            MailSystem.SendMail(mPlayer.GameUserId, mailinfo, mServerArea.GameAreaId).Coroutine();
                        }

                        b_Response.Config = Id;
                        knapsack.UseItem(ItemLists.First().Value, "道具抽奖消耗");
                    }
                    else
                        b_Response.Error = 1609;
                }
            }
            b_Reply(b_Response);
            return true;
        }
    }
}