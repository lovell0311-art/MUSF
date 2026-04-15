using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using ETModel.HttpProto;
using CustomFrameWork;
using CustomFrameWork.Component;
using System.Net;
using System.Threading.Tasks;
using MongoDB.Bson;
using System.Linq;

namespace ETHotfix
{
	[HttpHandler(AppType.GM, "/api/player/")]
    public class Http_GM_Player : AHttpHandler
    {

        // 查询玩家在线状态
        // UserId
        [Post]  // Url-> /api/player/GetLoginRecord
        public async Task<HttpResult> GetLoginRecord(HttpListenerRequest req, GetLoginRecordParam param)
        {
            if(param == null) return Error(msg: "参数错误");

            if (!long.TryParse(param.UserId, out long userId))
            {
                return Error(msg: "参数错误2");
            }

            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                l2sGetLoginRecord = (LoginCenter2S_GetLoginRecord)await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = userId
                });
            }
            catch(Exception e)
            {
                return Error(msg: e.ToString());
            }

            return Ok(data: new
            {
                Online = l2sGetLoginRecord.UserId == userId,
                GateServerId = l2sGetLoginRecord.GateServerId,
                GameUserId = l2sGetLoginRecord.GameUserId.ToString(),
                GameServerId = l2sGetLoginRecord.GameServerId
            });
        }

        [Post]  // Url-> /api/player/UpdateAccountIdentity
        public async Task<HttpResult> UpdateAccountIdentity(HttpListenerRequest req, UpdateAccountIdentityParam param)
        {
            if (param == null) return Error(msg: "参数错误");

            if (!long.TryParse(param.UserId, out long userId))
            {
                return Error(msg: "参数错误2");
            }

            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                l2sGetLoginRecord = (LoginCenter2S_GetLoginRecord)await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = userId
                });
            }
            catch (Exception e)
            {
                return Error(msg: e.ToString());
            }

            if (l2sGetLoginRecord.GameUserId != 0)
            {
                // 账号在线
                // 告诉服务器更新AccountIdentity
                GM2Game_UpdateAccountIdentity gM2Game_UpdateAccountIdentity = new GM2Game_UpdateAccountIdentity()
                {
                    GameUserId = l2sGetLoginRecord.GameUserId
                };
                var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, l2sGetLoginRecord.GameServerId);
                Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
                await gameSession.Call(gM2Game_UpdateAccountIdentity);
            }
            return Ok();
        }

        // 发送邮件给指定玩家
        [Post]  // Url-> /api/player/SendMail
        public async Task<HttpResult> SendMail(HttpListenerRequest req, SendMailParam param)
        {
            if(param == null)
            {
                return Error(msg: "参数错误");
            }
            if (!long.TryParse(param.UserId, out long userId) || userId == 0)
            {
                return Error(msg: "参数错误2");
            }
            if (!long.TryParse(param.GameUserId, out long gameUserId) || gameUserId == 0)
            {
                return Error(msg: "参数错误3");
            }
            // 查找玩家在哪个服务器
            LoginCenter2S_GetLoginRecord l2sGetLoginRecord = null;
            try
            {
                var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                IResponse response = await loginCenterSession.Call(new S2LoginCenter_GetLoginRecord()
                {
                    UserId = userId
                });
                l2sGetLoginRecord = response as LoginCenter2S_GetLoginRecord;
                if (l2sGetLoginRecord == null)
                {
                    return Error(msg: "LoginCenter 没开启");
                }
            }
            catch (Exception e)
            {
                return Error(msg: e.ToString());
            }

 
            if (l2sGetLoginRecord.GameUserId != gameUserId)
            {
                // 角色不在线，直接发送
                MailInfo mailinfo = new MailInfo();
                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(param.ZoneId);
                mailinfo.MailName = param.Name;
                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                mailinfo.MailContent = param.Content;
                mailinfo.MailState = 0;
                mailinfo.ReceiveOrNot = 0;
                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                mailinfo.MailEnclosure.AddRange(param.MailItems);

                await MailSystem.SendMail(gameUserId, mailinfo, param.ZoneId);
                return Ok();
            }

            // 玩家不在线

            GM2Game_SendMail gm2game_SendMail = new GM2Game_SendMail();
            gm2game_SendMail.ZoneId = param.ZoneId;
            gm2game_SendMail.UserId = userId;
            gm2game_SendMail.GameUserId = gameUserId;
            gm2game_SendMail.Name = param.Name;
            gm2game_SendMail.Content = param.Content;
            gm2game_SendMail.MailItemsBson = param.MailItems.ToJson();

            var gameInfo = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, l2sGetLoginRecord.GameServerId);
            Session gameSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameInfo.ServerInnerIP);
            IResponse response2 = await gameSession.Call(gm2game_SendMail);
            Game2GM_SendMail game2gm_SendMail = response2 as Game2GM_SendMail;
            if (game2gm_SendMail == null || game2gm_SendMail.Error == 200105)
            {
                // 离线发送
                MailInfo mailinfo = new MailInfo();
                mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(param.ZoneId);
                mailinfo.MailName = param.Name;
                mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
                mailinfo.MailContent = param.Content;
                mailinfo.MailState = 0;
                mailinfo.ReceiveOrNot = 0;
                mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
                mailinfo.MailEnclosure.AddRange(param.MailItems);

                await MailSystem.SendMail(gameUserId, mailinfo, param.ZoneId);
            }

            return Ok();
        }


        // 发送邮件给所有玩家
        [Post]  // Url-> /api/player/SendMail
        public async Task<HttpResult> SendFullMail(HttpListenerRequest req, SendFullMailParam param)
        {
            if (param == null)
            {
                return Error(msg: "参数错误");
            }
            G2G_SendFullServiceMailMessage g2M_SendFullServiceMailMessage=new G2G_SendFullServiceMailMessage();
            g2M_SendFullServiceMailMessage.ZoneId = param.ZoneId;
            g2M_SendFullServiceMailMessage.Name = param.Name;
            g2M_SendFullServiceMailMessage.Content = param.Content;
            g2M_SendFullServiceMailMessage.MailItems = Help_JsonSerializeHelper.Serialize(param.MailItems);

            var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
            foreach (var Server in mMatchConfigs)
            {
                Dictionary<int, List<int>> keyValuePairs = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(Server.RunParameter);
                int mZoneId = keyValuePairs.FirstOrDefault().Key >> 16;
                if (mZoneId == param.ZoneId)
                    Game.Scene.GetComponent<NetInnerComponent>().Get(Server.ServerInnerIP).Send(g2M_SendFullServiceMailMessage);
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, param.ZoneId);

            MailInfo mailinfo = new MailInfo();
            mailinfo.MailId = IdGeneraterNew.Instance.GenerateUnitId(param.ZoneId);
            mailinfo.MailName = param.Name;
            mailinfo.MailAcceptanceTime = Help_TimeHelper.GetNowSecond();
            mailinfo.MailContent = param.Content;
            mailinfo.MailState = 0;
            mailinfo.ReceiveOrNot = 0;
            mailinfo.MailValidTime = Help_TimeHelper.GetNowSecond() + 1296000;
            mailinfo.MailEnclosure.AddRange(param.MailItems);


            DBMailData dBMailData = new DBMailData();
            dBMailData.Id = IdGeneraterNew.Instance.GenerateUnitId(param.ZoneId);
            dBMailData.MaliID = mailinfo.MailId;
            dBMailData.MailName = mailinfo.MailName;
            dBMailData.MailAcceptanceTime = mailinfo.MailAcceptanceTime;
            dBMailData.MailContent = mailinfo.MailContent;
            dBMailData.MailValidTime = mailinfo.MailValidTime;
            dBMailData.MailState = mailinfo.MailState;
            dBMailData.ReceiveOrNot = mailinfo.ReceiveOrNot;
            dBMailData.GameUserID = 0;//
            dBMailData.MailEnclosure = Help_JsonSerializeHelper.Serialize(mailinfo.MailEnclosure,true);
            dBProxy.Save(dBMailData).Coroutine();

            //MailSystem.SendFullServiceMail(param.ZoneId,mailinfo);
            return Ok();

        }

        // 添加称号
        [Post]  // Url-> /api/player/AddTitle
        public async Task<HttpResult> AddTitle(HttpListenerRequest req, AddTitleParam param)
        {
            if (param == null)
            {
                return Error(msg: "参数错误");
            }
            if (!long.TryParse(param.UserId, out long userId) || userId == 0)
            {
                return Error(msg: "参数错误2");
            }
            if (!long.TryParse(param.GameUserId, out long gameUserId) || gameUserId == 0)
            {
                return Error(msg: "参数错误3");
            }
            if (!long.TryParse(param.EndTime, out long endTime))
            {
                return Error(msg: "参数错误4");
            }
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, param.ZoneId);

            DBPlayerTitle dBPlayerTitle = new DBPlayerTitle();
            dBPlayerTitle.TitleID = param.TitleId;
            dBPlayerTitle.Id = IdGeneraterNew.Instance.GenerateUnitId(param.ZoneId);
            dBPlayerTitle.Type = param.Type;
            if (dBPlayerTitle.Type == 0)
            {
                // 绑定账号
                dBPlayerTitle.UserId = userId;
            }
            else
            {
                // 绑定角色
                dBPlayerTitle.UserId = gameUserId;
            }
            dBPlayerTitle.BingTime = Help_TimeHelper.GetNowSecond();
            dBPlayerTitle.EndTime = endTime;
            dBPlayerTitle.IsDisabled = 0;
            dBProxy.Save(dBPlayerTitle).Coroutine();

            return Ok();
        }

        // 删除称号
        [Post]  // Url-> /api/player/DelTitle
        public async Task<HttpResult> DelTitle(HttpListenerRequest req, DelTitleParam param)
        {
            if (param == null)
            {
                return Error(msg: "参数错误");
            }
            if (!long.TryParse(param.UserId, out long userId) || userId == 0)
            {
                return Error(msg: "参数错误2");
            }
            if (!long.TryParse(param.GameUserId, out long gameUserId) || gameUserId == 0)
            {
                return Error(msg: "参数错误3");
            }
            if (!long.TryParse(param.DBId, out long id) || id == 0)
            {
                return Error(msg: "参数错误4");
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, param.ZoneId);

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("IsDisabled", 1);
            dBProxy.UpdateOneSet<DBPlayerTitle>(p => p.Id == id, dic).Coroutine();

            return Ok();
        }
    }
}
