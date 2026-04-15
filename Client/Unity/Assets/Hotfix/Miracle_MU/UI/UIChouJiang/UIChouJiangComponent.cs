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

    public class Prize
    {
        public int boxId;//物品位置
        public string iconRes;//物品图标资源名
        public string name;//物品名字
    }

    public enum E_DrawType 
    {
     RULE,//抽奖规则
     DRAWRECORD,//抽奖记录
    }
    //抽奖记录
    public class DrawLog
    {
        public long logId;
        public long UserID;
        public long GameUserId;
        public string NickName;
        public string Des;
    }

    [ObjectSystem]
    public class UIChouJiangComponentAwake : AwakeSystem<UIChouJiangComponent>
    {
        public override void Awake(UIChouJiangComponent self)
        {
            self.reference = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.Init_Sp();
            self.InitPrize();

            self.mojingCount = self.reference.GetText("mojing");
            self.mojingCount.text = $"魔晶：{UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing)}";

            self.DreawCount = self.reference.GetText("DreawCount");

            self.EnergyFill = self.reference.GetImage("EnergyFill");

            self.RecordsInfos = self.reference.GetImage("RecordsInfos").gameObject;
            self.Rule = self.reference.GetGameObject("Rule");
            self.reference.GetToggle("DrawRule").onValueChanged.AddSingleListener((value) => 
            {
                if (value == false) return;
                self.ChangeDrawInfo(E_DrawType.RULE);
            });
            self.reference.GetToggle("DrawRecords").onValueChanged.AddSingleListener((value) => 
            {
                if (value == false) return;
                self.ChangeDrawInfo(E_DrawType.DRAWRECORD);
            });

            self.reference.GetToggle("DrawRule").isOn = true;
            self.Rule.SetActive(true);
            self.RecordsInfos.SetActive(false);

            self.Init_DrawScrollView();

            self.Init_Show();

            self.reference.GetButton("StartPrize").onClick.AddSingleListener(() =>
            {
                self.IsJumpTg = false;
                self.OnClickDrawFun().Coroutine();

            });
            self.reference.GetButton("StartPrize_10").onClick.AddSingleListener(() =>
            {
                self.IsJumpTg = true;
                self.OnClickDrawFun(10).Coroutine();
              
            });
            self.reference.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                if (self.IsOnClickPlaying)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint,"正在抽奖，稍后关闭");
                    return;
                }
                UIComponent.Instance.Remove(UIType.UIChouJiang);
            });
            self.PlayingAction = self.Show_Prizes;

            self.IsJumpTg = true;
        }
    }

    [ObjectSystem]
    public class UIChouJiangComponentUpdate : UpdateSystem<UIChouJiangComponent>
    {
        public override void Update(UIChouJiangComponent self)
        {
            if (self.drawEnd) return;
            if (!self.IsOnClickPlaying) return;
            //抽奖显示
            self.rewardTiming += Time.deltaTime;
            if (self.rewardTiming >= self.rewardTime)
            {
                self.rewardTiming = 0;

                self.haloIndex++;
                if (self.haloIndex >=self.rewardTransArr.Length)
                {
                    self.haloIndex = 0;
                }
                self.SetDrawPos(self.haloIndex);

            }

        }
    }

    public partial class UIChouJiangComponent : Component
    {

        public ReferenceCollector reference;
        public Dictionary<int, Prize> PrizeDic = new Dictionary<int, Prize>();//奖品字典 key:奖品所在位置 value:奖品信息
        public List<int> PrizeList = new List<int>();//中奖ID集合

        private Sprite defaultSp, TurnSp, SelectSp;
        private SpriteAtlas spriteAtlas;
        public Text mojingCount;//魔晶数量
        public Text DreawCount;//抽奖次数
        public Image EnergyFill;//抽奖进度

        public int OneDraw = 25;//单次抽奖花费魔晶数
        public int TenDraw = 200;//单次抽奖花费魔晶数

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
        public int haloIndex =0;
        //抽奖结束的事件
        public Action PlayingAction;
      
        //连抽集合
        Queue<int> indexList = new Queue<int>();
        public bool IsMultiDraw = false;//是否连抽
        // 点了抽奖按钮正在抽奖
        private bool isOnClickPlaying;
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
            DrawLogScrollrect=reference.GetImage("RecordsInfos").GetComponent<ScrollRect>();
            DrawLogContent = reference.GetGameObject("Content");
            DrawLogScrollView = ComponentFactory.Create<UICircularScrollView<DrawLog>>();
            DrawLogScrollView.ItemInfoCallBack = InitDrawCallBack;
            DrawLogScrollView.InitInfo(E_Direction.Vertical, 1, 0, 10);
            DrawLogScrollView.IninContent(DrawLogContent, DrawLogScrollrect);
        }

        private void InitDrawCallBack(GameObject go, DrawLog info) 
        {
            go.GetComponent<Text>().text = "<color=yellow>"+info.NickName+"</color> "+info.Des;
        }

        public void ChangeDrawInfo(E_DrawType drawType = E_DrawType.RULE)
        {
            if (drawType == E_DrawType.RULE)
            {
                Rule.SetActive(true);
                RecordsInfos.SetActive(false);
            }
            else if (drawType == E_DrawType.DRAWRECORD)
            {
                Rule.SetActive(false);
                RecordsInfos.SetActive(true);
                //抽奖记录
                GetLotteryLog().Coroutine();
            }

            async ETVoid GetLotteryLog() 
            {
                G2C_GetLotteryLog g2C_GetLotteryLog = (G2C_GetLotteryLog)await SessionComponent.Instance.Session.Call(new C2G_GetLotteryLog 
                {
                GameUserId=UnitEntityComponent.Instance.LocaRoleUUID,
                EndLogId=0,
                Count=20,
                });
                if (g2C_GetLotteryLog.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_GetLotteryLog.Error.GetTipInfo());
                }
                else
                {
                    DrawLogLis.Clear();
                    foreach (var item in g2C_GetLotteryLog.AllLog)
                    {
                        DrawLogLis.Add(new DrawLog 
                        {
                         logId=item.LogId,
                         UserID=item.UserId,
                         GameUserId=item.GameUserId,
                         NickName=item.NickName,
                         Des=item.Desc
                        });
                    }
                    DrawLogScrollView.Items = DrawLogLis;
                }
               
            }
        }

        public void UpdateMoJing() 
        {
           mojingCount.text = $"魔晶：{UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing)}";
        }


        public void Init_Sp()
        {
            defaultSp = reference.GetSprite("3");
            TurnSp = reference.GetSprite("turn");
            SelectSp = reference.GetSprite("select");
        }

        //初始化奖品
        public void InitPrize()
        {
            spriteAtlas = SpriteUtility.Instance.GetSpriteAtlas(AtalsType.UI_ChouJiang_Icons);
            rewardTran = reference.GetGameObject("Prizes").transform;

            InitDraw().Coroutine();
          
            // 默认展示时间
            rewardTime = setrewardTime;
            rewardTiming = 0;

            DrawEnd = false;
            DrawWinning = false;
            IsOnClickPlaying = false;

            //获取抽奖信息
            async ETVoid InitDraw()
            {
                G2C_OpenLotteryWin g2C_OpenLotteryWin = (G2C_OpenLotteryWin)await SessionComponent.Instance.Session.Call(new C2G_OpenLotteryWin { });
                if (g2C_OpenLotteryWin.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenLotteryWin.Error.GetTipInfo());
                }
                else
                {
                    //抽奖物品
                    rewardTransArr = new Image[rewardTran.childCount];
                    foreach (var item in g2C_OpenLotteryWin.AllGiftInfo)
                    {
                        var prize = rewardTran.Find((item.Id).ToString());
                        //Log.DebugGreen($"{item.ItemIcon}  {item.Id}  {item.ItemName}");
                        Sprite sprite = spriteAtlas.GetSprite(item.ItemIcon);
                        if (sprite == null)
                        {
                            sprite = spriteAtlas.GetSprite("金币");
                        }
                        prize.Find("icon").GetComponent<Image>().sprite = sprite;
                        prize.Find("icon").GetComponent<Image>().SetNativeSize();
                        prize.Find("name").GetComponent<Text>().text = item.ItemName;
                        
                        rewardTransArr[item.Id-1] = prize.GetComponent<Image>();
                        PrizeDic[(int)item.Id-1] = new Prize { boxId=(int)item.Id,iconRes=item.ItemIcon,name=item.ItemName};
                    }
                    EnergyFill.fillAmount = (float)g2C_OpenLotteryWin.TotalCount / g2C_OpenLotteryWin.TotalCountMax;
                    DreawCount.text = $"已抽次数:{g2C_OpenLotteryWin.TotalCount}/{g2C_OpenLotteryWin.TotalCountMax}";

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
            if ((count == 1 && mojingcount < OneDraw)|| count == 10 && mojingcount < TenDraw)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"魔晶不足");
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
               //背包已满 邮件发送 g2C_StartLottery.IsSendMail == 1
                tips.SetActive(g2C_StartLottery.IsSendMail == 1);
                
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
                indexList.Enqueue(id-1);
                PrizeList.Add(id-1);
            
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
            if (DrawWinning && indexList.Count>0&& index == indexList.Peek())
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
            SpriteUtility.Instance.ClearAtals(AtalsType.UI_ChouJiang_Icons);
            DrawLogScrollView.Dispose();
            DrawLogLis.Clear();
            base.Dispose();
        }

    }
}
