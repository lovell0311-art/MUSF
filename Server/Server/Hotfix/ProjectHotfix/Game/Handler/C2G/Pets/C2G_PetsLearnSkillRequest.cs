using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_PetsLearnSkillRequestHandler : AMActorRpcHandler<C2G_PetsLearnSkillRequest, G2C_PetsLearnSkillResponse>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_PetsLearnSkillRequest b_Request, G2C_PetsLearnSkillResponse b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }
        protected override async Task<bool> Run(C2G_PetsLearnSkillRequest b_Request, G2C_PetsLearnSkillResponse b_Response, Action<IMessage> b_Reply)
        {
            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("不存在!");
                b_Reply(b_Response);
                return false;
            }
            var bknapsack = mPlayer.GetCustomComponent<BackpackComponent>();
            if (bknapsack == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                b_Reply(b_Response);
                return false;
            }

            var mGamePalyer = mPlayer.GetCustomComponent<GamePlayer>();
            if (mGamePalyer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1601);
                b_Reply(b_Response);
                return false;
            }

            void PetsLearnSkill(Pets pets, int SkillID)
            {
                var mSkillCreateBuilder = Root.MainFactory.GetCustomComponent<PetsSkillCreateBuilder>();
                //var mBattleMasterCreateBuilder = Root.MainFactory.GetCustomComponent<BattleMasterCreateBuilder>();
                var SkillJsonDic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Pets_SkillConfigJson>().JsonDic;
                if (SkillJsonDic != null)
                {
                    if (SkillJsonDic.TryGetValue(SkillID, out var SkillJson))
                    {
                        if (pets.SkillGroup.ContainsKey(SkillID) == false)
                        {
                            var mSkillInstance = mSkillCreateBuilder.CreateHeroSKill(SkillID);
                            pets.SkillGroup[mSkillInstance.Id] = mSkillInstance;
                            if (SkillJson.skillType == 1)
                            {
                                pets.SkillGroup[mSkillInstance.Id] = mSkillInstance;
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

            if (bknapsack.mItemDict.TryGetValue(b_Request.Skill.ItemID, out var ItemInfo) != false)
            {
                bool IsUseOK = false;
                var SkillId = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Item_SkillBooksConfigJson>().JsonDic;
                if (SkillId != null)
                {
                    if (SkillId.TryGetValue(ItemInfo.ConfigID, out Item_SkillBooksConfig Value) != false)
                    {
                        if (mGamePalyer.Pets != null && mGamePalyer.Pets.dBPetsData.PetsId == b_Request.PetsID)
                        {
                            if (Value.ValueDic.TryGetValue(mGamePalyer.Pets.dBPetsData.ConfigID, out int SKillID) != false)
                            {
                                mGamePalyer.Pets.dBPetsData.SkillId.Add(SKillID);
                                PetsLearnSkill(mGamePalyer.Pets, SKillID);
                                mGamePalyer.Pets.dBPetsData.SkillID = Help_JsonSerializeHelper.Serialize(mGamePalyer.Pets.dBPetsData.SkillId);

                                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                                mWriteDataComponent.Save(mGamePalyer.Pets.dBPetsData, dBProxy).Coroutine();
                                IsUseOK = true;
                            }
                        }
                        else if (mGamePalyer.PetsList.TryGetValue(b_Request.PetsID, out var PetsInfo) != false)
                        {
                            if (Value.ValueDic.TryGetValue(PetsInfo.dBPetsData.ConfigID, out int SKillID) != false)
                            {
                                PetsInfo.dBPetsData.SkillId.Add(SKillID);
                                PetsLearnSkill(PetsInfo, SKillID);
                                PetsInfo.dBPetsData.SkillID = Help_JsonSerializeHelper.Serialize(PetsInfo.dBPetsData.SkillId);

                                DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
                                var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)mPlayer.GameAreaId);
                                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)mPlayer.GameAreaId);
                                mWriteDataComponent.Save(PetsInfo.dBPetsData, dBProxy).Coroutine();
                                IsUseOK = true;
                            }
                        }
                        else
                        {
                            b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1604);
                            b_Reply(b_Response);
                            return false;
                        }

                    }

                }

                if (IsUseOK)
                {
                    if (ItemInfo.GetProp(EItemValue.Quantity) >= 2)
                    {
                        //物品数量减少1，广播物品属性改变
                        bknapsack.UseItem(ItemInfo, "宠物增加经验", 1);
                    }
                    else
                    {
                        //物品用完，删除物品
                        bknapsack.DeleteItem(ItemInfo, "宠物增加经验");
                    }
                }
                else
                {
                    b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(1602);
                    b_Reply(b_Response);
                    return false;
                }

            }

            b_Reply(b_Response);
            return true;
        }
    }
}