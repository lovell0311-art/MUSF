using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    /// <summary>
    /// 二转，第二阶段
    /// </summary>
    [GameTaskRewardMethod]
    public class TaskReward_CareerChange_2_2 : IGameTaskRewardHandler
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
            if (gamePlayer.Data.PlayerTypeId == (int)E_GameOccupation.Swordsman)
            {
                // 开启骑士连击技能
                int mStudySkillId = 122;

                DataCacheManageComponent mDataCacheManageComponent = ownPlayer.GetCustomComponent<DataCacheManageComponent>();
                var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
                var mDatalist = mDataCache.DataQuery(p => p.GameUserId == ownPlayer.GameUserId);
                DBGameSkillData mData = mDatalist[0];

                {
                    var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                    var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Skill_SwordsmanConfigJson>().JsonDic;
                    if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillJson))
                    {
                        if (gamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                        {
                            var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                            gamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                        }
                    }
                }

                mData.SkillId.Add(mStudySkillId);

                G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                mSkillSingle.SkillId = mStudySkillId;
                ownPlayer.Send(mSkillSingle);

                mData.Serialize();

                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManagerComponent.GetZoneDB(DBType.Core, (int)ownPlayer.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(ownPlayer.GameAreaId);
                mWriteDataComponent.Save(mData, dBProxy2).Coroutine();

                Log.PLog($"a:{ownPlayer.UserId} r:{ownPlayer.GameUserId} 完成二转 第二阶段 (骑士连击技能开启) Level:{gamePlayer.Data.Level}");
            }
        }
    }
}
