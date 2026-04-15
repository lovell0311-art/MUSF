using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_WarAllianceEstablishHandler : AMActorRpcHandler<C2G_WarAllianceEstablishRequest, G2C_WarAllianceEstablishResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_WarAllianceEstablishRequest b_Request, G2C_WarAllianceEstablishResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_WarAllianceEstablishRequest b_Request, G2C_WarAllianceEstablishResponse b_Response, Action<IMessage> b_Reply)
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
            var mGamePlayer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2516);
                b_Reply(b_Response);
                return false;
            }

            if (mGamePlayer.Data.Level < 10)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2516);
                b_Reply(b_Response);
                return false;
            }

            PlayerWarAllianceComponent playerWarAllianceComponent = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>();
            if (playerWarAllianceComponent != null && playerWarAllianceComponent.WarAllianceID != 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2517);
                b_Reply(b_Response);
                return false;
            }
            if (b_Request.WarAllianceName.Length >= 17)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2530);
                b_Reply(b_Response);
                return false;
            }
            long DeleteTiem = Help_TimeHelper.GetNowSecond();
            if (DeleteTiem - playerWarAllianceComponent.DeleteTime < 86400 && playerWarAllianceComponent.DeleteTime != 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2518);
                b_Reply(b_Response);
                return false;
            }
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
            var WarAllianceInfo = await dBProxy2.Query<DBWarAllianceData>(p => p.DBWarAllianceName == b_Request.WarAllianceName && p.IsDisabled != 1);
            if (WarAllianceInfo.Count >= 1)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2519);
                b_Reply(b_Response);
                return false;
            }

            G2M_WarAllianceEstablishRequest g2M_WarAllianceEstablishRequest = new G2M_WarAllianceEstablishRequest();
            g2M_WarAllianceEstablishRequest.PlayerInfo = new GMStruct_MemberInfo();
            g2M_WarAllianceEstablishRequest.AppendData = b_Request.AppendData;
            g2M_WarAllianceEstablishRequest.WarAllianceName = b_Request.WarAllianceName;
            g2M_WarAllianceEstablishRequest.WarAllianceBadge = b_Request.WarAllianceBadge;
            g2M_WarAllianceEstablishRequest.PlayerInfo.GameUserID = mPlayer.GameUserId;
            g2M_WarAllianceEstablishRequest.PlayerInfo.MemberName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
            g2M_WarAllianceEstablishRequest.PlayerInfo.MemberLevel = mPlayer.GetCustomComponent<GamePlayer>().Data.Level;
            g2M_WarAllianceEstablishRequest.PlayerInfo.MemberClassType = mPlayer.GetCustomComponent<GamePlayer>().Data.PlayerTypeId;
            g2M_WarAllianceEstablishRequest.PlayerInfo.GameServerID = OptionComponent.Options.AppId;
            //if (mPlayer.GetCustomComponent<GamePlayer>().Data.PlayerTypeId == (int)E_GameOccupation.Holyteacher)
            //{
            //    g2M_WarAllianceEstablishRequest.Command = 100;//圣导师统率
            //}

            IResponse Message = await playerWarAllianceComponent.WarAllianceSendMessageG_M(g2M_WarAllianceEstablishRequest) as IResponse;
            if (Message == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2501);
                b_Reply(b_Response);
                return false;
            }
            else if (Message.Error != 0)
            {
                b_Response.Error = Message.Error;
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("创建失败!");
                b_Reply(b_Response);
                return false;
            }
            else
            {
                
                M2G_WarAllianceEstablishResponse m2G_WarAllianceEstablishResponse = Message as M2G_WarAllianceEstablishResponse;
                
                playerWarAllianceComponent.UpData(m2G_WarAllianceEstablishResponse.Info);
                playerWarAllianceComponent.UpDateWarAlliancePlayerInfo();

                b_Response.Info = new Struct_WarAllinceInfo();
                b_Response.Info = playerWarAllianceComponent.GetInfo();
                b_Reply(b_Response);
                return true;
            }
        }
    }
}
