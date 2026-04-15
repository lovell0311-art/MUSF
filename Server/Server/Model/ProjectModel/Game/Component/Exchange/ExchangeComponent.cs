using CustomFrameWork;
using CustomFrameWork.Baseic;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;

namespace ETModel
{
    public class ExchangeComponent : TCustomComponent<Player>
    {
        public const int BoxWidth = 8;
        public const int BoxHeight = 4;
        public Player mPlayer
        {
            get
            {
                return Parent;
            }
        }

        /// <summary>
        /// 交易中的玩家ID，没有则为0
        /// </summary>
        public long ExchangeTargetGameUserId;
        /// <summary>
        /// 是否锁定交易物品
        /// </summary>
        public bool IsLock;
        /// <summary>
        /// 交易物品
        /// </summary>
        public Dictionary<long,Vector2> ItemPosDict = new Dictionary<long,Vector2>();
        /// <summary>
        /// 交易物品栏
        /// </summary>
        public ItemsBoxStatus BoxStatus;
        /// <summary>
        /// 交易金额
        /// </summary>
        public int ExchangeCoin;
        /// <summary>
        /// 取消交易冷却时间20秒
        /// </summary>
        public long ExchangeTime;
        /// <summary>
        /// 每次开始交易的时候初始化状态（勿调用，请调用StartExchange方法）
        /// </summary>
        /// <param name="targetPlayer"></param>
        public void InitState(long targetPlayer)
        {
            ExchangeTargetGameUserId = targetPlayer;
            IsLock = false;
            ItemPosDict.Clear();
            if (BoxStatus == null)
            {
                BoxStatus = new ItemsBoxStatus();
            }
            BoxStatus.Init(BoxWidth, BoxWidth * BoxHeight);
            ExchangeCoin = 0;
        }

        public void ClearState()
        {
            ExchangeTargetGameUserId = 0;
            IsLock = false;
            ItemPosDict.Clear();
            BoxStatus = null;
            ExchangeCoin = 0;
            ExchangeTime = 0;
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;
            ClearState();
            base.Dispose();
        }

        
    }
}
