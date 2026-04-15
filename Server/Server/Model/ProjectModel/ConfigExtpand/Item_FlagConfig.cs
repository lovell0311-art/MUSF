using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// W物品.xlsx-旗帜
    /// </summary>
    public partial class Item_FlagConfig
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
            conf.QualityAttr = QualityAttr;
            conf.AppendAttrId = new List<int>(AppendAttrId);
            conf.BaseAttrId = new List<int>(BaseAttrId);
            conf.Level = Level;
            conf.Durable = Durable;
            conf.ReqLvl = ReqLvl;
            conf.UseRole = UseRoleDic;
            conf.UpdatePropMethod = UpdatePropMethod;
            conf.Sell = Sell;
            return conf;
        }
    }
}