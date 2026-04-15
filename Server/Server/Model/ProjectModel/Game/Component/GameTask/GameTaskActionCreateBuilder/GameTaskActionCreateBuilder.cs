using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class GameTaskActionMethodAttribute : BaseAttribute
    {
        public EGameTaskActionType ActionType;
        public GameTaskActionMethodAttribute(EGameTaskActionType actionType) { ActionType = actionType; }

    }


    public interface IGameTaskActionHandler
    {
        /// <summary>
        /// 初始化任务进度
        /// </summary>
        public void InitTaskProgress(GameTask gameTask, Player ownPlayer);

        /// <summary>
        /// 尝试完成任务
        /// </summary>
        /// <param name="gameTask"></param>
        public void TryCompleteTask(GameTask gameTask);

        /// <summary>
        /// 领取奖励后
        /// </summary>
        /// <param name="gameTask"></param>
        public void AfterReceiveReward(GameTask gameTask, Player ownPlayer);
    }




    public class GameTaskActionCreateBuilder : TCustomComponent<MainFactory>
    {
        public Dictionary<EGameTaskActionType, IGameTaskActionHandler> GameTaskActionDict = new Dictionary<EGameTaskActionType, IGameTaskActionHandler>();


        public override void Dispose()
        {
            if (IsDisposeable) return;

            GameTaskActionDict.Clear();

            base.Dispose();
        }
    }
}
