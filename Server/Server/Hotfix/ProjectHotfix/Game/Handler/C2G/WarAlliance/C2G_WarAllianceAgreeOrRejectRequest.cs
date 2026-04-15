
using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_WarAllianceAgreeOrRejectRequestHandler : AMActorRpcHandler<C2G_WarAllianceAgreeOrRejectRequest, G2C_WarAllianceAgreeOrRejectResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_WarAllianceAgreeOrRejectRequest b_Request, G2C_WarAllianceAgreeOrRejectResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_WarAllianceAgreeOrRejectRequest b_Request, G2C_WarAllianceAgreeOrRejectResponse b_Response, Action<IMessage> b_Reply)
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

            if (playerWarAllianceComponent.MemberPost != (int)PostType.AllianceLeader && playerWarAllianceComponent.MemberPost != (int)PostType.ViceAllianceLeader)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2507);
                b_Reply(b_Response);
                return false;
            }

            if (b_Request.GameUserID.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2514);
                b_Reply(b_Response);
                return false;
            }

            G2M_WarAllianceAgreeOrRejectRequest g2M_WarAllianceAgreeOrRejectRequest = new G2M_WarAllianceAgreeOrRejectRequest();
            g2M_WarAllianceAgreeOrRejectRequest.AppendData = b_Request.AppendData;
            g2M_WarAllianceAgreeOrRejectRequest.WarAllianceID = playerWarAllianceComponent.WarAllianceID;
            g2M_WarAllianceAgreeOrRejectRequest.Type = b_Request.Type;
            //g2M_WarAllianceAgreeOrRejectRequest.GameMemeber.AddRange(b_Request.GameUserID.array);

            long[] GMember = new long[b_Request.GameUserID.Count];
            int Index = 0;
            var mDBProxy = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>().GetZoneDB(DBType.Core, mAreaId);
            if (mDBProxy == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2500);
                b_Reply(b_Response);
                return false;
            }

            foreach (long gMember in b_Request.GameUserID.array)
            {
                if (gMember == 0) break;

                var WInfo = await mDBProxy.Query<DBMemberInfo>(p => p.MemberID == gMember && p.IsDisabled == 1);
                if (WInfo.Count != 0)
                {
                    G2M_SendDeleteList g2M_SendDeleteList = new G2M_SendDeleteList();
                    g2M_SendDeleteList.WarAllianceID = playerWarAllianceComponent.WarAllianceID;
                    g2M_SendDeleteList.GameUserID = gMember;
                    mPlayer.GetSessionMGMT().Send(g2M_SendDeleteList);
                    continue;
                } 
                GMember[Index] = gMember;
                Index++;
            }

            if (GMember[0] == 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2523);
                b_Reply(b_Response);
                return true;
            }
            g2M_WarAllianceAgreeOrRejectRequest.GameMemeber.AddRange(GMember);

            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_WarAllianceAgreeOrRejectRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("操作失败!");
                b_Reply(b_Response);
                return false;
            }
            else
            {
                b_Reply(b_Response);
                return true;
            }
        }
    }
}