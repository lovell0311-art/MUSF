using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [MessageHandler]
    public class G2C_AgreeToAddFriend_notice_Hander : AMHandler<G2C_AgreeToAddFriend_notice>
    {
        protected override void Run(ETModel.Session session, G2C_AgreeToAddFriend_notice message)
        {
            UIComponent.Instance.VisibleUI(UIType.UIHint, $"和玩家{message.CharName}成为好友");
            //如果在当前好友界面且没有在输入消息,刷新一下
            if (UIComponent.Instance.Get(UIType.UIFirendList)?.GetComponent<UIFriendListComponent>()?.Cur_E_Friends == E_FriendsTogNewType.Friend
                && string.IsNullOrEmpty(UIComponent.Instance.Get(UIType.UIFirendList)?.GetComponent<UIFriendListComponent>()?.chatInput.text))
            {
                UIComponent.Instance.Get(UIType.UIFirendList).GetComponent<UIFriendListComponent>()?.RequestFriendList(4).Coroutine();
            }
        }
    }

}
