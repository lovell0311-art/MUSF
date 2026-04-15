using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
using TencentCloud.Smpn.V20190822.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_FriendPositionHandler : AMActorRpcHandler<C2G_FriendPositionRequest, G2C_FriendPositionResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_FriendPositionRequest b_Request, G2C_FriendPositionResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_FriendPositionRequest b_Request, G2C_FriendPositionResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }

            Player FriendPlayer = null;
            var mGameAreaId = mAreaId;
            for (int i = 0, len = mServerArea.VirtualIdlist.Count; i < len; i++)
            {
                mGameAreaId = mServerArea.VirtualIdlist[i] >> 16;
                FriendPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mGameAreaId, b_Request.GameUserId);
                if (FriendPlayer != null) break;
            }

            if (mPlayer == null || FriendPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("账号不存在!");
                b_Reply(b_Response);
                return false;
            }

            var MapInfo = mPlayer.GetCustomComponent<GamePlayer>();
            if (MapInfo == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(913);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("位置信息异常!");
                b_Reply(b_Response);
                return false;
            }

            b_Reply(b_Response);
           

            G2C_FriendPositionMessage g2C_FriendPositionMessage = new G2C_FriendPositionMessage();
            g2C_FriendPositionMessage.GameUserId = mPlayer.GameUserId;
            g2C_FriendPositionMessage.X = MapInfo.UnitData.X;
            g2C_FriendPositionMessage.Y = MapInfo.UnitData.Y;
            g2C_FriendPositionMessage.MapID = MapInfo.UnitData.Index;
            g2C_FriendPositionMessage.DateTime = Help_TimeHelper.GetNowSecond();
            g2C_FriendPositionMessage.CharName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
            FriendPlayer.Send(g2C_FriendPositionMessage);

            return true;
        }
    }
}
