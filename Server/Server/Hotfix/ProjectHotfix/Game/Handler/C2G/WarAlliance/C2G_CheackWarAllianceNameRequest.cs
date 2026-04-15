using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_CheackWarAllianceNameRequestHandler : AMActorRpcHandler<C2G_CheackWarAllianceNameRequest, G2C_CheackWarAllianceNameResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_CheackWarAllianceNameRequest b_Request, G2C_CheackWarAllianceNameResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_CheackWarAllianceNameRequest b_Request, G2C_CheackWarAllianceNameResponse b_Response, Action<IMessage> b_Reply)
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
            if(b_Request.WarAllianceName.Length >= 17)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2530);
                b_Reply(b_Response);
                return false;
            }
            G2M_CheackWarAllianceNameRequest g2M_CheackWarAllianceNameRequest = new G2M_CheackWarAllianceNameRequest();
            g2M_CheackWarAllianceNameRequest.WarAllianceName = b_Request.WarAllianceName;

            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_CheackWarAllianceNameRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;// Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2506);
                b_Reply(b_Response);
                return false;
            }
            else
            {
                b_Reply(b_Response);
                return true;
            }
            /*
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            var WarAllianceInfo = await dBProxy2.Query<DBWarAllianceData>(p => p.DBWarAllianceName == b_Request.WarAllianceName && p.IsDisabled != 1);
            if (WarAllianceInfo.Count >= 1)
            {
                b_Response.Error =  0;// .ERR_DuplicateName;
                b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("战盟名称重复!");
                b_Reply(b_Response);
                return false;
            }

            b_Reply(b_Response);
            return true;*/
        }
    }
}