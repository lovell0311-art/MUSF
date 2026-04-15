using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PlayerGetWarAlliacneNameRequestHandler : AMActorRpcHandler<C2G_PlayerGetWarAlliacneNameRequest, C2G_PlayerGetWarAlliacneNameResponse>
    {
        protected override async Task<bool> Run(C2G_PlayerGetWarAlliacneNameRequest b_Request, C2G_PlayerGetWarAlliacneNameResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            Player mFriendPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.GameUserID);
            if (mPlayer == null || mFriendPlayer ==null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_AccountNotExists;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }

            PlayerWarAllianceComponent playerWarAllianceComponent = mFriendPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (playerWarAllianceComponent == null || playerWarAllianceComponent.WarAllianceID == 0)
            {
                b_Response.Error =  0;// .ERR_WarAllianceInfoError;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("战盟信息错误!");
                b_Reply(b_Response);
                return false;
            }

            b_Response.WarAllianceName = playerWarAllianceComponent.WarAllianceName;
            b_Reply(b_Response);
            return true;
        }
    }
}
