using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C宠物-点卡2.0.xlsx-宠物经验
    /// </summary>
    public partial class Pets_ExpConfig : C_ConfigInfo
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 升级所需经验
        /// </summary>
        public long Exprience { get; set; }
        /// <summary>
        /// 升级累积需要经验
        /// </summary>
        public long ExprenceLevel { get; set; }
    }
    /// <summary>
    /// 配置数据:C宠物-点卡2.0.xlsx-宠物经验
    /// </summary>
    [ReadConfigAttribute(typeof(Pets_ExpConfig), new AppType[] { AppType.Game,AppType.Map })]
    public class Pets_ExpConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Pets_ExpConfig> JsonDic = new Dictionary<int, Pets_ExpConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Pets_ExpConfigJson(string b_ReadStr)
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
            List<Pets_ExpConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Pets_ExpConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Pets_ExpConfigJson);
    }
}