using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// T特殊掉落-点卡2.0.xlsx-特殊掉落
    /// </summary>
    public partial class BoosEnemy_DropConfig : C_ConfigInfo
    {
        /// <summary>
        /// 掉落组ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 不重复
        /// </summary>
        public int NotRepeat { get; set; }
        /// <summary>
        /// #必掉道具
        /// </summary>
        public Dictionary<int,int> MustFall { get; set; }
        /// <summary>
        /// #数量权重
        /// </summary>
        public Dictionary<int,int> CountRate { get; set; }
    }
    /// <summary>
    /// 配置数据:T特殊掉落-点卡2.0.xlsx-特殊掉落
    /// </summary>
    [ReadConfigAttribute(typeof(BoosEnemy_DropConfig), new AppType[] { AppType.Game })]
    public class BoosEnemy_DropConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, BoosEnemy_DropConfig> JsonDic = new Dictionary<int, BoosEnemy_DropConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public BoosEnemy_DropConfigJson(string b_ReadStr)
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
            List<BoosEnemy_DropConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<BoosEnemy_DropConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(BoosEnemy_DropConfigJson);
    }
}