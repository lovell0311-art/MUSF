using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_FriendChatHandler : AMActorRpcHandler<C2G_FriendChatRequest, G2C_FriendChatResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_FriendChatRequest b_Request, G2C_FriendChatResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_FriendChatRequest b_Request, G2C_FriendChatResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                b_Reply(b_Response);
                return false;
            }

            var FriendSystem = mPlayer.GetCustomComponent<FriendComponent>();
            if (FriendSystem == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                b_Reply(b_Response);
                return false;
            }
            if (FriendSystem.FriendLiset[4].TryGetValue(b_Request.GameUserId, out Friend BeFriend) != true) return false;

            Player FriendPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(BeFriend.AreaId, b_Request.GameUserId);
            if (FriendPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return false;
            }

            if (b_Request.IMessage.Length > 50)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1302);
                b_Reply(b_Response);
                return false;
            }
            FriendComponent friendComponent = mPlayer.GetCustomComponent<FriendComponent>();
            FriendComponent BefriendComponent = FriendPlayer.GetCustomComponent<FriendComponent>();
            if (friendComponent == null || BefriendComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1303);
                b_Reply(b_Response);
                return false;
            }
            
            if (friendComponent.GetFriendTypeList((int)FirendType.FIRENDSLIST).ContainsKey(b_Request.GameUserId))
            {
                if (BefriendComponent.GetFriendTypeList((int)FirendType.FIRENDSLIST).ContainsKey(mPlayer.GameUserId))
                {
                    G2C_FriendChat_notice g2C_FriendChat_Notice = new G2C_FriendChat_notice();
                    g2C_FriendChat_Notice.IMessage = b_Request.IMessage;
                    g2C_FriendChat_Notice.GameUserId = mPlayer.GameUserId;
                    g2C_FriendChat_Notice.DateTiem = Help_TimeHelper.GetNowSecond();
                    g2C_FriendChat_Notice.CharName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;

                    FriendPlayer.Send(g2C_FriendChat_Notice);
                    b_Reply(b_Response);
                    return true;
                }
            }

            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1304);
            b_Reply(b_Response);
            return false;

        }
    }
}