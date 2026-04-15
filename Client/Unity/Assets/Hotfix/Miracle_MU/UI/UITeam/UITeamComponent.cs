using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{

    public enum E_TeamType 
    {
     NearTeam=0,//附近队伍
     NearPlayer,//附近玩家
     MyFriends,//我的好友
     MyWar,//我的战盟
     Apply//申请列表
    }

    
    
    /// <summary>
    /// 组队 
    /// </summary>
    public partial class UITeamComponent : Component,IUGUIStatus
    {

        GameObject NearTeamPanel, NearPlayerPanel, ApplyPanel;
        public string curType;
        Toggle applytoggle,InvitePlayerTog;//申请 列表 

        public void Init_Show() 
        {
            switch (curType)
            {
                case "InvitePlayer":
                    //显示 邀请好友界面
                    InvitePlayerTog.isOn = true;
                    break;
                case "GetNearTeam"://附近队伍
                    GetNaerTeam().Coroutine();
                    break;
                default:
                    break;
            }
        }

        public void Init() 
        {
            ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UITeam));//打开组队面板

            NearTeamPanel = collector.GetImage("NearTeamPanel").gameObject;//附近队伍 面板
            NearPlayerPanel = collector.GetImage("NearPlayersPanel").gameObject;// 附近 玩家、我的好友、我的战盟
            ApplyPanel = collector.GetImage("ApplyPanel").gameObject;//申请 面板


            InitTog();

            applytoggle.gameObject.SetActive(TeamDatas.MyTeamState!=null&&TeamDatas.MyTeamState.IsCaptain);//队长   才显示 申请列表

            //初始化Tog
            void InitTog()
            {
                Transform Togs = collector.GetGameObject("Togs").transform;
                for (int i = 0, length=Togs.childCount; i < length; i++)
                {
                    E_TeamType type = (E_TeamType)i;
                    
                    Togs.GetChild(i).GetComponent<Toggle>().onValueChanged.AddSingleListener((value) => TogEvent(value, type));
                }
                applytoggle = collector.GetToggle("ApplyList_Tog"); //申请 Tog
                InvitePlayerTog = collector.GetToggle("NearPlayers_Tog");
            }
        }

        public void OnInVisibility()
        {
           
        }

        public void OnVisible(object[] data)
        {
            if (data.Length > 0)
            {
                curType = data[0].ToString();
            }
        }

        public void OnVisible()
        {
           
        }

        public void TogEvent(bool isOn,E_TeamType teamType)
        {
            if (!isOn) return;

            NearTeamPanel.SetActive(teamType==E_TeamType.NearTeam);
            NearPlayerPanel.SetActive(teamType==E_TeamType.NearPlayer|| teamType == E_TeamType.MyFriends|| teamType == E_TeamType.MyWar);
            ApplyPanel.SetActive(teamType==E_TeamType.Apply);

            switch (teamType)
            {
                case E_TeamType.NearTeam:
                    GetNaerTeam().Coroutine();
                    break;
                case E_TeamType.NearPlayer:
                    InitNearPlayers();//获取附近的玩家
                    break;
                case E_TeamType.MyFriends:
                    Init_MyFriends();
                    break;
                case E_TeamType.MyWar:
                    Init_MyWarMembers();
                    break;
                case E_TeamType.Apply:
                  ShowApplyList();
                    break;
                default:
                    break;
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            NearPlayersScrollView.Dispose();
            NearTeamScrollView.Dispose();
            ApplyPlayersScrollView.Dispose();

           
        }

    }
}