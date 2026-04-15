using System;
using System.Collections;
using System.Diagnostics;

using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 检查属性，当变动时，通知队友
    /// </summary>
    [Timer(TimerType.PlayerInTeamPropertyChangeNotice)]
    public class PlayerInTeamPropertyChangeNoticeTimer : ATimer<TeamComponent>
    {
        public override void Run(TeamComponent self)
        {
            if(self.__GamePlayer == null)
            {
                self.__GamePlayer = self.mPlayer.GetCustomComponent<GamePlayer>();
                if (self.__GamePlayer == null) return;
            }

            if (self.LastHPMax != self.__GamePlayer.GetNumerial(E_GameProperty.PROP_HP_MAX))
            {
                self.LastHPMax = self.__GamePlayer.GetNumerial(E_GameProperty.PROP_HP_MAX);
                self.IsChanged = true;
            }
            if (self.LastHP != self.__GamePlayer.GetNumerial(E_GameProperty.PROP_HP))
            {
                self.LastHP = self.__GamePlayer.GetNumerial(E_GameProperty.PROP_HP);
                self.IsChanged = true;
            }
            if (self.LastMPMax != self.__GamePlayer.GetNumerial(E_GameProperty.PROP_MP_MAX))
            {
                self.LastMPMax = self.__GamePlayer.GetNumerial(E_GameProperty.PROP_MP_MAX);
                self.IsChanged = true;
            }
            if (self.LastMP != self.__GamePlayer.GetNumerial(E_GameProperty.PROP_MP))
            {
                self.LastMP = self.__GamePlayer.GetNumerial(E_GameProperty.PROP_MP);
                self.IsChanged = true;
            }
            if(self.IsChanged == true)
            {
                self.PropValueChangeNotice();
                self.IsChanged = false;
            }
        }
    }

    [EventMethod(typeof(TeamComponent), EventSystemType.INIT)]
    public class TeamComponentEventOnInit : ITEventMethodOnInit<TeamComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(TeamComponent b_Component)
        {
            b_Component.OnInit();
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000, TimerType.PlayerInTeamPropertyChangeNotice, b_Component);
        }
    }

    [EventMethod(typeof(TeamComponent), EventSystemType.DISPOSE)]
    public class TeamComponentEventOnDispose : ITEventMethodOnDispose<TeamComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnDispose(TeamComponent b_Component)
        {
            Log.Info("TeamComponent.OnDispose");
            ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
        }
    }

    public static partial class TeamComponentSystem
    {
        public static void OnInit(this TeamComponent b_Component)
        {
            
        }
        /// <summary>
        /// 自身离开队伍时调用
        /// </summary>
        /// <param name="b_Component"></param>
        public static void OnLeaveTeam(this TeamComponent b_Component)
        {
            b_Component.Parent.Send(new G2C_MySelfLeaveTeam_notice());
        }
        /// <summary>
        /// 进入队伍时初始化 并发送自身进入队伍推送
        /// </summary>
        /// <param name="b_Component"></param>
        public static void Init(this TeamComponent b_Component,long teamID,bool isCaptain)
        {
            b_Component.TeamID = teamID;
            b_Component.IsCaptain = isCaptain;
            //发送自身进入队伍推送
            var notice = new G2C_MySelfEnterTeam_notice()
            {
                ChatRoomID = teamID,
                SelfStatus = b_Component.GetTeamStatus(),
            };
            TeamManageComponent manager = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
            var OtherPlayerDict = manager.GetAllByTeamID(teamID);
            foreach (var item in OtherPlayerDict)
            {
                TeamComponent otherComponent = item.Value.GetCustomComponent<TeamComponent>();
                if (otherComponent != null)
                {
                    notice.OtherPlayerStatus.Add(otherComponent.GetTeamStatus());
                }
            }
            b_Component.Parent.Send(notice);
        }

        public static PlayerInTeamStatus GetTeamStatus(this TeamComponent b_Component)
        {
            var result = new PlayerInTeamStatus();
            result.GameUserId = b_Component.Parent.GameUserId;
            result.IsCaptain = b_Component.IsCaptain;
            result.Status = b_Component.GetTeamProperty();
            result.UserName = b_Component.Parent.GetCustomComponent<GamePlayer>().Data.NickName;
            
            return result;
        }

        public static PlayerInTeamProperty GetTeamProperty(this TeamComponent b_Component)
        {
            GamePlayer gamePlayer = b_Component.Parent.GetCustomComponent<GamePlayer>();
            //var result = new PlayerInTeamProperty()
            //{
            //    HP = gamePlayer.GetNumerial(E_GameProperty.PROP_HP),
            //    HPMax = gamePlayer.GetNumerial(E_GameProperty.PROP_HP_MAX),
            //    Level = gamePlayer.Data.Level,
            //    MP = gamePlayer.GetNumerial(E_GameProperty.PROP_MP),
            //    MPMax = gamePlayer.GetNumerial(E_GameProperty.PROP_MP_MAX),
            //};
            var result = new PlayerInTeamProperty();

            result.HP = gamePlayer.GetNumerial(E_GameProperty.PROP_HP);
            result.HPMax = gamePlayer.GetNumerial(E_GameProperty.PROP_HP_MAX);
            result.Level = gamePlayer.Data.Level;
            result.MP = gamePlayer.GetNumerial(E_GameProperty.PROP_MP);
            result.MPMax = gamePlayer.GetNumerial(E_GameProperty.PROP_MP_MAX);
            
            return result;
        }

        /// <summary>
        /// 设置队长并推送状态给全队
        /// </summary>
        /// <param name="b_Component"></param>
        public static void SetCaptain(this TeamComponent b_Component,bool isCapitain)
        {
            b_Component.IsCaptain = isCapitain;
            var teamManager = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
            var members = teamManager.GetAllByTeamID(b_Component.TeamID);
            var notice = new G2C_OtherPlayerEnterMyTeam_notice();
            notice.EnteredPlayerStatus.Add(b_Component.GetTeamStatus());
            foreach (var item in members)
            {
                item.Value.Send(notice);
            }
        }

        /// <summary>
        /// 当自己的HP、MP、Level有变化的时候，调用这个方法推送给全队队员属性变化
        /// mPlayer.GetCustomComponent<TeamComponent>()?.PropValueChangeNotice()
        /// </summary>
        /// <param name="b_Component"></param>
        public static void PropValueChangeNotice(this TeamComponent b_Component)
        {
            var teamManager = Root.MainFactory.GetCustomComponent<TeamManageComponent>();
            var members = teamManager.GetAllByTeamID(b_Component.TeamID);
            var notice = new G2C_PlayerPropChangeInTheTeam_notice() { 
                ChangedPlayerGameUserId = b_Component.Parent.GameUserId,
                ChangedPlayerStatus = b_Component.GetTeamProperty(),
            };
            foreach (var item in members)
            {
                item.Value.Send(notice);
            }
        }
    }
}