using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;


namespace ETHotfix
{
    [GameTaskActionMethod(EGameTaskActionType.EquipmentRelatedSettings)]
    public class GameTaskAction_EquipmentRelatedSettings : IGameTaskActionHandler
    {
        /// <summary>
        /// 初始化任务进度
        /// </summary>
        public void InitTaskProgress(GameTask gameTask, Player ownPlayer)
        {
            for(int i=0;i<gameTask.TaskProgress.Count;++i)
            {
                gameTask.TaskProgress[i] = 0;
            }
        }

        /// <summary>
        /// 尝试完成任务
        /// </summary>
        /// <param name="gameTask"></param>
        public void TryCompleteTask(GameTask gameTask)
        {
            // 检测任务是否完成
            bool isComplete = true;
            for (int i = 0; i < gameTask.Config.TaskTargetCount.Count; ++i)
            {
                if (gameTask.TaskProgress[i] < gameTask.Config.TaskTargetCount[i])
                {
                    isComplete = false;
                    break;
                }
            }
            if (!isComplete)
            {
                return;
            }

            // 设置任务状态为完成的
            gameTask.TaskState = EGameTaskState.Complete;
        }

        /// <summary>
        /// 领取奖励后
        /// </summary>
        /// <param name="gameTask"></param>
        public void AfterReceiveReward(GameTask gameTask, Player player)
        {

        }

    }


    [EventMethod("EquipmentRelatedSettings")]
    public class PassTask_EquipmentRelatedSettings : ITEventMethodOnRun<ETModel.EventType.EquipmentRelatedSettings>
    {
        public void OnRun(ETModel.EventType.EquipmentRelatedSettings args)
        {
            GameTasksComponent gameTasksCom;
            gameTasksCom = args.player.GetCustomComponent<GameTasksComponent>();

            if (gameTasksCom == null) return;
            using ListComponent<GameTask> gameTaskList = ListComponent<GameTask>.Create();
            List<GameTask> list = gameTaskList;
            gameTasksCom.GetTasksInProgressByActionType(ref list,EGameTaskActionType.EquipmentRelatedSettings);

            foreach (var gameTask in gameTaskList)
            {
                //强化
                if (gameTask.Config.TaskTargetId.Contains(-3))
                {
                    if (gameTask.Config.TaskTargetId[0] == -3 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        if(args.item != null)
                            gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, args.item.GetProp(EItemValue.Level));
                    }
                }
                //锻造
                //if (gameTask.Config.TaskTargetId.Contains(-4))
                //{
                //    if (gameTask.Config.TaskTargetId[0] == -4 &&
                //        gameTask.CanUpdateProgress(0))
                //    {
                //        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, args.item.GetProp(EItemValue.ForgeLevel));
                //    }
                //}
                //称号
                if (gameTask.Config.TaskTargetId.Contains(-7))
                {
                    if (gameTask.Config.TaskTargetId[0] == -7 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, args.TitleCount);
                    }
                }
                //继承
                if (gameTask.Config.TaskTargetId.Contains(-5))
                {
                    if (gameTask.Config.TaskTargetId[0] == -5 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
                    }
                }
                //宠物
                if (gameTask.Config.TaskTargetId.Contains(-8))
                {
                    if (gameTask.Config.TaskTargetId[0] == -8 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
                    }
                }
                //坐骑
                if (gameTask.Config.TaskTargetId.Contains(-9))
                {
                    if (gameTask.Config.TaskTargetId[0] == -9 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
                    }
                }
                //翅膀
                if (gameTask.Config.TaskTargetId.Contains(-10))
                {
                    if (gameTask.Config.TaskTargetId[0] == -10 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
                    }
                }
                //镶嵌
                if (gameTask.Config.TaskTargetId.Contains(-6))
                {
                    if (gameTask.Config.TaskTargetId[0] == -6 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
                    }
                }
                //藏宝阁交易
                if (gameTask.Config.TaskTargetId.Contains(-11))
                {
                    if (gameTask.Config.TaskTargetId[0] == -11 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, 1);
                    }
                }
                //强化加10
                if (gameTask.Config.TaskTargetId.Contains(-12))
                {
                    if (gameTask.Config.TaskTargetId[0] == -12 &&
                        gameTask.CanUpdateProgress(0))
                    {
                        gameTasksCom.TriggerTaskAction(gameTask.ConfigId, 0, args.ItemCount);
                    }
                }
            }
        }

    }



}
