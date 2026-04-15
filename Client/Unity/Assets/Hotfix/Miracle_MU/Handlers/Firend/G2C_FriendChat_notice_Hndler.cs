using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_FriendChat_notice_Hndler : AMHandler<G2C_FriendChat_notice>//好友聊天通知
    {
        protected override void Run(ETModel.Session session, G2C_FriendChat_notice message)
        {
            //如果消息缓存中没有这个玩家ID,先添加进来
            if (!FriendListData.friendChatNewInfos.ContainsKey(message.GameUserId))
            {
                FriendListData.friendChatNewInfos.Add(message.GameUserId, new List<FriendChatNewInfo>());
            }
            //接收到的消息缓存起来
            FriendChatNewInfo chatNewInfo = new FriendChatNewInfo()
            {
                UUID = message.GameUserId,
                NickName = message.CharName,
                Time = message.DateTiem,
                Message = message.IMessage,
                type = ChatType.OtherSideMessage
            };
            List<FriendChatNewInfo> value;
            if (FriendListData.friendChatNewInfos.TryGetValue(message.GameUserId, out value))
            {
                value.Add(chatNewInfo);
            }
          //  Log.Debug($"接收到{message.CharName}玩家发送的普通消息");
            if (FriendListData.FriendList.Count > 0)
            {
                foreach (var item in FriendListData.FriendList)
                {
                    //如果通知的是当前选择的好友,刷新
                    if (UIComponent.Instance.Get(UIType.UIFirendList).GetComponent<UIFriendListComponent>()?.nowFriendInfo?.UUID == message.GameUserId)
                    {
                        UIComponent.Instance.Get(UIType.UIFirendList).GetComponent<UIFriendListComponent>().uICircular_Chat.Items =
                            FriendListData.friendChatNewInfos[message.GameUserId];
                        UIComponent.Instance.Get(UIType.UIFirendList).GetComponent<UIFriendListComponent>().chatView.verticalNormalizedPosition = 0f;
                    }
                    //如果当前在好友界面,更新顺序
                    if (UIComponent.Instance.Get(UIType.UIFirendList).GetComponent<UIFriendListComponent>().Cur_E_Friends == E_FriendsTogNewType.Friend)
                    {
                        int a = FriendListData.FriendList.IndexOf(item);
                        UIComponent.Instance.Get(UIType.UIFirendList).GetComponent<UIFriendListComponent>().Swap(FriendListData.FriendList, a, 0);
                        UIComponent.Instance.Get(UIType.UIFirendList).GetComponent<UIFriendListComponent>().uICircular_Friend.Items = FriendListData.FriendList;
                        return;
                    }
                }
            }
        }
    }

}
