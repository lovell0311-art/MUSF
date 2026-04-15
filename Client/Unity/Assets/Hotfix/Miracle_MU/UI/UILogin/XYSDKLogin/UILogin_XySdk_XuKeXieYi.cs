using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    /// <summary>
    /// 服务协议
    /// </summary>
    public partial class UILogin_XySdk
    {
        public GameObject XieYiPanel;
        public UIHyperlinkText XieYiLink;
        ReferenceCollector collector_Xieyi;
        public void InitXueKeXieYi() 
        {
            if (ETModel.Init.instance.e_SDK == E_SDK.HaXi)
            {
                ShowXieYiPanel(false);
                referenceCollector.GetToggle("Toggle").gameObject.SetActive(false);
            }
            else
            {
                if (PlayerPrefs.GetInt("IsAgreen") == 0)//未阅读
                {
                    ShowXieYiPanel(true);
                }
                else
                {
                    ShowXieYiPanel(false);
                }
            }

            collector_Xieyi = XieYiPanel.GetReferenceCollector();
            XieYiLink=collector_Xieyi.GetText("Info").GetComponent<UIHyperlinkText>();
            XieYiLink.RegisterClickCallback(ShowUrl);
          
            collector_Xieyi.GetButton("AgrenBtn").onClick.AddSingleListener(() => 
            {
                if (PlayerPrefs.GetInt("IsAgreen") == 0)
                {
                    PlayerPrefs.SetInt("IsAgreen", 1);
                }
                ShowXieYiPanel(false);
                IsRead = true;
                referenceCollector.GetToggle("Toggle").isOn = true;
            });
            collector_Xieyi.GetButton("DisAgrenBtn").onClick.AddSingleListener(() => 
            {
                if (PlayerPrefs.GetInt("IsAgreen") == 1)
                {
                    PlayerPrefs.SetInt("IsAgreen", 0);
                }
                IsRead = false;
                referenceCollector.GetToggle("Toggle").isOn = false;
                ShowXieYiPanel(false);
            });
        }
        //显示隐私协议面板
        public void ShowXieYiPanel(bool isShow)
        {
            XieYiPanel.SetActive(isShow);
        }

        private void ShowUrl(string s,string s1) 
        {
            Log.DebugGreen($"s:{s}");
            if (ETModel.Init.instance.e_SDK == E_SDK.SHOU_Q)
            {
               
                if (s.Contains("服务协议"))
                {
                    UIComponent.Instance.VisibleUI(UIType.UI_UserAgreement, "https://beefuncloud.game.androidscloud.com/H5-web/BY_SYXKXY.html");
                }
                else if (s.Contains("隐私政策"))
                {
                    UIComponent.Instance.VisibleUI(UIType.UI_UserAgreement, "https://beefuncloud.game.androidscloud.com/H5-web/BY_YSXY.html");
                }

            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UI_UserAgreement);
            }
        }
    }
}
