using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// Npc-点卡2.0.xlsx-npc
    /// </summary>
    public partial class Npc_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Npc名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 资源名
        /// </summary>
        public string ResName { get; set; }
        /// <summary>
        /// 小地图图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// Npc类型
        /// </summary>
        public int NpcType { get; set; }
        /// <summary>
        /// 描述,简介
        /// </summary>
        public string Info { get; set; }
        /// <summary>
        /// 附加信息
        /// </summary>
        public string OtherData { get; set; }
        /// <summary>
        /// 商店物品ID
        /// </summary>
        public string EquipData { get; set; }
    }
    /// <summary>
    /// 配置数据:Npc-点卡2.0.xlsx-npc
    /// </summary>
    [ReadConfigAttribute(typeof(Npc_InfoConfig), new AppType[] { AppType.Game,AppType.Map,AppType.Robot })]
    public class Npc_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Npc_InfoConfig> JsonDic = new Dictionary<int, Npc_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Npc_InfoConfigJson(string b_ReadStr)
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
            List<Npc_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Npc_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Npc_InfoConfigJson);
    }
}