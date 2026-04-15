using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    namespace EventType
    {
        /// <summary>
        /// 敌人死亡
        /// </summary>
        public class EnemyDeath
        {
            public static readonly EnemyDeath Instance = new EnemyDeath();
            /// <summary>
            /// 死亡的敌人
            /// </summary>
            public Enemy enemy;
            /// <summary>
            /// 凶手
            /// </summary>
            public CombatSource attacker;
            /// <summary>
            /// 所在地图
            /// </summary>
            public MapComponent map;
        }

        /// <summary>
        /// 玩家角色死亡
        /// </summary>
        public class GamePlayerDeath
        {
            public static readonly GamePlayerDeath Instance = new GamePlayerDeath();
            /// <summary>
            /// 死亡的玩家角色
            /// </summary>
            public GamePlayer gamePlayer;
            /// <summary>
            /// 凶手
            /// </summary>
            public CombatSource attacker;
            /// <summary>
            /// 所在地图
            /// </summary>
            public MapComponent map;
        }

        /// <summary>
        /// 角色升级
        /// </summary>
        public class GamePlayerLevelUp
        {
            public static readonly GamePlayerLevelUp Instance = new GamePlayerLevelUp();
            /// <summary>
            /// 升级的玩家角色
            /// </summary>
            public GamePlayer gamePlayer;
            /// <summary>
            /// 升级前的等级
            /// </summary>
            public int oldLevel;
            /// <summary>
            /// 当前等级
            /// </summary>
            public int newLevel;
        }

        /// <summary>
        /// 角色进入转移点
        /// </summary>
        public class GamePlayerEnterTransferPoint
        {
            public static readonly GamePlayerEnterTransferPoint Instance = new GamePlayerEnterTransferPoint();
            /// <summary>
            /// 触发玩家角色
            /// </summary>
            public GamePlayer triggerGamePlayer;
            /// <summary>
            /// 进入的转移点
            /// </summary>
            public int transferPoint;

        }

        /// <summary>
        /// 角色转职成功
        /// </summary>
        public class GamePlayerCareerChangeComplete
        {
            public static readonly GamePlayerCareerChangeComplete Instance = new GamePlayerCareerChangeComplete();
            /// <summary>
            /// 转职的玩家角色
            /// </summary>
            public GamePlayer gamePlayer;
            /// <summary>
            /// 之前的转职等级
            /// </summary>
            public int oldOccupationLevel;
            /// <summary>
            /// 现在的转职等级
            /// </summary>
            public int newOccupationLevel;

        }

        /// <summary>
        /// 配置属性点
        /// </summary>
        public class GamePlayerConfigureAttributePoint
        {
            public static readonly GamePlayerConfigureAttributePoint Instance = new GamePlayerConfigureAttributePoint();
            /// <summary>
            /// 分配属性点的角色
            /// </summary>
            public GamePlayer gamePlayer;
            /// <summary>
            /// 分配的属性点
            /// </summary>
            public E_GameProperty gameProperty;
            /// <summary>
            /// 添加了多少属性点
            /// </summary>
            public int addPointNumber;
        }

        /// <summary>
        /// 收到客户端消息 C2G_UpdateProgress
        /// </summary>
        public class ClientMsgC2G_UpdateTaskProgress
        { 
            public static readonly ClientMsgC2G_UpdateTaskProgress Instance = new ClientMsgC2G_UpdateTaskProgress();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 收到的消息
            /// </summary>
            public ETHotfix.C2G_UpdateTaskProgress request;
        }

        /// <summary>
        /// 使用地图传送
        /// </summary>
        public class PlayerUseMapDelivery
        {
            public static readonly PlayerUseMapDelivery Instance = new PlayerUseMapDelivery();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 传送点id
            /// </summary>
            public int transferPointId;
        }

        /// <summary>
        /// CombatSource 进入或切换地图
        /// </summary>
        public class CombatSourceEnterOrSwitchMap
        {
            public static readonly CombatSourceEnterOrSwitchMap Instance = new CombatSourceEnterOrSwitchMap();
            /// <summary>
            /// 触发单位
            /// </summary>
            public CombatSource combatSource;
            /// <summary>
            /// 传送前的地图
            /// <para>可能第一次进入地图，为null</para>
            /// </summary>
            public MapComponent oldMap;
            /// <summary>
            /// 进入的新地图
            /// </summary>
            public MapComponent newMap;

        }


        /// <summary>
        /// CombatSource 离开地图
        /// </summary>
        public class CombatSourceLeaveMap
        {
            public static readonly CombatSourceLeaveMap Instance = new CombatSourceLeaveMap();
            /// <summary>
            /// 触发单位
            /// </summary>
            public CombatSource combatSource;
            /// <summary>
            /// 离开的地图
            /// </summary>
            public MapComponent leavedMap;
        }


        public class PlayerCompleteDemonCopy
        {
            public static readonly PlayerCompleteDemonCopy Instance = new PlayerCompleteDemonCopy();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 传送点id
            /// </summary>
            public int transferPointId;
        }
        public class PlayerCompleteRedCopy
        {
            public static readonly PlayerCompleteRedCopy Instance = new PlayerCompleteRedCopy();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 传送点id
            /// </summary>
            public int transferPointId;
        }
        public class BreakThroughTheGate
        {
            public static readonly BreakThroughTheGate Instance = new BreakThroughTheGate();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 传送点id
            /// </summary>
            public int transferPointId;
        }
        public class DemonPlazaGainsPoints
        {
            public static readonly DemonPlazaGainsPoints Instance = new DemonPlazaGainsPoints();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 恶魔广场分数
            /// </summary>
            public int transferPoint;
        }
        /// <summary>
        /// 玩家上线准备完成
        /// </summary>
        public class PlayerReadyComplete
        {
            public static readonly PlayerReadyComplete Instance = new PlayerReadyComplete();
            /// <summary>
            /// 上线准备完成的玩家
            /// </summary>
            public Player player;
        }

        /// <summary>
        /// 领取狩猎任务
        /// </summary>
        public class ReceiveHuntingTask
        {
            public static readonly ReceiveHuntingTask Instance = new ReceiveHuntingTask();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 任务配置id
            /// </summary>
            public int taskConfigId;
        }

        /// <summary>
        /// 领取新手buff
        /// </summary>
        public class ReceiveXinShouBuff
        {
            public static readonly ReceiveXinShouBuff Instance = new ReceiveXinShouBuff();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
        }
        public class EquipmentRelatedSettings
        {
            public static readonly EquipmentRelatedSettings Instance = new EquipmentRelatedSettings();
            /// <summary>
            /// 触发玩家
            /// </summary>
            public Player player;
            /// <summary>
            /// 物品
            /// </summary>
            public Item item;
            /// <summary>
            /// 称号数量
            /// </summary>
            public int TitleCount;
            /// <summary>
            /// 加10数量
            /// </summary>
            public int ItemCount;
        }
    }
}
