using System;
using System.Collections.Generic;
using System.Text;
using ETModel;

namespace ETHotfix
{
    public static class EItemValueHelper
    {
        /// <summary>
        /// 这个属性需要保存
        /// </summary>
        /// <returns></returns>
        public static bool NeedToSave(this EItemValue self)
        {
            return Enum.IsDefined(typeof(EDBItemValue), (int)self);
        }
    }
}
