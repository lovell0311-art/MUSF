using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using System.Threading.Tasks;

namespace ETHotfix
{
    /// <summary>
    /// 三转，第三阶段
    /// </summary>
    [GameTaskRewardMethod]
    public class TaskReward_CareerChange_3_3 : IGameTaskRewardHandler
    {
        /// <summary>
        /// 可以给奖励
        /// </summary>
        /// <param name="gameTask"></param>
        public bool RewardsCanBeGiven(GameTask gameTask, Player ownPlayer, ItemsBoxStatus.LockList lockList, out int err)
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
            int oldLevel = gamePlayer.Data.OccupationLevel;
            int oldFreePoint = gamePlayer.Data.FreePoint;
            gamePlayer.Data.OccupationLevel = 3;
            gamePlayer.Data.FreePoint += 30;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)gamePlayer.Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)gamePlayer.Player.GameAreaId);
            mWriteDataComponent.Save(gamePlayer.Data, dBProxy).Coroutine();

            GameTasksHelper.NotifyAroundPlayer(ownPlayer, new G2C_CareerChangeComplete_notice()
            {
                GameUserId = gamePlayer.InstanceId,
                OccupationLevel = gamePlayer.Data.OccupationLevel,
            });

            // 推送 属性点变动
            G2C_ChangeValue_notice mChangeValueMessage = new G2C_ChangeValue_notice();
            mChangeValueMessage.GameUserId = gamePlayer.InstanceId;
            mChangeValueMessage.Info.Add(new G2C_BattleKVData()
            {
                Key = (int)E_GameProperty.FreePoint,
                Value = gamePlayer.Data.FreePoint
            });
            ownPlayer.Send(mChangeValueMessage);

            var mGamePlayer = ownPlayer.GetCustomComponent<GamePlayer>();

            if (!ConstServer.PlayerMaster)
                if (mGamePlayer.Data.Level > 400 && mGamePlayer.Data.OccupationLevel >= 3)
            {
                async Task master()
                {
                    // 初始化大师技能需要

                    var mPlayer = ownPlayer;
                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, mPlayer.GameAreaId);

                    DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
                    var mDataCache_Skill = mDataCacheManageComponent.Get<DBMasterData>();
                    if (mDataCache_Skill == null)
                    {
                        mDataCache_Skill = await HelpDb_DBMasterData.Init(mPlayer, mDataCacheManageComponent, dBProxy2);
                    }
                    var mDatalist_Skill = mDataCache_Skill.OnlyOne();
                    if (mDatalist_Skill == null)
                    {
                        mDatalist_Skill = new DBMasterData()
                        {
                            Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                            GameUserId = mPlayer.GameUserId,
                            PropertyPoint = mGamePlayer.Data.Level - 401
                        };

                        bool mSaveResult = await dBProxy2.Save(mDatalist_Skill);
                        if (mSaveResult == false)
                        {
                            Log.Error($"{mPlayer.GameUserId}大师初始化异常 2");
                            return;
                        }
                        mDataCache_Skill.DataAdd(mDatalist_Skill);
                    }
                    if (mDatalist_Skill.SkillId == null) mDatalist_Skill.SkillId = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(mDatalist_Skill.Skill);

                    mDatalist_Skill.PropertyPoint++;
                    var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mGamePlayer.Player.GameAreaId);
                    mWriteDataComponent.Save(mDatalist_Skill, dBProxy2).Coroutine();
                }
                master().Coroutine();
            }


            // 发布 GamePlayerCareerChangeComplete 事件
            ETModel.EventType.GamePlayerCareerChangeComplete.Instance.gamePlayer = gamePlayer;
            ETModel.EventType.GamePlayerCareerChangeComplete.Instance.oldOccupationLevel = oldLevel;
            ETModel.EventType.GamePlayerCareerChangeComplete.Instance.newOccupationLevel = gamePlayer.Data.OccupationLevel;
            Root.EventSystem.OnRun("GamePlayerCareerChangeComplete", ETModel.EventType.GamePlayerCareerChangeComplete.Instance);

            Log.PLog($"a:{ownPlayer.UserId} r:{ownPlayer.GameUserId} 完成三转 第三阶段 (三转完成) Level:{gamePlayer.Data.Level} FreePoint:{oldFreePoint} => {gamePlayer.Data.FreePoint}");
        }
    }
}
