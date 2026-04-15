using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// J觉醒-点卡2.0.xlsx-血脉
    /// </summary>
    public partial class BloodAwakening_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 武魂名称
        /// </summary>
        public string BloodAwakening { get; set; }
        /// <summary>
        /// 显示资源
        /// </summary>
        public string DisplayResource { get; set; }
        /// <summary>
        /// 激活需求
        /// </summary>
        public string ActivateNeed { get; set; }
        /// <summary>
        /// 净化需求
        /// </summary>
        public string PurityNeed { get; set; }
        /// <summary>
        /// 净化时间(秒)
        /// </summary>
        public string PurityTime { get; set; }
        /// <summary>
        /// 一环属性节点
        /// </summary>
        public string AttributeNode1 { get; set; }
        /// <summary>
        /// 二环属性节点
        /// </summary>
        public string AttributeNode2 { get; set; }
        /// <summary>
        /// 三环属性节点
        /// </summary>
        public string AttributeNode3 { get; set; }
        /// <summary>
        /// 四环属性节点
        /// </summary>
        public string AttributeNode4 { get; set; }
        /// <summary>
        /// 五环属性节点
        /// </summary>
        public string AttributeNode5 { get; set; }
    }
    /// <summary>
    /// 配置数据:J觉醒-点卡2.0.xlsx-血脉
    /// </summary>
    [ReadConfigAttribute(typeof(BloodAwakening_InfoConfig), new AppType[] { AppType.Game })]
    public class BloodAwakening_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, BloodAwakening_InfoConfig> JsonDic = new Dictionary<int, BloodAwakening_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public BloodAwakening_InfoConfigJson(string b_ReadStr)
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
            List<BloodAwakening_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BloodAwakening_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(BloodAwakening_InfoConfigJson);
    }
}