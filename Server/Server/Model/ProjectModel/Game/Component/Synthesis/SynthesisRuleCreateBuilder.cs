using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class SynthesisRuleAttribute : CustomFrameWork.BaseAttribute
    {
        public string SynthesisRuleName { get; }
        public SynthesisRuleAttribute(Type b_SynthesisRule)
        {
            this.SynthesisRuleName = b_SynthesisRule.Name;
        }
    }
    public abstract class C_SynthesisRule<T, T1, T2, T3, T4> : ADataContext
    {
        public virtual async Task Run(T b_Player, T1 b_ItemList, T2 Method, T3 b_Response ,T4 client)
        {

        }
    }

    public sealed class SynthesisRuleCreateBuilder : TCustomComponent<MainFactory>
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