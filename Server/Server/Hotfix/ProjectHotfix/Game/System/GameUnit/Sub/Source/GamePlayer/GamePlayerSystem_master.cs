using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{



    public static partial class GamePlayerSystem
    {

        public static void AwakeMaster(this GamePlayer b_Component)
        {
            if (b_Component.MasterGroup == null) b_Component.MasterGroup = new Dictionary<int, C_BattleMaster>();
        }
        public static async Task DataUpdateMaster(this GamePlayer b_Component)
        {
            var mPlayer = b_Component.Player;
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();

            var mDataCache = mDataCacheManageComponent.Get<DBMasterData>();
            if (mDataCache == null)
            {
                var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                mDataCache = await HelpDb_DBMasterData.Init(mPlayer, mDataCacheManageComponent, dBProxy);
            }
            var mData = mDataCache.OnlyOne();
            if (mData == null)
            {
                if (!ConstServer.PlayerMaster)
                {
                    mData = new DBMasterData()
                    {
                        Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                        GameUserId = mPlayer.GameUserId,
                        PropertyPoint = b_Component.Data.Level - 400
                    };
                }
                else 
                {
                    mData = new DBMasterData()
                    {
                        Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId),
                        GameUserId = mPlayer.GameUserId,
                        PropertyPoint = 0
                    };
                }

                var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);
                bool mSaveResult = await dBProxy.Save(mData);
                if (mSaveResult == false)
                {
                    return;
                }
                mDataCache.DataAdd(mData);
            }
            if (mData.SkillId == null) mData.SkillId = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, int>>(mData.Skill);

            // bug 造成的问题补偿 没加点的才有
            if (mData.SkillId.Count == 0)
            {
                if (!ConstServer.PlayerMaster)
                    mData.PropertyPoint = b_Component.Data.Level - 400;
                else
                    mData.PropertyPoint = 0;
            }

            var mBattleMasterCreateBuilder = Root.MainFactory.GetCustomComponent<BattleMasterCreateBuilder>();

            if (mData.SkillId.Count > 0)
            {
                var mKeylist = mData.SkillId.Keys.ToArray();
                switch ((E_GameOccupation)b_Component.Data.PlayerTypeId)
                {
                    case E_GameOccupation.Spell:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_SpellConfigJson>().JsonDic;

                            for (int i = 0, len = mKeylist.Length; i < len; i++)
                            {
                                var mSkillId = mKeylist[i];

                                if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                                {
                                    if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                                    {
                                        var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                                        bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(b_Component, null, false);
                                        }
                                        else
                                        {
                                            Log.Error(mSkillJson.Name + "不能使用");
                                        }

                                        b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Swordsman:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_SwordsmanConfigJson>().JsonDic;

                            for (int i = 0, len = mKeylist.Length; i < len; i++)
                            {
                                var mSkillId = mKeylist[i];

                                if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                                {
                                    if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                                    {
                                        var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                                        bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(b_Component, null, false);
                                        }
                                        else
                                        {
                                            Log.Error(mSkillJson.Name + "不能使用");
                                        }

                                        b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Archer:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_ArcherConfigJson>().JsonDic;

                            for (int i = 0, len = mKeylist.Length; i < len; i++)
                            {
                                var mSkillId = mKeylist[i];

                                if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                                {
                                    if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                                    {
                                        var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                                        bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(b_Component, null, false);
                                        }
                                        else
                                        {
                                            Log.Error(mSkillJson.Name + "不能使用");
                                        }

                                        b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Spellsword:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_SpellswordConfigJson>().JsonDic;

                            for (int i = 0, len = mKeylist.Length; i < len; i++)
                            {
                                var mSkillId = mKeylist[i];

                                if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                                {
                                    if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                                    {
                                        var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                                        bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(b_Component, null, false);
                                        }
                                        else
                                        {
                                            Log.Error(mSkillJson.Name + "不能使用");
                                        }

                                        b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Holyteacher:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_HolyteacherConfigJson>().JsonDic;

                            for (int i = 0, len = mKeylist.Length; i < len; i++)
                            {
                                var mSkillId = mKeylist[i];

                                if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                                {
                                    if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                                    {
                                        var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                                        bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(b_Component, null, false);
                                        }
                                        else
                                        {
                                            Log.Error(mSkillJson.Name + "不能使用");
                                        }

                                        b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.SummonWarlock:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_SummonWarlockConfigJson>().JsonDic;

                            for (int i = 0, len = mKeylist.Length; i < len; i++)
                            {
                                var mSkillId = mKeylist[i];

                                if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                                {
                                    if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                                    {
                                        var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                                        bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(b_Component, null, false);
                                        }
                                        else
                                        {
                                            Log.Error(mSkillJson.Name + "不能使用");
                                        }

                                        b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.Combat:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_CombatConfigJson>().JsonDic;

                            for (int i = 0, len = mKeylist.Length; i < len; i++)
                            {
                                var mSkillId = mKeylist[i];

                                if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                                {
                                    if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                                    {
                                        var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                                        bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(b_Component, null, false);
                                        }
                                        else
                                        {
                                            Log.Error(mSkillJson.Name + "不能使用");
                                        }

                                        b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                                    }
                                }
                            }
                        }
                        break;
                    case E_GameOccupation.GrowLancer:
                        {
                            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_DreamKnightConfigJson>().JsonDic;

                            for (int i = 0, len = mKeylist.Length; i < len; i++)
                            {
                                var mSkillId = mKeylist[i];

                                if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                                {
                                    if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                                    {
                                        var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                                        bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(b_Component, null, false);
                                        }
                                        else
                                        {
                                            Log.Error(mSkillJson.Name + "不能使用");
                                        }

                                        b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
                
                var mCareerJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_CareerConfigJson>().JsonDic;

                for (int i = 0, len = mKeylist.Length; i < len; i++)
                {
                    var mSkillId = mKeylist[i];

                    if (mCareerJsonDic.TryGetValue(mSkillId, out var mSkillJson))
                    {
                        if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                        {
                            var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                            bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                            if (mUseResult)
                            {
                                mSkillInstance.UseSkill(b_Component, null, false);
                            }
                            else
                            {
                                Log.Error(mSkillJson.Name + "不能使用");
                            }

                            b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                        }
                    }
                }
                {
                    var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<BattleMaster_ALLConfigJson>().JsonDic;
                    for (int i = 0, len = mKeylist.Length; i < len; i++)
                    {
                        var mSkillId = mKeylist[i];
                        var mLevel = mData.SkillId[mSkillId];

                        if (mLevel <= 0) continue;

                        if (mJsonDic.TryGetValue(mSkillId, out var mSkillJson) == false) continue;

                        switch ((E_BattleMaster_Id)mSkillId)
                        {
                            case E_BattleMaster_Id.Common2001:
                            case E_BattleMaster_Id.Common2002:
                            case E_BattleMaster_Id.Common2003:
                            case E_BattleMaster_Id.Common2004:
                            case E_BattleMaster_Id.Common2005:
                            case E_BattleMaster_Id.Common2006:
                            case E_BattleMaster_Id.Common2007:
                            case E_BattleMaster_Id.Common2008:
                            case E_BattleMaster_Id.Common2009:
                            case E_BattleMaster_Id.Common2010:
                            case E_BattleMaster_Id.Common2011:
                            case E_BattleMaster_Id.Common2012:
                            case E_BattleMaster_Id.Common2013:
                                {
                                    if (b_Component.MasterGroup.ContainsKey(mSkillId) == false)
                                    {
                                        var mSkillInstance = mBattleMasterCreateBuilder.CreateHeroMaster(mSkillId, mData);
                                        bool mUseResult = mSkillInstance.TryUse(b_Component, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(b_Component, null, false);
                                        }
                                        else
                                        {
                                            Log.Error(mSkillJson.Name + "不能使用");
                                        }

                                        b_Component.MasterGroup[mSkillInstance.Id] = mSkillInstance;
                                    }
                                }
                                break;
                            default: break;
                        }
                    }
                }
            }

            b_Component.EnsureMasterGrantedSkills(false);
        }
    }
}
