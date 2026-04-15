using System.Collections.Generic;
using System.Linq;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETHotfix;

namespace ETModel
{
    /// <summary>
    /// 聊天管理器 适用聊天房间、世界广播
    /// 世界RoomID用GameAreaId，其他房间建议用CustomFrameWork.IdGenerater.GenerateInstanceId
    /// </summary>
    public class ChatManageComponent : TCustomComponent<MainFactory>
    {
        /// <summary>
        /// 全部的房间 Key-ChatRoomID  Value.Key-Player.GameUserId  Value.Value-Player
        /// </summary>
        Dictionary<long, Dictionary<long, Player>> AllRoomDic = new Dictionary<long, Dictionary<long, Player>>();

        public Dictionary<long, Dictionary<long, Struct_ItemAllProperty>> ShareItemDic = new Dictionary<long, Dictionary<long, Struct_ItemAllProperty>>();

        public override void Dispose()
        {
            if (this.IsDisposeable) return;

            if (AllRoomDic.Count > 0)
            {
                var mTempDiclist = AllRoomDic.Values.ToList();
                for (int i = 0, len = mTempDiclist.Count; i < len; i++)
                {
                    var mTempDic = mTempDiclist[i];

                    mTempDic.Clear();
                }
                AllRoomDic.Clear();
            }

            base.Dispose();
        }

        /// <summary>
        /// 添加一个Player到指定房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="targetPlayer"></param>
        public void AddPlayerInRoom(long roomID, Player targetPlayer)
        {
            if (AllRoomDic.TryGetValue(roomID, out var roomPlayerDict) == false)
            {
                roomPlayerDict = AllRoomDic[roomID] = new Dictionary<long, Player>();
            }

            if (roomPlayerDict.TryGetValue(targetPlayer.GameUserId, out Player mResult) == false)
            {
                roomPlayerDict.Add(targetPlayer.GameUserId, targetPlayer);
            }
        }

        /// <summary>
        /// 获取该房间下的所有Player
        /// </summary>
        /// <param name="roomID"></param>
        /// <returns></returns>
        public Dictionary<long, Player> GetAllByRoomID(long roomID)
        {
            if (AllRoomDic.TryGetValue(roomID, out var mZoneUserDic))
            {
                return mZoneUserDic;
            }
            return null;
        }

        /// <summary>
        /// Player离开房间 所有成员离开房间后删除房间
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="targetPlayer"></param>
        /// <returns></returns>
        public bool LeaveRoom(long roomID, Player targetPlayer)
        {
            var mData = GetAllByRoomID(roomID);
            if (mData != null)
            {
                if (mData.TryGetValue(targetPlayer.GameUserId, out Player mUser))
                {
                    mData.Remove(targetPlayer.GameUserId);
                    if (mData.Count <= 0)
                    {
                        AllRoomDic.Remove(roomID);  //房间内没有人员则删除房间
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Player销毁时注销所有已注册的房间
        /// </summary>
        public void PlayerDispose(Player mPlayer)
        {
            Log.Debug("================玩家退出房间");
            var mTempDiclist = AllRoomDic.Keys.ToList();
            for (int i = 0, len = mTempDiclist.Count; i < len; i++)
            {
                LeaveRoom(mTempDiclist[i], mPlayer);
            }
        }

    }
}