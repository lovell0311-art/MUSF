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


namespace ETHotfix
{
	[HttpHandler(AppType.GM, "/api/account/")]
    public class Http_GM_Account : AHttpHandler
    {

        // 封禁玩家
        [Post]  // Url-> /api/account/Ban
        public async Task<HttpResult> Ban(HttpListenerRequest req, BanParam param)
        {
            if(param == null) return Error(msg: "参数错误");
            if(!long.TryParse(param.UserId,out long userId)) return Error(msg: "参数错误2");
            if (!long.TryParse(param.BanTillTime, out long banTillTime)) return Error(msg: "参数错误3");

            await AccountHelper.Ban(
                userId,
                banTillTime,
                param.BanReason);
            return Ok();
        }

        // 踢玩家下线
        [Post]  // Url-> /api/account/Kick
        public async Task<HttpResult> Kick(HttpListenerRequest req, KickParam param)
        {
            if (param == null) return Error(msg: "参数错误");
            if (!long.TryParse(param.UserId, out long userId)) return Error(msg: "参数错误2");

            await AccountHelper.Kick(
                userId,
                param.Reason);
            return Ok();
        }
    }
}
