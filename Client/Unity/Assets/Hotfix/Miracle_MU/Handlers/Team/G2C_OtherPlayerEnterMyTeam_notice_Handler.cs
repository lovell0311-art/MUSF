using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 通知 自己所在的队伍中，有其他玩家加入或状态改变时推送
    /// </summary>
    [MessageHandler]
    public class G2C_OtherPlayerEnterMyTeam_notice_Handler : AMHandler<G2C_OtherPlayerEnterMyTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_OtherPlayerEnterMyTeam_notice message)
        {
            // 进入的玩家或玩家状态改变(状态改变包括自己,比如改变队长)
            foreach (PlayerInTeamStatus playerInfo in message.EnteredPlayerStatus)
            {
                if (playerInfo.GameUserId == UnitEntityComponent.Instance.LocaRoleUUID) //是本地玩家
                {
                    //是队长
                    TeamDatas.MyTeamState = new MyTeamInfo
                    {
                        GameUserId = playerInfo.GameUserId,
                        UserName = playerInfo.UserName,
                        IsCaptain = playerInfo.IsCaptain
                    };
                }

                TeamMateInfo mateInfo = new TeamMateInfo
                {
                    GameUserId = playerInfo.GameUserId,
                    UserName = playerInfo.UserName,
                    IsCaptain = playerInfo.IsCaptain,
                    Status = new TeamMateProperty
                    {
                        HP = playerInfo.Status.HP,
                        HPMax = playerInfo.Status.HPMax,
                        MP = playerInfo.Status.MP,
                        MPMax = playerInfo.Status.MPMax,
                        Level = playerInfo.Status.Level,
                    },
                };
           
                TeamDatas.AddPlayer(mateInfo);
                UIMainComponent.Instance.ShowTeamMember(mateInfo);

            }
        }
    }
}