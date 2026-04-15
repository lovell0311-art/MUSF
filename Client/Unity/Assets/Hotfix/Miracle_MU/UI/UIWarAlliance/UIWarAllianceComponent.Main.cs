using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

using UnityEngine.UI;

namespace ETHotfix
{
    public enum E_WarMainType 
    {
     Info,//主界面
     Members,//成员面板
     Apply//审核
    }
    /// <summary>
    /// 战盟 信息
    /// </summary>
    public partial class UIWarAllianceComponent
    {

        ReferenceCollector collector_Main;
        GameObject Info, Members, Apply;
        GameObject WarApply_Toggle;//申请列表 按钮 TODO 盟主 与副盟主 才显示 申请列表按按钮
        public Image WarAlliance_WarApply;
        public void Init_WarMainInfo() 
        {
            collector_Main = WarMainPanel.GetReferenceCollector();
            WarApply_Toggle = collector_Main.GetToggle("WarApply_Toggle").gameObject;
            collector_Main.GetToggle("WarInfo_Toggle").onValueChanged.AddSingleListener((value) => { TogEvent(value, E_WarMainType.Info); });
            collector_Main.GetToggle("WarMember_Toggle").onValueChanged.AddSingleListener((value) => { TogEvent(value, E_WarMainType.Members); });
            collector_Main.GetToggle("WarApply_Toggle").onValueChanged.AddSingleListener((value) => { TogEvent(value, E_WarMainType.Apply); });
            WarAlliance_WarApply = collector_Main.GetImage("WarAlliance_WarApply");

            WarRedCheak();

            Info = collector_Main.GetGameObject("Info");
            Members = collector_Main.GetGameObject("Members");
            Apply = collector_Main.GetGameObject("Apply");

            ShowMainPanel(E_WarMainType.Info);

            Init_Info();
            Init_Member();
            Init_Apply();
            LookWarInfo();
        }
        public void WarRedCheak()
        {
            WarAlliance_WarApply.gameObject.SetActive(RedDotManagerComponent.RedDotManager.GetRedDotCount(E_RedDotDefine.Root_WarAlliance_WarApply) > 0);
            UIMainComponent.Instance.RedDotFriendCheack();
        }
        public void Apply_isOn(int Post)
        {
            collector_Main.GetToggle("WarApply_Toggle").gameObject.SetActive(Post == 2 || Post == 3);
        }

        private void TogEvent(bool isOn,E_WarMainType type) 
        {
            if (isOn)
            {
                ShowMainPanel(type);
            }
        }

        public void ShowMainPanel(E_WarMainType type)
        {
            Info.SetActive(type==E_WarMainType.Info);
            Members.SetActive(type==E_WarMainType.Members);
            Apply.SetActive(type==E_WarMainType.Apply);
            switch (type)
            {
                case E_WarMainType.Info:
                    
                    break;
                case E_WarMainType.Members:
                    //请求战盟成员
                    OpenMemberListAsync(0).Coroutine();
                    break;
                case E_WarMainType.Apply:
                    RefreachApply();
                    break;
                default:
                    break;
            }
        }

        public void RefreachApply()
        {
            if (Apply.activeSelf)
            {
                //请求战盟申请成员
                RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_WarAlliance_WarApply, 0);
                WarRedCheak();
                OpenMemberListAsync(1).Coroutine();
            }
        }

    }
}