using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// F服务器配置.xlsx-启动
    /// </summary>
    public partial class StartUp_ServerConfig : C_ConfigInfo
    {
        /// <summary>
        /// 进程id
        /// </summary>
        public int AppId { get; set; }
        /// <summary>
        /// 区id
        /// </summary>
        public int ZoneId { get; set; }
        /// <summary>
        /// 服务器类型
        /// </summary>
        public AppType AppType { get; set; }
        /// <summary>
        /// 内网IP
        /// </summary>
        public Dictionary<string,string> InnerConfig { get; set; }
        /// <summary>
        /// 外网IP
        /// </summary>
        public Dictionary<string,string> OuterConfig { get; set; }
        /// <summary>
        /// 是否打印日志
        /// </summary>
        public int PointLog { get; set; }
        /// <summary>
        /// 运行参数
        /// </summary>
        public string RunParameter { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 是否PVP
        /// </summary>
        public int IsPVP { get; set; }
        /// <summary>
        /// 是否会员
        /// </summary>
        public int IsVIP { get; set; }
    }
    /// <summary>
    /// 配置数据:F服务器配置.xlsx-启动
    /// </summary>
    [ReadConfigAttribute(typeof(StartUp_ServerConfig), new AppType[] { AppType.AllServer })]
    public class StartUp_ServerConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, StartUp_ServerConfig> JsonDic = new Dictionary<int, StartUp_ServerConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public StartUp_ServerConfigJson(string b_ReadStr)
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
            List<StartUp_ServerConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<StartUp_ServerConfig>>(b_ReadStr);
            for (int i = 0; i < jsonData.Count; i++)
            {
                var mConfig = jsonData[i];
                mConfig.InitExpand();
                JsonDic[mConfig.AppId] = mConfig;
            }
        }
        /// <summary>
        /// ConfigType
        /// </summary>
        public override Type ConfigType => typeof(StartUp_ServerConfigJson);
    }
}