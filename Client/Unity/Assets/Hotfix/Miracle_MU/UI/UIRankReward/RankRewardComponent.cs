using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;
using static GluonGui.WorkspaceWindow.Views.Checkin.Operations.CheckinViewDeleteOperation;
using static UnityEditor.Progress;

namespace ETHotfix
{
    public class ReWardInfo 
    {
        public string lev;//等级
        public int id;
        public List<int> itemId;//奖励物品ID
        public int miracleCoin;//奖励金币数量
        public int glodCount;//奖励金币数量
        public bool State;//领取状态 true 已领取 false 未领取

    }

    public class ReWardInfos 
    {
        // 公开一个key值为整型，value值是链表的字典
        public Dictionary<int, List<ReWardInfo>> items;//key 等级 value:奖励集合（免费、付费）
    }

    [ObjectSystem]
    public class RankRewardComponentAwake : AwakeSystem<RankRewardComponent>
    {
        public override void Awake(RankRewardComponent self)
        {
            self.referenceCollector = self.GetParent<UI>().GameObject.GetReferenceCollector(); // 获取ui脚本
            self.roleLeve = UnitEntityComponent.Instance.LocalRole.Level; // 获取角色等级
            self.referenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(()=>UIComponent.Instance.Remove(UIType.UI_RankReward));// 关闭等级奖励面板
            self.referenceCollector.GetButton("GetAllBtn").onClick.AddSingleListener(() => self.GetAllReward().Coroutine()); //监听一键领取按钮
            self.gameObject = self.referenceCollector.GetGameObject("Content"); //获取Content
            self.ChooseItem = self.referenceCollector.GetImage("ChooseItem").gameObject; //获取Content
            self.ChooseItem.SetActive(false);
            self.ChooseItem.GetReferenceCollector().GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                self.DisItems();
                self.ChooseItem.gameObject.SetActive(false);
            });
            self.scrollRect = self.referenceCollector.GetImage("Scroll View").GetComponent<ScrollRect>(); // 获取Scroll View
            self.scrollViewInfo = ComponentFactory.Create<UICircularScrollView<ReWardInfos>>(); // 生成奖励信息的循环滚动视图
            self.scrollViewInfo.ItemInfoCallBack = self.InitInfoCallBack; // 循环信息内容
            self.scrollViewInfo.InitInfo(E_Direction.Horizontal, 1,12, 0); // 水平滑动，1排，12个子对象，间隙为0
            self.scrollViewInfo.IninContent(self.gameObject, self.scrollRect);// 初始化滑动信息
            self.Init(); // 调用
           
        }

    }
    
    [ObjectSystem]
    public class RankRewardComponentStart : StartSystem<RankRewardComponent>
    {
        public override void Start(RankRewardComponent self)
        {
            //self.InitReWardInfo();// 调用
        }
    }
    /// <summary>
    /// 等级奖励
    /// </summary>
    public class RankRewardComponent : Component
    {
        public ReferenceCollector referenceCollector; // 声明

        public List<ReWardInfos> ReWardList=new List<ReWardInfos>();//声明一个ReWardInfos型的链表

        public ScrollRect scrollRect;// 声明
        public GameObject gameObject;// 声明
        public GameObject ChooseItem;// 声明
        public UICircularScrollView<ReWardInfos> scrollViewInfo; //声明奖励活动信息
        public int roleLeve; //角色等级
        public static readonly int UILayer = LayerMask.NameToLayer(LayerNames.UI);
        public const int TitleId = 60005;//天使之翼 称号ID
        bool IsHaveTianShiZhiYi = false; 
        public long receiveStatus;//领取进度
        private int ActiveID = 10;//活动ID
        private int FreeId = 20;//免费物品id限制

        public List<ReWardInfo> freeListItem = new List<ReWardInfo>();
        public List<ReWardInfo> nofreeListItem = new List<ReWardInfo>();

        private List<int> AllRewardId = new List<int>();//所有奖励物品ID
        public void InitReWardInfo() // 初始化奖励活动信息
        {
            freeListItem.Clear();
            nofreeListItem.Clear();
            ReWardList.Clear();
            AllRewardId.Clear();
            // 获取配置信息
            IConfig[] configs= ConfigComponent.Instance.GetAll<LevelUpRewards_RewardConfig>();
            int count = 0; // 状态初值
            foreach (LevelUpRewards_RewardConfig config in configs)//遍历配置表信息
            {
                count = (int)config.Id - 1;
                ReWardInfo rewardInfo = new ReWardInfo()// 重新赋值
                {
                    id = (int)config.Id, // id
                    lev = config.Limit.ToString(), // 等级
                    itemId = config.ItemID, // 奖励物品ID
                    glodCount = config.GoldCoin, // 奖励金币数量
                    miracleCoin = config.MiracleCoin, // 奖励奇迹币数量
                    State =(receiveStatus & 1L << count) == 1L << count,//true 已领取 false 未领取
                };
              //  Log.DebugGreen($"{config.Id}receiveStatus:{receiveStatus}->{(receiveStatus & 1L << count) == 1L << count}");// 领取进度 已领取或未领取
                if (ReWardList.Exists(x => x.items.ContainsKey(config.Limit)))//奖励列表存在配置表等级
                {
                    ReWardInfos dic = ReWardList.Find(x => x.items.ContainsKey(config.Limit));//找到配置表等级
                    dic.items[config.Limit].Add(rewardInfo);//在rewardInfo里面添加等级信息
                    nofreeListItem.Add(rewardInfo);
                }
                else
                {
                    ReWardInfos reWardInfos = new ReWardInfos
                    {
                        items = new Dictionary<int, List<ReWardInfo>>() // 得到一个key值整型，value值链表的字典
                    };
                    reWardInfos.items[config.Limit]=new List<ReWardInfo>() { rewardInfo }; // 得到新配置信息
                    ReWardList.Add(reWardInfos); //添加
                    freeListItem.Add(rewardInfo);
                }
                if(!((receiveStatus & 1L << count) == 1L << count) && config.Limit <= roleLeve)
                {
                    if(IsHaveTianShiZhiYi)
                        AllRewardId.Add((int)config.Id);
                    else
                    {
                        if(config.Id < FreeId)
                        {
                            AllRewardId.Add((int)config.Id);
                        }
                    }
                }
                //count++;
            }
            scrollViewInfo.Items = ReWardList; // 循环视图信息内容
        }


        ////获取相应活动的数据
        //public async ETTask OpenMiracleActivitiesRequest(int ActiveId)
        //{
        //    Log.DebugBrown("经过" + ActiveId);
        //    G2C_OpenMiracleActivitiesResponse g2C_OpenMiracleActivities = (G2C_OpenMiracleActivitiesResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMiracleActivitiesRequest()
        //    {
        //        MiracleActivitiesID = ActiveId
        //    });
        //    Log.DebugBrown("获取抽奖的信息" + g2C_OpenMiracleActivities.Error + "状态" + g2C_OpenMiracleActivities.Info.Value32A + ":" + g2C_OpenMiracleActivities.Info.Value64A);
        //    if (g2C_OpenMiracleActivities.Error != 0)
        //    {
        //        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
        //    }
        //    else
        //    {
        //        //pass = g2C_OpenMiracleActivities.Info.Value32A;
        //        times = g2C_OpenMiracleActivities.Info.Value64A;
        //        receiveStatus = g2C_OpenMiracleActivities.Info.Status;
        //    }
        //}
        public void Init() 
        {


           // OpenMiracleActivitiesRequest(14).Coroutine();
            IsHaveTianShiZhiYi = TitleManager.allTitles.Exists(x => x.TitleId == TitleId);
           // Log.DebugGreen($"IsHaveTianShiZhiYi:{IsHaveTianShiZhiYi}"); // 有无天使之翼 
            GetStates(10).Coroutine(); //协程

            //获取领取进度
            async ETVoid GetStates(int ActiveId) // 参数
            {
                // 客户端和服务端的信息交互
                G2C_OpenMiracleActivitiesResponse g2C_OpenMiracleActivities = (G2C_OpenMiracleActivitiesResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMiracleActivitiesRequest()
                {
                    MiracleActivitiesID = ActiveId
                });
                Log.DebugBrown("等级奖励" + g2C_OpenMiracleActivities.Error);
                if (g2C_OpenMiracleActivities.Error != 0)// 服务端的消息指令
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo()); //信息提示
                }                
                else
                {
                    receiveStatus = g2C_OpenMiracleActivities.Info.Value64A; // 从服务端获取领取进度
                  //  Log.DebugBrown($"领取进度：{g2C_OpenMiracleActivities.Info.Value64A}");
                    InitReWardInfo(); //调用
                }
            }
        }

        public void InitInfoCallBack(GameObject go, ReWardInfos Info) 
        {
            ReferenceCollector reference = go.GetReferenceCollector();  // 滑动视图
            GameObject freemask = reference.GetImage("freemask").gameObject;
            GameObject mask = reference.GetImage("mask").gameObject;
            Text text = reference.GetText("Text").GetComponent<Text>(); // 获取文本
            Dictionary<int, List<ReWardInfo>> dic = Info.items;//奖励物品
            //根据是否拥有天使之翼 设置mask显示隐藏
            foreach (var infos in dic) // 遍历奖励物品数据
            {
               // reference.GetImage("mask").gameObject.SetActive(!IsHaveTianShiZhiYi || infos.Key > roleLeve);
                #region 是否可领取事件
                if (infos.Key <= roleLeve)//满足领取条件  <=人物等级
                {
                    List<int> rewardids = new List<int>();//领取物品ID  整型列表
                    bool isCanRecevie = false;//是否可领取  不可
                    bool isFreeCanRecevie = true;//免费物品是否领取
                    bool isNoFreeCanRecevie = true;//免费物品是否领取
                    foreach (var item in infos.Value)//遍历 免费、付费物品
                    {
                        if (IsHaveTianShiZhiYi)//拥有天使之翼
                        {                           
                            if (item.State == false)//未领取
                            {
                                isCanRecevie = !item.State; // 可领取
                                //Log.DebugGreen($"{item.id}->{item.State}");
                            }
                            if (item.id < FreeId && item.State == false)//免费物品 未领取
                            {
                                isFreeCanRecevie = false;
                                rewardids.Add(item.id);//添加到 可领取
                            }
                            if (item.id >= FreeId && item.State == false)//付费物品 未领取
                            {
                                isNoFreeCanRecevie = false;
                                rewardids.Add(item.id);
                            }
                        }
                        else
                        {
                            //免费物品 未领取
                            if (item.id < FreeId && item.State == false)
                            {
                                isFreeCanRecevie = false;
                                rewardids.Add(item.id);
                                isCanRecevie = !item.State; // 可领取
                                //AllRewardId.Add(item.id);
                            }
                        }
                    }
                    if (isCanRecevie)//可领取
                    {
                       // freemask.SetActive(isFreeCanRecevie); // 隐藏
                      //  mask.SetActive(isNoFreeCanRecevie); // 显示
                        text.text = "领取"; // 文本赋值
                        //注册领取事件
                        reference.GetButton("GetBtn").onClick.AddSingleListener(() => // 按钮监听
                        {
                            //领取奖励
                            Log.DebugGreen("领取奖励" + JsonHelper.ToJson(rewardids));
                            GetReward(rewardids).Coroutine(); // 协程
                            async ETVoid GetReward(List<int> rewardIdList)
                            {
                                // 信息交互  刷新等级活动接收反馈
                                G2C_RushGradeActivityReceiveResponse g2C_OpenMiracleActivities = (G2C_RushGradeActivityReceiveResponse)await SessionComponent.Instance.Session.Call(new C2G_RushGradeActivityReceiveRequest()
                                {
                                    MiracleActivitiesID = ActiveID,
                                    RewardID = (new Google.Protobuf.Collections.RepeatedField<int>() { rewardIdList }),
                                });  
                                if (g2C_OpenMiracleActivities.Error == 2400)//活动未开启  服务端指令
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo()); // 服务端消息指令
                                    return;
                                }
                                if (g2C_OpenMiracleActivities.Error != 0) //服务端指令
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo()); // 服务端消息指令
                                }
                                else
                                {
                                    UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功"); // 消息提示

                                    receiveStatus = g2C_OpenMiracleActivities.Info.Value64A;  // 从服务端获取领取进度
                                    foreach (var id in rewardIdList) //遍历
                                    {
                                        Log.DebugYellow(id.ToString());
                                        AllRewardId.Remove(id); // 移除
                                    }
                                 //   freemask.SetActive(true); // 显示
                                  //  mask.SetActive(true); // 显示

                                    text.text = "已领取"; // 文本赋值
                                    reference.GetButton("GetBtn").onClick.RemoveAllListeners(); // 解除按钮监听
                                    //return; // 返回
                                    InitReWardInfo();
                                }
                            }
                        });
                    }
                    else//不可领取
                    {
                        //已领取
                      //  freemask.SetActive(true); // 隐藏
                      //  mask.SetActive(true); // 显示
                        reference.GetText("Text").GetComponent<Text>().text = "已领取";// 文本赋值
                        reference.GetButton("GetBtn").onClick.RemoveAllListeners();  // 解除按钮监听
                    }
                }
                else  //不满足 领取条件
                {
                    //不满足 领取条件
                  //  reference.GetImage("freemask").gameObject.SetActive(true);  // 显示
                    reference.GetText("Text").GetComponent<Text>().text = infos.Key.ToString(); // 领取
                    reference.GetButton("GetBtn").onClick.RemoveAllListeners(); //按钮监听无效
                }
                #endregion
                int index = 0;
                //显示奖励物品
                foreach (var info in infos.Value) // 遍历字典信息值
                {
                    //if (int.Parse(info.lev) <= roleLeve) //转换  <=人物等级
                    //{
                    //    if (info.State == false && index == 0)//未领取
                    //    {
                    //        text.text = "领取"; // 赋值
                    //        freemask.SetActive(false);
                    //        Log.DebugGreen($"未领取{info.id}");
                    //    }
                    //    else
                    //    {
                    //        info.State = true;
                    //        freemask.SetActive(true); // 显示
                    //        mask.SetActive(true); // 显示
                    //        text.text = "已领取"; // 赋值
                    //        reference.GetButton("GetBtn").onClick.RemoveAllListeners();// 解除监听
                    //        index++;
                    //        //Log.DebugGreen(infos.Key.ToString() + $"状态 {info.State}" + reference.name + freemask.gameObject.activeSelf + int.Parse(info.lev).ToString());
                    //    }
                    //}
                    //else
                    //{
                    //    freemask.SetActive(true); // 显示
                    //    mask.SetActive(true); // 显示
                    //    reference.GetText("Text").GetComponent<Text>().text = infos.Key.ToString(); // 数值 //10，30，50~
                    //    reference.GetButton("GetBtn").onClick.RemoveAllListeners();// 无效
                    //    Log.DebugRed(infos.Key.ToString());
                    //}
                    for (int i = 0, length = freeListItem.Count; i < length; i++)
                    {
                        if (info.id == freeListItem[i].id)
                        {
                            reference.GetImage("freekuang").GetComponent<Button>().onClick.AddSingleListener(() =>
                            {
                                ShowItems(freeListItem[i]);
                            });
                            break;
                        }
                    }
                    for (int i = 0, length = nofreeListItem.Count; i < length; i++)
                    {
                        if (info.id == nofreeListItem[i].id)
                        {
                            reference.GetImage("kuang").GetComponent<Button>().onClick.AddSingleListener(() =>
                            {
                                ShowItems(nofreeListItem[i]);
                            });
                            break;
                        }
                    }
                }
            }
           
        }
        public List<GameObject> ListItem = new List<GameObject>();
        public void ShowItems(ReWardInfo info)
        {
            for (int i = 0,length = ChooseItem.GetReferenceCollector().GetGameObject("Items").transform.childCount; i < length; i++)
            {
                ChooseItem.GetReferenceCollector().GetGameObject("Items").transform.GetChild(i).gameObject.SetActive(false);
            }
            //金币
            if (info.glodCount != 0)
            {
                ChooseItem.GetReferenceCollector().GetGameObject("Items").transform.Find("jinbi").gameObject.SetActive(true);
                ChooseItem.GetReferenceCollector().GetGameObject("Items").transform.Find("jinbi/name").GetComponent<Text>().text = $"金币 X{info.glodCount}";
            }
            ////奇迹币
            //if (info.miracleCoin != 0)
            //{
            //    ChooseItem.GetReferenceCollector().GetGameObject("Items").transform.Find("qijibi").gameObject.SetActive(true);
            //    ChooseItem.GetReferenceCollector().GetGameObject("Items").transform.Find("qijibi/name").GetComponent<Text>().text = $"U币 X{info.miracleCoin}";
            //}

            Dictionary<int, int> ItemDic = new Dictionary<int, int>();
            List<int> itemList = new List<int>();
            //物品
            if (info.itemId.Count != 0)
            {
                for (int i = 0, length = info.itemId.Count; i < length; i++)
                {

                    if (itemList.Contains(info.itemId[i]) == false)
                    {
                        itemList.Add(info.itemId[i]);
                    }

                    if (ItemDic.ContainsKey(info.itemId[i]))
                    {
                        ItemDic[info.itemId[i]] += 1;
                    }
                    else
                    {
                        ItemDic[info.itemId[i]] = 1;
                    }
                }

                for (int i = 0, length = itemList.Count; i < length; i++)
                {

                    LevelUpActivity_RewardPropsConfig activity_RewardProps = ConfigComponent.Instance.GetItem<LevelUpActivity_RewardPropsConfig>(itemList[i]);
                    ((long)activity_RewardProps.ItemId).GetItemInfo_Out(out Item_infoConfig item_Info);
                    //显示物品模型
                    GameObject obj = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
                  //  Log.DebugBrown("等级奖励物品" + activity_RewardProps.ItemId + ":::id" + itemList[i]);
                    Transform target = ChooseItem.GetReferenceCollector().GetGameObject("Items").transform.Find($"item{i}").transform;
                    target.gameObject.SetActive(true);
                    if (obj != null)
                    {
                        obj.SetUI(activity_RewardProps.Level);

                        obj.transform.SetParent(target, false);

                        Vector3 vector = Vector3.zero;
                        RectTransform rect = target.GetComponent<RectTransform>();
                        MeshSize.Result result = MeshSize.GetMeshSize(obj.transform, UILayer);
                        float scale = result.GetScreenScaleFactor(new Vector2(rect.rect.width, rect.rect.height) * 60) * 3;

                        if (scale > 60) scale = 60;
                        vector.z = -10;
                        obj.transform.localPosition = vector;

                        obj.transform.localScale = Vector3.one * scale;
                        ListItem.Add(obj);
                    }
                  //  target.Find("name").GetComponent<Text>().text = $"{item_Info.Name} X{ItemDic[itemList[i]]}";
                   target.Find("name").GetComponent<Text>().text = $"{item_Info.Name} X{ItemDic[itemList[i]]* activity_RewardProps.Quantity}";
                }
            }
            // ints.Clear();
            ItemDic.Clear();
            ChooseItem.gameObject.SetActive(true);
        }
        public void DisItems()
        {
            foreach (var item in ListItem)
            {
                if (item != null)
                {
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(item, item.name.StringToAB());
                }
            }
            ListItem.Clear();
        }
        //一键领取
        public async ETVoid GetAllReward()
        {
            if(JsonHelper.ToJson(AllRewardId).Length < 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"没有可领取的奖励!");
            }
            //  Log.DebugGreen($"一键领取：{JsonHelper.ToJson(AllRewardId)}");
            /*if (AllRewardId.Count == 0)
            {
                return;
            }*/

           /* foreach (var item in AllRewardId)
            {
                Log.DebugGreen($"领取的所有物品->{item}");
            }*/
            G2C_RushGradeActivityReceiveResponse g2C_OpenMiracleActivities = (G2C_RushGradeActivityReceiveResponse)await SessionComponent.Instance.Session.Call(new C2G_RushGradeActivityReceiveRequest()
            {
                MiracleActivitiesID = ActiveID,
                RewardID = new Google.Protobuf.Collections.RepeatedField<int>() { AllRewardId },
            });
            if (g2C_OpenMiracleActivities.Error == 2400)//活动未开启
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
                return;
            }
            if (g2C_OpenMiracleActivities.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功");
                receiveStatus = g2C_OpenMiracleActivities.Info.Value64A; //0 1

                //for (int i = 0; i <gameObject.transform.childCount; i++)
                //{
                //    Transform item=gameObject.transform.GetChild(i);
                //    Log.DebugRed(item.ToString());
                //    item.GetReferenceCollector().GetText("Text").text = "已领取";
                //    item.GetReferenceCollector().GetImage("freemask").gameObject.SetActive(true);
                //    item.GetReferenceCollector().GetImage("mask").gameObject.SetActive(true);
                //    item.GetReferenceCollector().GetButton("GetBtn").onClick.RemoveAllListeners();
                    
                //}
                InitReWardInfo();
            }
            
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            scrollViewInfo.Dispose();
            ReWardList.Clear();
        }
    }
}