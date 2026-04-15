using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// D大师.xlsx-新增大师技能
    /// </summary>
    public partial class BattleMaster_ALLConfig
    {
        /// <summary>
        /// 附加数值
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> OtherDataDic { get; set; }
        [JsonIgnore] public Dictionary<int, int> UseRoleDic { get; set; }
        
        public override void InitExpand()
        {
            OtherDataDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(OtherData);
            UseRoleDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(UseRole);
        }
    }
}