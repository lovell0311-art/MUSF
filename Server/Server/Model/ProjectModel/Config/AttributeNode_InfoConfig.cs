using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// J觉醒-点卡2.0.xlsx-属性节点
    /// </summary>
    public partial class AttributeNode_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// 节点Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 激活需求
        /// </summary>
        public string ActivateNeed { get; set; }
        /// <summary>
        /// 节点属性
        /// </summary>
        public List<int> AttributeNode { get; set; }
    }
    /// <summary>
    /// 配置数据:J觉醒-点卡2.0.xlsx-属性节点
    /// </summary>
    [ReadConfigAttribute(typeof(AttributeNode_InfoConfig), new AppType[] { AppType.Game })]
    public class AttributeNode_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, AttributeNode_InfoConfig> JsonDic = new Dictionary<int, AttributeNode_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public AttributeNode_InfoConfigJson(string b_ReadStr)
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
            List<AttributeNode_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AttributeNode_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(AttributeNode_InfoConfigJson);
    }
}