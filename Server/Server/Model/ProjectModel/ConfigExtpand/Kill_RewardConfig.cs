using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using TencentCloud.Tdmq.V20200217.Models;
using Newtonsoft.Json;
using CustomFrameWork;

namespace ETModel
{
    /// <summary>
    /// ÄêÊÞ»î¶¯
    /// </summary>
    public partial class Kill_RewardConfig
    {
        [JsonIgnore] public Dictionary<int, int> RewardList { get; set; }
        [JsonIgnore] public Dictionary<int,int> BrushTimeDic { get; set; }
        [JsonIgnore] public List<int> ExistenceTimeDic { get; set; }
        public override void InitExpand()
        {
            //RewardList = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,int>>(Drop);
            BrushTimeDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int,int>>(BrushTime);
            //ExistenceTimeDic = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(BrushTime);
        }
    }
}