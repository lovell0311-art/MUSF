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
	[HttpHandler(AppType.GM, "/api/server/")]
    public class Http_GM_Server : AHttpHandler
    {

        // 游戏服务器状态
        [Post]  // Url-> /api/server/GameStatus
        public async Task<HttpResult> GameStatus(HttpListenerRequest req, GameStatusParam param)
        {
            if(param == null)
            {
                return Error(msg: "参数错误");
            }

            Game2GM_GetGameServerStatus game2GM_GetGameServerStatus = null;
     
            int ServerStatus = 0;
            int OnlineCount = 0;
            int EnterGameCount = 0;
            try
            {
                var gameServer = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(AppType.Game, param.ServerId);
                if(gameServer == null)
                {
                    return Ok(data: new
                    {
                        ServerStatus = 0,
                        OnlineCount = 0,
                        EnterGameCount = 0
                    });
                }

                Session gameServerSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameServer.ServerInnerIP);
                game2GM_GetGameServerStatus = (Game2GM_GetGameServerStatus)await gameServerSession.Call(new GM2Game_GetGameServerStatus() { });
                ServerStatus = 1;
                OnlineCount = game2GM_GetGameServerStatus.OnlineCount;
                EnterGameCount = game2GM_GetGameServerStatus.EnterGameCount;
            }
            catch(Exception e)
            {
                ServerStatus = 0;
            }

            return Ok(data: new
            {
                ServerStatus = ServerStatus,
                OnlineCount = OnlineCount,
                EnterGameCount = EnterGameCount
            });
        }

        // 运行代码
        [Post]  // Url-> /api/server/GameStatus
        public async Task<HttpResult> RunCode(HttpListenerRequest req, RunCodeParam param)
        {
            if (param == null)
            {
                return Error(msg: "参数错误");
            }

            S2GM_RunCode s2GM_RunCode = null;

            string runReturn = "";
            
            try
            {
                var gameServer = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(param.ServerId);
                if (gameServer == null)
                {
                    return Ok(data: new
                    {
                        Return = "与目标服务器失去连接"
                    });
                }

                Session gameServerSession = Game.Scene.GetComponent<NetInnerComponent>().Get(gameServer.ServerInnerIP);
                s2GM_RunCode = (S2GM_RunCode)await gameServerSession.Call(new GM2S_RunCode() { Code = param.Code});
                runReturn = s2GM_RunCode.Return;
            }
            catch (Exception e)
            {
                runReturn = "与目标服务器失去连接";
            }

            return Ok(data: new
            {
                Return = runReturn
            });
        }


    }
}
