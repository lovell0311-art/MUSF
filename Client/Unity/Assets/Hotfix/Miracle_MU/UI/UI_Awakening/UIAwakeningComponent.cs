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
    public class UIAwakeningComponentAwake : AwakeSystem<UIAwakeningComponent>
    {
        public override void Awake(UIAwakeningComponent self)
        {
            self.reference = self.GetParent<UI>().GameObject.GetReferenceCollector();
            //--
           
            self.InitUi();
        }

    }


    [ObjectSystem]
    public class UIAwakeningComponentUpdate : UpdateSystem<UIAwakeningComponent>
    {
        public override void Update(UIAwakeningComponent self)
        {
            if (self.timestamp!=0)
            {
              self.progress_bg.transform.Find("time_text").GetComponent<Text>().text = "剩余进度：" + TimeHelper.DifferenceSecondsToTime(self.timestamp);
                self.progress_bg.transform.Find("progress").GetComponent<Image>().fillAmount = 1-(float)TimeHelper.GetTimestamp() / self.timestamp;
            }
        }

    }
    public partial class UIAwakeningComponent : Component
    {
        

        public int receiveStatus = 0;
        public Image DrawPrizes, Good;//获奖面板/物品详情
        public long times = 0;//下次领奖的时间戳
        /// <summary>
        /// ///////////
        /// </summary>
        public ReferenceCollector reference;
        public Dictionary<int, Prize> PrizeDic = new Dictionary<int, Prize>();//奖品字典 key:奖品所在位置 value:奖品信息
        public Dictionary<int, int> OneLoop = new Dictionary<int, int>();//一环
        public Dictionary<int, int> TwoLoop = new Dictionary<int, int>();//二环
        public Dictionary<int, int> ThreeLoop = new Dictionary<int, int>();//三环
        public Dictionary<int, int> FourLoop = new Dictionary<int, int>();//四环
        public Dictionary<int, int> FiveLoop = new Dictionary<int, int>();//五环
        public Dictionary<int, int> ConsumeDic = new Dictionary<int, int>();//消耗品
        public Dictionary<int, int> NodeDic = new Dictionary<int, int>();//存储已解锁的节点属性数据
        public List<int> BloodIdlist = new List<int>();//当前已经解锁的系统
        public List<int> PrizeList = new List<int>();//中奖ID集合
        public BloodVesselInfo bloodinfo = new BloodVesselInfo();//血脉数据
        public bool isOnClickPlaying;
        public bool IsOnClickPlaying
        {
            get => isOnClickPlaying;
            set
            {
                isOnClickPlaying = value;
            }
        }

        /// <summary>
        /// ----------------------------------------------
        /// </summary>
        /// pr
        private Text Des;
        public Image tips, yuandian, img_obj, progress_bg;
        private Button CloseBtn, Btn1, Btn2, Btn3, Use_Btn, Btn_Close, Btn_evolution;
        public IConfig[] config;
        public long timestamp = 0;
        public Dictionary<int, List<int>> DicIteminfo = new Dictionary<int, List<int>>();//储存表的数据
        /// <summary>
        /// 当前血脉id
        /// </summary>
        public int TypeSystem = 1;
        /// <summary>
        /// 当前环id
        /// </summary>
        public int Loop = 0;
        /// <summary>
        /// 当前的节点id
        /// </summary>
        public int Node = 0;

        /// <summary>
        /// 当前持有的最高环
        /// </summary>
        public int MaxLoop = 1;

        /// <summary>
        /// 重新显示ui状态
        /// </summary>
        /// 
        public void OnRefresh()
        {
        }


        public void DistributeObjects(BloodVesselInfo bloodinfo)
        {
            Des.text = null;
            if (NodeDic.Count>0)
            {
                string consume_des = null;
                foreach (var item in NodeDic)
                {
                    AttributeNode_InfoConfig Attribute_info = ConfigComponent.Instance.GetItem<AttributeNode_InfoConfig>(item.Key);
                    if (Attribute_info == null)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "AttributeNode_InfoConfig未找到ID为:" +item.Key);
                        return;
                    }
                    ConsumeDic = GlobalDataManager.GetDictionary(Attribute_info.ActivateNeed);
                    
                    //foreach (var item1 in ConsumeDic)
                    //{
                    //    ((long)item1.Key).GetItemInfo_Out(out Item_infoConfig item_Info);
                    //    consume_text += $"<color=green>{item_Info.Name + "X" + item1.Value}</color>\n";
                    //}
                    for (int i = 0; i < Attribute_info.AttributeNode.Count; i++)
                    {
                        ItemAttrEntry_BaseConfig itemAttrEntry_Base = ConfigComponent.Instance.GetItem<ItemAttrEntry_BaseConfig>(Attribute_info.AttributeNode[i]);
                        consume_des += string.Format(itemAttrEntry_Base.Name, itemAttrEntry_Base.Value0)+'\n';

                    }
                }
                Des.text = consume_des;
            }
    
            timestamp = bloodinfo.NextStage;
            progress_bg.gameObject.SetActive(timestamp != 0);
            if (progress_bg.gameObject.activeSelf==true)
            {
                progress_bg.transform.Find("time_text").GetComponent<Text>().text ="剩余进度："+ TimeHelper.DifferenceSecondsToTime(timestamp);
            }


            OneLoop.Clear();
            TwoLoop.Clear();
            ThreeLoop.Clear();
            FourLoop.Clear();
            FiveLoop.Clear();
           
            BloodAwakening_InfoConfig blood_info = ConfigComponent.Instance.GetItem<BloodAwakening_InfoConfig>(TypeSystem);
            OneLoop = GlobalDataManager.GetDictionary(blood_info.AttributeNode1);
            TwoLoop = GlobalDataManager.GetDictionary(blood_info.AttributeNode2);
            ThreeLoop = GlobalDataManager.GetDictionary(blood_info.AttributeNode3);
            FourLoop = GlobalDataManager.GetDictionary(blood_info.AttributeNode4);
            FiveLoop = GlobalDataManager.GetDictionary(blood_info.AttributeNode5);
            // Log.DebugBrown("blood_info.AttributeNode1" + OneLoop.Count);
            // Log.DebugBrown("blood_info.PurityTime" + TwoLoop.Count);
            img_obj.gameObject.gameObject.SetActive(true);
            int a1 = 0, a2 = 0, a3 = 0, a4 = 0, a5 = 0;
            if (yuandian.transform.childCount>0)
            {
                for (int i = 0; i < yuandian.transform.childCount; i++)
                {
                    GameObject.Destroy(yuandian.transform.GetChild(i).gameObject);
                }
            }
            foreach (var item in OneLoop)
            {
                a1++;
                float angle = a1 * Mathf.PI * 2 / OneLoop.Count;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * 150f;
                GameObject obj = GameObject.Instantiate(img_obj.gameObject);
                obj.transform.SetParent(yuandian.transform, false);
                obj.transform.position = yuandian.transform.position;
                obj.GetComponent<RectTransform>().anchoredPosition = position;
                obj.transform.name = 1.ToString();
                //RectTransform rectTransform = obj.GetComponent<RectTransform>();
                //if (rectTransform != null)
                //{
                //    rectTransform.anchoredPosition = position;
                //}
                obj.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                if (NodeDic.ContainsKey(item.Key))
                {
                    obj.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    obj.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
                    obj.transform.GetChild(0).GetComponent<Button>().interactable = false;
                }
                obj.transform.GetChild(0).GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    Log.DebugBrown("1点击的对象是" + item.Key);
                    tips.gameObject.SetActive(true);
                    tips.transform.GetChild(0).gameObject.SetActive(true);
                    tips.transform.GetChild(1).gameObject.SetActive(false);
                    tips.transform.GetChild(2).gameObject.SetActive(false);
                    tips.transform.GetChild(3).gameObject.SetActive(false);
                    tips.transform.GetChild(4).gameObject.SetActive(false);
                    ShowTips(item.Key,obj.transform.name);
                });
            }

            foreach (var item in TwoLoop)
            {
                a2++;
                float angle = a2 * Mathf.PI * 2 / TwoLoop.Count;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * 220f;
                GameObject obj = GameObject.Instantiate(img_obj.gameObject);
                obj.transform.SetParent(yuandian.transform, false);
                obj.transform.position = yuandian.transform.position;
                obj.GetComponent<RectTransform>().anchoredPosition = position;
              //  obj.transform.position = position;
                obj.transform.name = 2.ToString();
                obj.transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
                if (NodeDic.ContainsKey(item.Key))
                {
                    obj.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    obj.transform.GetChild(0).GetChild(3).gameObject.SetActive(true);
                    obj.transform.GetChild(0).GetComponent<Button>().interactable = false;
                }
                //RectTransform rectTransform = obj.GetComponent<RectTransform>();
                //if (rectTransform != null)
                //{
                //    rectTransform.anchoredPosition = position;
                //}
                obj.transform.GetChild(0).GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    Log.DebugBrown("2点击的对象是" + item.Key);
                    tips.gameObject.SetActive(true);
                    tips.transform.GetChild(0).gameObject.SetActive(true);
                    tips.transform.GetChild(1).gameObject.SetActive(false);
                    tips.transform.GetChild(2).gameObject.SetActive(false);
                    tips.transform.GetChild(3).gameObject.SetActive(false);
                    tips.transform.GetChild(4).gameObject.SetActive(false);
                    ShowTips(item.Key, obj.transform.name);
                });
            }


            foreach (var item in ThreeLoop)
            {
                a3++;
                float angle = a3 * Mathf.PI * 2 / ThreeLoop.Count;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * 300f;
                GameObject obj = GameObject.Instantiate(img_obj.gameObject);
                obj.transform.SetParent(yuandian.transform, false);
                obj.transform.position = yuandian.transform.position;
                obj.GetComponent<RectTransform>().anchoredPosition = position;
                obj.transform.name = 3.ToString();
                obj.transform.GetChild(0).GetChild(4).gameObject.SetActive(true);
                if (NodeDic.ContainsKey(item.Key))
                {
                    obj.transform.GetChild(0).GetChild(4).gameObject.SetActive(false);
                    obj.transform.GetChild(0).GetChild(5).gameObject.SetActive(true);
                    obj.transform.GetChild(0).GetComponent<Button>().interactable = false;
                }
                obj.transform.GetChild(0).GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    Log.DebugBrown("3点击的对象是" + item.Key);
                    tips.gameObject.SetActive(true);
                    tips.transform.GetChild(0).gameObject.SetActive(true);
                    tips.transform.GetChild(1).gameObject.SetActive(false);
                    tips.transform.GetChild(2).gameObject.SetActive(false);
                    tips.transform.GetChild(3).gameObject.SetActive(false);
                    tips.transform.GetChild(4).gameObject.SetActive(false);
                    ShowTips(item.Key, obj.transform.name);
                });
            }


            foreach (var item in FourLoop)
            {
                a4++;
                float angle = a4 * Mathf.PI * 2 / FourLoop.Count;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * 380f;
                GameObject obj = GameObject.Instantiate(img_obj.gameObject);
                obj.transform.SetParent(yuandian.transform, false);
                obj.transform.position = yuandian.transform.position;
                obj.GetComponent<RectTransform>().anchoredPosition = position;
                obj.transform.name = 4.ToString();
                obj.transform.GetChild(0).GetChild(6).gameObject.SetActive(true);
                if (NodeDic.ContainsKey(item.Key))
                {
                    obj.transform.GetChild(0).GetChild(6).gameObject.SetActive(false);
                    obj.transform.GetChild(0).GetChild(7).gameObject.SetActive(true);
                    obj.transform.GetChild(0).GetComponent<Button>().interactable = false;
                }
                obj.transform.GetChild(0).GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    Log.DebugBrown("4点击的对象是" + item.Key);
                    tips.gameObject.SetActive(true);
                    tips.transform.GetChild(0).gameObject.SetActive(true);
                    tips.transform.GetChild(1).gameObject.SetActive(false);
                    tips.transform.GetChild(2).gameObject.SetActive(false);
                    tips.transform.GetChild(3).gameObject.SetActive(false);
                    tips.transform.GetChild(4).gameObject.SetActive(false);
                    ShowTips(item.Key, obj.transform.name);
                });
            }
            foreach (var item in FiveLoop)
            {
                a5++;
                float angle = a5 * Mathf.PI * 2 / FiveLoop.Count;
                Vector3 position = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle)) * 450f;
                GameObject obj = GameObject.Instantiate(img_obj.gameObject);
                obj.transform.SetParent(yuandian.transform, false);
                obj.transform.position = yuandian.transform.position;
                obj.GetComponent<RectTransform>().anchoredPosition = position;
                obj.transform.name = 5.ToString();
                obj.transform.GetChild(0).GetChild(8).gameObject.SetActive(true);
                if (NodeDic.ContainsKey(item.Key))
                {
                    obj.transform.GetChild(0).GetChild(8).gameObject.SetActive(false);
                    obj.transform.GetChild(0).GetChild(9).gameObject.SetActive(true);
                    obj.transform.GetChild(0).GetComponent<Button>().interactable = false;
                }
                obj.transform.GetChild(0).GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    Log.DebugBrown("5点击的对象是" + item.Key);
                    tips.gameObject.SetActive(true);
                    tips.transform.GetChild(0).gameObject.SetActive(true);
                    tips.transform.GetChild(1).gameObject.SetActive(false);
                    tips.transform.GetChild(2).gameObject.SetActive(false);
                    tips.transform.GetChild(3).gameObject.SetActive(false);
                    tips.transform.GetChild(4).gameObject.SetActive(false);
                    ShowTips(item.Key, obj.transform.name);
                });
            }
            img_obj.gameObject.gameObject.SetActive(false);
        }

        /// <summary>
        /// tips显示净化
        /// </summary>
        private void ShowTips_Type(int type)
        {
            if (type==0)
            {
                BloodAwakening_InfoConfig AttributeNode_info = ConfigComponent.Instance.GetItem<BloodAwakening_InfoConfig>(TypeSystem);
                if (AttributeNode_info == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "AttributeNode_InfoConfig未找到ID为:" + TypeSystem);
                    return;
                }
                ConsumeDic = GlobalDataManager.GetDictionary(AttributeNode_info.PurityNeed);
                string consume_text = null;
                foreach (var item in ConsumeDic)
                {
                    ((long)item.Key).GetItemInfo_Out(out Item_infoConfig item_Info);
                    consume_text += $"<color=green>{item_Info.Name + "X" + item.Value}</color>\n";
                }
                tips.transform.Find("Type5/des").GetComponent<Text>().text = "是否消耗：" + '\n' + consume_text + "减少15天日期。";
            }
            else if(type==1)
            {
                BloodAwakening_InfoConfig AttributeNode_info = ConfigComponent.Instance.GetItem<BloodAwakening_InfoConfig>(TypeSystem);
                if (AttributeNode_info == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "AttributeNode_InfoConfig未找到ID为:" + TypeSystem);
                    return;
                }
                ConsumeDic = GlobalDataManager.GetDictionary(AttributeNode_info.ActivateNeed);
                string consume_text = null;
                foreach (var item in ConsumeDic)
                {
                    ((long)item.Key).GetItemInfo_Out(out Item_infoConfig item_Info);
                    consume_text += $"<color=green>{item_Info.Name + "X" + item.Value}</color>\n";
                }
                tips.transform.Find("Type4/des").GetComponent<Text>().text = "是否消耗：" + '\n' + consume_text + "解锁下一环内容。";
            }
           
        }


        /// <summary>
        /// 显示tips内容
        /// </summary>
        private void ShowTips(int id, string name)
        {
            Node = id;
            Loop= int.Parse(name);
            ConsumeDic.Clear();
            AttributeNode_InfoConfig AttributeNode_info = ConfigComponent.Instance.GetItem<AttributeNode_InfoConfig>(id);
            if (AttributeNode_info==null)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "AttributeNode_InfoConfig未找到ID为:"+id);
                return;
            }
            ConsumeDic = GlobalDataManager.GetDictionary(AttributeNode_info.ActivateNeed);
            string consume_text = null;
            string consume_des = null;
            foreach (var item in ConsumeDic)
            {
                ((long)item.Key).GetItemInfo_Out(out Item_infoConfig item_Info);
                consume_text += $"<color=green>{item_Info.Name+"X"+item.Value}</color>\n";
            }
            for (int i = 0; i < AttributeNode_info.AttributeNode.Count; i++)
            {
                ItemAttrEntry_BaseConfig itemAttrEntry_Base = ConfigComponent.Instance.GetItem<ItemAttrEntry_BaseConfig>(AttributeNode_info.AttributeNode[i]);
                consume_des+= string.Format(itemAttrEntry_Base.Name,itemAttrEntry_Base.Value0);

            }
            tips.transform.Find("Type1/des").GetComponent<Text>().text = "是否消耗：" + '\n' + consume_text + "激活:"+ consume_des;
        }

        /// <summary>
        /// 打开系统获取数据
        /// </summary>
        /// <returns></returns>
        public async ETTask OpenAwakening()
        {
            G2C_BloodVesselInterface g2C_OpenBloodVessel = (G2C_BloodVesselInterface)await SessionComponent.Instance.Session.Call(new C2G_BloodVesselInterface()
            {

            }); ;
            Log.DebugBrown("g2C_OpenBloodVessel" + JsonHelper.ToJson(g2C_OpenBloodVessel));
            BloodIdlist.Clear();
            if (g2C_OpenBloodVessel.Error==0)
            {
                BloodIdlist.AddRange(g2C_OpenBloodVessel.BloodIdList);
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenBloodVessel.Error.GetTipInfo());
                return;
            }
            Log.DebugBrown("当前存入的数量是" + BloodIdlist.Count);
            if (BloodIdlist.Count == 0)
            {

                tips.gameObject.SetActive(true);
                tips.transform.GetChild(0).gameObject.SetActive(false);
                tips.transform.GetChild(1).gameObject.SetActive(true);
                tips.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                InitAwakening(1).Coroutine();
            }
        }
        /// <summary>
        /// 获取血脉信息
        /// </summary>
        /// <returns></returns>
        public async ETTask InitAwakening(int id)
        {
            Log.DebugBrown("当前获取的血脉ID" + id);
            NodeDic.Clear();
            G2C_GetCurrentBloodInfoVessels req = (G2C_GetCurrentBloodInfoVessels)await SessionComponent.Instance.Session.Call(new C2G_GetCurrentBloodInfoVessels()
            {
                BloodId=id
            }); ;
            Log.DebugBrown("G2C_GetCurrentBloodInfoVessels" + JsonHelper.ToJson(req));
            if (req.Error == 0)
            {
                // BloodIdlist.AddRange(req.BloodIdList);
                bloodinfo = req.BloodInfo;
                for (int i = 0; i < req.BloodInfo.NodeList.count; i++)
                {
                    if (req.BloodInfo.NodeList[i].ActivatedNode.count!=0)
                    {
                        for (int j = 0; j < req.BloodInfo.NodeList[i].ActivatedNode.count; j++)
                        {
                            if (!NodeDic.ContainsKey(req.BloodInfo.NodeList[i].ActivatedNode[j]))
                            {
                                NodeDic.Add(req.BloodInfo.NodeList[i].ActivatedNode[j], 0);
                            }
                        }
                    }
                }
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, req.Error.GetTipInfo());
                return;
            }
            foreach (var item in req.BloodInfo.NodeList)
            {
                MaxLoop  = item.RingId;
            }
            foreach (var item in NodeDic)
            {
                Log.DebugGreen("打印已经解锁的id" + item.Key);
            }
            DistributeObjects(bloodinfo);
        }


        /// <summary>
        /// 激活解锁血脉
        /// </summary>
        /// <returns></returns>
        public async ETTask ActivateAwakening(int ID)
        {
            G2C_ActivateBloodVessels g2C_OpenBloodVessel = (G2C_ActivateBloodVessels)await SessionComponent.Instance.Session.Call(new C2G_ActivateBloodVessels()
            {
                BloodId = ID
            }) ; ;
            Log.DebugBrown("G2C_ActivateBloodVessels" + JsonHelper.ToJson(g2C_OpenBloodVessel));
            bloodinfo = g2C_OpenBloodVessel.BloodInfo;
            for (int i = 0; i < g2C_OpenBloodVessel.BloodInfo.NodeList.count; i++)
            {
                if (g2C_OpenBloodVessel.BloodInfo.NodeList[i].ActivatedNode.count != 0)
                {
                    for (int j = 0; j < g2C_OpenBloodVessel.BloodInfo.NodeList[i].ActivatedNode.count; j++)
                    {
                        if (!NodeDic.ContainsKey(g2C_OpenBloodVessel.BloodInfo.NodeList[i].ActivatedNode[j]))
                        {
                            NodeDic.Add(g2C_OpenBloodVessel.BloodInfo.NodeList[i].ActivatedNode[j], 0);
                        }
                    }
                }
            }
            if (g2C_OpenBloodVessel.Error == 0)
            {
               
                // BloodIdlist.AddRange(g2C_OpenBloodVessel.BloodIdList);
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenBloodVessel.Error.GetTipInfo());
                return;
            }
            DistributeObjects(g2C_OpenBloodVessel.BloodInfo);
        }

        /// <summary>
        /// 使用血脉
        /// </summary>
        /// <returns></returns>
        public async ETTask UseAwakening(int ID)
        {
            G2C_UseBloodLineVessels g2C_UseBlood = (G2C_UseBloodLineVessels)await SessionComponent.Instance.Session.Call(new C2G_UseBloodLineVessels()
            {
                BloodId = ID
            }); ;
            Log.DebugBrown("G2C_UseBloodLineVessels" + JsonHelper.ToJson(g2C_UseBlood));
            if (g2C_UseBlood.Error == 3810)
            {
                // BloodIdlist.AddRange(g2C_OpenBloodVessel.BloodIdList);
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_UseBlood.Error.GetTipInfo());
                return;
            }
        }


        /// <summary>
        /// 激活血脉节点
        /// </summary>
        /// <returns></returns>
        public async ETTask ActivateInfoAwakening(int ID, int loop, int node)
        {

            Log.DebugBrown("这是激活协议" + ID + ":::" + loop + ":::" + node);
            G2C_NodeActivationVessels g2C_ActivateInfo = (G2C_NodeActivationVessels)await SessionComponent.Instance.Session.Call(new C2G_NodeActivationVessels()
            {
                BloodId = ID,
                RingId=loop,
                Node=node
                
            }); ;
            Log.DebugBrown("激活节点" + JsonHelper.ToJson(g2C_ActivateInfo));
            if (g2C_ActivateInfo.Error == 3810)
            {
                bloodinfo = g2C_ActivateInfo.BloodInfo;
                for (int i = 0; i < g2C_ActivateInfo.BloodInfo.NodeList.count; i++)
                {
                    if (g2C_ActivateInfo.BloodInfo.NodeList[i].ActivatedNode.count != 0)
                    {
                        for (int j = 0; j < g2C_ActivateInfo.BloodInfo.NodeList[i].ActivatedNode.count; j++)
                        {
                            if (!NodeDic.ContainsKey(g2C_ActivateInfo.BloodInfo.NodeList[i].ActivatedNode[j]))
                            {
                                NodeDic.Add(g2C_ActivateInfo.BloodInfo.NodeList[i].ActivatedNode[j], 0);
                            }
                        }
                    }
                }
                foreach (var item in NodeDic)
                {
                    Log.DebugBrown("更新数据" + item.Key);
                }
                DistributeObjects(g2C_ActivateInfo.BloodInfo);
               
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ActivateInfo.Error.GetTipInfo());
                return;
            }
        }



        /// <summary>
        /// 进化解锁光环
        /// </summary>
        /// <returns></returns>
        public async ETTask EvolutionAwakening(int ID, int loop)
        {

            Log.DebugBrown("这是进化协议" + ID + ":::" + loop);
            if (timestamp!=0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"当前血脉正在进化，无法进化下一环");
                return;
            }
            G2C_PurifyBloodVessels g2C_ActivateInfo = (G2C_PurifyBloodVessels)await SessionComponent.Instance.Session.Call(new C2G_PurifyBloodVessels()
            {
                BloodId = ID,
                RingId = loop,

            }); ;
            Log.DebugBrown("G2C_PurifyBloodVessels" + JsonHelper.ToJson(g2C_ActivateInfo));
            if (g2C_ActivateInfo.Error ==3806)
            {
                bloodinfo = g2C_ActivateInfo.BloodInfo;
                progress_bg.gameObject.SetActive(true);
                timestamp = g2C_ActivateInfo.BloodInfo.NextStage;
                DistributeObjects(g2C_ActivateInfo.BloodInfo);
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ActivateInfo.Error.GetTipInfo());
                return;
            }
        }

        /// <summary>
        /// 加速解锁光环
        /// </summary>
        /// <returns></returns>
        public async ETTask SpeedUpAwakening(int ID)
        {

            Log.DebugBrown("这是加速协议" + ID);
            G2C_CleanUpSpeedVessels g2C_ActivateInfo = (G2C_CleanUpSpeedVessels)await SessionComponent.Instance.Session.Call(new C2G_CleanUpSpeedVessels()
            {
                BloodId = ID,
            }); ;
            Log.DebugBrown("G2C_PurifyBloodVessels" + JsonHelper.ToJson(g2C_ActivateInfo));
            if (g2C_ActivateInfo.Error == 3809|| g2C_ActivateInfo.Error == 3808)
            {
                bloodinfo = g2C_ActivateInfo.BloodInfo;
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ActivateInfo.Error.GetTipInfo());
                foreach (var item in g2C_ActivateInfo.BloodInfo.NodeList)
                {
                    Debug.Log("打印环id" + item.RingId);
                    MaxLoop= item.RingId;
                }
                DistributeObjects(g2C_ActivateInfo.BloodInfo);
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ActivateInfo.Error.GetTipInfo());
                return;
            }
         
        }



        /// <summary>
        /// 初始化抽奖的物品
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal void InitUi()
        {
            OpenAwakening().Coroutine();
            yuandian = reference.GetImage("yuandian");
            img_obj = reference.GetImage("img_obj");
            tips = reference.GetImage("tips");
            progress_bg = reference.GetImage("progress_bg");
            Des = reference.GetText("Des");
            Btn1 = reference.GetButton("Btn1");
            Btn2 = reference.GetButton("Btn2");
            Btn3 = reference.GetButton("Btn3");
            Use_Btn = reference.GetButton("Use_Btn");
            Btn_Close = reference.GetButton("Btn_Close");
            Btn_evolution = reference.GetButton("Btn_evolution");
            Btn_Close.onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.Remove(UIType.UIAwakening);
            });
            //打开进化界面
            Btn_evolution.onClick.AddSingleListener(() =>
            {
                tips.gameObject.SetActive(true);
                tips.transform.GetChild(0).gameObject.SetActive(false);
                tips.transform.GetChild(1).gameObject.SetActive(false);
                tips.transform.GetChild(2).gameObject.SetActive(false);
                tips.transform.GetChild(4).gameObject.SetActive(false);
                tips.transform.GetChild(3).gameObject.SetActive(true);
                ShowTips_Type(1);
            });
            tips.transform.Find("Type1/tips_close").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                tips.gameObject.SetActive(false);
            });
            tips.transform.Find("Type2/tips_close").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                tips.gameObject.SetActive(false);
            });
            tips.transform.Find("Type3/tips_close").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                tips.gameObject.SetActive(false);
            });
            tips.transform.Find("Type4/tips_close").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                tips.gameObject.SetActive(false);
            });
            tips.transform.Find("Type5/tips_close").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                tips.gameObject.SetActive(false);
            });

            //加速
            progress_bg.transform.Find("Btn_speed").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                tips.gameObject.SetActive(true);
                tips.transform.GetChild(0).gameObject.SetActive(false);
                tips.transform.GetChild(1).gameObject.SetActive(false);
                tips.transform.GetChild(2).gameObject.SetActive(false);
                tips.transform.GetChild(4).gameObject.SetActive(true);
                tips.transform.GetChild(3).gameObject.SetActive(false);
                ShowTips_Type(0);
            });
            //进化光环
            tips.transform.Find("Type4/tips_activate").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                EvolutionAwakening(TypeSystem,MaxLoop+1).Coroutine();
                tips.gameObject.SetActive(false);
            });
            //加速光环
            tips.transform.Find("Type5/tips_activate").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                SpeedUpAwakening(TypeSystem).Coroutine();
                tips.gameObject.SetActive(false);
            });
            //激活节点请求
            tips.transform.Find("Type1/tips_activate").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                ActivateInfoAwakening(TypeSystem,Loop,Node).Coroutine();
                tips.gameObject.SetActive(false);
            });
            //激活血脉请求
            tips.transform.Find("Type2/tips_activate").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                ActivateAwakening(TypeSystem).Coroutine();
                tips.gameObject.SetActive(false);
            });
            //使用血脉请求
            tips.transform.Find("Type3/tips_activate").GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                UseAwakening(TypeSystem).Coroutine();
                tips.gameObject.SetActive(false);
            });
            //使用血脉
            Use_Btn.onClick.AddSingleListener(() =>
            {
                tips.gameObject.SetActive(true);
                tips.transform.GetChild(0).gameObject.SetActive(false);
                tips.transform.GetChild(1).gameObject.SetActive(false);
                tips.transform.GetChild(2).gameObject.SetActive(true);
                tips.transform.GetChild(3).gameObject.SetActive(false);
                if (TypeSystem==1)
                {
                    tips.transform.GetChild(2).Find("title").GetComponent<Text>().text = "天武血脉";
                }
                else if (TypeSystem==2)
                {
                    tips.transform.GetChild(2).Find("title").GetComponent<Text>().text = "火凤血脉";
                }
                else
                {
                    tips.transform.GetChild(2).Find("title").GetComponent<Text>().text = "神龙血脉";
                }
               
            });
            Btn1.onClick.AddSingleListener(() =>
            {
                TypeSystem = 1;
                bool satisfy = false;
                if (BloodIdlist.Count!=0)
                {
                    
                    for (int i = 0; i < BloodIdlist.Count; i++)
                    {
                        if (BloodIdlist[i]==TypeSystem)
                        {
                            satisfy = true;
                            break;
                        }
                    }
                }
                if (satisfy==false)//不满足走激活血脉
                {
                    tips.gameObject.SetActive(true);
                    tips.transform.GetChild(0).gameObject.SetActive(false);
                    tips.transform.GetChild(1).gameObject.SetActive(true);
                    tips.transform.GetChild(2).gameObject.SetActive(false);
                    tips.transform.GetChild(3).gameObject.SetActive(false);
                }
                else//满足走查看
                {
                    InitAwakening(TypeSystem).Coroutine();
                }
                Btn1.transform.GetChild(0).gameObject.SetActive(true);
                Btn2.transform.GetChild(0).gameObject.SetActive(false);
                Btn3.transform.GetChild(0).gameObject.SetActive(false);

            });
            Btn2.onClick.AddSingleListener(() =>
            {
                TypeSystem = 2;
                bool satisfy = false;
                if (BloodIdlist.Count != 0)
                {

                    for (int i = 0; i < BloodIdlist.Count; i++)
                    {
                        if (BloodIdlist[i] == TypeSystem)
                        {
                            satisfy = true;
                            break;
                        }
                    }
                }
                if (satisfy == false)//不满足走激活血脉
                {
                    tips.gameObject.SetActive(true);
                    tips.transform.GetChild(0).gameObject.SetActive(false);
                    tips.transform.GetChild(1).gameObject.SetActive(true);
                    tips.transform.GetChild(2).gameObject.SetActive(false);
                    tips.transform.GetChild(3).gameObject.SetActive(false);
                }
                else//满足走查看
                {
                    InitAwakening(TypeSystem).Coroutine();
                }
                Btn1.transform.GetChild(0).gameObject.SetActive(false);
                Btn2.transform.GetChild(0).gameObject.SetActive(true);
                Btn3.transform.GetChild(0).gameObject.SetActive(false);
            });
            Btn3.onClick.AddSingleListener(() =>
            {
                TypeSystem = 3;
                bool satisfy = false;
                if (BloodIdlist.Count != 0)
                {

                    for (int i = 0; i < BloodIdlist.Count; i++)
                    {
                        if (BloodIdlist[i] == TypeSystem)
                        {
                            satisfy = true;
                            break;
                        }
                    }
                }
                if (satisfy == false)//不满足走激活血脉
                {
                    tips.gameObject.SetActive(true);
                    tips.transform.GetChild(0).gameObject.SetActive(false);
                    tips.transform.GetChild(1).gameObject.SetActive(true);
                    tips.transform.GetChild(2).gameObject.SetActive(false);
                    tips.transform.GetChild(3).gameObject.SetActive(false);
                }
                else//满足走查看
                {
                    InitAwakening(TypeSystem).Coroutine();
                }
                Btn1.transform.GetChild(0).gameObject.SetActive(false);
                Btn2.transform.GetChild(0).gameObject.SetActive(false);
                Btn3.transform.GetChild(0).gameObject.SetActive(true);
            });
            //说明是新玩家，需要主动弹tips
         
        }







        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }
            SpriteUtility.Instance.ClearAtals(AtalsType.UI_Welfare_Icons); ;
            base.Dispose();
        }

    }
}
