using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// Z坐骑-点卡2.0.xlsx-宝箱类型
    /// </summary>
    public partial class Mounts_DarkHorseExpConfig : C_ConfigInfo
    {
        /// <summary>
        /// 物品id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 经验
        /// </summary>
        public int Exp { get; set; }
    }
    /// <summary>
    /// 配置数据:Z坐骑-点卡2.0.xlsx-宝箱类型
    /// </summary>
    [ReadConfigAttribute(typeof(Mounts_DarkHorseExpConfig), new AppType[] { AppType.Game })]
    public class Mounts_DarkHorseExpConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Mounts_DarkHorseExpConfig> JsonDic = new Dictionary<int, Mounts_DarkHorseExpConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Mounts_DarkHorseExpConfigJson(string b_ReadStr)
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
            List<Mounts_DarkHorseExpConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Mounts_DarkHorseExpConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Mounts_DarkHorseExpConfigJson);
    }
}