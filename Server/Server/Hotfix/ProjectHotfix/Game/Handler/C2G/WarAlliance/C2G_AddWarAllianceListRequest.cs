using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_AddWarAllianceListRequestHandler : AMActorRpcHandler<C2G_AddWarAllianceListRequest, G2C_AddWarAllianceListResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AddWarAllianceListRequest b_Request, G2C_AddWarAllianceListResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AddWarAllianceListRequest b_Request, G2C_AddWarAllianceListResponse b_Response, Action<IMessage> b_Reply)
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
            if (playerWarAllianceComponent == null || playerWarAllianceComponent.WarAllianceID != 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2500);
                b_Reply(b_Response);
                return false;
            }

            if (await playerWarAllianceComponent.CheckRequestList(mAreaId)) { }
            
            G2M_AddWarAllianceListRequest g2M_AddWarAllianceListRequest = new G2M_AddWarAllianceListRequest();
            g2M_AddWarAllianceListRequest.AppendData = b_Request.AppendData;
            g2M_AddWarAllianceListRequest.Type = b_Request.Type;
            g2M_AddWarAllianceListRequest.GameUserID = mPlayer.GameUserId;
            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_AddWarAllianceListRequest) as IResponse;
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
                M2G_AddWarAllianceListResponse m2G_AddWarAllianceListResponse = Message as  M2G_AddWarAllianceListResponse;
                if (m2G_AddWarAllianceListResponse == null) return false;

                b_Response.List.AddRange(playerWarAllianceComponent.WarAllianceList);
                foreach (var Info in m2G_AddWarAllianceListResponse.WAInfo)
                {
                    Struct_WarAllinceInfo struct_WarAllinceInfo = new Struct_WarAllinceInfo();
                    struct_WarAllinceInfo.WarAllianceID = Info.WarAllianceID;
                    struct_WarAllinceInfo.WarAllianceName = Info.WarAllianceName;
                    struct_WarAllinceInfo.WarAllianceLevel = Info.WarAllianceLevel;
                    struct_WarAllinceInfo.LeaderName = Info.LeaderName;
                    struct_WarAllinceInfo.Currentquantity = Info.Currentquantity;
                    struct_WarAllinceInfo.WarAllianceNotice = Info.WarAllianceNotice;
                    struct_WarAllinceInfo.WarAllianceBadge = Info.WarAllianceBadge;
                    b_Response.WAInfo.Add(struct_WarAllinceInfo);
                }
                b_Reply(b_Response);
                return true;
            }
        }
    }
}