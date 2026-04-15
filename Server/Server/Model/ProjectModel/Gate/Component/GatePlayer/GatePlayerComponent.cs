using System;
using System.Threading.Tasks;
using ETHotfix;
using System.Collections.Generic;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;


namespace ETModel
{
    /// <summary>
    /// 玩家管理器
    /// </summary>
    public sealed class GatePlayerComponent : TCustomComponent<MainFactory>
    {
        private readonly Dictionary<long, GatePlayer> KeyValuePairs = new Dictionary<long, GatePlayer>();
        /// <summary>
        /// 心跳最大时间
        /// </summary>
        public static int kickTime = 30 * 1000;
        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            if (KeyValuePairs != null && KeyValuePairs.Count > 0)
            {
                var mAllPlayer = KeyValuePairs.Values.ToArray();
                for (int i = 0, len = mAllPlayer.Length; i < len; i++)
                {
                    mAllPlayer[i].Dispose();
                }
                KeyValuePairs.Clear();
            }
            base.Dispose();
        }
        public GatePlayer AddPlayerByUserID(long b_UserID)
        {
            if (KeyValuePairs.TryGetValue(b_UserID, out GatePlayer mResult) == false)
            {
                mResult = Root.CreateBuilder.GetInstance<GatePlayer, long>(b_UserID);

                KeyValuePairs[b_UserID] = mResult;
            }
            return mResult;
        }
        public GatePlayer Get(long b_UserId)
        {
            if (KeyValuePairs.TryGetValue(b_UserId, out GatePlayer mResult))
            {
                return mResult;
            }
            return null;
        }
        public GatePlayer Remove(long b_UserId)
        {
            if (KeyValuePairs.TryGetValue(b_UserId, out GatePlayer mResult))
            {
                KeyValuePairs.Remove(b_UserId);
                return mResult;
            }
            return null;
        }
    }
}