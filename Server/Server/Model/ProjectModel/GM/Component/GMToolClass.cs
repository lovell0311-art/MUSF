using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class GMReturn
    {
        public bool Succeed;
        public string Msg = "";
    }
    public class GMToolKey : DBBase
    {
        public string Key;
    }
    public class DBGMToolAccount : DBBase
    {
        public string Account;
        public string Password;
        public int AccountLevel;
    }
}
