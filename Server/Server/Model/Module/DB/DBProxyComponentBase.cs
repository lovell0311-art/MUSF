using System.Net;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 用来与数据库操作代理
    /// </summary>
    public class DBProxyComponentBase : ACustomComponent
    {
        public IPEndPoint dbAddress;

        public MultiMap<string, object> TcsQueue = new MultiMap<string, object>();
    }
}