using UnityEngine;
using ETModel;
using UnityEngine.UI;
using UnityEngine.U2D;
using System.Linq;
using System.Collections.Generic;

namespace ETHotfix
{
    [ObjectSystem]
    public class UISelectServerComponentAwake : AwakeSystem<UISelectServerComponent>
    {
        public override void Awake(UISelectServerComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class UISelectServerComponentStart : StartSystem<UISelectServerComponent>
    {
        public override void Start(UISelectServerComponent self)
        {
            self.SetCurLineName();
            self.AnnouncInit();
        }
    }

    /// <summary>
    ///选择服务器组件
    /// </summary>
    public partial class UISelectServerComponent : Component
    {
        public ReferenceCollector collector;
        private Button openSelectSevBtn, CloseBtn, EnterGameBtn, AgeBtn, AnnouncBtn;
        private GameObject AgePanel;//适龄面板
        private GameObject SelectSeverPanel;//选择服务器面板
        private GameObject Content;//服务器线路 父对象
        private GameObject ZonList;//服务器大区 父对象
        private Image stateimag;//当前 服务器的状态
        private SpriteAtlas LinestateAtlas;//图集
        private Text curLineTxt;//当前线路得名字
        private int curZoneID;//当前的大区ID
        private int curLineID;//当前的线路ID
        LastLineInfo lastLineInfo;
        private bool autoEnterTriggered;

        

        public void Awake()
        {
            collector = GetParent<UI>().GameObject.GetReferenceCollector();
            openSelectSevBtn = collector.GetButton("selectSeverBtn");
            CloseBtn = collector.GetButton("closeBtn");
            EnterGameBtn = collector.GetButton("EnterGameBtn");
            AgeBtn = collector.GetButton("AgeBtn");
            AnnouncBtn = collector.GetButton("AnnouncBtn");
            AgePanel = collector.GetImage("AgePanel").gameObject;
            SelectSeverPanel = collector.GetGameObject("SelectSeverPanel");
            Content = collector.GetGameObject("Content");
            ZonList = collector.GetGameObject("ZonList");
            stateimag = collector.GetImage("stateimag");
            curLineTxt = collector.GetText("curLineTxt");
            LinestateAtlas = collector.GetSpriteAtlas("LinestateAtlas");
            lastLineInfo = new LastLineInfo 
            {
             ZoneId=GlobalDataManager.EnterZoneID,
             LineId=GlobalDataManager.EnterLineID,
             lineName=GlobalDataManager.EnterZoneName,
             linestate=0
            };
            InitSelectSeverPanel().Coroutine();
            openSelectSevBtn.onClick.AddSingleListener(OnOpenClick);
            CloseBtn.onClick.AddSingleListener(OnClosebtnCLick);
            collector.GetButton("close").onClick.AddSingleListener(OnClosebtnCLick);
            EnterGameBtn.onClick.AddSingleListener(OnEnterGameClick);
            AgeBtn.onClick.AddSingleListener(() => { AgePanel.SetActive(true); });
            AnnouncBtn.onClick.AddSingleListener(() => { AnnouncCollector.gameObject.SetActive(true); });

            AgePanel.transform.Find("AgeBG/CloseBtn").GetComponent<Button>().onClick.AddSingleListener(() => { AgePanel.SetActive(false); });
            AgePanel.SetActive(false);
            SelectSeverPanel.SetActive(true);
            RepairServerSelectVisuals();
            ScheduleServerSelectVisualRepairPasses();
           
        }
        private void OnOpenClick()
        {

            SelectSeverPanel.SetActive(true);
            RepairServerSelectVisuals();

        }
        private void OnClosebtnCLick()
        {
            SelectSeverPanel.SetActive(false);
        }

        private List<ServerZoneInfo> BuildDefaultZoneInfos()
        {
            return new List<ServerZoneInfo>
            {
                new ServerZoneInfo
                {
                    ZoneId = 1,
                    Zonename = "永久区",
                    ZoneType = (int)E_LineStateType.NORMAL,
                    ZoneState = (int)E_LineState.OPEN
                }
            };
        }

        private void ApplyZoneInfos(List<ServerZoneInfo> serverZoneInfosList, bool forceSelectFirst)
        {
            int starIndex = serverZoneInfosList.Count;
            int endIndx = ZonList.transform.childCount;
            Log.DebugBrown("大区的数量" + starIndex + ":列表数量" + endIndx);
            if (endIndx > starIndex)
            {
                for (int i = starIndex; i < endIndx; i++)
                {
                    Transform item = ZonList.transform.GetChild(i);
                    item.gameObject.SetActive(false);
                }
            }

            Transform temp = ZonList.transform.GetChild(0);
            int index = 0;
            for (int i = 0; i < serverZoneInfosList.Count; i++)
            {
                ServerZoneInfo serverZoneInfo = serverZoneInfosList[i];

                Transform zoneItem = null;
                if (index < endIndx)
                {
                    zoneItem = ZonList.transform.GetChild(index);
                }
                else
                {
                    zoneItem = GameObject.Instantiate<Transform>(temp, ZonList.transform);
                }

                zoneItem.Find("Label").GetComponent<Text>().text = serverZoneInfo.Zonename;
                zoneItem.gameObject.SetActive(true);
                zoneItem.GetComponent<Toggle>().onValueChanged.AddSingleListener((ison) =>
                {
                    if (!ison) return;
                    if (serverZoneInfo.ZoneState == (int)E_LineState.CLOSE)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "大区还未开放");
                        return;
                    }
                    GlobalDataManager.EnterZoneID = serverZoneInfo.ZoneId;
                    GlobalDataManager.EnterZoneName = serverZoneInfo.Zonename.ToString();
                    curZoneID = serverZoneInfo.ZoneId;
                    RepairServerSelectVisuals();
                    RefreshLines(serverZoneInfo.ZoneId).Coroutine();
                });
                zoneItem.GetComponent<Toggle>().isOn = forceSelectFirst && index == 0;
                index++;
            }

            if (serverZoneInfosList.Count > 0)
            {
                GlobalDataManager.EnterZoneID = serverZoneInfosList[0].ZoneId;
                GlobalDataManager.EnterZoneName = serverZoneInfosList[0].Zonename;
                curZoneID = serverZoneInfosList[0].ZoneId;
                if (forceSelectFirst)
                {
                    RefreshLines(serverZoneInfosList[0].ZoneId).Coroutine();
                }
            }

            RepairServerSelectVisuals();
        }

        private void ApplyLineInfos(int zoneId, LineInfo[] lineInfos)
        {
            int severLinecount = lineInfos.Length;
            int ItemCount = Content.transform.childCount;
            Log.DebugBrown("当前大区的线路" + severLinecount + ":" + ItemCount);
            if (severLinecount == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请先选择自己的服务区");
                return;
            }
            if (ItemCount > severLinecount)
            {
                for (int i = severLinecount; i < ItemCount; i++)
                {
                    Transform item = Content.transform.GetChild(i);
                    item.gameObject.SetActive(false);
                }
            }
            BubbleSort(lineInfos);
            Transform temp = Content.transform.GetChild(0);
            for (int i = 0; i < severLinecount; i++)
            {
                Transform lineItem;

                LineInfo serverLineInfo = lineInfos[i];
                if (i < ItemCount)
                {
                    lineItem = Content.transform.GetChild(i);
                }
                else
                {
                    lineItem = GameObject.Instantiate<Transform>(temp, Content.transform);
                }

                lineItem.Find("state").GetComponent<Image>().sprite = GetSprite((E_LineStateType)serverLineInfo.GameAreaType);
                lineItem.Find("Text").GetComponent<Text>().text = serverLineInfo.GameAreaNickName;

                if (serverLineInfo.GameAreaId == GlobalDataManager.EnterLineID && zoneId == GlobalDataManager.EnterZoneID)
                {
                    GlobalDataManager.EnterZoneName = serverLineInfo.GameAreaNickName;
                    lastLineInfo.lineName = serverLineInfo.GameAreaNickName;
                    lastLineInfo.ZoneId = zoneId;
                    lastLineInfo.LineId = serverLineInfo.GameAreaId;
                    lastLineInfo.linestate = serverLineInfo.GameAreaType;
                    SetCurLineName();
                }

                lineItem.gameObject.SetActive(true);
                lineItem.GetComponent<Button>().onClick.AddSingleListener(() =>
                {
                    curZoneID = zoneId;
                    curLineID = serverLineInfo.GameAreaId;
                    GlobalDataManager.EnterLineID = serverLineInfo.GameAreaId;

                    lastLineInfo.lineName = serverLineInfo.GameAreaNickName;
                    lastLineInfo.ZoneId = zoneId;
                    lastLineInfo.LineId = serverLineInfo.GameAreaId;
                    lastLineInfo.linestate = serverLineInfo.GameAreaType;

                    SetCurLineName();
                    SelectSeverPanel.SetActive(false);
                });
            }

            if (severLinecount > 0)
            {
                LineInfo firstLine = lineInfos[0];
                curZoneID = zoneId;
                curLineID = firstLine.GameAreaId;
                GlobalDataManager.EnterLineID = firstLine.GameAreaId;
                GlobalDataManager.EnterZoneID = zoneId;
                GlobalDataManager.EnterZoneName = firstLine.GameAreaNickName;
                lastLineInfo.lineName = firstLine.GameAreaNickName;
                lastLineInfo.ZoneId = zoneId;
                lastLineInfo.LineId = firstLine.GameAreaId;
                lastLineInfo.linestate = firstLine.GameAreaType;
                SetCurLineName();
                TryAutoEnterGameArea().Coroutine();
            }

            RepairServerSelectVisuals();
        }

        private LineInfo[] BuildDefaultLineInfos(int zoneId)
        {
            return new[]
            {
                new LineInfo
                {
                    GameAreaId = 1,
                    GameAreaNickName = "永久区1线",
                    GameAreaType = (int)E_LineStateType.NORMAL,
                    IsGameAreaState = (int)E_LineState.OPEN
                },
                new LineInfo
                {
                    GameAreaId = 3,
                    GameAreaNickName = "永久区3线",
                    GameAreaType = (int)E_LineStateType.HOT,
                    IsGameAreaState = (int)E_LineState.OPEN
                },
                new LineInfo
                {
                    GameAreaId = 6,
                    GameAreaNickName = "永久区6线",
                    GameAreaType = (int)E_LineStateType.NEW,
                    IsGameAreaState = (int)E_LineState.OPEN
                }
            };
        }

        private void OnEnterGameClick()
        {
            Log.DebugBrown("1当前的数据" + curLineTxt.text.Length);
            if (curLineTxt.text.Length==0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请重新选择线路");
                return;
            }
            EntertGameArea().Coroutine();
         
        }

        private async ETVoid EntertGameArea()
        {
            LoginStageTrace.Append($"EnterGameArea start zone={curZoneID} line={curLineID} curLine={curLineTxt.text}");
            G2C_LoginSystemEnterGameAreaMessage g2C_EnterGameAreaMessage = (G2C_LoginSystemEnterGameAreaMessage)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemEnterGameAreaMessage
            {
                GameAreaId = curZoneID,//大区id
                LineId = curLineID//线路id
            });
            LoginStageTrace.Append($"EnterGameArea response error={g2C_EnterGameAreaMessage.Error} gameIds={g2C_EnterGameAreaMessage.GameIds} occupations={g2C_EnterGameAreaMessage.GameOccupation?.Count ?? 0}");
            Log.DebugBrown("请求登录游戏" + "区id" + curZoneID + ":" + curLineID);
            //提示错误信息
            if (g2C_EnterGameAreaMessage.Error != 0)
            {
                LoginStageTrace.Append($"EnterGameArea failed error={g2C_EnterGameAreaMessage.Error}");
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText(g2C_EnterGameAreaMessage.Error.GetTipInfo(), true);
                return;
            }
            else
            {
                LoginStageTrace.Append("EnterGameArea success cache occupations");
                RoleArchiveInfoManager.Instance.CanCreatRoleList = g2C_EnterGameAreaMessage.GameOccupation.ToList();
            }

            //获取角色信息
            LoginStageTrace.Append($"GetGamePlayerInfo start gameId={g2C_EnterGameAreaMessage.GameIds}");
            G2C_LoginSystemGetGamePlayerInfoResponse g2C_GamePlayerGetInfoResponse = (G2C_LoginSystemGetGamePlayerInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemGetGamePlayerInfoRequest
            {
                GameId = g2C_EnterGameAreaMessage.GameIds
            });
            LoginStageTrace.Append($"GetGamePlayerInfo response error={g2C_GamePlayerGetInfoResponse.Error} count={g2C_GamePlayerGetInfoResponse.GameInfos?.count ?? -1}");
            //提示错误信息
            if (g2C_GamePlayerGetInfoResponse.Error != 0)
            {
                LoginStageTrace.Append($"GetGamePlayerInfo failed error={g2C_GamePlayerGetInfoResponse.Error}");
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText(g2C_GamePlayerGetInfoResponse.Error.GetTipInfo(), true);
                return;
            }
            //缓存角色信息 显示角色使用
            RoleArchiveInfoManager.Instance.ResetRoleArchives();
            int length = g2C_GamePlayerGetInfoResponse.GameInfos.count;
            LoginStageTrace.Append($"GetGamePlayerInfo cache start count={length}");
            for (int i = 0; i < length; i++)
            {
                //缓存 角色信息 方便创建角色使用
                G2C_LoginSystemGetGamePlayerInfoMessage roleInfos = g2C_GamePlayerGetInfoResponse.GameInfos[i];
                RoleArchiveInfo roleArchive = new RoleArchiveInfo
                {
                    UUID = roleInfos.GameId,
                    Name = roleInfos.NickName,
                    Level = roleInfos.Level,
                    RoleType = roleInfos.PlayerType,
                    struct_ItemIns = roleInfos.AllEquipStatus.ToList(),
                    ClassLev=roleInfos.OccupationLevel
                };
                RoleArchiveInfoManager.Instance.Add(roleInfos.GameId, roleArchive);
            }
            LoginStageTrace.Append($"GetGamePlayerInfo cache finish total={RoleArchiveInfoManager.Instance.Count()}");
           
            //XYSDK 登录游戏服务器：在用户选择游戏服务器登录后调用
            //XySdkComponent.Instance.xySdk.LoginGame(new string[] {$"{GlobalDataManager.XYUUID}",$"{681}",$"{GlobalDataManager.EnterZoneID}" });

            //开始加载选择角色场景
            //场景加载进度
            LoginStageTrace.Append("ChooseRole visible loading start");
            Game.Scene.GetComponent<UIComponent>().VisibleUI(UIType.UISceneLoading);
            LoginStageTrace.Append("ChooseRole visible loading finish");
            GlobalDataManager.GCClear();
            LoginStageTrace.Append("ChooseRole GC clear finish");
            LoginStageTrace.Append("ChooseRole load scene start");
            SceneComponent.Instance.LoadScene(SceneName.ChooseRole.ToString());
            LoginStageTrace.Append("ChooseRole load scene call finish");
            UIComponent.Instance.Remove(UIType.UISelectServer);
            LoginStageTrace.Append("ChooseRole remove select server finish");
        }
        /// <summary>
        /// 设置当前线路得名字
        /// </summary>
        public void SetCurLineName()
        {
            if (string.IsNullOrEmpty(lastLineInfo.lineName))
            {
                SelectSeverPanel.SetActive(true);
                curLineTxt.text = string.Empty;
                curLineID = 0;
                curZoneID = 0;
                return;
            }

            stateimag.sprite = GetSprite((E_LineStateType)lastLineInfo.linestate);
            curLineTxt.text = lastLineInfo.lineName;
            curLineID = lastLineInfo.LineId;
            curZoneID = lastLineInfo.ZoneId;
           
            GlobalDataManager.EnterZoneID= lastLineInfo.ZoneId;
            GlobalDataManager.EnterZoneName = lastLineInfo.lineName;
            GlobalDataManager.EnterLineID = lastLineInfo.LineId;
            RepairServerSelectVisuals();

        }
        
        /// <summary>
        /// 初始化 选择列表面板
        /// </summary>
        private async ETVoid InitSelectSeverPanel()
        {
            List<ServerZoneInfo> defaultZones = BuildDefaultZoneInfos();
            ApplyZoneInfos(defaultZones, false);
            if (defaultZones.Count > 0)
            {
                ApplyLineInfos(defaultZones[0].ZoneId, BuildDefaultLineInfos(defaultZones[0].ZoneId));
            }

            List<ServerZoneInfo> serverZoneInfosList = null;
            try
            {
                G2C_LoginSystemGetServerInfoResponse g2C_GetGameAreaInfo = (G2C_LoginSystemGetServerInfoResponse)await SessionComponent.Instance.Session.Call(
                    new C2G_LoginSystemGetServerInfoRequest { GetGameAreaPage = 1 });
                var g2C_GameAreaInfos = g2C_GetGameAreaInfo.GameAreaInfos;
                serverZoneInfosList = new List<ServerZoneInfo>(g2C_GameAreaInfos.count);
                int indexs = 0;
                foreach (var item in g2C_GameAreaInfos)
                {
                    serverZoneInfosList.Insert(indexs, new ServerZoneInfo
                    {
                        ZoneId = item.GameAreaId,
                        Zonename = item.GameAreaNickName,
                        ZoneType = item.GameAreaType,
                        ZoneState = item.IsGameAreaState
                    });
                    indexs++;
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"InitSelectSeverPanel error: {e}");
            }

            if (serverZoneInfosList == null || serverZoneInfosList.Count == 0)
            {
                return;
            }
            ApplyZoneInfos(serverZoneInfosList, true);
        }

        /// <summary>
        /// 刷新当前大区的线路
        /// </summary>
        /// <param name="zoneId"></param>
        private async ETVoid RefreshLines(int zoneId)
        {
            LineInfo[] lineInfos = null;
            try
            {
                G2C_LoginSystemGetServerLineInfoResponse g2C_GetGameAreaLineInfo = (G2C_LoginSystemGetServerLineInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemGetServerLineInfoRequest
                {
                    GetGameAreaPage = 1,
                    AreaId = zoneId
                });
                var g2C_GetGameAreaLineInfos = g2C_GetGameAreaLineInfo.GameAreaInfos;
                int count = g2C_GetGameAreaLineInfos.Count;
                lineInfos = new LineInfo[count];
                int indexs = 0;
                foreach (var item in g2C_GetGameAreaLineInfo.GameAreaInfos)
                {
                    lineInfos[indexs] = new LineInfo
                    {
                        GameAreaId = item.GameAreaId,
                        GameAreaNickName = item.GameAreaNickName,
                        GameAreaType = item.GameAreaType,
                        IsGameAreaState = item.IsGameAreaState
                    };
                    indexs++;
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"RefreshLines error zone:{zoneId} {e}");
            }

            if (lineInfos == null || lineInfos.Length == 0)
            {
                return;
            }
            ApplyLineInfos(zoneId, lineInfos);
        }

        private async ETVoid TryAutoEnterGameArea()
        {
            if (!LoginDiagnosticOptions.AutoFlowEnabled)
            {
                return;
            }

            if (autoEnterTriggered)
            {
                return;
            }

            if (curZoneID <= 0 || curLineID <= 0 || string.IsNullOrEmpty(curLineTxt.text))
            {
                return;
            }

            autoEnterTriggered = true;
            LoginStageTrace.Append($"AutoEnterGameArea scheduled zone={curZoneID} line={curLineID} curLine={curLineTxt.text}");
            await TimerComponent.Instance.WaitAsync(300);

            if (this.IsDisposed)
            {
                return;
            }

            if (curZoneID <= 0 || curLineID <= 0 || string.IsNullOrEmpty(curLineTxt.text))
            {
                return;
            }

            LoginStageTrace.Append($"AutoEnterGameArea fire zone={curZoneID} line={curLineID}");
            EntertGameArea().Coroutine();
        }
        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="arr"></param>
      private  static void BubbleSort(LineInfo[] arr)
        {
            int n = arr.Length;
            bool swapped;
            for (int i = 0; i < n - 1; i++)
            {
                swapped = false;
                for (int j = 0; j < n - i - 1; j++)
                {
                    if (arr[j].GameAreaId > arr[j + 1].GameAreaId)
                    {
                        // Swap the elements
                        LineInfo temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                        swapped = true;
                    }
                }

                // If no two elements were swapped by inner loop, then break
                if (!swapped)
                    break;
            }
        }


        /// <summary>
        /// 根据服务器状态 获得对应得sprite
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Sprite GetSprite(E_LineStateType type)
        {
            if (LinestateAtlas == null)
            {
                return null;
            }

            try
            {
                switch (type)
                {
                    case E_LineStateType.NORMAL:
                        return LinestateAtlas.GetSprite("green");
                    case E_LineStateType.HOT:
                        return LinestateAtlas.GetSprite("red");
                    case E_LineStateType.NEW:
                    case E_LineStateType.NONE:
                    default:
                        return LinestateAtlas.GetSprite("yellow");
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"UISelectServer GetSprite error type:{type} {e}");
                return null;
            }
        }
        

    }
}
