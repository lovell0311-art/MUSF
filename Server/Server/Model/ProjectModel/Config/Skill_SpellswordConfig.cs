using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// J技能-点卡2.0.xlsx-魔剑士
    /// </summary>
    public partial class Skill_SpellswordConfig : C_ConfigInfo
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 介绍
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 技能图标
        /// </summary>
        public string Icon { get; set; }
        /// <summary>
        /// 技能音效
        /// </summary>
        public string SoundName { get; set; }
        /// <summary>
        /// 动画技能ID
        /// </summary>
        public string AnimatorTriggerIndex { get; set; }
        /// <summary>
        /// 攻击特效
        /// </summary>
        public string AttackEffect { get; set; }
        /// <summary>
        /// 受击特效
        /// </summary>
        public string HitEffect { get; set; }
        /// <summary>
        /// 技能位置
        /// </summary>
        public int skillType { get; set; }
        /// <summary>
        /// 伤害延迟
        /// </summary>
        public int DamageWait { get; set; }
        /// <summary>
        /// 伤害延迟2
        /// </summary>
        public int DamageWait2 { get; set; }
        /// <summary>
        /// 最小动作时间
        /// </summary>
        public int MinActionTime { get; set; }
        /// <summary>
        /// 最大动作时间
        /// </summary>
        public int MaxActionTime { get; set; }
        /// <summary>
        /// 最大释放距离
        /// </summary>
        public int Distance { get; set; }
        /// <summary>
        /// 技能CD
        /// </summary>
        public int CoolTime { get; set; }
        /// <summary>
        /// 持续时间(毫秒)
        /// </summary>
        public int PersistentTime { get; set; }
        /// <summary>
        /// 消耗
        /// </summary>
        public string Consume { get; set; }
        /// <summary>
        /// 附加数值
        /// </summary>
        public string OtherData { get; set; }
        /// <summary>
        /// 使用要求
        /// </summary>
        public string UseStandard { get; set; }
    }
    /// <summary>
    /// 配置数据:J技能-点卡2.0.xlsx-魔剑士
    /// </summary>
    [ReadConfigAttribute(typeof(Skill_SpellswordConfig), new AppType[] { AppType.Game,AppType.Map,AppType.Robot })]
    public class Skill_SpellswordConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Skill_SpellswordConfig> JsonDic = new Dictionary<int, Skill_SpellswordConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Skill_SpellswordConfigJson(string b_ReadStr)
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
            List<Skill_SpellswordConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Skill_SpellswordConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Skill_SpellswordConfigJson);
    }
}