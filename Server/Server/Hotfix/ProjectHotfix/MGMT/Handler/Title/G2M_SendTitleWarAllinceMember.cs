using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Bm.V20180423.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_SendTitleWarAllinceMemberHandler : AMHandler<G2M_SendTitleWarAllinceMember>
    {
        protected override async Task<bool> Run(G2M_SendTitleWarAllinceMember b_Request)
        {
            int mAreaId =(int) b_Request.AppendData;
            var Waralliancecomponent = Root.MainFactory.GetCustomComponent<WarAllianceComponent>();
            if (Waralliancecomponent == null)
            {
                return false;
            }

            var warAllianceMmber = Waralliancecomponent.GetWarAlliance(b_Request.WarAllianceID);
            if (warAllianceMmber == null)
            {
                return false;
            }
            async Task SetTitle(DBPlayerTitle dBPlayerTitle)
            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
                if (dBProxy2 != null)
                {
                    var Title = await dBProxy2.Query<DBPlayerTitle>(P => P.TitleID == dBPlayerTitle.TitleID && P.UserId == dBPlayerTitle.UserId);
                    if (Title != null && Title.Count > 0)
                    {
                        DBPlayerTitle dBPlayerTitle1 = Title[0] as DBPlayerTitle;
                        dBPlayerTitle.Id = dBPlayerTitle1.Id;
                        await dBProxy2.Save(dBPlayerTitle);
                    }
                    else
                    {
                        await dBProxy2.Save(dBPlayerTitle);
                    }
                }
            }
            //List<int> list = Help_JsonSerializeHelper.DeSerialize<List<int>>(OptionComponent.Options.RunParameter);
            var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
            
            var MemberList = warAllianceMmber.MemberList;
            DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 19, 30, 0);
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            foreach (var MemberID in MemberList)
            {
                DBPlayerTitle dBPlayerTitle1 = new DBPlayerTitle();
                dBPlayerTitle1.Id = IdGeneraterNew.Instance.GenerateUnitId(OptionComponent.Options.ZoneId);
                dBPlayerTitle1.UserId = MemberID.Value.MemberID;
                dBPlayerTitle1.Type = 0;
                dBPlayerTitle1.UseID = 0;
                dBPlayerTitle1.BingTime = Help_TimeHelper.DateConversionTime(time);
                dBPlayerTitle1.EndTime = dBPlayerTitle1.BingTime + 604799;
                dBPlayerTitle1.IsDisabled = 0;

                M2G_SettlementRankingMessage m2G_SettlementRankingMessage = new M2G_SettlementRankingMessage();
                if (MemberID.Value.MemberPost == 3)
                {
                    dBPlayerTitle1.TitleID = 60001;
                    m2G_SettlementRankingMessage.Value32A = 60001;
                    m2G_SettlementRankingMessage.Value32B = 1;
                }
                else
                {
                    dBPlayerTitle1.TitleID = 60002;
                    m2G_SettlementRankingMessage.Value32A = 60002;
                    m2G_SettlementRankingMessage.Value32B = 0;
                }
                m2G_SettlementRankingMessage.Value64A = MemberID.Value.MemberID;
                m2G_SettlementRankingMessage.RankType =(int)RankType.SiegeWarfare;
                m2G_SettlementRankingMessage.StrA = MemberID.Value.MemberName;
                m2G_SettlementRankingMessage.Value64B = 0;

                //攻城战邮件
                {
                    MailItem mailItem = new MailItem();
                    ItemCreateAttr itemCreateAttr = new ItemCreateAttr();
                    itemCreateAttr.Quantity = 1;
                    itemCreateAttr.CustomAttrMethod.Add("ItemRandAddExcAttr_3_6");
                    itemCreateAttr.ValidTime = 7 * 24 * 60 * 60;
                    itemCreateAttr.IsBind = 2;
                    mailItem.ItemConfigID = 340003;
                    mailItem.ItemID = 0;
                    mailItem.CreateAttr = itemCreateAttr;
                    var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, OptionComponent.Options.ZoneId);
                    DBMailData dBMailData = new DBMailData();
                    dBMailData.Id = IdGeneraterNew.Instance.GenerateUnitId(OptionComponent.Options.ZoneId);
                    dBMailData.MaliID = IdGeneraterNew.Instance.GenerateUnitId(OptionComponent.Options.ZoneId);
                    dBMailData.MailName = "攻城战胜利奖励";
                    dBMailData.MailAcceptanceTime = 0;
                    dBMailData.MailContent = "恭喜玩家获得战胜方称号以及7天冰封旗帜，奖励已发放请在属性页或者背包查看。";
                    dBMailData.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                    dBMailData.MailState = 0;
                    dBMailData.ReceiveOrNot = 0;
                    dBMailData.GameUserID = MemberID.Value.MemberID;
                    List<MailItem> mailItems = new List<MailItem>() { mailItem };
                    dBMailData.MailEnclosure = Help_JsonSerializeHelper.Serialize(mailItems,true);
                    await dBProxy2.Save(dBMailData);
                    //MailSystem.SendMail(GameUserId, mailinfo, mAreaId).Coroutine();
                }
                await SetTitle(dBPlayerTitle1);
                foreach (var Info in mMatchConfigs)
                {
                    if (Info.ZoneId == OptionComponent.Options.ZoneId)
                        Game.Scene.GetComponent<NetInnerComponent>().Get(Info.ServerInnerIP).Send(m2G_SettlementRankingMessage);
                }
            }
            return true;
        }
    }
}