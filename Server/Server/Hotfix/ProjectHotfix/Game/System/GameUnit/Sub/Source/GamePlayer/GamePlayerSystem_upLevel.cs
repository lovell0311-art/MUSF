using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using Org.BouncyCastle.Asn1.Ocsp;

namespace ETHotfix
{



    public static partial class GamePlayerSystem
    {
        public static void AddExprienceMaxRate(this GamePlayer b_Component, int b_AddExprienceMaxRate)
        {
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mRoleExprienceDic = mReadConfigComponent.GetJson<Role_ExperienceConfigJson>().JsonDic;
            if (mRoleExprienceDic.TryGetValue(b_Component.Data.Level, out var mExperienceConfig))
            {
                var oldExp = b_Component.Data.Exp;
                b_Component.Data.Exp += (int)(mExperienceConfig.Exprience * b_AddExprienceMaxRate * 0.01f);
                if (b_Component.Data.Exp < 0) b_Component.Data.Exp = 0;
                oldExp = b_Component.Data.Exp - oldExp;

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.Player.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)b_Component.Player.GameAreaId);
                mWriteDataComponent.Save(b_Component.Data, dBProxy).Coroutine();

                G2C_ChangeValue_notice mExprienceNotice = new G2C_ChangeValue_notice();
                mExprienceNotice.GameUserId = b_Component.InstanceId;

                G2C_BattleKVData mExpMessage = new G2C_BattleKVData();
                mExpMessage.Key = (int)E_GameProperty.ExprienceDrop;
                mExpMessage.Value = oldExp;
                mExprienceNotice.Info.Add(mExpMessage);
                mExpMessage = new G2C_BattleKVData();
                mExpMessage.Key = (int)E_GameProperty.Exprience;
                mExpMessage.Value = b_Component.Data.Exp;
                mExprienceNotice.Info.Add(mExpMessage);

                b_Component.Player.Send(mExprienceNotice);
            }
        }

        /// <summary>
        /// 获取经验
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="b_AddExprience"></param>
        public static bool AddExprience(this GamePlayer b_Component, int b_AddExprience)
        {
            long mLibraryExprience = b_AddExprience;
            int levelLimit = 800;
            bool mUpLevel = false;
            int oldLevel = b_Component.Data.Level;
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mRoleExprienceDic = mReadConfigComponent.GetJson<Role_ExperienceConfigJson>().JsonDic;
            do
            {
                if (mRoleExprienceDic.TryGetValue(b_Component.Data.Level, out var mExperienceConfig) == false) break;

                var mConfigExprience = mExperienceConfig.Exprience;
                var mHasExprienceTemp = b_Component.Data.Exp;

                var mAddExprienceTemp = mLibraryExprience;
                if (mHasExprienceTemp + mAddExprienceTemp >= mConfigExprience)
                {
                    mAddExprienceTemp = mConfigExprience - mHasExprienceTemp;
                }

                if (mAddExprienceTemp != 0)
                {
                    b_Component.Data.Exp += mAddExprienceTemp;
                    mLibraryExprience -= mAddExprienceTemp;
                }

                if (b_Component.Data.Exp < mConfigExprience) break;

                if (b_Component.Data.Level == levelLimit) break;
                if (b_Component.Data.Level >= 400 && b_Component.Data.OccupationLevel < 3) break;

                b_Component.Data.Exp = 0;
                b_Component.Data.Level++;
                UpLevelMethod();

                void UpLevelMethod()
                {
                    //// 角色到达五十级开启宠物系统赠送一只玉兔
                    //if (b_Component.Data.Level == 50 && b_Component.PetsList.Count == 0 && b_Component.Pets == null)
                    //    b_Component.FreeTrialPets(100);


                    if (mReadConfigComponent.GetJson<CreateRole_InfoConfigJson>().JsonDic.TryGetValue(b_Component.Data.PlayerTypeId, out var mPlayerTypeConfig))
                    {
                        int OccupationLevel = b_Component.Data.OccupationLevel;
                        if (b_Component.Data.ReincarnateCnt >= 1)
                        {
                            if (b_Component.Data.Level < 120)
                                OccupationLevel = 0;
                            else if (120 <= b_Component.Data.Level && b_Component.Data.Level < 220)
                                OccupationLevel = 1;
                            else if (220 <= b_Component.Data.Level && b_Component.Data.Level <= 400)
                            {
                                OccupationLevel = 2;
                                if (b_Component.Data.PlayerTypeId == (int)E_GameOccupation.Spellsword || b_Component.Data.PlayerTypeId == (int)E_GameOccupation.Holyteacher
                                    || b_Component.Data.PlayerTypeId == (int)E_GameOccupation.Combat || b_Component.Data.PlayerTypeId == (int)E_GameOccupation.GrowLancer)
                                    OccupationLevel = 1;
                            }
                        }
                        if (mPlayerTypeConfig.AppendLevelDic.TryGetValue(OccupationLevel, out var mFreePoint))
                        {
                            b_Component.Data.FreePoint += mFreePoint;
                        }
                    }
                    //邀请等级到达奖励等级触发
                    {
                        if (b_Component.Code != "" && (b_Component.Data.Level == 120 || b_Component.Data.Level == 220))
                        {
                            G2M_PromotionLevel g2M_PromotionLevel = new G2M_PromotionLevel();
                            g2M_PromotionLevel.Code = b_Component.Code;
                            g2M_PromotionLevel.UserID = b_Component.Player.UserId;
                            g2M_PromotionLevel.Level = b_Component.Data.Level;
                            var loginCenterList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.LoginCenter);
                            Session loginCenterSession = Game.Scene.GetComponent<NetInnerComponent>().Get(loginCenterList[0].ServerInnerIP);
                            loginCenterSession.Send(g2M_PromotionLevel);
                        }
                    }
                    //等级排行
                    G2M_EnterRankingRequest g2M_EnterRankingRequest = new G2M_EnterRankingRequest();
                    g2M_EnterRankingRequest.AppendData = b_Component.Player.SourceGameAreaId;
                    g2M_EnterRankingRequest.Value64A = b_Component.Player.GameUserId;
                    g2M_EnterRankingRequest.RankType = (int)RankType.LevelRank;
                    g2M_EnterRankingRequest.StrA = b_Component.Data.NickName;
                    g2M_EnterRankingRequest.Value32A = b_Component.Data.Level;
                    g2M_EnterRankingRequest.Value32B = b_Component.Data.ReincarnateCnt;
                    g2M_EnterRankingRequest.Value32C = b_Component.Data.PlayerTypeId;
                    b_Component.Player.GetSessionMGMT().Send(g2M_EnterRankingRequest);

                    var WA = b_Component.Player.GetCustomComponent<PlayerWarAllianceComponent>();
                    if (WA != null && WA.WarAllianceID != 0)
                    {
                        WA.UpDateWarAlliancePlayerInfo();
                    }
                    if (!ConstServer.PlayerMaster)
                        if (b_Component.Data.Level > 400 && b_Component.Data.OccupationLevel >= 3)
                        {
                            async Task master()
                            {
                                // 初始化大师技能需要

                                var mPlayer = b_Component.Player;
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
                                        PropertyPoint = b_Component.Data.Level - 401
                                    };

                                    bool mSaveResult = await dBProxy2.Save(mDatalist_Skill);
                                    if (mSaveResult == false)
                                    {
                                        Log.Error($"{mPlayer.GameUserId}大师初始化异常 1");
                                        return;
                                    }
                                    mDataCache_Skill.DataAdd(mDatalist_Skill);
                                }
                                if (mDatalist_Skill.SkillId == null) mDatalist_Skill.SkillId = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(mDatalist_Skill.Skill);

                                mDatalist_Skill.PropertyPoint++;
                                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)b_Component.Player.GameAreaId);
                                mWriteDataComponent.Save(mDatalist_Skill, dBProxy2).Coroutine();
                            }
                            master().Coroutine();
                        }

                    mUpLevel = true;
                }

            } while (true);

            var mAddAllExprience = b_AddExprience - mLibraryExprience;

            G2C_ChangeValue_notice mExprienceNotice = new G2C_ChangeValue_notice();
            mExprienceNotice.GameUserId = b_Component.InstanceId;
            G2C_BattleKVData mExpMessage = new G2C_BattleKVData();
            mExpMessage.Key = (int)E_GameProperty.ExprienceDrop;
            mExpMessage.Value = mAddAllExprience;
            mExprienceNotice.Info.Add(mExpMessage);
            mExpMessage = new G2C_BattleKVData();
            mExpMessage.Key = (int)E_GameProperty.Exprience;
            mExpMessage.Value = b_Component.Data.Exp;
            mExprienceNotice.Info.Add(mExpMessage);
            if (mUpLevel)
            {
                b_Component.DataUpdateProperty();

                var mHP_MAX = b_Component.GetNumerial(E_GameProperty.PROP_HP_MAX);
                if (b_Component.UnitData.Hp < mHP_MAX) b_Component.UnitData.Hp = mHP_MAX;

                var mMP_MAX = b_Component.GetNumerial(E_GameProperty.PROP_MP_MAX);
                if (b_Component.UnitData.Mp < mMP_MAX) b_Component.UnitData.Mp = mMP_MAX;

                var mSd_MAX = b_Component.GetNumerial(E_GameProperty.PROP_SD_MAX);
                if (b_Component.UnitData.SD < mSd_MAX) b_Component.UnitData.SD = mSd_MAX;

                var mAG_MAX = b_Component.GetNumerial(E_GameProperty.PROP_AG_MAX);
                if (b_Component.UnitData.AG < mAG_MAX) b_Component.UnitData.AG = mAG_MAX;

                // 发布 GamePlayerLevelUp 事件
                ETModel.EventType.GamePlayerLevelUp.Instance.gamePlayer = b_Component;
                ETModel.EventType.GamePlayerLevelUp.Instance.oldLevel = oldLevel;
                ETModel.EventType.GamePlayerLevelUp.Instance.newLevel = b_Component.Data.Level;
                Root.EventSystem.OnRun("GamePlayerLevelUp", ETModel.EventType.GamePlayerLevelUp.Instance);

                void AddPropertyNotice(G2C_ChangeValue_notice b_ChangeValue_notice, E_GameProperty b_GameProperty)
                {
                    G2C_BattleKVData mBattleKVData = new G2C_BattleKVData();
                    mBattleKVData.Key = (int)b_GameProperty;
                    mBattleKVData.Value = b_Component.GetNumerial(b_GameProperty);
                    b_ChangeValue_notice.Info.Add(mBattleKVData);
                }

                AddPropertyNotice(mExprienceNotice, E_GameProperty.Level);
                AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_HP);
                AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_HP_MAX);
                AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_MP);
                AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_MP_MAX);
                AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_SD);
                AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_SD_MAX);
                AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_AG);
                AddPropertyNotice(mExprienceNotice, E_GameProperty.PROP_AG_MAX);
                AddPropertyNotice(mExprienceNotice, E_GameProperty.FreePoint);
            }

            if (mAddAllExprience != 0)
            {
                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.Player.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)b_Component.Player.GameAreaId);
                mWriteDataComponent.Save(b_Component.Data, dBProxy).Coroutine();
            }

            b_Component.Player.Send(mExprienceNotice);
            return mAddAllExprience != 0;
        }
    }
}