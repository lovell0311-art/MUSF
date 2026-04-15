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
    public class C2G_DeleteOrBlockFriendHandler : AMActorRpcHandler<C2G_DeleteOrBlockFriendRequest, G2C_DeleteOrBlockFriendResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_DeleteOrBlockFriendRequest b_Request, G2C_DeleteOrBlockFriendResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_DeleteOrBlockFriendRequest b_Request, G2C_DeleteOrBlockFriendResponse b_Response, Action<IMessage> b_Reply)
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

            var mFriendComponent = mPlayer.GetCustomComponent<FriendComponent>();
            if (mFriendComponent == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(900);
                b_Reply(b_Response);
                return false;
            }

            var FirendList = mFriendComponent.GetFriendTypeList(b_Request.ListType);
            if (FirendList == null && FirendList.Count <= 0)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(907);
                b_Reply(b_Response);
                return false;
            }

            if (b_Request.Type == 0)
            {
                int AreaId = mFriendComponent.DeleteFriend(b_Request.ListType, b_Request.GameUserId);
                if(AreaId == 0 )return false;

                Player mBePlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(AreaId, b_Request.GameUserId);
                if (mBePlayer != null)
                {
                    var mBeFriend = mBePlayer.GetCustomComponent<FriendComponent>();
                    if (mBeFriend != null)
                    {
                        mBeFriend.DeleteFriend(b_Request.ListType, mPlayer.GameUserId);
                    }
                    else
                    {
                        mBeFriend = mBePlayer.AddCustomComponent<FriendComponent>();
                        await mBeFriend.Init(mAreaId);
                        mBeFriend.DeleteFriend(b_Request.ListType, mPlayer.GameUserId);
                    }
                }
                else
                {
                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, AreaId);

                    var mQueryNamelist = await dBProxy.Query<DBFriendData>(p => p.GameUserId == b_Request.GameUserId && p.FriendUserId == mPlayer.GameUserId);
                    if (mQueryNamelist != null && mQueryNamelist.Count >= 1)
                    {
                        DBFriendData dBFriendData = mQueryNamelist[0] as DBFriendData;
                        dBFriendData.ListType = 0;
                        dBFriendData.IsDisabled = 1;
                        var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(AreaId);
                        mWriteDataComponent.Save(dBFriendData, dBProxy).Coroutine();
                    }
                }
            }
            else if(b_Request.Type == 1)
            {
                if (b_Request.ListType == 0)
                {
                    Player mFriendPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.GameUserId);
                    if (mFriendComponent.BlockRecommendFriend(mFriendPlayer))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(908);
                        b_Reply(b_Response);
                        return false;
                    }

                    b_Reply(b_Response);
                    return true;
                }
                if (!mFriendComponent.BlockFriend(b_Request.ListType, b_Request.GameUserId))
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(908);
                    b_Reply(b_Response);
                    return false;
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}