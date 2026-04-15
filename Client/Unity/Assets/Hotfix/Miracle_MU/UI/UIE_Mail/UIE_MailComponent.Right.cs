using Codice.CM.Common;
using ETModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIE_MailComponent
    {
        public static readonly int UILayer = LayerMask.NameToLayer(LayerNames.UI);
        public ReferenceCollector collectorRight;
        public Text ContentTxt, TimeTxt, TitleTxt;
        public Button DelBtn, GetBtn;
        public GameObject items;
        public List<GameObject> itemsList = new List<GameObject>();
        List<long> id = new List<long>();
        public void InitRight()
        {
            collectorRight = collector.GetImage("Right").gameObject.GetReferenceCollector();
            ContentTxt = collectorRight.GetText("ContentTxt");
            TimeTxt = collectorRight.GetText("TimeTxt");
            TitleTxt = collectorRight.GetText("TitleTxt");
            items = collectorRight.GetImage("Items").gameObject;

            DelBtn = collectorRight.GetButton("DelBtn");
            GetBtn = collectorRight.GetButton("GetBtn");
            //删除邮件
            DelBtn.onClick.AddSingleListener(() =>
            {
                List<long> id = new List<long>();
                id.Add(UIE_MailData.lastClickEmail.uIE_MailInfo.id);
                MailDeleteItemRequest(id).Coroutine();
            });
            //领取邮件
            GetBtn.onClick.AddSingleListener(() =>
            {
                id.Clear();
                id.Add(UIE_MailData.lastClickEmail.uIE_MailInfo.id);
                MailReceiveItemRequest(id).Coroutine();
            });
        }
        /// <summary>
        /// 领取附件
        /// </summary>
        /// <param name="emailIDList"></param>
        /// <returns></returns>
        public async ETTask MailReceiveItemRequest(List<long> emailIDList)
        {
            G2C_MailReceiveItemResponse g2C_MailDeleteItem = (G2C_MailReceiveItemResponse)await SessionComponent.Instance.Session.Call(new C2G_MailReceiveItemRequest()
            {
              //  MailId = emailIDList
                MailId = new Google.Protobuf.Collections.RepeatedField<long> { emailIDList }
            });
         
            if (g2C_MailDeleteItem.Error == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功");
                for (int i = 0; i < emailIDList.Count; i++)
                {
                    for (int j = 0; j < UIE_MailData.uIE_MailInfos.Count; j++)
                    {
                        if (UIE_MailData.uIE_MailInfos[j].id == emailIDList[i])
                        {
                            UIE_MailData.uIE_MailInfos[j].AttachmentIsGet = true;
                            UIE_MailData.uIE_MailInfos[j].emailIsRend = true;
                        }
                    }
                    
                }
                uICircular.Items = UIE_MailData.uIE_MailInfos;
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MailDeleteItem.Error.GetTipInfo());
            }
        }
        /// <summary>
        /// 删除邮件
        /// </summary>
        /// <param name="emailIDList"></param>
        /// <returns></returns>
        public async ETTask MailDeleteItemRequest(List<long> emailIDList)
        {
            G2C_MailDeleteItemResponse g2C_MailDeleteItem = (G2C_MailDeleteItemResponse)await SessionComponent.Instance.Session.Call(new C2G_MailDeleteItemRequest()
            {
                MailId = new Google.Protobuf.Collections.RepeatedField<long> { emailIDList }
            });
            if(g2C_MailDeleteItem.Error == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"删除成功");
                for (int i = 0; i < emailIDList.Count; i++)
                {
                    for (int j = 0; j < UIE_MailData.uIE_MailInfos.Count; j++)
                    {
                        if (UIE_MailData.uIE_MailInfos[j].id == emailIDList[i])
                        {
                            UIE_MailData.uIE_MailInfos.Remove(UIE_MailData.uIE_MailInfos[j]);
                            break;
                        }
                    } 
                }
                if (UIE_MailData.uIE_MailInfos.Count == 0)
                {
                    uICircular.Items.Clear();
                    BG1.SetActive(true);
                }
                else
                {
                    UIE_MailData.uIE_MailInfos.First().isClick = true;
                }
                ModelHide();
                uICircular.Items = UIE_MailData.uIE_MailInfos;
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MailDeleteItem.Error.GetTipInfo());
            }
        }

        public void SetEmailContent(UIE_MailInfo uIE_MailInfo)
        {
            ModelHide();
            TitleTxt.text = uIE_MailInfo.Title;
            ContentTxt.text = uIE_MailInfo.emailContent;
           // Log.DebugBrown("邮件" + uIE_MailInfo.mailAttachment[0].ConfigId);

            for (int i = 0; i < items.transform.childCount; i++)
            {
                items.transform.GetChild(i).gameObject.SetActive(false);
            }
          //  Log.DebugGreen($"到期时间 -> {uIE_MailInfo.email_EndTime} 客户端时间 -> {TimeHelper.ClientNow()} 客户端时间2{TimeHelper.ClientNowSeconds()}");
            string acceptanceTime = TimerComponent.Instance.ScendToYMD(uIE_MailInfo.email_Time);
            string ValidTime = TimerComponent.Instance.ScendToHM(uIE_MailInfo.email_EndTime - TimeHelper.ClientNowSeconds());
            TimeTxt.text = $"时间: {acceptanceTime}  有效期:{ValidTime}";

            DelBtn.gameObject.SetActive(uIE_MailInfo.AttachmentIsGet);
            GetBtn.gameObject.SetActive(!uIE_MailInfo.AttachmentIsGet);

            int count = uIE_MailInfo.mailAttachment.Count;
            for (int i = 0; i < 6; i++)//最多6个附件
            {
               
                if (i < count)
                {

                    if ((long)uIE_MailInfo.mailAttachment[i].ConfigId!= 320316)
                    {
                        items.transform.GetChild(i).gameObject.SetActive(i < count);
                        if (uIE_MailInfo.AttachmentIsGet && i < count)
                        {
                            items.transform.GetChild(i).Find("Image").gameObject.SetActive(true);

                        }
                        else
                        {
                            items.transform.GetChild(i).Find("Image").gameObject.SetActive(false);
                        }
                        if (((long)uIE_MailInfo.mailAttachment[i].ConfigId) == 320294)//金币
                        {
                            items.transform.GetChild(i).Find("Name").gameObject.SetActive(true);
                            items.transform.GetChild(i).Find("Name").GetComponent<Text>().text = "金币";
                            GameObject obj = ResourcesComponent.Instance.LoadGameObject("Other_JinbiUI".StringToAB(), "Other_JinbiUI");
                            Vector3 vector = items.transform.GetChild(i).Find("Image").transform.position;
                            vector.z = 99.5f;
                            obj.transform.position = vector;
                            obj.transform.localScale = Vector3.one * 0.5f;
                            obj.SetLayer(LayerNames.UI);
                            itemsList.Add(obj);
                        }
                        //else if (((long)uIE_MailInfo.mailAttachment[i].ConfigId) == 320316)
                        //{
                        //    //items.transform.GetChild(i).Find("Name").gameObject.SetActive(true);
                        //    //items.transform.GetChild(i).Find("Name").GetComponent<Text>().text = "U币";
                        //    //GameObject obj = ResourcesComponent.Instance.LoadGameObject("Other_qijibi".StringToAB(), "Other_qijibi");
                        //    //Vector3 vector = items.transform.GetChild(i).Find("Image").transform.position;
                        //    //vector.z = 99.5f;
                        //    //obj.transform.position = vector;
                        //    //obj.transform.localScale = Vector3.one;
                        //    //obj.SetLayer(LayerNames.UI);
                        //    //itemsList.Add(obj);
                        //}
                        else
                        {
                            ((long)uIE_MailInfo.mailAttachment[i].ConfigId).GetItemInfo_Out(out Item_infoConfig item_Info);
                            if (item_Info.Name != null)
                            {
                                items.transform.GetChild(i).Find("Name").gameObject.SetActive(true);
                                items.transform.GetChild(i).Find("Name").GetComponent<Text>().text = $"{item_Info.Name}";
                                GameObject obj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);//显示当前附件的模型
                                if (obj == null) continue;
                                Vector3 vector = items.transform.GetChild(i).Find("Image").transform.position;

                                RectTransform rect = items.transform.GetChild(i).GetComponent<RectTransform>();
                                MeshSize.Result result = MeshSize.GetMeshSize(obj.transform, UILayer);
                                float scale = result.GetScreenScaleFactor(new Vector2(rect.rect.width, rect.rect.height) * 1.2f); // ???????ui????????
                                if (scale > 1f) scale = 1f; // ???С???????

                                vector.z = 99.5f;
                                obj.transform.position = vector;
                                //  obj.transform.localScale = Vector3.one * 0.5f;
                                obj.transform.localScale *= scale;
                                obj.transform.GetChild(0).gameObject.SetActive(false);
                                if (uIE_MailInfo.mailAttachment[i].ConfigId == 320501 ||
                                    uIE_MailInfo.mailAttachment[i].ConfigId == 320502 ||
                                    uIE_MailInfo.mailAttachment[i].ConfigId == 320503 ||
                                    uIE_MailInfo.mailAttachment[i].ConfigId == 320504 ||
                                    uIE_MailInfo.mailAttachment[i].ConfigId == 320505 ||
                                    uIE_MailInfo.mailAttachment[i].ConfigId == 320506 ||
                                    uIE_MailInfo.mailAttachment[i].ConfigId == 320507)
                                    obj.transform.localScale *= 0.01f;
                                itemsList.Add(obj);
                            }
                        }
                        items.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"{uIE_MailInfo.mailAttachment[i].AttachmentCount}";
                    }
                }
                   
                
            }
        }
        public void ModelHide()
        {
            for (int i = 0; i < itemsList.Count; i++)
            {
                ResourcesComponent.Instance.DestoryGameObjectImmediate(itemsList[i], itemsList[i].name.StringToAB());
            }
            itemsList.Clear();
        }
    }
}
