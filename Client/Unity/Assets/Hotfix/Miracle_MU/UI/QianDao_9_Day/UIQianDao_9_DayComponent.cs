using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;
using UnityEngine.UI;



namespace ETHotfix
{
    [ObjectSystem]
    public class UIQianDao_9_DayComponentAwake : AwakeSystem<UIQianDao_9_DayComponent>
    {
        public override void Awake(UIQianDao_9_DayComponent self)
        {
            self.referenceCollector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.referenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(()=>UIComponent.Instance.Remove(UIType.UIQianDao_9_Day));
            self.InitTitle_Receive();
        }
    }

    /// <summary>
    /// 七日充值
    /// </summary>
    public class UIQianDao_9_DayComponent : Component
    {
       public ReferenceCollector referenceCollector;

        //签到
        public void InitTitle_Receive() 
        {
            for (int i = 1; i <= 9; i++) 
            {
                Transform item = referenceCollector.GetImage($"Item_{i}").transform;
                int day = i;

                SevenDay_RewardPropsConfig sevenDay_ = ConfigComponent.Instance.GetItem<SevenDay_RewardPropsConfig>(i);

                item.Find("TopUp").Find("Text").GetComponent<Text>().text = "签到";
               // item.Find("Image/Text").GetComponent<Text>().text = $"奇迹币{sevenDay_.MagicCrystal}";
                if (GlobalDataManager.SevenDaysToRechargeDic.ContainsKey(i) && GlobalDataManager.SevenDaysToRechargeDic[i])
                {
                    //已领取
                    item.Find("TopUp").Find("Text").GetComponent<Text>().text = "已签到";
                    item.Find("TopUp").GetComponent<Button>().interactable = false;
                    continue;
                }
                item.Find("TopUp").GetComponent<Button>().onClick.AddSingleListener(async () =>
                {
                    //Log.DebugGreen("签到");
                   ///签到
                   G2C_NationalDaySignin g2C_ShopMallBuyItemResponse = (G2C_NationalDaySignin)await SessionComponent.Instance.Session.Call(new C2G_NationalDaySignin{});
                   if (g2C_ShopMallBuyItemResponse.Error != 0)
                   {
                       UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ShopMallBuyItemResponse.Message);

                        //UIComponent.Instance.VisibleUI(UIType.UIHint, "签到失败");
                    }
                   else
                   {
                       UIComponent.Instance.VisibleUI(UIType.UIHint, "签到成功");
                       GlobalDataManager.SevenDaysToRechargeDic[day] = true;
                       item.Find("TopUp").Find("Text").GetComponent<Text>().text = "已签到";
                       item.Find("TopUp").GetComponent<Button>().interactable = false;
                   }

               });
            }
        }
        public bool IsCanCToday()
        {
            string todayTimeStr = System.DateTime.Now.ToString("yyyy/M/d");
            string timeStr = null;
            for (int i = 1; i <= 9; i++)
            {
                if (GlobalDataManager.SevenDaysToRechargeDic2.ContainsKey(i))
                {
                    timeStr = GlobalDataManager.SevenDaysToRechargeDic2[i];
                    if (timeStr == todayTimeStr)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            TopUp_7_DayComponent.Instance = null;

        }
    }
}
