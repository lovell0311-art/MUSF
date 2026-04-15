using System;
using System.Collections.Generic;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;

namespace ETModel
{
    public sealed class SkillCreateBuilder : TCustomComponent<MainFactory>
    {
        public Dictionary<int, (Type, C_ConfigInfo)> CacheDatas = new Dictionary<int, (Type, C_ConfigInfo)>();


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