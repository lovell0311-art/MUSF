
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    /// <summary>
    /// 使用宠物
    /// </summary>
    [ItemUseRule(typeof(UsePetsItem))]
    public class UsePetsItem : C_ItemUseRule<Player, Item, IResponse>
    {
        public override async Task Run(Player b_Player, Item b_Item, IResponse b_Response)
        {
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(b_Player.SourceGameAreaId);
            if (mServerArea == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }

            var mCurrentTemp = b_Player.GetCustomComponent<GamePlayer>();

            var mMapComponent = Help_MapHelper.GetMapByMapId(mServerArea, mCurrentTemp.UnitData.Index, b_Player.GameUserId);
            if (mMapComponent == null)
            {
                b_Response.Error = 99;
                b_Response.Message = "参数不对";
                return;
            }
            var mBattleComponent = mMapComponent.GetCustomComponent<BattleComponent>();

            if (b_Item.CanUse(mCurrentTemp) == false)
            {
                b_Response.Error = 2302;
                return;
            }

            DBProxyManagerComponent mDBProxyManagerComponent = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManagerComponent.GetZoneDB(DBType.Core, (int)b_Player.GameAreaId);
            DBPetsData dBPetsData = null;
            var PetList = await dBProxy2.Query<DBPetsData>(p => p.GameUserId == 0 && p.PetsId == b_Item.ItemUID && p.IsDisabled != 1);
            if (PetList != null && PetList.Count > 0)
            {
                dBPetsData = PetList[0] as DBPetsData;
            }

            Pets pets = Root.CreateBuilder.GetInstance<Pets>();
            var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
            if (mJsonDic.TryGetValue(b_Item.ConfigData.PetsId, out var mPetJson) == false)
            {
                b_Response.Error = 1604;
                return;
            }
            string targetPetName = string.IsNullOrWhiteSpace(b_Item.ConfigData?.Name) ? mPetJson.Name : b_Item.ConfigData.Name;
            DataCacheManageComponent mDataCacheManageComponent = b_Player.AddCustomComponent<DataCacheManageComponent>();
            var dataCache = mDataCacheManageComponent.Get<DBPetsData>();

            if (dBPetsData != null)
            {
                var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<PetsSkillCreateBuilder>();

                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Player.GameAreaId);
                var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;

                var mSecondTick = Help_TimeHelper.GetNowSecond();
                var mTick = Help_TimeHelper.GetNow();

                if (dBPetsData.PetsTrialTime != 0 && mSecondTick - dBPetsData.PetsTrialTime >= 0)
                {
                    dBPetsData.IsDisabled = 1;
                    mWriteDataComponent.Save(dBPetsData, dBProxy2).Coroutine();
                    G2C_InsertPetsMessage g2C_InsertPetsMessage = new G2C_InsertPetsMessage();
                    g2C_InsertPetsMessage.State = 1;
                    g2C_InsertPetsMessage.MessageID = 1610;
                    b_Player.Send(g2C_InsertPetsMessage);
                    return;
                }

                dBPetsData.GameUserId = b_Player.GameUserId;
                dBPetsData.PetsName = targetPetName;
                pets.Identity = E_Identity.Pet;
                pets.SetConfig(dBPetsData, mPetJson);
                pets.AfterAwake();
                pets.SetInstanceId(dBPetsData.PetsId);
                pets.DataAddPropertyBufferGotoMap(mBattleComponent);
                pets.GameHeroSexType = (E_GameHeroSexType)mPetJson.AttackType;
                pets.AddValueExcellent();
                pets.UpDataEnhanceA();
                pets.DataUpdateProperty();
                pets.GamePlayer = mCurrentTemp;
                if (SkillJsonDic != null)
                {
                    if (pets.SkillGroup == null) pets.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
                    pets.dBPetsData.SkillId = Help_JsonSerializeHelper.DeSerialize<List<int>>(dBPetsData.SkillID);
                    foreach (var Id in mPetJson.SkillID)
                    {
                        if (!pets.dBPetsData.SkillId.Contains(Id))
                        {
                            pets.dBPetsData.SkillId.Add(Id);
                            pets.dBPetsData.SkillID = Help_JsonSerializeHelper.Serialize(pets.dBPetsData.SkillId);
                        }
                    }
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
                                    if (SkillID == dBPetsData.UseSkillID)
                                    {
                                        pets.SkillCurrent = mSkillInstance;
                                    }
                                }
                                else if (SkillJson.skillType == 2)
                                {

                                    bool mUseResult = mSkillInstance.TryUse(pets, pets, null, null, null);
                                    if (mUseResult)
                                    {
                                        mSkillInstance.UseSkill(pets, pets, mBattleComponent);
                                    }
                                }
                            }
                        }
                    }
                }
                pets.dBPetsData.ExcellentId = Help_JsonSerializeHelper.DeSerialize<List<int>>(pets.dBPetsData.Excellent);

                //pets.UpdataExcellentValue();
                if (pets.dBPetsData.DeathTime != 0 && mTick - pets.dBPetsData.DeathTime < mPetJson.Regen)
                {
                    pets.IsDeath = true;
                    pets.DeathSleepTime = pets.dBPetsData.DeathTime + mPetJson.Regen;
                }
                else if (pets.dBPetsData.DeathTime != 0 && mTick - pets.dBPetsData.DeathTime > mPetJson.Regen)
                {
                    pets.dBPetsData.PetsHP = pets.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    pets.dBPetsData.PetsMP = pets.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                    pets.dBPetsData.DeathTime = 0;
                    pets.IsDeath = false;
                }
                pets.GamePlayer = mCurrentTemp;

                mCurrentTemp.PetsList.Add(pets.InstanceId, pets);

                if (pets.dBPetsData.PetsTrialTime > 0)
                {
                    // 添加宠物试用计时组件
                    pets.AddCustomComponent<PetsTrialTimeComponent>();
                }

                var backPackDatas = dataCache.DataQuery(p => p.GameUserId == b_Player.GameUserId && p.PetsId == pets.InstanceId);

                if (backPackDatas.Count == 0)
                {
                    dataCache.DataAdd(dBPetsData);
                    await dBProxy2.Save(dBPetsData);
                }
            }
            else
            {
                dBPetsData = new DBPetsData();
                pets.IsDeath = false;
                pets.DeathSleepTime = 0;
                pets.Identity = E_Identity.Pet;
                pets.SetConfig(dBPetsData, mPetJson);
                pets.GameHeroSexType = (E_GameHeroSexType)mPetJson.PetsType;
                {
                    dBPetsData.Id = IdGeneraterNew.Instance.GenerateUnitId(b_Player.GameAreaId);
                    dBPetsData.GameUserId = b_Player.GameUserId;
                    dBPetsData.ConfigID = mPetJson.Id;
                    dBPetsData.PetsId = b_Item.ItemUID;
                    dBPetsData.PetsName = targetPetName;
                    dBPetsData.PetsLevel = 1;
                    dBPetsData.PetsUseState = 0;
                    dBPetsData.PetsExp = 0;
                    dBPetsData.DeathTime = 0;
                    dBPetsData.AttributePoint = 0;
                    dBPetsData.PetsSTR = mPetJson.Strength;
                    dBPetsData.PetsDEX = mPetJson.Agility;
                    dBPetsData.PetsPSTR = mPetJson.BoneGas;
                    dBPetsData.PetsPINT = mPetJson.Willpower;
                    dBPetsData.IsDisabled = 0;
                    dBPetsData.UseSkillID = 0;
                    if (mPetJson.AttackType == 0)
                    {
                        dBPetsData.PetsHP = (33 + (1 * 4) + (dBPetsData.PetsPSTR * 6)) * (1 + 0);
                        dBPetsData.PetsMP = (int)(10 + ((1 - 1) * 0.5) + ((dBPetsData.PetsPINT + dBPetsData.PintAdd) * 1)) * (1 + 0);
                    }
                    else
                    {
                        dBPetsData.PetsHP = (44 + (1 * 3) + (dBPetsData.PetsPSTR * 4)) * (1 + 0);
                        dBPetsData.PetsMP = ((1 - 1) * 2) + ((dBPetsData.PetsPINT + dBPetsData.PintAdd) * 2) * (1 + 0);
                    }

                    if (mPetJson.Id == 100)
                        dBPetsData.PetsTrialTime = Help_TimeHelper.GetNowSecond() + mPetJson.TrialTime;
                    else
                        dBPetsData.PetsTrialTime = 0;

                    pets.GamePropertyDic[E_GameProperty.MoveSpeed] = mPetJson.MoSpeed;
                    pets.GameHeroSexType = (E_GameHeroSexType)mPetJson.AttackType;
                    pets.AfterAwake();
                    pets.SetInstanceId(dBPetsData.PetsId);
                    pets.DataAddPropertyBufferGotoMap(mBattleComponent);
                    pets.UpDataEnhanceA();
                    pets.DataUpdateProperty();
                    //int SkillID = mPetJson.SkillID;
                    var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<PetsSkillCreateBuilder>();
                    var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;
                    if (SkillJsonDic != null)
                    {
                        pets.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
                        foreach (var skill in mPetJson.SkillID)
                        {
                            if (pets.SkillGroup.ContainsKey(skill) == false)
                            {
                                var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(skill);
                                pets.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                                if (SkillJsonDic[skill].skillType == 1)
                                {
                                    if (skill == dBPetsData.UseSkillID)
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
                            }
                            dBPetsData.SkillId.Add(skill);
                        }
                    }

                    // 如果物品有没有卓越属性，就用默认的概率添加卓越
                    // 有卓越，就用物品自带的卓越属性
                    if (b_Item.data.ExcellentEntry.Count == 0)
                    {
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
                    }
                    else
                    {
                        // 使用物品上自带的卓越
                        dBPetsData.ExcellentId = b_Item.data.ExcellentEntry.ToList();
                    }

                    dBPetsData.SkillID = Help_JsonSerializeHelper.Serialize(dBPetsData.SkillId);
                    dBPetsData.Excellent = Help_JsonSerializeHelper.Serialize(dBPetsData.ExcellentId);
                }
                //pets.UpdataExcellentValue();
                pets.GamePlayer = mCurrentTemp;
                mCurrentTemp.PetsList.Add(dBPetsData.PetsId, pets);

                if (pets.dBPetsData.PetsTrialTime > 0)
                {
                    // 添加宠物试用计时组件
                    pets.AddCustomComponent<PetsTrialTimeComponent>();
                }

                var backPackDatas = dataCache.DataQuery(p => p.GameUserId == b_Player.GameUserId && p.PetsId == pets.InstanceId);

                if (backPackDatas.Count == 0)
                {
                    dataCache.DataAdd(dBPetsData);
                    await dBProxy2.Save(dBPetsData);
                }
                else
                {
                    Log.Error($"角色: {b_Player.GameUserId}名称:{b_Player.GetCustomComponent<GamePlayer>().Data.NickName}PetsConfingID: {dBPetsData.ConfigID}PetsID:{dBPetsData.PetsId}写库失败");
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1608);
                    return;
                }
            }
        }
    }
}
