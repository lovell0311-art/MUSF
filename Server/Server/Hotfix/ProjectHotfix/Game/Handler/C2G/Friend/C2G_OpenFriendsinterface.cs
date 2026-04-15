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
    public class C2G_OpenFriendsinterfaceHandler : AMActorRpcHandler<C2G_OpenFriendsinterfaceRequest, G2C_OpenFriendsinterfaceResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_OpenFriendsinterfaceRequest b_Request, G2C_OpenFriendsinterfaceResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_OpenFriendsinterfaceRequest b_Request, G2C_OpenFriendsinterfaceResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
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
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(900);
                b_Reply(b_Response);
                return true;
            }

            if (iFriendComponent != null)
            {
                var Friendlist = iFriendComponent.GetFriendTypeList(b_Request.ListType);

                if (Friendlist == null || Friendlist.Count > 100)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(907);
                    b_Reply(b_Response);
                    return true;
                }

                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
                if (mServerArea == null)
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                    b_Reply(b_Response);
                    return true;
                }
                Player mmPlayer = null;
                foreach (var fiend in Friendlist)
                {
                    for (int i = 0, len = mServerArea.VirtualIdlist.Count; i < len; i++)
                    {
                        int mGameAreaId = mServerArea.VirtualIdlist[i] >> 16;
                        mmPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mGameAreaId, fiend.Value.GameUserId);
                        if (mmPlayer != null) break;
                    }
                    Struct_Friends struct_Friends = new Struct_Friends();
                    if (mmPlayer != null)
                    {
                        struct_Friends.GameUserId = fiend.Value.GameUserId;
                        struct_Friends.CharName = fiend.Value.CharName;
                        struct_Friends.ListType = fiend.Value.ListType;
                        struct_Friends.TimeDate = fiend.Value.TimeDate;
                        struct_Friends.IState = 0;
                        struct_Friends.ILV = mmPlayer.GetCustomComponent<GamePlayer>().Data.Level;
                        struct_Friends.ClassType = mmPlayer.GetCustomComponent<GamePlayer>().Data.PlayerTypeId;
                        struct_Friends.WarAlliancePost = mmPlayer.GetCustomComponent<PlayerWarAllianceComponent>().MemberPost;
                        struct_Friends.WarAllianceName = mmPlayer.GetCustomComponent<PlayerWarAllianceComponent>().WarAllianceName;
                        struct_Friends.OccupationLevel = mmPlayer.GetCustomComponent<GamePlayer>().Data.OccupationLevel;
                    }
                    else
                    {
                        struct_Friends.GameUserId = fiend.Value.GameUserId;
                        struct_Friends.CharName = fiend.Value.CharName;
                        struct_Friends.ListType = fiend.Value.ListType;
                        struct_Friends.TimeDate = fiend.Value.TimeDate;
                        struct_Friends.IState = 1;
                        struct_Friends.ILV = fiend.Value.iLv;
                        struct_Friends.ClassType = fiend.Value.ClassType;
                        struct_Friends.WarAlliancePost = fiend.Value.WarAlliancePost;
                        struct_Friends.WarAllianceName = fiend.Value.WarAllianceName;
                        struct_Friends.OccupationLevel =0;
                    }

                    b_Response.FList.Add(struct_Friends);
                }
            }

            b_Reply(b_Response);
            return true;
        }
    }
}