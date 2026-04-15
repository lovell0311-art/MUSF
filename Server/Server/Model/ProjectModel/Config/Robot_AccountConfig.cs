using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// AI机器人-点卡2.0.xlsx-机器人账号
    /// </summary>
    public partial class Robot_AccountConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Passwd { get; set; }
        /// <summary>
        /// 区id
        /// </summary>
        public int ZoneId { get; set; }
        /// <summary>
        /// 线id
        /// </summary>
        public int LineId { get; set; }
    }
    /// <summary>
    /// 配置数据:AI机器人-点卡2.0.xlsx-机器人账号
    /// </summary>
    [ReadConfigAttribute(typeof(Robot_AccountConfig), new AppType[] { AppType.Robot })]
    public class Robot_AccountConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Robot_AccountConfig> JsonDic = new Dictionary<int, Robot_AccountConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Robot_AccountConfigJson(string b_ReadStr)
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
            List<Robot_AccountConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Robot_AccountConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Robot_AccountConfigJson);
    }
}