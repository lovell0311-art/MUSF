using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;

using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// J技能.xlsx-弓箭手
    /// </summary>
    public partial class Skill_ArcherConfig
    {

        /// <summary>
        /// 消耗
        /// </summary>
        [JsonIgnore] public Dictionary<int,int> ConsumeDic { get; set; }
        /// <summary>
        /// 附加数值
        /// </summary>
        [JsonIgnore] public Dictionary<int,int> OtherDataDic { get; set; }
        /// <summary>
        /// 使用要求
        /// </summary>
        [JsonIgnore] public Dictionary<int,int> UseStandardDic { get; set; }

        public override void InitExpand()
        {
            ConsumeDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(Consume);
            OtherDataDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(OtherData);
            UseStandardDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(UseStandard);

        }
    }
}