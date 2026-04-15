using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// W物品.xlsx-项链
    /// </summary>
    public partial class Item_NecklaceConfig
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
            conf.QualityAttr = QualityAttr;
            conf.Level = Level;
            conf.AppendAttrId = new List<int>(AppendAttrId);
            conf.BaseAttrId = new List<int>(BaseAttrId);
            conf.ExtraAttrId = new List<int>(ExtraAttrId);
            conf.Durable = Durable;
            conf.ReqLvl = ReqLvl;
            conf.UseRole = UseRoleDic;
            conf.UpdatePropMethod = UpdatePropMethod;
            conf.Sell = Sell;
            return conf;
        }
    }
}