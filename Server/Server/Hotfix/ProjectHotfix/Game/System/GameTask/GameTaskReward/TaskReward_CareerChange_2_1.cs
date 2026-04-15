using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    /// <summary>
    /// 二转，第一阶段
    /// </summary>
    [GameTaskRewardMethod]
    public class TaskReward_CareerChange_2_1 : IGameTaskRewardHandler
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

            gamePlayer.Data.OccupationLevel = 2;
            int oldFreePoint = gamePlayer.Data.FreePoint;
            int addFreePoint = gamePlayer.Data.Level - 220;
            gamePlayer.Data.FreePoint += addFreePoint;

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

            // 发布 GamePlayerCareerChangeComplete 事件
            ETModel.EventType.GamePlayerCareerChangeComplete.Instance.gamePlayer = gamePlayer;
            ETModel.EventType.GamePlayerCareerChangeComplete.Instance.oldOccupationLevel = oldLevel;
            ETModel.EventType.GamePlayerCareerChangeComplete.Instance.newOccupationLevel = gamePlayer.Data.OccupationLevel;
            Root.EventSystem.OnRun("GamePlayerCareerChangeComplete", ETModel.EventType.GamePlayerCareerChangeComplete.Instance);

            Log.PLog($"a:{ownPlayer.UserId} r:{ownPlayer.GameUserId} 完成二转 第一阶段 (二转完成) Level:{gamePlayer.Data.Level} FreePoint:{oldFreePoint} => {gamePlayer.Data.FreePoint}");

            switch ((E_GameOccupation)gamePlayer.Data.PlayerTypeId)
            {
                case E_GameOccupation.None:
                    break;
                case E_GameOccupation.Spell:
                    {
                        // 开启骑士连击技能
                        int mStudySkillId = 27;

                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>().GetJson<Skill_SpellConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillJson))
                        {
                            if (gamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                            {
                                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                                gamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                            }

                            DataCacheManageComponent mDataCacheManageComponent = ownPlayer.GetCustomComponent<DataCacheManageComponent>();
                            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
                            var mDatalist = mDataCache.DataQuery(p => p.GameUserId == ownPlayer.GameUserId);
                            DBGameSkillData mData = mDatalist[0];

                            if (mData.SkillId.Contains(mStudySkillId) == false)
                            {
                                mData.SkillId.Add(mStudySkillId);

                                G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                                mSkillSingle.SkillId = mStudySkillId;
                                ownPlayer.Send(mSkillSingle);

                                mData.Serialize();

                                mWriteDataComponent.Save(mData, dBProxy).Coroutine();
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Swordsman:
                    break;
                case E_GameOccupation.Archer:
                    {
                        // 开启骑士连击技能
                        int mStudySkillId = 220;

                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>().GetJson<Skill_ArcherConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillJson))
                        {
                            if (gamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                            {
                                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                                gamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                            }


                            DataCacheManageComponent mDataCacheManageComponent = ownPlayer.GetCustomComponent<DataCacheManageComponent>();
                            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
                            var mDatalist = mDataCache.DataQuery(p => p.GameUserId == ownPlayer.GameUserId);
                            DBGameSkillData mData = mDatalist[0];

                            if (mData.SkillId.Contains(mStudySkillId) == false)
                            {
                                mData.SkillId.Add(mStudySkillId);

                                G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                                mSkillSingle.SkillId = mStudySkillId;
                                ownPlayer.Send(mSkillSingle);

                                mData.Serialize();

                                mWriteDataComponent.Save(mData, dBProxy).Coroutine();
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Spellsword:
                    {
                        // 开启骑士连击技能
                        int mStudySkillId = 329;

                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>().GetJson<Skill_SpellswordConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillJson))
                        {
                            if (gamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                            {
                                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                                gamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                            }

                            DataCacheManageComponent mDataCacheManageComponent = ownPlayer.GetCustomComponent<DataCacheManageComponent>();
                            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
                            var mDatalist = mDataCache.DataQuery(p => p.GameUserId == ownPlayer.GameUserId);
                            DBGameSkillData mData = mDatalist[0];

                            if (mData.SkillId.Contains(mStudySkillId) == false)
                            {
                                mData.SkillId.Add(mStudySkillId);

                                G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                                mSkillSingle.SkillId = mStudySkillId;
                                ownPlayer.Send(mSkillSingle);

                                mData.Serialize();

                                mWriteDataComponent.Save(mData, dBProxy).Coroutine();
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Holyteacher:
                    {
                        // 开启骑士连击技能
                        int mStudySkillId = 417;

                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>().GetJson<Skill_HolyteacherConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillJson))
                        {
                            if (gamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                            {
                                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                                gamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                            }

                            DataCacheManageComponent mDataCacheManageComponent = ownPlayer.GetCustomComponent<DataCacheManageComponent>();
                            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
                            var mDatalist = mDataCache.DataQuery(p => p.GameUserId == ownPlayer.GameUserId);
                            DBGameSkillData mData = mDatalist[0];

                            if (mData.SkillId.Contains(mStudySkillId) == false)
                            {
                                mData.SkillId.Add(mStudySkillId);

                                G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                                mSkillSingle.SkillId = mStudySkillId;
                                ownPlayer.Send(mSkillSingle);

                                mData.Serialize();

                                mWriteDataComponent.Save(mData, dBProxy).Coroutine();
                            }
                        }
                    }
                    break;
                case E_GameOccupation.SummonWarlock:
                    {
                        // 开启骑士连击技能
                        int mStudySkillId = 525;

                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<SkillCreateBuilder>();
                        var mJsonDic = Root.MainFactory.GetCustomComponent<CustomFrameWork.Component.ReadConfigComponent>().GetJson<Skill_SummonWarlockConfigJson>().JsonDic;
                        if (mJsonDic.TryGetValue(mStudySkillId, out var mSkillJson))
                        {
                            if (gamePlayer.SkillGroup.ContainsKey(mStudySkillId) == false)
                            {
                                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(mStudySkillId);

                                gamePlayer.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                            }

                            DataCacheManageComponent mDataCacheManageComponent = ownPlayer.GetCustomComponent<DataCacheManageComponent>();
                            var mDataCache = mDataCacheManageComponent.Get<DBGameSkillData>();
                            var mDatalist = mDataCache.DataQuery(p => p.GameUserId == ownPlayer.GameUserId);
                            DBGameSkillData mData = mDatalist[0];

                            if (mData.SkillId.Contains(mStudySkillId) == false)
                            {
                                mData.SkillId.Add(mStudySkillId);

                                G2C_StudySkillSingle_notice mSkillSingle = new G2C_StudySkillSingle_notice();
                                mSkillSingle.SkillId = mStudySkillId;
                                ownPlayer.Send(mSkillSingle);

                                mData.Serialize();

                                mWriteDataComponent.Save(mData, dBProxy).Coroutine();
                            }
                        }
                    }
                    break;
                case E_GameOccupation.Combat:
                    break;
                case E_GameOccupation.GrowLancer:
                    break;
                default:
                    break;
            }
        }
    }
}
