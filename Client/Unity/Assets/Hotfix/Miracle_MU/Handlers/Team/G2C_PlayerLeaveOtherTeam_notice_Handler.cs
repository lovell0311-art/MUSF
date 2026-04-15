using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 通知 广播其他玩家离开队伍
    /// </summary>
    [MessageHandler]
    public class G2C_PlayerLeaveOtherTeam_notice_Handler : AMHandler<G2C_PlayerLeaveOtherTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_PlayerLeaveOtherTeam_notice message)
        {
            ///遍历 离开队伍的玩家的ID
            foreach (var playerId in message.LeavedPlayerGameUserIds)
            {
               
               
                    UIMainComponent.Instance.HideTeamMember(playerId);
                    TeamDatas.RemovePlayer(playerId);
              
            }
        }
    }
}