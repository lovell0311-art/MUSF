using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using UnityEngine.U2D;
using System;
using System.Linq;
using static NPOI.HSSF.Util.HSSFColor;
using static UnityEditor.Progress;
using static UnityEditor.ShaderData;


namespace ETHotfix
{

    //public class Prize
    //{
    //    public int boxId;//物品位置
    //    public string iconRes;//物品图标资源名
    //    public string name;//物品名字
    //}

    //public enum E_DrawType
    //{
    //    RULE,//抽奖规则
    //    DRAWRECORD,//抽奖记录
    //}
    ////抽奖记录
    //public class DrawLog
    //{
    //    public long logId;
    //    public long UserID;
    //    public long GameUserId;
    //    public string NickName;
    //    public string Des;
    //}

    [ObjectSystem]
    public class UIWelfareComponentAwake : AwakeSystem<UIWelfareComponent>
    {
        public override void Awake(UIWelfareComponent self)
        {
            self.reference = self.GetParent<UI>().GameObject.GetReferenceCollector();
            //--

            self.InitUi();
            self.reference.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                if (self.IsOnClickPlaying)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "正在抽奖，稍后关闭");
                    return;
                }
                UIComponent.Instance.Remove(UIType.UIWelfare);
            });
        }
    }

    [ObjectSystem]
    public class UIWelfareComponentUpdate : UpdateSystem<UIWelfareComponent>
    {
        public override void Update(UIWelfareComponent self)
        {
            if (self.isOnClickPlaying == true)//激活抽奖
            {
                self.BeginTime += Time.deltaTime;
                if (self.Onindex < 30)
                {
                    if (self.BeginTime >= 0.1f)
                    {
                        self.OnRefresh();
                        self.rewardTran.transform.GetChild(self.Index).GetChild(0).gameObject.SetActive(true);
                        if (self.Index > 6)
                        {
                            self.Index = 0;
                        }
                        else
                        {
                            self.Index += 1;
                        }
                        self.Onindex += 1;
                        self.BeginTime = 0;
                    }
                }
                else
                {
                    if (self.BeginTime >= 0.5f)
                    {
                        self.OnRefresh();
                        self.rewardTran.transform.GetChild(self.Index).GetChild(0).gameObject.SetActive(true);
                        if (self.Index == (self.ItemIndex))//等于目标索引直接重置
                        {
                            self.isOnClickPlaying = false;
                            self.Index = 0;
                            self.Onindex = 0;
                            Log.DebugBrown("抽奖停止了" + (self.ItemIndex));
                            if (self.ItemIndex != 0)
                            {
                                self.OnUi((self.ItemIndex));
                            }
                            else
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "谢谢惠顾！");
                            }
                            // self.DrawPrizes.gameObject.SetActive(true);

                        }

                        if (self.Index > 6)
                        {
                            self.Index = 0;
                        }
                        else
                        {
                            self.Index += 1;
                        }
                        self.BeginTime = 0;
                    }
                }
            }
        }

    }

    public partial class UIWelfareComponent : Component
    {


        public float BeginTime = 0;
        public int Index = 0, Onindex = 0, receiveStatus = 0;
        public int ItemIndex = 0;
        public IConfig[] config;
        public Image DrawPrizes, Good;//获奖面板/物品详情
        public int pass = 0, itemcount = 0;//领奖状态/抽奖卷数量
        public long times = 0;//下次领奖的时间戳
        /// <summary>
        /// ///////////
        /// </summary>
        public ReferenceCollector reference;
        public Dictionary<int, Prize> PrizeDic = new Dictionary<int, Prize>();//奖品字典 key:奖品所在位置 value:奖品信息
        public List<int> PrizeList = new List<int>();//中奖ID集合

        private Sprite defaultSp, TurnSp, SelectSp;
        private SpriteAtlas spriteAtlas;
        public Text mojingCount;//魔晶数量
        public Text DreawCount;//抽奖次数
        public Image EnergyFill;//抽奖进度

        public int OneDraw = 20;//单次抽奖花费魔晶数
        public int TenDraw = 190;//单次抽奖花费魔晶数

        public UICircularScrollView<DrawLog> DrawLogScrollView;
        public ScrollRect DrawLogScrollrect;
        public GameObject DrawLogContent;
        public List<DrawLog> DrawLogLis = new List<DrawLog>();

        public GameObject RecordsInfos, Rule;


        // 抽奖图片父物体
        public Transform rewardTran;
        // 抽奖图片
        public Image[] rewardTransArr;
        // 抽奖结束 -- 结束状态，光环不转
        public bool drawEnd;
        // 中奖
        private bool drawWinning;
        //展示状态时间 --> 控制光环转动初始速度
        public float setrewardTime = 1f;
        public float rewardTime;
        public float rewardTiming = 0;

        public bool IsJumpTg = false;//是否跳过动画

        // 当前光环所在奖励的索引
        public int haloIndex = 0;
        //抽奖结束的事件
        public Action PlayingAction;

        //连抽集合
        Queue<int> indexList = new Queue<int>();
        public bool IsMultiDraw = false;//是否连抽
        // 点了抽奖按钮正在抽奖
        public bool isOnClickPlaying;
        public bool IsOnClickPlaying
        {
            get => isOnClickPlaying;
            set
            {
                isOnClickPlaying = value;
            }
        }
        public bool DrawWinning
        {
            get => drawWinning;
            set => drawWinning = value;
        }
        public bool DrawEnd
        {
            get => drawEnd;
            set
            {
                drawEnd = value;

            }
        }

        public void Init_DrawScrollView()
        {
            DrawLogScrollrect = reference.GetImage("RecordsInfos").GetComponent<ScrollRect>();
            DrawLogContent = reference.GetGameObject("Content");
            DrawLogScrollView = ComponentFactory.Create<UICircularScrollView<DrawLog>>();
            DrawLogScrollView.ItemInfoCallBack = InitDrawCallBack;
            DrawLogScrollView.InitInfo(E_Direction.Vertical, 1, 0, 10);
            DrawLogScrollView.IninContent(DrawLogContent, DrawLogScrollrect);
        }

        private void InitDrawCallBack(GameObject go, DrawLog info)
        {
            go.GetComponent<Text>().text = "<color=yellow>" + info.NickName + "</color> " + info.Des;
        }
        /// <summary>
        /// 重新显示ui状态
        /// </summary>
        public void OnRefresh()
        {
            for (int i = 0; i < rewardTran.transform.childCount; i++)
            {
                rewardTran.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 展示奖品
        /// </summary>
        /// <param name="itemIndex"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void OnUi(int itemIndex)
        {
            DrawPrizes.gameObject.SetActive(true);
            DrawPrizes.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            DrawPrizes.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            if (itemIndex == 0)
            {
                DrawPrizes.gameObject.transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                DrawPrizes.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            }
            GoldLottery_ItemInfoConfig gold = ConfigComponent.Instance.GetItem<GoldLottery_ItemInfoConfig>((itemIndex));
            if (gold == null)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "配置id" + (itemIndex) + "为空");
                return;
            }
            Sprite sprite = spriteAtlas.GetSprite(gold.IconName);
            Good.transform.Find("icon").GetComponent<Image>().sprite = sprite;
            Good.transform.Find("name").GetComponent<Text>().text = gold.ItemName + "X" + gold.Quantity;
            //  "<color=#23C2EF>" + this.GetProperValue(E_ItemValue.ForgeValue) + "</color>"
        }


        //获取相应活动的数据
        public async ETTask OpenMiracleActivitiesRequest(int ActiveId)
        {
            Log.DebugBrown("经过" + ActiveId);
            G2C_OpenMiracleActivitiesResponse g2C_OpenMiracleActivities = (G2C_OpenMiracleActivitiesResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMiracleActivitiesRequest()
            {
                MiracleActivitiesID = ActiveId
            });
            Log.DebugBrown("获取抽奖的信息" + g2C_OpenMiracleActivities.Error + "状态" + g2C_OpenMiracleActivities.Info.Value32A + ":" + g2C_OpenMiracleActivities.Info.Value64A);
            if (g2C_OpenMiracleActivities.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenMiracleActivities.Error.GetTipInfo());
            }
            else
            {
                //pass = g2C_OpenMiracleActivities.Info.Value32A;
                times = g2C_OpenMiracleActivities.Info.Value64A;
                receiveStatus = g2C_OpenMiracleActivities.Info.Status;
            }
        }
        /// <summary>
        /// 初始化抽奖的物品
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        internal void InitUi()
        {
            OpenMiracleActivitiesRequest(14).Coroutine();
            spriteAtlas = SpriteUtility.Instance.GetSpriteAtlas(AtalsType.UI_Welfare_Icons);
            config = ConfigComponent.Instance.GetAll<GoldLottery_ItemInfoConfig>();
            int count = 0;
            rewardTran = reference.GetGameObject("Prizes").transform;
            DrawPrizes = reference.GetImage("DrawPrizes");
            Good = reference.GetImage("Good");
            DrawPrizes.transform.GetChild(2).GetComponent<Button>().onClick.AddSingleListener(() =>
            {
                DrawPrizes.gameObject.SetActive(false);
            });
            foreach (var item in config.Cast<GoldLottery_ItemInfoConfig>())
            {
                if (item.Id <= 100)
                {
                    Sprite sprite = spriteAtlas.GetSprite(item.IconName);
                    if (sprite == null)
                    {
                        sprite = spriteAtlas.GetSprite("U币");
                    }
                    rewardTran.transform.GetChild(count).Find("icon").GetComponent<Image>().sprite = sprite;
                    // rewardTran.transform.GetChild(count).Find("icon").GetComponent<Image>().SetNativeSize();
                    rewardTran.transform.GetChild(count).Find("name").GetComponent<Text>().text = item.ItemName;
                    Log.DebugBrown("item.IconName" + item.IconName + ":::" + item.ItemName);
                    count++;
                }

            }
            reference.GetButton("StartPrize").onClick.AddSingleListener(() =>
            {
                itemcount = 0;
                KnapsackDataItem dataItem = new KnapsackDataItem();
                dataItem.Id = 310139;//抽奖卷
                dataItem.Id.GetItemInfo_Out(out Item_infoConfig item_Info);
                KnapsackDataItem knapsackData = null;
                foreach (var item1 in KnapsackItemsManager.KnapsackItems.Values)
                {
                    if (item1.ConfigId == dataItem.Id)
                    {
                        knapsackData = item1;
                        itemcount += knapsackData.GetProperValue(E_ItemValue.Quantity);
                    }
                }
                Log.Debug("时间戳" + TimeHelper.CompareTimestamps(times) + ":" + pass + ":抽奖卷的数量" + itemcount);

                if (itemcount > 0)
                {
                    if (isOnClickPlaying == false)
                    {
                        OnRefresh();
                        Begin_Welfare();
                    }
                }
                else
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "抽奖劵不足");
                }
            });

        }

        private void Begin_Welfare()
        {
            InitDraw().Coroutine();
            Log.DebugGreen("活动抽奖");
            //获取抽奖信息
            async ETVoid InitDraw()
            {
                G2C_MayDayLuckyDraw g2C_MayDayLuckyDraw = (G2C_MayDayLuckyDraw)await SessionComponent.Instance.Session.Call(new C2G_MayDayLuckyDraw { });
                Log.DebugGreen("活动抽奖" + g2C_MayDayLuckyDraw.Error + "活动id" + g2C_MayDayLuckyDraw.Config);
                if (g2C_MayDayLuckyDraw.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_MayDayLuckyDraw.Error.GetTipInfo());
                }
                else
                {
                    pass = 1;
                    if (g2C_MayDayLuckyDraw.Config == 100)
                    {

                        ItemIndex = 0;
                    }
                    else
                    {
                        ItemIndex = g2C_MayDayLuckyDraw.Config;
                    }
                    isOnClickPlaying = true;

                }
            }
        }


        /// <summary>
        /// 抽奖
        /// </summary>
        /// <param name="count">抽奖次数</param>
        /// <returns></returns>
        public async ETVoid OnClickDrawFun(int count = 1)
        {
            if (IsOnClickPlaying)
            {
                return;
            }
            var mojingcount = UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing);
            if ((count == 1 && mojingcount < OneDraw) || count == 10 && mojingcount < TenDraw)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "魔晶不足");
                return;
            }

            G2C_StartLottery g2C_StartLottery = (G2C_StartLottery)await SessionComponent.Instance.Session.Call(new C2G_StartLottery { Count = count });
            if (g2C_StartLottery.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_StartLottery.Error.GetTipInfo());
            }
            else
            {

                EnergyFill.fillAmount = (float)g2C_StartLottery.TotalCount / g2C_StartLottery.TotalCountMax;
                DreawCount.text = $"已抽次数:{g2C_StartLottery.TotalCount}/{g2C_StartLottery.TotalCountMax}";
                StarMultiDraw(g2C_StartLottery.Ids.ToList());

            }
        }
        /// <summary>
        /// 开始抽奖
        /// </summary>
        /// <param name="idlist">中奖奖品ID集合</param>
        public void StarMultiDraw(List<long> idlist)
        {
            PrizeList.Clear();
            foreach (int id in idlist)
            {
                indexList.Enqueue(id - 1);
                PrizeList.Add(id - 1);

            }
            if (!IsOnClickPlaying)
            {
                haloIndex = -1;
                RePrepare();
                IsOnClickPlaying = true;
                DrawEnd = false;
                DrawWinning = false;
                if (IsJumpTg)
                {
                    rewardTime = 0.02f;
                    DrawWinning = true;
                }
                else
                {
                    StartDrawAni().Coroutine();
                }
            }
        }

        /// <summary>
        /// 开始抽奖动画
        /// 先快后慢 -- 根据需求调整时间
        /// </summary>
        /// <returns></returns>
        public async ETVoid StartDrawAni()
        {
            rewardTime = setrewardTime;

            // 加速
            for (int i = 0; i < setrewardTime / 0.05f - 1; i++)
            {
                await TimerComponent.Instance.WaitAsync(50);
                rewardTime -= 0.05f;
            }
            await TimerComponent.Instance.WaitAsync(2000);

            // 减速
            for (int i = 0; i < setrewardTime / 0.05f - 4; i++)
            {
                await TimerComponent.Instance.WaitAsync(50);
                rewardTime += 0.02f;
            }

            await TimerComponent.Instance.WaitAsync(500);
            DrawWinning = true;
        }

        /// <summary>
        /// 抽奖转盘转动
        /// </summary>
        /// <param name="index"></param>
        public void SetDrawPos(int index)
        {
            var sp = rewardTransArr[index - 1 < 0 ? rewardTran.childCount - 1 : index - 1];

            if (sp.sprite.name != ChangePrizeItemState(E_PrizeState.SELECT).name)
            {
                sp.sprite = ChangePrizeItemState(E_PrizeState.DEFAULT);
                //rewardTransArr[index].sprite = ChangePrizeItemState(E_PrizeState.TURN);
            }
            if (rewardTransArr[index].sprite.name != ChangePrizeItemState(E_PrizeState.SELECT).name)
            {
                rewardTransArr[index].sprite = ChangePrizeItemState(E_PrizeState.TURN);
            }

            //中将 &&此ID==中将ID
            if (DrawWinning && indexList.Count > 0 && index == indexList.Peek())
            {
                rewardTransArr[index].sprite = ChangePrizeItemState(E_PrizeState.SELECT);
                indexList.Dequeue();

                //展示中将结果 
                if (indexList.Count == 0)
                {
                    PlayingAction?.Invoke();

                    IsOnClickPlaying = false;
                    DrawEnd = true;
                    IsMultiDraw = false;

                    return;
                }


                if (IsJumpTg)
                {
                    rewardTime = 0.02f;
                    DrawWinning = true;
                }
                else
                {
                    rewardTime = setrewardTime;
                    rewardTiming = 0;
                    DrawWinning = false;
                    StartDrawAni().Coroutine();
                }

            }
        }

        //重置
        public void RePrepare()
        {
            if (IsOnClickPlaying)
            {
                return;
            }
            rewardTime = setrewardTime;
            rewardTiming = 0;

            DrawEnd = false;
            DrawWinning = false;
            IsOnClickPlaying = false;
            if (true)
            {
                for (int i = 0; i < rewardTransArr.Length; i++)
                {
                    rewardTransArr[i].sprite = ChangePrizeItemState();
                }
            }

        }

        public Sprite ChangePrizeItemState(E_PrizeState state = E_PrizeState.DEFAULT)
        {
            switch (state)
            {
                case E_PrizeState.DEFAULT:
                    return defaultSp;
                case E_PrizeState.TURN:
                    return TurnSp;
                case E_PrizeState.SELECT:
                    return SelectSp;
                default:
                    return defaultSp;
            }
        }
        public enum E_PrizeState
        {
            DEFAULT,//默认
            TURN,//滚动显示
            SELECT,//选中
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
