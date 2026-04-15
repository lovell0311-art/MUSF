using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{

    public enum E_PostType 
    {
     MengZhu,//盟主
     FuMengZhu,//副盟主
     XiaoDuiZhang//队长
    }
    /// <summary>
    ///  战盟 职位设置
    /// </summary>
    public partial class UIWarAllianceComponent
    {

        ReferenceCollector collector_PostSet;
        Text TipInfo;
        public GameObject PostSetPanel;
        int CurPostIndex = -1;
        public void Init_PostSet() 
        {
            PostSetPanel = collector.GetImage("PostSet").gameObject;
            collector_PostSet = collector.GetImage("PostSet").gameObject.GetReferenceCollector();
            TipInfo = collector_PostSet.GetText("Info");
            collector_PostSet.GetToggle("Toggle").onValueChanged.AddSingleListener((value => { PostSetTogEvent(value,E_PostType.MengZhu); }));
            collector_PostSet.GetToggle("Toggle_1").onValueChanged.AddSingleListener((value => { PostSetTogEvent(value,E_PostType.FuMengZhu); }));
            collector_PostSet.GetToggle("Toggle_2").onValueChanged.AddSingleListener((value => { PostSetTogEvent(value,E_PostType.XiaoDuiZhang); }));
            TipInfo.text = string.Empty;
            //确定 任命职位
            collector_PostSet.GetButton("SureBtn").onClick.AddSingleListener(() =>
            {
                if (CurPostIndex != -1)
                {
                    Isremoved = false;
                    WarAllianceAppointmentAsync(CurChooseWarMemberInfo.UUID,CurPostIndex).Coroutine();
                    PostSetPanel.SetActive(false);
                }
            });
            //关闭职位 任命面板
            collector_PostSet.GetButton("CancelBtn").onClick.AddSingleListener(() => PostSetPanel.SetActive(false));

            PostSetPanel.SetActive(false);
        }

        public void PostSetTogEvent(bool isOn, E_PostType type) 
        {
            if (!isOn) return;
            switch (type)
            {
                case E_PostType.MengZhu:
                    TipInfo.text = $"任命 <color=green>{CurChooseWarMemberInfo.Name}</color> 为<color=red> 盟主 </color> 吗？";
                    CurPostIndex = 3;
                    break;
                case E_PostType.FuMengZhu:
                    TipInfo.text = $"任命 <color=green>{CurChooseWarMemberInfo.Name}</color> 为<color=red> 副盟主 </color> 吗？";
                    CurPostIndex = 2;
                    break;
                case E_PostType.XiaoDuiZhang:
                    TipInfo.text = $"任命 <color=green>{CurChooseWarMemberInfo.Name}</color> 为<color=red> 战斗队长 </color> 吗？";
                    CurPostIndex = 1;
                    break;
                default:
                    break;
            }
        }

        public void ShoePostSetPanel()
        {
            TipInfo.text = $"任命 <color=green>{CurChooseWarMemberInfo.Name}</color> 为";
            PostSetPanel.SetActive(true);
        }

        public void HidePostSetPanel() 
        {
            PostSetPanel.SetActive(false);
        }
    }
}