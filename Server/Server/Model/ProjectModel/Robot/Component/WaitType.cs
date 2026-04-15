using ETModel;

namespace ETModel.Robot
{
    namespace WaitType
    {
        public struct Wait_MapChangeFinish : IWaitType
        {
            public int Error
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 攻击开始通知
        /// </summary>
        public struct Wait_AttackStartNotice : IWaitType
        {
            public int Error
            {
                get;
                set;
            }

            public ETHotfix.G2C_AttackStart_notice message;
        }

        public struct Wait_ReadyResponse : IWaitType
        {
            public int Error
            {
                get;
                set;
            }

            public ETHotfix.G2C_ReadyResponse message;
        }

        /// <summary>
        /// 物品进入背包
        /// </summary>
        public struct Wait_Unit_ItemIntoBackpack : IWaitType
        {
            public int Error
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 物品离开背包
        /// </summary>
        public struct Wait_Unit_ItemLeaveBackpack : IWaitType
        {
            public int Error
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 自己离开地图
        /// </summary>
        public struct Wait_Unit_LeaveMap : IWaitType
        {
            public int Error
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 自己穿戴装备
        /// </summary>
        public struct Wait_Unit_EquipItem : IWaitType
        {
            public int Error
            {
                get;
                set;
            }
        }

        /// <summary>
        /// 自己脱下装备
        /// </summary>
        public struct Wait_Unit_UnloadEquipItem : IWaitType
        {
            public int Error
            {
                get;
                set;
            }
        }
    }
}
