using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// W物品.xlsx-装备
    /// </summary>
    public partial class Item_EquipmentConfig
    {

        /// <summary>
        /// 使用职业
        /// </summary>
        [JsonIgnore] public Dictionary<int,int> UseRoleDic { get; set; }

        public override void InitExpand()
        {
            UseRoleDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(UseRole);
        }

        public ItemConfig ToItemConfig()
        {
            var conf = new ItemConfig();
            conf.Id = Id;
            conf.KindName = KindName;
            conf.Name = Name;
            conf.Slot = Slot;
            conf.Skill = Skill;
            conf.X = X;
            conf.Y = Y;
            conf.StackSize = StackSize;
            conf.Drop = Drop;
            conf.QualityAttr = QualityAttr;
            conf.Level = Level;
            conf.BaseAttrId = new List<int>(BaseAttrId);
            conf.AppendAttrId = new List<int>(AppendAttrId);
            conf.ExtraAttrId = new List<int>(ExtraAttrId);
            conf.ExtraAttrId2 = new List<int>(ExtraAttrId2);
            conf.Is400 = Is400;
            conf.TwoHand = TwoHand;
            conf.DamageMin = DamageMin;
            conf.DamageMax = DamageMax;
            conf.Curse = Curse;
            conf.UpPet = UpPet;
            conf.MagicPct = MagicPct;
            conf.AttackSpeed = AttackSpeed;
            conf.WalkSpeed = WalkSpeed;
            conf.Defense = Defense;
            conf.DefenseRate = DefenseRate;
            conf.Durable = Durable;
            conf.ReqLvl = ReqLvl;
            conf.ReqStr = ReqStr;
            conf.ReqAgi = ReqAgi;
            conf.ReqVit = ReqVit;
            conf.ReqEne = ReqEne;
            conf.ReqCom = ReqCom;
            conf.UseRole = UseRoleDic;
            conf.UpdatePropMethod = UpdatePropMethod;
            conf.Sell = Sell;
            return conf;
        }

    }
}