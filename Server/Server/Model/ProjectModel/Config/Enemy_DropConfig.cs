using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// G怪物_点卡2.0.xlsx-怪物掉落概率
    /// </summary>
    public partial class Enemy_DropConfig : C_ConfigInfo
    {
        /// <summary>
        /// ID
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 怪物等级
        /// </summary>
        public int MonsterLevel { get; set; }
        /// <summary>
        /// 装备
        /// </summary>
        public int Equip { get; set; }
        /// <summary>
        /// 项链
        /// </summary>
        public int Necklace { get; set; }
        /// <summary>
        /// 戒指
        /// </summary>
        public int Ring { get; set; }
        /// <summary>
        /// 技能书|石
        /// </summary>
        public int SkillBook { get; set; }
        /// <summary>
        /// 消耗品
        /// </summary>
        public int Consumables { get; set; }
        /// <summary>
        /// 光之石
        /// </summary>
        public int LightStone { get; set; }
        /// <summary>
        /// 玛雅之石
        /// </summary>
        public int MayaStone { get; set; }
        /// <summary>
        /// 祝福宝石
        /// </summary>
        public int BlessingGem { get; set; }
        /// <summary>
        /// 灵魂宝石
        /// </summary>
        public int SoulGem { get; set; }
        /// <summary>
        /// 生命宝石
        /// </summary>
        public int LifeGem { get; set; }
        /// <summary>
        /// 创造宝石
        /// </summary>
        public int CreateGem { get; set; }
        /// <summary>
        /// 守护宝石
        /// </summary>
        public int GuardGem { get; set; }
        /// <summary>
        /// 再生原石
        /// </summary>
        public int ReviveOriginalStone { get; set; }
        /// <summary>
        /// 荧光宝石火
        /// </summary>
        public int FluorescentDropsFire { get; set; }
        /// <summary>
        /// 荧光宝石土
        /// </summary>
        public int FluorescentDropsSoil { get; set; }
        /// <summary>
        /// 荧光宝石雷
        /// </summary>
        public int FluorescentDropsMine { get; set; }
        /// <summary>
        /// 荧光宝石风
        /// </summary>
        public int FluorescentDropsWind { get; set; }
        /// <summary>
        /// 荧光宝石冰
        /// </summary>
        public int FluorescentDropsIce { get; set; }
        /// <summary>
        /// 荧光宝石水
        /// </summary>
        public int FluorescentDropsWater { get; set; }
        /// <summary>
        /// 幸运宝石
        /// </summary>
        public int LuckyGem { get; set; }
        /// <summary>
        /// 卓越宝石
        /// </summary>
        public int ExcellentGem { get; set; }
        /// <summary>
        /// 金币
        /// </summary>
        public int MiracleCoin { get; set; }
        /// <summary>
        /// 金币
        /// </summary>
        public int GoldCoin { get; set; }
        /// <summary>
        /// 无掉落
        /// </summary>
        public int NoDrop { get; set; }
        /// <summary>
        /// 之和
        /// </summary>
        public int sum { get; set; }
    }
    /// <summary>
    /// 配置数据:G怪物_点卡2.0.xlsx-怪物掉落概率
    /// </summary>
    [ReadConfigAttribute(typeof(Enemy_DropConfig), new AppType[] { AppType.Game,AppType.Map })]
    public class Enemy_DropConfigJson : C_ConfigJson
    {
        /// <summary>
        /// 反序列化后的配置数据
        /// </summary>
        public readonly Dictionary<int, Enemy_DropConfig> JsonDic = new Dictionary<int, Enemy_DropConfig>();
        /// <summary>
        /// 初始化配置表 更新表的信息
        /// </summary>
        /// <param name="b_ReadStr">新的数据集合</param>
        public Enemy_DropConfigJson(string b_ReadStr)
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
            List<Enemy_DropConfig> jsonData = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Enemy_DropConfig>>(b_ReadStr);
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
        public override Type ConfigType => typeof(Enemy_DropConfigJson);
    }
}