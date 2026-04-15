using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Component;
using Newtonsoft.Json;
using TencentCloud.Cynosdb.V20190107.Models;

namespace ETModel
{
    /// <summary>
    /// 镶嵌属性
    /// </summary>
    public partial class FluoreSet_AttrConfig
    {
        public int ToItemConfig(int Id,int Level)
        {
            if(keyValuePairs.TryGetValue((Id,Level),out int Value))
                return Value;

            return Level0;
        }

        [JsonIgnore] public Dictionary<(int,int),int> keyValuePairs = new Dictionary<(int,int),int>();

        public override void InitExpand()
        {
            keyValuePairs.Add((Id, 0), Level0);
            keyValuePairs.Add((Id, 1), Level1);
            keyValuePairs.Add((Id, 2), Level2);
            keyValuePairs.Add((Id, 3), Level3);
            keyValuePairs.Add((Id, 4), Level4);
            keyValuePairs.Add((Id, 5), Level5);
            keyValuePairs.Add((Id, 6), Level6);
            keyValuePairs.Add((Id, 7), Level7);
            keyValuePairs.Add((Id, 8), Level8);
            keyValuePairs.Add((Id, 9), Level9);
        }
    }
}