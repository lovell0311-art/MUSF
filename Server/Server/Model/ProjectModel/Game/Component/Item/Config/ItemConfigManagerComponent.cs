using CustomFrameWork.Baseic;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class ItemConfigManagerComponent : TCustomComponent<CustomComponent>
    {
        public readonly Dictionary<int, ItemConfig> ItemConfigDict = new Dictionary<int, ItemConfig>();
        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            ItemConfigDict.Clear();

            base.Dispose();
        }

    }
}
