using System.Collections.Generic;

namespace ETModel.HttpProto
{
    public class GetLoginRecordParam
    {
        public string UserId { get; set; }
    }

    public class UpdateAccountIdentityParam
    {
        public string UserId { get; set; }
    }

    public class SendMailParam
    {
        public int ZoneId { get; set; }
        public string UserId { get; set; }
        public string GameUserId { get; set; }

        public string Name { get; set; }
        public string Content { get; set; }
        public List<MailItem> MailItems { get; set; }
    }

    public class SendFullMailParam
    {
        public int ZoneId { get; set; }

        public string Name { get; set; }
        public string Content { get; set; }
        public List<MailItem> MailItems { get; set; }
    }

    public class AddTitleParam
    {
        public int ZoneId { get; set; }
        public string UserId { get; set; }
        public string GameUserId { get; set; }

        public int TitleId { get; set; }
        public int Type { get; set; }
        public string EndTime { get; set; }
    }

    public class DelTitleParam
    {
        public int ZoneId { get; set; }
        public string UserId { get; set; }
        public string GameUserId { get; set; }

        public string DBId { get; set; }
        public int TitleId { get; set; }
    }

}
