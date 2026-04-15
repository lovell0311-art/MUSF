using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// F副本-点卡2.0.xlsx-副本进入条件
    /// </summary>
    public partial class BattleIntoInfo_IntoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 副本名字
        /// </summary>
        public string BattleName { get; set; }
        /// <summary>
        /// 入场角色等级1
        /// </summary>
        public string IntoRoleLevel1 { get; set; }
        /// <summary>
        /// 入场角色等级2
        /// </summary>
        public string IntoRoleLevel2 { get; set; }
        /// <summary>
        /// 入场角色等级3
        /// </summary>
        public string IntoRoleLevel3 { get; set; }
        /// <summary>
        /// 入场角色等级4
        /// </summary>
        public string IntoRoleLevel4 { get; set; }
        /// <summary>
        /// 入场角色等级5
        /// </summary>
        public string IntoRoleLevel5 { get; set; }
        /// <summary>
        /// 入场角色等级6
        /// </summary>
        public string IntoRoleLevel6 { get; set; }
        /// <summary>
        /// 入场角色等级7
        /// </summary>
        public string IntoRoleLevel7 { get; set; }
    }
    /// <summary>
    /// 配置数据:F副本-点卡2.0.xlsx-副本进入条件
    /// </summary>
    [ReadConfigAttribute(typeof(BattleIntoInfo_IntoConfig), new AppType[] { AppType.Game })]
    public class BattleIntoInfo_IntoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, BattleIntoInfo_IntoConfig> JsonDic = new Dictionary<int, BattleIntoInfo_IntoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public BattleIntoInfo_IntoConfigJson(string b_ReadStr)
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
            List<BattleIntoInfo_IntoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BattleIntoInfo_IntoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(BattleIntoInfo_IntoConfigJson);
    }
}