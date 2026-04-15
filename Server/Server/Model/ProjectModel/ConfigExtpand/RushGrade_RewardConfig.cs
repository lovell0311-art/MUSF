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
    /// ³å¼¶»î¶¯
    /// </summary>
    public partial class RushGrade_RewardConfig
    {
        [JsonIgnore] public List<int> RewardList { get; set; }

        public override void InitExpand()
        {
            //RewardList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<int>>(ItemID);
        }
    }
}