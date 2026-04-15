
using ETModel;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ETHotfix
{
    [ObjectSystem]
    public class UICongZhiPaiHangBangComponentAwake : AwakeSystem<UICongZhiPaiHangBangComponent>
    {
        public override void Awake(UICongZhiPaiHangBangComponent self)
        {
            self.Awale(); 
        }
    }
    public class UICongZhiPaiHangBangComponent : Component
    {
        public ReferenceCollector collector;
        public UICircularScrollView<Rank_status> uICircular_ChongZhi;
        public List<Rank_status> uICongZhisList = new List<Rank_status>();
        public GameObject Content;
        public ScrollRect scrollRect;
        internal void Awale()
        {
            collector = GetParent<UI>().GameObject.GetReferenceCollector();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                Close();
            });
            Content = collector.GetGameObject("Content");
            scrollRect = collector.GetImage("Scroll View").GetComponent<ScrollRect>();

            InitUICircular_ChongZhi();
            GetRankInfoRequest().Coroutine();
        }
        long UerID = 0;
        int RankId = 0;
        public async ETVoid GetRankInfoRequest()
        {
            bool isSelf = false;
            G2C_GetRankInfoResponse g2C_GetRankInfo = (G2C_GetRankInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_GetRankInfoRequest(){ RankType=1});
            Log.DebugBrown("打印排行榜数据" + JsonHelper.ToJson(g2C_GetRankInfo)+":积分"+ UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.AllianceIntegral));
            if(g2C_GetRankInfo.Error == 0)
            {
                //Log.DebugGreen($"排行榜人数 -》 {g2C_GetRankInfo.RankList.Count} GlobalDataManager.XYUUID:{GlobalDataManager.XYUUID}");
                //for (int i = 0, length = g2C_GetRankInfo.RankList.Count; i < length; i++)
                //{
                //    uICongZhisList.Add(g2C_GetRankInfo.RankList[i]);
                //   // Log.DebugBrown($"{i}->{g2C_GetRankInfo.RankList[i].UserID}");
                //    //if (g2C_GetRankInfo.RankList[i].UserID != null && g2C_GetRankInfo.RankList[i].UserID.ToInt32() == GlobalDataManager.XYUUID)
                //    //{
                //    //    isSelf = true;
                //    //}
                //}
                UerID = g2C_GetRankInfo.GameUserID;
                List<Rank_status> list1 = new List<Rank_status>();//记录转生等级
                List<Rank_status> list2 = new List<Rank_status>();//记录等级
                for (int i = 0; i < g2C_GetRankInfo.RankList.count; i++)
                {
                    if (g2C_GetRankInfo.RankList[i].Value32B!=0)
                    {
                        list1.Add(g2C_GetRankInfo.RankList[i]);
                    }
                    else
                    {
                        list2.Add(g2C_GetRankInfo.RankList[i]);
                    }
                }
                //冒泡排序
                Rank_status temp = new Rank_status();
                for (int i = 0, length = list2.Count-1; i < length; i++)
                {
                    for (int j = 0; j < list2.Count-i-1; j++)
                    {
                        if (list2[j].Value32B< list2[j + 1].Value32B)
                        {
                            temp = list2[j];
                            list2[j] = list2[j + 1];
                            list2[j + 1] = temp;
                        }
                    }
                }

                Rank_status temp1 = new Rank_status();
                for (int i = 0, length = list1.Count - 1; i < length; i++)
                {
                    for (int j = 0; j < list1.Count - i - 1; j++)
                    {
                        if (list1[j].Value32B < list1[j + 1].Value32B)
                        {
                            temp1 = list1[j];
                            list1[j] = list1[j + 1];
                            list1[j + 1] = temp1;
                        }
                    }
                }
                uICongZhisList.AddRange(list1);
                uICongZhisList.AddRange(list2);
                //for (int i = 0; i < uICongZhisList.Count; i++)
                //{
                //    Log.DebugBrown("排序后的数据" + uICongZhisList[i].Value32B + "::" + uICongZhisList[i].Value32B);
                //}
                //foreach (var item in g2C_GetRankInfo.RankList)
                //{

                //    Log.DebugBrown("排序后的数据" + item.Value32B + "::" + item.Value32B);
                //    uICongZhisList.Add(item);
                   
                //    //if (string.IsNullOrEmpty(item.UserName)) continue;
                //    //if (UerID != 0 && item.UserID == UerID)
                //    //{
                //    //    RankId = item.RankID;
                //    //    isSelf = true;
                //    //}
                 
                //}
                if(!isSelf)
                    collector.GetText("NoList").gameObject.GetComponent<Text>().text = "暂未上榜";
                else
                    collector.GetText("NoList").gameObject.GetComponent<Text>().text = $"当前排名：{RankId}";
                uICircular_ChongZhi.Items = uICongZhisList;
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetRankInfo.Error.GetTipInfo());
            }
        }
        public void Close()
        {
            uICongZhisList.Clear();
            uICircular_ChongZhi.Dispose();
            UIComponent.Instance.Remove(UIType.UICongZhiPaiHangBang);
        }

        public void InitUICircular_ChongZhi()
        {
            uICircular_ChongZhi = ComponentFactory.Create<UICircularScrollView<Rank_status>>();
            uICircular_ChongZhi.InitInfo(E_Direction.Vertical, 1, 0, 10);
            uICircular_ChongZhi.ItemInfoCallBack = InitChongZhiItem;
            uICircular_ChongZhi.IninContent(Content, scrollRect);
        }

        private void InitChongZhiItem(GameObject item, Rank_status uICongZhiData)
        {

            item.transform.Find("RankTxt").GetComponent<Text>().text = uICongZhiData.StrB;
            item.transform.Find("NameTxt").GetComponent<Text>().text = uICongZhiData.StrA.ToString();
            item.transform.Find("LevelTxt").GetComponent<Text>().text = uICongZhiData.Value32B.ToString();
           // item.transform.Find("TorquelTxt").GetComponent<Text>().text = uICongZhiData.Value32B.ToString();
            item.transform.Find("OccupationTxt").GetComponent<Text>().text = GetOccupationTxt((int)uICongZhiData.Value64B);
            item.transform.Find("one").gameObject.SetActive(uICongZhiData.StrB == "1");
            item.transform.Find("two").gameObject.SetActive(uICongZhiData.StrB == "2");
            item.transform.Find("three").gameObject.SetActive(uICongZhiData.StrB == "3");
            // item.transform.Find("four").gameObject.SetActive(uICongZhiData.RankID >= 4);
            if (!string.IsNullOrEmpty(uICongZhiData.StrA))
                item.transform.Find("BG").gameObject.SetActive(UerID != 0 && uICongZhiData.Value64A == UerID);
           // Log.DebugGreen($"{uICongZhiData.UserID.ToInt32()}---hahh---{GlobalDataManager.XYUUID}");
        }
        private string GetOccupationTxt(int type)
        {
            if (type==1)
            {
                return "魔法师";
            }
            else if (type==2)
            {
                return "剑士";
            }
            else if (type == 3)
            {
                return "弓箭手";
            }
            else if (type == 4)
            {
                return "魔剑士";
            }
            else if (type == 5)
            {
                return "圣导师";
            }
            else if (type == 6)
            {
                return "召唤师";
            }
            else if (type == 7)
            {
                return "格斗家";
            }
            else if (type == 8)
            {
                return "梦幻骑士";
            }
            return "异常";
        }
    }

}