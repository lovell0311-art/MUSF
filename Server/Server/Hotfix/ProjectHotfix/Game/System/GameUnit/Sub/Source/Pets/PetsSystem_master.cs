using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{



    public static partial class PetsSystem
    {

        public static void AwakeMaster(this Pets b_Component)
        {
            if (b_Component.MasterGroup == null) b_Component.MasterGroup = new Dictionary<int, C_BattleMaster>();
        }
        public static bool PetsUseItem(this Pets b_Component, int[] ItemID, int Type = 0)
        {
            if (b_Component.IsDeath) return false;
            if(b_Component.TakeMedicineInterval - Help_TimeHelper.GetNow() <= 0)
                b_Component.TakeMedicineInterval = Help_TimeHelper.GetNow()+500;
            else
                return false;

            BackpackComponent backpack = b_Component.GamePlayer.Player.GetCustomComponent<BackpackComponent>();
            if (backpack == null)
            {
                return false;
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
            if (curItem == null) return false;
            if (Type == 0)
            {
                int maxHP = b_Component.GetNumerial(E_GameProperty.PROP_HP_MAX);
                int addPer;
                int addValue;
                if (310002 == curItem.ConfigID)
                {
                    addValue = curItem.ConfigData.Value2;
                }
                else
                {
                    addPer = curItem.ConfigData.Value;
                    addValue = (int)MathF.Ceiling(maxHP * addPer * 0.01f);
                    var mCount = curItem.ConfigData.Value2 * 10 - b_Component.dBPetsData.PetsLevel * 2;
                    if (mCount > 0)
                    {
                        addValue += mCount;
                    }
                }
                b_Component.dBPetsData.PetsHP += addValue;
                if (b_Component.dBPetsData.PetsHP > maxHP)
                {
                    b_Component.dBPetsData.PetsHP = maxHP;
                }

                backpack.UseItem(curItem.ItemUID, "宠物使用物品", 1);
                return true;
                /*var itemConfig = curItem.ConfigData;
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
                mItemUseRule.Dispose();*/
            }
            else if (Type == 1)
            {
                int maxMP = b_Component.GetNumerial(E_GameProperty.PROP_MP_MAX);
                int addPer;
                int addValue;
                if (310005 == curItem.ConfigID)
                {
                    addValue = curItem.ConfigData.Value2;
                }
                else
                {
                    addPer = curItem.ConfigData.Value;
                    addValue = (int)MathF.Ceiling(maxMP * addPer * 0.01f);
                    var mCount = curItem.ConfigData.Value2 * 10 - b_Component.dBPetsData.PetsLevel;
                    if (mCount > 0)
                    {
                        addValue += mCount;
                    }
                }
                b_Component.dBPetsData.PetsMP += addValue;
                if (b_Component.dBPetsData.PetsMP > maxMP)
                {
                    b_Component.dBPetsData.PetsMP = maxMP;
                }

                backpack.UseItem(curItem.ItemUID, "宠物使用物品", 1);
                return true;
                /*int addPer = curItem.ConfigData.Value;
                int maxMP = b_Component.GetNumerial(E_GameProperty.PROP_MP_MAX);

                b_Component.dBPetsData.PetsMP += (int)MathF.Ceiling(maxMP * addPer * 0.01f);
                if (b_Component.dBPetsData.PetsMP > maxMP)
                {
                    b_Component.dBPetsData.PetsMP = maxMP;
                }
                backpack.UseItem(curItem.ItemUID, "宠物使用物品", 1);
                return true;*/
            }
            return false;
        }
        public static async Task<bool> AddExprience(this Pets b_Component, int b_AddExprience)
        {
            if (b_Component.dBPetsData.PetsLevel >= b_Component.GamePlayer.Data.Level) return false;

            b_Component.dBPetsData.PetsExp += b_AddExprience;
            bool mUpLevel = false;
            int OldLv = b_Component.dBPetsData.PetsLevel;
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var mRoleExprienceDic = mReadConfigComponent.GetJson<Pets_ExpConfigJson>().JsonDic;
            do
            {
                if (mRoleExprienceDic.TryGetValue(b_Component.dBPetsData.PetsLevel, out var mExperienceConfig))
                {
                    if (b_Component.dBPetsData.PetsExp >= mExperienceConfig.Exprience)
                    {
                        b_Component.dBPetsData.PetsExp -= mExperienceConfig.Exprience;
                        b_Component.dBPetsData.PetsLevel++;
                        b_Component.dBPetsData.AttributePoint += b_Component.Config.AppendLevel;
                        mUpLevel = true;
                        //b_Component.GamePlayer.PetsDleExcellent(b_Component);
                    }
                    else
                        break;
                }
                else
                    break;
            } while (true);

            if(mUpLevel)
            {
                b_Component.DataUpdateProperty(1);
                b_Component.UpdataExcellentValue(OldLv);
                C2G_PetcLevelUpdataMessage c2G_PetcLevelUpdataMessage = new C2G_PetcLevelUpdataMessage();
                b_Component.GamePlayer.Player.Send(c2G_PetcLevelUpdataMessage);
            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.GamePlayer.Player.GameAreaId);
            var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get((int)b_Component.GamePlayer.Player.GameAreaId);
            mWriteDataComponent.Save(b_Component.dBPetsData, dBProxy).Coroutine();
            return true;
            /*G2C_ChangeValue_notice mExprienceNotice = new G2C_ChangeValue_notice();
            G2C_BattleKVData mExpMessage = new G2C_BattleKVData();
            mExpMessage.Key = (int)E_GameProperty.ExprienceDrop;
            mExpMessage.Value = b_AddExprience;
            mExprienceNotice.Info.Add(mExpMessage);
            mExpMessage = new G2C_BattleKVData();
            mExpMessage.Key = (int)E_GameProperty.Exprience;
            mExpMessage.Value = b_Component.dBPetsData.PetsExp;
            mExprienceNotice.Info.Add(mExpMessage);
            if (mUpLevel)
            {
                mExpMessage = new G2C_BattleKVData();
                mExpMessage.Key = (int)E_GameProperty.Level;
                mExpMessage.Value = b_Component.dBPetsData.PetsLevel;
                mExprienceNotice.Info.Add(mExpMessage);
            }
            b_Component.Player.Send(mExprienceNotice);*/
        }

        public static void KillReply(this Pets b_Component, BattleComponent b_BattleComponent)
        {
            if (b_Component != null && !b_Component.IsDeath)
            {
                var mKillEnemyReplyHp = b_Component.GetNumerialFunc(E_GameProperty.PROP_HP_MAX) / 8;
                mKillEnemyReplyHp *= b_Component.GetNumerialFunc(E_GameProperty.KillMonsterReplyHp_8);
                if (mKillEnemyReplyHp > 0)
                {
                    b_Component.dBPetsData.PetsHP += mKillEnemyReplyHp;
                    var mHpMax = b_Component.GetNumerialFunc(E_GameProperty.PROP_HP_MAX);
                    if (b_Component.dBPetsData.PetsHP > mHpMax)
                        b_Component.dBPetsData.PetsHP = mHpMax;
                }
                var mKillEnemyReplyMp = b_Component.GetNumerialFunc(E_GameProperty.PROP_MP_MAX) / 8;
                mKillEnemyReplyMp *= b_Component.GetNumerialFunc(E_GameProperty.KillMonsterReplyMp_8);
                if (mKillEnemyReplyMp > 0)
                {
                    b_Component.dBPetsData.PetsMP += mKillEnemyReplyMp;
                    var mMpMax = b_Component.GetNumerialFunc(E_GameProperty.PROP_MP_MAX);
                    if (b_Component.dBPetsData.PetsMP > mMpMax)
                        b_Component.dBPetsData.PetsMP = mMpMax;
                }
            }
        }
    }
}