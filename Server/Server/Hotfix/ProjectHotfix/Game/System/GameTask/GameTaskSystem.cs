using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;


namespace ETHotfix
{
    public static class GameTaskSystem
    {
        public static void UpdateProgress(this GameTask self,int progressId,int value)
        {
            Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<GameTask_ActionConfigJson>().JsonDic.TryGetValue((int)self.Config.TaskActionType, out var conf);

            if ((EGameTaskProgressType)conf.TaskProgressType == EGameTaskProgressType.Add)
            {
                self.TaskProgress[progressId] += value;
            }
            else if((EGameTaskProgressType)conf.TaskProgressType == EGameTaskProgressType.Sub)
            {
                self.TaskProgress[progressId] -= value;
            }
            else if((EGameTaskProgressType)conf.TaskProgressType == EGameTaskProgressType.Update)
            {
                self.TaskProgress[progressId] = value;
            }
            if(self.Config.TaskTargetCount[progressId] < self.TaskProgress[progressId])
            {
                self.TaskProgress[progressId] = self.Config.TaskTargetCount[progressId];
            }
        }

        public static bool CanUpdateProgress(this GameTask self,int progressId)
        {
            return self.Config.TaskTargetCount[progressId] > self.TaskProgress[progressId];
        }

        /// <summary>
        /// 初始化任务进度
        /// </summary>
        /// <param name="self"></param>
        public static void InitProgress(this GameTask self, Player player)
        {
            Root.MainFactory.GetCustomComponent<GameTaskActionCreateBuilder>().GetHandlerByGameTask(self).InitTaskProgress(self, player);
        }

        public static void TryCompleteTask(this GameTask self)
        {
            if (self.TaskState != EGameTaskState.Doing)
            {
                return;
            }
            Root.MainFactory.GetCustomComponent<GameTaskActionCreateBuilder>().GetHandlerByGameTask(self).TryCompleteTask(self);

            if(self.TaskState == EGameTaskState.Complete)
            {
                Log.Info($"[任务] 任务完成。 taskId = {self.ConfigId}");
            }
        }

        /// <summary>
        /// 领取奖励后
        /// </summary>
        /// <param name="self"></param>
        public static void AfterReceiveReward(this GameTask self, Player player)
        {
            Root.MainFactory.GetCustomComponent<GameTaskActionCreateBuilder>().GetHandlerByGameTask(self).AfterReceiveReward(self, player);
        }




        public static Struct_TaskInfo ToMessage(this GameTask self)
        {
            self.Struct_TaskInfoProto.Progress.Clear();
            self.Struct_TaskInfoProto.Id = self.ConfigId;
            self.Struct_TaskInfoProto.State = (int)self.TaskState;
            self.Struct_TaskInfoProto.Progress.AddRange(self.TaskProgress);
            self.Struct_TaskInfoProto.StartTask = self.StartTask;
            return self.Struct_TaskInfoProto;
        }

        /// <summary>
        /// 通知任务信息变动
        /// </summary>
        /// <param name="self"></param>
        /// <param name="player"></param>
        public static void NotifyChanged(this GameTask self,Player player)
        {
            GameTask.G2C_UpdateGameTaskProto.TaskInfo = self.ToMessage();
            player.Send(GameTask.G2C_UpdateGameTaskProto);
        }
    }
}
