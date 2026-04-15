using CustomFrameWork.Baseic;
using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class GMKeyComponent : TCustomComponent<MainFactory>
    {
        public GMToolKey gMToolKey = new GMToolKey();

        public Dictionary<string, DBGMToolAccount> AccountDic;
        public string Index;
    }
}
