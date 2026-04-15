using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public enum ActivitRewardType
    {
        OneBit = 1,
        TowBit = 1 << 1,
        ThreeBit = 1 << 2,
        FourBit = 1 << 3,
        FiveBit = 1 << 4,
        SixBit = 1 << 5,
        SevenBit = 1 << 6,
        EightBit = 1 << 7,
        NineBit = 1 << 8,
        TenBit = 1 << 9,
        ElevenBit = 1 << 10,
        TwelveBit = 1 << 11,
        ThirteenBit = 1 << 12,
        FourteenBit = 1 << 13,
        FifteenBit = 1 << 14,
        SixteenBit = 1 << 15,
        SeventeenBit = 1 << 16,
        EightteenBit = 1 << 17,
        NineteenBit = 1 << 18,
        TwentyBit = 1 << 19,
    }
    public class SprintInfo
    {
        public int Id;
        public int Limit;//领取限制
        public int ActivityID;//活动ID
        public int GoldCoin;//奖励金币数量
        public int MiracleCoin;//奖励奇迹币数量
        public List<int> ItemID;//奖励道具ID
        public int State;//领取状态
    }
    [ObjectSystem]
    public class UISprintComponentStart : StartSystem<UISprintComponent>
    {
        public override void Start(UISprintComponent self)
        {
            self.Start();
        }
    }
    public class UISprintComponent : Component,IUGUIStatus
    {
        public GameObject content;
        public ScrollRect scrollRect;
        RoleEntity roleEntity;//本地玩家
        ReferenceCollector collector;
        UICircularScrollView<SprintInfo> uICircularScrollView;
        public List<SprintInfo> sprintList = new List<SprintInfo>();
        public int receiveStatus;//领取状态
        public void Start()
        {
            collector = GetParent<UI>().GameObject.GetReferenceCollector();
            roleEntity = UnitEntityComponent.Instance.LocalRole;
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.InVisibilityUI(UIType.UISprint);
            });
            content = collector.GetGameObject("Content");
            scrollRect = collector.GetImage("Scroll View").GetComponent<ScrollRect>();
            InitUICircularScrollView();
            LoadGradeConfig();
        }
        public void InitUICircularScrollView()
        {
            uICircularScrollView = ComponentFactory.Create<UICircularScrollView<SprintInfo>>();
            uICircularScrollView.InitInfo(E_Direction.Vertical, 1, 0, 20);
            uICircularScrollView.ItemInfoCallBack = InitInfoCallBack;
            uICircularScrollView.IninContent(content, scrollRect);
        }

        private void InitInfoCallBack(GameObject item, SprintInfo sprintInfo)
        {
            item.transform.Find("Level").GetComponent<Text>().text = sprintInfo.Limit.ToString() + "级";
            item.transform.Find("Glod").Find("Count").GetComponent<Text>().text = sprintInfo.GoldCoin.ToString();
            item.transform.Find("Mu").Find("Count").GetComponent<Text>().text = sprintInfo.MiracleCoin.ToString();
            item.transform.Find("ReceiveBtn").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                RushGradeActivityReceiveRequest(1, sprintInfo.Id, item, sprintInfo).Coroutine();

            });
            SetReceiveStata(item, sprintInfo);
        }

        public void SetReceiveStata(GameObject item, SprintInfo sprintInfo)
        {
            bool state = (sprintInfo.State & receiveStatus) == sprintInfo.State;
            item.transform.Find("ReceiveBtn").gameObject.SetActive(!state);
            item.transform.Find("OnReceiveTxt").gameObject.SetActive(state);
        }

        private void LoadGradeConfig()
        {
            sprintList.Clear();
            var rushGradeConfig = ConfigComponent.Instance.GetAll<RushGrade_RewardConfig>();
            int count = 0;
            foreach (var item in rushGradeConfig.Cast<RushGrade_RewardConfig>())
            {
                SprintInfo sprintInfo = new SprintInfo();
                sprintInfo.Id = (int)item.Id;
                sprintInfo.Limit = item.Limit;
                sprintInfo.ActivityID = item.ActivityID;
                sprintInfo.GoldCoin = item.GoldCoin;
                sprintInfo.MiracleCoin = item.MiracleCoin;
             
                sprintInfo.State = (int)Enum.GetValues(typeof(ActivitRewardType)).GetValue(count);
                count++;
                sprintList.Add(sprintInfo);
            }
        }

        public void OnVisible(object[] data)
        {
            
        }

        public void OnVisible()
        {
            OpenMiracleActivitiesRequest(1).Coroutine();
        }

        public void OnInVisibility()
        {
            
        }
        //获取相应活动的数据
        public async ETTask OpenMiracleActivitiesRequest(int ActiveId)
        {
            G2C_OpenMiracleActivitiesResponse g2C_OpenMiracleActivities = (G2C_OpenMiracleActivitiesResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMiracleActivitiesRequest()
            {
                MiracleActivitiesID = ActiveId
            });
            if(g2C_OpenMiracleActivities.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
            }
            else
            {
               
                receiveStatus = g2C_OpenMiracleActivities.Info.Status;

                uICircularScrollView.Items = sprintList;
            }
        }
        //冲级活动领奖
        public async ETTask RushGradeActivityReceiveRequest(int activeId,int rewardID, GameObject item, SprintInfo sprintInfo)
        {
            if (this.roleEntity.Property.GetProperValue(E_GameProperty.Level) < sprintInfo.Limit) 
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"等级不足");
                return;
            }

            G2C_RushGradeActivityReceiveResponse g2C_OpenMiracleActivities = (G2C_RushGradeActivityReceiveResponse)await SessionComponent.Instance.Session.Call(new C2G_RushGradeActivityReceiveRequest()
            {
                MiracleActivitiesID = activeId,
                RewardID = new Google.Protobuf.Collections.RepeatedField<int> { rewardID}
            });
            if (g2C_OpenMiracleActivities.Error == 2400)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
                UIComponent.Instance.Remove(UIType.UINewYearActivity);
                return;
            }
            if (g2C_OpenMiracleActivities.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功");
               
                receiveStatus = g2C_OpenMiracleActivities.Info.Status;
                SetReceiveStata(item, sprintInfo);
                return ;
            }
        }
    }

}
