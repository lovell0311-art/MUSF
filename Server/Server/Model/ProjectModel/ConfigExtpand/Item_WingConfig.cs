using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// W物品.xlsx-翅膀
    /// </summary>
    public partial class Item_WingConfig
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
            conf.Name = Name;
            conf.Slot = Slot;
            conf.Skill = Skill;
            conf.X = X;
            conf.Y = Y;
            conf.StackSize = StackSize;
            conf.Drop = Drop;
            conf.AppendAttrId = new List<int>(AppendAttrId);
            conf.BaseAttrId = new List<int>(BaseAttrId);
            conf.SpecialAttrId = new List<int>(SpecialAttrId);
            conf.Level = Level;
            conf.Defense = Defense;
            conf.Durable = Durable;
            conf.DamagePct = DamagePct;
            conf.DamageAbsPct = DamageAbsPct;
            conf.WingLevel = WingLevel;
            conf.ReqLvl = ReqLvl;
            conf.UseRole = UseRoleDic;
            conf.UpdatePropMethod = UpdatePropMethod;
            conf.Sell = Sell;
            return conf;
        }
    }
}