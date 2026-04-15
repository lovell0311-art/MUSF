using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class LoginInfo
    {
        public long UserId;
        public long GameUserId;
        public int GateServerId;
        public int GameServerId;
        public string ConnectIp;
        public LoginInfo()
        {
            UserId = 0;
            GameUserId = 0;
            GateServerId = 0;
            GameServerId = 0;
            ConnectIp = null;
        }
    }


    public class LoginInfoRecordComponent : TCustomComponent<MainFactory>
    {
        private Dictionary<long, LoginInfo> LoginInfoDict = new Dictionary<long, LoginInfo>();

        public override void Dispose()
        {
            if (IsDisposeable) return;


            LoginInfoDict.Clear();

            base.Dispose();
        }

        public bool IsExist(long userId)
        {
            return LoginInfoDict.ContainsKey(userId);
        }

        public LoginInfo Get(long userId)
        {
            if(LoginInfoDict.TryGetValue(userId,out LoginInfo info))
            {
                return info;
            }
            return null;
        }

        public void Add(LoginInfo info)
        {
            LoginInfoDict[info.UserId] = info;
        }

        public void Remove(long userId)
        {
            LoginInfoDict.Remove(userId);
        }

    }
}
