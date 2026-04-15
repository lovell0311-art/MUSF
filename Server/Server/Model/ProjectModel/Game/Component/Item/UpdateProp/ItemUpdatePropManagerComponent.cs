using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork.Baseic;


namespace ETModel
{
#if SERVER
    public class ItemUpdatePropManagerComponent : TCustomComponent<CustomComponent>
#else
    public class ItemUpdatePropManagerComponent : TCustomComponent<CustomComponent>
#endif
    {
        public readonly Dictionary<string, IItemUpdatePropHandler> UpdateMethodDict = new Dictionary<string, IItemUpdatePropHandler> ();

        public IItemUpdatePropHandler DefaultHandler;
        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            UpdateMethodDict.Clear();
            DefaultHandler = null;

            base.Dispose();
        }
    }
}
