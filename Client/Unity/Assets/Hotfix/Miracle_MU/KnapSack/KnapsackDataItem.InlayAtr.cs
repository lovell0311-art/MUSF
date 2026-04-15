using ILRuntime.Runtime;

using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// 镶嵌属性
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 获取装备的镶嵌属性
        /// </summary>
        /// <param name="list"></param>
        public void GetInlayAtr(ref List<string> list) 
        {
            for (int i = 0, length=GetProperValue(E_ItemValue.FluoreSlotCount); i < length; i++)
            {
                if (GetProperValue(E_ItemValue.FluoreSlot1 + i) is int PropId && PropId != 0)
                {
                    FluoreSet_AttrConfig fluoreSet_AttrConfig = ConfigComponent.Instance.GetItem<FluoreSet_AttrConfig>((PropId / 100).ToInt32());//获取荧光宝石 属性配置表
                   // list.Add($"<color={ColorTools.LuckyItemColor}>镶宝{i + 1}:{GetYingGuangBaoShiAtr(fluoreSet_AttrConfig.fluore)}({string.Format(fluoreSet_AttrConfig.Info, GetAtrValue(fluoreSet_AttrConfig, (PropId % 100).ToInt32()))})</color>");
                    var str = $"镶宝{i + 1}:{GetYingGuangBaoShiAtr(fluoreSet_AttrConfig.fluore)}({string.Format(fluoreSet_AttrConfig.Info, GetAtrValue(fluoreSet_AttrConfig, (PropId % 100).ToInt32()))})";
                    var  strings= SplitStringIntoMultipart(str,13);
                    for (int j = 0; j < strings.Length; j++)
                    {
                        list.Add($"<color={ColorTools.LuckyItemColor}>{strings[j]}</color>");
                    }
                }
                else
                {
                    list.Add($"<color={ColorTools.LuckyItemColor}>镶宝{i + 1}：可以镶嵌</color>");
                }
            }
            if (GetProperValue(E_ItemValue.FluoreSlotCount) == 0) return;
            list.Add("");
        }
        /// <summary>
        /// 判断是否
        /// </summary>
        /// <returns></returns>
        public bool GetHaveInLayAtr()
        {
            if (GetProperValue(E_ItemValue.FluoreSlotCount) == 0) return false;
            else
            {
                int count = 0;
                for (int i = 0, length = GetProperValue(E_ItemValue.FluoreSlotCount); i < length; i++)
                {
                    if (GetProperValue(E_ItemValue.FluoreSlot1 + i) is int PropId && PropId != 0)
                    {
                        count++;
                    }
                }
                if(count == 0) return false;
                else
                {
                    return true;
                }
            }
        }
        /// <summary>
        /// 获取荧光宝石 对应的属性
        /// </summary>
        /// <param name="configId"></param>
        /// <returns></returns>
       public string GetYingGuangBaoShiAtr(int configId) => configId switch
        {
            270008 => "火",
            270009 => "水",
            270010 => "冰",
            270011 => "风",
            270012 => "雷",
            270013 => "土",
            _ => string.Empty,
        };
        /// <summary>
        /// 获取对应等级的 属性值
        /// </summary>
        /// <param name="fluoreSet_Attr"></param>
        /// <param name="lev"></param>
        /// <returns></returns>
       public float GetAtrValue(FluoreSet_AttrConfig fluoreSet_Attr, int lev) => lev switch
        {
            0 => (float)fluoreSet_Attr.Level0 / 10000,
            1 => (float)fluoreSet_Attr.Level1 / 10000,
            2 => (float)fluoreSet_Attr.Level2 / 10000,
            3 => (float)fluoreSet_Attr.Level3 / 10000,
            4 => (float)fluoreSet_Attr.Level4 / 10000,
            5 => (float)fluoreSet_Attr.Level5/ 10000,
            6 => (float)fluoreSet_Attr.Level6 / 10000,
            7 => (float)fluoreSet_Attr.Level7/ 10000,
            8 => (float)fluoreSet_Attr.Level8 / 10000,
            9 => (float)fluoreSet_Attr.Level9/ 10000,
            _ => 0,
        };
        /// <summary>
        /// 将字符串分割
        /// </summary>
        /// <param name="input"></param>
        /// <param name="eachCount">每一段的长度（一个汉字长度为2）</param>
        /// <returns></returns>
        public static string[] SplitStringIntoMultipart(string input, int eachCount)
        {
            if (input.Length == 0)
                return new string[0];
            if (input.Length <= eachCount)
                return new string[1] { input };
            int partNum;
            if (input.Length % eachCount == 0)
                partNum = input.Length / eachCount;
            else
                partNum = input.Length / eachCount + 1;

            string[] result = new string[partNum];
            for (int i = 0; i < partNum - 1; i++)
                result[i] = input.Substring(i * eachCount, eachCount);

            result[partNum - 1] = input.Substring((partNum - 1) * eachCount);
            return result;

        }
    }
}