using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// W物品.xlsx-消耗品(血瓶|药水|实力提升卷轴)
    /// </summary>
    public partial class Item_ConsumablesConfig
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
            conf.Level = Level;
            conf.Value = Value;
            conf.Value2 = Value2;
            conf.ReqLvl = ReqLvl;
            conf.ReqStr = ReqStr;
            conf.ReqAgi = ReqAgi;
            conf.ReqVit = ReqVit;
            conf.ReqEne = ReqEne;
            conf.ReqCom = ReqCom;
            conf.UseRole = UseRoleDic;
            conf.UseMethod = UseMethod;
            conf.UpdatePropMethod = UpdatePropMethod;
            conf.Sell = Sell;
            return conf;
        }

    }
}