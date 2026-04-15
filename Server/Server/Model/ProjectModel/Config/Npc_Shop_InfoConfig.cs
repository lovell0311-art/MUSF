using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// Npc_Shop-点卡2.0.xlsx-npc
    /// </summary>
    public partial class Npc_Shop_InfoConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 物品ID
        /// </summary>
        public int itemID { get; set; }
        /// <summary>
        /// 技能
        /// </summary>
        public int Skill { get; set; }
        /// <summary>
        /// 强化等级
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 追加ID
        /// </summary>
        public int OptValue { get; set; }
        /// <summary>
        /// 追加等级
        /// </summary>
        public int OptLevel { get; set; }
        /// <summary>
        /// 幸运属性
        /// </summary>
        public int LuckyAttribute { get; set; }
        /// <summary>
        /// 套装属性
        /// </summary>
        public int SetID { get; set; }
        /// <summary>
        /// 卓越属性
        /// </summary>
        public string OptionExcellent { get; set; }
        /// <summary>
        /// 380属性
        /// </summary>
        public int Option380 { get; set; }
        /// <summary>
        /// 再生属性
        /// </summary>
        public int OptionRebirth { get; set; }
        /// <summary>
        /// 再生属性等级
        /// </summary>
        public int OptionRebirthLevel { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public int Price { get; set; }
    }
    /// <summary>
    /// 配置数据:Npc_Shop-点卡2.0.xlsx-npc
    /// </summary>
    [ReadConfigAttribute(typeof(Npc_Shop_InfoConfig), new AppType[] { AppType.Game })]
    public class Npc_Shop_InfoConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Npc_Shop_InfoConfig> JsonDic = new Dictionary<int, Npc_Shop_InfoConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Npc_Shop_InfoConfigJson(string b_ReadStr)
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
            List<Npc_Shop_InfoConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Npc_Shop_InfoConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Npc_Shop_InfoConfigJson);
    }
}