using Google.Protobuf.Collections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETHotfix
{
    /// <summary>
    /// 角色存档信息
    /// </summary>
    public class RoleArchiveInfo 
    {
        /// <summary>
        /// 角色的唯一UUID
        /// </summary>
        public long UUID;
        /// <summary>
        /// 角色名字
        /// </summary>
        public string Name;
        /// <summary>
        /// 角色类型
        /// </summary>
        public int RoleType;
        /// <summary>
        /// 角色等级
        /// </summary>
        public int Level;
        /// <summary>
        /// 转职等级
        /// </summary>
        public int ClassLev;
        /// <summary>
        /// 穿戴的装备集合
        /// </summary>
       public  List<G2C_LoginSystemEquipItemMessage> struct_ItemIns=new List<G2C_LoginSystemEquipItemMessage>();
    }
    /// <summary>
    /// 预览角色管理
    /// </summary>
    public class RoleArchiveInfoManager:SimpleSingleton<RoleArchiveInfoManager>
    {
        /// <summary>可创建角色ID</summary>
        public List<long> CanCreatRoleList=new List<long>();
        /// <summary>可创建角色的数量</summary>
        public  int MaxRoleArchive = 5;
        /// <summary>
        /// 魔剑士创建的最低等级
        /// </summary>
        public  int CreatMagicswordsman = 220;
        /// <summary>
        /// 圣导师创建的最低等级
        /// </summary>
        public  int CreatHolymentor = 250;
        /// <summary>
        ///角色存档字典 用于选择和删除角色
        /// </summary>
        public  Dictionary<long, RoleArchiveInfo> roleArchiveInfosDic = new Dictionary<long, RoleArchiveInfo>();
        private readonly List<long> orderedRoleUUIDs = new List<long>();
        /// <summary>
        /// 当前选择的角色的UUID
        /// </summary>
        public long curSelectRoleUUID = 0;
        /// <summary>
        /// 本地玩家的UUID
        /// </summary>
        public long LoadRoleUUID => curSelectRoleUUID;
        /// <summary>
        /// 当前选择的角色信息(第一次进入场景时 创建角色使用)
        /// </summary>
        public RoleArchiveInfo curSelectRoleArchiveInfo => GetRoleArchiveInfo(curSelectRoleUUID);
       
        /// <summary>
        /// 获取RoleArchiveInfo
        /// </summary>
        /// <param name="Index"></param>
        /// <returns></returns>
        public RoleArchiveInfo GetRoleArchiveInfo(long Index)
        {
            RoleArchiveInfo roleArchiveInfo = null ;
            if (roleArchiveInfosDic.ContainsKey(Index))
            {
                roleArchiveInfo= roleArchiveInfosDic[Index];
            }
            return roleArchiveInfo;
        
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="roleUUID"></param>
        /// <param name="roleArchiveInfo"></param>
        public void Add(long roleUUID,RoleArchiveInfo roleArchiveInfo)
        {
            roleArchiveInfosDic[roleUUID] = roleArchiveInfo;
            orderedRoleUUIDs.Remove(roleUUID);
            orderedRoleUUIDs.Add(roleUUID);
        }

        public void ResetRoleArchives(bool clearCanCreatRoleList = false)
        {
            roleArchiveInfosDic.Clear();
            orderedRoleUUIDs.Clear();
            curSelectRoleUUID = 0;
            if (clearCanCreatRoleList)
            {
                CanCreatRoleList.Clear();
            }
        }
        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="Index"></param>
        public void Remove(long roleUUID)
        {
            if (roleArchiveInfosDic.ContainsKey(roleUUID))
            {
                roleArchiveInfosDic.Remove(roleUUID);
                orderedRoleUUIDs.Remove(roleUUID);
             }
        }
        /// <summary>
        /// roleArchiveInfosDic.Count
        /// </summary>
        /// <returns></returns>
        public int Count() 
        {
            return roleArchiveInfosDic.Count;
        }

        public List<RoleArchiveInfo> GetRoleArchivesInDisplayOrder()
        {
            List<RoleArchiveInfo> orderedArchives = new List<RoleArchiveInfo>();
            foreach (long roleUUID in orderedRoleUUIDs)
            {
                if (roleArchiveInfosDic.TryGetValue(roleUUID, out RoleArchiveInfo roleArchiveInfo) && roleArchiveInfo != null)
                {
                    orderedArchives.Add(roleArchiveInfo);
                }
            }

            if (orderedArchives.Count == roleArchiveInfosDic.Count)
            {
                return orderedArchives;
            }

            foreach (KeyValuePair<long, RoleArchiveInfo> pair in roleArchiveInfosDic)
            {
                if (pair.Value != null && !orderedArchives.Contains(pair.Value))
                {
                    orderedArchives.Add(pair.Value);
                }
            }

            return orderedArchives;
        }
        public override void Disponse()
        {
            Release();
            roleArchiveInfosDic.Clear();
            orderedRoleUUIDs.Clear();
            base.Disponse();
            CanCreatRoleList.Clear();
        }
    }
}
