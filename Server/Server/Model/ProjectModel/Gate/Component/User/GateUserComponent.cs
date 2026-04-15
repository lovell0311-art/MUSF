using System;
using System.Threading.Tasks;
using ETHotfix;
using System.Collections.Generic;
using CustomFrameWork.Baseic;
using CustomFrameWork;
using System.Linq;

namespace ETModel
{
    /// <summary>
    /// 玩家管理器
    /// </summary>
    public sealed class GateUserComponent : TCustomComponent<GateUserComponent>
    {
        private readonly Dictionary<long, GateUser> KeyValuePairs = new Dictionary<long, GateUser>();
        /// <summary>
        /// 心跳最大时间
        /// </summary>
        public static int kickTime = 30 * 1000;
        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            RemoveAll();

            base.Dispose();
        }
        public void RemoveAll()
        {
            if (KeyValuePairs != null && KeyValuePairs.Count > 0)
            {
                var mTemplist = KeyValuePairs.Values.ToList();
                for (int i = 0, len = mTemplist.Count; i < len; i++)
                {
                    mTemplist[i].Dispose();
                }
                KeyValuePairs.Clear();
            }
        }


        public GateUser AddUserByUserId(long b_UserID)
        {
            if (KeyValuePairs.TryGetValue(b_UserID, out GateUser mResult) == false)
            {
                mResult = Root.CreateBuilder.GetInstance<GateUser, long>(b_UserID);

                KeyValuePairs[b_UserID] = mResult;
            }
            return mResult;
        }

        public GateUser GetUserByUserId(long b_UserID)
        {
            if (KeyValuePairs.TryGetValue(b_UserID, out GateUser mResult))
            {
                return mResult;
            }
            return null;
        }


        public void RemoveUserByUserId(long b_UserID)
        {
            if (KeyValuePairs.TryGetValue(b_UserID, out GateUser mResult))
            {
                KeyValuePairs.Remove(b_UserID);
                mResult.Dispose();
            }
        }

    }
}