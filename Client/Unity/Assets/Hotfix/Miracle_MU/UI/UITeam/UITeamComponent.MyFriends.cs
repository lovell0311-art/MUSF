using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 我的好友
    /// </summary>
    public partial class UITeamComponent
    {
        public void Init_MyFriends()
        {
            List<OtherPlayerInfo> MyFriendsList = new List<OtherPlayerInfo>();
            GetMyFriends().Coroutine();

            ///获取我的好友列表
            async ETVoid GetMyFriends()
            {
                G2C_OpenFriendsinterfaceResponse g2C_OpenFriendsinterface = (G2C_OpenFriendsinterfaceResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenFriendsinterfaceRequest
                {
                    ListType = 4
                });
                if (g2C_OpenFriendsinterface.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenFriendsinterface.Error.GetTipInfo());
                }
                else
                {
                    MyFriendsList.Clear();
                    foreach (var friend in g2C_OpenFriendsinterface.FList)
                    {
                        MyFriendsList.Add(new OtherPlayerInfo
                        {
                            roleUUId = friend.GameUserId,
                            roleName = friend.CharName,
                            roleLev = friend.ILV,
                            roleType = friend.ClassType,
                            warName = friend.WarAllianceName,
                            isInvite = false
                        });
                    }
                    MyFriendsList.Sort((m1, m2) =>
                    {
                        return m2.roleLev.CompareTo(m1.roleLev);
                    });
                    NearPlayersScrollView.Items = MyFriendsList;
                }
            }
        }
    }
}
