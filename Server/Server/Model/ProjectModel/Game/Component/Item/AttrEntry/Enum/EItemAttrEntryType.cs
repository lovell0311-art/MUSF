using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public enum EItemAttrEntryType
    {
        None = -1,
        Base = 0,       // 基础属性
        Set = 1,        // 套装
        Excellent = 2,  // 卓越
        Regen = 3,      // 再生
        Entry380 = 4,   // 380
        Special = 6,    // 特殊 用于翅膀
        Extra = 7,      // 额外 用于套装
        Append = 8,     // 追加
    }
}
