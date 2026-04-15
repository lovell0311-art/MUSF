using ETModel;
using ILRuntime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ETHotfix
{

    public enum E_TaskType
    {
        MainTask = 1,
        HuntingTask,
        ActivityTask,
        EntrustTask,
        CareerChangeTask,
        ActivityMap,
        /// <summary>
        /// 通行证
        /// </summary>
        Passport
    }
    public enum E_TaskStatus
    {
        NotGitTask,//未领取任务
        AlreadyGitTask,//已经领取任务
        CompleteTask,//完成任务 领取奖励
        None
    }

    public enum E_TaskActionType
    {
        /// <summary>
        /// 击杀怪物  [1,...] [1,...]
        /// </summary>
        KillMonster = 1,
        /// <summary>
        /// 提升等级    [0] [1]
        /// </summary>
        LevelUp = 2,
        /// <summary>
        /// 进入传送点   [1] [1]
        /// </summary>
        EnterTransferPoint = 3,
        /// <summary>
        /// 收集物品    [1,...] [1,...]
        /// </summary>
        CollectItem = 4,
        /// <summary>
        /// 组队击杀怪物   [1,...] [1,...]
        /// </summary>
        TeamKillMonster = 5,
        /// <summary>
        /// 自动完成    [0] [1]
        /// </summary>
        AutoComplete = 6,
        /// <summary>
        /// 击杀指定地图中的怪物    [0] [1]
        /// </summary>
        KillMonsterInMap = 7,
        /// <summary>
        /// 完成1次血色城堡
        /// </summary>
        CompleteRedCopy = 8,
        /// <summary>
        /// 完成1次恶魔广场
        /// </summary>
        CompleteDemonCopy = 9,
        /// <summary>
        /// 破坏血色城堡城门
        /// </summary>
        BreakThroughTheGate = 10,
        /// <summary>
        /// 转职完成   [0] [1-3]
        /// </summary>
        CareerChangeCompleted = 11,
        /// <summary>
        /// 在恶魔广场获取点数
        /// </summary>
        DemonPlazaGainsPoints = 12,
        /// <summary>
        /// 收集并提交物品 [1,...] [1,...]
        /// </summary>
        CollectAndSubmitItem = 14,
        /// <summary>
        /// 有翅膀(判断背包，装备栏有没有翅膀或披风) [0] [1]
        /// </summary>
        HaveWing = 15,
        /// <summary>
        /// 丢弃物品    [0] [1]
        /// </summary>
        DiscardItem = 100,
        /// <summary>
        /// 属性加点
        /// </summary>
        ConfigureAttributePoint = 101,
        /// <summary>
        /// 穿戴装备
        /// </summary>
        WearEquip = 102,
        /// <summary>
        /// 设置技能
        /// </summary>
        SetSkill = 103,
        /// <summary>
        /// 使用传送
        /// </summary>
        UseTransfer = 104,
        /// <summary>
        /// 领取狩猎任务
        /// </summary>
        ReceiveHuntingTask = 105,
        /// <summary>
        /// 领取新手buff
        /// </summary>
        ReceiveXinShouBuff = 106,
    }

    /// <summary>
    ///任务数据
    /// </summary>
    public static class TaskDatas
    {
        public static List<TaskInfo> MainTaskList = new List<TaskInfo>();///主线任务
        public static List<TaskInfo> ActivityTaskList = new List<TaskInfo>();///活动任务
        public static List<TaskInfo> HuntTaskList = new List<TaskInfo>();///狩猎任务
        public static List<TaskInfo> EntrustTaskList = new List<TaskInfo>();///委托任务
        public static List<TaskInfo> CareerChangeTaskList = new List<TaskInfo>();///转职任务
        public static List<TaskInfo> PassportTaskList = new List<TaskInfo>();///通行证

        public static Dictionary<int, StringBuilder> TaskRewardDic = new Dictionary<int, StringBuilder>();//任务奖励字典

        public static Action AutoNavCallBack;

        public static TaskInfo MainTaskInfo, ActivityTaskInfo, HuntTaskInfo, EntrustTaskInfo, ChangeTaskInfo, PassportTaskInfo328, PassportTaskInfo688;
        public static ActiveMapInfo activeMapInfo;

        public static int CurTaskMapId;
        /// <summary>
        /// 初始化所有任务
        /// </summary>
        public static void GetAllTask()
        {

            GetAllMainTask();
            GetAllAcitivityTask();
            GetAllHuntTask();
            GetAllCareerChangeTask();
            GetAllEntrustTask();
            GetTaskRewardItems();
            GetPassportTasks();
        }
        /// <summary>
        /// 获取下一个任务
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static TaskInfo GetNextTaskInfo(this TaskInfo self)
        {
            var curtask = GetCurTaskInfo(self.Id);
            //Log.DebugGreen($"{curtask.Id}->下一个任务ID:{curtask.NextTaskId}");
            var task = GetCurTaskInfo(curtask.NextTaskId);
            if (task == null) return self;
            else return task;
        }

        /// <summary>
        /// 获取当前正在进行的任务
        /// </summary>
        /// <param name="taskType"></param>
        /// <returns></returns>
        public static TaskInfo GetCurTaskInfo(E_TaskType taskType)
        {
            switch (taskType)
            {
                case E_TaskType.MainTask:
                    return MainTaskInfo;
                case E_TaskType.HuntingTask:
                    //if (HuntTaskInfo == null)
                    //    200000.ToInt64().GetTaskInfo_Out(out HuntTaskInfo);
                    return HuntTaskInfo;
                case E_TaskType.ActivityTask:
                    if (ActivityTaskInfo == null)
                        300000.ToInt64().GetTaskInfo_Out(out ActivityTaskInfo);
                    return ActivityTaskInfo;
                case E_TaskType.EntrustTask:
                    if (EntrustTaskInfo == null)
                        400000.ToInt64().GetTaskInfo_Out(out EntrustTaskInfo);
                    return EntrustTaskInfo;
                case E_TaskType.CareerChangeTask:
                    if (ChangeTaskInfo == null)
                    {
                        switch (UnitEntityComponent.Instance.LocalRole.RoleType)
                        {
                            case E_RoleType.Swordsman:
                                500000.ToInt64().GetTaskInfo_Out(out ChangeTaskInfo);
                                break;

                            case E_RoleType.Archer:
                            case E_RoleType.Magician:
                            case E_RoleType.Summoner:
                                501000.ToInt64().GetTaskInfo_Out(out ChangeTaskInfo);
                                break;
                            case E_RoleType.Magicswordsman:
                            case E_RoleType.Holymentor:
                            case E_RoleType.Gladiator:
                            case E_RoleType.GrowLancer:
                                502000.ToInt64().GetTaskInfo_Out(out ChangeTaskInfo);
                                break;

                            case E_RoleType.Runemage:
                                break;
                            case E_RoleType.StrongWind:
                                break;
                            case E_RoleType.Gunners:
                                break;
                            case E_RoleType.WhiteMagician:
                                break;
                            case E_RoleType.WomanMagician:
                                break;

                        }

                    }
                    return ChangeTaskInfo;
                case E_TaskType.ActivityMap:
                    if (activeMapInfo == null)
                        600000.ToInt64().GetTaskInfo_Out(out activeMapInfo);
                    break;
                default:
                    break;
            }
            return null;
        }
        /// <summary>
        /// 当前任务是否完成
        /// </summary>
        /// <param name="self"></param>
        /// <returns>true 完成 false 未完成</returns>
        public static bool CurTaskIsComplete(this TaskInfo self)
        {
            E_TaskType taskType = (E_TaskType)(self.Id / 100000);

            switch (taskType)
            {
                case E_TaskType.MainTask:
                    if (MainTaskInfo == null)
                        return false;
                    if (self.Id < MainTaskInfo.Id)
                    {
                        self.IsReceiveRewards = true;
                        return true;
                    }
                    return false;
                case E_TaskType.HuntingTask:
                    if (HuntTaskInfo == null)
                        return false;
                    if (self.Id < HuntTaskInfo.Id)
                    {
                        self.IsReceiveRewards = true;
                        return true;
                    }
                    return false;
                case E_TaskType.ActivityTask:
                    if (ActivityTaskInfo == null)
                        return false;
                    if (self.Id < ActivityTaskInfo.Id)
                    {
                        self.IsReceiveRewards = true;
                        return true;
                    }
                    return false;
                case E_TaskType.EntrustTask:
                    if (EntrustTaskInfo == null)
                        return false;

                    Log.DebugWhtie($"self.State==3:{self.State == 3}");
                    if (self.Id < EntrustTaskInfo.Id || (self.Id == EntrustTaskInfo.Id && (EntrustTaskInfo.State == 3 || EntrustTaskInfo.State == 2)))
                    {
                        if (self.Id < EntrustTaskInfo.Id)
                            self.IsReceiveRewards = true;
                        else if (self.Id == EntrustTaskInfo.Id)
                        {
                            self.IsReceiveRewards = EntrustTaskInfo.State == 3;
                        }
                        return true;
                    }
                    return false;
                case E_TaskType.CareerChangeTask:
                    if (ChangeTaskInfo == null)
                        return false;
                    if (self.Id < ChangeTaskInfo.Id)
                    {
                        self.IsReceiveRewards = true;
                        return true;
                    }
                    return false;
            }
            return false;
        }

        /// <summary>
        /// 更具任务ID 获取当前任务信息
        /// </summary>
        /// <param name="taskId"></param>
        /// <returns></returns>
        public static TaskInfo GetCurTaskInfo(long taskId)
        {
            Log.DebugBrown("更新的任务id" + taskId);
            E_TaskType taskType = (E_TaskType)(taskId / 100000);

            List<TaskInfo> taskList = new List<TaskInfo>();
            switch (taskType)
            {
                case E_TaskType.MainTask:
                    taskList = MainTaskList;
                    break;
                case E_TaskType.HuntingTask:
                    taskList = HuntTaskList;
                    break;
                case E_TaskType.ActivityTask:
                    taskList = ActivityTaskList;
                    break;
                case E_TaskType.EntrustTask:
                    taskList = EntrustTaskList;
                    break;
                case E_TaskType.CareerChangeTask:
                    taskList = CareerChangeTaskList;
                    break;
                case E_TaskType.Passport:
                    taskList = PassportTaskList;
                    break;
            }
            return GetTaskInfo();

            ///获取当前任务
            TaskInfo GetTaskInfo()
            {
                //for (int i = 0; i < taskList.Count; i++)
                //{
                //    if (taskList[i].Id== taskId)
                //    {
                //        Log.DebugBrown("数据" + taskList[i].Id);
                //        return taskList[i];
                //    }
                //}
                //return null;
                return taskList.Find(r => r.Id == taskId);
                //if (taskList.Exists(r => r.Id == taskId))
                //{

                //    return taskList.Find(r => r.Id == taskId);
                //}
                //return null;
            }
        }
        /// <summary>
        /// 获取任务奖励物品
        /// </summary>
        public static void GetTaskRewardItems()
        {
            TaskRewardDic.Clear();
            var allReWardConfigs = ConfigComponent.Instance.GetAll<GameTask_RewardItemConfig>();
            for (int i = 0, length = allReWardConfigs.Length; i < length; i++)
            {
                GameTask_RewardItemConfig rewardItemConfig = (GameTask_RewardItemConfig)allReWardConfigs[i];
                if (TaskRewardDic.ContainsKey(rewardItemConfig.TaskId) == false)
                    TaskRewardDic[rewardItemConfig.TaskId] = new StringBuilder().Append($"{rewardItemConfig.Remark}、");
                else
                    TaskRewardDic[rewardItemConfig.TaskId].Append($"{rewardItemConfig.Remark}、");
            }
        }

        public static void GetPassportTasks()
        {
            PassportTaskList.Clear();
            var configs = ConfigComponent.Instance.GetAll<PassportTask_PassportConfig>();
            for (int i = 0, length = configs.Length; i < length; i++)
            {
                PassportTask_PassportConfig task = (PassportTask_PassportConfig)configs[i];
                task.Id.GetTaskInfo_Out(out TaskInfo taskInfo);

                //设置下一个任务
                for (int t = 0; t < task.TaskBeforeId.Count; t++)
                {
                    if (MainTaskList.Find(r => r.Id == task.TaskBeforeId[t]) is TaskInfo info)
                    {
                        info.NextTaskId = task.Id;
                    }
                }

                PassportTaskList.Add(taskInfo);
            }
        }

        /// <summary>
        /// 初始化当前 任务
        /// </summary>
        /// <param name="taskId"></param>
        /// <param name="state"></param>
        /// <param name="taskProgress"></param>
        public static void InitCurTask(int taskId, int state, List<int> taskProgress)
        {
            E_TaskType taskType = (E_TaskType)(taskId / 100000);

            //taskId.ToInt64().GetTaskInfo_Out(out TaskInfo taskInfo);
            TaskInfo taskInfo = GetTaskInfo(taskType, taskId);
            if (taskInfo == null)
            {
                Log.Info("未查找到任务 " + taskType + "  " + taskId);
                return;
            }



            taskInfo.State = state;
            taskInfo.TaskProgress = taskProgress;
            ChangeTaskStatus(taskType, taskInfo);
          //  Log.DebugBrown("任务进度" + taskInfo.ReqCoin + "::进度" + taskInfo.TaskProgress[0] + "count" + taskInfo.TaskProgress.Count+"目标数量"+taskInfo.TaskTargetCount[0]);
            switch (taskType)
            {
                case E_TaskType.MainTask:
                    MainTaskInfo = taskInfo;
                    break;
                case E_TaskType.HuntingTask:

                    HuntTaskInfo = taskInfo;
                    break;
                case E_TaskType.ActivityTask:

                    ActivityTaskInfo = taskInfo;
                    break;
                case E_TaskType.EntrustTask:

                    EntrustTaskInfo = taskInfo;
                    break;
                case E_TaskType.CareerChangeTask:
                    ChangeTaskInfo = taskInfo;
                    break;
                case E_TaskType.Passport:
                    {
                        if (taskId / 1000 == 700)
                        {
                            PassportTaskInfo328 = taskInfo;
                        }
                        else
                        {
                            PassportTaskInfo688 = taskInfo;
                        }
                        break;
                    }
            }

            {
                UIMainComponent.Instance?.SetTaskInfo(taskInfo);
            }
        }
        /// 获取所有主线任务                                                                       
        public static void GetAllMainTask()
        {
            MainTaskList.Clear();
            var allMainTask = ConfigComponent.Instance.GetAll<GameTask_MainConfig>();
            for (int i = 0, length = allMainTask.Length; i < length; i++)
            {
                GameTask_MainConfig task = (GameTask_MainConfig)allMainTask[i];
                task.Id.GetTaskInfo_Out(out TaskInfo taskInfo);
                if (taskInfo.IsMyTask())
                {
                    //设置下一个任务
                    for (int t = 0; t < task.TaskBeforeId.Count; t++)
                    {
                        if (MainTaskList.Find(r => r.Id == task.TaskBeforeId[t]) is TaskInfo info)
                        {
                            info.NextTaskId = task.Id;
                        }
                    }

                    MainTaskList.Add(taskInfo);
                }
            }

        }
        /// <summary>
        /// 获取所有转职任务
        /// </summary>
        public static void GetAllCareerChangeTask()
        {
            CareerChangeTaskList.Clear();
            var allMainTask = ConfigComponent.Instance.GetAll<GameTask_CareerChangeConfig>();
            for (int i = 0, length = allMainTask.Length; i < length; i++)
            {
                GameTask_CareerChangeConfig task = (GameTask_CareerChangeConfig)allMainTask[i];
                task.Id.GetTaskInfo_Out(out TaskInfo taskInfo);
                if (taskInfo.IsMyTask())
                {
                    //设置下一个任务
                    for (int t = 0; t < taskInfo.TaskBeforeId.Count; t++)
                    {
                        if (CareerChangeTaskList.Find(r => r.Id == taskInfo.TaskBeforeId[t]) is TaskInfo info)
                        {
                            info.NextTaskId = taskInfo.Id;
                        }
                    }
                    CareerChangeTaskList.Add(taskInfo);
                }
            }
        }
        /// <summary>
        /// 获取所有狩猎任务
        /// </summary>
        public static void GetAllHuntTask()
        {
            HuntTaskList.Clear();
            var allMainTask = ConfigComponent.Instance.GetAll<GameTask_HuntingConfig>();
            for (int i = 0, length = allMainTask.Length; i < length; i++)
            {
                GameTask_HuntingConfig task = (GameTask_HuntingConfig)allMainTask[i];
                task.Id.GetTaskInfo_Out(out TaskInfo taskInfo);
                if (taskInfo.IsMyTask())
                {
                    //设置下一个任务
                    taskInfo.NextTaskId = task.Id + 1;
                    HuntTaskList.Add(taskInfo);
                }
            }
        }
        /// <summary>
        /// 获取所有活动任务
        /// </summary>
        public static void GetAllAcitivityTask()
        {
            ActivityTaskList.Clear();
            var allMainTask = ConfigComponent.Instance.GetAll<GameTask_ActivityConfig>();
            for (int i = 0, length = allMainTask.Length; i < length; i++)
            {
                GameTask_ActivityConfig task = (GameTask_ActivityConfig)allMainTask[i];
                task.Id.GetTaskInfo_Out(out TaskInfo taskInfo);
                if (taskInfo.IsMyTask())
                {
                    //设置下一个任务
                    for (int t = 0; t < taskInfo.TaskBeforeId.Count; t++)
                    {
                        if (ActivityTaskList.Find(r => r.Id == taskInfo.TaskBeforeId[t]) is TaskInfo info)
                        {
                            info.NextTaskId = taskInfo.Id;
                        }
                    }
                    ActivityTaskList.Add(taskInfo);
                }
            }
        }
        /// <summary>
        /// 获取所有委托任务
        /// </summary>
        public static void GetAllEntrustTask()
        {
            EntrustTaskList.Clear();
            var allMainTask = ConfigComponent.Instance.GetAll<GameTask_EntrustConfig>();
            for (int i = 0, length = allMainTask.Length; i < length; i++)
            {
                GameTask_EntrustConfig task = (GameTask_EntrustConfig)allMainTask[i];
                task.Id.GetTaskInfo_Out(out TaskInfo taskInfo);
                if (taskInfo.IsMyTask())
                {
                    //设置下一个任务
                    for (int t = 0; t < taskInfo.TaskBeforeId.Count; t++)
                    {
                        if (EntrustTaskList.Find(r => r.Id == taskInfo.TaskBeforeId[t]) is TaskInfo info)
                        {
                            info.NextTaskId = taskInfo.Id;
                        }
                    }
                    EntrustTaskList.Add(taskInfo);
                }
            }
        }
        /// <summary>
        /// 是否可以寻路到目标点
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        public static void Nav2Target(this TaskInfo taskInfo)
        {

            Map_TransferPointConfig map_Transfer = ConfigComponent.Instance.GetItem<Map_TransferPointConfig>(taskInfo.MapId);
            if (map_Transfer == null)
            {
                return;
            }

            SceneName sceneName = (SceneName)map_Transfer.MapId;

            if (sceneName.ToString() != SceneComponent.Instance.CurrentSceneName)
            {
                ChangeScene(map_Transfer.Id.ToInt32()).Coroutine();
            }
            else
            {
                if (sceneName.ToString() == SceneName.ShiLuoZhiTa.ToString() || sceneName.ToString() == SceneName.DiXiaCheng.ToString())
                {
                    if (AstarComponent.Instance.GetNode(taskInfo.Pos_X, taskInfo.Pos_Y).isWalkable == false)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, "目标点为不可行走区域");
                        return;
                    }
                    AstarComponent.Instance.FindPath(UnitEntityComponent.Instance.LocalRole.CurrentNodePos, AstarComponent.Instance.GetNode(taskInfo.Pos_X, taskInfo.Pos_Y), AstarNavCallback);

                    void AstarNavCallback(List<AstarNode> nodes)
                    {

                        if (nodes.Count == 0)
                        {

                            ChangeScene(map_Transfer.Id.ToInt32()).Coroutine();
                        }
                        else
                        {
                            //直接移动到 任务地点
                            UnitEntityComponent.Instance.LocalRole.GetComponent<UnitEntityPathComponent>().NavTarget(AstarComponent.Instance.GetNode(taskInfo.Pos_X, taskInfo.Pos_Y));
                        }
                    }
                }
                else
                {
                    //直接移动到 任务地点
                    UnitEntityComponent.Instance.LocalRole.GetComponent<UnitEntityPathComponent>().NavTarget(AstarComponent.Instance.GetNode(taskInfo.Pos_X, taskInfo.Pos_Y), NavTargetFinsh);
                }

            }
            void NavTargetFinsh()
            {
                UIMainComponent.Instance.HookTog.isOn = (taskInfo.IsAutoHitMonster == 1);

                //Log.Info("移动到目标点  1" );
                //已经移动到目标点----//查询当前是否存在引导

                GuideComponent.Instance.CheckIsShowGuide(true);

                ////查询当前是否存在任务
                //if (BeginnerGuideData.IsComplete(3))
                //{
                //    BeginnerGuideData.SetBeginnerGuide(3);
                //    UIMainComponent.Instance.SetBeginnerGuide();
                //}
                //else if (BeginnerGuideData.IsComplete(29))
                //{
                //    BeginnerGuideData.SetBeginnerGuide(29);
                //    UIMainComponent.Instance.SetBeginnerGuide();
                //}else if (BeginnerGuideData.IsComplete(38))
                //{
                //    BeginnerGuideData.SetBeginnerGuide(38);
                //    UIMainComponent.Instance.SetBeginnerGuide();
                //}
            }
            ///切换到任务场景
            async ETVoid ChangeScene(int mapId)
            {
                G2C_MapDeliveryResponse response = (G2C_MapDeliveryResponse)await SessionComponent.Instance.Session.Call(new C2G_MapDeliveryRequest
                {
                    MapId = mapId
                });
                if (response.Error != 0)
                {
                    Log.DebugGreen($"传送异常：{response.Error}");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
                }
                else
                {

                    //传送完成 移动到目标点
                    AutoNavCallBack = () =>
                    {
                        UnitEntityComponent.Instance.LocalRole.GetComponent<UnitEntityPathComponent>().NavTarget(AstarComponent.Instance.GetNode(taskInfo.Pos_X, taskInfo.Pos_Y), () => AutoNavCallBack = null);
                    };
                }
            }
        }


        /// <summary>
        /// 是否是玩家的任务
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        static bool IsMyTask(this TaskInfo taskInfo) => (UnitEntityComponent.Instance.LocalRole.RoleType) switch
        {
            E_RoleType.Magician => taskInfo.Spell != -1,
            E_RoleType.Swordsman => taskInfo.Swordsman != -1,
            E_RoleType.Archer => taskInfo.Archer != -1,
            E_RoleType.Magicswordsman => taskInfo.Spellsword != -1,
            E_RoleType.Holymentor => taskInfo.Holyteacher != -1,
            E_RoleType.Summoner => taskInfo.SummonWarlock != -1,
            E_RoleType.Gladiator => taskInfo.Combat != -1,
            E_RoleType.GrowLancer => taskInfo.GrowLancer != -1,
            _ => false
        };
        /// <summary>
        /// 等级是否满足
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns></returns>
        public static bool CheckLev(this TaskInfo taskInfo)
        {
            return !(UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.Level) < taskInfo.ReqLevelMin || UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.Level) > taskInfo.ReqLevelMax);
        }
        /// <summary>
        /// 检查前一个 任务是否完成
        /// </summary>
        /// <param name="taskInfo"></param>
        /// <returns>true 任务完成 false 未完成</returns>
        public static bool CheckTaskBeforeIsComplete(this TaskInfo taskInfo, List<TaskInfo> taskInfos)
        {
            bool iscomplete = false;
            if (taskInfo.TaskBeforeId.Count == 0) return true;//0 没有前置任务
                                                              // if (taskInfos.Exists(r => r.Id == taskInfo.TaskBeforeId))
            {
                //var task = taskInfos.Find(r => r.Id == taskInfo.TaskBeforeId);
                // iscomplete = task.IsComplete;
            }
            return iscomplete;
        }
        /// <summary>
        /// 改变任务 进度
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="taskInfo"></param>
        public static void ChangeTaskStatus(E_TaskType taskType, TaskInfo taskInfo)
        {
            List<TaskInfo> taskInfos = new List<TaskInfo>();
            switch (taskType)
            {
                case E_TaskType.MainTask:
                    taskInfos = MainTaskList;
                    break;
                case E_TaskType.ActivityTask:
                    taskInfos = ActivityTaskList;
                    break;
                case E_TaskType.HuntingTask:
                    taskInfos = HuntTaskList;
                    break;
                case E_TaskType.EntrustTask:
                    taskInfos = EntrustTaskList;
                    break;
                case E_TaskType.CareerChangeTask:
                    taskInfos = CareerChangeTaskList;
                    break;
                default:
                    break;
            }
            if (taskInfos.Exists(r => r.Id == taskInfo.Id))
            {
                var task = taskInfos.Find(r => r.Id == taskInfo.Id);
                task.State = taskInfo.State;
                if (taskInfo.State == 3)
                {
                    task.IsReceiveRewards = true;
                }
            }
        }

        public static TaskInfo GetTaskInfo(E_TaskType taskType, int taskId)
        {
            switch (taskType)
            {
                case E_TaskType.MainTask:
                    {
                        return MainTaskList.Find(p => p.Id == taskId);
                    }
                case E_TaskType.ActivityTask:
                    {
                        return ActivityTaskList.Find(p => p.Id == taskId);
                    }
                case E_TaskType.HuntingTask:
                    {
                        return HuntTaskList.Find(p => p.Id == taskId);
                    }
                case E_TaskType.EntrustTask:
                    {
                        return EntrustTaskList.Find(p => p.Id == taskId);
                    }
                case E_TaskType.CareerChangeTask:
                    {
                        return CareerChangeTaskList.Find(p => p.Id == taskId);
                    }
                case E_TaskType.Passport:
                    {
                        Log.Info("---------------- " + PassportTaskList.Count + "   " + taskId);
                        return PassportTaskList.Find(p => p.Id == taskId);
                    }
            }

            return null;
        }
        /// <summary>
        /// 获取主线任务
        /// </summary>
        /// <param name="taskInfo"></param>
        public static void GitMainTask(out TaskInfo taskInfo)
        {
            taskInfo = new TaskInfo();
            var task = MainTaskList.First();
            if (task.CheckLev() && task.CheckTaskBeforeIsComplete(MainTaskList))
            {
                taskInfo = task;
            }
        }

        /// <summary>
        /// 清理任务信息
        /// </summary>
        public static void ClearTask()
        {
            MainTaskInfo = null;
            ActivityTaskInfo = null;
            HuntTaskInfo = null;
            EntrustTaskInfo = null;
            ChangeTaskInfo = null;
            PassportTaskInfo328 = null;

        }
    }
}