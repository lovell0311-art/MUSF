using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// X镶嵌-点卡2.0.xlsx-Strengthen
    /// </summary>
    public partial class FluoreSet_StrengthenConfig : C_ConfigInfo
    {
        /// <summary>
        /// 等级
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 升级成功率
        /// </summary>
        public int SuccessRate { get; set; }
    }
    /// <summary>
    /// 配置数据:X镶嵌-点卡2.0.xlsx-Strengthen
    /// </summary>
    [ReadConfigAttribute(typeof(FluoreSet_StrengthenConfig), new AppType[] { AppType.Game })]
    public class FluoreSet_StrengthenConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, FluoreSet_StrengthenConfig> JsonDic = new Dictionary<int, FluoreSet_StrengthenConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public FluoreSet_StrengthenConfigJson(string b_ReadStr)
        {
            ReadData(b_ReadStr);
        }
        /// <summary>
        /// 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public override void ReadData(string b_ReadStr)
        {
            JsonDic.Clear();
            List<FluoreSet_StrengthenConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FluoreSet_StrengthenConfig>>(b_ReadStr);
            for (int i = 0; i < jsonData.Count; i++)
            {
                var mConfig = jsonData[i];
                mConfig.InitExpand();
                JsonDic[mConfig.Id] = mConfig;
            }
        }
        /// <summary>
        /// ConfigType
        /// </summary>
        public override Type ConfigType => typeof(FluoreSet_StrengthenConfigJson);
    }
}