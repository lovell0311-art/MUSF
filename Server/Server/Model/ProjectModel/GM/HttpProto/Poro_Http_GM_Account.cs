using System.Collections.Generic;

namespace ETModel.HttpProto
{
    public class BanParam
    {
        public string UserId { get; set; }
        public string BanTillTime { get; set; }
        public string BanReason { get; set; }
    }

    public class KickParam
    {
        public string UserId { get; set; }
        public string Reason { get; set; }
    }

}
