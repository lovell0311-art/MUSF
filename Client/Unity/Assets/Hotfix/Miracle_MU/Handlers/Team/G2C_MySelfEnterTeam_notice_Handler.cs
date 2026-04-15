using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 自身进入队伍
    /// 如需队伍聊天时，HotfixChatMessage.proto->C2G_EnterChatRoom 通过此消息订阅 ChatRoomID 传递的房间。
    /// 房间id 需客户端进行记录,用于离开队伍时退订
    /// </summary>
    [MessageHandler]
    public class G2C_MySelfEnterTeam_notice_Handler : AMHandler<G2C_MySelfEnterTeam_notice>
    {
        protected override void Run(ETModel.Session session, G2C_MySelfEnterTeam_notice message)
        {

            foreach (PlayerInTeamStatus player in message.OtherPlayerStatus)
            {
             
                TeamMateInfo mateInfo = new TeamMateInfo
                {
                    GameUserId = player.GameUserId,
                    UserName = player.UserName,
                    IsCaptain = player.IsCaptain,
                    Status = new TeamMateProperty 
                    {
                    HP=player.Status.HP,
                    MPMax=player.Status.MPMax,
                    HPMax=player.Status.HPMax,
                    MP=player.Status.MP,
                    Level=player.Status.Level,
                    },
                };
                TeamDatas.AddPlayer(mateInfo);
             
                UIMainComponent.Instance.ShowTeamMember(mateInfo);
            }

            TeamDatas.MyTeamState = new MyTeamInfo
            {
                GameUserId = message.SelfStatus.GameUserId,
                UserName = message.SelfStatus.UserName,
                IsCaptain = message.SelfStatus.IsCaptain,
            };//缓存本地玩家的队伍信息
            UIMainComponent.Instance.ChangeTeamInfo();
            EnterChatRoom().Coroutine();


            ///请求进入聊天房间
            async ETVoid EnterChatRoom()
            {
               
                G2C_EnterChatRoom g2C_EnterChatRoom = (G2C_EnterChatRoom)await SessionComponent.Instance.Session.Call(new C2G_EnterChatRoom
                {
                    ChatRoomID = message.ChatRoomID,
                });
                if (g2C_EnterChatRoom.Error != 0)
                {
                    Log.DebugRed($"请求进入聊天房间错误:{g2C_EnterChatRoom.Error.GetTipInfo()}");
                }
                else
                {
                  
                    TeamDatas.ChatRoomId = message.ChatRoomID;
                    ChatMessageDataManager.valuePairs[E_ChatType.Team] = message.ChatRoomID;

                }
            }
        }
    }
}