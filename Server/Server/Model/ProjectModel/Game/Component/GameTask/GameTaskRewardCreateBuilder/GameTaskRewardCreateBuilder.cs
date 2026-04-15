using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class GameTaskRewardMethodAttribute : BaseAttribute
    {
        public GameTaskRewardMethodAttribute() { }

    }


    public interface IGameTaskRewardHandler
    {
        /// <summary>
        /// 可以给奖励
        /// </summary>
        /// <param name="gameTask"></param>
        public bool RewardsCanBeGiven(GameTask gameTask, Player ownPlayer,ItemsBoxStatus.LockList lockList,out int err);

        /// <summary>
        /// 发放奖励
        /// </summary>
        public void StartGivingRewards(GameTask gameTask, Player ownPlayer);
    }




    public class GameTaskRewardCreateBuilder : TCustomComponent<MainFactory>
    {
        public Dictionary<string, IGameTaskRewardHandler> GameTaskActionDict = new Dictionary<string, IGameTaskRewardHandler>();


        public override void Dispose()
        {
            if (IsDisposeable) return;

            GameTaskActionDict.Clear();

            base.Dispose();
        }
    }
}
