using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    public partial class Map_InfoConfig
    {
        /// <summary>
        /// 附加数值
        /// </summary>
        [JsonIgnore] public Dictionary<int, string> MonsterPathDic { get; set; }

        public override void InitExpand()
        {
            MonsterPathDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, string>>(MonsterPath);

        }
    }
}