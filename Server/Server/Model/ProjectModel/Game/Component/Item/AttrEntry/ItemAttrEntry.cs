using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    /// <summary>
    /// 物品属性词条
    /// </summary>
    public class ItemAttrEntry 
    {
        public readonly int ConfigId;
        private EItemAttrEntryType type = EItemAttrEntryType.None;
        public EItemAttrEntryType Type { get
            {
                if (type == EItemAttrEntryType.None)
                {
                    type = (EItemAttrEntryType)(ConfigId / 100000);
                }
                return type;
            }
        }

        public EItemAttrEntryPropId PropId;
        public readonly int Level;
        /// <summary>
        /// 属性会根据耐久衰减
        /// </summary>
        public bool WillWeaken = false;
        public int Value;
        public int AppendValue = 0;

        public ItemAttrEntry(int configId,int level)
        {
            ConfigId = configId;
            Level = level;
        }
    }
}
