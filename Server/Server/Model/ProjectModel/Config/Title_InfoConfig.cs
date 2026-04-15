using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// C称号-点卡2.0.xlsx-称号
    /// </summary>
    public partial class Title_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 称号
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 属性1
        /// </summary>
        public string TitleAttribute { get; set; }
        /// <summary>
        /// 持续时长
        /// </summary>
        public int Duration { get; set; }
        /// <summary>
        /// 资源名
        /// </summary>
        public string AsstetName { get; set; }
        /// <summary>
        /// 获取途径
        /// </summary>
        public string GetWay { get; set; }
        /// <summary>
        /// 附加属性描述
        /// </summary>
        public string AttributeDescribe { get; set; }
    }
    /// <summary>
    /// 配置数据:C称号-点卡2.0.xlsx-称号
    /// </summary>
    [ReadConfigAttribute(typeof(Title_InfoConfig), new AppType[] { AppType.Game })]
    public class Title_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Title_InfoConfig> JsonDic = new Dictionary<int, Title_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Title_InfoConfigJson(string b_ReadStr)
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
            List<Title_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Title_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Title_InfoConfigJson);
    }
}