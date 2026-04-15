namespace ETModel
{


    public class SessionGateUserComponent : Component
    {
        public long UserId { get; set; }
        public long GameUserId { get; set; }
        public int TransferServerId { get; set; }


        public int CheckServerId { get; set; }
        public int GateServerId { get; set; }
        public int GameServerId { get; set; }


        /// <summary>
        /// 区服Id
        /// </summary>
        public int GameAreaId { get; set; }
        /// <summary>
        /// 区服线路Id
        /// </summary>
        public int GameAreaLineId { get; set; }


        public override void Dispose()
        {
            if (IsDisposed) return;

            base.Dispose();
            GameUserId = 0;
            UserId = 0;
            GameAreaId = 0;
            GameAreaLineId = 0;
        }
    }
}