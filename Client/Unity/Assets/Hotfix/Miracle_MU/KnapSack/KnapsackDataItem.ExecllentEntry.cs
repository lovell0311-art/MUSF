using System.Collections.Generic;

namespace ETHotfix
{
    /// <summary>
    /// 卓越属性
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 卓越属性
        /// key:配置表ID
        /// value:属性等级
        /// </summary>
        public Dictionary<int, int> ExecllentEntryDic = new Dictionary<int, int>();
        /// <summary>
        /// 是否有卓越属性
        /// </summary>
        public bool IsHaveExecllentEntry => ExecllentEntryDic.Count > 0;

        public void SetExecllentEntry(Struct_AttrEntry property)
        {
            ExecllentEntryDic[property.PropId] = property.Level;
        }
        public void SetExecllentEntry(int key,int value)
        {
            ExecllentEntryDic[key] = value;
        }

        /// <summary>
        /// 获取卓越属性
        /// </summary>
        /// <param name="list"></param>
        public void GetExecllentEntry(ref List<string> list)
        {
            foreach (var item in ExecllentEntryDic)
            {
                ItemAttrEntry_ExcConfig itemAttrEntry_Exc = ConfigComponent.Instance.GetItem<ItemAttrEntry_ExcConfig>(item.Key);
              
                list.Add($"<color={ColorTools.LuckyItemColor}>{itemAttrEntry_Exc.Name}</color>");
              
            }
            if (ExecllentEntryDic.Count == 0) return;
            list.Add("");
        }
    }
}
