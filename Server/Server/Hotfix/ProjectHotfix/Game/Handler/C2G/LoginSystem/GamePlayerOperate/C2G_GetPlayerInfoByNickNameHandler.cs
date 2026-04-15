using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_GetPlayerInfoByNickNameHandler : AMActorRpcHandler<C2G_GetPlayerInfoByNickName, G2C_GetPlayerInfoByNickName>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_GetPlayerInfoByNickName b_Request, G2C_GetPlayerInfoByNickName b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_GetPlayerInfoByNickName b_Request, G2C_GetPlayerInfoByNickName b_Response, Action<IMessage> b_Reply)
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
            if (string.IsNullOrWhiteSpace(b_Request.NickName))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(301);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不能输入空昵称!");
                b_Reply(b_Response);
                return false;
            }
            //查找目标玩家
            var playerDict = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllByZone(mAreaId);
            foreach (var item in playerDict)
            {
                var curGamePlayer = item.Value.GetCustomComponent<GamePlayer>();
                if (curGamePlayer != null && curGamePlayer.Data.NickName == b_Request.NickName)
                {
                    b_Response.GameUserId = item.Value.GameUserId;
                    b_Reply(b_Response);
                    return true;
                }
            }
            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(302);
            //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("目标玩家不存在或不在线!");
            b_Reply(b_Response);
            return false;
        }
    }
}