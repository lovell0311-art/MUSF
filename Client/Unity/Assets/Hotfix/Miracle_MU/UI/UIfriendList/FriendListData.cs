using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETHotfix
{
    public enum E_FriendsTogNewType
    {

        Black = 1,//黑名单
        Enemy,//仇人
        Friend,//好友
        Null
    }
    public enum E_AddFriendsTogNewType
    {
        Recommend,//推荐列表
        Apply,//申请列表
        Null
    }
    public enum ChatType
    {
        MyChatMessage,//推荐列表
        OtherSideMessage,//申请列表
        Null
    }
    /// <summary>
    /// 好友列表数据
    /// </summary>
    public static class FriendListData
    {
        public static List<FriendInfo> FriendList = new List<FriendInfo>();
        public static List<FriendInfo> BlackList = new List<FriendInfo>();
        public static List<FriendInfo> EnemyList = new List<FriendInfo>();
        public static List<FriendInfo> ApplyList = new List<FriendInfo>();
        public static List<FriendInfo> RecommendList = new List<FriendInfo>();
        /// <summary>
        /// 好友聊天缓存
        /// </summary>
        public static Dictionary<long, List<FriendChatNewInfo>> friendChatNewInfos = new Dictionary<long, List<FriendChatNewInfo>>();

        /// <summary>
        /// 根据E_FriendsTogType获取对应的列表信息
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static List<FriendInfo> GetCurList(this E_FriendsTogNewType self) => self switch
        {
            E_FriendsTogNewType.Friend => FriendList,
            E_FriendsTogNewType.Black => BlackList,
            E_FriendsTogNewType.Enemy => EnemyList,
            _ => null
        };
        public static List<FriendInfo> GetAddFriendCurList(this E_AddFriendsTogNewType self) => self switch
        {
            E_AddFriendsTogNewType.Apply => ApplyList,
            E_AddFriendsTogNewType.Recommend => RecommendList,
            _ => null
        };
        public static List<FriendInfo> GetNowList(this E_FriendsTogNewType self)
        {
            switch (self)
            {
                case E_FriendsTogNewType.Black:
                    return BlackList;
                case E_FriendsTogNewType.Enemy:
                    return EnemyList;
                case E_FriendsTogNewType.Friend:
                    return FriendList;
                default:
                    break;
            }
            return null;
        }

        public static List<FriendInfo> GetAddFriendNowList(this E_AddFriendsTogNewType self)
        {
            switch (self)
            {
                case E_AddFriendsTogNewType.Recommend:
                    return RecommendList;
                case E_AddFriendsTogNewType.Apply:
                    return ApplyList;
                default:
                    break;

            }
            return null;
        }
        public static E_AddFriendsTogNewType GetE_AddFriendsTogNewType(int index)
        {
            switch (index)
            {
                case 0:
                    return E_AddFriendsTogNewType.Recommend;
                case 1:
                    return E_AddFriendsTogNewType.Apply;
                default:
                    return E_AddFriendsTogNewType.Null;
            }
        }
        public static E_FriendsTogNewType GetE_FriendsTogNewType(int index)
        {
            switch (index)
            {
                case 0:
                    return E_FriendsTogNewType.Friend;
                case 1:
                    return E_FriendsTogNewType.Enemy;
                case 2:
                    return E_FriendsTogNewType.Black;
                default:
                    return E_FriendsTogNewType.Null;
            }
        }

        public static void FriendClear()
        {
            FriendList.Clear();
            BlackList.Clear();
            EnemyList.Clear();
        }
        public static void AddFriendClear()
        {
            ApplyList.Clear();
            RecommendList.Clear();
        }
        /// <summary>
        /// 清理 数据
        /// </summary>
        public static void Clear() 
        {
            FriendList.Clear();
            BlackList.Clear();
            EnemyList.Clear();
            ApplyList.Clear();
            RecommendList.Clear();
        }
    }
}
