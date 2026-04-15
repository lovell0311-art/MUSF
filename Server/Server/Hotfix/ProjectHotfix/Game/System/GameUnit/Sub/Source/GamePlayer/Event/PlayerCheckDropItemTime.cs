using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    [Timer(TimerType.CheckDropItemTime)]
    public class CheckDropItemTimeTimer : ATimer<CheckDropItemTimeComponent>
    {
        public override void Run(CheckDropItemTimeComponent self)
        {
            if (self.Parent.OnlineStatus != EOnlineStatus.Online) return;   // 玩家正在进入游戏 或 正在下线

            var gamePlayer = self.Parent.GetCustomComponent<GamePlayer>();
            var ShopInfo   = self.Parent.GetCustomComponent<PlayerShopMallComponent>();
            long Time = Help_TimeHelper.GetNowSecond();
            string Updata = gamePlayer.dBCharacterDroplimit.MinimumGuarantee;
            long Id = gamePlayer.dBCharacterDroplimit.Id;
            var Title = self.Parent.GetCustomComponent<PlayerTitle>();

            int CoinCnt = 750;
            if (Title != null && !Title.CheckTitle(60012))
                CoinCnt = 1500;

            DateTime dateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            dateTime = dateTime.ToUniversalTime();
            // 获取时间戳（Unix 时间戳，以秒为单位）
            long timestamp = (long)(dateTime - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            int state = 0;
            state = ShopInfo.GetPlayerShopState(DeviationType.MaxMonthlyCard) ? 2 : 0;
            var NewDrop = new DBCharacterDroplimit(state, CoinCnt, timestamp);
            //宝石
            if (gamePlayer.dBCharacterDroplimit.GemstoneTime <= Time)
            {
                gamePlayer.dBCharacterDroplimit.GemstoneCnt = NewDrop.GemstoneCnt;
                gamePlayer.dBCharacterDroplimit.GemstoneTime = NewDrop.GemstoneTime;
            }
            //羽毛
            if (gamePlayer.dBCharacterDroplimit.FeatherTime <= Time)
            {
                gamePlayer.dBCharacterDroplimit.FeatherCnt = NewDrop.FeatherCnt;
                gamePlayer.dBCharacterDroplimit.FeatherTime = NewDrop.FeatherTime;
            }
            //藏宝图碎片
            if (gamePlayer.dBCharacterDroplimit.CangBaotuSPTime <= Time)
            {
                gamePlayer.dBCharacterDroplimit.CangBaotuSPCnt = NewDrop.CangBaotuSPCnt;
                gamePlayer.dBCharacterDroplimit.CangBaotuSPTime = NewDrop.CangBaotuSPTime;
            }
            //卓越
            if (gamePlayer.dBCharacterDroplimit.ExcellenceTime <= Time)
            {
                gamePlayer.dBCharacterDroplimit.ExcellenceCnt = NewDrop.ExcellenceCnt;
                gamePlayer.dBCharacterDroplimit.ExcellenceTime = NewDrop.ExcellenceTime;
            }
            //套装
            if (gamePlayer.dBCharacterDroplimit.SuitTime <= Time)
            {
                gamePlayer.dBCharacterDroplimit.SuitCnt = NewDrop.SuitCnt;
                gamePlayer.dBCharacterDroplimit.SuitTime = NewDrop.SuitTime;
            }
            if (gamePlayer.dBCharacterDroplimit.MiracleCoinTime <= Time)
            {
                gamePlayer.dBCharacterDroplimit.MiracleCoinCnt = NewDrop.MiracleCoinCnt;
                gamePlayer.dBCharacterDroplimit.MiracleCoinTime = NewDrop.MiracleCoinTime;
            }
            if (Updata != "")
                gamePlayer.dBCharacterDroplimit.MGLsit = Updata.Split(';').Select(x => x.Split(',')).ToDictionary(x => (int.Parse(x[0]), int.Parse(x[1])), x => int.Parse(x[2]));
            
            gamePlayer.SetDropItemDB();

            //if (gamePlayer.dBCharacterDroplimit.UpdataTiem <= Help_TimeHelper.GetNowSecond())
            //{
            //    string Updata = gamePlayer.dBCharacterDroplimit.MinimumGuarantee;

            //    long Id = gamePlayer.dBCharacterDroplimit.Id;
            //    int state = 0;
            //    state += ShopInfo.GetPlayerShopState(DeviationType.MinMonthlyCard) ? 1 : 0;
            //    state += ShopInfo.GetPlayerShopState(DeviationType.MaxMonthlyCard) ? 1 : 0;

            //    gamePlayer.dBCharacterDroplimit = new DBCharacterDroplimit(state);
            //    gamePlayer.dBCharacterDroplimit.Id = Id;
            //    gamePlayer.dBCharacterDroplimit.GameUserId = self.Parent.GameUserId;
            //    if(Updata != "")
            //        gamePlayer.dBCharacterDroplimit.MGLsit = Updata.Split(';').Select(x => x.Split(',')).ToDictionary(x => (int.Parse(x[0]), int.Parse(x[1])), x => int.Parse(x[2]));
            //    gamePlayer.SetDropItemDB();
            //}
        }
    }

    [EventMethod(typeof(CheckDropItemTimeComponent), EventSystemType.INIT)]
    public class CheckDropItemTimeComponentEventOnInit : ITEventMethodOnInit<CheckDropItemTimeComponent>
    {
        public void OnInit(CheckDropItemTimeComponent b_Component)
        {
            // 10秒循环检查，如果不需要那么准确。间隔可以改长
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000, TimerType.CheckDropItemTime, b_Component);
        }
    }

    [EventMethod(typeof(CheckDropItemTimeComponent), EventSystemType.DISPOSE)]
    public class CheckDropItemTimeComponentOnDispose : ITEventMethodOnDispose<CheckDropItemTimeComponent>
    {
        public override void OnDispose(CheckDropItemTimeComponent b_Component)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
        }
    }
}
