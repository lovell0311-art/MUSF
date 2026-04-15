using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using UnityEngine.U2D;
using System;
using System.Linq;


namespace ETHotfix
{

    [ObjectSystem]
    public class UITangibleLimitComponentAwake : AwakeSystem<UITangibleLimitComponent>
    {
        public override void Awake(UITangibleLimitComponent self)
        {
            self.reference = self.GetParent<UI>().GameObject.GetReferenceCollector();
            //--

            self.InitUi();
            self.reference.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                if (self.IsOnClickPlaying)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "正在抽奖，稍后关闭");
                    return;
                }
                UIComponent.Instance.Remove(UIType.UITangibleLimit);
            });
        }
    }
    public partial class UITangibleLimitComponent : Component
    {


        public int receiveStatus = 0;
        public Image DrawPrizes, Good;//获奖面板/物品详情
        public long times = 0;//下次领奖的时间戳
        /// <summary>
        /// ///////////
        /// </summary>
        public ReferenceCollector reference;
        public Dictionary<int, Prize> PrizeDic = new Dictionary<int, Prize>();//奖品字典 key:奖品所在位置 value:奖品信息
        public List<int> PrizeList = new List<int>();//中奖ID集合

        public bool isOnClickPlaying;
        public bool IsOnClickPlaying
        {
            get => isOnClickPlaying;
            set
            {
                isOnClickPlaying = value;
            }
        }

        /// <summary>
        /// ----------------------------------------------
        /// </summary>
        private GameObject obj1, obj2,obj3, Content;
        private Image tips, yaundian, zijiedian;
        private Button CloseBtn;
        public IConfig[] config;
        public Dictionary<int, List<int>> DicIteminfo = new Dictionary<int, List<int>>();//储存表的数据
        /// <summary>
        /// 重新显示ui状态
        /// </summary>
        public void OnRefresh()
        {
        }
        /// <summary>
        /// 展示奖品
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnUi(int itemIndex)
        {
            tips.gameObject.SetActive(true);
            obj1.gameObject.SetActive(itemIndex == 4);
            obj2.gameObject.SetActive(itemIndex == 2);
            obj3.gameObject.SetActive(itemIndex == 3);
            if (itemIndex==2||itemIndex==4)//自选（2级弹窗）
            {
               
                if (obj1.gameObject.activeSelf==true)
                {
                    for (int i = 0; i < obj1.transform.childCount; i++)
                    {
                        int index = i;
                        obj1.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddSingleListener(() =>
                        {
                            Log.DebugBrown("请求购买大类" + itemIndex + ":小类" + DicIteminfo[itemIndex][index]);
                               OpenTangibleLimit(itemIndex, DicIteminfo[itemIndex][index]).Coroutine();
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < obj2.transform.childCount; i++)
                    {
                        int index = i;
                        obj2.transform.GetChild(i).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddSingleListener(() =>
                        {
                            Log.DebugBrown("请求购买大类" + itemIndex + ":小类" + DicIteminfo[itemIndex][index]);
                            OpenTangibleLimit(itemIndex, DicIteminfo[itemIndex][index]).Coroutine();
                        });
                    }
                }

            }
            else if (itemIndex==1)//月卡
            {
                foreach (var item in DicIteminfo)
                {
                    Debug.Log("数据" + item.Key + "::" + item.Value);
                }
                OpenTangibleLimit(itemIndex,DicIteminfo[itemIndex][0]).Coroutine();
            }
            else if(itemIndex==3)//对应职业的武器
            {
                obj3.transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    var config1 = ConfigComponent.Instance.GetAll<ValueGift_ItemInfoConfig>();
                    foreach (var item in config1.Cast<ValueGift_ItemInfoConfig>())
                    {
                        if (item.RoleType == (int)UnitEntityComponent.Instance.LocalRole.RoleType)
                        {
                            OpenTangibleLimit(itemIndex, (int)item.Id).Coroutine();
                            break;
                        }

                    }
                });
                //Log.DebugBrown("当前职业" + (int)UnitEntityComponent.Instance.LocalRole.RoleType);
                //var config1 = ConfigComponent.Instance.GetAll<ValueGift_ItemInfoConfig>();
                //foreach (var item in config1.Cast<ValueGift_ItemInfoConfig>())
                //{
                //    if (item.RoleType== (int)UnitEntityComponent.Instance.LocalRole.RoleType)
                //    {
                //           OpenTangibleLimit(itemIndex,(int)item.Id).Coroutine();
                //        break;
                //    }

                //}

            }
        }


        //获取相应活动的数据
        public async ETTask OpenTangibleLimit(int ActiveId, int v)
        {
            Log.DebugBrown("选择的礼包id" + ActiveId + ":" + v);
            G2C_BuyYourOwnGiftPackResponse g2C_OpenMiracleActivities = (G2C_BuyYourOwnGiftPackResponse)await SessionComponent.Instance.Session.Call(new C2G_BuyYourOwnGiftPackRequest()
            {
                MaxType = ActiveId,
                MinType = v
            }); ;
            if (g2C_OpenMiracleActivities.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"购买成功");
                tips.gameObject.SetActive(false);
            }
        }


        //获取相应活动的数据
        public async ETTask OpenMiracleActivitiesRequest(int ActiveId)
        {
            Log.DebugBrown("经过" + ActiveId);
            G2C_OpenMiracleActivitiesResponse g2C_OpenMiracleActivities = (G2C_OpenMiracleActivitiesResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMiracleActivitiesRequest()
            {
                MiracleActivitiesID = ActiveId
            });
            Log.DebugBrown("获取抽奖的信息" + g2C_OpenMiracleActivities.Error + "状态" + g2C_OpenMiracleActivities.Info.Value32A + ":" + g2C_OpenMiracleActivities.Info.Value64A);
            if (g2C_OpenMiracleActivities.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
            }
            else
            {
                //pass = g2C_OpenMiracleActivities.Info.Value32A;
                times = g2C_OpenMiracleActivities.Info.Value64A;
                receiveStatus = g2C_OpenMiracleActivities.Info.Status;
            }
        }



        public void DistributeObjects()
        {
            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI * 2 / 8;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * 200f;
                GameObject obj = GameObject.Instantiate(zijiedian.gameObject);
                obj.transform.SetParent(yaundian.transform, false);
                obj.transform.position = yaundian.transform.position;
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = position;
                }
                Log.DebugBrown(rectTransform.gameObject.name);
            }

            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI * 2 / 8;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * 400f;
                GameObject obj = GameObject.Instantiate(zijiedian.gameObject);
                obj.transform.SetParent(yaundian.transform, false);
                obj.transform.position = yaundian.transform.position;
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = position;
                }
                Log.DebugBrown(rectTransform.gameObject.name);
            }

            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI * 2 / 8;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * 600f;
                GameObject obj = GameObject.Instantiate(zijiedian.gameObject);
                obj.transform.SetParent(yaundian.transform, false);
                obj.transform.position = yaundian.transform.position;
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = position;
                }
                Log.DebugBrown(rectTransform.gameObject.name);
            }

            for (int i = 0; i < 8; i++)
            {
                float angle = i * Mathf.PI * 2 / 8;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * 800f;
                GameObject obj = GameObject.Instantiate(zijiedian.gameObject);
                obj.transform.SetParent(yaundian.transform, false);
                obj.transform.position = yaundian.transform.position;
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.anchoredPosition = position;
                }
                Log.DebugBrown(rectTransform.gameObject.name);
            }
        }

        /// <summary>
        /// 初始化抽奖的物品
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal void InitUi()
        {
            OpenMiracleActivitiesRequest(14).Coroutine();
            obj1 = reference.GetGameObject("obj1");
            obj2 = reference.GetGameObject("obj2");
            obj3 = reference.GetGameObject("obj3");
            Content = reference.GetGameObject("Content");
            tips = reference.GetImage("tips");
            yaundian = reference.GetImage("yaundian");
            zijiedian = reference.GetImage("zijiedian");
            tips.gameObject.SetActive(false);
            obj1.gameObject.SetActive(false);
            obj3.gameObject.SetActive(false);
            obj2.gameObject.SetActive(false);
           // DistributeObjects();
            tips.transform.Find("mask").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                tips.gameObject.SetActive(false);
            });
            tips.gameObject.SetActive(false);
            config = ConfigComponent.Instance.GetAll<ValueGift_TypeConfig>();
            //int index = 0;
            //Content.transform.GetChild(index).Find("name").GetComponent<Text>().text = "";
            //index++;
           var config1 = ConfigComponent.Instance.GetAll<ValueGift_ItemInfoConfig>();
            //获取数据
            foreach (var item in config1.Cast<ValueGift_ItemInfoConfig>())
            {

                if (!DicIteminfo.ContainsKey(item.TypeId))//若字典没有这个数据缓存
                {
                    DicIteminfo.Add(item.TypeId,new List<int>());
                    DicIteminfo[item.TypeId].Add((int)item.Id);
                }
                else
                {
                    DicIteminfo[item.TypeId].Add((int)item.Id);
                }

            }
            int index = 0;
            foreach (var item in config.Cast<ValueGift_TypeConfig>())
            {
                if (index<Content.transform.childCount)
                {
                    Content.transform.GetChild(index).Find("Btn_buy").GetComponent<Button>().onClick.AddSingleListener(() =>
                    {
                        Log.DebugBrown("点击的是" + item.Id);
                        OnUi((int)item.Id);
                    });
                }
                index++;
            }
            //Content.transform.GetChild(0).gameObject.SetActive(false);
            //Content.transform.GetChild(3).gameObject.SetActive(false);
        }



 

  

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            SpriteUtility.Instance.ClearAtals(AtalsType.UI_Welfare_Icons); ;
            base.Dispose();
        }

    }
}
