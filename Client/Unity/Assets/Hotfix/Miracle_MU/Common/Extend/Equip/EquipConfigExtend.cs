
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 装备 配置表 拓展类
    /// </summary>
    public static class EquipConfigExtend
    {
        /// <summary>
        /// 物品配置表 扩展类
        /// </summary>
        public static IConfig GetItemConfig(this long self) => (self / 10000) switch
        {
            22 => ConfigComponent.Instance.GetItem<Item_WingConfig>((int)self),//翅膀
            23 => ConfigComponent.Instance.GetItem<Item_NecklaceConfig>((int)self),//项链
            24 => ConfigComponent.Instance.GetItem<Item_RingsConfig>((int)self),//戒指
            25 => ConfigComponent.Instance.GetItem<Item_DanglerConfig>((int)self),//项链
            26 => ConfigComponent.Instance.GetItem<Item_MountsConfig>((int)self),//坐骑
            27 => ConfigComponent.Instance.GetItem<Item_FGemstoneConfig>((int)self),//荧光宝石
            28 => ConfigComponent.Instance.GetItem<Item_GemstoneConfig>((int)self),//宝石
            29 => ConfigComponent.Instance.GetItem<Item_SkillBooksConfig>((int)self),//技能书
            30 => ConfigComponent.Instance.GetItem<Item_GuardConfig>((int)self),//守护
            31 => ConfigComponent.Instance.GetItem<Item_ConsumablesConfig>((int)self),//消耗品（血瓶、药水、实力提升卷轴）
            32 => ConfigComponent.Instance.GetItem<Item_OtherConfig>((int)self),//其他
            33 => ConfigComponent.Instance.GetItem<Item_TaskConfig>((int)self),//任务
            34 => ConfigComponent.Instance.GetItem<Item_FlagConfig>((int)self),//旗帜
            35 => ConfigComponent.Instance.GetItem<Item_PetConfig>((int)self),//宠物
            36 => ConfigComponent.Instance.GetItem<Item_BraceletConfig>((int)self),//手环
            _ => ConfigComponent.Instance.GetItem<Item_EquipmentConfig>((int)self)//装备
        };


        public static void GetItemInfo_Ref(this long self, ref Item_infoConfig item_Info)
        {
            if (self.GetItemConfig() is Item_EquipmentConfig item_Equipment)//装备
            {
                item_Info.Id = item_Equipment.Id;

                item_Info.AppendAttrId = item_Equipment.AppendAttrId;
                item_Info.Is400 = item_Equipment.Is400;
                item_Info.Curse = item_Equipment.Curse;
                item_Info.UpPet = item_Equipment.UpPet;
                item_Info.Name = item_Equipment.Name;
                item_Info.ResName = item_Equipment.ResName;
                item_Info.Slot = item_Equipment.Slot;
                item_Info.Type = (int)item_Equipment.Id / 10000;
                item_Info.Skill = item_Equipment.Skill;
                item_Info.X = item_Equipment.X;
                item_Info.Y = item_Equipment.Y;
                item_Info.StackSize = item_Equipment.StackSize;
                item_Info.Drop = item_Equipment.Drop;
                item_Info.TwoHand = item_Equipment.TwoHand;
                item_Info.DropLevel = item_Equipment.Level;
                item_Info.DamageMin = item_Equipment.DamageMin;
                item_Info.DamageMax = item_Equipment.DamageMax;
                item_Info.MagicPct = item_Equipment.MagicPct;
                item_Info.AttackSpeed = item_Equipment.AttackSpeed;
                item_Info.WalkSpeed = item_Equipment.WalkSpeed;
                item_Info.Defense = item_Equipment.Defense;
                item_Info.DefenseRate = item_Equipment.DefenseRate;
                item_Info.Durable = item_Equipment.Durable;
                item_Info.ReqLvl = item_Equipment.ReqLvl;
                item_Info.ReqStr = item_Equipment.ReqStr;
                item_Info.ReqAgi = item_Equipment.ReqAgi;
                item_Info.ReqVit = item_Equipment.ReqVit;
                item_Info.ReqEne = item_Equipment.ReqEne;
                item_Info.ReqCom = item_Equipment.ReqCom;
                item_Info.UseRole = item_Equipment.UseRole;
                item_Info.Prompt = item_Equipment.Prompt;
                item_Info.NormalDropWeight = item_Equipment.NormalDropWeight;
                item_Info.AppendDropWeight = item_Equipment.AppendDropWeight;
                item_Info.SkillDropWeight = item_Equipment.SkillDropWeight;
                item_Info.LuckyDropWeight = item_Equipment.LuckyDropWeight;
                item_Info.ExcellentDropWeight=item_Equipment.ExcellentDropWeight;
                item_Info.SetDropWeight = item_Equipment.SetDropWeight;
                item_Info.SocketDropWeight = item_Equipment.SocketDropWeight;
                item_Info.BaseAttrId=item_Equipment.BaseAttrId;
            }
            else if (self.GetItemConfig() is Item_WingConfig item_Wing) //翅膀
            {
              
                item_Info.BaseAttrId = item_Wing.BaseAttrId;
                item_Info.Id = item_Wing.Id;
                item_Info.Type = (int)item_Wing.Id / 10000;
                item_Info.Name = item_Wing.Name;
                item_Info.ResName = item_Wing.ResName;
                item_Info.Slot = item_Wing.Slot;
                item_Info.Skill = item_Wing.Skill;
                item_Info.X = item_Wing.X;
                item_Info.Y = item_Wing.Y;
                item_Info.StackSize = item_Wing.StackSize;
                item_Info.Drop = item_Wing.Drop;
                item_Info.DropLevel = item_Wing.Level;
                item_Info.Defense = item_Wing.Defense;
                item_Info.Durable = item_Wing.Durable;
                item_Info.ReqLvl = item_Wing.ReqLvl;
                item_Info.UseRole = item_Wing.UseRole;
                item_Info.Prompt = item_Wing.Prompt;
                

            }
            else if (self.GetItemConfig() is Item_NecklaceConfig item_Necklace)//项链
            {
                item_Info.Id = item_Necklace.Id;
                item_Info.Type = (int)item_Necklace.Id / 10000;
                item_Info.BaseAttrId = item_Necklace.BaseAttrId;
                item_Info.Name = item_Necklace.Name;
                item_Info.ResName = item_Necklace.ResName;
                item_Info.Slot = item_Necklace.Slot;
                item_Info.Skill = item_Necklace.Skill;
                item_Info.X = item_Necklace.X;
                item_Info.Y = item_Necklace.Y;
                item_Info.StackSize = item_Necklace.StackSize;
                item_Info.Drop = item_Necklace.Drop;
                item_Info.Durable = item_Necklace.Durable;
                item_Info.ReqLvl = item_Necklace.ReqLvl;
                item_Info.UseRole = item_Necklace.UseRole;
                item_Info.Prompt = item_Necklace.Prompt;

                item_Info.NormalDropWeight = item_Necklace.NormalDropWeight;
                item_Info.AppendDropWeight = item_Necklace.AppendDropWeight;
              
                item_Info.ExcellentDropWeight = item_Necklace.ExcellentDropWeight;
                item_Info.SetDropWeight = item_Necklace.SetDropWeight;
                item_Info.SocketDropWeight = item_Necklace.SocketDropWeight;
            }
            else if (self.GetItemConfig() is Item_RingsConfig item_Rings)//戒指
            {
                item_Info.Id = item_Rings.Id;
                item_Info.Type = (int)item_Rings.Id / 10000;
                item_Info.BaseAttrId = item_Rings.BaseAttrId;
                item_Info.Name = item_Rings.Name;
                item_Info.ResName = item_Rings.ResName;
                item_Info.Slot = item_Rings.Slot;
                item_Info.Skill = item_Rings.Skill;
                item_Info.X = item_Rings.X;
                item_Info.Y = item_Rings.Y;
                item_Info.StackSize = item_Rings.StackSize;
                item_Info.Drop = item_Rings.Drop;

                item_Info.Durable = item_Rings.Durable;
                item_Info.ReqLvl = item_Rings.ReqLvl;

                item_Info.UseRole = item_Rings.UseRole;
                item_Info.Prompt = item_Rings.Prompt;

                item_Info.NormalDropWeight = item_Rings.NormalDropWeight;
                item_Info.AppendDropWeight = item_Rings.AppendDropWeight;
              
                item_Info.ExcellentDropWeight = item_Rings.ExcellentDropWeight;
                item_Info.SetDropWeight = item_Rings.SetDropWeight;
              
            }
            else if (self.GetItemConfig() is Item_DanglerConfig item_Dangler)//项链
            {
                item_Info.Id = item_Dangler.Id;
                item_Info.Type = (int)item_Dangler.Id / 10000;
                item_Info.IsExc = item_Dangler.IsExc;
                item_Info.Name = item_Dangler.Name;
                item_Info.ResName = item_Dangler.ResName;
                item_Info.Slot = item_Dangler.Slot;
                item_Info.Skill = item_Dangler.Skill;
                item_Info.X = item_Dangler.X;
                item_Info.Y = item_Dangler.Y;
                item_Info.StackSize = item_Dangler.StackSize;
                item_Info.Drop = item_Dangler.Drop;
               
                item_Info.Durable = item_Dangler.Durable;
                item_Info.ReqLvl = item_Dangler.ReqLvl;

                item_Info.UseRole = item_Dangler.UseRole;
                item_Info.Prompt = item_Dangler.Prompt;

                
            }
            else if (self.GetItemConfig() is Item_MountsConfig item_Mounts)//坐骑
            {
                item_Info.Id = item_Mounts.Id;
                item_Info.Type = (int)item_Mounts.Id / 10000;
                item_Info.BaseAttrId = item_Mounts.BaseAttrId;
                item_Info.Name = item_Mounts.Name;
                item_Info.ResName = item_Mounts.ResName;
                item_Info.Slot = item_Mounts.Slot;
                item_Info.Skill = item_Mounts.Skill;
                item_Info.X = item_Mounts.X;
                item_Info.Y = item_Mounts.Y;
                item_Info.StackSize = item_Mounts.StackSize;
                item_Info.Drop = item_Mounts.Drop;

                item_Info.ReqLvl = item_Mounts.ReqLvl;

                item_Info.UseRole = item_Mounts.UseRole;
                item_Info.Prompt = item_Mounts.Prompt;


            }
            if (self.GetItemConfig() is Item_FGemstoneConfig item_FGemstone)//银光宝石
            {
                item_Info.Id = item_FGemstone.Id;
                item_Info.Type = (int)item_FGemstone.Id / 10000;
                item_Info.Name = item_FGemstone.Name;
                item_Info.ResName = item_FGemstone.ResName;
                item_Info.Slot = item_FGemstone.Slot;
                item_Info.Skill = item_FGemstone.Skill;
                item_Info.X = item_FGemstone.X;
                item_Info.Y = item_FGemstone.Y;
                item_Info.StackSize = item_FGemstone.StackSize;
                item_Info.Drop = item_FGemstone.Drop;

                item_Info.Prompt = item_FGemstone.Prompt;
            }
            else if (self.GetItemConfig() is Item_GemstoneConfig item_Gemstone)//宝石
            {
                item_Info.Type = (int)item_Gemstone.Id / 10000;
                item_Info.Id = item_Gemstone.Id;
                item_Info.Name = item_Gemstone.Name;
                item_Info.ResName = item_Gemstone.ResName;
                item_Info.Slot = item_Gemstone.Slot;
                item_Info.Skill = item_Gemstone.Skill;
                item_Info.X = item_Gemstone.X;
                item_Info.Y = item_Gemstone.Y;
                item_Info.StackSize = item_Gemstone.StackSize;
                item_Info.Drop = item_Gemstone.Drop;

                item_Info.Prompt = item_Gemstone.Prompt;
            }
            else if (self.GetItemConfig() is Item_SkillBooksConfig item_SkillBooks)//技能书
            {
                item_Info.Type = (int)item_SkillBooks.Id / 10000;
                item_Info.Id = item_SkillBooks.Id;
                item_Info.Name = item_SkillBooks.Name;
                item_Info.ResName = item_SkillBooks.ResName;
                item_Info.Slot = item_SkillBooks.Slot;
                item_Info.Skill = item_SkillBooks.Skill;
                item_Info.X = item_SkillBooks.X;
                item_Info.Y = item_SkillBooks.Y;
                item_Info.StackSize = item_SkillBooks.StackSize;
                item_Info.Drop = item_SkillBooks.Drop;
                item_Info.ReqLvl = item_SkillBooks.ReqLvl;
                item_Info.ReqStr = item_SkillBooks.ReqStr;
                item_Info.ReqAgi = item_SkillBooks.ReqAgi;
                item_Info.ReqVit = item_SkillBooks.ReqVit;
                item_Info.ReqEne = item_SkillBooks.ReqEne;
                item_Info.ReqCom = item_SkillBooks.ReqCom;
                item_Info.UseRole = item_SkillBooks.UseRole;
                item_Info.Prompt = item_SkillBooks.Prompt;

                item_Info.NormalDropWeight = item_SkillBooks.NormalDropWeight;
              
            }
            else if (self.GetItemConfig() is Item_GuardConfig item_Guard)//守护
            {
                item_Info.Type = (int)item_Guard.Id / 10000;
                item_Info.Id = item_Guard.Id;
                item_Info.Name = item_Guard.Name;
                item_Info.ResName = item_Guard.ResName;
                item_Info.Slot = item_Guard.Slot;
                item_Info.Skill = item_Guard.Skill;
                item_Info.X = item_Guard.X;
                item_Info.Y = item_Guard.Y;
                item_Info.StackSize = item_Guard.StackSize;
                item_Info.Drop = item_Guard.Drop;
                item_Info.BaseAttrId = item_Guard.BaseAttrId;
                item_Info.ReqLvl = item_Guard.ReqLvl;
                item_Info.ReqStr = item_Guard.ReqStr;
                item_Info.ReqAgi = item_Guard.ReqAgi;
                item_Info.ReqVit = item_Guard.ReqVit;
                item_Info.ReqEne = item_Guard.ReqEne;
                item_Info.ReqCom = item_Guard.ReqCom;
                item_Info.UseRole = item_Guard.UseRole;
                item_Info.Prompt = item_Guard.Prompt;
            }
            else if (self.GetItemConfig() is Item_ConsumablesConfig item_Consumables)//消耗品
            {
                item_Info.Type = (int)item_Consumables.Id / 10000;
                item_Info.Id = item_Consumables.Id;
                item_Info.Name = item_Consumables.Name;
                item_Info.ResName = item_Consumables.ResName;
                item_Info.Slot = item_Consumables.Slot;
                item_Info.Skill = item_Consumables.Skill;
                item_Info.X = item_Consumables.X;
                item_Info.Y = item_Consumables.Y;
                item_Info.StackSize = item_Consumables.StackSize;
                item_Info.Drop = item_Consumables.Drop;
                item_Info.DropLevel = item_Consumables.Level;

                item_Info.ReqLvl = item_Consumables.ReqLvl;
                item_Info.ReqStr = item_Consumables.ReqStr;
                item_Info.ReqAgi = item_Consumables.ReqAgi;
                item_Info.ReqVit = item_Consumables.ReqVit;
                item_Info.ReqEne = item_Consumables.ReqEne;
                item_Info.ReqCom = item_Consumables.ReqCom;
                item_Info.UseRole = item_Consumables.UseRole;
                item_Info.Prompt = item_Consumables.Prompt;

                item_Info.NormalDropWeight = item_Consumables.NormalDropWeight;
              
            }
            else if (self.GetItemConfig() is Item_OtherConfig item_Other)//其他
            {
                item_Info.Type = (int)item_Other.Id / 10000;
                item_Info.Id = item_Other.Id;
                item_Info.Name = item_Other.Name;
                item_Info.ResName = item_Other.ResName;
                item_Info.Slot = item_Other.Slot;
                item_Info.Skill = item_Other.Skill;
                item_Info.X = item_Other.X;
                item_Info.Y = item_Other.Y;
                item_Info.StackSize = item_Other.StackSize;
                item_Info.Drop = item_Other.Drop;

                item_Info.DropLevel = item_Other.Level;

                item_Info.Prompt = item_Other.Prompt;
            }
            else if (self.GetItemConfig() is Item_TaskConfig item_TaskConfig)//任务
            {
                item_Info.Type = (int)item_TaskConfig.Id / 10000;
                item_Info.Id = item_TaskConfig.Id;
                item_Info.Name = item_TaskConfig.Name;
                item_Info.ResName = item_TaskConfig.ResName;
                item_Info.Slot = item_TaskConfig.Slot;
                item_Info.Skill = item_TaskConfig.Skill;
                item_Info.X = item_TaskConfig.X;
                item_Info.Y = item_TaskConfig.Y;
                item_Info.StackSize = item_TaskConfig.StackSize;
                item_Info.Drop = item_TaskConfig.Drop;
                item_Info.DropLevel = item_TaskConfig.Level;

                item_Info.Prompt = item_TaskConfig.Prompt;
            }
            else if (self.GetItemConfig() is Item_FlagConfig item_FlagConfig)//旗帜
            {
                item_Info.BaseAttrId = item_FlagConfig.BaseAttrId;
                item_Info.Type = (int)item_FlagConfig.Id / 10000;
                item_Info.Id = item_FlagConfig.Id;
                item_Info.Name = item_FlagConfig.Name;
                item_Info.ResName = item_FlagConfig.ResName;
                item_Info.Slot = item_FlagConfig.Slot;
                item_Info.Skill = item_FlagConfig.Skill;
                item_Info.X = item_FlagConfig.X;
                item_Info.Y = item_FlagConfig.Y;
                item_Info.StackSize = item_FlagConfig.StackSize;
                item_Info.DropLevel = item_FlagConfig.Level;
                item_Info.Durable = item_FlagConfig.Durable;
                item_Info.ReqLvl = item_FlagConfig.ReqLvl;
                item_Info.UseRole = item_FlagConfig.UseRole;
                item_Info.Prompt = item_FlagConfig.Prompt;
            }
            else if (self.GetItemConfig() is Item_PetConfig item_Pet)//宠物
            {

                item_Info.Id = item_Pet.Id;
                item_Info.Type = (int)item_Pet.Id / 10000;
                item_Info.Name = item_Pet.Name;
                item_Info.ResName = item_Pet.ResName;
                item_Info.Slot = item_Pet.Slot;
                item_Info.Skill = item_Pet.Skill;
                item_Info.X = item_Pet.X;
                item_Info.Y = item_Pet.Y;
                item_Info.StackSize = item_Pet.StackSize;
                item_Info.Drop = item_Pet.Drop;
                item_Info.BaseAttrId = item_Pet.BaseAttrId;
                item_Info.UseRole = item_Pet.UseRole;

            }
            else if (self.GetItemConfig() is Item_BraceletConfig item_BraceletConfig)//手环
            {
                item_Info.BaseAttrId = item_BraceletConfig.BaseAttrId;
                item_Info.Type = (int)item_BraceletConfig.Id / 10000;
                item_Info.Id = item_BraceletConfig.Id;
                item_Info.Name = item_BraceletConfig.Name;
                item_Info.ResName = item_BraceletConfig.ResName;
                item_Info.Slot = item_BraceletConfig.Slot;
                item_Info.Skill = item_BraceletConfig.Skill;
                item_Info.X = item_BraceletConfig.X;
                item_Info.Y = item_BraceletConfig.Y;
                item_Info.StackSize = item_BraceletConfig.StackSize;
                item_Info.DropLevel = item_BraceletConfig.Level;
                item_Info.Durable = item_BraceletConfig.Durable;
                item_Info.ReqLvl = item_BraceletConfig.ReqLvl;
                item_Info.UseRole = item_BraceletConfig.UseRole;
                item_Info.Prompt = item_BraceletConfig.Prompt;

                item_Info.NormalDropWeight = item_BraceletConfig.NormalDropWeight;
                item_Info.AppendDropWeight = item_BraceletConfig.AppendDropWeight;
     
                item_Info.ExcellentDropWeight = item_BraceletConfig.ExcellentDropWeight;
                item_Info.SetDropWeight = item_BraceletConfig.SetDropWeight;
        
            }
        }

        public static void GetItemInfo_Out(this long self, out Item_infoConfig item_Info)
        {
            item_Info = new Item_infoConfig();
            if (self.GetItemConfig() is Item_EquipmentConfig item_Equipment)//装备
            {
                item_Info.Id = item_Equipment.Id;

                item_Info.AppendAttrId = item_Equipment.AppendAttrId;
                item_Info.Is400 = item_Equipment.Is400;
                item_Info.Curse = item_Equipment.Curse;
                item_Info.UpPet = item_Equipment.UpPet;
                item_Info.Name = item_Equipment.Name;
                item_Info.ResName = item_Equipment.ResName;
                item_Info.Slot = item_Equipment.Slot;
                item_Info.Type = (int)item_Equipment.Id / 10000;
                item_Info.Skill = item_Equipment.Skill;
                item_Info.X = item_Equipment.X;
                item_Info.Y = item_Equipment.Y;
                item_Info.StackSize = item_Equipment.StackSize;
                item_Info.Drop = item_Equipment.Drop;
                item_Info.TwoHand = item_Equipment.TwoHand;
                item_Info.DropLevel = item_Equipment.Level;
                item_Info.DamageMin = item_Equipment.DamageMin;
                item_Info.DamageMax = item_Equipment.DamageMax;
                item_Info.MagicPct = item_Equipment.MagicPct;
                item_Info.AttackSpeed = item_Equipment.AttackSpeed;
                item_Info.WalkSpeed = item_Equipment.WalkSpeed;
                item_Info.Defense = item_Equipment.Defense;
                item_Info.DefenseRate = item_Equipment.DefenseRate;
                item_Info.Durable = item_Equipment.Durable;
                item_Info.ReqLvl = item_Equipment.ReqLvl;
                item_Info.ReqStr = item_Equipment.ReqStr;
                item_Info.ReqAgi = item_Equipment.ReqAgi;
                item_Info.ReqVit = item_Equipment.ReqVit;
                item_Info.ReqEne = item_Equipment.ReqEne;
                item_Info.ReqCom = item_Equipment.ReqCom;
                item_Info.UseRole = item_Equipment.UseRole;
                item_Info.Prompt = item_Equipment.Prompt;
                item_Info.Sell = item_Equipment.Sell;

                item_Info.NormalDropWeight = item_Equipment.NormalDropWeight;
                item_Info.AppendDropWeight = item_Equipment.AppendDropWeight;
                item_Info.SkillDropWeight = item_Equipment.SkillDropWeight;
                item_Info.LuckyDropWeight = item_Equipment.LuckyDropWeight;
                item_Info.ExcellentDropWeight = item_Equipment.ExcellentDropWeight;
                item_Info.SetDropWeight = item_Equipment.SetDropWeight;
                item_Info.SocketDropWeight = item_Equipment.SocketDropWeight;
                item_Info.BaseAttrId = item_Equipment.BaseAttrId;
            }
            else if (self.GetItemConfig() is Item_WingConfig item_Wing) //翅膀
            {
                item_Info.BaseAttrId = item_Wing.BaseAttrId;
                item_Info.Type = (int)item_Wing.Id / 10000;
                item_Info.Id = item_Wing.Id;
                item_Info.Name = item_Wing.Name;
                item_Info.ResName = item_Wing.ResName;
                item_Info.Slot = item_Wing.Slot;
                item_Info.Skill = item_Wing.Skill;
                item_Info.X = item_Wing.X;
                item_Info.Y = item_Wing.Y;
                item_Info.StackSize = item_Wing.StackSize;
                item_Info.Drop = item_Wing.Drop;
                item_Info.DropLevel = item_Wing.Level;
                item_Info.Defense = item_Wing.Defense;
                item_Info.Durable = item_Wing.Durable;
                item_Info.ReqLvl = item_Wing.ReqLvl;
                item_Info.UseRole = item_Wing.UseRole;
                item_Info.Prompt = item_Wing.Prompt;
                item_Info.Sell = item_Wing.Sell;
            }
            else if (self.GetItemConfig() is Item_NecklaceConfig item_Necklace)//项链
            {
                item_Info.Id = item_Necklace.Id;
                item_Info.BaseAttrId = item_Necklace.BaseAttrId;
                item_Info.Type = (int)item_Necklace.Id / 10000;
                item_Info.Name = item_Necklace.Name;
                item_Info.ResName = item_Necklace.ResName;
                item_Info.Slot = item_Necklace.Slot;
                item_Info.Skill = item_Necklace.Skill;
                item_Info.X = item_Necklace.X;
                item_Info.Y = item_Necklace.Y;
                item_Info.StackSize = item_Necklace.StackSize;
                item_Info.Drop = item_Necklace.Drop;
                item_Info.Durable = item_Necklace.Durable;
                item_Info.ReqLvl = item_Necklace.ReqLvl;
                item_Info.UseRole = item_Necklace.UseRole;
                item_Info.Prompt = item_Necklace.Prompt;
                item_Info.Sell = item_Necklace.Sell;

                item_Info.NormalDropWeight = item_Necklace.NormalDropWeight;
                item_Info.AppendDropWeight = item_Necklace.AppendDropWeight;

                item_Info.ExcellentDropWeight = item_Necklace.ExcellentDropWeight;
                item_Info.SetDropWeight = item_Necklace.SetDropWeight;
                item_Info.SocketDropWeight = item_Necklace.SocketDropWeight;
            }
            else if (self.GetItemConfig() is Item_RingsConfig item_Rings)//戒指
            {
                item_Info.BaseAttrId = item_Rings.BaseAttrId;
                item_Info.Id = item_Rings.Id;
                item_Info.Type = (int)item_Rings.Id / 10000;
                item_Info.Name = item_Rings.Name;
                item_Info.ResName = item_Rings.ResName;
                item_Info.Slot = item_Rings.Slot;
                item_Info.Skill = item_Rings.Skill;
                item_Info.X = item_Rings.X;
                item_Info.Y = item_Rings.Y;
                item_Info.StackSize = item_Rings.StackSize;
                item_Info.Drop = item_Rings.Drop;

                item_Info.Durable = item_Rings.Durable;
                item_Info.ReqLvl = item_Rings.ReqLvl;

                item_Info.UseRole = item_Rings.UseRole;
                item_Info.Prompt = item_Rings.Prompt;
                item_Info.Sell = item_Rings.Sell;

                item_Info.NormalDropWeight = item_Rings.NormalDropWeight;
                item_Info.AppendDropWeight = item_Rings.AppendDropWeight;

                item_Info.ExcellentDropWeight = item_Rings.ExcellentDropWeight;
                item_Info.SetDropWeight = item_Rings.SetDropWeight;
            }
            else if (self.GetItemConfig() is Item_DanglerConfig item_Dangler)//项链
            {
               
                item_Info.Id = item_Dangler.Id;
                item_Info.Type = (int)item_Dangler.Id / 10000;
                item_Info.IsExc = item_Dangler.IsExc;
                item_Info.Name = item_Dangler.Name;
                item_Info.ResName = item_Dangler.ResName;
                item_Info.Slot = item_Dangler.Slot;
                item_Info.Skill = item_Dangler.Skill;
                item_Info.X = item_Dangler.X;
                item_Info.Y = item_Dangler.Y;
                item_Info.StackSize = item_Dangler.StackSize;
                item_Info.Drop = item_Dangler.Drop;

                item_Info.Durable = item_Dangler.Durable;
                item_Info.ReqLvl = item_Dangler.ReqLvl;

                item_Info.UseRole = item_Dangler.UseRole;
                item_Info.Prompt = item_Dangler.Prompt;
                item_Info.Sell = item_Dangler.Sell;
            }
            else if (self.GetItemConfig() is Item_MountsConfig item_Mounts)//坐骑
            {
                item_Info.BaseAttrId = item_Mounts.BaseAttrId;
                item_Info.Id = item_Mounts.Id;
                item_Info.Type = (int)item_Mounts.Id / 10000;
                item_Info.Name = item_Mounts.Name;
                item_Info.ResName = item_Mounts.ResName;
                item_Info.Slot = item_Mounts.Slot;
                item_Info.Skill = item_Mounts.Skill;
                item_Info.X = item_Mounts.X;
                item_Info.Y = item_Mounts.Y;
                item_Info.StackSize = item_Mounts.StackSize;
                item_Info.Drop = item_Mounts.Drop;

                item_Info.ReqLvl = item_Mounts.ReqLvl;

                item_Info.UseRole = item_Mounts.UseRole;
                item_Info.Prompt = item_Mounts.Prompt;
                item_Info.Sell = item_Mounts.Sell;
            }
            else if (self.GetItemConfig() is Item_PetConfig item_Pet)//宠物
            {
               
                item_Info.Id = item_Pet.Id;
                item_Info.Type = (int)item_Pet.Id / 10000;
                item_Info.Name = item_Pet.Name;
                item_Info.ResName = item_Pet.ResName;
                item_Info.Slot = item_Pet.Slot;
                item_Info.Skill = item_Pet.Skill;
                item_Info.X = item_Pet.X;
                item_Info.Y = item_Pet.Y;
                item_Info.StackSize = item_Pet.StackSize;
                item_Info.Drop = item_Pet.Drop;
                item_Info.Sell = item_Pet.Sell;
                item_Info.UseRole = item_Pet.UseRole;
                item_Info.BaseAttrId = item_Pet.BaseAttrId;
            }
            if (self.GetItemConfig() is Item_FGemstoneConfig item_FGemstone)//银光宝石
            {
                
                item_Info.Id = item_FGemstone.Id;
                item_Info.Type = (int)item_FGemstone.Id / 10000;
                item_Info.Name = item_FGemstone.Name;
                item_Info.ResName = item_FGemstone.ResName;
                item_Info.Slot = item_FGemstone.Slot;
                item_Info.Skill = item_FGemstone.Skill;
                item_Info.X = item_FGemstone.X;
                item_Info.Y = item_FGemstone.Y;
                item_Info.StackSize = item_FGemstone.StackSize;
                item_Info.Drop = item_FGemstone.Drop;

                item_Info.Prompt = item_FGemstone.Prompt;
                item_Info.Sell = item_FGemstone.Sell;
            }
            else if (self.GetItemConfig() is Item_GemstoneConfig item_Gemstone)//宝石
            {
                item_Info.Id = item_Gemstone.Id;
                item_Info.Name = item_Gemstone.Name;
                item_Info.ResName = item_Gemstone.ResName;
                item_Info.Slot = item_Gemstone.Slot;
                item_Info.Skill = item_Gemstone.Skill;
                item_Info.X = item_Gemstone.X;
                item_Info.Y = item_Gemstone.Y;
                item_Info.StackSize = item_Gemstone.StackSize;
                item_Info.Drop = item_Gemstone.Drop;

                item_Info.Prompt = item_Gemstone.Prompt;
                item_Info.Sell = item_Gemstone.Sell;
            }
            else if (self.GetItemConfig() is Item_SkillBooksConfig item_SkillBooks)//技能书
            {
                item_Info.Type = (int)item_SkillBooks.Id / 10000;
                item_Info.Id = item_SkillBooks.Id;
                item_Info.Name = item_SkillBooks.Name;
                item_Info.ResName = item_SkillBooks.ResName;
                item_Info.Slot = item_SkillBooks.Slot;
                item_Info.Skill = item_SkillBooks.Skill;
                item_Info.X = item_SkillBooks.X;
                item_Info.Y = item_SkillBooks.Y;
                item_Info.StackSize = item_SkillBooks.StackSize;
                item_Info.Drop = item_SkillBooks.Drop;
                item_Info.ReqLvl = item_SkillBooks.ReqLvl;
                item_Info.ReqStr = item_SkillBooks.ReqStr;
                item_Info.ReqAgi = item_SkillBooks.ReqAgi;
                item_Info.ReqVit = item_SkillBooks.ReqVit;
                item_Info.ReqEne = item_SkillBooks.ReqEne;
                item_Info.ReqCom = item_SkillBooks.ReqCom;
                item_Info.UseRole = item_SkillBooks.UseRole;
                item_Info.Prompt = item_SkillBooks.Prompt;
                item_Info.Sell = item_SkillBooks.Sell;

                item_Info.NormalDropWeight = item_SkillBooks.NormalDropWeight;
            }
            else if (self.GetItemConfig() is Item_GuardConfig item_Guard)//守护
            {
                item_Info.BaseAttrId = item_Guard.BaseAttrId;
                item_Info.Type = (int)item_Guard.Id / 10000;
                item_Info.Id = item_Guard.Id;
                item_Info.Name = item_Guard.Name;
                item_Info.ResName = item_Guard.ResName;
                item_Info.Slot = item_Guard.Slot;
                item_Info.Skill = item_Guard.Skill;
                item_Info.X = item_Guard.X;
                item_Info.Y = item_Guard.Y;
                item_Info.StackSize = item_Guard.StackSize;
                item_Info.Drop = item_Guard.Drop;

                item_Info.ReqLvl = item_Guard.ReqLvl;
                item_Info.ReqStr = item_Guard.ReqStr;
                item_Info.ReqAgi = item_Guard.ReqAgi;
                item_Info.ReqVit = item_Guard.ReqVit;
                item_Info.ReqEne = item_Guard.ReqEne;
                item_Info.ReqCom = item_Guard.ReqCom;
                item_Info.UseRole = item_Guard.UseRole;
                item_Info.Prompt = item_Guard.Prompt;
                item_Info.Sell = item_Guard.Sell;
            }
            else if (self.GetItemConfig() is Item_ConsumablesConfig item_Consumables)//消耗品
            {
                item_Info.Type = (int)item_Consumables.Id / 10000;
                item_Info.Id = item_Consumables.Id;
                item_Info.Name = item_Consumables.Name;
                item_Info.ResName = item_Consumables.ResName;
                item_Info.Slot = item_Consumables.Slot;
                item_Info.Skill = item_Consumables.Skill;
                item_Info.X = item_Consumables.X;
                item_Info.Y = item_Consumables.Y;
                item_Info.StackSize = item_Consumables.StackSize;
                item_Info.Drop = item_Consumables.Drop;
                item_Info.DropLevel = item_Consumables.Level;

                item_Info.ReqLvl = item_Consumables.ReqLvl;
                item_Info.ReqStr = item_Consumables.ReqStr;
                item_Info.ReqAgi = item_Consumables.ReqAgi;
                item_Info.ReqVit = item_Consumables.ReqVit;
                item_Info.ReqEne = item_Consumables.ReqEne;
                item_Info.ReqCom = item_Consumables.ReqCom;
                item_Info.UseRole = item_Consumables.UseRole;
                item_Info.Prompt = item_Consumables.Prompt;
                item_Info.Sell = item_Consumables.Sell;

                item_Info.NormalDropWeight = item_Consumables.NormalDropWeight;
            }
            else if (self.GetItemConfig() is Item_OtherConfig item_Other)//其他
            {
                item_Info.Type = (int)item_Other.Id / 10000;
                item_Info.Id = item_Other.Id;
                item_Info.Name = item_Other.Name;
                item_Info.ResName = item_Other.ResName;
                item_Info.Slot = item_Other.Slot;
                item_Info.Skill = item_Other.Skill;
                item_Info.X = item_Other.X;
                item_Info.Y = item_Other.Y;
                item_Info.StackSize = item_Other.StackSize;
                item_Info.Drop = item_Other.Drop;

                item_Info.DropLevel = item_Other.Level;

                item_Info.Prompt = item_Other.Prompt;
                item_Info.Sell = item_Other.Sell;
            }
            else if (self.GetItemConfig() is Item_TaskConfig item_TaskConfig)//任务
            {
                item_Info.Type = (int)item_TaskConfig.Id / 10000;
                item_Info.Id = item_TaskConfig.Id;
                item_Info.Name = item_TaskConfig.Name;
                item_Info.ResName = item_TaskConfig.ResName;
                item_Info.Slot = item_TaskConfig.Slot;
                item_Info.Skill = item_TaskConfig.Skill;
                item_Info.X = item_TaskConfig.X;
                item_Info.Y = item_TaskConfig.Y;
                item_Info.StackSize = item_TaskConfig.StackSize;
                item_Info.Drop = item_TaskConfig.Drop;
                item_Info.DropLevel = item_TaskConfig.Level;

                item_Info.Prompt = item_TaskConfig.Prompt;
                item_Info.Sell = item_TaskConfig.Sell;
            }
            else if (self.GetItemConfig() is Item_FlagConfig item_FlagConfig)//旗帜
            {
                item_Info.BaseAttrId = item_FlagConfig.BaseAttrId;
                item_Info.Type = (int)item_FlagConfig.Id / 10000;
                item_Info.Id = item_FlagConfig.Id;
                item_Info.Name = item_FlagConfig.Name;
                item_Info.ResName = item_FlagConfig.ResName;
                item_Info.Slot = item_FlagConfig.Slot;
                item_Info.Skill = item_FlagConfig.Skill;
                item_Info.X = item_FlagConfig.X;
                item_Info.Y = item_FlagConfig.Y;
                item_Info.StackSize = item_FlagConfig.StackSize;
                item_Info.DropLevel = item_FlagConfig.Level;
                item_Info.Durable = item_FlagConfig.Durable;
                item_Info.ReqLvl = item_FlagConfig.ReqLvl;
                item_Info.UseRole = item_FlagConfig.UseRole;
                item_Info.Prompt = item_FlagConfig.Prompt;
                item_Info.Sell = item_FlagConfig.Sell;
            } 
            else if (self.GetItemConfig() is Item_BraceletConfig item_BraceletConfig)//手环
            {
                item_Info.BaseAttrId = item_BraceletConfig.BaseAttrId;
                item_Info.Type = (int)item_BraceletConfig.Id / 10000;
                item_Info.Id = item_BraceletConfig.Id;
                item_Info.Name = item_BraceletConfig.Name;
                item_Info.ResName = item_BraceletConfig.ResName;
                item_Info.Slot = item_BraceletConfig.Slot;
                item_Info.Skill = item_BraceletConfig.Skill;
                item_Info.X = item_BraceletConfig.X;
                item_Info.Y = item_BraceletConfig.Y;
                item_Info.StackSize = item_BraceletConfig.StackSize;
                item_Info.DropLevel = item_BraceletConfig.Level;
                item_Info.Durable = item_BraceletConfig.Durable;
                item_Info.ReqLvl = item_BraceletConfig.ReqLvl;
                item_Info.UseRole = item_BraceletConfig.UseRole;
                item_Info.Prompt = item_BraceletConfig.Prompt;
                item_Info.Sell = item_BraceletConfig.Sell;

                item_Info.NormalDropWeight = item_BraceletConfig.NormalDropWeight;
                item_Info.AppendDropWeight = item_BraceletConfig.AppendDropWeight;

                item_Info.ExcellentDropWeight = item_BraceletConfig.ExcellentDropWeight;
                item_Info.SetDropWeight = item_BraceletConfig.SetDropWeight;
            }
        }

        /// <summary>
        /// 字符串 转为 字典
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>

        public static Dictionary<int, int> StringToDictionary(this string self)
        {
            Dictionary<int, int> dic = new Dictionary<int, int>();
            Regex reg = new Regex(@"[\{\}]");  //去掉{}
            Regex reg1 = new Regex(@"[\""]");  //去掉""
            string a = reg1.Replace(self, "");
            string a1 = reg.Replace(a, "");
           
            var str = a1.Split(',');
            
            if (str.Length > 0)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    var dicStrs = str[i].Split(':');
                    if (int.TryParse(dicStrs[0], out int k) && int.TryParse(dicStrs[1], out int result))
                    {
                        dic[k] = result;
                    }

                }
            }
            return dic;
        }
        /// <summary>
        /// 字符串转为字典
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Dictionary<int, string> StringToDictionary_String(this string self)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            Regex reg = new Regex(@"[\{\}]");  //去掉{}
            string a1 = reg.Replace(self, "");
            var str = a1.Split(',');
            if (str.Length > 0)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    var dicStrs = str[i].Split(':');
                    if (int.TryParse(dicStrs[0], out int k) && dicStrs[1] is string result)
                    {
                        dic[k] = result;
                    }

                }
            }
            return dic;
        }
        /// <summary>
        /// 玩家是否可以穿戴该件装备
        /// </summary>
        /// <param name="self">装备配置表</param>
        /// <param name="roleType">玩家的类型</param>
        /// <param name="roleclassLev">玩家的转职等级</param>
        /// <returns></returns>
        public static bool IsCanUer(this Item_infoConfig self, int roleType, int roleclassLev)
        {
            if (string.IsNullOrEmpty(self.UseRole)) return false;
            bool iscanuser = false;
            var dic = self.UseRole.StringToDictionary();
        
            foreach (var item in dic)
            {
              
                if (item.Key == roleType && item.Value <= roleclassLev)
                {
                 
                    iscanuser = true;
                    break;
                }
            }
            return iscanuser;
        }

    }


}
