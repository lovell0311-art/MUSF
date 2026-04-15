using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// W物品.xlsx-任务物品
    /// </summary>
    public partial class Item_PetConfig
    {


        /// <summary>
        /// 使用职业
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> UseRoleDic { get; set; }

        public override void InitExpand()
        {
            UseRoleDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(UseRole);
        }

        public ItemConfig ToItemConfig()
        {
            var conf = new ItemConfig();
            conf.Id = Id;
            conf.KindName = "宠物";
            conf.Name = Name;
            conf.Slot = Slot;
            conf.X = X;
            conf.Y = Y;
            conf.StackSize = StackSize;
            conf.Drop = Drop;
            conf.QualityAttr = QualityAttr;
            conf.Level = Level;
            conf.BaseAttrId = new List<int>(BaseAttrId);
            conf.UseRole = UseRoleDic;
            conf.Sell = Sell;
            conf.PetsId = PetId;
            conf.UseMethod = UseMethod;
            return conf;
        }
    }
}