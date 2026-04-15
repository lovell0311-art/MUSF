using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using ILRuntime.Runtime;
using System.Text;

namespace ETHotfix
{

    public enum E_ShiLianType
    {
        Init,
        Info,
        Success,
        Fail,
        Rank,
        Reward
    }

    [ObjectSystem]
    public class UIShiLianZhiDiComponentAwake : AwakeSystem<UIShiLianZhiDiComponent>
    {
        public override void Awake(UIShiLianZhiDiComponent self)
        {
            self.referenceCollector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.referenceCollector.gameObject.GetComponent<Canvas>().planeDistance = 80;
            self.referenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIShiLianZhiDi));
            self.enterCount = self.referenceCollector.GetText("enterCount");
            self.InitPanels();
            self.referenceCollector.GetButton("EnterBtn").onClick.AddSingleListener(async() => 
            {
                if (self.CoastCount != 0)
                {
                   
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"是否花费<color=red>{self.CoastCount}魔晶</color>进入试炼之地？");
                    uIConfirmComponent.AddActionEvent(() =>
                    {
                        self.ChangePanel(E_ShiLianType.Info);
                        self.RefreshInfoPnaleInfos(++self.curIndex);
                    });
                }
                else
                {
                    self.ChangePanel(E_ShiLianType.Info);
                    self.RefreshInfoPnaleInfos(++self.curIndex);
                }
              

            }); 
            self.referenceCollector.GetButton("SaoDangdBtn").onClick.AddSingleListener(async() => 
            {

               // if (self.CoastCount != 0)
                {
                    UIConfirmComponent uIConfirmComponent = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirmComponent.SetTipText($"是否花费<color=red>{self.CoastCount+30}魔晶</color>扫荡试炼之地？");
                    uIConfirmComponent.AddActionEvent(async () =>
                    {
                        G2C_SweepTheTrialGrounds g2C_SweepTheTrial = (G2C_SweepTheTrialGrounds)await SessionComponent.Instance.Session.Call(new C2G_SweepTheTrialGrounds());
                        if (g2C_SweepTheTrial.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SweepTheTrial.Error.GetTipInfo());
                        }
                        else
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "扫荡完成");
                            G2C_OpenClimbingTowerNPC g2C_OpenClimbing = (G2C_OpenClimbingTowerNPC)await SessionComponent.Instance.Session.Call(new C2G_OpenClimbingTowerNPC { });
                            if (g2C_OpenClimbing.Error != 0)
                            {
                                self.CoastCount = g2C_OpenClimbing.Expend;
                            }
                            
                        }

                    });

                }
              
              

            });
            self.referenceCollector.GetButton("rankBtn").onClick.AddSingleListener(() => { self.ShowRank(); });
            self.referenceCollector.GetButton("RewardBtn").onClick.AddSingleListener(() => { self.ShowReward(); });
            self.ChangePanel(E_ShiLianType.Init);

            self.InitSuccessPanel();
            self.InitRank();
            self.InitReWard();
         
        }
    }

    /// <summary>
    /// 试炼之地
    /// </summary>
    public class UIShiLianZhiDiComponent : Component,IUGUIStatus
    {

        public GameObject InitPanel, SuccessPanel, FailPanel, InfoPanel,RankPanel,ReWardRankPanel;
        public ReferenceCollector referenceCollector;
        public Text enterCount;

        public int curIndex =0;//当前层数

        List<RanKingInfo> RankList = new List<RanKingInfo>();
        Transform rankParent, rankitem;

        Transform rewardParent, rewarditem;

        public int CoastCount = 0;//花费魔晶数量

        public void InitPanels()
        {
            InitPanel = referenceCollector.GetImage("bg").gameObject;
            SuccessPanel = referenceCollector.GetImage("Success").gameObject;
            FailPanel = referenceCollector.GetImage("Fail").gameObject;
            InfoPanel = referenceCollector.GetImage("EnterInfo").gameObject;
            RankPanel = referenceCollector.GetImage("Rank").gameObject;
            ReWardRankPanel = referenceCollector.GetImage("ReWardRankPanel").gameObject;

        }

        public void InitRank()
        {
            ReferenceCollector collector = RankPanel.GetReferenceCollector();
            rankParent = collector.GetGameObject("Content").transform;
            rankitem = collector.GetGameObject("title_").transform;
            collector.GetButton("ExitBtn").onClick.AddSingleListener(() => 
            {
                RankPanel.SetActive(false);
            });
        }

        public void InitReWard() 
        {
            ReferenceCollector collector = ReWardRankPanel.GetReferenceCollector();
            rewardParent = collector.GetGameObject("Content").transform;
            rewarditem = collector.GetText("title_").transform;
            collector.GetButton("ExitBtn").onClick.AddSingleListener(() =>
            {
                ReWardRankPanel.SetActive(false);
            });
        }

        public void InitSuccessPanel() 
        {
            ReferenceCollector reference = SuccessPanel.GetReferenceCollector();
           
            reference.GetButton("EnterBtn").onClick.AddSingleListener(() => 
            {
                ChangePanel(E_ShiLianType.Info);
                RefreshInfoPnaleInfos(++curIndex);
               
            });
            
            reference.GetButton("ExitBtn").onClick.AddSingleListener(async () => 
            {
                //退出
                G2C_SetOutToChallenge g2C_SetOutTo = (G2C_SetOutToChallenge)await SessionComponent.Instance.Session.Call(new C2G_SetOutToChallenge { Type=0});
                if (g2C_SetOutTo.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SetOutTo.Error.GetTipInfo());
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                    UIComponent.Instance.Remove(UIType.UIShiLianZhiDi);
                }


            });


        }


        public  void RefreshInfoPnaleInfos(int num) 
        {
            ReferenceCollector reference = InfoPanel.GetReferenceCollector();
            TrialTower_MonsterConfig trialTower_Monster = ConfigComponent.Instance.GetItem<TrialTower_MonsterConfig>(num);
            if (trialTower_Monster == null) 
            {
                
                return;
            }
            reference.GetButton("EnterBtn").onClick.AddSingleListener(async () =>
            {
                if (num == 1)
                {
                    //进入
                    G2C_EnterTheTestTower g2C_SetOutTo = (G2C_EnterTheTestTower)await SessionComponent.Instance.Session.Call(new C2G_EnterTheTestTower { });
                    if (g2C_SetOutTo.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SetOutTo.Error.GetTipInfo());
                        UIComponent.Instance.Remove(UIType.UIShiLianZhiDi);
                    }
                    else
                    {
                        UIComponent.Instance.Remove(UIType.UIShiLianZhiDi);
                    }
                }
                else
                {
                    //进入
                    G2C_SetOutToChallenge g2C_SetOutTo = (G2C_SetOutToChallenge)await SessionComponent.Instance.Session.Call(new C2G_SetOutToChallenge { Type = 1 });
                    if (g2C_SetOutTo.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SetOutTo.Error.GetTipInfo());
                        UIComponent.Instance.Remove(UIType.UIShiLianZhiDi);
                    }
                    else
                    {
                        UIComponent.Instance.Remove(UIType.UIShiLianZhiDi);
                    }
                }
            });

            reference.GetButton("ExitBtn").onClick.AddSingleListener(async () =>
            {
                //退出
                if (num == 1)
                {
                    UIComponent.Instance.Remove(UIType.UIShiLianZhiDi);
                }
                else
                {
                    G2C_SetOutToChallenge g2C_SetOutTo = (G2C_SetOutToChallenge)await SessionComponent.Instance.Session.Call(new C2G_SetOutToChallenge { Type = 0 });
                    if (g2C_SetOutTo.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SetOutTo.Error.GetTipInfo());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                        UIComponent.Instance.Remove(UIType.UIShiLianZhiDi);
                    }
                }


            });


            reference.GetText("title_").text = $"试炼之地第 {num} 层";
            reference.GetText("info").text = $"{trialTower_Monster.Monster} x{trialTower_Monster.Number}";

            //奖励
            TrialTower_RewardsConfig trialTower_Rewards = ConfigComponent.Instance.GetItem<TrialTower_RewardsConfig>(num);
            if (trialTower_Rewards == null) return;
            Dictionary<int, int> rewardDic = trialTower_Rewards.Reward.StringToDictionary();

            StringBuilder sb = new StringBuilder();
            foreach (var item in rewardDic)
            {
                Activity_RewardPropsConfig activity_RewardProps = ConfigComponent.Instance.GetItem<Activity_RewardPropsConfig>(item.Key);
                ((long)activity_RewardProps.ItemId).GetItemInfo_Out(out Item_infoConfig item_Info);
                if (item_Info != null)
                {
                    sb.Append(activity_RewardProps.Remark+"\n");
                }
            }
            reference.GetText("rewardInfo").text=sb.ToString();
        }


        public void ChangePanel(E_ShiLianType shiLianType)
        {
            InitPanel.SetActive(shiLianType==E_ShiLianType.Init);
            InfoPanel.SetActive(shiLianType==E_ShiLianType.Info);
            SuccessPanel.SetActive(shiLianType==E_ShiLianType.Success);
            FailPanel.SetActive(shiLianType==E_ShiLianType.Fail);
            RankPanel.SetActive(shiLianType==E_ShiLianType.Rank);
            ReWardRankPanel.SetActive(shiLianType==E_ShiLianType.Reward);
        }

        


        /// <summary>
        /// 显示完成当前层试炼奖励
        /// </summary>
        /// <param name="num"></param>
        public void ShowSuccess(int num) 
        {
            UIMainComponent.Instance.StopOnHook();
            curIndex = num;
            TrialTower_RewardsConfig trialTower_Rewards= ConfigComponent.Instance.GetItem<TrialTower_RewardsConfig>(num);
            if (trialTower_Rewards == null) return;
            ReferenceCollector reference = SuccessPanel.GetReferenceCollector();

            Dictionary<int, int> rewardDic = trialTower_Rewards.Reward.StringToDictionary();
            StringBuilder sb = new StringBuilder();
            foreach (var item in rewardDic)
            {
                Activity_RewardPropsConfig activity_RewardProps = ConfigComponent.Instance.GetItem<Activity_RewardPropsConfig>(item.Key);
                ((long)activity_RewardProps.ItemId).GetItemInfo_Out(out Item_infoConfig item_Info);
                if (item_Info != null)
                {

                    sb.Append(activity_RewardProps.Remark + "\n");
                }
            }
            reference.GetText("rewardInfo").text = sb.ToString();

            var nextIndex = num + 1;
            if (ConfigComponent.Instance.GetItem<TrialTower_RewardsConfig>(nextIndex) == null)
            {
                //最后一层
                reference.GetButton("SuccessBtn").gameObject.SetActive(true);
                reference.GetButton("ExitBtn").gameObject.SetActive(false);
                reference.GetButton("EnterBtn").gameObject.SetActive(false);
                reference.GetButton("SuccessBtn").onClick.AddSingleListener(async() =>
                {
                    //退出
                    G2C_SetOutToChallenge g2C_SetOutTo = (G2C_SetOutToChallenge)await SessionComponent.Instance.Session.Call(new C2G_SetOutToChallenge { Type = 0 });
                    if (g2C_SetOutTo.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SetOutTo.Error.GetTipInfo());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                        UIComponent.Instance.Remove(UIType.UIShiLianZhiDi);
                    }
                });
               

            }
            else
            {
                reference.GetButton("SuccessBtn").gameObject.SetActive(false);
                reference.GetButton("EnterBtn").onClick.AddSingleListener(() =>
                {
                    ChangePanel(E_ShiLianType.Info);
                    RefreshInfoPnaleInfos(++curIndex);

                });

                reference.GetButton("ExitBtn").onClick.AddSingleListener(async () =>
                {
                    //退出
                    G2C_SetOutToChallenge g2C_SetOutTo = (G2C_SetOutToChallenge)await SessionComponent.Instance.Session.Call(new C2G_SetOutToChallenge { Type = 0 });
                    if (g2C_SetOutTo.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SetOutTo.Error.GetTipInfo());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
                        UIComponent.Instance.Remove(UIType.UIShiLianZhiDi);
                    }
                });
            }
        }


        public void InitRankInfo(List<RanKingInfo> rankinfo)
        {
            RankList.Clear();
          
            RankList = rankinfo;
            ShowRank();
        }

        public void ShowRank() 
        {
           // RankPanel.SetActive(true);
            int count = rankParent.childCount;
            if (RankList.Count == 0)
            {
                for (int i = 0; i < count; i++)
                {
                    rankParent.GetChild(i).gameObject.SetActive(false);
                }
                return;
            }
            for (int i = RankList.Count; i < count; i++)
            {
                rankParent.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < RankList.Count; i++)
            {
                Transform item;
                if (i < count)
                {
                    item = rankParent.GetChild(i);
                }
                else
                {
                    item = GameObject.Instantiate<Transform>(rankitem,rankParent);
                }
                item.Find("rank").GetComponent<Text>().text = $"{i+1}";
                item.Find("name").GetComponent<Text>().text = $"{RankList[i].Name}";
                item.Find("rank (1)").GetComponent<Text>().text = $"第{RankList[i].Cnt}层";
                item.gameObject.SetActive(true);
            }
           
        }

      
        public void ShowReward()
        {
            IConfig[] configs =ConfigComponent.Instance.GetAll<TrialTower_RewardsConfig>();

            for (int i = 0; i < rewardParent.childCount; i++)
            {
                rewardParent.GetChild(i).gameObject.SetActive(i < configs.Length);
            }

            for (int i = 0; i < configs.Length; i++)
            {
                Transform item = i < rewardParent.childCount ? rewardParent.GetChild(i) : GameObject.Instantiate<Transform>(rewarditem, rewardParent);
                TrialTower_RewardsConfig trialTowerRewards = (TrialTower_RewardsConfig)configs[i];
                Dictionary<int, int> rewardDic = trialTowerRewards.Reward.StringToDictionary();

                string rewardText = $"第{trialTowerRewards.Id}层:\t{GetRewardText(rewardDic)}";
                item.GetComponent<Text>().text = rewardText;
                item.gameObject.SetActive(true);
            }

            ReWardRankPanel.SetActive(true);
        }

        private string GetRewardText(Dictionary<int, int> rewardDic)
        {
            List<string> rewardTexts = new List<string>();

            foreach (var reward in rewardDic)
            {
                Activity_RewardPropsConfig rewardConfig = ConfigComponent.Instance.GetItem<Activity_RewardPropsConfig>(reward.Key);
                if (rewardConfig != null)
                {
                    rewardTexts.Add(rewardConfig.Remark);
                }
            }

            return string.Join("、", rewardTexts);
        }



        public void OnVisible(object[] data)
        {
            if (data.Length == 2)
            {
                int currentCount = int.Parse(data[0].ToString());
                int totalCost = int.Parse(data[1].ToString());

                if (TitleManager.allTitles.Exists(x => x.TitleId == 60005) && currentCount <= 3)
                {
                    SetButtonTextAndCount("免费进入", string.Empty, 0);
                }
                else
                {
                    SetButtonTextAndCount("进入", $"进入需消耗{totalCost}魔晶", totalCost);
                }
            }
        }

        private void SetButtonTextAndCount(string buttonText, string countText, int coastCount)
        {
            referenceCollector.GetButton("EnterBtn").GetComponentInChildren<Text>().text = buttonText;
            enterCount.text = countText;
            CoastCount = coastCount;
        }
        public void OnVisible()
        {
          
        }

        public void OnInVisibility()
        {
            
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
           
            base.Dispose();
        }

       
    }
}
