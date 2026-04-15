using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System;

namespace ETHotfix
{


    public enum E_PanelType
    {
        Task = 0,//任务
        Team = 1//队伍
    }

    

    /// <summary>
    /// 队伍 任务 模块
    /// </summary>
    public partial class UIMainComponent
    {

        public GameObject TeamPanel;
        GameObject panel;
        Transform Team, Task/*, Enhance*/;
        Toggle openTog, Team_Tog, /*Enhance_Tog;//,*/ Task_Tog;
        Transform btns;
        Text info;

        public void Init_Team()
        {
            TeamPanel = ReferenceCollector_Main.GetGameObject("TeamPane");
            ReferenceCollector referenceCollectorData_Team = TeamPanel.GetReferenceCollector();
            panel = referenceCollectorData_Team.GetImage("panel").gameObject;
            InitTask();
            InitTeam();
            InitOpenTog();

            btns = referenceCollectorData_Team.GetGameObject("btns").transform;
            btns.gameObject.SetActive(false);

            ///初始化 面板 开关Tog
            void InitOpenTog() 
            {
                openTog = referenceCollectorData_Team.GetToggle("Open_Tog");
                openTog.onValueChanged.AddSingleListener(Show_HidePanel);
                openTog.isOn = true;
            }

            ///初始化 任务 组件
            void InitTask() 
            {
                Task_Tog = referenceCollectorData_Team.GetToggle("Task_Tog");
                Task_Tog.onValueChanged.AddSingleListener((value) => RefreShPanel(value, E_PanelType.Task));
                Task = referenceCollectorData_Team.GetGameObject("Task").transform;//任务
               // Task.gameObject.SetActive(true);
                //Enhance = referenceCollectorData_Team.GetImage("Enhance").transform;//强化
                //Enhance.gameObject.SetActive(false);
            }
            ///初始化 队伍

            void InitTeam() 
            {
                Team_Tog = referenceCollectorData_Team.GetToggle("Team_Tog");
                Team_Tog.onValueChanged.AddSingleListener((value) => RefreShPanel(value, E_PanelType.Team));
                Team = referenceCollectorData_Team.GetGameObject("Team").transform;//创建队伍
                Team.gameObject.SetActive(true);
                ///打开队伍面板
                referenceCollectorData_Team.GetButton("NearTeamBtn").onClick.AddSingleListener(()=>UIComponent.Instance.VisibleUI(UIType.UITeam, "GetNearTeam"));
                ///创建队伍
                referenceCollectorData_Team.GetButton("CeatTeamBtn").onClick.AddSingleListener(()=> 
                {

                    //判断是否已经 拥有队伍
                    if (TeamDatas.MyTeamState != null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint,"你已拥有 不能创建队伍");
                        return;
                    }

                    CreatTeam().Coroutine();

                });

              //  Team.gameObject.SetActive(false);

                ///是否已经加入队伍
                info = referenceCollectorData_Team.GetText("Info");

                //是否自动接收组队
                referenceCollectorData_Team.GetToggle("IsAutoTream_Tog").onValueChanged.AddSingleListener((value) => 
                {
                    TeamDatas.IsAutoAcceptTeam = value;
                });
                referenceCollectorData_Team.GetToggle("IsAutoTream_Tog").isOn = TeamDatas.IsAutoAcceptTeam;

                //Team_Tog.isOn = true;//默认显示队伍
                //Task_Tog.isOn = false;//默认显示任务
            }
            ///请求创建队伍
            async ETVoid CreatTeam() 
            {
                G2C_CreateTeam g2C_Create = (G2C_CreateTeam)await SessionComponent.Instance.Session.Call(new C2G_CreateTeam { });
                if (g2C_Create.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Create.Error.GetTipInfo());
                }
                else
                {
                    openTog.isOn = false;
                   
                    //成功会推送G2C_MySelfEnterTeam_notice
                }
            }
        }

        /// <summary>
        /// 打开 任务、组队面板
        /// </summary>
        /// <param name="isOn"></param>

        public void Show_HidePanel(bool isOn)
        {
            openTog.transform.GetChild(0).GetComponent<Image>().enabled = !isOn;
            panel.SetActive(!isOn);
            //Task_Tog.isOn = isOn;
        }
        /// <summary>
        /// Task、Team面板
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="panelType"></param>
        public void RefreShPanel(bool isOn, E_PanelType panelType)
        {
            //Team.gameObject.SetActive(true);
            //return;
            switch (panelType)
            {
                case E_PanelType.Task:
                    Task.gameObject.SetActive(isOn);
                    break;
                case E_PanelType.Team:
                    Team.gameObject.SetActive(isOn);
                    ChangeTeamInfo();
                    break;
            }
        }
        /// <summary>
        /// 是否拥有队伍
        /// </summary>
        public void ChangeTeamInfo()
        {
            info.text = TeamDatas.MyTeamState != null ? "已拥有队伍" : "当前暂未加入队伍";
        }


    }
}
