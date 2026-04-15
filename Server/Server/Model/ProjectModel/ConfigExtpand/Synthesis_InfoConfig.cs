using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// Synthesis.xlsx
    /// </summary>
    public partial class Synthesis_InfoConfig
    {

        /// <summary>
        /// 附加信息
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> NeedItemsDic { get; set; }
        public override void InitExpand()
        {
            NeedItemsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(NeedItems); 
        }
    }
}