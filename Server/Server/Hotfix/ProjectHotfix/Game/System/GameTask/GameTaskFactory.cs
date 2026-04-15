using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    public static class GameTaskFactory
    {
        /// <summary>
        /// 创建任务
        /// </summary>
        /// <param name="info">游戏任务信息</param>
        /// <returns></returns>
        public static GameTask Create(GameTaskConfig info)
        {
            var gameTask = new GameTask();
            gameTask.ConfigId = info.ConfigId;
            gameTask.Config = info;
            gameTask.TaskProgress = new List<int>();
            for (int i = 0; i < gameTask.Config.TaskTargetCount.Count;++i)
            {
                gameTask.TaskProgress.Add(0);
            }

            return gameTask;
        }



        public static GameTask Create(int configId)
        {
            if (Root.MainFactory.GetCustomComponent<GameTaskConfigManager>().TryGetConfig(configId,out var conf))
            {
                return Create(conf);
            }
            return null;
        }



    }
}
