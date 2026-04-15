using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 通知 自己所在的队伍中，有其他玩家离开
    /// </summary>
    [MessageHandler]
    public class G2C_OtherPlayerLeaveMyTeam_notice_Handler : AMHandler<G2C_OtherPlayerLeaveMyTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_OtherPlayerLeaveMyTeam_notice message)
        {
            //离开的玩家
            foreach (var playerId in message.LeavedPlayerGameUserIds)
            {

             
               
                    UIMainComponent.Instance.HideTeamMember(playerId);
                    TeamDatas.RemovePlayer(playerId);
              //  Log.DebugYellow($"离开队伍：{playerId}");
            
            }
        }
    }
}