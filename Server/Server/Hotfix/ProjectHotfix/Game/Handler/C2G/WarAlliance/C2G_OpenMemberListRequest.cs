
using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_OpenMemberListRequestHandler : AMActorRpcHandler<C2G_OpenMemberListRequest, G2C_OpenMemberListResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenMemberListRequest b_Request, G2C_OpenMemberListResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenMemberListRequest b_Request, G2C_OpenMemberListResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = ErrorCodeHotfix.ERR_AccountNotExists;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }

            PlayerWarAllianceComponent playerWarAllianceComponent = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (playerWarAllianceComponent == null || playerWarAllianceComponent.WarAllianceID == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2500);
                b_Reply(b_Response);
                return false;
            }

            if(b_Request.Type != 0 && b_Request.Type != 1)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2511);
                b_Reply(b_Response);
                return false;
            }

            G2M_OpenMemberListRequest g2M_OpenMemberListRequest = new G2M_OpenMemberListRequest();
            g2M_OpenMemberListRequest.AppendData = b_Request.AppendData;
            g2M_OpenMemberListRequest.WarAllianceID = playerWarAllianceComponent.WarAllianceID;
            g2M_OpenMemberListRequest.Type = b_Request.Type;
            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_OpenMemberListRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;//Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2515);
                b_Reply(b_Response);
                return false;
            }
            else
            {
                M2G_OpenMemberListResponse m2G_WarAllianceEstablishResponse = Message as M2G_OpenMemberListResponse;
                
                foreach (var info in m2G_WarAllianceEstablishResponse.MemberList)
                {
                    Struct_MemberInfo struct_MemberInfo = new Struct_MemberInfo();
                    struct_MemberInfo.GameUserID = info.GameUserID;
                    struct_MemberInfo.MemberName = info.MemberName;
                    struct_MemberInfo.MemberLevel = info.MemberLevel;
                    struct_MemberInfo.MemberClassType = info.MemberClassType;
                    struct_MemberInfo.MemberPost = info.MemberPost;
                    struct_MemberInfo.MeberState = info.MeberState;
                    b_Response.MemberList.Add(struct_MemberInfo);
                }
                b_Reply(b_Response);
                return true;
            }
        }
    }
}