using ETModel;
using System;
using System.Collections.Generic;

namespace ETHotfix
{

    /// <summary>
    /// 队伍数据
    /// </summary>
    public  class TeamDatas
    {
        public static int TeamMembersMaxCount = 5;//队伍的 最大人数
        public static List<NearTeamInfo> NearTeamList = new List<NearTeamInfo>();//附近的队伍 信息
        public static MyTeamInfo MyTeamState = null;//本地玩家的队伍状态
        public static List<TeamMateInfo> OtherTeamMemberStatusList = new List<TeamMateInfo>();//队伍中的其他玩家
        public static bool IsAutoAcceptTeam = false;//是否自动接收组队
        public static Action action;
        public static long ChatRoomId = -1;//聊天房间ID
        public static List<OtherPlayerInfo> ApplyPlayersList=new List<OtherPlayerInfo>();//好友申请 列表
     
        /// <summary>
        /// 是否是队长
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public static bool IsCaptain(long playerId)
        {
            bool iscaption = false;
            foreach (var TeamMember in OtherTeamMemberStatusList)
            {
                if (TeamMember.GameUserId == playerId)
                {
                    iscaption= TeamMember.IsCaptain;
                    break;
                }
            }
            return iscaption;

        }

        /// <summary>
        /// 从队伍中移除 该成员
        /// </summary>
        /// <param name="playerId">被移除的玩家的ID</param>
        public static void RemovePlayer(long playerId)
        {
            if (OtherTeamMemberStatusList.Exists(r => r.GameUserId == playerId))
            {
                var Player = OtherTeamMemberStatusList.Find(r => r.GameUserId == playerId);
                OtherTeamMemberStatusList.Remove(Player);
                action?.Invoke();
            }
        }
        /// <summary>
        /// 向队伍中添加 该成员
        /// </summary>
        /// <param name="inTeamStatus"></param>
        public static void AddPlayer(TeamMateInfo inTeamStatus)
        {
            if (!OtherTeamMemberStatusList.Exists(r => r.GameUserId == inTeamStatus.GameUserId))
            {
                OtherTeamMemberStatusList.Add(inTeamStatus);
            }
            else
            {
                ///属性变动
                var playerInTeam = OtherTeamMemberStatusList.Find(r => r.GameUserId == inTeamStatus.GameUserId);
                playerInTeam.IsCaptain = inTeamStatus.IsCaptain;
                playerInTeam.GameUserId = inTeamStatus.GameUserId;
                playerInTeam.UserName = inTeamStatus.UserName;
                playerInTeam.Status = inTeamStatus.Status;
            }
        }

        public static void Clear()
        {
            NearTeamList.Clear();
            OtherTeamMemberStatusList.Clear();
            ApplyPlayersList.Clear();
           
            if (MyTeamState != null)
                LeaveTeam().Coroutine();

            async ETVoid LeaveTeam()
            {
                Log.DebugBrown($"请求离开队伍");
                //离开队伍  没有参数
                G2C_LeaveTheTeam g2C_Leave = (G2C_LeaveTheTeam)await SessionComponent.Instance.Session.Call(new C2G_LeaveTheTeam
                {

                });
                if (g2C_Leave.Error != 0)
                {
                   // UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Leave.Error.GetTipInfo());
                   // Log.DebugRed($"{g2C_Leave.Error}->{g2C_Leave.Error.GetTipInfo()}");
                }
                else 
                {
                    MyTeamState = null;
                }
            }
            MyTeamState = null;
        }
    }
    /// <summary>
    /// 其他玩家的信息
    /// </summary>
    public class OtherPlayerInfo
    {
        public long roleUUId;//uuid
        public string roleName;//名字
        public int roleLev;//等级
        public int OccupationLevel;//转职等级
        public int roleType;//职业类型
        public string warName;//战盟名
        public long TeamId;//加入的队伍id 0.未加入队伍 0+.队伍id
        public bool isInvite;//是否已经邀请
    }
    /// <summary>
    /// 自己所在队伍 的状态信息
    /// </summary>
    public class MyTeamInfo
    {
        public long GameUserId;//自己的 UUID
        public string UserName;//自己的 名字
        // 是队长
        public bool IsCaptain;//自己是否是队长
    }

    /// <summary>
    /// 附近的队伍
    /// </summary>
    public class NearTeamInfo
    {
        public long uid;//队伍的UID
        public string TeamName;//队伍名字
        public int TeamMemberCount;//队伍人数
        public bool isInvite;//是否已经申请
    }
    /// <summary>
    /// 队友信息
    /// </summary>
    public class TeamMateInfo
    {
        public long GameUserId;//队友的UUID
        public string UserName;//队友的 名字
        public bool IsCaptain;//是否是 队长
        public TeamMateProperty Status;//队友的属性
    }
    /// <summary>
    /// 队友的 基本上属性
    /// </summary>
    public class TeamMateProperty
    {
        public int HPMax;//最大生命值
        public int HP;//当前生命值
        public int MPMax;//最大MP
        public int MP;//当前Mp
        public int Level;//等级
    }

   
}