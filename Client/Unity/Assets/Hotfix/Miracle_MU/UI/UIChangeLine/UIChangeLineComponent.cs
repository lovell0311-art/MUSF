using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{

    [ObjectSystem]
    public class UIChangeLineComponentAwake : AwakeSystem<UIChangeLineComponent>
    {
        public override void Awake(UIChangeLineComponent self)
        {
          
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIChangeLine));
            self.Content = self.collector.GetGameObject("Lines").transform;
            self.GetCurZoneSeverLines(GlobalDataManager.EnterZoneID).Coroutine();
        }
    }
    public class UIChangeLineComponent : Component
    {
        public ReferenceCollector collector;
        public Transform Content;
        float changeLineSpacetime;//线路切换间隔时间
        
        /// <summary>
        /// 获取当前大区下的所有线路
        /// </summary>
        /// <param name="zoneId">大区ID</param>
        /// <returns></returns>
        public async ETVoid GetCurZoneSeverLines(int zoneId)
        {
            //获取大区下的线路信息
            G2C_LoginSystemGetServerLineInfoResponse g2C_GetGameAreaLineInfo = (G2C_LoginSystemGetServerLineInfoResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemGetServerLineInfoRequest
            {
                GetGameAreaPage = 1,
                AreaId = zoneId
            });
            var g2C_GetGameAreaLineInfos = g2C_GetGameAreaLineInfo.GameAreaInfos;
            //显示线路列表
            int severLinecount = g2C_GetGameAreaLineInfos.Count;//当前大区得线路数量
            int ItemCount = Content.transform.childCount;//线路Item数量
            Log.DebugBrown("打开切线面获取的线路数量" + severLinecount);
            if (ItemCount > severLinecount)//线路Item数量 >当前大区得线路数量 隐藏多余的列表Item
            {
                for (int i = severLinecount; i < ItemCount; i++)
                {
                    Transform item = Content.transform.GetChild(i);
                    item.gameObject.SetActive(false);
                }
            }

            LineInfo[] lineInfos = new LineInfo[severLinecount];
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
            BubbleSort(lineInfos);
            Transform temp = Content.transform.GetChild(0);
            for (int i = 0; i < severLinecount; i++)
            {
                Transform lineItem;
            
                LineInfo serverLineInfo = lineInfos[i];

                if (i < ItemCount)
                    lineItem = Content.transform.GetChild(i);
                else
                    lineItem = GameObject.Instantiate<Transform>(temp, Content.transform);
                //设置线路名字
                lineItem.Find("name").GetComponent<Text>().text =serverLineInfo.GameAreaNickName;
                lineItem.gameObject.SetActive(true);
                lineItem.GetComponent<Button>().onClick.AddSingleListener(async () =>
                {
                    if (Time.time < changeLineSpacetime)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "线路切换太频繁 请稍后再试");
                        return;
                    }

                    UIMainComponent.Instance.UserMount(true);
                    changeLineSpacetime = Time.time + 10;//每隔五秒 切换一次
                    UIMainComponent.Instance.IsChangeLine = true;
                    //暂停挂机
                    if (RoleOnHookComponent.Instance.IsOnHooking)
                    {
                        UIMainComponent.Instance.StopOnHook();
                        RoleOnHookComponent.Instance.IsAttack = false;
                        RoleOnHookComponent.Instance.IsReturen = false;
                        RoleOnHookComponent.Instance.pathComponent.StopMove();
                    }

                    ////选择线路后 保存选择的线路信息 方便下一次使用
                    //LastLineInfo lastLineInfo = new LastLineInfo
                    //{
                    //    lineName = serverLineInfo.GameAreaNickName,//名字
                    //    linestate = (int)serverLineInfo.GameAreaType,//状态
                    //    ZoneId = zoneId,//大区id
                    //    LineId = serverLineInfo.GameAreaId//线路id
                    //};
                   

                    //LogCollectionComponent.Instance.Info($"#换线# 开始换线 当前大区:{zoneId}  线路：{serverLineInfo.GameAreaId}");
                    //LocalDataJsonComponent.Instance.SavaData(lastLineInfo, LocalJsonDataKeys.LastLineInfo);
                   
                    //换线
                    Gate2C_KickRole gate2C_KickRole = (Gate2C_KickRole)await SessionComponent.Instance.Session.Call(new C2Gate_KickRole { });
                    if (gate2C_KickRole.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, gate2C_KickRole.Error.GetTipInfo());
                        LogCollectionComponent.Instance.Warn($"#换线# 下线角色失败 Error:{gate2C_KickRole.Error}");
                    }
                    else
                    {
                        //清理实体数据
                       
                        UnitEntityComponent.Instance.Clear(false);
                        TeamDatas.Clear();
                        TreasureMapComponent.Instance.Clear();// 宝藏小地图icon
                        UnitEntityComponent.Instance.LocalRole.GetComponent<BufferComponent>().ClearBuffer();
                        G2C_LoginSystemEnterGameAreaMessage g2C_EnterGameAreaMessage = (G2C_LoginSystemEnterGameAreaMessage)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemEnterGameAreaMessage
                        {
                            GameAreaId = zoneId,//大区id
                            LineId = serverLineInfo.GameAreaId//线路id
                        });
                        //提示错误信息
                        if (g2C_EnterGameAreaMessage.Error != 0)
                        {

                            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                            uIConfirm.SetTipText(g2C_EnterGameAreaMessage.Error.GetTipInfo(), true);
                            return;
                        }
                        else
                        {
                            RoleArchiveInfoManager.Instance.CanCreatRoleList = g2C_EnterGameAreaMessage.GameOccupation.ToList();
                        }
                        G2C_StartGameGamePlayerResponse g2C_GamePlayerStartGameResponse = (G2C_StartGameGamePlayerResponse)await SessionComponent.Instance.Session.Call(new C2G_StartGameGamePlayerRequest
                        {
                            GameUserId = UnitEntityComponent.Instance.LocaRoleUUID
                        });
                        if (g2C_GamePlayerStartGameResponse.Error != 0)
                        {
                            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                            uIConfirm.SetTipText($"{g2C_GamePlayerStartGameResponse.Error.GetTipInfo()}", true);
                            return;
                        }
                        else
                        {
                            //创建玩家
                        
                            LogCollectionComponent.Instance.GameUserId = UnitEntityComponent.Instance.LocaRoleUUID;
                            //请求获取 玩家的场景 坐标信息 以及属性信息
                            G2C_ReadyResponse g2C_Ready = (G2C_ReadyResponse)await SessionComponent.Instance.Session.Call(new C2G_ReadyRequest { });
                            if (g2C_Ready.Error != 0)
                            {
                                Log.DebugRed($"{g2C_Ready.Error.GetTipInfo()}");
                                return;
                            }

                            G2C_EnterChatRoom g2C_EnterChatRoom = (G2C_EnterChatRoom)await SessionComponent.Instance.Session.Call(new C2G_EnterChatRoom
                            {
                                ChatRoomID = GlobalDataManager.EnterZoneID
                            });
                            if (g2C_EnterChatRoom.Error != 0)
                            {
                                Log.DebugRed($"请求进入聊天房间错误:{g2C_EnterChatRoom.Error.GetTipInfo()}");

                            }
                            else
                            {
                               
                                ChatMessageDataManager.valuePairs[E_ChatType.World] = GlobalDataManager.EnterZoneID;
                            }
                            CameraComponent.Instance.MainCamera.farClipPlane = 450;
                            
                        }
                    }
                   
                   // UnitEntityComponent.Instance.LocalRole.GetComponent<BufferComponent>().ClearBuffer();
                    LogCollectionComponent.Instance.Info($"#换线# 线路切换完成");
                    GlobalDataManager.EnterLineID = serverLineInfo.GameAreaId;
                    GlobalDataManager.EnterZoneName = serverLineInfo.GameAreaNickName;
                    UIMainComponent.Instance.IsChangeLine = false;
                    //关闭 线路选择面板
                    UIMainComponent.Instance.SetCurSeverLine();
                    UIComponent.Instance.Remove(UIType.UIChangeLine);
                    UIMainComponent.Instance.ResetTeamWhenChangeLine();
                });
            }
        }


        private static void BubbleSort(LineInfo[] arr)
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
        public string GetColor(E_LineStateType stateType)
        {
            switch (stateType)
            {
                case E_LineStateType.NORMAL:
                    return ColorTools.GetColorHtmlString(Color.green);
                case E_LineStateType.HOT:
                    return ColorTools.GetColorHtmlString(Color.red);
                case E_LineStateType.NEW:
                    return ColorTools.GetColorHtmlString(Color.yellow);
                case E_LineStateType.NONE:
                    return ColorTools.GetColorHtmlString(Color.green);
                default:
                    break;
            }
            return ColorTools.GetColorHtmlString(Color.green);
        }


    }


    
}