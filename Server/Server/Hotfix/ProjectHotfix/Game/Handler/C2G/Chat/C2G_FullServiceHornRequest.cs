using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Kms.V20190118.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_FullServiceHornRequestHandler : AMActorRpcHandler<C2G_FullServiceHornRequest, G2C_FullServiceHornResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_FullServiceHornRequest b_Request, G2C_FullServiceHornResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_FullServiceHornRequest b_Request, G2C_FullServiceHornResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("游戏区服不存在!");
                b_Reply(b_Response);
                return false;
            }
            //检查道具
            //var Bk = mPlayer.GetCustomComponent<BackpackComponent>();
            //if (Bk == null)
            //{
            //    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
            //    //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
            //    b_Reply(b_Response);
            //    return false;
            //}
            //检查月卡
            if (!mPlayer.GetCustomComponent<PlayerShopMallComponent>().GetPlayerShopState(DeviationType.MaxMonthlyCard))
            {
                 b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2211);
                 b_Reply(b_Response);
                 return false;
            }
            
            int LineId = mPlayer.SourceGameAreaId & 0xffff;

            G2C_FullServiceHornnotice g2C_FullServiceHornnotice = new G2C_FullServiceHornnotice();
            g2C_FullServiceHornnotice.SendGameUserId = mPlayer.GameUserId;
            g2C_FullServiceHornnotice.SendUserName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
            g2C_FullServiceHornnotice.SendTime = CustomFrameWork.TimeHelper.ClientNowSeconds();
            g2C_FullServiceHornnotice.MessageInfo = b_Request.ChatMessage;
            g2C_FullServiceHornnotice.LineId = LineId;
            var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Game);
            foreach (var Server in mMatchConfigs)
            {
                Dictionary<int, List<int>> keyValuePairs = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, List<int>>>(Server.RunParameter);
                int AreaId = 1;
                foreach (var KeyValuePair in keyValuePairs)
                {
                    AreaId = KeyValuePair.Key >> 16;
                    break;
                }
                if (mAreaId == AreaId)
                    Game.Scene.GetComponent<NetInnerComponent>().Get(Server.ServerInnerIP).Send(g2C_FullServiceHornnotice);
            }
            b_Reply(b_Response);
            return true;
        }
    }
}