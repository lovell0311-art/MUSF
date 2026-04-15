using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_AddWarAllianceRequestHandler : AMActorRpcHandler<C2G_AddWarAllianceRequest, G2C_AddWarAllianceResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AddWarAllianceRequest b_Request, G2C_AddWarAllianceResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AddWarAllianceRequest b_Request, G2C_AddWarAllianceResponse b_Response, Action<IMessage> b_Reply)
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
            var GameDate = mPlayer.GetCustomComponent<GamePlayer>();
            if (GameDate == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2500);
                b_Reply(b_Response);
                return false;
            }
            if (GameDate.Data.Level < 10)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2516);
                b_Reply(b_Response);
                return false;
            }
            if (Help_TimeHelper.GetNowSecond() - playerWarAllianceComponent.ExitTime <= 7200 && 0 != playerWarAllianceComponent.ExitTime)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2503);
                b_Reply(b_Response);
                return false;
            }

            if (playerWarAllianceComponent.CheckWarAllianceList(b_Request.WarAllianceID))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2504);
                b_Reply(b_Response);
                return false;
            }
            GMStruct_MemberInfo gMStruct_MemberInfo = new GMStruct_MemberInfo();
            gMStruct_MemberInfo.GameUserID = mPlayer.GameUserId;
            gMStruct_MemberInfo.MemberName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
            gMStruct_MemberInfo.MemberLevel = mPlayer.GetCustomComponent<GamePlayer>().Data.Level;
            gMStruct_MemberInfo.MemberClassType = mPlayer.GetCustomComponent<GamePlayer>().Data.PlayerTypeId;
            gMStruct_MemberInfo.GameServerID = OptionComponent.Options.AppId;
            gMStruct_MemberInfo.MemberPost = 0;
            gMStruct_MemberInfo.MeberState = 0;

            G2M_AddWarAllianceRequest g2M_AddWarAllianceRequest = new G2M_AddWarAllianceRequest();
            g2M_AddWarAllianceRequest.GameMemeber = new GMStruct_MemberInfo();
            g2M_AddWarAllianceRequest.AppendData = b_Request.AppendData;
            g2M_AddWarAllianceRequest.WarAllianceID = b_Request.WarAllianceID;
            g2M_AddWarAllianceRequest.GameMemeber = gMStruct_MemberInfo;

            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_AddWarAllianceRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;// Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2505);
                b_Reply(b_Response);
                return false;
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (playerWarAllianceComponent.WarAllianceList[i] == 0)
                    {
                        playerWarAllianceComponent.WarAllianceList[i] = b_Request.WarAllianceID;
                        break;
                    }
                }
                b_Reply(b_Response);
                return true;
            }
        }
    }
}
