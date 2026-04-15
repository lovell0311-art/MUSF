using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// J角色-点卡2.0.xlsx-角色经验
    /// </summary>
    public partial class Role_ExperienceConfig : C_ConfigInfo
    {
        /// <summary>
        /// 唯一id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 升级所需经验
        /// </summary>
        public long Exprience { get; set; }
        /// <summary>
        /// 升级累积需要经验
        /// </summary>
        public long ExprenceLevel { get; set; }
    }
    /// <summary>
    /// 配置数据:J角色-点卡2.0.xlsx-角色经验
    /// </summary>
    [ReadConfigAttribute(typeof(Role_ExperienceConfig), new AppType[] { AppType.Game })]
    public class Role_ExperienceConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Role_ExperienceConfig> JsonDic = new Dictionary<int, Role_ExperienceConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Role_ExperienceConfigJson(string b_ReadStr)
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
            List<Role_ExperienceConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Role_ExperienceConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Role_ExperienceConfigJson);
    }
}