using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.EventType;
using SharpCompress.Common;
using System.Security.Cryptography;


namespace ETHotfix
{
    /// <summary>
    /// 玩家上线准备完成
    /// </summary>
    [EventMethod("PlayerReadyComplete")]
    public class PlayerReadyComplete_PetInit : ITEventMethodOnRun<PlayerReadyComplete>
    {
        public void OnRun(PlayerReadyComplete args)
        {
            GamePlayer gamePlayer = args.player.GetCustomComponent<GamePlayer>();
            using (ListComponent<Pets> petsList = ListComponent<Pets>.Create())
            {
                if (gamePlayer.Pets != null)
                {
                    petsList.Add(gamePlayer.Pets);
                }
                if (gamePlayer.PetsList != null && gamePlayer.PetsList.Count > 0)
                {
                    petsList.AddRange(gamePlayer.PetsList.Values);
                }

                foreach (Pets pets in petsList)
                {
                    if (pets.IsDeath == true)
                    {
                        // 宠物是死亡的
                        // 添加复活组件
                        pets.AddCustomComponent<RebirthComponent>();
                    }
                    if (pets.dBPetsData.PetsTrialTime != 0)
                    {
                        // 宠物有试用时间
                        // 添加宠物试用计时组件
                        pets.AddCustomComponent<PetsTrialTimeComponent>();

                    }
                }
            }
        }
    }


    public static partial class GamePlayerSystem
    {

        public static void AwakePets(this GamePlayer b_Component)
        {
            b_Component.Pets = null;
            b_Component.PetsList = new Dictionary<long, Pets>();
        }
        /// <summary>
        /// 宠物数据读库
        /// </summary>
        /// <param name="b_Component"></param>
        public static async Task<bool> DataUpdatePets(this GamePlayer b_Component)
        {
            var mPlayer = b_Component.Player;
            DataCacheManageComponent mDataCacheManageComponent = mPlayer.GetCustomComponent<DataCacheManageComponent>();
            var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<PetsSkillCreateBuilder>();
            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManagerComponent.GetZoneDB(DBType.Core, mPlayer.GameAreaId);

            var mDataCache = mDataCacheManageComponent.Get<DBPetsData>();
            if (mDataCache == null)
            {
                mDataCache = await mDataCacheManageComponent.Add<DBPetsData>(dBProxy, p => p.GameUserId == mPlayer.GameUserId && p.IsDisabled != 1);
            }

            var PetsList = mDataCache.DataQuery(e => e.GameUserId == mPlayer.GameUserId);

            if (PetsList != null)
            {
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
                var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;

                C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Component.Player.SourceGameAreaId);
                mServerArea.GetCustomComponent<MapManageComponent>().keyValuePairs.TryGetValue(b_Component.UnitData.Index, out var mapComponent);

                var mSecondTick = Help_TimeHelper.GetNowSecond();
                var mTick = Help_TimeHelper.GetNow();
                foreach (var p in PetsList)
                {
                    if (p.PetsTrialTime != 0 && mSecondTick - p.PetsTrialTime >= 0)
                    {
                        p.IsDisabled = 1;
                        mWriteDataComponent.Save(p, dBProxy).Coroutine();
                        G2C_InsertPetsMessage g2C_InsertPetsMessage = new G2C_InsertPetsMessage();
                        g2C_InsertPetsMessage.State = 1;
                        g2C_InsertPetsMessage.MessageID = 1610;
                        mPlayer.Send(g2C_InsertPetsMessage);
                        continue;
                    }

                    if (mJsonDic.TryGetValue(p.ConfigID, out Pets_InfoConfig pets_Info) == false) return false;

                    Pets pets = Root.CreateBuilder.GetInstance<Pets>();
                    pets.Identity = E_Identity.Pet;
                    pets.SetConfig(p, pets_Info);
                    pets.AfterAwake();
                    pets.SetInstanceId(p.PetsId);
                    pets.DataAddPropertyBuffer();
                    pets.GameHeroSexType = (E_GameHeroSexType)pets_Info.AttackType;
                    pets.AddValueExcellent();
                    pets.UpDataEnhanceA();
                    pets.DataUpdateProperty();

                    if (SkillJsonDic != null)
                    {
                        if (pets.SkillGroup == null) pets.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
                        pets.dBPetsData.SkillId = Help_JsonSerializeHelper.DeSerialize<List<int>>(p.SkillID);
                        var SkillList = pets.dBPetsData.SkillId;
                        foreach (var SkillID in SkillList)
                        {
                            if (SkillJsonDic.TryGetValue(SkillID, out var SkillJson))
                            {
                                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(SkillID);
                                
                                if (pets.SkillGroup.ContainsKey(SkillID) == false)
                                {
                                    pets.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                    if (SkillJson.skillType == 1)
                                    {
                                        if (SkillID == p.UseSkillID)
                                        {
                                            pets.SkillCurrent = mSkillInstance;
                                        }
                                    }
                                    else if (SkillJson.skillType == 2)
                                    {

                                        bool mUseResult = mSkillInstance.TryUse(pets, pets, null, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(pets, pets, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    pets.dBPetsData.ExcellentId = Help_JsonSerializeHelper.DeSerialize<List<int>>(pets.dBPetsData.Excellent);
                    pets.AddValueExcellent();
                    pets.DataUpdateProperty();
                    pets.UpdataExcellentValue();
                    Log.PLog("PetsInfoLog",$"RDB1 Name:{b_Component.Data.NickName}PetsName{pets.dBPetsData.PetsName} DeathTime:{p.DeathTime} NowTime:{Help_TimeHelper.GetNow()} SleepTime:{0} ChaZhi:{0}");
                    if (pets.dBPetsData.DeathTime != 0 && mTick - pets.dBPetsData.DeathTime < pets_Info.Regen)
                    {
                        pets.IsDeath = true;
                        //if (pets.dBPetsData.PetsUseState == 1)
                        pets.DeathSleepTime = pets.dBPetsData.DeathTime + pets_Info.Regen;
                        //else
                        //pets.DeathSleepTime = (pets.dBPetsData.DeathTime - mTick) + pets_Info.Regen;
                        //Log.PLog("PetsInfoLog", $"RDB2 Name:{b_Component.Data.NickName}PetsName{pets.dBPetsData.PetsName} DeathTime:{pets.dBPetsData.DeathTime} NowTime:{Help_TimeHelper.GetNow()} SleepTime:{pets.DeathSleepTime} ChaZhi:{p.DeathTime - Help_TimeHelper.GetNow()} ConfigTime:{mJsonDic[p.ConfigID].Regen}");
                    }
                    else if (pets.dBPetsData.DeathTime != 0 && mTick - pets.dBPetsData.DeathTime > pets_Info.Regen)
                    {
                        pets.dBPetsData.PetsHP = pets.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                        pets.dBPetsData.PetsMP = pets.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                        pets.dBPetsData.DeathTime = 0;
                        pets.IsDeath = false;
                        mWriteDataComponent.Save(p, dBProxy).Coroutine();
                    }
                    //Log.PLog("PetsInfoLog", $"RDB3 Name:{b_Component.Data.NickName}PetsName{pets.dBPetsData.PetsName} DeathTime:{pets.dBPetsData.DeathTime} NowTime:{Help_TimeHelper.GetNow()} SleepTime:{pets.DeathSleepTime} ChaZhi:{pets.dBPetsData.DeathTime - Help_TimeHelper.GetNow()} ConfigTime:{mJsonDic[p.ConfigID].Regen}");
                    // 不管有没有出战，都设置GamePlayer
                    pets.GamePlayer = b_Component;
                    pets.IsDeath = false;
                    //var Item = await dBProxy.Query<DBItemData>(p => p.Id == pets.dBPetsData.PetsId);
                    //if (Item != null)
                    //{
                    //    if ((Item[0] as DBItemData).IsDispose >= 1698318000)
                    //    {
                    //        pets.dBPetsData.PetsUseState = 0;
                    //        pets.dBPetsData.IsDisabled = 1;
                    //        mWriteDataComponent.Save(pets.dBPetsData, dBProxy).Coroutine();
                    //    }
                    //}
                    if (pets.dBPetsData.PetsUseState == 1)
                    {
                        pets.UnitData.Angle = b_Component.UnitData.Angle;
                        b_Component.Pets = pets;
                        {
                            List<C_FindTheWay2D> mMapFieldDic = new List<C_FindTheWay2D>();

                            int mRepelValue = 1;
                            var mCurrentTemp = mapComponent.GetFindTheWay2D(b_Component.UnitData.X + mRepelValue, b_Component.UnitData.Y + mRepelValue);
                            if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false && mMapFieldDic.Contains(mCurrentTemp) == false) mMapFieldDic.Add(mCurrentTemp);

                            mCurrentTemp = mapComponent.GetFindTheWay2D(b_Component.UnitData.X + mRepelValue, b_Component.UnitData.Y - mRepelValue);
                            if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false && mMapFieldDic.Contains(mCurrentTemp) == false) mMapFieldDic.Add(mCurrentTemp);

                            mCurrentTemp = mapComponent.GetFindTheWay2D(b_Component.UnitData.X - mRepelValue, b_Component.UnitData.Y + mRepelValue);
                            if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false && mMapFieldDic.Contains(mCurrentTemp) == false) mMapFieldDic.Add(mCurrentTemp);

                            mCurrentTemp = mapComponent.GetFindTheWay2D(b_Component.UnitData.X - mRepelValue, b_Component.UnitData.Y - mRepelValue);
                            if (mCurrentTemp != null && mCurrentTemp.IsStaticObstacle == false && mMapFieldDic.Contains(mCurrentTemp) == false) mMapFieldDic.Add(mCurrentTemp);

                            if (mMapFieldDic.Count > 0)
                            {
                                var mRandomIndex = Help_RandomHelper.Range(0, mMapFieldDic.Count);
                                var mRandomResult = mMapFieldDic[mRandomIndex];

                                //                                 pets.UnitData.Index = mRandomResult.Map.MapId;
                                //                                 pets.UnitData.X = mRandomResult.X;
                                //                                 pets.UnitData.Y = mRandomResult.Y;
                                pets.UnitData.Angle = b_Component.UnitData.Angle;
                                b_Component.Pets = pets;
                            }
                            else
                            {
                                var mRandomResult = mapComponent.GetFindTheWay2D(b_Component);

                                //                                 pets.UnitData.Index = mRandomResult.Map.MapId;
                                //                                 pets.UnitData.X = mRandomResult.X;
                                //                                 pets.UnitData.Y = mRandomResult.Y;
                                pets.UnitData.Angle = b_Component.UnitData.Angle;
                                b_Component.Pets = pets;
                            }
                        }
                    }
                    else
                    {
                        /* pets.UnitData.Index = -1;*/
                        if(pets.dBPetsData.IsDisabled != 1)
                        b_Component.PetsList.Add(pets.InstanceId, pets);
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// 角色获得宠物,Free = true 表示赠送的
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="PetsConfigID"></param>
        /// <param name="Free"></param>
        public static bool FreeTrialPets(this GamePlayer b_Component, int PetsConfigID, bool Free = true)
        {
            if (b_Component != null)
            {
                var mPlayer = b_Component.Player;

                var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
                if (mJsonDic != null)
                {
                    Pets pets = Root.CreateBuilder.GetInstance<Pets>();
                    DBPetsData dBPetsData = new DBPetsData();

                    pets.IsDeath = false;
                    pets.DeathSleepTime = 0;
                    pets.Identity = E_Identity.Pet;
                    pets.SetConfig(dBPetsData, mJsonDic[PetsConfigID]);
                    pets.GameHeroSexType = (E_GameHeroSexType)mJsonDic[PetsConfigID].PetsType;
                    {
                        dBPetsData.Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                        dBPetsData.GameUserId = mPlayer.GameUserId;
                        dBPetsData.ConfigID = PetsConfigID;
                        dBPetsData.PetsId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
                        dBPetsData.PetsName = mJsonDic[PetsConfigID].Name;
                        dBPetsData.PetsLevel = 1;
                        dBPetsData.PetsUseState = 0;
                        dBPetsData.PetsExp = 0;
                        dBPetsData.DeathTime = 0;
                        dBPetsData.AttributePoint = 0;
                        dBPetsData.PetsSTR = mJsonDic[PetsConfigID].Strength;
                        dBPetsData.PetsDEX = mJsonDic[PetsConfigID].Agility;
                        dBPetsData.PetsPSTR = mJsonDic[PetsConfigID].BoneGas;
                        dBPetsData.PetsPINT = mJsonDic[PetsConfigID].Willpower;
                        dBPetsData.IsDisabled = 0;
                        dBPetsData.UseSkillID = 0;
                        if (mJsonDic[PetsConfigID].AttackType == 0)
                        {
                            dBPetsData.PetsHP = (33 + (1 * 2) + (dBPetsData.PetsPSTR * 3)) * (1 + 0);
                            dBPetsData.PetsMP = (int)(10 + ((1 - 1) * 0.5) + ((dBPetsData.PetsPINT + dBPetsData.PintAdd) * 1)) * (1 + 0);
                        }
                        else
                        {
                            dBPetsData.PetsHP = (44 + (1 * 1) + (dBPetsData.PetsPSTR * 1)) * (1 + 0);
                            dBPetsData.PetsMP = ((1 - 1) * 2) + ((dBPetsData.PetsPINT + dBPetsData.PintAdd) * 2) * (1 + 0);
                        }


                        if (PetsConfigID == 100)
                            dBPetsData.PetsTrialTime = Help_TimeHelper.GetNowSecond() + mJsonDic[PetsConfigID].TrialTime;
                        else
                            dBPetsData.PetsTrialTime = 0;

                        pets.GamePropertyDic[E_GameProperty.MoveSpeed] = mJsonDic[PetsConfigID].MoSpeed;
                        pets.GameHeroSexType = (E_GameHeroSexType)mJsonDic[PetsConfigID].AttackType;
                        //pets.AfterAwake();
                        pets.SetInstanceId(dBPetsData.PetsId);
                        //pets.DataAddPropertyBuffer();
                        //pets.AddValueExcellent();
                        //pets.UpDataEnhanceA();
                        //pets.DataUpdateProperty();
                        //int SkillID = mJsonDic[PetsConfigID].SkillID;
                        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<PetsSkillCreateBuilder>();
                        var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;
                        if (SkillJsonDic != null)
                        {
                            pets.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
                            foreach (var skill in mJsonDic[PetsConfigID].SkillID)
                            {
                                if (pets.SkillGroup.ContainsKey(skill) == false)
                                {
                                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(skill);
                                    pets.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                    if (SkillJsonDic[skill].skillType == 1)
                                    {
                                        //if (skill == dBPetsData.UseSkillID)
                                        {
                                            pets.SkillCurrent = mSkillInstance;
                                        }
                                    }
                                    else if (SkillJsonDic[skill].skillType == 2)
                                    {
                                        bool mUseResult = mSkillInstance.TryUse(pets, pets, null, null, null);
                                        if (mUseResult)
                                        {
                                            mSkillInstance.UseSkill(pets, pets, null);
                                        }
                                    }
                                    dBPetsData.SkillId.Add(skill);

                                    //if (Free)
                                    //{
                                    //    var mSkillInstance2 = mSkillCreateBuilder.CreateHeroSKill(SkillID + 1);
                                    //    pets.SkillGroup[mSkillInstance2.Id] = mSkillInstance2;
                                    //    if (SkillJsonDic[SkillID + 1].skillType == 1)
                                    //    {
                                    //        if (SkillID + 1 == dBPetsData.UseSkillID)
                                    //        {
                                    //            pets.SkillCurrent = mSkillInstance2;
                                    //        }
                                    //    }
                                    //    else if (SkillJsonDic[SkillID + 1].skillType == 2)
                                    //    {
                                    //        bool mUseResult = mSkillInstance2.TryUse(pets, pets, null, null, null);
                                    //        if (mUseResult)
                                    //        {
                                    //            mSkillInstance2.UseSkill(pets, pets, null);
                                    //        }
                                    //    }
                                    //    dBPetsData.SkillId.Add(SkillID + 1);
                                    //}
                                }
                            }
                        }

                        var excAttrEntryManager = Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
                        if (excAttrEntryManager.TryGetPetsAttrEntry(out var selector))
                        {
                            if (excAttrEntryManager.PetsExcAttrEntryCount.TryGetValue(out int count))
                            {
                                var newSelector = new RandomSelector<int>(selector);
                                do
                                {
                                    if (newSelector.TryGetValueAndRemove(out var entryId))
                                    {
                                        dBPetsData.ExcellentId.Add(entryId);
                                    }
                                    else
                                    {
                                        // 词条取空了
                                        break;
                                    }
                                    count--;
                                }
                                while (count > 0);
                            }
                        }
                        dBPetsData.SkillID = Help_JsonSerializeHelper.Serialize(dBPetsData.SkillId);
                        dBPetsData.Excellent = Help_JsonSerializeHelper.Serialize(dBPetsData.ExcellentId);
                    }
                    //pets.AfterAwake(dBPetsData);
                    //pets.SetInstanceId(dBPetsData.PetsId);
                    //pets.DataAddPropertyBuffer();
                    //pets.AddValueExcellent();
                    //pets.DataUpdateProperty();
                    //pets.UpdataExcellentValue();
                    pets.GamePlayer = b_Component;
                    b_Component.PetsList.Add(dBPetsData.PetsId, pets);

                    if (pets.dBPetsData.PetsTrialTime > 0)
                    {
                        // 添加宠物试用计时组件
                        pets.AddCustomComponent<PetsTrialTimeComponent>();
                    }

                    DataCacheManageComponent mDataCacheManageComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();
                    var dataCache = mDataCacheManageComponent.Get<DBPetsData>();
                    var backPackDatas = dataCache.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.PetsId == dBPetsData.PetsId);

                    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                    if (backPackDatas.Count == 0)
                    {
                        dataCache.DataAdd(dBPetsData);
                        dBProxy.Save(dBPetsData).Coroutine();
                    }

                    if (Free)
                    {
                        G2C_InsertPetsMessage g2C_InsertPetsMessage = new G2C_InsertPetsMessage();
                        g2C_InsertPetsMessage.State = 1;
                        g2C_InsertPetsMessage.MessageID = 0;
                        mPlayer.Send(g2C_InsertPetsMessage);
                        Log.Debug($"角色: {mPlayer.GameUserId}名称:{mPlayer.GetCustomComponent<GamePlayer>().Data.NickName}PetsConfingID: {dBPetsData.ConfigID}PetsID:{dBPetsData.PetsId}写库");
                    }
                    return true;
                }
                return false;
            }
            return false;
        }
        /// <summary>
        /// 宠物使用恢复道具,使用宠物专用药瓶
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="ItemID"></param>
        public static async Task PetsUseItem(this GamePlayer b_Component, int[] ItemID)
        {
            BackpackComponent backpack = b_Component.Player.GetCustomComponent<BackpackComponent>();
            if (backpack == null)
            {
                return;
            }
            //找到物品
            Item curItem = null;
            foreach (var Item in backpack.mItemDict)
            {
                if (ItemID.Contains(Item.Value.ConfigID))
                {
                    curItem = Item.Value;
                }
            }
            if (curItem == null) return;

            var itemConfig = curItem.ConfigData;
            if (curItem.Type != EItemType.Consumables || string.IsNullOrWhiteSpace(itemConfig.UseMethod))
            {
                return;
            }
            var mItemUseRuleCreateBuilder = Root.MainFactory.GetCustomComponent<ItemUseRuleCreateBuilder>();
            if (mItemUseRuleCreateBuilder.CacheDatas.TryGetValue(itemConfig.UseMethod, out var mItemUseRuleType) == false)
            {
                return;
            }

            var mItemUseRule = Root.CreateBuilder.GetInstance<C_ItemUseRule<Player, Item, IResponse>>(mItemUseRuleType);
            await mItemUseRule.Run(b_Component.Player, curItem, null);
            mItemUseRule.Dispose();

            backpack.UseItem(curItem.ItemUID, "宠物使用物品", 1);
            return;
        }
        /// <summary>
        /// 宠物加卓越属性给角色
        /// </summary>
        /// <param name="b_Component"></param>
        public static void PetsUseExcellent(this GamePlayer b_Component, Pets PetsInfo)
        {
            if (PetsInfo != null)
            {
                List<int> IDList = PetsInfo.dBPetsData.ExcellentId;
                var excellent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_ExcConfigJson>().JsonDic;
                var itemAttrEntryManager = Root.MainFactory.GetCustomComponent<ItemAttrEntryManager>();
                foreach (int i in IDList)
                {
                    if (excellent.ContainsKey(i) != false)
                    {
                        var excEntry = itemAttrEntryManager.GetOrCreate(i, 0);
                        if (excEntry == null)
                        {
                            Log.Error($"没找到物品属性词条。entryId={i}");
                        }
                        else
                        {
                            excEntry.ApplyPropTo(b_Component);
                        }
                    }
                }
            }
        }
        public static void PetsDleExcellent(this GamePlayer b_Component, Pets PetsInfo)
        {
            if (PetsInfo != null)
            {
                List<int> IDList = PetsInfo.dBPetsData.ExcellentId;
                var excellent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_ExcConfigJson>().JsonDic;
                foreach (int i in IDList)
                {
                    if (excellent.ContainsKey(i) != false)
                    {
                        Excellent Type = (Excellent)excellent[i].PropId;
                        int Value_A = excellent[i].Value0;
                        switch (Type)
                        {
                            case Excellent.EHP:
                                PetsInfo.GamePropertyDic[E_GameProperty.ExcellentAttackRate] -= Value_A;
                                break;
                            case Excellent.API:
                                int Value_B = PetsInfo.dBPetsData.PetsLevel / Value_A;
                                PetsInfo.GamePropertyDic[E_GameProperty.MaxAtteck] -= Value_B;
                                PetsInfo.GamePropertyDic[E_GameProperty.MinAtteck] -= Value_B;
                                break;
                            case Excellent.APIP:
                                PetsInfo.GamePropertyDic[E_GameProperty.MaxAtteck] -= PetsInfo.GetNumerialFunc(E_GameProperty.MaxAtteck) * Value_A;
                                PetsInfo.GamePropertyDic[E_GameProperty.MinAtteck] -= PetsInfo.GetNumerialFunc(E_GameProperty.MinAtteck) * Value_A;
                                break;
                            case Excellent.ASI:
                                PetsInfo.GamePropertyDic[E_GameProperty.AttackSpeed] -= Value_A;
                                break;
                            case Excellent.HGWKM:
                                PetsInfo.GamePropertyDic[E_GameProperty.KillEnemyReplyHpRate] -= PetsInfo.GetNumerialFunc(E_GameProperty.PROP_HP_MAX) / Value_A;
                                break;
                            case Excellent.MVGWKM:
                                PetsInfo.GamePropertyDic[E_GameProperty.KillEnemyReplyMpRate] -= PetsInfo.GetNumerialFunc(E_GameProperty.PROP_MP_MAX) / Value_A;
                                break;
                            case Excellent.MHI:
                                PetsInfo.GamePropertyDic[E_GameProperty.HealthBonus] -= Value_A;
                                break;
                            case Excellent.MMVI:
                                PetsInfo.GamePropertyDic[E_GameProperty.MagicBonus] -= Value_A;
                                break;
                            case Excellent.DR:
                                PetsInfo.GamePropertyDic[E_GameProperty.InjuryValueRate_Reduce] -= Value_A;
                                break;
                            case Excellent.IR:
                                PetsInfo.GamePropertyDic[E_GameProperty.BackInjuryRate] -= Value_A;
                                break;
                            case Excellent.DSR:
                                PetsInfo.GamePropertyDic[E_GameProperty.DefenseRate] -= Value_A;
                                break;
                            case Excellent.GGWKMI:
                                PetsInfo.GamePropertyDic[E_GameProperty.AddGoldCoinRate_Increase] -= Value_A;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }


        public static async Task<bool> PetsTurnEquipmentChuan(this GamePlayer self, long ItemId, int PetsId, string PetsName = null)
        {
            if (ItemId <= 0) return false;
            if (self.Pets != null) return false;
            if (self.PetsList.TryGetValue(ItemId, out var PetInfo))
            {
                if (!string.IsNullOrWhiteSpace(PetsName))
                {
                    PetInfo.dBPetsData.PetsName = PetsName;
                }
                PetInfo.UnitData.Angle = self.UnitData.Angle;
                PetInfo.GamePlayer = self;
                PetInfo.Pathlist = null;
                PetInfo.dBPetsData.PetsUseState = 1;

                self.Pets = PetInfo;
                self.PetsList.Remove(ItemId);

                MapComponent targetMapComponent = self.CurrentMap;
                if (targetMapComponent != null)
                {
                    var mFindTheWay = targetMapComponent.GetFindTheWay2D(self);
                    targetMapComponent.MoveSendNotice(null, mFindTheWay, PetInfo);
                }


                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, self.Player.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(self.Player.GameAreaId);
                mWriteDataComponent.Save(PetInfo.dBPetsData, dBProxy).Coroutine();
                return true;
            }
            else
            {
                Pets pets = Root.CreateBuilder.GetInstance<Pets>();
                var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
                if (mJsonDic.TryGetValue(PetsId, out var mPetJson) == false) return false;
                string targetPetName = string.IsNullOrWhiteSpace(PetsName) ? mPetJson.Name : PetsName;

                DBPetsData dBPetsData = new DBPetsData();
                pets.IsDeath = false;
                pets.DeathSleepTime = 0;
                pets.Identity = E_Identity.Pet;
                pets.SetConfig(dBPetsData, mPetJson);
                pets.GameHeroSexType = (E_GameHeroSexType)mPetJson.PetsType;
                {
                    dBPetsData.Id = IdGeneraterNew.Instance.GenerateUnitId(self.Player.GameAreaId);
                    dBPetsData.GameUserId = self.Player.GameUserId;
                    dBPetsData.ConfigID = mPetJson.Id;
                    dBPetsData.PetsId = ItemId;
                    dBPetsData.PetsName = targetPetName;
                    dBPetsData.PetsLevel = 1;
                    dBPetsData.PetsUseState = 1;
                    dBPetsData.PetsExp = 0;
                    dBPetsData.DeathTime = 0;
                    dBPetsData.AttributePoint = 0;
                    dBPetsData.PetsSTR = 0;
                    dBPetsData.PetsDEX = 0;
                    dBPetsData.PetsPSTR = 0;
                    dBPetsData.PetsPINT = 0;
                    dBPetsData.IsDisabled = 0;
                    dBPetsData.UseSkillID = 0;
                    dBPetsData.PetsHP = 0;
                    dBPetsData.PetsMP = 0;
                    dBPetsData.PetsTrialTime = 0;

                    pets.GamePropertyDic[E_GameProperty.MoveSpeed] = mPetJson.MoSpeed;
                    pets.GameHeroSexType = (E_GameHeroSexType)mPetJson.AttackType;
                    pets.SetInstanceId(dBPetsData.PetsId);
                    pets.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
                    var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<PetsSkillCreateBuilder>();
                    var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;
                    if (SkillJsonDic != null)
                    {
                        foreach (var skill in mJsonDic[mPetJson.Id].SkillID)
                        {
                            if (pets.SkillGroup.ContainsKey(skill) == false)
                            {
                                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(skill);
                                pets.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                if (SkillJsonDic[skill].skillType == 1)
                                {
                                    //if (skill == dBPetsData.UseSkillID)
                                    {
                                        pets.SkillCurrent = mSkillInstance;
                                    }
                                }
                                else if (SkillJsonDic[skill].skillType == 2)
                                {
                                    bool mUseResult = mSkillInstance.TryUse(pets, pets, null, null, null);
                                    if (mUseResult)
                                    {
                                        mSkillInstance.UseSkill(pets, pets, null);
                                    }
                                }
                                dBPetsData.SkillId.Add(skill);
                            }
                        }
                    }
                }
                dBPetsData.SkillID = Help_JsonSerializeHelper.Serialize(dBPetsData.SkillId);
                pets.GamePlayer = self;
                self.Pets = pets;

                MapComponent targetMapComponent = self.CurrentMap;
                if (targetMapComponent != null)
                {
                    var mFindTheWay = targetMapComponent.GetFindTheWay2D(self);
                    targetMapComponent.MoveSendNotice(null, mFindTheWay, pets);
                }

                DataCacheManageComponent mDataCacheManageComponent = self.Player.AddCustomComponent<DataCacheManageComponent>();
                var dataCache = mDataCacheManageComponent.Get<DBPetsData>();
                var backPackDatas = dataCache.DataQuery(p => p.GameUserId == self.Player.GameUserId && p.PetsId == pets.InstanceId);

                DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy2 = mDBProxyManagerComponent.GetZoneDB(DBType.Core, self.Player.GameAreaId);
                if (backPackDatas.Count == 0)
                {
                    dataCache.DataAdd(dBPetsData);
                    await dBProxy2.Save(dBPetsData);
                }
                return true;
            }
        }

        public static bool PetsTurnEquipmentTuo(this GamePlayer self, long ItemId, int PetsId)
        {
            if (ItemId <= 0) return false;
            if (self.Pets == null) return false;
            if (!self.PetsList.Keys.Contains(ItemId))
            {
                Pets pets = new Pets();
                pets = self.Pets;
                pets.dBPetsData.PetsUseState = 0;
                MapComponent targetMapComponent = self.CurrentMap;
                if (targetMapComponent != null)
                {
                    var mFindTheWay = targetMapComponent.GetFindTheWay2D(self.Pets);

                    targetMapComponent.QuitMap(mFindTheWay, self.Pets);
                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                    mAttackResultNotice.AttackTarget = self.Pets.InstanceId;
                    mAttackResultNotice.HpValue = 0;
                    targetMapComponent.SendNotice(self.Pets, mAttackResultNotice);
                }
                self.Pets = null;
                self.PetsList.Add(ItemId, pets);

                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, self.Player.GameAreaId);
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(self.Player.GameAreaId);
                mWriteDataComponent.Save(pets.dBPetsData, dBProxy).Coroutine();
                return true;
            }
            return false;
        }
    }
}
