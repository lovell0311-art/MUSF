using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// D大师.xlsx-剑士
    /// </summary>
    public partial class BattleMaster_SwordsmanConfig
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
    public partial class BattleMaster_DreamKnightConfig
    {
        /// <summary>
        /// 附加数值
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> OtherDataDic { get; set; }

        public override void InitExpand()
        {
            OtherDataDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(OtherData);

        }
    }
    public partial class BattleMaster_CombatConfig
    {
        /// <summary>
        /// 附加数值
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> OtherDataDic { get; set; }

        public override void InitExpand()
        {
            OtherDataDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(OtherData);

        }
    }
}