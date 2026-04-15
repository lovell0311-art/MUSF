using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// W物品套装-点卡2.0.xlsx-套装类型
    /// </summary>
    public partial class SetItem_TypeConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 套装名
        /// </summary>
        public string SetName { get; set; }
        /// <summary>
        /// 套装物品id
        /// </summary>
        public List<int> ItemsId { get; set; }
        /// <summary>
        /// 选择概率
        /// </summary>
        public int Rate { get; set; }
        /// <summary>
        /// 2件套装属性
        /// </summary>
        public List<int> AttrId2 { get; set; }
        /// <summary>
        /// 3件套装属性
        /// </summary>
        public List<int> AttrId3 { get; set; }
        /// <summary>
        /// 4件套装属性
        /// </summary>
        public List<int> AttrId4 { get; set; }
        /// <summary>
        /// 5件套装属性
        /// </summary>
        public List<int> AttrId5 { get; set; }
        /// <summary>
        /// 6件套装属性
        /// </summary>
        public List<int> AttrId6 { get; set; }
        /// <summary>
        /// 7件套装属性
        /// </summary>
        public List<int> AttrId7 { get; set; }
        /// <summary>
        /// 8件套装属性
        /// </summary>
        public List<int> AttrId8 { get; set; }
        /// <summary>
        /// 9件套装属性
        /// </summary>
        public List<int> AttrId9 { get; set; }
        /// <summary>
        /// 10件套装属性
        /// </summary>
        public List<int> AttrId10 { get; set; }
    }
    /// <summary>
    /// 配置数据:W物品套装-点卡2.0.xlsx-套装类型
    /// </summary>
    [ReadConfigAttribute(typeof(SetItem_TypeConfig), new AppType[] { AppType.Game,AppType.GM })]
    public class SetItem_TypeConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, SetItem_TypeConfig> JsonDic = new Dictionary<int, SetItem_TypeConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public SetItem_TypeConfigJson(string b_ReadStr)
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
            List<SetItem_TypeConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<SetItem_TypeConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(SetItem_TypeConfigJson);
    }
}