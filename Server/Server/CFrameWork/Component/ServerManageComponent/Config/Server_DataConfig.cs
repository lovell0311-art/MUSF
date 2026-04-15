using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CustomFrameWork.Component
{
    /// <summary>
    /// S数据服务.xlsx-数据服务
    /// </summary>
    public class Server_DataConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 数据库地址
        /// </summary>
        public string DBConnection { get; set; }
        /// <summary>
        /// 数据库名
        /// </summary>
        public string DBName { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public int DBType { get; set; }
        /// <summary>
        /// 数据库区服
        /// </summary>
        public int DBZone { get; set; }
        /// <summary>
        /// appid
        /// </summary>
        public int AppId { get; set; }
    }
    /// <summary>
    /// 配置数据:S数据服务.xlsx-数据服务
    /// </summary>
    [ReadConfigAttribute(typeof(Server_DataConfig), new AppType[] { AppType.AllServer })]
    public class Server_DataConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Server_DataConfig> JsonDic = new Dictionary<int, Server_DataConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Server_DataConfigJson(string b_ReadStr)
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
            List<Server_DataConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Server_DataConfig>>(b_ReadStr);
            Server_DataConfig mServer_DataConfig;
            for (int i = 0; i < jsonData.Count; i++)
            {
                mServer_DataConfig = jsonData[i];
                JsonDic[mServer_DataConfig.Id] = mServer_DataConfig;
            }
        }
        /// <summary>
        /// ConfigType
        /// </summary>
        public override Type ConfigType => typeof(Server_DataConfigJson);
    }
}