using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    [EventMethod(typeof(ExcAttrEntryManagerComponent), EventSystemType.INIT)]
    public class ExcAttrEntryManagerComponentStartSystem : ITEventMethodOnInit<ExcAttrEntryManagerComponent>
    {
        public void OnInit(ExcAttrEntryManagerComponent self)
        {
            self.Load();
        }
    }

    [EventMethod(typeof(ExcAttrEntryManagerComponent), EventSystemType.LOAD)]
    public class ExcAttrEntryManagerComponentLoadSystem : ITEventMethodOnLoad<ExcAttrEntryManagerComponent>
    {
        public override void OnLoad(ExcAttrEntryManagerComponent self)
        {
            self.Load();
        }
    }




    public static class ExcAttrEntryManagerComponentSystem
    {

        public static void Load(this ExcAttrEntryManagerComponent self)
        {
            self.__WeaponAttrEntry.Clear();
            self.__ArmorAttrEntry.Clear();
            self.__PetsAttrEntry.Clear();
            self.ExcAttrEntryCount.Clear();
            self.PetsExcAttrEntryCount.Clear();
            self.__FlagAttrEntry.Clear();

            var allConf = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_ExcConfigJson>().JsonDic.Values;

            foreach(var conf in allConf)
            {
                switch ((EItemExcType)conf.EntryType)
                {
                    case EItemExcType.WeaponAndNecklace:
                        self.__WeaponAttrEntry.Add(conf.Id, conf.Rate);
                        self.__PetsAttrEntry.Add(conf.Id, conf.PetsRate);
                        break;
                    case EItemExcType.ArmorAndRings:
                        self.__ArmorAttrEntry.Add(conf.Id, conf.Rate);
                        break;
                    default:
                        Log.Error($"未知的卓越属性词条类型，conf.EntryType={conf.EntryType}");
                        break;
                }
                self.__FlagAttrEntry.Add(conf.Id, conf.FlagRate);
                //self.__PetsAttrEntry.Add(conf.Id, conf.PetsRate);
            }

            // 物品
            var entryRateConf = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntryRate_ExcConfigJson>().JsonDic.Values;
            foreach(var conf in entryRateConf)
            {
                //物品
                self.ExcAttrEntryCount.Add(conf.StripCnt, conf.Rate);
                //宠物
                self.PetsExcAttrEntryCount.Add(conf.StripCnt, conf.PetsRate);
                //旗帜
                self.FlagExcAttrEntryCount.Add(conf.StripCnt, conf.FlagRate);
            }

            // 宠物
            //var petsEntryRateConf = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<PetsAttrEntry_RateConfigJson>().JsonDic.Values;
            //foreach (var conf in petsEntryRateConf)
            //{
            //    self.PetsExcAttrEntryCount.Add(conf.StripCnt, conf.Rate);
            //}

            //荧光宝石属性
            self.YingGuangBaoShi = new Dictionary<int, Dictionary<int, List<int>>>();
            List<int> LinShi = new List<int>() {270008,270009,270010,270011,270012,270013};
            for (int i = 0; i < LinShi.Count; i++)
            { 
                int ConfigId = LinShi[i];
                //给荧光宝石随机附加属性            
                var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
                var mConfigDic = mReadConfigComponent.GetJson<FluoreSet_AttrConfigJson>().JsonDic;
                List<int> weightList = new List<int>();
                List<int> configIDList = new List<int>();
                foreach (var configItem in mConfigDic)
                {
                    if (configItem.Value.fluore == ConfigId)
                    {
                        weightList.Add(configItem.Value.weight);
                        configIDList.Add(configItem.Value.Id);
                    }
                }
                if (weightList.Count <= 0)
                {
                    Log.Warning($"荧光宝石属性异常{ConfigId}");
                }
                Dictionary<int,List<int>> keyValuePairs = new Dictionary<int,List<int>>();
                keyValuePairs.Add(1, weightList);
                keyValuePairs.Add(2, configIDList);
                self.YingGuangBaoShi.Add(ConfigId, keyValuePairs);
            }
                
        }
        public static int TryGetYingGuangBaoShi(this ExcAttrEntryManagerComponent self,int ItemConfig)
        {
            if (self.YingGuangBaoShi.TryGetValue(ItemConfig, out var Value))
            {
                var weightList = Value[1];
                var configIDList  = Value[2];
                int randomID = StrengthenItemSystem.StrengthenResult(weightList);
                if (randomID == -1)
                    Log.Warning($"C2G_FluoreGemsCompose 荧光宝石合成错误 randomID = -1,createrGemID = {ItemConfig}");
                return configIDList[randomID] * 100 + 0;
            }
            Log.Warning($"获取荧光宝石属性异常");
            return 0;
        }
        public static bool TryGetPetsAttrEntry(this ExcAttrEntryManagerComponent self, out RandomSelector<int> value)
        {
            if (self.__PetsAttrEntry != null)
            {
                value = self.__PetsAttrEntry;
                return true;
            }
            value=null;
            return false;
        }
        /// <summary>
        /// 尝试通过物品获取词条选择器
        /// </summary>
        /// <param name="self"></param>
        /// <param name="item"></param>
        /// <param name="value">输出的选择器</param>
        /// <returns>有没有找到对应的词条选择器</returns>
        public static bool TryGetSelectorByItem(this ExcAttrEntryManagerComponent self,Item item,out RandomSelector<int> value)
        {
            switch (item.Type)
            {
                case EItemType.Swords:// 武器
                case EItemType.Axes:
                case EItemType.Maces:
                case EItemType.Bows:
                case EItemType.Crossbows:
                case EItemType.Spears:
                case EItemType.Staffs:
                case EItemType.MagicBook:
                case EItemType.Scepter:
                case EItemType.RuneWand:
                case EItemType.FistBlade:
                case EItemType.MagicSword:
                case EItemType.ShortSword:
                case EItemType.MagicGun:
                case EItemType.Necklace:// 项链
                case EItemType.Arrow:// 箭筒
                    // 武器项链
                    value = self.__WeaponAttrEntry;
                    return true;
                case EItemType.Shields: // 防具
                case EItemType.Helms:
                case EItemType.Armors:
                case EItemType.Pants:
                case EItemType.Gloves:
                case EItemType.Boots:
                    
                case EItemType.Rings:// 戒指
                case EItemType.Bracelet:// 手环
                case EItemType.Flag:// 旗帜
                case EItemType.Guard:// 守护
                    // 防具戒指
                    value = self.__ArmorAttrEntry;
                    return true;
                case EItemType.Pets:
                    value = self.__PetsAttrEntry;
                     return true;
                default:
                    break;
            }
            value = null;
            return false;
        }


    }
}
