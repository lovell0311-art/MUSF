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
    public class C2G_AddFriendsHandler : AMActorRpcHandler<C2G_AddFriendsRequest, G2C_AddFriendsResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AddFriendsRequest b_Request, G2C_AddFriendsResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AddFriendsRequest b_Request, G2C_AddFriendsResponse b_Response, Action<IMessage> b_Reply)
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
                return false;
            }
            if (mPlayer.GameAreaId <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(201);
                b_Reply(b_Response);
                return false;
            }
            Player FriendPlayer = null;
            var mGameAreaId = mAreaId;
            for (int i = 0, len = mServerArea.VirtualIdlist.Count; i < len; i++)
            {
                mGameAreaId =mServerArea.VirtualIdlist[i] >> 16;
                FriendPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mGameAreaId, b_Request.GameUserId);
                if (FriendPlayer != null) break;
            }
            if (FriendPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(912);
                b_Reply(b_Response);
                return false;
            }

            var FGamePlayer = FriendPlayer.GetCustomComponent<FriendComponent>();
            if (FGamePlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(900);
                b_Reply(b_Response);
                return false;
            }
            if (FGamePlayer.CheckFriendList(mPlayer.GameUserId))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(916);
                b_Reply(b_Response);
                return false;
            }

            if (!FGamePlayer.AddFriends(mPlayer, mGameAreaId))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(902);
                b_Reply(b_Response);
                return false;
            }
            //构建通知，通知被申请者
            Struct_Friends friend = new Struct_Friends();
            friend.GameUserId = mPlayer.GameUserId;
            friend.CharName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
            friend.ListType = (int)FirendType.APPLICATION;
            friend.IState = 0;
            friend.ILV = mPlayer.GetCustomComponent<GamePlayer>().Data.Level;
            friend.WarAlliancePost = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>().MemberPost;
            friend.WarAllianceName = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>().WarAllianceName;
            G2C_AddFriendsnotice_notice g2C_AddFriendsnotice_Notice = new G2C_AddFriendsnotice_notice();
            g2C_AddFriendsnotice_Notice.FriendInfo = friend;
            FriendPlayer.Send(g2C_AddFriendsnotice_Notice);
            //数据库写入
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, mGameAreaId);
            if (dBProxy == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(904);
                b_Reply(b_Response);
                return false;
            }

            DBFriendData dBFriendData = new DBFriendData()
            {
                Id = IdGeneraterNew.Instance.GenerateUnitId(mAreaId),
                FriendUserId = mPlayer.GameUserId,
                CharName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName,
                ListType = (int)FirendType.APPLICATION,
                GameUserId = FriendPlayer.GameUserId,
                AreaId = mAreaId,
                IsDisabled = 0,
            };
            DataCacheManageComponent mDataCacheManageComponent = FriendPlayer.AddCustomComponent<DataCacheManageComponent>();
            if (mDataCacheManageComponent == null) return false;
            var dataCache = mDataCacheManageComponent.Get<DBFriendData>();
            if (dataCache == null) return false;

            var backPackDatas = dataCache.DataQuery(p => p.GameUserId == FriendPlayer.GameUserId && p.FriendUserId == mPlayer.GameUserId && p.IsDisabled != 1 && p.ListType == 3);
            if (backPackDatas != null && backPackDatas.Count >= 1) return false;

            if (backPackDatas.Count == 0 || backPackDatas == null)
            {
                dataCache.DataAdd(dBFriendData);
                await dBProxy.Save(dBFriendData);
            }

            b_Reply(b_Response);
            return true;

        }
    }
}