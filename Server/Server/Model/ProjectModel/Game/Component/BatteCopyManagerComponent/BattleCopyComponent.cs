using ETHotfix;
using System.Linq;
using System.Collections.Generic;
using TencentCloud.Cfw.V20190904.Models;
using CustomFrameWork.Baseic;
using TencentCloud.Ame.V20190916.Models;
using TencentCloud.Mgobe.V20201014.Models;

namespace ETModel
{
    public enum CopyState
    {
        Wait = 0,
        Prepare = 1,
        Open = 2,
        Close = 3,
    }
    public static partial class CopyTimerType
    {
        /// <summary>
        /// 检查副本是否结算
        /// </summary>
        public const int CheckCopyTime = 6000;
        /// <summary>
        /// 藏宝图创建怪物
        /// </summary>
        public const int CreateBobTime = 6001;
    }
    public enum CopyRoomState
    {
        None =0,//初始状态阶段
        wave1Over =1,//第一波怪阶段统计数量
        DoorBroken =2,//可以攻击大门
        wave2Over = 3,//第二波怪阶段统计数量
        CrystalBroken = 4,//可以攻击水晶棺
        CommitWeapon = 5,//提交天使大剑
        Accomplish = 6,//完成提交任务结束
    }
    public static class RMobInfo
    {
        public static int MobConfig = 596;
        public static int X = 76;
        public static int Y = 106;
    }
    public static class EMobInfo
    {
        public static int MobConfig = 628;
        public static int X = 77;
        public static int Y = 84;
    }
    public class BattleCopyComponent : CustomFrameWork.ADataContext<BatteCopyManagerComponent>
    {
        /// <summary>
        /// 副本类型
        /// </summary>
        public int Type = 0;

       /// <summary>
       /// 准备时间
       /// </summary>
        public long prepareTimer;

        /// <summary>
        /// 开启时间
        /// </summary>
        public long startTimer;

        /// <summary>
        /// 结束时间
        /// </summary>
        public long endTimer;

        /// <summary>
        /// 副本持续时间
        /// </summary>
        public int DurationnTime;

        /// <summary>
        /// 副本状态
        /// </summary>
        public CopyState copyState;

        /// <summary>
        /// 存储配置表的开发时间
        /// </summary>
        public int[] timeKeys;
        public int[] timeValues;
        public List<string> openTimeList = new List<string>();
        //public bool IsTimelimit;
        /// <summary>
        /// 副本开放条件配置
        /// </summary>
        public BattleCopy_ConditionConfig config;

        public BatteCopyManagerComponent Parent { get; private set; }

        /// <summary>
        /// 副本玩家临时存储
        /// </summary>
        public Dictionary<long, Player> players = new Dictionary<long, Player>();

        /// <summary>
        /// 副本玩家数据
        /// </summary>
        public Dictionary<long, CopyRankData> copyRankDataDic = new Dictionary<long, CopyRankData>();

        /// <summary>
        /// 副本房间  Key:Level或NPCID
        /// </summary>
        public Dictionary<long, Dictionary<int,BattleCopyRoom>> battleCopyRoomDic = new Dictionary<long, Dictionary<int,BattleCopyRoom>>();

        /// <summary>
        /// 副本状态变更通知消息
        /// </summary>
        public G2C_BattleCopyStateUpdate_notice message = new G2C_BattleCopyStateUpdate_notice();

        public override void ContextAwake(BatteCopyManagerComponent b_Args)
        {
            Parent = b_Args;
        }

        public override void Dispose()
        {
            if (IsDisposeable) return;
            Parent = null;
            openTimeList.Clear();

            foreach (var i in battleCopyRoomDic)
            {
                foreach (var j in i.Value)
                {
                    j.Value.Dispose();
                }
            }

            battleCopyRoomDic.Clear();
            copyRankDataDic.Clear();
            timeKeys = null;
            timeValues = null;
            base.Dispose();
        }
    }
    public class CopyTime : CustomComponent
    {
        public long TimerId;
        public long BelongGameUserId;
        public long BelongGameNpcId;
        public int  GameAreaId;
        public CopyTime(int AreaId, long UserId, long NpcId)
        {
            GameAreaId = AreaId;
            BelongGameUserId = UserId;
            BelongGameNpcId = NpcId;
            TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000 * 1800, CopyTimerType.CheckCopyTime, this);
        }
        public override void Dispose()
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref TimerId);
            TimerId = 0;
            BelongGameUserId = 0;
            BelongGameNpcId = 0 ;
            GameAreaId = 0;
        }
    }
    public class MobTime : CustomComponent
    {
        public long TimerId;
        public long BelongGameNpcId;
        public int copyRandomIndex;
        public int GameAreaId;
        public MobTime(int AreaId, long NpcId,int NpcConfig)
        {
            GameAreaId = AreaId;
            BelongGameNpcId = NpcId;
            copyRandomIndex = NpcConfig;
            TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(60000, CopyTimerType.CreateBobTime, this);
        }
        public override void Dispose()
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref TimerId);
            TimerId = 0;
            copyRandomIndex = 0;
            BelongGameNpcId = 0;
            GameAreaId = 0;
        }
    }
    public class BattleCopyRoom : CustomFrameWork.ADataContext<BattleCopyComponent>
    {
        /// <summary>
        /// 房间类型
        /// </summary>
        public int type = 0;

        /// <summary>
        /// 房间等级
        /// </summary>
        public int level = 0;
        public int Index = 0;
        public int needNum = 0;

        public int currentNum = 0;

        public bool IsLock = false;

        public int DoorNum = 0;

        public int CoffinNum = 0;

        public int round = 0;

        /// <summary>
        /// 副本房间所属玩家
        /// </summary>
        public long BelongGameUserId;
        /// <summary>
        /// 副本房间所属物品
        /// </summary>
        public long BelongItemID;
        /// <summary>
        /// 副本房间所属NPC
        /// </summary>
        public GameNpc BelongGameNpc;
        public int NpcMapId = 0;

        public List<long> AttackList = new List<long>();

        /// <summary>
        /// 房间玩家
        /// </summary>
        public Dictionary<long, string> playerDic = new Dictionary<long, string>();

        public MapComponent mapComponent;
        public CopyTime copyTime = null;
        public MobTime MobTime = null;
        public bool IsJionState = false;
        public int MobBossId = 0;
        public BattleCopyComponent Parent { get; private set; }

        public override void ContextAwake(BattleCopyComponent self)
        {
            Parent = self;
            type = self.Type;
        }
        public void Dispose()
        {
            type = 0;
            level = 0;
            needNum = 0;
            currentNum = 0;
            IsLock = false;
            DoorNum = 0;
            CoffinNum = 0;
            round = 0;
            BelongGameUserId = 0;
            BelongItemID = 0;
            BelongGameNpc = null;
            NpcMapId = 0;
            AttackList.Clear();
            playerDic.Clear();
            mapComponent.Dispose();
            copyTime = null;
            MobTime = null;
            IsJionState = false;
            MobBossId = 0;
            Parent = null;
        }
    }

    public class CopyRankData
    {
        public int Score { get; set; }
        public int Exp { get; set; }
        public int Coin { get; set; }
        public long Level { get; set; }
        public int Index { get; set; }

        public int Number { get; set; }

        public void Set(int _exp, int _score)
        {
            Exp += _exp;
            Score += _score;
        }

        public void SetCoin(int _coin)
        {
            Coin += _coin;
        }

    }
}
