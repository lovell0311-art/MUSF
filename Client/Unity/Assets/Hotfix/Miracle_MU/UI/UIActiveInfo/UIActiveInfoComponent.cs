using ETModel;
using ILRuntime.Runtime;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;
using Newtonsoft.Json;

namespace ETHotfix
{
    public static class Enums
    {
        public static T Next<T>(this T v) where T : struct
        {
            return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).SkipWhile(e => !v.Equals(e)).Skip(1).First();
        }

        public static T Previous<T>(this T v) where T : struct
        {
            return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).Reverse().SkipWhile(e => !v.Equals(e)).Skip(1).First();
        }
    }
    public class FuBenOpenTimeData
    {
        public long Id;
        public string Name;
        public string OpenTime1;
        public string OpenTime2;
        public string OpenTime3;
        public string OpenTime4;
        public string OpenTime5;
        public string OpenTime6;
        public string OpenTime7;
        public string OpenTime8;
        public string OpenTime9;
        public string OpenTime10;
        public string OpenTime11;
        public string OpenTime12;
        public string OpenTime13;
        public string OpenTime14;
        public string OpenTime15;
        public string OpenTime16;
        public string OpenTime17;
        public string OpenTime18;
        public string OpenTime19;
    }
    public class ActiveIntoTimeData
    {
        public string AM_PM_time;
        public string Time;
    }
    [ObjectSystem]
    public class UIActiveInfoComponentAwake : AwakeSystem<UIActiveInfoComponent>
    {
        public override void Awake(UIActiveInfoComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.Content = self.collector.GetGameObject("Content");
            self.scrollRect = self.collector.GetImage("Scroll View").GetComponent<ScrollRect>();
            self.FuBenName = self.collector.GetText("FuBenName");
            self.Title = self.collector.GetText("Title");
            self.AM_PMOrMapLevel = self.collector.GetText("AM_PMOrMapLevel");
            self.TimeOrRoleLevel = self.collector.GetText("TimeOrRoleLevel");

            self.InitBtn();
            self.InitUICircular();
        }
    }
    public class UIActiveInfoComponent : Component
    {
        public ReferenceCollector collector;
        public GameObject Content;
        public ScrollRect scrollRect;
        public Text FuBenName,Title;
        public Text AM_PMOrMapLevel, TimeOrRoleLevel;
        public UICircularScrollView<ActiveIntoTimeData> uICircular;
        public List<ActiveIntoTimeData> activeConditionList = new List<ActiveIntoTimeData>();
        public string FubenTitle;
        FuBenname fuBenname_local;
        ActiveType activeType_local;

        public void InitBtn()
        {
            //关闭按钮
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UIActiveInfo);
            });
            //右键，更新活动信息
            collector.GetButton("JianTouRightBtn").onClick.AddSingleListener(() =>
            {
                SetActiveInfo(fuBenname_local.Next(), FubenTitle, activeType_local);
            });
            //左键，更新活动信息
            collector.GetButton("JiantouLeftBtn").onClick.AddSingleListener(() =>
            {
                SetActiveInfo(fuBenname_local.Previous(), FubenTitle, activeType_local);
            });
        }
        public void InitUICircular()
        {
            uICircular = ComponentFactory.Create<UICircularScrollView<ActiveIntoTimeData>>();
            uICircular.InitInfo(E_Direction.Vertical, 1, 0, 10);
            uICircular.ItemInfoCallBack = InitItem;
            uICircular.IninContent(Content, scrollRect);
        }

        private void InitItem(GameObject item, ActiveIntoTimeData activeIntoTimeData)
        {
            item.transform.Find("AM_PMOrMapLevel").GetComponent<Text>().text = activeIntoTimeData.AM_PM_time;
            item.transform.Find("TimeOrRoleLevel").GetComponent<Text>().text = activeIntoTimeData.Time;
        }

        public void SetActiveInfo(FuBenname fuBenname, string fubenTitle, ActiveType activeType)
        {
            activeType_local = activeType;
            FubenTitle = fubenTitle;
            fuBenname_local = fuBenname;
            FuBenName.text = GetFuBenName();
            Title.text = fubenTitle;
            switch (activeType)
            {
                case ActiveType.ActiveIntoTime:
                    AM_PMOrMapLevel.text = "AM/PM";
                    TimeOrRoleLevel.text = "小时";
                    EMoData(fuBenname);
                    break;
                case ActiveType.ActiveIntoLevel:
                    AM_PMOrMapLevel.text = "地图等级";
                    TimeOrRoleLevel.text = "角色等级";
                    XueSeData(fuBenname);
                    break;
                default:
                    break;
            }

            string GetFuBenName() => fuBenname switch
            {
                FuBenname.EMoSquare => "恶魔广场",
                FuBenname.XueSeCastle => "血色城堡",
                _=>null
            };
        }
        public void EMoData(FuBenname fuBenname)
        {
            activeConditionList.Clear();
            BattleCopyConfig_OpenConfig battleCopyConfig_Open = ConfigComponent.Instance.GetItem<BattleCopyConfig_OpenConfig>(fuBenname.ToInt32() + 1);
            
            ActiveIntoTimeData activeIntoTimeData1 = new ActiveIntoTimeData();
            activeIntoTimeData1.Time = battleCopyConfig_Open.OpenTime1.Replace('+', ':');
            activeIntoTimeData1.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData1);
            ActiveIntoTimeData activeIntoTimeData2 = new ActiveIntoTimeData();
            activeIntoTimeData2.Time = battleCopyConfig_Open.OpenTime2.Replace('+', ':');
            activeIntoTimeData2.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData2);

            ActiveIntoTimeData activeIntoTimeData3 = new ActiveIntoTimeData();
            activeIntoTimeData3.Time = battleCopyConfig_Open.OpenTime3.Replace('+', ':');
            activeIntoTimeData3.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData3);

            ActiveIntoTimeData activeIntoTimeData4 = new ActiveIntoTimeData();
            activeIntoTimeData4.Time = battleCopyConfig_Open.OpenTime4.Replace('+', ':');
            activeIntoTimeData4.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData4);

            ActiveIntoTimeData activeIntoTimeData5 = new ActiveIntoTimeData();
            activeIntoTimeData5.Time = battleCopyConfig_Open.OpenTime5.Replace('+', ':');
            activeIntoTimeData5.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData5);

            ActiveIntoTimeData activeIntoTimeData6 = new ActiveIntoTimeData();
            activeIntoTimeData6.Time = battleCopyConfig_Open.OpenTime6.Replace('+', ':');
            activeIntoTimeData6.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData6);

            ActiveIntoTimeData activeIntoTimeData7 = new ActiveIntoTimeData();
            activeIntoTimeData7.Time = battleCopyConfig_Open.OpenTime7.Replace('+', ':');
            activeIntoTimeData7.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData7);

            ActiveIntoTimeData activeIntoTimeData8 = new ActiveIntoTimeData();
            activeIntoTimeData8.Time = battleCopyConfig_Open.OpenTime8.Replace('+', ':');
            activeIntoTimeData8.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData8);

            ActiveIntoTimeData activeIntoTimeData9 = new ActiveIntoTimeData();
            activeIntoTimeData9.Time = battleCopyConfig_Open.OpenTime9.Replace('+', ':');
            activeIntoTimeData9.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData9);

            ActiveIntoTimeData activeIntoTimeData10 = new ActiveIntoTimeData();
            activeIntoTimeData10.Time = battleCopyConfig_Open.OpenTime10.Replace('+', ':');
            activeIntoTimeData10.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData10);

            ActiveIntoTimeData activeIntoTimeData11 = new ActiveIntoTimeData();
            activeIntoTimeData11.Time = battleCopyConfig_Open.OpenTime11.Replace('+', ':');
            activeIntoTimeData11.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData11);

            ActiveIntoTimeData activeIntoTimeData12 = new ActiveIntoTimeData();
            activeIntoTimeData12.Time = battleCopyConfig_Open.OpenTime12.Replace('+', ':');
            activeIntoTimeData12.AM_PM_time = "AM";
            activeConditionList.Add(activeIntoTimeData12);

            ActiveIntoTimeData activeIntoTimeData13 = new ActiveIntoTimeData();
            activeIntoTimeData13.Time = battleCopyConfig_Open.OpenTime13.Replace('+', ':');
            activeIntoTimeData13.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData13);

            ActiveIntoTimeData activeIntoTimeData14 = new ActiveIntoTimeData();
            activeIntoTimeData14.Time = battleCopyConfig_Open.OpenTime14.Replace('+', ':');
            activeIntoTimeData14.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData14);

            ActiveIntoTimeData activeIntoTimeData15 = new ActiveIntoTimeData();
            activeIntoTimeData15.Time = battleCopyConfig_Open.OpenTime15.Replace('+', ':');
            activeIntoTimeData15.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData15);

            ActiveIntoTimeData activeIntoTimeData16 = new ActiveIntoTimeData();
            activeIntoTimeData16.Time = battleCopyConfig_Open.OpenTime16.Replace('+', ':');
            activeIntoTimeData16.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData16);
            ActiveIntoTimeData activeIntoTimeData17 = new ActiveIntoTimeData();
            activeIntoTimeData17.Time = battleCopyConfig_Open.OpenTime17.Replace('+', ':');
            activeIntoTimeData17.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData17);
            ActiveIntoTimeData activeIntoTimeData18 = new ActiveIntoTimeData();
            activeIntoTimeData18.Time = battleCopyConfig_Open.OpenTime18.Replace('+', ':');
            activeIntoTimeData18.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData18);
            ActiveIntoTimeData activeIntoTimeData19 = new ActiveIntoTimeData();
            activeIntoTimeData19.Time = battleCopyConfig_Open.OpenTime19.Replace('+', ':');
            activeIntoTimeData19.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData19);
            ActiveIntoTimeData activeIntoTimeData20 = new ActiveIntoTimeData();
            activeIntoTimeData20.Time = battleCopyConfig_Open.OpenTime20.Replace('+', ':');
            activeIntoTimeData20.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData20);
            ActiveIntoTimeData activeIntoTimeData21 = new ActiveIntoTimeData();
            activeIntoTimeData21.Time = battleCopyConfig_Open.OpenTime21.Replace('+', ':');
            activeIntoTimeData21.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData21);
            ActiveIntoTimeData activeIntoTimeData22 = new ActiveIntoTimeData();
            activeIntoTimeData22.Time = battleCopyConfig_Open.OpenTime22.Replace('+', ':');
            activeIntoTimeData22.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData22);
            ActiveIntoTimeData activeIntoTimeData23 = new ActiveIntoTimeData();
            activeIntoTimeData23.Time = battleCopyConfig_Open.OpenTime23.Replace('+', ':');
            activeIntoTimeData23.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData23);
            ActiveIntoTimeData activeIntoTimeData24 = new ActiveIntoTimeData();
            activeIntoTimeData24.Time = battleCopyConfig_Open.OpenTime24.Replace('+', ':');
            activeIntoTimeData24.AM_PM_time = "PM";
            activeConditionList.Add(activeIntoTimeData24);
            //=========================================================================================



            //ActiveIntoTimeData activeIntoTimeData7 = new ActiveIntoTimeData();
            //activeIntoTimeData7.Time = $"{battleCopyConfig_Open.OpenTime7.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime7.Split('+')[1]}" ;
            //activeIntoTimeData7.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData7);

            //ActiveIntoTimeData activeIntoTimeData8 = new ActiveIntoTimeData();
            //activeIntoTimeData8.Time = $"{battleCopyConfig_Open.OpenTime8.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime8.Split('+')[1]}";
            //activeIntoTimeData8.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData8);

            //ActiveIntoTimeData activeIntoTimeData9 = new ActiveIntoTimeData();
            //activeIntoTimeData9.Time = $"{battleCopyConfig_Open.OpenTime9.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime9.Split('+')[1]}";
            //activeIntoTimeData9.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData9);

            //ActiveIntoTimeData activeIntoTimeData10 = new ActiveIntoTimeData();
            //activeIntoTimeData10.Time = $"{battleCopyConfig_Open.OpenTime10.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime10.Split('+')[1]}";
            //activeIntoTimeData10.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData10);

            //ActiveIntoTimeData activeIntoTimeData11 = new ActiveIntoTimeData();
            //activeIntoTimeData11.Time = $"{battleCopyConfig_Open.OpenTime11.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime11.Split('+')[1]}";
            //activeIntoTimeData11.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData11);

            //ActiveIntoTimeData activeIntoTimeData12 = new ActiveIntoTimeData();
            //activeIntoTimeData12.Time = $"{battleCopyConfig_Open.OpenTime12.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime12.Split('+')[1]}";
            //activeIntoTimeData12.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData12);

            //ActiveIntoTimeData activeIntoTimeData13 = new ActiveIntoTimeData();
            //activeIntoTimeData13.Time = $"{battleCopyConfig_Open.OpenTime13.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime13.Split('+')[1]}";
            //activeIntoTimeData13.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData13);

            //ActiveIntoTimeData activeIntoTimeData14 = new ActiveIntoTimeData();
            //activeIntoTimeData14.Time = $"{battleCopyConfig_Open.OpenTime14.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime14.Split('+')[1]}";
            //activeIntoTimeData14.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData14);

            //ActiveIntoTimeData activeIntoTimeData15 = new ActiveIntoTimeData();
            //activeIntoTimeData15.Time = $"{battleCopyConfig_Open.OpenTime15.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime15.Split('+')[1]}";
            //activeIntoTimeData15.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData15);

            //ActiveIntoTimeData activeIntoTimeData16 = new ActiveIntoTimeData();
            //activeIntoTimeData16.Time = $"{battleCopyConfig_Open.OpenTime16.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime16.Split('+')[1]}";
            //activeIntoTimeData16.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData16);

            //ActiveIntoTimeData activeIntoTimeData17 = new ActiveIntoTimeData();
            //activeIntoTimeData17.Time = $"{battleCopyConfig_Open.OpenTime17.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime17.Split('+')[1]}";
            //activeIntoTimeData17.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData17);

            //ActiveIntoTimeData activeIntoTimeData18 = new ActiveIntoTimeData();
            //activeIntoTimeData18.Time = $"{battleCopyConfig_Open.OpenTime18.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime18.Split('+')[1]}";
            //activeIntoTimeData18.AM_PM_time = "PM";
            //activeConditionList.Add(activeIntoTimeData18);

            /*  ActiveIntoTimeData activeIntoTimeData19 = new ActiveIntoTimeData();
              activeIntoTimeData19.Time = $"{battleCopyConfig_Open.OpenTime19.Split('+')[0].ToInt32() - 12}:{battleCopyConfig_Open.OpenTime19.Split('+')[1]}";
              activeIntoTimeData19.AM_PM_time = "PM";
              activeConditionList.Add(activeIntoTimeData19);*/


            uICircular.Items = activeConditionList;




        }

        public void XueSeData(FuBenname fuBenname)
        {
            activeConditionList.Clear();
            BattleIntoInfo_IntoConfig battleIntoInfo_IntoConfig = ConfigComponent.Instance.GetItem<BattleIntoInfo_IntoConfig>(fuBenname.ToInt32() + 1);

            ActiveIntoTimeData activeIntoTimeData1 = new ActiveIntoTimeData();
            activeIntoTimeData1.Time = battleIntoInfo_IntoConfig.IntoRoleLevel1;
            activeIntoTimeData1.AM_PM_time = "1";
            activeConditionList.Add(activeIntoTimeData1);

            ActiveIntoTimeData activeIntoTimeData2 = new ActiveIntoTimeData();
            activeIntoTimeData2.Time = battleIntoInfo_IntoConfig.IntoRoleLevel2;
            activeIntoTimeData2.AM_PM_time = "2";
            activeConditionList.Add(activeIntoTimeData2);

            ActiveIntoTimeData activeIntoTimeData3 = new ActiveIntoTimeData();
            activeIntoTimeData3.Time = battleIntoInfo_IntoConfig.IntoRoleLevel3;
            activeIntoTimeData3.AM_PM_time = "3";
            activeConditionList.Add(activeIntoTimeData3);

            ActiveIntoTimeData activeIntoTimeData4 = new ActiveIntoTimeData();
            activeIntoTimeData4.Time = battleIntoInfo_IntoConfig.IntoRoleLevel4;
            activeIntoTimeData4.AM_PM_time = "4";
            activeConditionList.Add(activeIntoTimeData4);

            ActiveIntoTimeData activeIntoTimeData5 = new ActiveIntoTimeData();
            activeIntoTimeData5.Time = battleIntoInfo_IntoConfig.IntoRoleLevel5;
            activeIntoTimeData5.AM_PM_time = "5";
            activeConditionList.Add(activeIntoTimeData5);

            ActiveIntoTimeData activeIntoTimeData6 = new ActiveIntoTimeData();
            activeIntoTimeData6.Time = battleIntoInfo_IntoConfig.IntoRoleLevel6;
            activeIntoTimeData6.AM_PM_time = "6";
            activeConditionList.Add(activeIntoTimeData6);

            ActiveIntoTimeData activeIntoTimeData7 = new ActiveIntoTimeData();
            activeIntoTimeData7.Time = battleIntoInfo_IntoConfig.IntoRoleLevel7;
            activeIntoTimeData7.AM_PM_time = "7";
            activeConditionList.Add(activeIntoTimeData7);


            uICircular.Items = activeConditionList;
        }
    }

}
