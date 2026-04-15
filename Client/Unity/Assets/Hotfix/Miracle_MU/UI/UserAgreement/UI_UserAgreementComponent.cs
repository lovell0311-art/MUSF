using ETModel;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UI_UserAgreementComponentAwake : AwakeSystem<UI_UserAgreementComponent>
    {
        public override void Awake(UI_UserAgreementComponent self)
        {
            self.referenceCollector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.webMediator = self.referenceCollector.GetGameObject("xieyi").GetComponent<WebMediator>();
            
            //self.webMediator.Show("https://www.baidu.com");

            self.referenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                self.webMediator.Hide();
                UIComponent.Instance.Remove(UIType.UI_UserAgreement);
            });
            
        }
    }

    [ObjectSystem]
    public class UI_UserAgreementComponentStart : StartSystem<UI_UserAgreementComponent>
    {
        public override void Start(UI_UserAgreementComponent self)
        {
            //Application.OpenURL(self.url);
            self.webMediator.Show(self.url);
           
        }
    }
    /// <summary>
    /// 用户协议
    /// </summary>
    public class UI_UserAgreementComponent : Component,IUGUIStatus
    {
        public ReferenceCollector referenceCollector;
        public Button closeBtn;
        public WebMediator webMediator;
        
            public string url = "https://www.hjsk.cn/static/docD/policy1.html";
       // public string url = "https://www.hjsk.cn/static/doc/policy.html";
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
        }

        public void OnInVisibility()
        {
          
        }

        public void OnVisible(object[] data)
        {
            if (data.Length > 0)
            { 
             url = data[0].ToString();
            }
        }

        public void OnVisible()
        {
          
        }
    }

    
}
