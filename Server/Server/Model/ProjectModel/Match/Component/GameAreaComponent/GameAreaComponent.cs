using System.Collections.Generic;
using System.Diagnostics;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class GameAreaComponent : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 游戏服信息
        /// </summary>
        public List<C_GameAreaInfo> ServerQueryResult = new List<C_GameAreaInfo>();
        public override void Dispose()
        {
            if (IsDisposeable) return;

            ClearData();
            base.Dispose();
        }


        public void ClearData()
        {
            if (ServerQueryResult != null && ServerQueryResult.Count > 0)
            {
                for (int i = 0, len = ServerQueryResult.Count; i < len; i++)
                {
                    ServerQueryResult[i].Dispose();
                }
                ServerQueryResult.Clear();
            }
        }
    }
    public class C_GameAreaInfo : ADataContext<int>
    {
        public int GameAreaId { get; set; }
        public int RealLine { get; set; }
        public string NickName { get; set; }
        public long CreateTime { get; set; }
        /// <summary>
        /// 总人数
        /// </summary>
        public int PlayerCount { get; set; }
        /// <summary>
        /// 各个游戏服人数
        /// </summary>
        public Dictionary<int, int> GameServerInfo { get; set; }
        public int State { get; set; } = 0;

        public override void ContextAwake(int b_AreaId)
        {
            this.GameAreaId = (int)((uint)b_AreaId >> 16);
            this.RealLine = (int)((uint)b_AreaId & 0xffff);
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            GameAreaId = 0;
            NickName = default;
            CreateTime = 0;
            GameServerInfo = null;
            PlayerCount = 0;
            State = 0;
            base.Dispose();
        }
    }
}