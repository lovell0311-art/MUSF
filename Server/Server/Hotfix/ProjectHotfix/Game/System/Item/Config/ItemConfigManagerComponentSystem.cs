using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [EventMethod(typeof(ItemConfigManagerComponent), EventSystemType.INIT)]
    public class ItemConfigManagerComponentStartSystem : ITEventMethodOnInit<ItemConfigManagerComponent>
    {
        public void OnInit(ItemConfigManagerComponent self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(ItemConfigManagerComponent), EventSystemType.LOAD)]
    public class ItemConfigManagerComponentLoadSystem : ITEventMethodOnLoad<ItemConfigManagerComponent>
    {
        public override void OnLoad(ItemConfigManagerComponent self)
        {
            self.OnInit();
        }
    }





    public static class ItemConfigManagerComponentSystem
    {
        public static void OnInit(this ItemConfigManagerComponent self)
        {
            self.ItemConfigDict.Clear();
        }

        /// <summary>
        /// 获取物品配置
        /// </summary>
        /// <param name="self"></param>
        /// <param name="configId"></param>
        /// <returns></returns>
        public static ItemConfig Get(this ItemConfigManagerComponent self, int configId)
        {
            ItemConfig conf = null;
            if (self.ItemConfigDict.TryGetValue(configId, out conf))
            {
                return conf;
            }
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            switch ((EItemType)(configId / 10000))
            {
                case EItemType.Swords:
                case EItemType.Axes:
                case EItemType.Maces:
                case EItemType.Bows:
                case EItemType.Crossbows:
                case EItemType.Arrow:
                case EItemType.Spears:
                case EItemType.Staffs:
                case EItemType.MagicBook:
                case EItemType.Scepter:
                case EItemType.RuneWand:
                case EItemType.FistBlade:
                case EItemType.MagicSword:
                case EItemType.ShortSword:
                case EItemType.MagicGun:
                case EItemType.Shields:
                case EItemType.Helms:
                case EItemType.Armors:
                case EItemType.Pants:
                case EItemType.Gloves:
                case EItemType.Boots:
                    {
                        if (mReadConfigComponent.GetJson<Item_EquipmentConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Wing:
                    {
                        if (mReadConfigComponent.GetJson<Item_WingConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Necklace:
                    {
                        if (mReadConfigComponent.GetJson<Item_NecklaceConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Rings:
                    {
                        if (mReadConfigComponent.GetJson<Item_RingsConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Dangler:
                    {
                        if (mReadConfigComponent.GetJson<Item_DanglerConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Mounts:
                    {
                        if (mReadConfigComponent.GetJson<Item_MountsConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.FGemstone:
                    {
                        if (mReadConfigComponent.GetJson<Item_FGemstoneConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Gemstone:
                    {
                        if (mReadConfigComponent.GetJson<Item_GemstoneConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.SkillBooks:
                    {
                        if (mReadConfigComponent.GetJson<Item_SkillBooksConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Guard:
                    {
                        if (mReadConfigComponent.GetJson<Item_GuardConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Consumables:
                    {
                        if (mReadConfigComponent.GetJson<Item_ConsumablesConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Other:
                    {
                        if (mReadConfigComponent.GetJson<Item_OtherConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Task:
                    {
                        if (mReadConfigComponent.GetJson<Item_TaskConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Flag:
                    {
                        if (mReadConfigComponent.GetJson<Item_FlagConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Pets:
                    {
                        if (mReadConfigComponent.GetJson<Item_PetConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                case EItemType.Bracelet:
                    {
                        if (mReadConfigComponent.GetJson<Item_BraceletConfigJson>().JsonDic.TryGetValue(configId, out var confTemp))
                        {
                            conf = confTemp.ToItemConfig();
                        }
                    }
                    break;
                default:
                    conf = null;
                    break;
            }
            if (conf != null)
            {
                conf.KindId = ItemConfig.GetKindId(conf.KindName);
                self.ItemConfigDict.Add(conf.Id, conf);
            }
            return conf;
        }

    }
}
