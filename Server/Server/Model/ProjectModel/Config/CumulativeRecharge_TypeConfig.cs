using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C充值礼包-点卡2.0.xlsx-累计充值
    /// </summary>
    public partial class CumulativeRecharge_TypeConfig : C_ConfigInfo
    {
        /// <summary>
        /// 累计充值id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 所需金额
        /// </summary>
        public int Money { get; set; }
        /// <summary>
        /// 物品描述
        /// </summary>
        public string Description { get; set; }
    }
    /// <summary>
    /// 配置数据:C充值礼包-点卡2.0.xlsx-累计充值
    /// </summary>
    [ReadConfigAttribute(typeof(CumulativeRecharge_TypeConfig), new AppType[] { AppType.Game })]
    public class CumulativeRecharge_TypeConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, CumulativeRecharge_TypeConfig> JsonDic = new Dictionary<int, CumulativeRecharge_TypeConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public CumulativeRecharge_TypeConfigJson(string b_ReadStr)
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
            List<CumulativeRecharge_TypeConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CumulativeRecharge_TypeConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(CumulativeRecharge_TypeConfigJson);
    }
}