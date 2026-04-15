using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// J技能.xlsx-怪物技能
    /// </summary>
    public partial class Skill_monsterConfig
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