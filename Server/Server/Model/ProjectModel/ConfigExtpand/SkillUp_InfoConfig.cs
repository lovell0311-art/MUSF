using System;
using System.Collections;
using System.Collections.Generic;
using CustomFrameWork;
using Newtonsoft.Json;

namespace ETModel
{
    public partial class SkillUp_InfoConfig
    {
        /// <summary>
        /// 升级需求材料
        /// </summary>
        [JsonIgnore] public Dictionary<int, Dictionary<int,int>> LvItemInfoDic { get; set; }
        /// <summary>
        /// 升级获得属性
        /// </summary>
        [JsonIgnore] public Dictionary<int, Dictionary<int, int>> LvUpGetDic { get; set; }
        public override void InitExpand()
        {
            LvItemInfoDic = new Dictionary<int, Dictionary<int, int>>();
            LvUpGetDic = new Dictionary<int, Dictionary<int, int>>();
            LvItemInfoDic = Help_JsonSerializeHelper.DeSerialize<Dictionary<int, Dictionary<int, int>>>(LvItemInfo);
            LvUpGetDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, Dictionary<int, int>>>(LvUpGet);
        }
    }
}
