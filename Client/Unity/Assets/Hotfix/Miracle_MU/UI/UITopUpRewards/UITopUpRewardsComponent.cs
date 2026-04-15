using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

namespace ETHotfix
{

    public class TopUpRewardsData
    {
        public int id;
        public string Name;
        public int Money;
        public List<CumulativeRecharge_ItemInfoConfig> topUpItems = new List<CumulativeRecharge_ItemInfoConfig>();
        public bool isGet;
        /// <summary>
        /// 物品信息
        /// </summary>
        public string Description;
    }
    public class TopUpItemData
    {
        /// <summary>
        /// 累计充值类型
        /// </summary>
        public int id;
        /// <summary>
        /// 物品Id
        /// </summary>
        public int configId;
        /// <summary>
        /// 小类Id
        /// </summary>
        public int id2;
        /// <summary>
        /// 数量
        /// </summary>
        public int itemCount;
        /// <summary>
        /// 是否可选
        /// </summary>
        public int IsSelectable;
        /// <summary>
        /// 物品名
        /// </summary>
        public string ItemName;
    }

    [ObjectSystem]
    public class UITopUpRewardsComponentAwake : AwakeSystem<UITopUpRewardsComponent>
    {
        public override void Awake(UITopUpRewardsComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.Awake();
        }
    }
    public partial class UITopUpRewardsComponent : Component
    {
        RoleEntity RoleEntity_ => UnitEntityComponent.Instance.LocalRole;
        public ReferenceCollector collector;
        public GameObject Content, Item;
        public GameObject levelBg;
        public Button GetItemPanel;
        public Button One, Two, Three;
        public ScrollRect TopUpScrollView;
        public List<TopUpRewardsData> uITitleInfos = new List<TopUpRewardsData>();
        public int oneId, twoId;
        public Text TopUpAmount;
        public TopUpRewardsData curClickTopUp = new TopUpRewardsData();
        public void Awake()
        {
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UITopUpRewards);
            });
            Content = collector.GetGameObject("Content");
            TopUpScrollView = collector.GetImage("Scroll View").gameObject.GetComponent<ScrollRect>();
            levelBg = collector.GetImage("levelBg").gameObject;
            Item = collector.GetImage("Item").gameObject;
            TopUpAmount = collector.GetText("TopUpAmount");
            InitSelectItem();
            OpenCumulativeRecharge().Coroutine();
        }
        public void InitTopUpInfo()
        {
            var recharge_Type = ConfigComponent.Instance.GetAll<CumulativeRecharge_TypeConfig>();
            var recharge_ItemInfo = ConfigComponent.Instance.GetAll<CumulativeRecharge_ItemInfoConfig>();
            foreach (var item in recharge_Type.Cast<CumulativeRecharge_TypeConfig>())
            {
                TopUpRewardsData topUpRewards = new TopUpRewardsData();
                topUpRewards.id = (int)item.Id;
                topUpRewards.Name = item.Name;
                topUpRewards.Money = item.Money;
                topUpRewards.isGet = TopUpRewardsGlob.Configids.Contains((int)item.Id);
                foreach (var itemtype in recharge_ItemInfo.Cast<CumulativeRecharge_ItemInfoConfig>())
                {
                    if (itemtype.TypeId == item.Id && (itemtype.RoleType == 0 || itemtype.RoleType == (int)RoleEntity_.RoleType))
                    {
                        topUpRewards.topUpItems.Add(itemtype);
                    }
                }
                topUpRewards.Description = item.Description;
                uITitleInfos.Add(topUpRewards);
            }
            InitUICircular_Rewards();
        }

        private void InitUICircular_Rewards()
        {
            for (int i = 0, length = uITitleInfos.Count; i < length; i++)
            {
                GameObject item = GameObject.Instantiate(Item, Content.transform);
                item.name = (i + i).ToString();
                item.SetActive(true);
                int n = i;
                item.transform.Find("freekuang").GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    // Log.DebugGreen($"查看物品{uITitleInfos[n].topUpItems == null}");
                    //SetSelectIsShow(uITitleInfos[n].topUpItems);
                    LoadModle(uITitleInfos[n].topUpItems);
                });
                item.transform.Find("topUpBtnfree/moneyFreeTxt").GetComponent<Text>().text = uITitleInfos[i].Name;
            }
        }

    }
}
