using CustomFrameWork.Baseic;
using CustomFrameWork;

using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    public class GameUserComponent : TCustomComponent<MainFactory>
    {

        /// <summary>
        /// 全部的user
        /// </summary>
        Dictionary<long, GameUser> KeyValuePairs = new Dictionary<long, GameUser>();

        public override void Dispose()
        {
            if (this.IsDisposeable) return;
            
            RemoveAllUser();

            base.Dispose();
        }
        /// <summary>
        /// 移除全部用户    
        /// </summary>
        public void RemoveAllUser()
        {
            if (KeyValuePairs != null)
            {
                if (KeyValuePairs.Count > 0)
                {
                    var mUsers = KeyValuePairs.Values.ToList();
                    for (int i = 0, len = mUsers.Count; i < len; i++)
                    {
                        mUsers[i].Dispose();
                    }
                    KeyValuePairs.Clear();
                }
                var mIpConnectDic = Root.MainFactory.GetCustomComponent<IpCacheComponent>().IpConnectDic;
                mIpConnectDic.Clear();
            }
        }

        public GameUser AddUserByUserId(long b_UserID)
        {
            if (KeyValuePairs.TryGetValue(b_UserID, out GameUser mResult) == false)
            {
                mResult = Root.CreateBuilder.GetInstance<GameUser>();
                mResult.UserId = b_UserID;

                KeyValuePairs[b_UserID] = mResult;
            }
            return mResult;
        }

        public GameUser GetPlayer(long b_UserId)
        {
            if (KeyValuePairs.TryGetValue(b_UserId, out var mZoneUserDic))
            {
                return mZoneUserDic;
            }
            return null;
        }
        public bool Remove(long b_UserId)
        {
            if (KeyValuePairs.TryGetValue(b_UserId, out var mUser))
            {
                KeyValuePairs.Remove(b_UserId);

                var mIpConnectDic = Root.MainFactory.GetCustomComponent<IpCacheComponent>().IpConnectDic;
                if (mIpConnectDic.TryGetValue(mUser.ConnectIp, out var connectCount))
                {
                    connectCount--;
                    if (connectCount < 0) connectCount = 0;
                    mIpConnectDic[mUser.ConnectIp] = connectCount;
                }
                //System.Console.WriteLine($"链接:{connectCount}");

                mUser.Dispose();
                return true;
            }
            return false;
        }

        public int GetUserCount()
        {
            return KeyValuePairs.Count;
        }

        public List<GameUser> GetGameUsers()
        {
            return KeyValuePairs.Values.ToList();
        }
    }
}