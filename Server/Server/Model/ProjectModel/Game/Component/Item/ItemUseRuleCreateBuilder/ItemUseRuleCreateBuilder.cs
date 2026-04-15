using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class ItemUseRuleAttribute : CustomFrameWork.BaseAttribute
    {
        public string ItemUseRuleName { get; }
        public ItemUseRuleAttribute(Type b_ItemUseRule)
        {
            this.ItemUseRuleName = b_ItemUseRule.Name;
        }
    }
    public abstract class C_ItemUseRule<T, T1, T2> : ADataContext
    {
        public virtual async Task Run(T b_Player, T1 b_Item, T2 b_Response)
        {

        }
    }

    public sealed class ItemUseRuleCreateBuilder : TCustomComponent<MainFactory>
    {
        public Dictionary<string, Type> CacheDatas = new Dictionary<string, Type>();


        public override void Dispose()
        {
            if (this.IsDisposeable)
            {
                return;
            }

            if (CacheDatas != null)
            {
                if (CacheDatas.Count > 0)
                {
                    CacheDatas.Clear();
                }
            }


            base.Dispose();
        }
    }
}