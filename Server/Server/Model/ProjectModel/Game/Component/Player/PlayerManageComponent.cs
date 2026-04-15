using System.Collections.Generic;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class PlayerManageComponent : TCustomComponent<MainFactory>
    {

        /// <summary>
        /// 全部的user
        /// </summary>
        public Dictionary<int, Dictionary<long, Player>> AllUserDic = new Dictionary<int, Dictionary<long, Player>>();

        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            if (AllUserDic.Count > 0)
            {
                var mTempDiclist = AllUserDic.Values.ToList();
                for (int i = 0, len = mTempDiclist.Count; i < len; i++)
                {
                    var mTempDic = mTempDiclist[i];

                    var mTemp = mTempDic.Values.ToArray();
                    for (int j = 0, jlen = mTemp.Length; j < jlen; j++)
                    {
                        mTemp[j].Dispose();
                    }
                }
                AllUserDic.Clear();
            }

            base.Dispose();
        }


        public Player AddPlayerByUserID(int b_Zone, long b_UserID)
        {
            if (AllUserDic.TryGetValue(b_Zone, out var mZonePlayers) == false)
            {
                mZonePlayers = AllUserDic[b_Zone] = new Dictionary<long, Player>();
            }

            if (mZonePlayers.TryGetValue(b_UserID, out Player mResult) == false)
            {
                mResult = Root.CreateBuilder.GetInstance<Player>();
                mResult.GameUserId = b_UserID;
                mResult.GameAreaId = b_Zone;

                mZonePlayers[b_UserID] = mResult;
            }
            return mResult;
        }
        public Dictionary<int, Dictionary<long, Player>> GetAllPlayers()
        {
            return AllUserDic;
        }
        public Dictionary<long, Player> GetAllByZone(int b_Zone)
        {
            if (AllUserDic.TryGetValue(b_Zone, out var mZoneUserDic))
            {
                return mZoneUserDic;
            }
            return null;
        }
        public Player NameGet(int b_Zone, string CharName)
        {
            var mData = GetAllByZone(b_Zone);

            if (CharName == null) return null;

            if (mData != null)
            {

                foreach (var keyValuePairs in mData)
                {
                    var mGamePlayer = keyValuePairs.Value.GetCustomComponent<GamePlayer>();
                    if (mGamePlayer.Data.NickName == CharName)
                    {
                        return keyValuePairs.Value;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获取Player，只能取到状态为 Online 的 Player
        /// <para>获取任意状态的 Player，可以使用 UnsafeGet </para>
        /// </summary>
        public Player Get(int b_Zone, long b_UserId)
        {
            var mData = GetAllByZone(b_Zone);
            if (mData != null)
            {
                if (mData.TryGetValue(b_UserId, out Player mUser))
                {
                    return (mUser.OnlineStatus == EOnlineStatus.Online) ? mUser : null;
                }
            }
            return null;
        }
        /// <summary>
        /// 获取Player，不安全的
        /// </summary>
        public Player UnsafeGet(int b_Zone, long b_UserId)
        {
            var mData = GetAllByZone(b_Zone);
            if (mData != null)
            {
                if (mData.TryGetValue(b_UserId, out Player mUser))
                {
                    return mUser;
                }
            }
            return null;
        }

        public Player Remove(int b_Zone, long b_UserId)
        {
            var mData = GetAllByZone(b_Zone);
            if (mData != null)
            {
                if (mData.TryGetValue(b_UserId, out Player mUser))
                {
                    mData.Remove(b_UserId);
                    return mUser;
                }
            }
            return null;
        }

    }
}