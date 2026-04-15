using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    public static class ItemAttrEntryFactory
    {
        public static ItemAttrEntry Create(int configId,int level = 0)
        {
            ItemAttrEntry entry = new ItemAttrEntry(configId, level);
            switch ((EItemAttrEntryType)(configId / 100000))
            {
                case EItemAttrEntryType.Base:
                    if(Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_BaseConfigJson>().JsonDic.TryGetValue(configId, out var confBase))
                    {
                        entry.PropId = (EItemAttrEntryPropId)confBase.PropId;
                        entry.WillWeaken = (confBase.WillWeaken != 0);
                        entry.Value = GetValueByLevel(confBase, level);
                        entry.AppendValue = confBase.Outsattrib;
                    }
                    break;
                case EItemAttrEntryType.Set:
                    if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_SetConfigJson>().JsonDic.TryGetValue(configId, out var confSet))
                    {
                        entry.PropId = (EItemAttrEntryPropId)confSet.PropId;
                        entry.Value = GetValueByLevel(confSet, level);
                        entry.AppendValue = 0;
                    }
                    break;
                case EItemAttrEntryType.Excellent:
                    if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_ExcConfigJson>().JsonDic.TryGetValue(configId, out var confExc))
                    {
                        entry.PropId = (EItemAttrEntryPropId)confExc.PropId;
                        entry.Value = GetValueByLevel(confExc, level);
                        entry.AppendValue = 0;
                    }
                    break;
                case EItemAttrEntryType.Regen:
                    if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_RegenConfigJson>().JsonDic.TryGetValue(configId, out var confRegen))
                    {
                        entry.PropId = (EItemAttrEntryPropId)confRegen.PropId;
                        entry.Value = GetValueByLevel(confRegen, level);
                        entry.AppendValue = 0;
                    }
                    break;
                case EItemAttrEntryType.Entry380:
                    if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_380ConfigJson>().JsonDic.TryGetValue(configId, out var conf380))
                    {
                        entry.PropId = (EItemAttrEntryPropId)conf380.PropId;
                        entry.Value = GetValueByLevel(conf380, level);
                        entry.AppendValue = 0;
                    }
                    break;
                case EItemAttrEntryType.Special:
                    if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_SpecialConfigJson>().JsonDic.TryGetValue(configId, out var confSpecial))
                    {
                        entry.PropId = (EItemAttrEntryPropId)confSpecial.PropId;
                        entry.Value = GetValueByLevel(confSpecial, level);
                        entry.AppendValue = 0;
                    }
                    break;
                case EItemAttrEntryType.Extra:
                    if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_ExtraConfigJson>().JsonDic.TryGetValue(configId, out var confExtra))
                    {
                        entry.PropId = (EItemAttrEntryPropId)confExtra.PropId;
                        entry.Value = GetValueByLevel(confExtra, level);
                        entry.AppendValue = 0;
                    }
                    break;
                case EItemAttrEntryType.Append:
                    if (Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemAttrEntry_AppendConfigJson>().JsonDic.TryGetValue(configId, out var confAppend))
                    {
                        entry.PropId = (EItemAttrEntryPropId)confAppend.PropId;
                        entry.Value = GetValueByLevel(confAppend, level);
                        entry.AppendValue = 0;
                    }
                    break;
                default:
                    //throw new Exception($"无法创建ItemAttrEntry,未知的 configId = {configId}");
                    return null;
            }
            return entry;
        }

        #region GetValue
        private static int GetValueByLevel(ItemAttrEntry_BaseConfig conf, int level)
        {
            switch (level)
            {
                case 0: return conf.Value0;
                case 1: return conf.Value1;
                case 2: return conf.Value2;
                case 3: return conf.Value3;
                case 4: return conf.Value4;
                case 5: return conf.Value5;
                case 6: return conf.Value6;
                case 7: return conf.Value7;
                case 8: return conf.Value8;
                case 9: return conf.Value9;
                case 10: return conf.Value10;
                case 11: return conf.Value11;
                case 12: return conf.Value12;
                case 13: return conf.Value13;
                case 14: return conf.Value14;
                case 15: return conf.Value15;
            }
            return 0;
        }

        private static int GetValueByLevel(ItemAttrEntry_SetConfig conf, int level)
        {
            switch (level)
            {
                case 0: return conf.Value0;
            }
            return 0;
        }

        private static int GetValueByLevel(ItemAttrEntry_ExcConfig conf, int level)
        {
            switch (level)
            {
                case 0: return conf.Value0;
            }
            return 0;
        }

        private static int GetValueByLevel(ItemAttrEntry_RegenConfig conf, int level)
        {
            switch (level)
            {
                case 0: return conf.Value0;
                case 1: return conf.Value1;
                case 2: return conf.Value2;
                case 3: return conf.Value3;
                case 4: return conf.Value4;
                case 5: return conf.Value5;
                case 6: return conf.Value6;
                case 7: return conf.Value7;
                case 8: return conf.Value8;
                case 9: return conf.Value9;
                case 10: return conf.Value10;
                case 11: return conf.Value11;
                case 12: return conf.Value12;
                case 13: return conf.Value13;
                case 14: return conf.Value14;
                case 15: return conf.Value15;
            }
            return 0;
        }


        private static int GetValueByLevel(ItemAttrEntry_380Config conf, int level)
        {
            switch (level)
            {
                case 0: return conf.Value0;
            }
            return 0;
        }

        private static int GetValueByLevel(ItemAttrEntry_SpecialConfig conf, int level)
        {
            switch (level)
            {
                case 0: return conf.Value0;
                case 1: return conf.Value1;
                case 2: return conf.Value2;
                case 3: return conf.Value3;
            }
            return 0;
        }

        private static int GetValueByLevel(ItemAttrEntry_ExtraConfig conf, int level)
        {
            switch (level)
            {
                case 0: return conf.Value0;
                case 1: return conf.Value1;
                case 2: return conf.Value2;
            }
            return 0;
        }

        private static int GetValueByLevel(ItemAttrEntry_AppendConfig conf, int level)
        {
            switch (level)
            {
                case 0: return 0;
                case 1: return conf.Value1;
                case 2: return conf.Value2;
                case 3: return conf.Value3;
                case 4: return conf.Value4;
            }
            return 0;
        }



        #endregion
    }
}
