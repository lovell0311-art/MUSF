using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C宠物-点卡2.0.xlsx-强化属性
    /// </summary>
    public partial class Pets_EnhanceAttributeConfig : C_ConfigInfo
    {
        /// <summary>
        /// 强化组
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 增加属性1
        /// </summary>
        public int Enhance { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
    /// <summary>
    /// 配置数据:C宠物-点卡2.0.xlsx-强化属性
    /// </summary>
    [ReadConfigAttribute(typeof(Pets_EnhanceAttributeConfig), new AppType[] { AppType.Game,AppType.Map })]
    public class Pets_EnhanceAttributeConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Pets_EnhanceAttributeConfig> JsonDic = new Dictionary<int, Pets_EnhanceAttributeConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Pets_EnhanceAttributeConfigJson(string b_ReadStr)
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
            List<Pets_EnhanceAttributeConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pets_EnhanceAttributeConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Pets_EnhanceAttributeConfigJson);
    }
}