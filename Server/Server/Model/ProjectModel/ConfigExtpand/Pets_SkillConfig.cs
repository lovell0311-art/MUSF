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
    /// C宠物.xlsx-技能
    /// </summary>
    public partial class Pets_SkillConfig
    {

        /// <summary>
        /// 学习要求
        /// </summary>
        //[JsonIgnore] public List<int> LearnStandardDic { get; set; }
        /// <summary>
        /// 消耗
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> ConsumeDic { get; set; }
        /// <summary>
        /// 附加数值
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> OtherDataDic { get; set; }
        /// <summary>
        /// 使用要求
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> UseStandardDic { get; set; }

        public override void InitExpand()
        {
            ConsumeDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(Consume);
            OtherDataDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(OtherData);
            UseStandardDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(UseStandard);
            //LearnStandardDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(LearnStandard);
            //LearnStandardDic = Help_JsonSerializeHelper.DeSerialize < List<int>>(LearnStandard);

        }
    }
}