using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C宠物-点卡2.0.xlsx-卓越概率
    /// </summary>
    public partial class PetsAttrEntry_RateConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 条数
        /// </summary>
        public int StripCnt { get; set; }
        /// <summary>
        /// 概率(万分比)
        /// </summary>
        public int Rate { get; set; }
    }
    /// <summary>
    /// 配置数据:C宠物-点卡2.0.xlsx-卓越概率
    /// </summary>
    [ReadConfigAttribute(typeof(PetsAttrEntry_RateConfig), new AppType[] { AppType.Game,AppType.Map })]
    public class PetsAttrEntry_RateConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, PetsAttrEntry_RateConfig> JsonDic = new Dictionary<int, PetsAttrEntry_RateConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public PetsAttrEntry_RateConfigJson(string b_ReadStr)
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
            List<PetsAttrEntry_RateConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<PetsAttrEntry_RateConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(PetsAttrEntry_RateConfigJson);
    }
}