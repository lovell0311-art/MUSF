using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ETModel
{
    public partial class AttributeNode_InfoConfig
    {
        /// <summary>
        /// 激活需求
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> ActivateNeedDic { get; set; }
        public override void InitExpand()
        {
            ActivateNeedDic = new Dictionary<int, int>();
           
            ActivateNeedDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(ActivateNeed);
        }
    }
}
