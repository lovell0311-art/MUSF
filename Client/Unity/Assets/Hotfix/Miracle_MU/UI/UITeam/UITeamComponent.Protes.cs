using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 组队 协议
    /// </summary>
    public partial class UITeamComponent
    {

      

        /// <summary>
        /// 获取附近的队伍列表
        /// </summary>
        /// <returns></returns>
        public async ETVoid GetNaerTeam()
        {
            G2C_GetNearbyTeamList g2C_GetNearby = (G2C_GetNearbyTeamList)await SessionComponent.Instance.Session.Call(new C2G_GetNearbyTeamList { });
            if (g2C_GetNearby.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetNearby.Error.GetTipInfo());
            }
            else
            {
                if (g2C_GetNearby.TeamList.Count == 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"附近不存在队伍");
                    NearTeamScrollView.Items = null;
                    return;
                }
                var TempNearTeamList = new List<NearTeamInfo>();
                TempNearTeamList.Clear();
                foreach (var item in g2C_GetNearby.TeamList)
                {
                    TempNearTeamList.Add(new NearTeamInfo
                    {
                        uid = item.LeaderGameUserId,
                        TeamName = item.LeaderUserName,
                        TeamMemberCount = item.TeamMemberCount,
                        isInvite = TeamDatas.NearTeamList.Exists(r => r.uid == item.LeaderGameUserId) && TeamDatas.NearTeamList.Find(r => r.uid == item.LeaderGameUserId).isInvite
                    });
                }
                TeamDatas.NearTeamList.Clear();
                TeamDatas.NearTeamList = TempNearTeamList;
                TeamDatas.NearTeamList.Sort((m1,m2) => 
                {
                    return m1.TeamMemberCount.CompareTo(m2.TeamMemberCount);
                });
                NearTeamScrollView.Items=TeamDatas.NearTeamList;
                TempNearTeamList = null;
            }
        }

       
        /// <summary>
        /// 同意 或拒绝 玩家入队申请
        /// </summary>
        /// <param name="gameUserID">申请入队的玩家iD</param>
        /// <param name="IsAgree">是否同意</param>
        /// <param name="isAuto">是都自动回应 true.自动回应 对方将不会收到返回消息</param>
        /// <returns></returns>

        public async ETVoid ReplyPlayerApply(long gameUserID,bool IsAgree,bool isAuto) 
        {
            G2C_ReplyPlayerApply g2C_ReplyPlayer = (G2C_ReplyPlayerApply)await SessionComponent.Instance.Session.Call(new C2G_ReplyPlayerApply 
            {
             PlayerGameUserId=gameUserID,
             IsAgree=IsAgree,
             IsAuto=isAuto
            });
            if (g2C_ReplyPlayer.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ReplyPlayer.Error.GetTipInfo());
            }
            else
            { 
            
            }
        }
       
    }
}
