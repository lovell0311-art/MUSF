using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel.Robot
{

    public class RoleInfo
    {
        public long GameUserId;//角色id
        public string Name;//角色昵称
        public E_GameOccupation PlayerType;//角色类型
        public int Level;//角色等级
        public int OccupationLevel;// 转职次数
    }

    [ObjectSystem]
    public class RoleInfoComponentDestroySystem : DestroySystem<RoleInfoComponent>
    {
        public override void Destroy(RoleInfoComponent self)
        {
            self.RoleInfoDict.Clear();
        }
    }



    public class RoleInfoComponent : Entity
    {
        /// <summary>
        /// key:GameUserId
        /// </summary>
        public Dictionary<long, RoleInfo> RoleInfoDict = new Dictionary<long, RoleInfo>();
    }
}
