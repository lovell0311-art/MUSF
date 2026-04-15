using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_AddFriendsnotice_notice_Handler : AMHandler<G2C_AddFriendsnotice_notice>       
    {
        protected override void Run(ETModel.Session session, G2C_AddFriendsnotice_notice message)
        {
          //  Log.Debug("有玩家请求添加好友:" + message.FriendInfo.CharName);
            if (!FriendListData.ApplyList.Exists((f) => f.NickName == message.FriendInfo.CharName))
            {
                FriendListData.ApplyList.Add(new FriendInfo()
                {
                    NickName = message.FriendInfo.CharName,
                    UUID = message.FriendInfo.GameUserId,
                    State = (message.FriendInfo.IState == 0)? "离线" : "在线",
                    TimeDate = TimeHelper.ClientNowSeconds(),
                    Level = 367,
                });
            }

            //红点提示
            RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Friend_AddFirend_FirendApply, 1);
            UIMainComponent.Instance?.RedDotFriendCheack();
            //UIComponent.Instance.Get(UIType.UIFirendList)?.GetComponent<UIFriendListComponent>().RedDotFriendCheack();
        }
       
    }

}
