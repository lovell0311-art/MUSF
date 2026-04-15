using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// D大师.xlsx-法师
    /// </summary>
    public partial class BattleMaster_SpellConfig
    {
        /// <summary>
        /// 附加数值
        /// </summary>
        [JsonIgnore] public Dictionary<int,int> OtherDataDic { get; set; }

        public override void InitExpand()
        {
            OtherDataDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(OtherData);

        }
    }
}