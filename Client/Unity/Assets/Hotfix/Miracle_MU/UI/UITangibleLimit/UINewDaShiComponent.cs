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
    public class UINewDaShiComponentAwake : AwakeSystem<UINewDaShiComponent>
    {
        public override void Awake(UINewDaShiComponent self)
        {

            Log.DebugBrown("这是大师界面");
            self.reference = self.GetParent<UI>().GameObject.GetReferenceCollector();
            //--

            //  self.InitUi();
            self.reference.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
               
                UIComponent.Instance.Remove(UIType.UIDaShiNew);
            });
        }
    }
    public partial class UINewDaShiComponent : Component
    {

        public ReferenceCollector reference;
        public GameObject root1, root2, root3;
        public Image tips;
        public Text countTxt;//大师点数
        public Dictionary<int, List<int>> DicIteminfo = new Dictionary<int, List<int>>();//储存表的数据(A,B)
        public Dictionary<long, long> dicotherdata = new Dictionary<long, long>();
        //public Dictionary<int, int> masterDic = new Dictionary<int, int>();
        public int receiveStatus = 0, isbool = 1;
        //public void InitUi()
        //{
        //    root1 = reference.GetGameObject("root");
        //    root2 = reference.GetGameObject("root2");
        //    root3 = reference.GetGameObject("root3");
        //    tips = reference.GetImage("tips");
        //    countTxt = reference.GetText("countTxt");
        //    if (root1.transform.childCount > 0)
        //    {
        //        for (int i = 0; i < root1.transform.childCount; i++)
        //        {
        //            int index = i;
        //            root1.transform.GetChild(i).GetComponent<Button>().onClick.AddSingleListener(() =>
        //            {
        //                Log.DebugBrown("点击的是" + root1.transform.GetChild(index).name);

        //                OnTips(int.Parse(root1.transform.GetChild(index).name));
        //                tips.gameObject.SetActive(true);
        //            });

        //        }
        //    }
        //    if (root2.transform.childCount > 0)
        //    {
        //        for (int i = 0; i < root2.transform.childCount; i++)
        //        {
        //            int index = i;
        //            root2.transform.GetChild(i).GetComponent<Button>().onClick.AddSingleListener(() =>
        //            {
        //                Log.DebugBrown("点击的是" + root2.transform.GetChild(index).name);

        //                OnTips(int.Parse(root2.transform.GetChild(index).name));
        //                tips.gameObject.SetActive(true);
        //            });
        //        }
        //    }
        //    if (root3.transform.childCount > 0)
        //    {
        //        for (int i = 0; i < root3.transform.childCount; i++)
        //        {
        //            int index = i;
        //            // root1.transform.GetChild(i).gameObject.SetActive(true);
        //            root3.transform.GetChild(i).GetComponent<Button>().onClick.AddSingleListener(() =>
        //            {
        //                Log.DebugBrown("点击的是" + root3.transform.GetChild(index).name);

        //                OnTips(int.Parse(root3.transform.GetChild(index).name));
        //                tips.gameObject.SetActive(true);
        //            });
        //        }
        //    }
        //    reference.GetButton("Btn_type1").onClick.AddSingleListener(() =>
        //    {
        //        isbool = 1;
        //        SetTatenInfo();
        //        reference.GetButton("Btn_type1").transform.Find("checked").gameObject.SetActive(true);
        //        reference.GetButton("Btn_type2").transform.Find("checked").gameObject.SetActive(false);
        //        reference.GetButton("Btn_type3").transform.Find("checked").gameObject.SetActive(false);
        //    });
        //    reference.GetButton("Btn_type2").onClick.AddSingleListener(() =>
        //    {
        //        isbool = 2;
        //        SetTatenInfo();
        //        reference.GetButton("Btn_type1").transform.Find("checked").gameObject.SetActive(false);
        //        reference.GetButton("Btn_type2").transform.Find("checked").gameObject.SetActive(true);
        //        reference.GetButton("Btn_type3").transform.Find("checked").gameObject.SetActive(false);
        //    });
        //    reference.GetButton("Btn_type3").onClick.AddSingleListener(() =>
        //    {
        //        isbool = 3;
        //        SetTatenInfo();
        //        reference.GetButton("Btn_type1").transform.Find("checked").gameObject.SetActive(false);
        //        reference.GetButton("Btn_type2").transform.Find("checked").gameObject.SetActive(false);
        //        reference.GetButton("Btn_type3").transform.Find("checked").gameObject.SetActive(true);
        //    });
        //    //请求数据
        //    RequestDaShi().Coroutine();
        //}


        ///// <summary>
        ///// 天赋树状态
        ///// </summary>
        //public void SetTatenInfo()
        //{
        //    reference.GetImage("Show1").gameObject.SetActive(isbool == 1);
        //    reference.GetImage("Show2").gameObject.SetActive(isbool == 2);
        //    reference.GetImage("Show3").gameObject.SetActive(isbool == 3);

        //    if (root1.transform.childCount > 0)
        //    {

        //        for (int i = 0; i < root1.transform.childCount; i++)
        //        {
        //            root1.transform.GetChild(i).Find("img2").gameObject.SetActive(false);
        //            root1.transform.GetChild(i).Find("img1").gameObject.SetActive(false);
        //            root1.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"<color=red>{0}</color>/10";
        //        }
        //        foreach (var item in masterDic)
        //        {
        //            for (int i = 0; i < root1.transform.childCount; i++)
        //            {
        //                if (item.Key == int.Parse(root1.transform.GetChild(i).name))
        //                {
        //                    if (item.Value >= 10)
        //                    {

        //                        root1.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"<color=green>{item.Value}</color>/10";
        //                    }
        //                    else
        //                    {

        //                        root1.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"<color=red>{item.Value}</color>/10";
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (root2.transform.childCount > 0)
        //    {
        //        for (int i = 0; i < root2.transform.childCount; i++)
        //        {
        //            root2.transform.GetChild(i).Find("img2").gameObject.SetActive(false);
        //            root2.transform.GetChild(i).Find("img1").gameObject.SetActive(false);
        //            root2.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"<color=red>{0}</color>/10";
        //        }
        //        foreach (var item in masterDic)
        //        {
        //            for (int i = 0; i < root2.transform.childCount; i++)
        //            {
        //                if (item.Key == int.Parse(root2.transform.GetChild(i).name))
        //                {
        //                    if (item.Value >= 10)
        //                    {

        //                        root2.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"<color=green>{item.Value}</color>/10";
        //                    }
        //                    else
        //                    {

        //                        root2.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"<color=red>{item.Value}</color>/10";
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (root3.transform.childCount > 0)
        //    {
        //        for (int i = 0; i < root3.transform.childCount; i++)
        //        {
        //            root3.transform.GetChild(i).Find("img2").gameObject.SetActive(false);
        //            root3.transform.GetChild(i).Find("img1").gameObject.SetActive(false);
        //            root3.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"<color=red>{0}</color>/10";
        //        }
        //        foreach (var item in masterDic)
        //        {
        //            for (int i = 0; i < root3.transform.childCount; i++)
        //            {
        //                if (item.Key == int.Parse(root3.transform.GetChild(i).name))
        //                {
        //                    if (item.Value >= 10)
        //                    {

        //                        root3.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"<color=green>{item.Value}</color>/10";
        //                    }
        //                    else
        //                    {

        //                        root3.transform.GetChild(i).Find("Text").GetComponent<Text>().text = $"<color=red>{item.Value}</color>/10";
        //                    }
        //                }
        //            }
        //        }
        //    }

        //}




        ///// <summary>
        ///// 弹窗内容
        ///// </summary>
        ///// <param name="index"></param>
        //public void OnTips(int index)
        //{
        //    BattleMaster_CareerConfig config = ConfigComponent.Instance.GetItem<BattleMaster_CareerConfig>(index);
        //    Log.DebugBrown("点击的大师技能" + config.Name);
        //    tips.transform.Find("miaoshu").GetComponent<Text>().text = config.Name;
        //    dicotherdata.Clear();
        //    dicotherdata = GlobalDataManager.GetDictionary(config.OtherData);
        //    bool jk = true;
        //    if (config.LastIds.Count > 0)
        //    {
        //        for (int i = 0; i < config.LastIds.Count; i++)
        //        {
        //            if (masterDic.ContainsKey(config.LastIds[i]))
        //            {
        //                jk = true;
        //            }
        //            else
        //            {
        //                jk = false;
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Log.DebugBrown("该技能不需要解锁");
        //    }
        //    Log.DebugBrown("分割线============");
        //    if (jk == false)//未解锁
        //    {
        //        if (config.LastIds.Count > 0)
        //        {

        //            tips.transform.Find("Des").GetComponent<Text>().text = "解锁不满足条件，需解锁上一项";
        //        }
        //        //else
        //        //{
        //        //    tips.transform.Find("Des").GetComponent<Text>().text = "消耗大师点，解锁该属性";
        //        //}
        //    }
        //    else if (masterDic.ContainsKey(index) == false)
        //    {
        //        tips.transform.Find("Des").GetComponent<Text>().text = "消耗大师点，解锁该属性";
        //    }
        //    else//已经解锁
        //    {
        //        string infodata = "当前技能的等级：" + masterDic[index] + "Lv" + '\n' + config.Describe;

        //        Log.DebugBrown("infodata" + infodata);
        //        int value = (int)dicotherdata[masterDic[index]];
        //        if (false)
        //        {

        //        }
        //        else
        //        {
        //            if (config.OtherDataUse == 2)
        //            {
        //                if (config.Describe.Equals("{0:F}%"))
        //                {
        //                    infodata = config.Describe.Replace("{0:F}", ((1f / value) * 10000 * 100).ToString("F2")) + '\n' + "下一级提高至" + ((1f / (dicotherdata[masterDic[index] + 1])) * 10000 * 100).ToString("F2");
        //                }
        //                else
        //                {
        //                    infodata = config.Describe.Replace("{0:F}", (value / 10000).ToString("F2")) + '\n' + "下一级提高至" + ((dicotherdata[masterDic[index] + 1]) / 10000).ToString("F2");
        //                }

        //                infodata = infodata.Replace("/", "");
        //            }
        //            else if (config.OtherDataUse == 1)
        //            {
        //                infodata = config.Describe.Replace("{0:F}", ((float)value / 100).ToString()) + '\n' + "下一级提高至" + ((float)dicotherdata[masterDic[index] + 1] / 100);
        //            }
        //            else
        //            {
        //                infodata = config.Describe.Replace("{0:F}", value.ToString()) + '\n' + "下一级提高至" + (dicotherdata[masterDic[index] + 1] + 1);
        //            }
        //        }

        //        tips.transform.Find("Des").GetComponent<Text>().text = infodata + '\n' + "当前技能的等级：" + masterDic[index] + "Lv";
        //    }
        //    tips.transform.Find("Btn_yes").GetComponent<Button>().onClick.AddSingleListener(() =>
        //    {
        //        //UIComponent.Instance.VisibleUI(UIType.UIHint, "前置技能未解锁或该技能已加满10级");
        //        //if (masterDic.Count>0)
        //        //{
        //        //    if (!masterDic.ContainsKey(index))
        //        //    {
        //        //        if (jk == false || masterDic[index] >= 10)
        //        //        {
        //        //            UIComponent.Instance.VisibleUI(UIType.UIHint, "前置技能未解锁或该技能已加满10级");
        //        //            return;
        //        //        }
        //        //        else
        //        //        {
        //        //            AddMaterPoint(index).Coroutine();
        //        //        }
        //        //    }

        //        //}
        //        //else
        //        //{
        //        //    AddMaterPoint(index).Coroutine();
        //        //}
        //        if (jk == true)
        //        {

        //            AddMaterPoint(index).Coroutine();
        //        }

        //    });
        //    tips.transform.Find("Btn_no").GetComponent<Button>().onClick.AddSingleListener(() =>
        //    {
        //        tips.gameObject.SetActive(false);
        //    });
        //}




        ///// <summary>
        ///// 加点
        ///// </summary>
        ///// <returns></returns>
        //public async ETTask AddMaterPoint(int index)
        //{
        //    Log.DebugGreen("本次加点请求的id是" + index);
        //    //判断是否可以加点
        //    G2C_BattleMasterUpdateLevelResponse g2C_BattleMaster = (G2C_BattleMasterUpdateLevelResponse)await SessionComponent.Instance.Session.Call(new C2G_BattleMasterUpdateLevelRequest()
        //    {
        //        BattleMasterId = index
        //    });
        //    Log.DebugBrown("返回大师g2C_BattleMaster" + JsonHelper.ToJson(g2C_BattleMaster));
        //    if (g2C_BattleMaster.Error != 0)
        //    {
        //        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_BattleMaster.Error.GetTipInfo());
        //    }
        //    else
        //    {

        //        countTxt.text = "大师点数：" + g2C_BattleMaster.PropertyPoint.ToString();
        //        foreach (var item in g2C_BattleMaster.Info)
        //        {
        //            if (!masterDic.ContainsKey(item.Key))
        //            {
        //                masterDic.Add(item.Key, (int)item.Value);
        //            }
        //            else
        //            {
        //                masterDic[item.Key] = (int)item.Value;
        //            }
        //            //if (masterDic.ContainsKey(item.Key)&&masterDic[item.Key]!=item.Value)
        //            //{
        //            //    masterDic[item.Key] = (int)item.Value;
        //            //}
        //        }
        //        //Log.DebugBrown("更新数据后：" + JsonHelper.ToJson(masterDic));
        //        SetTatenInfo();
        //        OnTips(index);
        //    }
        //}


        //public async ETTask RequestDaShi()
        //{
        //    G2C_OpenBattleMasterResponse g2C_OpenBattleMaster = (G2C_OpenBattleMasterResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenBattleMasterRequest() { });
        //    Log.DebugBrown("获取到大师数据是" + JsonHelper.ToJson(g2C_OpenBattleMaster));
        //    if (g2C_OpenBattleMaster.Error != 0)
        //    {
        //        UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_OpenBattleMaster.Error.GetTipInfo()}");
        //    }
        //    else
        //    {
        //        countTxt.text = "大师点数：" + g2C_OpenBattleMaster.PropertyPoint.ToString();
        //        foreach (var item in g2C_OpenBattleMaster.Info)
        //        {

        //            masterDic.Add(item.Key, (int)item.Value);
        //        }
        //        SetTatenInfo();

        //        //  SetSkillTitle();
        //    }
        //}


    }
}
