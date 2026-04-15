using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ETModel
{
    public partial class BloodAwakening_InfoConfig
    {
        /// <summary>
        /// 激活需求
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> ActivateNeedDic { get; set; }
        /// <summary>
        /// 净化需求
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> PurityNeedDic { get; set; }
        /// <summary>
        /// 净化时间
        /// </summary>
        [JsonIgnore] public Dictionary<int, int> PurityTimeDic { get; set; }
        /// <summary>
        /// 所有环的属性节点
        /// </summary>
        [JsonIgnore] public Dictionary<int, Dictionary<int, bool>> AttributeNode { get; set; }
        public override void InitExpand()
        {
            ActivateNeedDic = new Dictionary<int, int>();
            PurityNeedDic = new Dictionary<int, int>();
            PurityTimeDic = new Dictionary<int, int>();
            AttributeNode = new Dictionary<int, Dictionary<int,bool>>();

            ActivateNeedDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(ActivateNeed);
            PurityNeedDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(PurityNeed);
            PurityTimeDic = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(PurityTime);
            
            Dictionary<int, bool> keyValuePairs = new Dictionary<int, bool>();
            Dictionary<int,int> keyValuePairs1 = new Dictionary<int, int>();
            keyValuePairs1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(AttributeNode1);
            foreach (var info in keyValuePairs1)
            {
                keyValuePairs.Add(info.Key, info.Value == 1 ? true : false);
            }
            AttributeNode.Add(1, keyValuePairs);

            keyValuePairs = new Dictionary<int, bool>();
            keyValuePairs1.Clear();
            keyValuePairs1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(AttributeNode2);
            foreach (var info in keyValuePairs1)
            {
                keyValuePairs.Add(info.Key, info.Value == 1 ? true : false);
            }
            AttributeNode.Add(2, keyValuePairs);

            keyValuePairs = new Dictionary<int, bool>();
            keyValuePairs1.Clear();
            keyValuePairs1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(AttributeNode3);
            foreach (var info in keyValuePairs1)
            {
                keyValuePairs.Add(info.Key, info.Value == 1 ? true : false);
            }
            AttributeNode.Add(3, keyValuePairs);

            keyValuePairs = new Dictionary<int, bool>();
            keyValuePairs1.Clear();
            keyValuePairs1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(AttributeNode4);
            foreach (var info in keyValuePairs1)
            {
                keyValuePairs.Add(info.Key, info.Value == 1 ? true : false);
            }
            AttributeNode.Add(4, keyValuePairs);

            keyValuePairs = new Dictionary<int, bool>();
            keyValuePairs1.Clear();
            keyValuePairs1 = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<int, int>>(AttributeNode5);
            foreach (var info in keyValuePairs1)
            {
                keyValuePairs.Add(info.Key, info.Value == 1 ? true : false);
            }
            AttributeNode.Add(5, keyValuePairs);
        }
    }
}
