using ETModel;
using System.Linq;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIE_MailComponentAwake : AwakeSystem<UIE_MailComponent>
    {
        public override void Awake(UIE_MailComponent self)
        {
            self.Awake();
            self.InitRight();
            self.InitLeft();
        }
    }
    public partial class UIE_MailComponent : Component,IUGUIStatus
    {
        public ReferenceCollector collector;
        public GameObject BG1;
        public void Awake()
        {
            collector = GetParent<UI>().GameObject.GetReferenceCollector();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UIE_Mail);
            });
            BG1 = collector.GetImage("BG1").gameObject;
            //TestInitE_Mail();

        }

        public void OnInVisibility()
        {

        }

        public void OnVisible(object[] data)
        {

        }

        public void OnVisible()
        {
            RedDotManagerComponent.RedDotManager.Set(E_RedDotDefine.Root_Email, 0);
            UIMainComponent.Instance.RedDotFriendCheack();
            OpenMailRequest().Coroutine();
        }

        public async ETTask OpenMailRequest()
        {
            G2C_OpenMailResponse g2C_OpenMail = (G2C_OpenMailResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMailRequest(){});
            Log.DebugBrown("打开了邮件系统" + g2C_OpenMail.MailList.count);
            if(g2C_OpenMail.Error == 0)
            {
                UIE_MailData.uIE_MailInfos.Clear();
                for (int i = 0,length = g2C_OpenMail.MailList.Count; i < length; i++)
                {
                    UIE_MailData.AddEmail(g2C_OpenMail.MailList[i]);
                }
                if (UIE_MailData.uIE_MailInfos.Count != 0)
                {
                    UIE_MailData.uIE_MailInfos.Sort((UIE_MailInfo u1, UIE_MailInfo u2) => u2.email_Time.CompareTo(u1.email_Time));
                    UIE_MailData.uIE_MailInfos.First().isClick = true;
                    UIE_MailData.uIE_MailInfos.First().emailIsRend = true;
                }
                
                if (UIE_MailData.uIE_MailInfos.Count == 0&& uICircular!=null)
                {
                    uICircular.Items?.Clear();
                }
                BG1.SetActive(UIE_MailData.uIE_MailInfos.Count == 0);

                uICircular.Items = UIE_MailData.uIE_MailInfos;
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMail.Error.GetTipInfo());
            }
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            ModelHide();
            uICircular.Dispose();
        }

    }

}
