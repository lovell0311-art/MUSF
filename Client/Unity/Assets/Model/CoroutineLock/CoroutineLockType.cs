namespace ETModel
{
    public static class CoroutineLockType
    {
        public const int None = 0;
        public const int RoleMove = 1;                  // 玩家移动
        public const int ActorLocationSender = 2;       // ActorLocationSender中队列消息 
        //public const int Mailbox = 3;                   // Mailbox中队列
        public const int ActorId = 3;                   // Actor 消息队列(临时用此方法保证消息处理的顺序)
        public const int UnitId = 4;                    // Map服务器上线下线时使用
        public const int DB = 5;
        public const int Resources = 6;
        public const int ResourcesLoader = 7;
        public const int CreateRole = 9;


        public const int LoginGame = 20;
        public const int LoginGate = 21;
        public const int LoginAccount = 22;
        public const int LoginCenterLock = 23;


        public const int GameUserLock = 30;


        public const int RobotHpUseItem = 50;
        public const int RobotMpUseItem = 51;
        public const int RobotLogin = 52;

        public const int PickItem = 53;//拾取物品

        public const int Max = 100; // 这个必须最大


    }
}