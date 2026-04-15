using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// W物品.xlsx-技能书|石
    /// </summary>
    public partial class Item_SkillBooksConfig 
    {
        /// <summary>
        /// 使用职业
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> UseRoleDic { get; set; }
        [JsonIgnore] public Dictionary<int, int> ValueDic { get; set; }
        public override void InitExpand()
        {
            UseRoleDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(UseRole);
            ValueDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(Value);
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
            conf.ReqLvl = ReqLvl;
            conf.ReqStr = ReqStr;
            conf.ReqAgi = ReqAgi;
            conf.ReqVit = ReqVit;
            conf.ReqEne = ReqEne;
            conf.ReqCom = ReqCom;
            conf.UseRole = UseRoleDic;
            conf.UpdatePropMethod = UpdatePropMethod;
            conf.Sell = Sell;
            foreach (var kv in ValueDic)
            {
                conf.GameOccupation2SkillId.Add((E_GameOccupation)kv.Key,kv.Value);
            }
            return conf;
        }
    }
}