using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// AI机器人-点卡2.0.xlsx-AI配置
    /// </summary>
    public partial class AI_Config : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 所属ai
        /// </summary>
        public int AIConfigId { get; set; }
        /// <summary>
        /// 此ai中的顺序
        /// </summary>
        public int Order { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 节点参数
        /// </summary>
        public List<int> NodeParams { get; set; }
    }
    /// <summary>
    /// 配置数据:AI机器人-点卡2.0.xlsx-AI配置
    /// </summary>
    [ReadConfigAttribute(typeof(AI_Config), new AppType[] { AppType.Robot })]
    public class AI_ConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, AI_Config> JsonDic = new Dictionary<int, AI_Config>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public AI_ConfigJson(string b_ReadStr)
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
            List<AI_Config> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<AI_Config>>(b_ReadStr);
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
        public override Type ConfigType => typeof(AI_ConfigJson);
    }
}