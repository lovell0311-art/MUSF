using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    /// <summary>
    /// 一转，第一阶段
    /// </summary>
    [GameTaskRewardMethod]
    public class TaskReward_CareerChange_1_1 : IGameTaskRewardHandler
    {
        /// <summary>
        /// 可以给奖励
        /// </summary>
        /// <param name="gameTask"></param>
        public bool RewardsCanBeGiven(GameTask gameTask, Player ownPlayer, ItemsBoxStatus.LockList lockList,out int err)
        {
            err = 0;
            return true;
        }

        /// <summary>
        /// 发放奖励
        /// </summary>
        public void StartGivingRewards(GameTask gameTask, Player ownPlayer)
        {
            var gamePlayer = ownPlayer.GetCustomComponent<GamePlayer>();
            int oldFreePoint = gamePlayer.Data.FreePoint;
            gamePlayer.Data.FreePoint += 10;
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)gamePlayer.Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)gamePlayer.Player.GameAreaId);
            mWriteDataComponent.Save(gamePlayer.Data, dBProxy).Coroutine();

            // 推送 属性点变动
            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            mChangeValueMessage.GameUserId = gamePlayer.InstanceId;
            mChangeValueMessage.Info.Add(new G2C_BattleKVData()
            {
                Key = (int)E_GameProperty.FreePoint,
                Value = gamePlayer.Data.FreePoint
            });
            ownPlayer.Send(mChangeValueMessage);

            Log.PLog($"a:{ownPlayer.UserId} r:{ownPlayer.GameUserId} 完成一转 第一阶段 Level:{gamePlayer.Data.Level} FreePoint:{oldFreePoint} => {gamePlayer.Data.FreePoint}");
        }
    }
}
