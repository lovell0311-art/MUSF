using System.Collections.Generic;
using System;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using TencentCloud.Bri.V20190328.Models;
using TencentCloud.Ecm.V20190719.Models;
using TencentCloud.Kms.V20190118.Models;
using TencentCloud.Mps.V20190612.Models;

namespace ETHotfix
{
    /// <summary>
    /// 好友组件
    /// </summary>
    public static partial class FriendComponentSystem
    {
        /// <summary>
        /// 初始化好友系统
        /// </summary>
        /// <param name="b_Component"></param>
        /// <returns></returns>
         public async static Task<bool> Init(this FriendComponent b_Component,int mAreaId)
        {
            b_Component.InitFriendList();
            b_Component.RecommendList.Clear();

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            DataCacheManageComponent mDataCacheComponent = b_Component.Parent.AddCustomComponent<DataCacheManageComponent>();
            var FrienddataCache = mDataCacheComponent.Get<DBFriendData>();
            if (FrienddataCache == null)
            {
                var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.Parent.GameAreaId);
                FrienddataCache = await mDataCacheComponent.Add<DBFriendData>(dBProxy2, p => p.GameUserId == b_Component.Parent.GameUserId);
            }
            var FriendList = FrienddataCache.DataQuery(p => p.GameUserId == b_Component.Parent.GameUserId && p.IsDisabled != 1);

            if (FriendList != null && FriendList.Count > 0)
            {
                foreach (DBFriendData dateFriend in FriendList)
                {
                    Friend friendinfo = new Friend();
                    friendinfo.GameUserId = dateFriend.FriendUserId;
                    friendinfo.ListType = dateFriend.ListType;
                    friendinfo.CharName = dateFriend.CharName;
                    friendinfo.TimeDate = dateFriend.TimeDate;
                    friendinfo.IsDisabled = dateFriend.IsDisabled;
                    friendinfo.AreaId = dateFriend.AreaId;

                    Player mBePlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(friendinfo.AreaId, dateFriend.FriendUserId);
                    if (mBePlayer != null)
                    {
                        friendinfo.iState = 0;
                        friendinfo.iLv = mBePlayer.GetCustomComponent<GamePlayer>().Data.Level;
                        var WaInfo = mBePlayer.GetCustomComponent<PlayerWarAllianceComponent>();
                        if (WaInfo != null)
                        {
                            friendinfo.WarAlliancePost = WaInfo.MemberPost;
                            friendinfo.WarAllianceName = WaInfo.WarAllianceName;
                        }
                        else
                        {
                            friendinfo.WarAlliancePost = 0;
                            friendinfo.WarAllianceName = "";
                        }
                    }
                    else
                    {
                        friendinfo.iState = 1;
                        friendinfo.iLv = 0;
                        friendinfo.WarAlliancePost = 0;
                        friendinfo.WarAllianceName = "";
                    }

                    b_Component.SetFriendType(friendinfo);
                }
                return true;
            }

            return true;
        }
        /// <summary>
        /// 改名更新好友名字
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="Name"></param>
        /// <returns></returns>
        public static async Task UpFriendName(this FriendComponent b_Component,string Name)
        {
            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy2 = mDBProxyManager.GetZoneDB(DBType.Core, (int)b_Component.Parent.GameAreaId);

            if (dBProxy2 != null) 
            {
                var List = await dBProxy2.Query<DBFriendData>(p => p.FriendUserId == b_Component.Parent.GameUserId && p.IsDisabled != 1);
                if (List != null && List.Count >= 1)
                {
                    foreach (var f in List) 
                    { 
                        (f as DBFriendData).CharName = Name;
                        await dBProxy2.Save(f as DBFriendData);
                    }
                }
            }
        }
        /// <summary>
        /// 根据类型获取不同列表
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<long, Friend> GetFriendTypeList(this FriendComponent b_Component, int type)
        {
            if (b_Component.FriendLiset.TryGetValue(type, out var keyValuePairs1))
            {
                return keyValuePairs1;
            }
            else 
            {
                Dictionary<long, Friend> keyValuePairs = new Dictionary<long, Friend>();
                b_Component.FriendLiset.Add(type, keyValuePairs);
                return keyValuePairs;
            }
        }
        /// <summary>
        /// 初始化玩家好友列表
        /// </summary>
        /// <param name="b_Component"></param>
        private static void InitFriendList(this FriendComponent b_Component)
        {
            b_Component.FriendLiset.Clear();

            for (int i = 1; i <= (int)FirendType.FIRENDSLIST; i++)
            {
                var FriendList = b_Component.GetFriendTypeList(i);
                if (FriendList == null)
                {
                    b_Component.FriendLiset[i] = new Dictionary<long, Friend>();
                }
            }
        }
        ///<summary>
        ///检查好友是否存在
        /// </summary>
        /// <param name="CharName"></param>
        public static bool CheckFriendList(this FriendComponent b_Component, long GameUserID)
        {
            Dictionary<long, Friend> keyValuePairs;// = new Dictionary<long, Friend>()

            for (int i = 0, len = b_Component.FriendLiset.Count; i <= len; i++)
            {
                if (b_Component.FriendLiset.TryGetValue(i, out keyValuePairs))
                {
                    if (keyValuePairs == null) continue;
                    foreach (var FriendInfo in keyValuePairs)
                    {
                        if (GameUserID == FriendInfo.Value.GameUserId) return true;
                    }
                }
            }
            return false;
        }
        /// <summary>
        /// 添加好友到申请列表
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="mPlayer"></param>
        /// <returns></returns>
        public static bool AddFriends(this FriendComponent b_Component, Player mPlayer,int mAreaId)
        {
            Dictionary<long, Friend> keyValuePairs; //= new Dictionary<long, Friend>();
            Friend friend = new Friend();

            keyValuePairs = b_Component.GetFriendTypeList((int)FirendType.APPLICATION);
            if (keyValuePairs == null)
            {
                b_Component.FriendLiset[(int)FirendType.APPLICATION] = keyValuePairs = new Dictionary<long, Friend>() { { friend.GameUserId, friend } };
                return true;
            }
            if (keyValuePairs.TryGetValue(mPlayer.GameUserId, out friend))
            { 
                if(friend.CharName == mPlayer.GetCustomComponent<GamePlayer>().Data.NickName)
                    return false;
            }

            if (friend == null)
                friend = new Friend();

            friend.GameUserId = mPlayer.GameUserId;
            friend.CharName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
            friend.ListType = (int)FirendType.APPLICATION;
            friend.AreaId = mAreaId;
            friend.iState = 0;    
            friend.IsDisabled = 0;
            friend.iLv = mPlayer.GetCustomComponent<GamePlayer>().Data.Level;
            friend.WarAlliancePost = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>().MemberPost;
            friend.WarAllianceName = mPlayer.GetCustomComponent<PlayerWarAllianceComponent>().WarAllianceName;
            if (keyValuePairs.Count >= 100) return false;
            keyValuePairs.Add(mPlayer.GameUserId, friend);
            return true;
        }

        ///<summary>
        ///同意添加修改好友类型
        ///</summary>
        public static bool SetFriendType(this FriendComponent b_component,Friend mfriend)
        {
            if (mfriend.ListType > 4 || mfriend.ListType < 1) return false;

            var FriendList = b_component.GetFriendTypeList(mfriend.ListType);
            if (FriendList == null)
            {
                b_component.FriendLiset[mfriend.ListType] = new Dictionary<long, Friend>() { { mfriend.GameUserId, mfriend } };
                return true;
            }
            if(FriendList.Count >= 100) return false;

            if(FriendList.ContainsKey(mfriend.GameUserId) != false) return  true;

            FriendList.Add(mfriend.GameUserId, mfriend);
            return true ;
        }

        ///<summary>
        ///好友写入数据库
        /// </summary>
        public static async Task<bool> SetFriendDB(this FriendComponent b_Component, Friend mfriend)
        {
            DataCacheManageComponent mDataCacheManageComponent = b_Component.Parent.GetCustomComponent<DataCacheManageComponent>();
            var dataCache = mDataCacheManageComponent.Get<DBFriendData>();
            var backPackDatas = dataCache.DataQuery(p => p.GameUserId == b_Component.Parent.GameUserId && p.FriendUserId == mfriend.GameUserId && p.IsDisabled != 1);
            if (backPackDatas == null) return false;

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, b_Component.Parent.GameAreaId);
            if (dBProxy == null) return false;

            DBFriendData dBFriendData = new DBFriendData()
            {
                Id = IdGeneraterNew.Instance.GenerateUnitId(b_Component.Parent.GameAreaId),
                FriendUserId = mfriend.GameUserId,
                CharName = mfriend.CharName,
                ListType = mfriend.ListType,
                GameUserId = b_Component.Parent.GameUserId,
                AreaId = mfriend.AreaId,
                IsDisabled = mfriend.IsDisabled,
            };

            if (backPackDatas.Count == 0)
            {
                dataCache.DataAdd(dBFriendData);
                await dBProxy.Save(dBFriendData);
            }
            else
            {
                var mWriteDataComponent = Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Get(b_Component.Parent.GameAreaId);
                dBFriendData.Id = backPackDatas[0].Id;
                mWriteDataComponent.Save(dBFriendData, dBProxy).Coroutine();
            }
            return true;
        }

        ///<summary>
        ///删除好友
        /// </summary>
        public static int DeleteFriend(this FriendComponent b_Component, int Type,long GameUserID)
        {
            if (b_Component == null) return 0;
            if(Type < 0 || Type >4) return 0;
             
            if (b_Component.FriendLiset[Type].TryGetValue(GameUserID, out Friend friend))
            {
                friend.ListType = 0;
                friend.IsDisabled = 1;
                b_Component.SetFriendDB(friend).Coroutine();
                int AreaId = friend.AreaId;
                b_Component.FriendLiset[Type].Remove(GameUserID);
                return AreaId;
            }
            return 0;
        }
        ///<summary>拉黑好友</summary>
        public static bool BlockFriend(this FriendComponent b_Component, int Type, long GameUserID)
        {

            if (b_Component == null) return false;
            if (Type < 0 || Type > 4) return false;
            
            Friend friend;
            if (b_Component.FriendLiset[Type].TryGetValue(GameUserID, out Friend Dlefriend))
            {
                friend = Dlefriend;
                friend.TimeDate = Help_TimeHelper.GetNowSecond();
                friend.ListType = (int)FirendType.BLACKLIST;
                b_Component.FriendLiset[Type].Remove(GameUserID);
                b_Component.FriendLiset[(int)FirendType.BLACKLIST].Add(friend.GameUserId, friend);
                b_Component.SetFriendDB(friend).Coroutine();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 拉黑推荐好友
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="GameUserID"></param>
        /// <returns></returns>
        public static bool BlockRecommendFriend(this FriendComponent b_Component, Player mPlayer)
        {
           if(b_Component == null) return false;   
           if(mPlayer == null) return false;

            Friend friend = new Friend();
            friend.GameUserId = mPlayer.GameUserId;
            friend.CharName = mPlayer.GetCustomComponent<GamePlayer>().Data.NickName;
            friend.TimeDate = Help_TimeHelper.GetNowSecond();
            friend.ListType = (int)FirendType.BLACKLIST; 

            b_Component.FriendLiset[1].Add(friend.GameUserId, friend);
            b_Component.SetFriendDB(friend).Coroutine();
            return true;
        }
        /// <summary>
        /// 添加推荐好友ID进列表
        /// </summary>
        /// <param name="b_Component"></param>
        /// <param name="GameUserID"></param>
        /// <returns></returns>
        public static bool AddRecommendList(this FriendComponent b_Component, long GameUserID)
        {
            long UID = 0;
            if (b_Component.RecommendList.TryGetValue(GameUserID, out UID))
            {
                b_Component.RecommendList.Remove(GameUserID);
                return false;
            }
            b_Component.RecommendList.Add(GameUserID, GameUserID);
            return true;
        }

        public static int GetRecommendCount(this FriendComponent b_Component)
        {
            return b_Component.RecommendList.Count;
        }

        public static async Task AddFOEList(this FriendComponent b_Component, GamePlayer AddTo, GamePlayer BeAddTo)
        {
            if (BeAddTo == null || b_Component == null) return;

            Dictionary<long, Friend> list = new Dictionary<long, Friend>();

            Friend friend = new Friend();
            friend.GameUserId = BeAddTo.Player.GameUserId;
            friend.CharName = BeAddTo.Data.NickName;
            friend.iLv = BeAddTo.Data.Level;
            friend.ListType = (int)FirendType.FOELIST;
            friend.IsDisabled = 0;
            friend.iState = 1;
            friend.TimeDate = Help_TimeHelper.GetNowSecond();
            friend.WarAlliancePost = BeAddTo.Player.GetCustomComponent<PlayerWarAllianceComponent>().MemberPost;
            friend.WarAllianceName = BeAddTo.Player.GetCustomComponent<PlayerWarAllianceComponent>().WarAllianceName;
            if (b_Component.FriendLiset.TryGetValue((int)FirendType.FOELIST, out list))
            {
                if (list.ContainsKey(friend.GameUserId) == true) return;
                list.Add(friend.GameUserId, friend);
            }
            else
            {
                if (!b_Component.SetFriendType(friend))
                    Log.Error($"仇人列表以满 添加失败 ID：{friend.GameUserId} Name：{friend.CharName}");

            }

            DBProxyManagerComponent mDBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();
            var dBProxy = mDBProxyManager.GetZoneDB(DBType.Core, AddTo.Player.GameAreaId);

            DBFriendData dBFriendData = new DBFriendData()
            {
                Id = IdGeneraterNew.Instance.GenerateUnitId(AddTo.Player.GameAreaId),
                GameUserId = AddTo.Player.GameUserId,
                FriendUserId = BeAddTo.Player.GameUserId,
                CharName = BeAddTo.Data.NickName,
                ListType = (int)FirendType.FOELIST,
                IsDisabled = 0,
                TimeDate = Help_TimeHelper.GetNowSecond(),
            };

            await dBProxy.Save(dBFriendData);
        }

    }
}
