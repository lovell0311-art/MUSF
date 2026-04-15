using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class ItemCustomAttrMethodAttribute : BaseAttribute
    {
        public ItemCustomAttrMethodAttribute() { }
    }

    public interface IItemCustomAttrMethodHandler
    {
        public void Run(Item item);
    }



    public class ItemCustomAttrMethodCreateBuilder : TCustomComponent<MainFactory>
    {
        public Dictionary<string, IItemCustomAttrMethodHandler> MethodDict = new Dictionary<string, IItemCustomAttrMethodHandler>();

        public override void Dispose()
        {
            if (IsDisposeable) return;
            MethodDict.Clear();

            base.Dispose();
        }

    }
}
