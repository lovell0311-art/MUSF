using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using ILRuntime.Runtime;
using LitJson;

using ETModel;
using UnityEditor;
using System.Linq;

namespace ETHotfix
{

    /// <summary>
    /// 任务
    /// </summary>
    public partial class UITaskComponent : Component
    {
        //public GameObject UIBeginnerGuideMove, UIBeginnerGuideGet, UIBeginnerGuideShouLie, UIBeginnerGuideGetTask, UIBeginnerGuideGetBuff;
        public ReferenceCollector collector;
        public RoleEntity roleEntity => UnitEntityComponent.Instance.LocalRole;

        UICircularScrollView<TaskInfo> TaskCircularScrollView;
        public ScrollRect MainScrollRect;
        public GameObject MainContent;
        public GameObject TaskBg;
        public GameObject ActiveMap;

        /// <summary>
        /// 当前任务
        /// </summary>
        public TaskInfo CurTaskInfo;
        public Text curTaskInfoState;

        public Text TaskDes, RequestText, RewardText;

        public E_TaskType curTaskType;
        public E_TaskStatus curTaskStatus;
        public Toggle curTog = null;


        public Button GitTaskBtn, GiveUpBtn, MoveBtn, GitRewardBtn;

        //任务类型 字典
        private readonly Dictionary<E_TaskType, Toggle> TaskTogDic = new Dictionary<E_TaskType, Toggle>();

        public bool IsNpc = true;

        //是否拥有小飞鞋
        public bool IsHaveXiaoFeiXie = false;

        public void IsHaveXiaofeixie()
        {
            var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
            foreach (var item in list)
            {
                if (KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id) == false) continue;
                if (item.ConfigId == 320402)
                {
                    IsHaveXiaoFeiXie = true;
                    break;
                }
            }

            MoveBtn.transform.Find("icon").gameObject.SetActive(IsHaveXiaoFeiXie);
        }



        /// <summary>
        /// 注册 按钮事件
        /// </summary>
        public void RegisterEvent()
        {
            //领取任务
            GitTaskBtn.onClick.AddSingleListener(async () =>
            {

                if (IsNpc == false)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请前往幻影导师 领取任务");
                    return;
                }

                if (CurTaskInfo == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请先选择一个任务");
                    return;
                }

                //是否是一次性任务
                if (CurTaskInfo.TaskType == E_TaskType.HuntingTask && CurTaskInfo.OneTimeTask == 1)
                {
                    if (TaskDatas.GetTaskInfo(E_TaskType.HuntingTask, (int)CurTaskInfo.Id).State >= 2)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "该任务 不能重复领取");
                        return;
                    }
                }
                var task = TaskDatas.GetCurTaskInfo(CurTaskInfo.TaskType);
                if (task != null && task.Id != CurTaskInfo.Id)
                {
                    if (task.State == 2)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"<color=white>{task.TaskName}</color>奖励还未领取 无法领取当前任务");
                        return;
                    }
                    else if (task.State == 1)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, $"<color=white>{task.TaskName}</color> 还未完成 无法领取当前任务");
                        return;
                    }

                }
                //判断等级是否足够
                if (roleEntity.Property.GetProperValue(E_GameProperty.Level) < CurTaskInfo.ReqLevelMin)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "不满足任务最低等级 无法领取");
                    return;
                }
                if (roleEntity.Property.GetProperValue(E_GameProperty.Level) > CurTaskInfo.ReqLevelMax)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "等级大于任务的最高等级 无法领取");
                    return;
                }

                G2C_ReceiveTask g2C_Receive = (G2C_ReceiveTask)await SessionComponent.Instance.Session.Call(new C2G_ReceiveTask
                {
                    TaskId = CurTaskInfo.Id.ToInt32()
                });
                //Log.Info(JsonHelper.ToJson(g2C_Receive));

                if (g2C_Receive.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Receive.Error.GetTipInfo());
                }
                else
                {
                    if (CurTaskInfo.Id == 400000)
                    {
                        UIComponent.Instance.Remove(UIType.UITask);
                    }
                    else
                    {
                        //if (BeginnerGuideData.IsComplete(42))
                        //{
                        //    BeginnerGuideData.SetBeginnerGuide(42);
                        //}
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "任务 领取成功！");
                        ChangeBtnStatus(E_TaskStatus.AlreadyGitTask);
                        UIMainComponent.Instance.SetTaskInfo(CurTaskInfo);
                        if (curTaskInfoState != null)
                            curTaskInfoState.text = "进行中";
                    }
                }


            });
            //放弃任务
            GiveUpBtn.onClick.AddSingleListener(async () =>
            {
                if (CurTaskInfo.Id == -1) return;

                G2C_AbandonTask g2C_Abandon = (G2C_AbandonTask)await SessionComponent.Instance.Session.Call(new C2G_AbandonTask { TaskId = CurTaskInfo.Id.ToInt32() });
                if (g2C_Abandon.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Abandon.Error.GetTipInfo());
                }
                else
                {
                    ChangeBtnStatus(E_TaskStatus.NotGitTask);
                    UIMainComponent.Instance.GiveUpTask(CurTaskInfo);
                    var taskindex = GetTaskIndex(CurTaskInfo);
                    if (MainContent.transform.Find($"{taskindex}") is Transform task)
                    {
                        task.GetComponent<Toggle>().isOn = false;
                    }
                    CurTaskInfo = null;
                    if (curTaskInfoState != null)
                        curTaskInfoState.text = "";
                }

            });

            //移动
            MoveBtn.onClick.AddSingleListener(() =>
            {
                //副本
                if (SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.cangbaotu.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.kalima_map.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.GuZhanChang.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.XueSeChengBao.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.EMoGuangChang.GetSceneName() || SceneComponent.Instance.CurrentSceneName.ToEnum<SceneName>().GetSceneName() == SceneName.ShiLianZhiDi.GetSceneName())
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "当前所在副本，无法进行任务");
                    return;
                }

                if (roleEntity.GetComponent<RoleStallUpComponent>().IsStallUp)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "当前处于摆摊状态 无法移动");
                    return;
                }
                if (IsNpc == false && curTaskType == E_TaskType.HuntingTask && UIMainComponent.Instance.HuntTask.gameObject.activeSelf == false)
                {
                    //狩猎任务 寻找最近的NPC
                    float mindis = 0;
                    AstarNode astar = null;
                    if (UIMainComponent.Instance.NPCSpawnPointLists.Count is int count && count != 0)
                    {
                        foreach (var item in UIMainComponent.Instance.NPCSpawnPointLists)
                        {
                            if (item.Index == 10025)//幻影导师
                            {
                                if (mindis == 0)
                                {
                                    mindis = PositionHelper.Distance(new Vector2(item.PositionX, item.PositionY), new Vector2(UIMainComponent.Instance.rolecurAstarNode.x, UIMainComponent.Instance.rolecurAstarNode.z));
                                }
                                Log.Info("mindis =" + mindis);
                                var dis = PositionHelper.Distance(new Vector2(item.PositionX, item.PositionY), new Vector2(UIMainComponent.Instance.rolecurAstarNode.x, UIMainComponent.Instance.rolecurAstarNode.z));
                                if (dis < mindis || astar == null)
                                {
                                    mindis = dis;
                                    astar = AstarComponent.Instance.GetNode(item.PositionX, item.PositionY);
                                    //break;
                                }
                            }

                        }
                        if (astar != null)
                        {

                            bool isbreak = false;
                            if (astar.isWalkable == false)
                            {
                                for (int i = -1; i < 1; i++)
                                {
                                    for (int j = -1; j < 1; j++)
                                    {
                                        if (i == 0 && j == 0) continue;
                                        /*    AstarNode vector = astar + new Vector3Int(i, 0, j) * 2;
                                            var nearNode = role.CurrentNodePos;
                                            //AstarNode node = AstarComponent.Instance.GetNodeVector(vector.x, vector.z);*/
                                        AstarNode node = AstarComponent.Instance.GetNode(astar.x + i, astar.z + j);
                                        if (node.isWalkable)
                                        {
                                            //可以站立

                                            //UnitEntityPathComponent.NavTarget(vector);//移动到该地点
                                            astar = node;
                                            isbreak = true;
                                            break;
                                        }

                                    }
                                    if (isbreak)
                                        break;
                                }
                            }
                            //移动到幻影导师
                            UnitEntityComponent.Instance.LocalRole.GetComponent<UnitEntityPathComponent>().NavTarget(astar);
                            UIComponent.Instance.Remove(UIType.UITask);
                        }
                    }
                    return;
                }

                if (RoleOnHookComponent.Instance.IsOnHooking)
                {
                    UIMainComponent.Instance.StopOnHook();
                }

                if (CurTaskInfo == null || CurTaskInfo.Id == -1 || CurTaskInfo.State == 0) return;

                if (CurTaskInfo.State == 2 || CurTaskInfo.State == 3)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "当前任务已完成\t请你领取下一个任务");
                    return;
                }

                if (IsHaveXiaoFeiXie && CurTaskInfo.Pos_X != 0)
                {
                    UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirm.SetTipText("是否使用小飞鞋特权\n直接飞到任务目标点");
                    uIConfirm.AddActionEvent(async () =>
                    {
                        G2C_TaskTransferResponse g2C_TaskTransfer = (G2C_TaskTransferResponse)await SessionComponent.Instance.Session.Call(new C2G_TaskTransferRequest { MapId = CurTaskInfo.MapId / 100, X = CurTaskInfo.Pos_X, Y = CurTaskInfo.Pos_Y });
                    });
                    uIConfirm.AddCancelEventAction(() => { CurTaskInfo.Nav2Target(); });
                }
                else
                {
                    CurTaskInfo.Nav2Target();
                }

                //if (BeginnerGuideData.IsComplete(28))
                //{
                //    UIMainComponent.Instance.SetMask(true);
                //    BeginnerGuideData.SetBeginnerGuide(28);
                //}
                //else if (BeginnerGuideData.IsComplete(37))
                //{
                //    UIMainComponent.Instance.SetMask(true);
                //    BeginnerGuideData.SetBeginnerGuide(37);
                //}
                Close();
            });
            //领取奖励
            GitRewardBtn.onClick.AddSingleListener(async () =>
            {
                if (CurTaskInfo == null)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "无法领取");
                    return;
                }

                /* if (IsNpc == false && CurTaskInfo.TaskType!=E_TaskType.MainTask)
                 {
                     UIComponent.Instance.VisibleUI(UIType.UIHint, "请前往幻影导师 提交任务");
                     return;
                 }*/

                if (CurTaskInfo.State == 1)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"<color=white>{CurTaskInfo.TaskName}</color> 还未完成 无法领取");
                    return;
                };
                if (CurTaskInfo.State == 3)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "奖励已经领取");
                    return;
                };
                if (CurTaskInfo.State == 2)
                {
                    Log.Info(" 发送数据");
                    G2C_ReceiveTaskReward g2C_ReceiveTask = (G2C_ReceiveTaskReward)await SessionComponent.Instance.Session.Call(new C2G_ReceiveTaskReward { TaskId = CurTaskInfo.Id.ToInt32() });

                    if (g2C_ReceiveTask.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_ReceiveTask.Error.GetTipInfo());
                    }
                    else
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "奖励领取 成功！");

                        CurTaskInfo.IsReceiveRewards = true;
                        if (CurTaskInfo.TaskType != E_TaskType.MainTask)
                        {
                            ChangeBtnStatus(E_TaskStatus.NotGitTask);
                            UIMainComponent.Instance.GiveUpTask(CurTaskInfo);
                        }
                        else
                        {
                            //自动切换到下一任务
                            CurTaskInfo = TaskDatas.MainTaskInfo;
                            TaskCircularScrollView.Items = TaskDatas.MainTaskList;
                        }

                        if (curTaskInfoState != null)
                            curTaskInfoState.text = "";
                    }
                }
            });
        }

        public void UpdateTask()
        {
            CurTaskInfo.IsReceiveRewards = true;
            if (CurTaskInfo.TaskType == E_TaskType.MainTask)
            {
                //自动切换到下一任务
                CurTaskInfo = TaskDatas.MainTaskInfo;
                TaskCircularScrollView.Items = TaskDatas.MainTaskList;
            }
        }

        public void Close()
        {
            UIComponent.Instance.Remove(UIType.UITask);
        }
        /// <summary>
        /// 初始化 任务类型选项
        /// </summary>
        public void InitTaskTypeTog()
        {
            Transform togs = collector.GetGameObject("TaskTypeTog").transform;

            togs.transform.GetChild(5).GetComponent<Toggle>().isOn = true;
            TaskTogDic.Clear();
            for (int i = 0, length = togs.childCount; i < length; i++)
            {

                Toggle toggle = togs.GetChild(i).GetComponent<Toggle>();
                E_TaskType taskType = (E_TaskType)(i + 1);
                if (curTaskType == taskType)
                {
                    curTog = toggle;
                }

                toggle.onValueChanged.AddSingleListener((value) =>
                {
                    toggle.transform.Find("Label").GetComponent<Text>().color = value ? ColorTools.GetColorHtmlString("#FFAE00") : Color.white;
                    if (!value) return;
                    ClearRewar();
                    curTaskType = taskType;
                    TaskBg.SetActive(taskType != E_TaskType.ActivityMap);
                    if (taskType == E_TaskType.ActivityMap)
                    {
                        GetBattleCopyInfoRequest().Coroutine();
                    }
                    else
                    {
                        ActiveMap.SetActive(false);
                    }
                    switch (taskType)
                    {

                        case E_TaskType.MainTask:
                            CurTaskInfo = TaskDatas.MainTaskInfo;
                            TaskCircularScrollView.Items = TaskDatas.MainTaskList;
                            ChangeBtnStatus(E_TaskStatus.CompleteTask);//只显示 领取奖励、立即移动
                            break;
                        case E_TaskType.HuntingTask:
                            CurTaskInfo = TaskDatas.HuntTaskInfo;
                            TaskCircularScrollView.Items = TaskDatas.HuntTaskList;
                            if (CurTaskInfo != null)
                            {
                                Log.Info("HuntTaskInfo -- " + CurTaskInfo.TaskName);
                                if (CurTaskInfo.State == 2)
                                {
                                    ChangeBtnStatus(E_TaskStatus.CompleteTask);
                                }
                                else if (CurTaskInfo.State == 3)
                                {
                                    ChangeBtnStatus(E_TaskStatus.NotGitTask);
                                }
                                else
                                {
                                    ChangeBtnStatus(E_TaskStatus.AlreadyGitTask);
                                }

                            }
                            else
                            {
                                ChangeBtnStatus(E_TaskStatus.NotGitTask);
                            }
                            break;
                        case E_TaskType.ActivityTask:
                            CurTaskInfo = TaskDatas.ActivityTaskInfo;
                            TaskCircularScrollView.Items = TaskDatas.ActivityTaskList;
                            if (CurTaskInfo != null)
                            {
                                // Log.DebugBrown($"urTaskInfo.State:{CurTaskInfo.State}");
                                if (CurTaskInfo.State == 2)
                                {
                                    ChangeBtnStatus(E_TaskStatus.CompleteTask);
                                }
                                else if (CurTaskInfo.State == 3)
                                {
                                    ChangeBtnStatus(E_TaskStatus.NotGitTask);
                                }
                                else
                                {
                                    ChangeBtnStatus(E_TaskStatus.AlreadyGitTask);
                                }

                            }
                            else
                            {
                                ChangeBtnStatus(E_TaskStatus.NotGitTask);
                            }
                            break;
                        case E_TaskType.EntrustTask:
                            CurTaskInfo = TaskDatas.EntrustTaskInfo;
                            TaskCircularScrollView.Items = TaskDatas.EntrustTaskList;
                            if (CurTaskInfo != null)
                            {
                                Log.Info($"存在委托任务 {CurTaskInfo.State}  {CurTaskInfo.TaskName}  {CurTaskInfo.Id}");
                                if (CurTaskInfo.State == 2)
                                {
                                    ChangeBtnStatus(E_TaskStatus.CompleteTask);
                                }
                                else if (CurTaskInfo.State == 3)
                                {
                                    ChangeBtnStatus(E_TaskStatus.NotGitTask);
                                }
                                else if (CurTaskInfo.State == 1)
                                {
                                    ChangeBtnStatus(E_TaskStatus.AlreadyGitTask);
                                }
                                else
                                {
                                    ChangeBtnStatus(E_TaskStatus.NotGitTask);
                                }
                            }
                            else
                            {
                                Log.Info("没有接取任务");

                                ChangeBtnStatus(E_TaskStatus.NotGitTask);
                            }
                            break;
                        case E_TaskType.CareerChangeTask:
                            CurTaskInfo = TaskDatas.ChangeTaskInfo;
                            TaskCircularScrollView.Items = TaskDatas.CareerChangeTaskList;
                            ChangeBtnStatus(E_TaskStatus.NotGitTask);
                            Log.DebugBrown($"转职任务");
                            break;
                        case E_TaskType.ActivityMap:
                            //   Log.DebugBrown($"活动地图");
                            break;
                    }

                    ShoWCurTaskInfo();
                });
                toggle.isOn = false;
                TaskTogDic[taskType] = toggle;
            }
        }

        public void InitMainTaskScrollView()
        {
            TaskCircularScrollView = ComponentFactory.Create<UICircularScrollView<TaskInfo>>();
            MainScrollRect = collector.GetImage("TaskScrollView").GetComponent<ScrollRect>();
            MainContent = collector.GetGameObject("Content");
            TaskCircularScrollView.ItemInfoCallBack = InitTaskCallBack;
            TaskCircularScrollView.InitInfo(E_Direction.Vertical, 1, 0, 10);
            TaskCircularScrollView.IninContent(MainContent, MainScrollRect);
        }

        private void InitTaskCallBack(GameObject go, TaskInfo info)
        {


            string levstr = $"({info.ReqLevelMin}~{info.ReqLevelMax}级)";//等级限制
            if (info.ReqLevelMax == 0)
            {
                levstr = $"({info.ReqLevelMin}级以上)";
            }
            go.transform.Find("Label").GetComponent<Text>().text = " - " + info.TaskName + levstr;//任务名字

            //  if (info.TaskType != E_TaskType.HuntingTask)//狩猎、委托任务 不显示已完成
            if (info.TaskType == E_TaskType.MainTask || info.TaskType == E_TaskType.EntrustTask)//狩猎、委托任务 不显示已完成
            {
                bool isComplete = info.State == 2 || info.CurTaskIsComplete();
                go.transform.Find("Complete").gameObject.SetActive(isComplete);

                if (CurTaskInfo != null && info.Id == CurTaskInfo.Id)
                {
                    go.transform.Find("Text").GetComponent<Text>().text = info.State == 1 ? "进行中" : info.State == 2 ? "已完成" : info.State == 3 ? "已领取" : string.Empty;//任务是否已完成
                }
                else
                {
                    go.transform.Find("Text").GetComponent<Text>().text = isComplete ? "已完成" : string.Empty;//任务是否已完成
                }


                if (info.IsReceiveRewards)
                {
                    go.GetComponent<Toggle>().interactable = !isComplete;//已完成 当前任务不可领取
                }
                else
                {
                    go.GetComponent<Toggle>().interactable = true;
                }
            }
            else
            {
                go.transform.Find("Text").GetComponent<Text>().text = string.Empty;
                go.GetComponent<Toggle>().interactable = true;
            }

            go.GetComponent<Toggle>().onValueChanged.AddSingleListener((value) =>
            {
                if (!value)
                {

                    return;
                }

                //if (info.TaskName == "消灭蜘蛛！")
                //{
                //    if (BeginnerGuideData.IsComplete(41))
                //    {
                //        BeginnerGuideData.SetBeginnerGuide(41);
                //        UIBeginnerGuideGetTask.SetActive(true);
                //    }
                //}


                //  if (info.TaskType==E_TaskType.HuntingTask|| info.TaskType == E_TaskType.EntrustTask)//当前所选的任务不是主线任务 
                if (info.TaskType != E_TaskType.MainTask)//当前所选的任务不是主线任务 
                {
                    CurTaskInfo = TaskDatas.GetCurTaskInfo(info.TaskType);
                    if (CurTaskInfo != null && CurTaskInfo.Id == info.Id)//已经领取 该任务
                    {
                        if (info.State == 2)//已完成任务
                        {
                            ChangeBtnStatus(E_TaskStatus.CompleteTask);
                        }
                        else if (info.State == 3)//已完成任务
                        {
                            ChangeBtnStatus(E_TaskStatus.NotGitTask);
                        }
                        else//未完成任务
                        {
                            ChangeBtnStatus(E_TaskStatus.AlreadyGitTask);

                        }
                    }
                    else
                    {
                        ChangeBtnStatus(E_TaskStatus.NotGitTask);
                    }

                }
                if (curTaskType == E_TaskType.MainTask)
                {

                }
                else
                {
                    CurTaskInfo = info;
                    curTaskInfoState = go.transform.Find("Text").GetComponent<Text>();
                }

                //更新 任务信息
                TaskDes.text = info.TaskName + "\n" + info.TaskDes;
                RequestText.text = GetRequestTxt();
                RewardText.text = $"奖励经验：{info.RewardExp}\n奖励金币：{info.RewardCoin}\n奖励道具：{GetRewardItems()}";

            });
            go.GetComponent<Toggle>().isOn = false;
            CurTaskInfo = TaskDatas.GetCurTaskInfo(info.TaskType);
            go.GetComponent<Toggle>().isOn = CurTaskInfo != null && CurTaskInfo.Id == info.Id;

            /*  //狩猎任务 
              if (info.TaskType == E_TaskType.HuntingTask)
              {
                  //玩家等级超过狩猎任务的等级上限时 狩猎任务不显示
                  if (info.ReqLevelMax < roleEntity.Property.GetProperValue(E_GameProperty.Level))
                  {
                      go.GetComponent<Toggle>().interactable = false;
                  }
              }*/

            string GetRequestTxt() => (info.TaskActionType) switch
            {
                1 => "",
                2 => $"消灭怪物：{info.MonsterName} ({info.TaskTargetCounts})",
                3 => $"需要道具：{info.MonsterName}" + GetTaskCount(),
                4 => $"等级达到：{info.MonsterName}",
                _ => string.Empty
            };

            string GetTaskCount()
            {
                return string.IsNullOrEmpty(info.MonsterName) ? "" : $"({info.TaskTargetCounts})";
            }
            ///获取奖励道具
            string GetRewardItems()
            {

                if (TaskDatas.TaskRewardDic.TryGetValue(info.Id.ToInt32(), out StringBuilder RewardItemName))
                {
                    int index = RewardItemName.ToString().LastIndexOf("、");
                    string st = RewardItemName.ToString().Substring(0, index);
                    return st;
                }
                return "无";
            }
        }

        public void ChangeBtnStatus(E_TaskStatus taskStatus)
        {

            switch (taskStatus)
            {
                case E_TaskStatus.NotGitTask://未领取任务
                    GiveUpBtn.gameObject.SetActive(false);
                    MoveBtn.gameObject.SetActive(true);//立即移动无法点击
                    GitRewardBtn.gameObject.SetActive(false);
                    GitTaskBtn.gameObject.SetActive(true);
                    break;
                case E_TaskStatus.AlreadyGitTask://已经领取任务
                    GiveUpBtn.gameObject.SetActive(true);
                    MoveBtn.gameObject.SetActive(true);
                    GitTaskBtn.gameObject.SetActive(false);
                    GitRewardBtn.gameObject.SetActive(false);
                    break;
                case E_TaskStatus.CompleteTask://任务完成
                    MoveBtn.gameObject.SetActive(true);
                    GitRewardBtn.gameObject.SetActive(true);
                    GiveUpBtn.gameObject.SetActive(false);
                    GitTaskBtn.gameObject.SetActive(false);
                    break;
                case E_TaskStatus.None:
                    GitTaskBtn.gameObject.SetActive(false);
                    MoveBtn.gameObject.SetActive(false);
                    GitRewardBtn.gameObject.SetActive(false);
                    GiveUpBtn.gameObject.SetActive(false);
                    break;
            }
        }

        public void ShowCurTaskInfo(TaskInfo taskInfo)
        {
            if (taskInfo == null)
            {

            }
            else
            {

                TaskTogDic[taskInfo.TaskType].isOn = false;
                TaskTogDic[taskInfo.TaskType].isOn = true;
                var index = GetTaskIndex(taskInfo);
                MainContent.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(5, 180 * (index / 3), 0);
                CurTaskInfo = taskInfo;
                ActiveMap.SetActive(false);
            }
        }
        //显示当前任务信息
        public void ShoWCurTaskInfo()
        {
            if (CurTaskInfo == null) return;
            int index = GetTaskIndex(CurTaskInfo);
            MainContent.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(5, 180 * (index / 3), 0);
        }
        //获取当前任务 在list中的索引
        public int GetTaskIndex(TaskInfo taskInfo)
        {
            int index = 0;
            if (taskInfo == null) return 0;
            switch ((E_TaskType)(taskInfo.Id / 100000))
            {
                case E_TaskType.MainTask:
                    index = TaskDatas.MainTaskList.FindIndex(x => x.Id == taskInfo.Id);//根据值找到索引值
                    break;
                case E_TaskType.HuntingTask:
                    index = TaskDatas.HuntTaskList.FindIndex(x => x.Id == taskInfo.Id);//根据值找到索引值
                    break;
                case E_TaskType.ActivityTask:
                    index = TaskDatas.ActivityTaskList.FindIndex(x => x.Id == taskInfo.Id);//根据值找到索引值
                    break;
                case E_TaskType.EntrustTask:
                    index = TaskDatas.EntrustTaskList.FindIndex(x => x.Id == taskInfo.Id);//根据值找到索引值
                    break;

            }
            return index;
        }


        public void ClearRewar()
        {
            TaskDes.text = string.Empty;
            RequestText.text = string.Empty;
            RewardText.text = string.Empty;
            CurTaskInfo = null;
        }
        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            TaskCircularScrollView.Dispose();
            IsNpc = true;
            ClearActiveMapList();
        }
    }
}
