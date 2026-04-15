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
    public class C2G_AgreeToAddFriendHandler : AMActorRpcHandler<C2G_AgreeToAddFriendRequest, G2C_AgreeToAddFriendResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_AgreeToAddFriendRequest b_Request, G2C_AgreeToAddFriendResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_AgreeToAddFriendRequest b_Request, G2C_AgreeToAddFriendResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);

            Player mBePlayer = null;
            int mGameAreaId = mAreaId;
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                b_Reply(b_Response);
                return true;
            }
            for (int i = 0, len = mServerArea.VirtualIdlist.Count; i < len; i++)
            {
                mGameAreaId =mServerArea.VirtualIdlist[i] >> 16;
                mBePlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mGameAreaId, b_Request.GameUserId);
                if (mBePlayer != null) break;
            }
            
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(900);
                b_Reply(b_Response);
                return false;
            }

            if (b_Request.Type == 0)
            {
                var DeleteList = mFriend.GetFriendTypeList((int)FirendType.APPLICATION);
                
                if (DeleteList.TryGetValue(b_Request.GameUserId, out Friend FriendInfo))
                {
                    FriendInfo.IsDisabled = 1;
                    if (await mFriend.SetFriendDB(FriendInfo) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(905);
                        b_Reply(b_Response);
                        return false;
                    }
                    DeleteList.Remove(b_Request.GameUserId);
                }
            }
            else if (b_Request.Type == 1)
            {
                var mAddList = mFriend.GetFriendTypeList((int)FirendType.APPLICATION);
                if (mAddList == null) return false;

                if (mAddList.TryGetValue(b_Request.GameUserId, out Friend FriendInfo))
                {

                    //FriendInfo.GameUserId = b_Request.GameUserId;
                    FriendInfo.ListType = (int)FirendType.FIRENDSLIST;
                    //FriendInfo.CharName = b_Request.CharName;
                    FriendInfo.iState = 0;

                    if (!mFriend.SetFriendType(FriendInfo))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(906);
                        b_Reply(b_Response);
                        return false;
                    }
                    if (await mFriend.SetFriendDB(FriendInfo) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(905);
                        b_Reply(b_Response);
                        return false;
                    }
                    mAddList.Remove(b_Request.GameUserId);
                }

                if (mBePlayer != null)
                {
                    var mBeFriend = mBePlayer.GetCustomComponent<FriendComponent>();
                    if (mBeFriend == null)
                    {
                        mBeFriend = mBePlayer.AddCustomComponent<FriendComponent>();
                        if (await mBeFriend.Init(mAreaId) != true) return false; 
                    }

                    Friend BeFriendInfo = new Friend();
                    //修改FriendInfo的数据作为发起者信息存入申请人列表中
                    BeFriendInfo.GameUserId = mPlayer.GameUserId;
                    BeFriendInfo.ListType = (int)FirendType.FIRENDSLIST;
                    BeFriendInfo.CharName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
                    BeFriendInfo.iState = 0;
                    BeFriendInfo.WarAlliancePost = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>().MemberPost;
                    BeFriendInfo.WarAllianceName = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>().WarAllianceName;
                    BeFriendInfo.AreaId = mGameAreaId;
                    if (!mBeFriend.SetFriendType(BeFriendInfo))
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(906);
                        b_Reply(b_Response);
                        return false;
                    }
                    if (await mBeFriend.SetFriendDB(BeFriendInfo) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(905);
                        b_Reply(b_Response);
                        return false;
                    }
                    G2C_AgreeToAddFriend_notice g2C_AgreeToAddFriend_Notice = new G2C_AgreeToAddFriend_notice();
                    g2C_AgreeToAddFriend_Notice.CharName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
                    mBePlayer.Send(g2C_AgreeToAddFriend_Notice);
                }
                else
                {
                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, FriendInfo.AreaId);

                    DBFriendData dBFriendData = new DBFriendData()
                    {
                        Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                        FriendUserId = mPlayer.GameUserId,
                        CharName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName,
                        ListType = (int)FirendType.FIRENDSLIST,
                        GameUserId = b_Request.GameUserId,
                        AreaId = mAreaId,
                        IsDisabled = 0,
                    };

                    if (await dBProxy.Save(dBFriendData) == false)
                    {
                        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(905);
                        b_Reply(b_Response);
                        return false;
                    }
                }
            }

            b_Reply(b_Response);
            return true;

        }
    }
}