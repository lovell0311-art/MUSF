using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIE_MailComponent
    {
        public ReferenceCollector collectorLeft;
        public UICircularScrollView<UIE_MailInfo> uICircular;
        public GameObject emailContent;
        public ScrollRect emailView;
        public Button AllDelBtn, AllGetBtn;
        public void InitLeft()
        {
            collectorLeft = collector.GetImage("Left").gameObject.GetReferenceCollector();
            emailContent = collectorLeft.GetGameObject("E_MailContent");
            emailView = collectorLeft.GetImage("E_MailView").GetComponent<ScrollRect>();
            AllDelBtn = collectorLeft.GetButton("AllDelBtn");
            AllGetBtn = collectorLeft.GetButton("AllGetBtn");
            //全部删除
            AllDelBtn.onClick.AddSingleListener(delegate ()
            {
                List<long> id = new List<long>();
                for (int j = 0, length1 = UIE_MailData.uIE_MailInfos.Count; j < length1; j++)
                {
                    id.Add(UIE_MailData.uIE_MailInfos[j].id);
                }
                MailDeleteItemRequest(id).Coroutine();
            });
            //全部领取
            AllGetBtn.onClick.AddSingleListener(delegate ()
            {
                List<long> id = new List<long>();
                for (int j = 0,length1 = UIE_MailData.uIE_MailInfos.Count; j < length1; j++)
                {
                    if(!UIE_MailData.uIE_MailInfos[j].AttachmentIsGet)
                        id.Add(UIE_MailData.uIE_MailInfos[j].id);
                }
                MailReceiveItemRequest(id).Coroutine();
            });
            UICircularInit();
            SetBtnShow();
        }
        /// <summary>
        /// 初始化邮件列表滑动框
        /// </summary>
        public void UICircularInit()
        {
            uICircular = ComponentFactory.Create<UICircularScrollView<UIE_MailInfo>>();
            uICircular.InitInfo(E_Direction.Vertical, 1, 0, 10);
            uICircular.ItemInfoCallBack = InitEmailItem;
            uICircular.IninContent(emailContent, emailView);
        }

        private void InitEmailItem(GameObject item, UIE_MailInfo uiE_MailInfo)
        {
            item.transform.Find("Background").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                //if (UIE_MailData.lastClickEmail.uIE_MailInfo.id == uiE_MailInfo.id) return;
                UIE_MailData.lastClickEmail.item.transform.Find("Checkmark").gameObject.SetActive(false);
                UIE_MailData.lastClickEmail.uIE_MailInfo.isClick = false;

                uiE_MailInfo.isClick = true;
                item.transform.Find("Checkmark").gameObject.SetActive(true);

                EmialClickEvent(item, uiE_MailInfo);
            });
            item.transform.Find("readImage").gameObject.SetActive(uiE_MailInfo.emailIsRend);
            item.transform.Find("readImage").Find("Image").gameObject.SetActive(!uiE_MailInfo.AttachmentIsGet);

            item.transform.Find("unreadImage").gameObject.SetActive(!uiE_MailInfo.emailIsRend);
            item.transform.Find("unreadImage").Find("Image").gameObject.SetActive(!uiE_MailInfo.AttachmentIsGet);

            item.transform.Find("Message").GetComponent<Text>().text = $"{uiE_MailInfo.Title}";

            item.transform.Find("Checkmark").gameObject.SetActive(uiE_MailInfo.isClick);
            if (uiE_MailInfo.isClick) EmialClickEvent(item, uiE_MailInfo);
            //if(uiE_MailInfo.AttachmentIsGet)
        }

        private void EmialClickEvent(GameObject item, UIE_MailInfo uiE_MailInfo)
        {
            if (UIE_MailData.lastClickEmail == null)
            {
                UIE_MailData.lastClickEmail = new UIE_MailItemInfo();
            }
            UIE_MailData.lastClickEmail.uIE_MailInfo = uiE_MailInfo;
            UIE_MailData.lastClickEmail.item = item;
            SetEmailContent(uiE_MailInfo);
            SetBtnShow();
            uiE_MailInfo.emailIsRend = true;
            item.transform.Find("readImage").gameObject.SetActive(uiE_MailInfo.emailIsRend);
            item.transform.Find("unreadImage").gameObject.SetActive(!uiE_MailInfo.emailIsRend);
            ReadMailRequest(uiE_MailInfo.id).Coroutine();
        }
        /// <summary>
        /// 读取邮件
        /// </summary>
        /// <param name="emailIDList"></param>
        /// <returns></returns>
        public async ETTask ReadMailRequest(long emailID)
        {
            G2C_ReadMailResponse g2C_MailDeleteItem = (G2C_ReadMailResponse)await SessionComponent.Instance.Session.Call(new C2G_ReadMailRequest()
            {
                MailId = emailID
            });
            if (g2C_MailDeleteItem.Error == 0)
            {
                //Log.DebugGreen("邮件读取成功");
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MailDeleteItem.Error.GetTipInfo());
            }
        }
        public void SetBtnShow()
        {
            AllDelBtn.gameObject.SetActive(UIE_MailData.GetNoReceive());
            AllGetBtn.gameObject.SetActive(!UIE_MailData.GetNoReceive());
        }

    }

}
