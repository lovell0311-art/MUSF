using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Mps.V20190612.Models;

#if DEVELOP
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_InsertPetsRequestHandler : AMActorRpcHandler<C2G_InsertPetsRequest, G2C_InsertPetsResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_InsertPetsRequest b_Request, G2C_InsertPetsResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_InsertPetsRequest b_Request, G2C_InsertPetsResponse b_Response, Action<IMessage> b_Reply)
        {
            b_Reply(b_Response);
            return true;
        }
        //宠物变成装备
        //{
        //    int mAreaId = (int)(b_Request.AppendData >> 16);
        //    Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
        //    if (mPlayer == null)
        //    {
        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
        //        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
        //        b_Reply(b_Response);
        //        return false;
        //    }

        //    var mGamePalyer = mPlayer.GetCustomComponent<GamePlayer>();
        //    if (mGamePalyer == null)
        //    {
        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
        //        b_Reply(b_Response);
        //        return false;
        //    }

        //    BattleComponent mBattleComponent = null;
        //    MapComponent mapComponent = mGamePalyer.CurrentMap;
        //    if (mapComponent != null)
        //    {
        //        mBattleComponent = mapComponent.GetCustomComponent<BattleComponent>();
        //    }

        //    var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
        //    if (mJsonDic.TryGetValue(b_Request.PetsConfigID, out var mPetJson) == false)
        //    {
        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1608);
        //        b_Reply(b_Response);
        //        return false;
        //    }

        //    Pets pets = Root.CreateBuilder.GetInstance<Pets>();
        //    DBPetsData dBPetsData = new DBPetsData();

        //    pets.GamePlayer = mGamePalyer;
        //    pets.IsDeath = false;
        //    pets.DeathSleepTime = 0;
        //    pets.Identity = E_Identity.Pet;
        //    pets.SetConfig(dBPetsData, mPetJson);
        //    pets.GameHeroSexType = (E_GameHeroSexType)mPetJson.PetsType;
        //    {
        //        dBPetsData.Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
        //        dBPetsData.GameUserId = mPlayer.GameUserId;
        //        dBPetsData.ConfigID = b_Request.PetsConfigID;
        //        dBPetsData.PetsId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
        //        dBPetsData.PetsName = mPetJson.Name;
        //        dBPetsData.PetsLevel = 1;
        //        dBPetsData.PetsUseState = 0;
        //        dBPetsData.PetsExp = 0;
        //        dBPetsData.DeathTime = 0;
        //        dBPetsData.AttributePoint = 0;
        //        dBPetsData.PetsSTR = mPetJson.Strength;
        //        dBPetsData.PetsDEX = mPetJson.Agility;
        //        dBPetsData.PetsPSTR = mPetJson.BoneGas;
        //        dBPetsData.PetsPINT = mPetJson.Willpower;
        //        dBPetsData.IsDisabled = 0;
        //        dBPetsData.UseSkillID = 0;
        //        if (mPetJson.AttackType == 0)
        //        {
        //            dBPetsData.PetsHP = (33 + (1 * 2) + (dBPetsData.PetsPSTR * 3)) * (1 + 0);
        //            dBPetsData.PetsMP = (int)(10 + ((1 - 1) * 0.5) + ((dBPetsData.PetsPINT + dBPetsData.PintAdd) * 1)) * (1 + 0);
        //        }
        //        else
        //        {
        //            dBPetsData.PetsHP = (44 + (1 * 1) + (dBPetsData.PetsPSTR * 1)) * (1 + 0);
        //            dBPetsData.PetsMP = ((1 - 1) * 2) + ((dBPetsData.PetsPINT + dBPetsData.PintAdd) * 2) * (1 + 0);
        //        }

        //        if (b_Request.PetsConfigID == 100)
        //            dBPetsData.PetsTrialTime = Help_TimeHelper.GetNowSecond() + mPetJson.TrialTime;
        //        else
        //            dBPetsData.PetsTrialTime = 0;

        //        pets.GamePropertyDic[E_GameProperty.MoveSpeed] = mPetJson.MoSpeed;
        //        pets.GameHeroSexType = (E_GameHeroSexType)mPetJson.AttackType;
        //        pets.SetInstanceId(dBPetsData.PetsId);
        //        //int SkillID = mPetJson.SkillID;
        //        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<PetsSkillCreateBuilder>();
        //        //var mBattleMasterCreateBuilder = Root.MainFactory.GetCustomComponent<BattleMasterCreateBuilder>();
        //        var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;
        //        if (SkillJsonDic != null)
        //        {
        //            pets.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
        //            foreach (var skill in mPetJson.SkillID)
        //            {
        //                dBPetsData.SkillId.Add(skill);
        //            }
        //        }

        //        var excAttrEntryManager = Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
        //        if (excAttrEntryManager.TryGetPetsAttrEntry(out var selector))
        //        {
        //            if (excAttrEntryManager.PetsExcAttrEntryCount.TryGetValue(out int count))
        //            {
        //                var newSelector = new RandomSelector<int>(selector);
        //                do
        //                {
        //                    if (newSelector.TryGetValueAndRemove(out var entryId))
        //                    {
        //                        dBPetsData.ExcellentId.Add(entryId);
        //                    }
        //                    else
        //                    {
        //                        // 词条取空了
        //                        break;
        //                    }
        //                    count--;
        //                }
        //                while (count > 0);
        //            }
        //        }
        //        dBPetsData.SkillID = Help_JsonSerializeHelper.Serialize(dBPetsData.SkillId);
        //        dBPetsData.Excellent = Help_JsonSerializeHelper.Serialize(dBPetsData.ExcellentId);
        //    }
        //    pets.GamePlayer = mGamePalyer;
        //    mGamePalyer.PetsList.Add(dBPetsData.PetsId, pets);

        //    if (pets.dBPetsData.PetsTrialTime > 0)
        //    {
        //        // 添加宠物试用计时组件
        //        pets.AddCustomComponent<PetsTrialTimeComponent>();
        //    }

        //    DataCacheManageComponent mDataCacheManageComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();
        //    var dataCache = mDataCacheManageComponent.Get<DBPetsData>();
        //    var backPackDatas = dataCache.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.PetsId == pets.InstanceId);

        //    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
        //    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);

        //    if (backPackDatas.Count == 0)
        //    {
        //        dataCache.DataAdd(dBPetsData);
        //        await dBProxy.Save(dBPetsData);
        //    }
        //    else
        //    {
        //        Log.Error($"角色: {mPlayer.GameUserId}名称:{mPlayer.GetCustomComponent<GamePlayer>().Data.NickName}PetsConfingID: {dBPetsData.ConfigID}PetsID:{dBPetsData.PetsId}写库失败");
        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1608);
        //        b_Reply(b_Response);
        //        return false;
        //    }

        //    b_Response.Info = new PetsInfo();
        //    b_Response.Info = pets.GetPetsInfo(out bool SetDB);
        //    b_Reply(b_Response);
        //    return true;
        //}
        #region 宠物改版
        //{
        //    int mAreaId = (int)(b_Request.AppendData >> 16);
        //    Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
        //    if (mPlayer == null)
        //    {
        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
        //        //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
        //        b_Reply(b_Response);
        //        return false;
        //    }

        //    var mGamePalyer = mPlayer.GetCustomComponent<GamePlayer>();
        //    if (mGamePalyer == null)
        //    {
        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
        //        b_Reply(b_Response);
        //        return false;
        //    }

        //    BattleComponent mBattleComponent = null;
        //    MapComponent mapComponent = mGamePalyer.CurrentMap;
        //    if (mapComponent != null)
        //    {
        //        mBattleComponent = mapComponent.GetCustomComponent<BattleComponent>();
        //    }

        //    var mJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_InfoConfigJson>().JsonDic;
        //    if (mJsonDic.TryGetValue(b_Request.PetsConfigID, out var mPetJson) == false)
        //    {
        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1608);
        //        b_Reply(b_Response);
        //        return false;
        //    }

        //    Pets pets = Root.CreateBuilder.GetInstance<Pets>();
        //    DBPetsData dBPetsData = new DBPetsData();

        //    pets.GamePlayer = mGamePalyer;
        //    pets.IsDeath = false;
        //    pets.DeathSleepTime = 0;
        //    pets.Identity = E_Identity.Pet;
        //    pets.SetConfig(dBPetsData, mPetJson);
        //    pets.GameHeroSexType = (E_GameHeroSexType)mPetJson.PetsType;
        //    {
        //        dBPetsData.Id = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
        //        dBPetsData.GameUserId = mPlayer.GameUserId;
        //        dBPetsData.ConfigID = b_Request.PetsConfigID;
        //        dBPetsData.PetsId = IdGeneraterNew.Instance.GenerateUnitId(mPlayer.GameAreaId);
        //        dBPetsData.PetsName = mPetJson.Name;
        //        dBPetsData.PetsLevel = 1;
        //        dBPetsData.PetsUseState = 0;
        //        dBPetsData.PetsExp = 0;
        //        dBPetsData.DeathTime = 0;
        //        dBPetsData.AttributePoint = 0;
        //        dBPetsData.PetsSTR = mPetJson.Strength;
        //        dBPetsData.PetsDEX = mPetJson.Agility;
        //        dBPetsData.PetsPSTR = mPetJson.BoneGas;
        //        dBPetsData.PetsPINT = mPetJson.Willpower;
        //        dBPetsData.IsDisabled = 0;
        //        dBPetsData.UseSkillID = 0;
        //        if (mPetJson.AttackType == 0)
        //        {
        //            dBPetsData.PetsHP = (33 + (1 * 2) + (dBPetsData.PetsPSTR * 3)) * (1 + 0);
        //            dBPetsData.PetsMP = (int)(10 + ((1 - 1) * 0.5) + ((dBPetsData.PetsPINT + dBPetsData.PintAdd) * 1)) * (1 + 0);
        //        }
        //        else
        //        {
        //            dBPetsData.PetsHP = (44 + (1 * 1) + (dBPetsData.PetsPSTR * 1)) * (1 + 0);
        //            dBPetsData.PetsMP = ((1 - 1) * 2) + ((dBPetsData.PetsPINT + dBPetsData.PintAdd) * 2) * (1 + 0);
        //        }

        //        if (b_Request.PetsConfigID == 100)
        //            dBPetsData.PetsTrialTime = Help_TimeHelper.GetNowSecond() + mPetJson.TrialTime;
        //        else
        //            dBPetsData.PetsTrialTime = 0;

        //        pets.GamePropertyDic[E_GameProperty.MoveSpeed] = mPetJson.MoSpeed;
        //        pets.GameHeroSexType = (E_GameHeroSexType)mPetJson.AttackType;
        //        pets.AfterAwake();
        //        pets.SetInstanceId(dBPetsData.PetsId);
        //        pets.DataAddPropertyBuffer();
        //        pets.DataUpdateProperty();
        //        //int SkillID = mPetJson.SkillID;
        //        var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<PetsSkillCreateBuilder>();
        //        //var mBattleMasterCreateBuilder = Root.MainFactory.GetCustomComponent<BattleMasterCreateBuilder>();
        //        var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;
        //        if (SkillJsonDic != null)
        //        {
        //            pets.SkillGroup = new Dictionary<int, C_HeroSkillSource>();
        //            foreach (var skill in mPetJson.SkillID)
        //            {
        //                if (pets.SkillGroup.ContainsKey(skill) == false)
        //                {
        //                    var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(skill);
        //                    pets.SkillGroup[mSkillInstance.Id] = mSkillInstance;
        //                    if (SkillJsonDic[skill].skillType == 1)
        //                    {
        //                        if (skill == dBPetsData.UseSkillID)
        //                        {
        //                            pets.SkillCurrent = mSkillInstance;
        //                        }
        //                    }
        //                    else if (SkillJsonDic[skill].skillType == 2)
        //                    {
        //                        bool mUseResult = mSkillInstance.TryUse(pets, pets, null, null, null);
        //                        if (mUseResult)
        //                        {


        //                            mSkillInstance.UseSkill(pets, pets, mBattleComponent);
        //                        }
        //                    }
        //                }
        //                dBPetsData.SkillId.Add(skill);
        //            }
        //        }

        //        var excAttrEntryManager = Root.MainFactory.GetCustomComponent<ExcAttrEntryManagerComponent>();
        //        if (excAttrEntryManager.TryGetPetsAttrEntry(out var selector))
        //        {
        //            if (excAttrEntryManager.PetsExcAttrEntryCount.TryGetValue(out int count))
        //            {
        //                var newSelector = new RandomSelector<int>(selector);
        //                do
        //                {
        //                    if (newSelector.TryGetValueAndRemove(out var entryId))
        //                    {
        //                        dBPetsData.ExcellentId.Add(entryId);
        //                    }
        //                    else
        //                    {
        //                        // 词条取空了
        //                        break;
        //                    }
        //                    count--;
        //                }
        //                while (count > 0);
        //            }
        //        }
        //        dBPetsData.SkillID = Help_JsonSerializeHelper.Serialize(dBPetsData.SkillId);
        //        dBPetsData.Excellent = Help_JsonSerializeHelper.Serialize(dBPetsData.ExcellentId);
        //    }
        //    //pets.AfterAwake(dBPetsData);
        //    //pets.SetInstanceId(dBPetsData.PetsId);
        //    //pets.DataAddPropertyBuffer();
        //    //pets.DataUpdateProperty();
        //    pets.UpdataExcellentValue();
        //    pets.GamePlayer = mGamePalyer;
        //    mGamePalyer.PetsList.Add(dBPetsData.PetsId, pets);

        //    if (pets.dBPetsData.PetsTrialTime > 0)
        //    {
        //        // 添加宠物试用计时组件
        //        pets.AddCustomComponent<PetsTrialTimeComponent>();
        //    }

        //    DataCacheManageComponent mDataCacheManageComponent = mPlayer.AddCustomComponent<DataCacheManageComponent>();
        //    var dataCache = mDataCacheManageComponent.Get<DBPetsData>();
        //    var backPackDatas = dataCache.DataQuery(p => p.GameUserId == mPlayer.GameUserId && p.PetsId == pets.InstanceId);

        //    DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
        //    var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);

        //    if (backPackDatas.Count == 0)
        //    {
        //        dataCache.DataAdd(dBPetsData);
        //        await dBProxy.Save(dBPetsData);
        //    }
        //    else
        //    {
        //        Log.Error($"角色: {mPlayer.GameUserId}名称:{mPlayer.GetCustomComponent<GamePlayer>().Data.NickName}PetsConfingID: {dBPetsData.ConfigID}PetsID:{dBPetsData.PetsId}写库失败");
        //        b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1608);
        //        b_Reply(b_Response);
        //        return false;
        //    }

        //    b_Response.Info = new PetsInfo();
        //    b_Response.Info = pets.GetPetsInfo(out bool SetDB);
        //    b_Reply(b_Response);
        //    return true;
        //}
        #endregion
    }
}
#endif