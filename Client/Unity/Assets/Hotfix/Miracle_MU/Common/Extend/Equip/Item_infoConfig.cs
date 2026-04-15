using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{

    ///<summary>
    ///物品 信息类
    /// </summary>
    public class Item_infoConfig
    {
        public long Id { get; set; }
        ///<summary>卓越装备id </summary>
        public long ExcItemId;
        ///<summary>物品ID </summary>
        public long ItemId;
        ///<summary>名字 </summary>
        public string Name;
        ///<summary>资源名 </summary>
        public string ResName;
        ///<summary>装备卡槽 </summary>
        public int Slot;
        ///<summary>类型 </summary>
        public int Type;
        ///<summary>技能 </summary>
        public int Skill;
        ///<summary>宽 </summary>
        public int X;
        ///<summary>高 </summary>
        public int Y;
        ///<summary>单组数量 </summary>
        public int StackSize;
        ///<summary>可以掉落 </summary>
        public int Drop;
        ///<summary>掉落等级 </summary>
        public int DropLevel;
        ///<summary>是卓越物品 </summary>
        public int IsExc;
        ///<summary>追加属性 
        ///1.追加攻击力   4 8 12 16
        //2.追加魔法攻击力   4 8 12 16
        //3.追加防御率   4 8 12 16
        //4.追加防御力   4 8 12 16
        //5.生命自动回复  1 2 3 4
        //6.攻击力/魔法攻击力/诅咒能力增加 4 8 12 16
        //7.追加防御力   5 10 15 20
        /// </summary>
        public List<int> AppendAttrId;
        ///<summary>基础属性</summary>
        public List<int> BaseAttrId;
        ///<summary>400 </summary>
        public int Is400;
        ///<summary>双手武器 </summary>
        public int TwoHand;
        ///<summary>最小伤害 </summary>
        public int DamageMin;
        ///<summary>最大伤害 </summary>
        public int DamageMax;
        ///<summary>诅咒 </summary>
        public int Curse;
        ///<summary>宠物提升 </summary>
        public int UpPet;
        ///<summary>魔力百分比 </summary>
        public int MagicPct;
        ///<summary>攻击速度 </summary>
        public int AttackSpeed;
        ///<summary>移动速度 </summary>
        public int WalkSpeed;
        ///<summary>防御 </summary>
        public int Defense;
        ///<summary>防御率 </summary>
        public int DefenseRate;
        ///<summary>耐久 </summary>
        public int Durable;
        ///<summary>需求等级 </summary>
        public int ReqLvl;
        ///<summary>力量 </summary>
        public int ReqStr;
        ///<summary>敏捷 </summary>
        public int ReqAgi;
        ///<summary>体力 </summary>
        public int ReqVit;
        ///<summary>智力 </summary>
        public int ReqEne;
        ///<summary>统率 </summary>
        public int ReqCom;
        ///<summary>使用职业 </summary>
        public string UseRole;
        ///<summary>是否可以使用 </summary>
        public bool IsCanUser;
        ///<summary>提示 </summary>
        public string Prompt;
        ///<summary>出售 1可。0不可 </summary>
        public int Sell;
        ///<summary>普通转隔掉落</summary>
        public int NormalDropWeight;
        ///<summary>追加装备掉落</summary>
        public int AppendDropWeight;
        ///<summary>技能装备掉落</summary>
        public int SkillDropWeight;
        ///<summary>幸运装备掉落</summary>
        public int LuckyDropWeight;  
        ///<summary>卓越装备掉落</summary>
        public int ExcellentDropWeight;
        ///<summary>套装装备掉落</summary>
        public int SetDropWeight;
        ///<summary>镶嵌装备</summary>
        public int SocketDropWeight;
    }
}
