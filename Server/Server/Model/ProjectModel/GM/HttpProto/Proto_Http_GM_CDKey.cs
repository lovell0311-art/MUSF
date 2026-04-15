using System.Collections.Generic;

namespace ETModel.HttpProto
{

    public class CDKeyAddTypeParam
    {
        public int ZoneId { get; set; }
        public List<MailItem> RewardList { get; set; }
    }


    public class CDKeyAddCodeParam
    {
        public int ZoneId { get; set; }
        public int RewardType { get; set; }
        public int Count { get; set; }
    }
}
