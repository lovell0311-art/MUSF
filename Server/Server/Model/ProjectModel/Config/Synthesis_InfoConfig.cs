using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// H合成-点卡22.0.xlsx-Synthesis
    /// </summary>
    public partial class Synthesis_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 合成方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// 需求物品
        /// </summary>
        public string NeedItems { get; set; }
        /// <summary>
        /// 所需金币
        /// </summary>
        public long NeedGold { get; set; }
        /// <summary>
        /// 基础成功率
        /// </summary>
        public int BaseSuccessRate { get; set; }
        /// <summary>
        /// 最高成功率
        /// </summary>
        public int MaxSuccessRate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Info { get; set; }
    }
    /// <summary>
    /// 配置数据:H合成-点卡22.0.xlsx-Synthesis
    /// </summary>
    [ReadConfigAttribute(typeof(Synthesis_InfoConfig), new AppType[] { AppType.Game,AppType.Robot })]
    public class Synthesis_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Synthesis_InfoConfig> JsonDic = new Dictionary<int, Synthesis_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Synthesis_InfoConfigJson(string b_ReadStr)
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
            List<Synthesis_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Synthesis_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Synthesis_InfoConfigJson);
    }
}