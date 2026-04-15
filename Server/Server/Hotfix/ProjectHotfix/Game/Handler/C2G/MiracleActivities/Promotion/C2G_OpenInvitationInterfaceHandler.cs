using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenInvitationInterfaceHandler : AMActorRpcHandler<C2G_OpenInvitationInterface, G2C_OpenInvitationInterface>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenInvitationInterface b_Request, G2C_OpenInvitationInterface b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenInvitationInterface b_Request, G2C_OpenInvitationInterface b_Response, Action<IMessage> b_Reply)
        {
            b_Reply(b_Response);
            return true;
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_AccountNotExists;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }

            G2M_GetPromotionCode g2M_GetPromotionCode = new G2M_GetPromotionCode();
            g2M_GetPromotionCode.AppendData = b_Request.AppendData;
            g2M_GetPromotionCode.UserID = mPlayer.UserId;
            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
            Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
            IResponse Message = await loginCenterSession.Call(g2M_GetPromotionCode);
            //IResponse Message =await  mPlayer.GetSessionMGMT().Call(g2M_GetPromotionCode);
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;//Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2502);
                b_Reply(b_Response);
                return false;
            }
            else
            {
               var Info = ( Message as M2G_GetPromotionCode).Codeinfo;
                if (Info != null)
                {
                    b_Response.Info = new InvitationInfo();
                    b_Response.Info.InvitationCode = Info.InvitationCode;
                    b_Response.Info.NumberCntI = Info.NumberCntI;
                    b_Response.Info.AwardStatusI = Info.AwardStatusI;
                    b_Response.Info.NumberCntII = Info.NumberCntII;
                    b_Response.Info.AwardStatusII = Info.AwardStatusII;
                    b_Response.Info.NumberCntIII = Info.NumberCntIII;
                    b_Response.Info.AwardStatusIII = Info.AwardStatusIII;
                    b_Reply(b_Response);
                    return true;
                }
                b_Response.Error = 303;
                b_Reply(b_Response);
                return false;
            }
        }
    }
}