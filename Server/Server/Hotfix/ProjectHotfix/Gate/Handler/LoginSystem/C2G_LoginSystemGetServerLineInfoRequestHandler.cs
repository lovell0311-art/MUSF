using CustomFrameWork.Component;
using CustomFrameWork;

using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_LoginSystemGetServerLineInfoRequestHandler : AMRpcHandler<C2G_LoginSystemGetServerLineInfoRequest, G2C_LoginSystemGetServerLineInfoResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2G_LoginSystemGetServerLineInfoRequest b_Request, G2C_LoginSystemGetServerLineInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            Log.Info($"#SelectServer# GetServerLine start session={session?.Id ?? 0} area={b_Request.AreaId} page={b_Request.GetGameAreaPage}");
            SessionGateUserComponent mConnectGate = session.GetComponent<SessionGateUserComponent>();
            if (mConnectGate == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(112);
                b_Reply(b_Response);
                return false;
            }
            long mUserId = mConnectGate.UserId;
            GateUser mGateUser = Root.MainFactory.GetCustomComponent<GateUserComponent>().GetUserByUserId(mUserId);
            if (mGateUser == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(120);
                // b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }

            if (b_Request.AreaId == 1)
            {
                b_Response.GameAreaInfos.Add(new G2C_LoginSystemServerLineInfoMessage
                {
                    GameAreaId = 1,
                    GameAreaNickName = "永久区1线",
                    GameAreaType = 0,
                    IsGameAreaState = 1
                });
                b_Response.GameAreaInfos.Add(new G2C_LoginSystemServerLineInfoMessage
                {
                    GameAreaId = 3,
                    GameAreaNickName = "永久区3线",
                    GameAreaType = 0,
                    IsGameAreaState = 1
                });
                b_Response.GameAreaInfos.Add(new G2C_LoginSystemServerLineInfoMessage
                {
                    GameAreaId = 6,
                    GameAreaNickName = "永久区6线",
                    GameAreaType = 0,
                    IsGameAreaState = 1
                });

                Log.Info($"#SelectServer# GetServerLine finish user:{mGateUser.UserID} area:{b_Request.AreaId} lines:{b_Response.GameAreaInfos.Count}");
                b_Reply(b_Response);
                return true;
            }

            G2M_GetGameAreaLineInfoMessage mPackage = new G2M_GetGameAreaLineInfoMessage();
            mPackage.GetGameAreaPage = b_Request.GetGameAreaPage;
            mPackage.UserId = mGateUser.UserID;
            mPackage.AreaId = b_Request.AreaId;

            var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Match);
            int mServerIndex = Help_RandomHelper.Range(0, mMatchConfigs.Length);
            M2G_GetGameAreaLineInfoMessage mResult = await Game.Scene.GetComponent<NetInnerComponent>().Get(mMatchConfigs[mServerIndex].ServerInnerIP).Call(mPackage) as M2G_GetGameAreaLineInfoMessage;
            if (mResult == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21003);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("没有匹配到有效的MATCH服务器!");
                b_Reply(b_Response);
                return false;
            }
            if (mResult.Error != 0)
            {
                b_Response.Error = mResult.Error;
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage(mResult.Message);
                b_Reply(b_Response);
                return false;
            }

            if (mResult.GameAreaInfos != null && mResult.GameAreaInfos.Count > 0)
            {
                //response.GameAreaInfos = new List<G2C_GameAreaInfoMessage>();
                for (int i = 0, len = mResult.GameAreaInfos.Count; i < len; i++)
                {
                    G2C_GameAreaInfoMessage2 mData = mResult.GameAreaInfos[i];
                    G2C_LoginSystemServerLineInfoMessage mCopyData = new G2C_LoginSystemServerLineInfoMessage();
                    mCopyData.GameAreaId = mData.GameAreaId;
                    mCopyData.GameAreaNickName = mData.GameAreaNickName;
                    mCopyData.GameAreaType = mData.GameAreaType;
                    mCopyData.IsGameAreaState = mData.IsGameAreaState;

                    b_Response.GameAreaInfos.Add(mCopyData);
                }
            }
            Log.Info($"#SelectServer# GetServerLine finish user:{mGateUser.UserID} area:{b_Request.AreaId} lines:{b_Response.GameAreaInfos.Count} error:{b_Response.Error}");
            b_Reply(b_Response);
            return true;
        }
    }
}
