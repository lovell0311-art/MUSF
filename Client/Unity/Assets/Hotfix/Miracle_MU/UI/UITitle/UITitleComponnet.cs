using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UITitleComponnetAwake : AwakeSystem<UITitleComponnet>
    {
        public override void Awake(UITitleComponnet self)
        {
            self.collectorTitle = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.TitleContent = self.collectorTitle.GetGameObject("Content");
            self.TitleView = self.collectorTitle.GetImage("Scroll View").GetComponent<ScrollRect>();
            self.Awake();
        }
    }
   // [ObjectSystem]
    public class UITitleComponnetStart : StartSystem<UITitleComponnet>
    {
        public override void Start(UITitleComponnet self)
        {
            self.InitUICircular_Title();
        }
    }


    public class UITitleComponnet : Component//,IUGUIStatus
    {
        public ReferenceCollector TitlesConfig;
        public List<UITitleInfo> uITitleInfos = new List<UITitleInfo>();
        public UICircularScrollView<UITitleInfo> uICircular_Title;
        public ReferenceCollector collectorTitle;
        public GameObject TitleContent;
        public ScrollRect TitleView;

       // public Dictionary<int,GameObject> Titles = new Dictionary<int, GameObject>();
        CancellationTokenSource tokenSource;
        CancellationToken cancelLogin;
        internal void Awake()
        {
            tokenSource = new CancellationTokenSource();
            cancelLogin = tokenSource.Token;
            collectorTitle.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UITitle);
            });

            InitUICircular_Title();
            OnVisible();
            //  TitleNounTimer();
        }
        /// <summary>
        /// 初始化称号滑动框
        /// </summary>
        public void InitUICircular_Title()
        {
            uICircular_Title = ComponentFactory.Create<UICircularScrollView<UITitleInfo>>();
            uICircular_Title.InitInfo(E_Direction.Vertical, 1, 0, 10);
            uICircular_Title.ItemInfoCallBack = InitTitleItem;
            uICircular_Title.IninContent(TitleContent, TitleView); 

        }

        private void InitTitleItem(GameObject item, UITitleInfo titleInfo)
        {
            for (int i = 0, length = item.transform.Find("TitleImage").childCount; i < length; i++)
            {
                if (item.transform.Find("TitleImage").GetChild(i).gameObject.name != titleInfo.TitleAssetsName)
                {
                    // ResourcesComponent.Instance.DestoryGameObjectImmediate(item.transform.Find("TitleImage").GetChild(i).gameObject, item.transform.Find("TitleImage").GetChild(i).gameObject.name.StringToAB());
                    //GameObject.Destroy(item.transform.Find("TitleImage").GetChild(i).gameObject);
                    item.transform.Find("TitleImage").GetChild(i).gameObject.SetActive(false);
                }
            }
            titleInfo.titleModle.transform.parent = item.transform.Find("TitleImage");
            titleInfo.titleModle.transform.localRotation = Quaternion.identity;
            titleInfo.titleModle.transform.localScale = Vector3.one * 0.2f;
            titleInfo.titleModle.transform.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, -65, 0);
            titleInfo.titleModle.SetActive(true);
            item.transform.Find("TitleImage").gameObject.SetActive(true);

            item.transform.Find("GetWay").GetComponent<Text>().text = titleInfo.GetWay;
            item.transform.Find("Attribute").GetComponent<Text>().text = titleInfo.Attribute;
            item.transform.Find("Button/Text").GetComponent<Text>().text = GetButtonDetailed();
            if(titleInfo.EndTime == 0)
            {
                item.transform.Find("Time").GetComponent<Text>().text = $"永久";
            }
            else if (titleInfo.EndTime == -1)
            {
                item.transform.Find("Time").GetComponent<Text>().text = String.Empty;
            }
            else
            {
                item.transform.Find("Time").GetComponent<Text>().text = TimerComponent.Instance.SecondTimeToDay(titleInfo.EndTime);
            }

           
            item.transform.Find("Button").GetComponent<Button>().onClick.AddSingleListener(() => { BtnEvent(titleInfo.UseInfo, (int)titleInfo.TitleId); });
            if(titleInfo.UseInfo == 0)
            {
                item.transform.Find("Button").GetComponent<Button>().interactable = false;
            }
            else
            {
                item.transform.Find("Button").GetComponent<Button>().interactable = true;
            }
            string GetButtonDetailed() => titleInfo.UseInfo switch
            {
                0 => "未获取",
                1 => "使用",
                2 => "卸下",
                _ => string.Empty
            };
        }
        public void BtnEvent(int index,int id)
        {
            Log.DebugBrown("称号" + index + ":id是" + id);
            if(index == 1)
            {
                SetPlayerTitle(id).Coroutine();
            }else if(index == 2)
            {
                SetPlayerTitle(0).Coroutine();
            }
        }
        public async ETVoid SetPlayerTitle(int id)
        {
            G2C_SetPlayerTitleResponse setPlayerTitle = (G2C_SetPlayerTitleResponse)await SessionComponent.Instance.Session.Call(new C2G_SetPlayerTitleRequest()
            {
                UseTitle = id
            });
            if (setPlayerTitle.Error == 0)
            {
                TitleManager.useID = id;
                UnitEntityComponent.Instance.LocalRole.GetComponent<UIUnitEntityHpBarComponent>()?.SetEntityTitle(id);
                UIComponent.Instance.Get(UIType.UIRoleInfo).GetComponent<UIRoleInfoComponent>()?.ChangeTitle();
                UIComponent.Instance.Remove(UIType.UITitle);
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,setPlayerTitle.Error.GetTipInfo()) ;
                UIComponent.Instance.Remove(UIType.UITitle);
            }
        }
        public void OnVisible(object[] data)
        {

        }

        public void OnVisible()
        {
            uITitleInfos.Clear();
            //获取所有称号
            var titleConfig = ConfigComponent.Instance.GetAll<TitleConfig_InfoConfig>();
            foreach (var item in titleConfig.Cast<TitleConfig_InfoConfig>())
            {
                UITitleInfo uITitleInfo = new UITitleInfo();
                uITitleInfo.TitleId = item.Id;
                uITitleInfo.GetWay = item.GetWay;
                uITitleInfo.Describe = item.Describe;
                uITitleInfo.TitleAssetsName = item.AsstetName;
                uITitleInfo.Attribute = item.AttributeDescribe;
                
                int n = 0;
                for (int i = 0, length = TitleManager.allTitles.Count; i < length; i++)
                {
                    if (TitleManager.allTitles[i].TitleId == item.Id)
                    {
                        n++;
                        uITitleInfo.BingTime = TitleManager.allTitles[i].BingTime;
                        uITitleInfo.EndTime = TitleManager.allTitles[i].EndTime;
                        break;
                    }
                }
                uITitleInfo.UseInfo = (n != 0) ? 1 : 0;
                if (TitleManager.useID == item.Id)
                {
                    uITitleInfo.UseInfo = 2;
                }

                uITitleInfo.titleModle = ResourcesComponent.Instance.LoadGameObject(item.AsstetName.StringToAB(), item.AsstetName);
                uITitleInfo.titleModle.SetActive(false);
                uITitleInfos.Add(uITitleInfo);
            }
            uICircular_Title.Items = uITitleInfos;
        }
        public override void Dispose()
        {
            tokenSource.Cancel();
            uICircular_Title.Dispose();
            base.Dispose();
        }
        public void OnInVisibility()
        {

        }
    }

}
