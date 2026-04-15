using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 通知 自身离开队伍 收到消息后，记得退订聊天房间
    /// </summary>
    [MessageHandler]
    public class G2C_MySelfLeaveTeam_notice_Handler : AMHandler<G2C_MySelfLeaveTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_MySelfLeaveTeam_notice message)
        {
            UIMainComponent.Instance.LeaveTeam();
            TeamDatas.MyTeamState = null;//清空 自己的队伍状态
            TeamDatas.OtherTeamMemberStatusList.Clear();//清理 队伍中的成员
            TeamDatas.ApplyPlayersList.Clear();// 清理 队伍申请 列表
            TeamDatas.NearTeamList.Clear();//清空附近队伍
            UIMainComponent.Instance.ChangeTeamInfo();
            LeaveChatRoom().Coroutine();

            ///离开队伍聊天房间
            static async ETVoid LeaveChatRoom() 
            {
                G2C_LeaveChatRoom g2C_Leave = (G2C_LeaveChatRoom)await SessionComponent.Instance.Session.Call(new C2G_LeaveChatRoom 
                {
                 ChatRoomID=TeamDatas.ChatRoomId
                });
                if (g2C_Leave.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,g2C_Leave.Error.GetTipInfo());
                }
            }
        }
    }
}
