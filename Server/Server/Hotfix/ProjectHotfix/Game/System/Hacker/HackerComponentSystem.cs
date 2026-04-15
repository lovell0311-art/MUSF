using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;


namespace ETHotfix
{
    [Timer(TimerType.HackerCheck)]
    public class HackerCheckTimerTimer : ATimer<HackerComponent>
    {
        public override void Run(HackerComponent self)
        {
            if(self._ClientTime == null)
            {
                self._ClientTime = self.Parent.GetCustomComponent<ClientTimeComponent>();
            }
           
            if (self._ClientTime.ClientDiffTime - self._ClientTime.ServerDiffTime > 1000)
            {
                SpeedUpCheatCheck.TimeTick.ServerTime = self._ClientTime.ServerDiffTime;
                SpeedUpCheatCheck.TimeTick.ClientTime = self._ClientTime.ClientDiffTime;
                self._SpeedUpCheatCheck.Add(SpeedUpCheatCheck.TimeTick);
                //Log.PLog("外挂", $"a:{self.Parent.UserId} r:{self.Parent.GameUserId} 玩家时间流速异常 DiffTime:{self._SpeedUpCheatCheck.OldDiff} RiskPct:{self._SpeedUpCheatCheck.RiskPct}");
                Log.PLog($"#外挂# a:{self.Parent.UserId} r:{self.Parent.GameUserId} 玩家时间流速异常 DiffTime:{self._SpeedUpCheatCheck.OldDiff} RiskPct:{self._SpeedUpCheatCheck.RiskPct}");
                // TODO 当玩家风险评估达到 100% 时，对其进行封禁
                if (self._SpeedUpCheatCheck.RiskPct >= 100f)
                {
                    Log.PLog($"#外挂# a:{self.Parent.UserId} r:{self.Parent.GameUserId} 玩家时间流速异常 DiffTime:{self._SpeedUpCheatCheck.OldDiff} RiskPct:{self._SpeedUpCheatCheck.RiskPct}");
                    self.Ban("使用加速外挂");
                }
            }
        }
    }

    [EventMethod(typeof(HackerComponent), EventSystemType.INIT)]
    public class HackerComponentSystemEventOnInit : ITEventMethodOnInit<HackerComponent>
    {
        public void OnInit(HackerComponent self)
        {
            self.OnInit();
            self.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(1000 * 10, TimerType.HackerCheck, self);
        }
    }

    [EventMethod(typeof(HackerComponent), EventSystemType.DISPOSE)]
    public class HackerComponentSystemEventOnDispose : ITEventMethodOnDispose<HackerComponent>
    {
        public override void OnDispose(HackerComponent self)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref self.TimerId);
        }
    }

    public static class HackerComponentSystem
    {
        public static void OnInit(this HackerComponent self)
        {
            self._SpeedUpCheatCheck = new SpeedUpCheatCheck();
        }

        public static void Ban(this HackerComponent self,string msg)
        {
            Log.PLog( $"#外挂# a:{self.Parent.UserId} r:{self.Parent.GameUserId} 封禁账号({msg})");
            AccountHelper.Ban(self.Parent.UserId, Help_TimeHelper.GetNow() + (1000 * 60 * 60 * 24 * 3), msg).Coroutine();
        }
    }
}
