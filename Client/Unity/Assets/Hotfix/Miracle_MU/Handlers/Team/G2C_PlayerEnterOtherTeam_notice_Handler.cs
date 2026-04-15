using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    /// <summary>
    ///通知 广播其他玩家进入队伍
    // 可视范围内，有玩家进入队伍时，此消息会到达
    // 刚进入视野的玩家主角，如在队伍中。此消息会到达
    // 用于区分，玩家是否在队伍中。
    /// </summary>
    [MessageHandler]
    public class G2C_PlayerEnterOthesrTeam_notice_Handler : AMHandler<G2C_PlayerEnterOtherTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_PlayerEnterOtherTeam_notice message)
        {
            foreach (var playerId in message.EnteredPlayersGameUserIds)
            {
              //  Log.DebugBrown($"广播其他玩家进入队伍:{playerId} 是否是本地玩家：{playerId==UnitEntityComponent.Instance.LocaRoleUUID}");
                RoleEntity entity = UnitEntityComponent.Instance.Get<RoleEntity>(playerId);
                entity.IsInTeam=true;
            }
        }
    }
}
