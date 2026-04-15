using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_SearchForFriendsHandler : AMActorRpcHandler<C2G_SearchForFriendsRequest, G2C_SearchForFriendsResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_SearchForFriendsRequest b_Request, G2C_SearchForFriendsResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_SearchForFriendsRequest b_Request, G2C_SearchForFriendsResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                b_Reply(b_Response);
                return true;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                b_Reply(b_Response);
                return false;
            }

            var iFriendComponent = mPlayer.GetCustomComponent<FriendComponent>();
            if (iFriendComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(909);
                b_Reply(b_Response);
                return false;
            }

            Player FrinedPlayer = new Player();
            bool IsExistence = true;//默认玩家存在
            for (int i = 0, len = mServerArea.VirtualIdlist.Count; i < len; i++)
            {
                var mGameAreaId = mServerArea.VirtualIdlist[i] >> 16;
                FrinedPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().NameGet(mGameAreaId, b_Request.CharName);
                if (FrinedPlayer != null)
                {
                    IsExistence = true;
                    break;
                }

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var mDBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mGameAreaId);

                var mQueryNamelist = await mDBProxy.Query<DBGamePlayerData>(p => p.NickName == b_Request.CharName);
                if (mQueryNamelist != null && mQueryNamelist.Count > 0)
                {
                    b_Response.CharName = b_Request.CharName;
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(914);
                    b_Reply(b_Response);
                    return true;
                }
                IsExistence = false;
            }

            if (!IsExistence)
            {
                b_Response.CharName = b_Request.CharName;
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(915);
                b_Reply(b_Response);
                return true;
            }

            if (iFriendComponent.CheckFriendList(FrinedPlayer.GameUserId))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(916);
                b_Reply(b_Response);
                return false;
            }

            var FPlayer = FrinedPlayer.GetCustomComponent<GamePlayer>();
            b_Response.GameUserId = FrinedPlayer.GameUserId;
            b_Response.CharName = FPlayer.Data.NickName;
            b_Response.IState = 0;
            b_Response.ILV = FPlayer.Data.Level;
            b_Reply(b_Response);

            return true;

        }
    }
}