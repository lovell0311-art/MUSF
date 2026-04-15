using ETModel;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public class RankingList
    {
        public long id;//玩家Id
        public int ranking;//名次
        public string name;//名称
        public int number;//分数
        public int exp;//经验
        public int bonus;//奖金
    }

    [ObjectSystem]
    public class UIFuBenPaiHangBangComponentAwake : AwakeSystem<UIFuBenPaiHangBangComponent>
    {
        public override void Awake(UIFuBenPaiHangBangComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.GetButton("SureBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UIFuBenPaiHangBang);
            });
            self.SelfContent = self.collector.GetImage("SelfContent").gameObject;
            self.Content = self.collector.GetGameObject("Content");
            self.Scroll_View = self.collector.GetImage("Scroll View").GetComponent<ScrollRect>();
            self.Awake();
        }
    }
    public class UIFuBenPaiHangBangComponent : Component
    {
        public ReferenceCollector collector;
        public ScrollRect Scroll_View;
        public GameObject Content;
        public GameObject SelfContent;
        public UICircularScrollView<RankingList> UICircular;
        public List<RankingList> rankingLists = new List<RankingList>();
        public void Awake()
        {
            Init_UICircular();
        }
        public void Init_UICircular()
        {
            UICircular = ComponentFactory.Create<UICircularScrollView<RankingList>>();
            UICircular.InitInfo(E_Direction.Vertical, 1, 0, 0);
            UICircular.ItemInfoCallBack = InitChatMessage;
            UICircular.IninContent(Content, Scroll_View);
            UICircular.Items = rankingLists;
        }
        public void InitChatMessage(GameObject item, RankingList rankingList)
        {
            SetContent(item, rankingList);
        }

        public void SetSelfRanking(RankingList rankingList)
        {
            SetContent(SelfContent, rankingList);
        }
        public void SetContent(GameObject item, RankingList rankingList)
        {
            item.transform.Find("rankingTxt").GetComponent<Text>().text = rankingList.ranking.ToString();
            item.transform.Find("nameTxt").GetComponent<Text>().text = rankingList.name.ToString();
            item.transform.Find("numberTxt").GetComponent<Text>().text = rankingList.number.ToString();
            item.transform.Find("expTxt").GetComponent<Text>().text = rankingList.exp.ToString();
            item.transform.Find("bonusTxt").GetComponent<Text>().text = rankingList.bonus.ToString();
        }
        public void SetValue(G2C_RedCastleSettlement_notice message)
        {
            rankingLists.Clear();
            RankingList rankingSelfList = new RankingList();
            BubbleSort(message.BatteCopyRankDatas);
            for (int i = 0; i < message.BatteCopyRankDatas.Count; i++)
            {
                RankingList rankingList = new RankingList();
                rankingList.id = message.BatteCopyRankDatas[i].GameUserId;
                rankingList.ranking =i+1;
                rankingList.number = message.BatteCopyRankDatas[i].Score;
                rankingList.bonus = message.BatteCopyRankDatas[i].Coin;
                rankingList.exp = message.BatteCopyRankDatas[i].EXP;
                rankingList.name = message.BatteCopyRankDatas[i].Name;
                rankingLists.Add(rankingList);
                if (message.BatteCopyRankDatas[i].GameUserId == RoleArchiveInfoManager.Instance.LoadRoleUUID)
                {
                    rankingSelfList = rankingList;
                }
            }
            UICircular.Items = rankingLists;
            SetSelfRanking(rankingSelfList);
        }

        private void BubbleSort(RepeatedField<BatteCopyRankData> arr)
        {
            int n = arr.count;
            bool swapped;
            for (int i = 0; i < n - 1; i++)
            {
                swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (arr[j].Score < arr[j + 1].Score)
                    {
                        // Swap the elements
                        BatteCopyRankData temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                        swapped = true;
                    }
                }

                // If no two elements were swapped by inner loop, then break
                if (!swapped)
                    break;
            }
        }
        //private static void BubbleSort(BatteCopyRankData[] arr)
        //{
        //    int n = arr.Length;
        //    bool swapped;
        //    for (int i = 0; i < n - 1; i++)
        //    {
        //        swapped = false;
        //        for (int j = 0; j < n - i - 1; j++)
        //        {
        //            if (arr[j].GameAreaId > arr[j + 1].GameAreaId)
        //            {
        //                // Swap the elements
        //                LineInfo temp = arr[j];
        //                arr[j] = arr[j + 1];
        //                arr[j + 1] = temp;
        //                swapped = true;
        //            }
        //        }

        //        // If no two elements were swapped by inner loop, then break
        //        if (!swapped)
        //            break;
        //    }
        //}
    }

}
