using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    [EventMethod(typeof(TeamManageComponent), EventSystemType.INIT)]
    public class TeamManageComponentEventOnInit : ITEventMethodOnInit<TeamManageComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(TeamManageComponent b_Component)
        {
            b_Component.OnInit();
        }
    }

    public static partial class TeamManageComponentSystem
    {
        public static void OnInit(this TeamManageComponent b_Component)
        {
            
        }
        /// <summary>
        /// 进入队伍时初始化 并发送自身进入队伍推送
        /// </summary>
        /// <param name="b_Component"></param>
        public static void Init(this TeamManageComponent b_Component,long teamID,bool isCaptain)
        {
            
        }

        /// <summary>
        /// 添加一个Player到指定队伍
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="teamID"></param>
        /// <param name="targetPlayer"></param>
        /// <returns>true-成功  false-满人等原因，无法添加</returns>
        public static bool AddPlayerInTeam(this TeamManageComponent b_Component , long teamID, Player targetPlayer)
        {
            bool isCapital = false;
            if (b_Component.AllTeamDic.TryGetValue(teamID, out var roomPlayerDict) == false)
            {
                roomPlayerDict = b_Component.AllTeamDic[teamID] = new Dictionary<long, Player>();
                isCapital = true;   //创建队伍的为队长
            }
            else if (roomPlayerDict.Count == 0)
            {
                isCapital = true;  //避免roomPlayerDict未清除干净的情况
            }
            //检查队伍是否已满
            if (roomPlayerDict.Count >= TeamManageComponent.MaxMemberCount)
            {
                return false;
            }
            if (roomPlayerDict.TryGetValue(targetPlayer.GameUserId, out Player mResult) == false)
            {
                roomPlayerDict.Add(targetPlayer.GameUserId, targetPlayer);
                var component = targetPlayer.GetCustomComponent<TeamComponent>();
                if (component == null)
                {
                    component = targetPlayer.AddCustomComponent<TeamComponent>();
                }
                component.Init(teamID, isCapital);

                //同步给其他队员
                foreach (var item in roomPlayerDict)
                {
                    if (item.Value.GameUserId != targetPlayer.GameUserId)
                    {
                        var notice = new G2C_OtherPlayerEnterMyTeam_notice();
                        notice.EnteredPlayerStatus.Add(component.GetTeamStatus());
                        item.Value.Send(notice);
                    }
                }
                //同步给附近玩家
                var notice2 = new G2C_PlayerEnterOtherTeam_notice();
                notice2.EnteredPlayersGameUserIds.Add(targetPlayer.GameUserId);
                targetPlayer.NotifyAroundPlayer(notice2);
            }
            return true;
        }

        /// <summary>
        /// Player离开队伍 所有队伍离开后删除队伍
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="targetPlayer"></param>
        /// <returns></returns>
        public static bool LeaveTeam(this TeamManageComponent b_Component,long teamID, Player targetPlayer, bool sendNotice = true)
        {
            var mData = b_Component.GetAllByTeamID(teamID);
            if (mData != null)
            {
                if (mData.TryGetValue(targetPlayer.GameUserId, out Player mUser))
                {
                    var targetTeamComponent = targetPlayer.GetCustomComponent<TeamComponent>();
                    mData.Remove(targetPlayer.GameUserId);
                    if (sendNotice)
                    {
                        targetTeamComponent?.OnLeaveTeam();
                    }
                    //判断离队的是不是队长，是队长的话让位给队员
                    if (targetTeamComponent != null && targetTeamComponent.IsCaptain && mData.Count > 0)
                    {
                        var members = mData.Values.ToList();
                        members[0].GetCustomComponent<TeamComponent>().SetCaptain(true);
                    }
                    targetPlayer.RemoveCustomComponent<TeamComponent>();
                    
                    //同步给队员离开队伍消息
                    foreach (var item in mData)
                    {
                        var notice = new G2C_OtherPlayerLeaveMyTeam_notice();
                        notice.LeavedPlayerGameUserIds.Add(targetPlayer.GameUserId);
                        item.Value.Send(notice);
                    }
                    //同步给附近玩家离开队伍消息
                    var notice2 = new G2C_PlayerLeaveOtherTeam_notice();
                    notice2.LeavedPlayerGameUserIds.Add(targetPlayer.GameUserId);
                    targetPlayer.NotifyAroundPlayer(notice2);

                    if (mData.Count <= 0)
                    {
                        b_Component.AllTeamDic.Remove(teamID);  //队伍内没有人员则删除队伍
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Player下线销毁时离队
        /// </summary>
        public static void PlayerDispose(this TeamManageComponent b_Component, Player mPlayer)
        {
            Log.Debug("================玩家离线退出组队");
            TeamComponent component = mPlayer.GetCustomComponent<TeamComponent>();
            if (component != null && component.TeamID > 0)
            {
                b_Component.LeaveTeam(component.TeamID, mPlayer, false);
            }
        }
    }
}