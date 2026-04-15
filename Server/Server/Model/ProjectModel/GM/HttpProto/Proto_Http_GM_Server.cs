namespace ETModel.HttpProto
{

    public class GameStatusParam
    {
        public int ServerId { get; set; }
    }

    public class RunCodeParam
    {
        public int ServerId { get; set; }
        public string Code { get; set; }
    }

    //public class Paystring
    //{
    //    public string Order_id { get; set; }
    //    public string App_order_id { get; set; }
    //    public string Sid { get; set; }
    //    public uid=10001&gid=12&rid=321&coins=500&product_id=111&app_user_name=zhanz&time=1302674580&platform_id=xy&money =5&extra1=1111&extra2=333&sign=e2b48012bdc1b29ae4025eacdbd94075
    //}
}
