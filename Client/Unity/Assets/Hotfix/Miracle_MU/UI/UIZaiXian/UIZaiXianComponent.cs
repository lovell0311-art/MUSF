using Codice.Client.Common;
using ETModel;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIZaiXianComponentAwake : AwakeSystem<UIZaiXianComponent>
    {
        public override void Awake(UIZaiXianComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.time = self.collector.GetText("time");
            self.freeReward = self.collector.GetText("freeReward");
            self.reward = self.collector.GetText("reward");
            self.Awake();
        }
    }

    public class UIZaiXianComponent : Component
    {
        public ReferenceCollector collector;
        public List<string> timeList = new List<string>();
        public List<string> freeRewardList = new List<string>();
        public List<string> rewardList = new List<string>();
        public Text time, freeReward, reward;
        public int Limit = 20;
        public void Awake()
        {
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() => { UIComponent.Instance.Remove(UIType.UIZaiXian); });
            timeList.Add("累积时长/小时\n\n");
            freeRewardList.Add("(免费)奖励\n\n");
           // rewardList.Add("带有称号[主宰无双]\n\n");
            InitOnline();
        }

        public void InitOnline()
        {
            var onlineDuration_Reward = ConfigComponent.Instance.GetAll<OnlineDuration_RewardConfig>();
            foreach (OnlineDuration_RewardConfig online in onlineDuration_Reward.Cast<OnlineDuration_RewardConfig>())
            {
                if (online.Id >= Limit)
                {
                    timeList.Add($"{online.Time / 60f}\n");
                    rewardList.Add($"{online.Item}\n");
                }
                else
                {
                    freeRewardList.Add($"{online.Item}\n");
                }
            }
            time.text = timeList.ListToString().Replace(",","");
            freeReward.text = freeRewardList.ListToString().Replace(",", "");
            reward.text = rewardList.ListToString().Replace(",", "");
        }
    }

}
