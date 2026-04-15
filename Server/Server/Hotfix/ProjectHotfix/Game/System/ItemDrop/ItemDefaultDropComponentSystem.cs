using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using ETModel;

namespace ETHotfix
{
    [EventMethod(typeof(ItemDefaultDropComponent), EventSystemType.INIT)]
    public class ItemDefaultDropComponentInitSystem : ITEventMethodOnInit<ItemDefaultDropComponent>
    {
        public void OnInit(ItemDefaultDropComponent self)
        {
            ItemDefaultDrop[] itemDrop = self.ItemDrop;

            for (int i = 0; i < itemDrop.Length; ++i)
            {
                itemDrop[i].Equip = new RandomSelector<ItemDropInfo>();
                itemDrop[i].Necklace = new RandomSelector<ItemDropInfo>();
                itemDrop[i].Ring = new RandomSelector<ItemDropInfo>();
                itemDrop[i].SkillBook = new RandomSelector<ItemDropInfo>();
                itemDrop[i].Consumables = new RandomSelector<ItemDropInfo>();
            }

            self.OnInit();
        }
    }

    [EventMethod(typeof(ItemDefaultDropComponent), EventSystemType.LOAD)]
    public class ItemDefaultDropComponentLoadSystem : ITEventMethodOnLoad<ItemDefaultDropComponent>
    {
        public override void OnLoad(ItemDefaultDropComponent self)
        {
            ItemDefaultDrop[] itemDrop = self.ItemDrop;

            for (int i = 0; i < itemDrop.Length; ++i)
            {
                itemDrop[i].Equip.Clear();
                itemDrop[i].Necklace.Clear();
                itemDrop[i].Ring.Clear();
                itemDrop[i].SkillBook.Clear();
                itemDrop[i].Consumables.Clear();
            }
            self.DefaultDropStrengthenLevel.Clear();

            self.OnLoad();
        }
    }

    public static class ItemDefaultDropComponentSystem
    {
        public static void OnInit(this ItemDefaultDropComponent self)
        {
            long startTime = Help_TimeHelper.GetNow();

            self.OnLoad();

            long endTime = Help_TimeHelper.GetNow();

            Log.Debug($"初始化物品默认掉落，time = {endTime - startTime}");
        }

        public static void OnLoad(this ItemDefaultDropComponent self)
        {
            // 技能掉落等级差
            int skillDropLevel = 0;
            // 幸运掉落等级差
            int luckyDropLevel = 0;
            // 卓越掉落等级差
            int excDropLevel = 26;
            // 套装掉落等级差
            int setDropLevel = 36;
            // 镶嵌掉落等级差
            int socketDropLevel = 0;

            // 掉落等级范围
            int dropLevelRange = 8;

            ItemDefaultDrop[] itemDrop = self.ItemDrop;
            int[] addedNoDropItemId = new int[ItemDefaultDropComponent.MAX_MONSTER_LEVEL];
            Array.Clear(addedNoDropItemId, 0, addedNoDropItemId.Length);

            #region 装备掉落
            void AddToItemDrop_Equip(int dropLevel, ItemDropInfo dropInfo, int weight)
            {
                int minLevel = dropLevel - dropLevelRange;
                if (minLevel < 1) minLevel = 1;
                int maxLevel = dropLevel + dropLevelRange;
                if (minLevel > ItemDefaultDropComponent.MAX_MONSTER_LEVEL) minLevel = ItemDefaultDropComponent.MAX_MONSTER_LEVEL;

                for (int i = minLevel; i < maxLevel; ++i)
                {
                    itemDrop[i].Equip.Add(dropInfo, weight);
                }
            }

            foreach (Item_EquipmentConfig config in Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Item_EquipmentConfigJson>().JsonDic.Values)
            {
                if (config.Drop != 1) continue; // << 这个物品没开启掉落
                if (config.NormalDropWeight > 0)
                {
                    // 普通装备
                    AddToItemDrop_Equip(config.Level, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Normal }, config.NormalDropWeight);
                }
                if (config.AppendDropWeight > 0)
                {
                    // 追加装备
                    AddToItemDrop_Equip(config.Level, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Append }, config.AppendDropWeight);
                }
                if (config.SkillDropWeight > 0)
                {
                    // 普通+技能装备
                    AddToItemDrop_Equip(config.Level + skillDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Skill }, config.SkillDropWeight);
                }
                if (config.LuckyDropWeight > 0)
                {
                    // 幸运装备
                    AddToItemDrop_Equip(config.Level + luckyDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Lucky }, config.LuckyDropWeight);
                }
                if (config.ExcellentDropWeight > 0)
                {
                    // 卓越装备
                    AddToItemDrop_Equip(config.Level + excDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Excellent }, config.ExcellentDropWeight);
                    if(config.Level > 100) AddToItemDrop_Equip(config.Level + excDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.NoDrop }, config.NormalDropWeight);
                }
                if (config.SetDropWeight > 0)
                {
                    // 套装装备
                    AddToItemDrop_Equip(config.Level + setDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Set }, config.SetDropWeight);
                    if (config.Level > 100) AddToItemDrop_Equip(config.Level + setDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.NoDrop }, config.NormalDropWeight);
                }
                if (config.SocketDropWeight > 0)
                {
                    // 镶嵌装备
                    AddToItemDrop_Equip(config.Level + socketDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Socket }, config.SocketDropWeight);
                    if (config.Level > 100) AddToItemDrop_Equip(config.Level + socketDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.NoDrop }, config.NormalDropWeight);
                }
            }
            #endregion

            #region 项链掉落
            void AddToItemDrop_Necklace(int dropLevel, ItemDropInfo dropInfo, int weight)
            {
                int minLevel = dropLevel - dropLevelRange;
                if (minLevel < 1) minLevel = 1;
                int maxLevel = dropLevel + dropLevelRange;
                if (minLevel >= ItemDefaultDropComponent.MAX_MONSTER_LEVEL) minLevel = ItemDefaultDropComponent.MAX_MONSTER_LEVEL;

                for (int i = minLevel; i < maxLevel; ++i)
                {
                    if(dropInfo.DropType != ItemDropType.NoDrop || addedNoDropItemId[i] != dropInfo.ItemConfigId)
                        itemDrop[i].Necklace.Add(dropInfo, weight);
                }
                switch (dropInfo.DropType)
                {
                    case ItemDropType.NoDrop:
                    case ItemDropType.Normal:
                        {
                            for (int i = minLevel; i < maxLevel; ++i)
                            {
                                addedNoDropItemId[i] = dropInfo.ItemConfigId;
                            }
                        }
                        break;
                }
            }

            foreach (Item_NecklaceConfig config in Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Item_NecklaceConfigJson>().JsonDic.Values)
            {
                if (config.Drop != 1) continue; // << 这个物品没开启掉落
                if (config.NormalDropWeight > 0)
                {
                    // 普通装备
                    AddToItemDrop_Necklace(config.Level, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Normal }, config.NormalDropWeight);
                }
                if (config.AppendDropWeight > 0)
                {
                    // 追加装备
                    AddToItemDrop_Necklace(config.Level, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Append }, config.AppendDropWeight);
                }
                if (config.ExcellentDropWeight > 0)
                {
                    // 卓越装备
                    AddToItemDrop_Necklace(config.Level + excDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Excellent }, config.ExcellentDropWeight);
                    AddToItemDrop_Necklace(config.Level + excDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.NoDrop }, config.NormalDropWeight);
                }
                if (config.SetDropWeight > 0)
                {
                    // 套装装备
                    AddToItemDrop_Necklace(config.Level + setDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Set }, config.SetDropWeight);
                    AddToItemDrop_Necklace(config.Level + setDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.NoDrop }, config.NormalDropWeight);
                }
                if (config.SocketDropWeight > 0)
                {
                    // 镶嵌装备
                    AddToItemDrop_Necklace(config.Level + socketDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Socket }, config.SocketDropWeight);
                    AddToItemDrop_Necklace(config.Level + socketDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.NoDrop }, config.NormalDropWeight);
                }
            }
            #endregion

            #region 戒指掉落
            void AddToItemDrop_Ring(int dropLevel, ItemDropInfo dropInfo, int weight)
            {
                int minLevel = dropLevel - dropLevelRange;
                if (minLevel < 1) minLevel = 1;
                int maxLevel = dropLevel + dropLevelRange;
                if (minLevel > ItemDefaultDropComponent.MAX_MONSTER_LEVEL) minLevel = ItemDefaultDropComponent.MAX_MONSTER_LEVEL;

                for (int i = minLevel; i < maxLevel; ++i)
                {
                    if (dropInfo.DropType != ItemDropType.NoDrop || addedNoDropItemId[i] != dropInfo.ItemConfigId)
                        itemDrop[i].Ring.Add(dropInfo, weight);
                }
                switch (dropInfo.DropType)
                {
                    case ItemDropType.NoDrop:
                    case ItemDropType.Normal:
                        {
                            for (int i = minLevel; i < maxLevel; ++i)
                            {
                                addedNoDropItemId[i] = dropInfo.ItemConfigId;
                            }
                        }
                        break;
                }
            }

            foreach (Item_RingsConfig config in Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Item_RingsConfigJson>().JsonDic.Values)
            {
                if (config.Drop != 1) continue; // << 这个物品没开启掉落
                if (config.NormalDropWeight > 0)
                {
                    // 普通装备
                    AddToItemDrop_Ring(config.Level, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Normal }, config.NormalDropWeight);
                }
                if (config.AppendDropWeight > 0)
                {
                    // 追加装备
                    AddToItemDrop_Ring(config.Level, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Append }, config.AppendDropWeight);
                }
                if (config.ExcellentDropWeight > 0)
                {
                    // 卓越装备
                    AddToItemDrop_Ring(config.Level + excDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Excellent }, config.ExcellentDropWeight);
                    AddToItemDrop_Ring(config.Level + excDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.NoDrop }, config.NormalDropWeight);
                }
                if (config.SetDropWeight > 0)
                {
                    // 套装装备
                    AddToItemDrop_Ring(config.Level + setDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Set }, config.SetDropWeight);
                    AddToItemDrop_Ring(config.Level + setDropLevel, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.NoDrop }, config.NormalDropWeight);
                }
            }
            #endregion

            #region 技能书掉落
            void AddToItemDrop_SkillBook(int dropLevel, ItemDropInfo dropInfo, int weight)
            {
                int minLevel = dropLevel - dropLevelRange;
                if (minLevel < 1) minLevel = 1;
                int maxLevel = dropLevel + dropLevelRange;
                if (minLevel > ItemDefaultDropComponent.MAX_MONSTER_LEVEL) minLevel = ItemDefaultDropComponent.MAX_MONSTER_LEVEL;

                for (int i = minLevel; i < maxLevel; ++i)
                {
                    itemDrop[i].SkillBook.Add(dropInfo, weight);
                }
            }

            foreach (Item_SkillBooksConfig config in Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Item_SkillBooksConfigJson>().JsonDic.Values)
            {
                if (config.Drop != 1) continue; // << 这个物品没开启掉落
                if (config.NormalDropWeight > 0)
                {
                    // 普通装备
                    AddToItemDrop_SkillBook(config.Level, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Normal }, config.NormalDropWeight);
                }
            }
            #endregion

            #region 消耗品掉落
            void AddToItemDrop_Consumables(int dropLevel, ItemDropInfo dropInfo, int weight)
            {
                int minLevel = dropLevel - dropLevelRange;
                if (minLevel < 1) minLevel = 1;
                int maxLevel = dropLevel + dropLevelRange;
                if (minLevel > ItemDefaultDropComponent.MAX_MONSTER_LEVEL) minLevel = ItemDefaultDropComponent.MAX_MONSTER_LEVEL;

                for (int i = minLevel; i < maxLevel; ++i)
                {
                    itemDrop[i].Consumables.Add(dropInfo, weight);
                }
            }

            foreach (Item_ConsumablesConfig config in Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Item_ConsumablesConfigJson>().JsonDic.Values)
            {
                if (config.Drop != 1) continue; // << 这个物品没开启掉落
                if (config.NormalDropWeight > 0)
                {
                    // 普通装备
                    AddToItemDrop_Consumables(config.Level, new ItemDropInfo() { ItemConfigId = config.Id, DropType = ItemDropType.Normal }, config.NormalDropWeight);
                }
            }
            #endregion



            #region 强化等级掉落概率
            foreach(ItemDrop_StrengthenConfig config in Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemDrop_StrengthenConfigJson>().JsonDic.Values)
            {
                self.DefaultDropStrengthenLevel.Add(config.Level, config.Weight);
            }
            #endregion
        }

    }
}
