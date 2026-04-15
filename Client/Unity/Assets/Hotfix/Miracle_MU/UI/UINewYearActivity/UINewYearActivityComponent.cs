using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UINewYearActivityComponentAwake : AwakeSystem<UINewYearActivityComponent>
    {
        public override void Awake(UINewYearActivityComponent self)
        {
            self.Awake();
        }
    }
    public class UINewYearActivityComponent : Component
    {
        ReferenceCollector collector;
        public Transform Content;
        public Text activeTime, activeTimeContent;
        public Text Tile;
        public Text activityIntroduce, activityIntroduceContent;
        public Text rewardIntroduce, rewardIntroduceContent;
        public Dictionary<string, int> rewards = new Dictionary<string, int>();
        public void Awake()
        {
            collector = GetParent<UI>().GameObject.GetReferenceCollector();

            activeTime = collector.GetText("activeTime");
            Tile = collector.GetText("Tile");
            Content = collector.GetImage("Content").gameObject.transform;
            activeTimeContent = collector.GetText("activeTimeContent");
            activityIntroduce = collector.GetText("activityIntroduce");
            activityIntroduceContent = collector.GetText("activityIntroduceContent");
            rewardIntroduce = collector.GetText("rewardIntroduce");
            rewardIntroduceContent = collector.GetText("rewardIntroduceContent");
            activeTime.text = "活动时间";
            activityIntroduce.text = "活动规则";
            rewardIntroduce.text = "活动奖励";
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
               
                UIComponent.Instance.Remove(UIType.UINewYearActivity);
            });
            collector.GetButton("renterBtn").onClick.AddSingleListener(() =>
            {
              
                UIComponent.Instance.Remove(UIType.UINewYearActivity);
            });

            NewYearMobCntRequest().Coroutine();
        }
        
        public void LoadData()
        {
            Activity_InfoConfig infoConfig = ConfigComponent.Instance.GetItem<Activity_InfoConfig>(8);
            Kill_RewardConfig kill_RewardConfig = ConfigComponent.Instance.GetItem<Kill_RewardConfig>(2);
           // Log.DebugGreen($"{infoConfig.OpenTime}");
         //   Log.DebugGreen($"{infoConfig.EndTime}");OMVZVB
          //  Log.DebugGreen($"每天{kill_RewardConfig.BrushTime}开启活动，每次活动持续时间为1小时");
            activeTimeContent.text = $"{infoConfig.OpenTime}-{infoConfig.EndTime},每天{kill_RewardConfig.BrushTime}开启活动，每次活动持续时间为1小时";
            activityIntroduceContent.text = infoConfig.ActivityIntroduce;
            rewardIntroduceContent.text = infoConfig.RewardIntroduce;
            Tile.text = infoConfig.Name;
            int count = 0;
            foreach (var item in rewards)
            {
                if (Content.GetChild(count) != null)
                {
                    Content.GetChild(count).GetComponent<Text>().text = item.Key;
                    Content.GetChild(count).Find("count").GetComponent<Text>().text = $"剩余{item.Value}只";
                    count++;
                }
            }
        }

        public async ETVoid NewYearMobCntRequest()
        {
                Log.DebugGreen("废物");
                G2C_NewYearMobCntResponse g2C_NewYearMob = (G2C_NewYearMobCntResponse)await SessionComponent.Instance.Session.Call(new C2G_NewYearMobCntRequest() { });
                if (g2C_NewYearMob.Error == 2400)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_NewYearMob.Error.GetTipInfo());
                    UIComponent.Instance.Remove(UIType.UINewYearActivity);
                    return;
                }
                if (g2C_NewYearMob.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_NewYearMob.Error.GetTipInfo());
                }
                else
                {
                    rewards.Clear();
                    for (int i = 0; i < g2C_NewYearMob.MobCnt.Count; i++)
                    {
                        rewards.Add(g2C_NewYearMob.MobCnt[i].MapID.ToEnum<SceneName, int>().GetSceneName(), g2C_NewYearMob.MobCnt[i].MobCnt);
                    }
                    LoadData();
                }
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
        }
    }
}