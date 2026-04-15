using CustomFrameWork.Component;
using CustomFrameWork;
using ETModel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_FriendRecommendHandler : AMActorRpcHandler<C2G_FriendRecommendRequest, G2C_FriendRecommendResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_FriendRecommendRequest b_Request, G2C_FriendRecommendResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_FriendRecommendRequest b_Request, G2C_FriendRecommendResponse b_Response, Action<IMessage> b_Reply)
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
            var mFriend = mPlayer.GetCustomComponent<FriendComponent>();
            if (mFriend == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(909);
                b_Reply(b_Response);
                return false;
            }

            var mDate = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().GetAllByZone(mAreaId);
            if(mDate == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(910);
                b_Reply(b_Response);
                return false;
            }

            if ((Help_TimeHelper.GetNowSecond() - mFriend.Refreshtime) < 30)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(911);
                b_Reply(b_Response);
                return false;
            }

           foreach(var PlayerInfo in mDate)
           {
                if(mFriend.AddRecommendList(PlayerInfo.Value.GameUserId))
                {
                    var mmPlayer = PlayerInfo.Value.GetCustomComponent<GamePlayer>();
                    if(mmPlayer == null ) continue;

                    if(PlayerInfo.Value.GameUserId == mPlayer.GameUserId) continue;
                    if (mFriend.CheckFriendList(PlayerInfo.Value.GameUserId)) continue;

                    b_Response.FList.Add(new Struct_Friends()
                    {
                        GameUserId = PlayerInfo.Value.GameUserId,
                        CharName = mmPlayer.Data.NickName,
                        IState =0,
                        ILV = mmPlayer.Data.Level,
                    });
                    if (mFriend.GetRecommendCount() >= 10)
                        break;
                }
           }

           mFriend.Refreshtime = Help_TimeHelper.GetNowSecond();
           b_Reply(b_Response);
           return true;
        }
    }
}