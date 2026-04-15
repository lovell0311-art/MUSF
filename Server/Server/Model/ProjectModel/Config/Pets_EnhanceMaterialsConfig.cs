using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C宠物-点卡2.0.xlsx-强化材料
    /// </summary>
    public partial class Pets_EnhanceMaterialsConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// #材料
        /// </summary>
        public Dictionary<int,int> Enhance { get; set; }
        /// <summary>
        /// #成功率
        /// </summary>
        public int SuccessRate { get; set; }
    }
    /// <summary>
    /// 配置数据:C宠物-点卡2.0.xlsx-强化材料
    /// </summary>
    [ReadConfigAttribute(typeof(Pets_EnhanceMaterialsConfig), new AppType[] { AppType.Game,AppType.Map })]
    public class Pets_EnhanceMaterialsConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Pets_EnhanceMaterialsConfig> JsonDic = new Dictionary<int, Pets_EnhanceMaterialsConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Pets_EnhanceMaterialsConfigJson(string b_ReadStr)
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
            List<Pets_EnhanceMaterialsConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pets_EnhanceMaterialsConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Pets_EnhanceMaterialsConfigJson);
    }
}