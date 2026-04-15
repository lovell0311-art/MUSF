using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    public static partial class TimerType
    {
        // 框架层0-1000，逻辑层的timer type 1000-9999
        public const int JewelryDurabilityDown = 1000;  // 首饰耐久下降
        public const int AncientBattlefieldCheck = 1001;  // 定时检查古战场的进入条件
        public const int CastleMasterCheck = 1002;  // 定时检查冰封城主是否过期
        public const int BattleComponentUpdateFrame = 1003; // 战斗组件定时更新
        public const int ItemValidTime = 1004; // 物品有效时间
        public const int ItemTimeLimit = 1005;//物品使用定时删除藏宝图
        public const int CombatSourceUpdateFrame = 1006;    // CombatSource 更新定时器
        public const int CombatSourceRebirth = 1007;    // CombatSource 复活定时器
        public const int PetsTrialTime = 1008;      // 宠物使用时间定时器
        public const int CheckDropItemTime = 1009;//角色掉落限制更新
        public const int CheckWing = 1010;      // 翅膀检查
        public const int CheckOnlineTime = 1011; //在线累计检查
        public const int TimingDispose = 1100;  // 定时销毁指定组件
        public const int HackerCheck = 1101;   // 反挂检测
        public const int NetworkSendLog = 1102;   // 网络发送日志
        public const int HyPayUpdate = 1103;   // 好易充值更新
        
        public const int PlayerInTeamPropertyChangeNotice = 2000; // 队友属性变动通知

        public const int GateSessionKeyNotice = 3000;   // gate session key 定时通知
        public const int CheckPrivilegedLine = 3001;//特权线路状态
        public const int TrialTowerTime = 3002;
        public const int AITimer = 5000;   // AI
        public const int OnlineStatistics = 5001;   // 在线统计
        public const int AutoAreaTimers = 5002;   // 在线开区
        public const int CommissuralAreaSignal = 5003;   // 合区信号
        // 不能超过10000
        //CopyTimerType 使用了6000-6010



        // Buff
        public const int Buff_WuDiForEnterMap = 8000; // 无敌buff

    }
}
