using System.Collections.Generic;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /// <summary>
    /// 组队管理器
    /// </summary>
    public class TeamManageComponent : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 队伍最大人数配置
        /// </summary>
        public const int MaxMemberCount = 5;
        /// <summary>
        /// 全部的组队数据 Key-ChatRoomID  Value.Key-Player.GameUserId  Value.Value-Player
        /// 除了自己的System脚本，外部不要调用
        /// </summary>
        public Dictionary<long, Dictionary<long, Player>> AllTeamDic = new Dictionary<long, Dictionary<long, Player>>();

        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            if (AllTeamDic.Count > 0)
            {
                var mTempDiclist = AllTeamDic.Values.ToList();
                for (int i = 0, len = mTempDiclist.Count; i < len; i++)
                {
                    var mTempDic = mTempDiclist[i];

                    mTempDic.Clear();
                }
                AllTeamDic.Clear();
            }

            base.Dispose();
        }

        

        /// <summary>
        /// 获取该队伍下的所有Player
        /// </summary>
        /// <param name="teamID"></param>
        /// <returns></returns>
        public Dictionary<long, Player> GetAllByTeamID(long teamID)
        {
            if (AllTeamDic.TryGetValue(teamID, out var mZoneUserDic))
            {
                //检查Player是否已经被释放 避免Player重用造成的bug
                var mTempDiclist = mZoneUserDic.Keys.ToList();
                for (int i = 0; i < mTempDiclist.Count; i++)
                {
                    long key = mTempDiclist[i];
                    if (mZoneUserDic[key] == null  || mZoneUserDic[key].IsDisposeable || mZoneUserDic[key].GameUserId != key)
                    {
                        mZoneUserDic.Remove(key);
                    }
                }
                return mZoneUserDic;
            }
            return null;
        }

        /// <summary>
        /// 是否可以添加队员
        /// </summary>
        /// <param name="teamID">队伍ID</param>
        /// <returns></returns>
        public bool isCanAddMember(long teamID)
        {
            if (AllTeamDic.TryGetValue(teamID, out var mUserDic))
            {
                return mUserDic.Count < MaxMemberCount;
            }
            return false;
        }

    }
}