using CustomFrameWork.Baseic;
using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class NameLockComponent : TCustomComponent<MainFactory>
    {
        public List<string> strings { get; set; } = new List<string>();  
        public override void Awake()
        {
            strings = new List<string>();
        }
        public override void Dispose()
        {
            strings.Clear();
        }
    }
}
