using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETModel
{
    /// <summary>
    /// 物品自定义价格
    /// </summary>
    public class ItemPriceComponent : TCustomComponent<CustomComponent>
    {
        public Dictionary<int,ItemPrice> __ItemPriceDict = new Dictionary<int, ItemPrice>();
        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            __ItemPriceDict.Clear();

            base.Dispose();
        }
    }
}
