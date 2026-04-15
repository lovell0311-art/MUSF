using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// T提示语-点卡2.0.xlsx-提示语
    /// </summary>
    public partial class Tips_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 提示语
        /// </summary>
        public string TipsDescribe { get; set; }
    }
    /// <summary>
    /// 配置数据:T提示语-点卡2.0.xlsx-提示语
    /// </summary>
    [ReadConfigAttribute(typeof(Tips_InfoConfig), new AppType[] { AppType.AllServer })]
    public class Tips_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Tips_InfoConfig> JsonDic = new Dictionary<int, Tips_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Tips_InfoConfigJson(string b_ReadStr)
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
            List<Tips_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Tips_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Tips_InfoConfigJson);
    }
}