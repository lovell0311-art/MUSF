using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Bri.V20190328.Models;


namespace ETHotfix
{
    public static class MailSystem
    {
        /// <summary>
        /// 指定发送给某个角色
        /// </summary>
        /// <param name="mailComponent"></param>
        /// <param name="UID"></param>
        /// <param name="mailInfo"></param>
        public static async Task SendMail(long UID, MailInfo mailInfo, int mAreaId,string itemLog = null)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
            DBMailData dBMailData = new DBMailData();
            dBMailData.Id = mailInfo.MailId;
            dBMailData.MaliID = mailInfo.MailId;
            dBMailData.MailName = mailInfo.MailName;
            dBMailData.MailAcceptanceTime = 0;
            dBMailData.MailContent = mailInfo.MailContent;
            dBMailData.MailValidTime = mailInfo.MailValidTime;
            dBMailData.MailState = mailInfo.MailState;
            dBMailData.ItemLog = itemLog;
            dBMailData.ReceiveOrNot = mailInfo.ReceiveOrNot;
            dBMailData.GameUserID = UID;
            dBMailData.MailEnclosure = Help_JsonSerializeHelper.Serialize(mailInfo.MailEnclosure,true);


            Player mBePlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, UID);
            if (mBePlayer != null)
            {
                dBMailData.MailAcceptanceTime = mailInfo.MailAcceptanceTime;
                G2C_ServerSendMailMessage g2C_ServerSendMailMessage = new G2C_ServerSendMailMessage();
                //g2C_ServerSendMailMessage.Mail = new Mailinfo();
                //g2C_ServerSendMailMessage.Mail.MailId = mailInfo.MailId;
                //g2C_ServerSendMailMessage.Mail.MailName = mailInfo.MailName;
                //g2C_ServerSendMailMessage.Mail.MailValidTime = mailInfo.MailValidTime;
                //g2C_ServerSendMailMessage.Mail.MailAcceptanceTime = mailInfo.MailAcceptanceTime;
                //g2C_ServerSendMailMessage.Mail.MailContent = mailInfo.MailContent;
                //g2C_ServerSendMailMessage.Mail.MailState = mailInfo.MailState;
                //g2C_ServerSendMailMessage.Mail.ReceiveOrNot = mailInfo.ReceiveOrNot;
                //for (int i = 0; i < mailInfo.MailEnclosure.Count; i++)
                //{
                //    Iteminfo iteminfo = new Iteminfo();
                //    iteminfo.ItemConfigID = mailInfo.MailEnclosure[i].ItemConfigID;
                //    iteminfo.ItemID = mailInfo.MailEnclosure[i].ItemID;
                //    iteminfo.ItemCnt = mailInfo.MailEnclosure[i].CreateAttr.Quantity;
                //    g2C_ServerSendMailMessage.Mail.MailEnclosure.Add(iteminfo);
                //}

                var PlayerMail = mBePlayer.GetCustomComponent<PlayerMailComponent>();
                if (PlayerMail == null)
                {
                    PlayerMail = mBePlayer.AddCustomComponent<PlayerMailComponent>();
                }
                PlayerMail.mailInfos.Add(mailInfo.MailId, mailInfo);
                DataCacheManageComponent mDataCacheManageComponent = mBePlayer.GetCustomComponent<DataCacheManageComponent>();
                var mDataCache = mDataCacheManageComponent.Get<DBMailData>();
                if (mDataCache == null)
                {
                    mDataCache = await mDataCacheManageComponent.Add<DBMailData>(dBProxy2, p => p.GameUserID == mBePlayer.GameUserId);
                }
                mDataCache.DataAdd(dBMailData);
                mBePlayer.Send(g2C_ServerSendMailMessage);
            }
            await dBProxy2.Save(dBMailData);
        }

        public static async Task SendGroupMail(List<long> UID, MailInfo mailInfo, int mAreaId)
        {
            foreach (long UID2 in UID)
            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);
                DBMailData dBMailData = new DBMailData();
                dBMailData.Id = mailInfo.MailId;
                dBMailData.MaliID = mailInfo.MailId;
                dBMailData.MailName = mailInfo.MailName;
                dBMailData.MailAcceptanceTime = 0;
                dBMailData.MailContent = mailInfo.MailContent;
                dBMailData.MailValidTime = mailInfo.MailValidTime;
                dBMailData.MailState = mailInfo.MailState;
                dBMailData.ReceiveOrNot = mailInfo.ReceiveOrNot;
                dBMailData.GameUserID = UID2;
                dBMailData.MailEnclosure = Help_JsonSerializeHelper.Serialize(mailInfo.MailEnclosure,true);


                Player mBePlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, UID2);
                if (mBePlayer != null)
                {
                    dBMailData.MailAcceptanceTime = mailInfo.MailAcceptanceTime;
                    G2C_ServerSendMailMessage g2C_ServerSendMailMessage = new G2C_ServerSendMailMessage();
                    //g2C_ServerSendMailMessage.Mail = new Mailinfo();
                    //g2C_ServerSendMailMessage.Mail.MailId = mailInfo.MailId;
                    //g2C_ServerSendMailMessage.Mail.MailName = mailInfo.MailName;
                    //g2C_ServerSendMailMessage.Mail.MailValidTime = mailInfo.MailValidTime;
                    //g2C_ServerSendMailMessage.Mail.MailAcceptanceTime = mailInfo.MailAcceptanceTime;
                    //g2C_ServerSendMailMessage.Mail.MailContent = mailInfo.MailContent;
                    //g2C_ServerSendMailMessage.Mail.MailState = mailInfo.MailState;
                    //g2C_ServerSendMailMessage.Mail.ReceiveOrNot = mailInfo.ReceiveOrNot;
                    //for (int i = 0; i < mailInfo.MailEnclosure.Count; i++)
                    //{
                    //    Iteminfo iteminfo = new Iteminfo();
                    //    iteminfo.ItemConfigID = mailInfo.MailEnclosure[i].ItemConfigID;
                    //    iteminfo.ItemID = mailInfo.MailEnclosure[i].ItemID;
                    //    iteminfo.ItemCnt = mailInfo.MailEnclosure[i].CreateAttr.Quantity;
                    //    g2C_ServerSendMailMessage.Mail.MailEnclosure.Add(iteminfo);
                    //}

                    var PlayerMail = mBePlayer.GetCustomComponent<PlayerMailComponent>();
                    if (PlayerMail == null)
                    {
                        PlayerMail = mBePlayer.AddCustomComponent<PlayerMailComponent>();
                    }
                    PlayerMail.mailInfos.Add(mailInfo.MailId, mailInfo);
                    DataCacheManageComponent mDataCacheManageComponent = mBePlayer.GetCustomComponent<DataCacheManageComponent>();
                    var mDataCache = mDataCacheManageComponent.Get<DBMailData>();
                    if (mDataCache == null)
                    {
                        mDataCache = await mDataCacheManageComponent.Add<DBMailData>(dBProxy2, p => p.GameUserID == mBePlayer.GameUserId);
                    }
                    mDataCache.DataAdd(dBMailData);
                    mBePlayer.Send(g2C_ServerSendMailMessage);
                }
                await dBProxy2.Save(dBMailData);
            }
        }

        public static async Task SendFullServiceMail(int mAreaId, MailInfo mailInfo)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mAreaId);

            //DBMailData dBMailData = new DBMailData();
            //dBMailData.Id = IdGeneraterNew.Instance.GenerateUnitId(mAreaId);
            //dBMailData.MaliID = mailInfo.MailId;
            //dBMailData.MailName = mailInfo.MailName;
            //dBMailData.MailAcceptanceTime = mailInfo.MailAcceptanceTime;
            //dBMailData.MailContent = mailInfo.MailContent;
            //dBMailData.MailValidTime = mailInfo.MailValidTime;
            //dBMailData.MailState = mailInfo.MailState;
            //dBMailData.ReceiveOrNot = mailInfo.ReceiveOrNot;
            //dBMailData.MailEnclosure = Help_JsonSerializeHelper.Serialize(mailInfo.MailEnclosure,true);

            var PlayerList = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllByZone(mAreaId);//await dBProxy.Query<DBGamePlayerData>(p => p.IsDisposePlayer != 1);
            if(PlayerList == null) return;

            foreach (var player in PlayerList)
            {
                DBMailData dBMailData = new DBMailData();
                dBMailData.Id = IdGeneraterNew.Instance.GenerateUnitId(mAreaId);
                dBMailData.MaliID = mailInfo.MailId;
                dBMailData.MailName = mailInfo.MailName;
                dBMailData.MailAcceptanceTime = mailInfo.MailAcceptanceTime;
                dBMailData.MailContent = mailInfo.MailContent;
                dBMailData.MailValidTime = mailInfo.MailValidTime;
                dBMailData.MailState = mailInfo.MailState;
                dBMailData.ReceiveOrNot = mailInfo.ReceiveOrNot;
                dBMailData.MailEnclosure = Help_JsonSerializeHelper.Serialize(mailInfo.MailEnclosure, true);
                dBMailData.Id = IdGeneraterNew.Instance.GenerateUnitId(mAreaId);
                dBMailData.GameUserID = player.Value.GameUserId;

                if (player.Value != null)
                {
                    G2C_ServerSendMailMessage g2C_ServerSendMailMessage = new G2C_ServerSendMailMessage();
                    //g2C_ServerSendMailMessage.Mail = new Mailinfo();
                    //g2C_ServerSendMailMessage.Mail.MailId = mailInfo.MailId;
                    //g2C_ServerSendMailMessage.Mail.MailName = mailInfo.MailName;
                    //g2C_ServerSendMailMessage.Mail.MailValidTime = mailInfo.MailValidTime;
                    //g2C_ServerSendMailMessage.Mail.MailAcceptanceTime = mailInfo.MailAcceptanceTime;
                    //g2C_ServerSendMailMessage.Mail.MailContent = mailInfo.MailContent;
                    //g2C_ServerSendMailMessage.Mail.MailState = mailInfo.MailState;
                    //g2C_ServerSendMailMessage.Mail.ReceiveOrNot = mailInfo.ReceiveOrNot;
                    //for (int i = 0; i < mailInfo.MailEnclosure.Count; i++)
                    //{
                    //    Iteminfo iteminfo = new Iteminfo();
                    //    iteminfo.ItemConfigID = mailInfo.MailEnclosure[i].ItemConfigID;
                    //    iteminfo.ItemID = mailInfo.MailEnclosure[i].ItemID;
                    //    iteminfo.ItemCnt = mailInfo.MailEnclosure[i].CreateAttr.Quantity;
                    //    g2C_ServerSendMailMessage.Mail.MailEnclosure.Add(iteminfo);
                    //}

                    var PlayerMail = player.Value.GetCustomComponent<PlayerMailComponent>();
                    if (PlayerMail == null)
                    {
                        PlayerMail = player.Value.AddCustomComponent<PlayerMailComponent>();
                    }
                    MailInfo mailInfo1 = mailInfo.CreatMailInfo();
                    PlayerMail.mailInfos.Add(mailInfo1.MailId, mailInfo1);
                    DataCacheManageComponent mDataCacheManageComponent = player.Value.GetCustomComponent<DataCacheManageComponent>();
                    var mDataCache = mDataCacheManageComponent.Get<DBMailData>();
                    if (mDataCache == null)
                    {
                        mDataCache = await mDataCacheManageComponent.Add<DBMailData>(dBProxy, p => p.GameUserID == player.Value.GameUserId);
                    }
                    mDataCache.DataAdd(dBMailData);
                    dBProxy.Save(dBMailData).Coroutine();
                    player.Value.Send(g2C_ServerSendMailMessage);
                    player.Value.GetCustomComponent<GamePlayer>().Data.NewServerMailTime = mailInfo.MailAcceptanceTime;
                    //async Task SetDBData()
                    //{
                        await dBProxy.Save(player.Value.GetCustomComponent<GamePlayer>().Data);
                    //}
                    //await SetDBData();
                }
            }
        }
    }
}