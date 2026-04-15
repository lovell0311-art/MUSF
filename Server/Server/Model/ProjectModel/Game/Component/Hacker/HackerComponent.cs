using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    /* @brief 加速外挂检测
    * @note 
    *	根据客户端与服务端的时间差，检查加速外挂.
    * 客户端与服务端存在时间差，并不意味着使用外挂。
    * 有些客户端时间流速会比服务器略快或略慢，或者
    * 玩家设备开启了自动校准时间。会突然与服务器的
    * 时间有所不同。
    */
    public class SpeedUpCheatCheck
    {
        public class TimeTickInfo
        {
            /* @brief 记录时间*/
            public long ServerTime = 0;
            public long ClientTime = 0;
        };
        public static TimeTickInfo TimeTick = new TimeTickInfo();
        /* @brief 上次的数据 */
        public TimeTickInfo OldTimeTick = new TimeTickInfo();
        public long OldDiff = 0;

        /* @brief 风险 > 100.f  */
        public double RiskPct = 0f;


        public void Add(TimeTickInfo timeTick)
        {
            AddToRiskCheck(timeTick);
        }

	    private void AddToRiskCheck(TimeTickInfo timeTick)
        {
            long nowDiff = timeTick.ClientTime - timeTick.ServerTime;
            if (OldDiff != 0)
            {
                long diff = nowDiff - OldDiff;
                if (diff > 0)
                {
                    RiskPct += (diff / (double)(diff + 50)) * 10;
                }
                else if (diff < 0)
                {
                    diff = Math.Abs(diff);
                    RiskPct -= (diff / (double)(diff + 50)) * 10;
                }
                RiskPct -= (timeTick.ServerTime - OldTimeTick.ServerTime) * 0.0002f;
                if (RiskPct < 0f)
                {
                    RiskPct = 0f;
                }
            }

            OldTimeTick.ServerTime = timeTick.ServerTime;
            OldTimeTick.ClientTime = timeTick.ClientTime;

            OldDiff = nowDiff;
        }
    };

    public class HackerComponent : TCustomComponent<Player>
    {
        public ClientTimeComponent _ClientTime = null;
        public SpeedUpCheatCheck _SpeedUpCheatCheck = null;
        public long TimerId = 0;

        public override void Dispose()
        {
            if (IsDisposeable) return;
            base.Dispose();

            _ClientTime = null;
            _SpeedUpCheatCheck = null;
            TimerId = 0;
        }
    }
}
