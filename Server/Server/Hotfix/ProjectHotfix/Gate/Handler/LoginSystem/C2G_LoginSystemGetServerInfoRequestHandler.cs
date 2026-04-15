using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETHotfix
{
    [MessageHandler(AppType.Gate)]
    public class C2G_LoginSystemGetServerInfoRequestHandler : AMRpcHandler<C2G_LoginSystemGetServerInfoRequest, G2C_LoginSystemGetServerInfoResponse>
    {
        protected override async Task<bool> CodeAsync(Session session, C2G_LoginSystemGetServerInfoRequest b_Request, G2C_LoginSystemGetServerInfoResponse b_Response, Action<IMessage> b_Reply)
        {
            Log.Info($"#SelectServer# GetServerInfo start session={session?.Id ?? 0}");
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
                //response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }/*
            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Realm);
            Session mSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
            R2G_GetServerLineInfoResponse mR2G_GetServerLineInfoResponse = (R2G_GetServerLineInfoResponse)await mSession.Call(new G2R_GetServerLineInfoRequest(){});

            foreach (var info in mR2G_GetServerLineInfoResponse.GameAreaInfos)
            {
                G2C_LoginSystemServerInfoMessage LineInfo = new G2C_LoginSystemServerInfoMessage();
                LineInfo.GameAreaId = info.GameAreaId;
                LineInfo.GameAreaNickName = info.GameAreaNickName;
                LineInfo.GameAreaType = info.GameAreaType;
                LineInfo.IsGameAreaState = info.IsGameAreaState;
                b_Response.GameAreaInfos.Add(LineInfo);
            }
            b_Reply(b_Response);
            return true;*/
            G2C_LoginSystemServerInfoMessage mCopyData2 = new G2C_LoginSystemServerInfoMessage();
            mCopyData2.GameAreaId = 1;
            mCopyData2.GameAreaNickName = "永久区";
            mCopyData2.GameAreaType = 0;
            mCopyData2.IsGameAreaState = 1;
            b_Response.GameAreaInfos.Add(mCopyData2);
            Log.Info($"#SelectServer# GetServerInfo finish user:{mGateUser.UserID} areas:{b_Response.GameAreaInfos.Count}");

            //mCopyData2 = new G2C_LoginSystemServerInfoMessage();
            //mCopyData2.GameAreaId = 2;
            //mCopyData2.GameAreaNickName = "二区";
            //mCopyData2.GameAreaType = 0;
            //mCopyData2.IsGameAreaState = 1;
            //b_Response.GameAreaInfos.Add(mCopyData2);

            //mCopyData2 = new G2C_LoginSystemServerInfoMessage();
            //mCopyData2.GameAreaId = 3;
            //mCopyData2.GameAreaNickName = "三区";
            //mCopyData2.GameAreaType = 0;
            //mCopyData2.IsGameAreaState = 1;
            //b_Response.GameAreaInfos.Add(mCopyData2);

            //mCopyData2 = new G2C_LoginSystemServerInfoMessage();
            //mCopyData2.GameAreaId = 4;
            //mCopyData2.GameAreaNickName = "四区";
            //mCopyData2.GameAreaType = 0;
            //mCopyData2.IsGameAreaState = 1;
            //b_Response.GameAreaInfos.Add(mCopyData2);

            //mCopyData2 = new G2C_LoginSystemServerInfoMessage();
            //mCopyData2.GameAreaId = 5;
            //mCopyData2.GameAreaNickName = "五区";
            //mCopyData2.GameAreaType = 0;
            //mCopyData2.IsGameAreaState = 1;
            //b_Response.GameAreaInfos.Add(mCopyData2);

            //mCopyData2 = new G2C_LoginSystemServerInfoMessage();
            //mCopyData2.GameAreaId = 6;
            //mCopyData2.GameAreaNickName = "六区";
            //mCopyData2.GameAreaType = 0;
            //mCopyData2.IsGameAreaState = 1;
            //b_Response.GameAreaInfos.Add(mCopyData2);
            //mCopyData2 = new G2C_LoginSystemServerInfoMessage();
            //mCopyData2.GameAreaId = 7;
            //mCopyData2.GameAreaNickName = "七区";
            //mCopyData2.GameAreaType = 0;
            //mCopyData2.IsGameAreaState = 1;
            //b_Response.GameAreaInfos.Add(mCopyData2);
            b_Reply(b_Response);
            return true;

            G2M_GetGameAreaInfoMessage mPackage = new G2M_GetGameAreaInfoMessage();
            mPackage.GetGameAreaPage = b_Request.GetGameAreaPage;
            mPackage.UserId = mGateUser.UserID;

            var mMatchConfigs = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Match);
            int mServerIndex = Help_RandomHelper.Range(0, mMatchConfigs.Length);
            M2G_GetGameAreaInfoMessage mResult = await Game.Scene.GetComponent<NetInnerComponent>().Get(mMatchConfigs[mServerIndex].ServerInnerIP).Call(mPackage) as M2G_GetGameAreaInfoMessage;
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
                return true;
            }
            if (mResult.GameAreaInfos != null && mResult.GameAreaInfos.Count > 0)
            {
                //response.GameAreaInfos = new List<G2C_GameAreaInfoMessage>();
                for (int i = 0, len = mResult.GameAreaInfos.Count; i < len; i++)
                {
                    G2C_GameAreaInfoMessage2 mData = mResult.GameAreaInfos[i];
                    G2C_LoginSystemServerInfoMessage mCopyData = new G2C_LoginSystemServerInfoMessage();
                    mCopyData.GameAreaId = mData.GameAreaId;
                    mCopyData.GameAreaNickName = mData.GameAreaNickName;
                    mCopyData.GameAreaType = mData.GameAreaType;
                    mCopyData.IsGameAreaState = mData.IsGameAreaState;

                    b_Response.GameAreaInfos.Add(mCopyData);
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}
