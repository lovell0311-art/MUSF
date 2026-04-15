using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// X镶嵌-点卡2.0.xlsx-荧光属性
    /// </summary>
    public partial class FluoreSet_AttrConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 属性随机概率（权重）
        /// </summary>
        public int weight { get; set; }
        /// <summary>
        /// 属性宝石ID
        /// </summary>
        public int fluore { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 属性
        /// </summary>
        public int Attribute { get; set; }
        /// <summary>
        /// 等级0
        /// </summary>
        public int Level0 { get; set; }
        /// <summary>
        /// 等级1
        /// </summary>
        public int Level1 { get; set; }
        /// <summary>
        /// 等级2
        /// </summary>
        public int Level2 { get; set; }
        /// <summary>
        /// 等级3
        /// </summary>
        public int Level3 { get; set; }
        /// <summary>
        /// 等级4
        /// </summary>
        public int Level4 { get; set; }
        /// <summary>
        /// 等级5
        /// </summary>
        public int Level5 { get; set; }
        /// <summary>
        /// 等级6
        /// </summary>
        public int Level6 { get; set; }
        /// <summary>
        /// 等级7
        /// </summary>
        public int Level7 { get; set; }
        /// <summary>
        /// 等级8
        /// </summary>
        public int Level8 { get; set; }
        /// <summary>
        /// 等级9
        /// </summary>
        public int Level9 { get; set; }
        /// <summary>
        /// 判断
        /// </summary>
        public int Judgment { get; set; }
    }
    /// <summary>
    /// 配置数据:X镶嵌-点卡2.0.xlsx-荧光属性
    /// </summary>
    [ReadConfigAttribute(typeof(FluoreSet_AttrConfig), new AppType[] { AppType.Game,AppType.GM })]
    public class FluoreSet_AttrConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, FluoreSet_AttrConfig> JsonDic = new Dictionary<int, FluoreSet_AttrConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public FluoreSet_AttrConfigJson(string b_ReadStr)
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
            List<FluoreSet_AttrConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<FluoreSet_AttrConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(FluoreSet_AttrConfigJson);
    }
}