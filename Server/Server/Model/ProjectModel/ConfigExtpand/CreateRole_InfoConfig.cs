using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;

namespace ETModel
{
    /// <summary>
    /// J角色.xlsx-角色
    /// </summary>
    public partial class CreateRole_InfoConfig
    {

        /// <summary>
        /// 升级获得
        /// </summary>
        [JsonIgnore] public Dictionary<int,int> AppendLevelDic { get; set; }
        [JsonIgnore] public Dictionary<int, int> MasterPointsDic { get; set; }
        public override void InitExpand()
        {
            AppendLevelDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(AppendLevel);
            MasterPointsDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(MasterPoints);

        }
    }
}