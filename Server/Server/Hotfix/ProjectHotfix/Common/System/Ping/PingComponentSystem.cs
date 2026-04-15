using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ETModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Threading.Tasks;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using CustomFrameWork;

namespace ETHotfix
{
    [EventMethod(typeof(PingComponent), EventSystemType.INIT)]
    public class PingComponentEventOnInit : ITEventMethodOnInit<PingComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(PingComponent b_Component)
        {
            b_Component.OnInit();
        }
    }
    /// <summary>
    /// 用来缓存数据
    /// </summary>
    public static class PingComponentSystem
    {
        public static void OnInit(this PingComponent b_Component)
        {
            b_Component.OnRun().Coroutine();
        }

        public static async Task OnRun(this PingComponent b_Component)
        {
            long mIntervalTicks = b_Component.Ping * 12; // 60秒
            var mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            while (true)
            {
                await mTimerComponent.WaitAsync(b_Component.Ping * 1000);

                if (b_Component.IsDisposeable) return;
                try
                {
                    T2T_Ping mPingMessage = new T2T_Ping();
                    long mTicks = Help_TimeHelper.GetNowSecond();
                    var mSessions = Game.Scene.GetComponent<NetOuterComponent>().GetClientSession();
                    for (int i = 0, len = mSessions.Length; i < len; i++)
                    {
                        var session = mSessions[i];

                        if (session.IsDisposed == false)
                        {
                            long mLastReceiveTick = b_Component.GetLastReceiveTick(session);
                            if (mTicks - mLastReceiveTick > mIntervalTicks)
                            {
                                session.GetComponent<NetworkSendLogComponent>()?.PrintLog();
                                Log.Info($"#Session##PingClose# ({session.Id}) LastRev={mLastReceiveTick} diff={mTicks - mLastReceiveTick}");
                                Game.Scene.GetComponent<NetOuterComponent>().Remove(session.Id);
                                continue;
                            }

                            session.Send(mPingMessage);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }
            }
        }

        private static long GetLastReceiveTick(this PingComponent b_Component, Session b_Session)
        {
            if (b_Session.Channel is KChannel kChannel)
            {
                long mLastReceiveTick = Math.Max(kChannel.LastRecvTimestamp, kChannel.LastRecvPingTimestamp) / 1000;
                b_Component.ReceiveTickDic[b_Session.Id] = mLastReceiveTick;
                return mLastReceiveTick;
            }

            if (b_Component.ReceiveTickDic.TryGetValue(b_Session.Id, out long mCachedTick))
            {
                return mCachedTick;
            }

            mCachedTick = Help_TimeHelper.GetNowSecond();
            b_Component.ReceiveTickDic[b_Session.Id] = mCachedTick;
            return mCachedTick;
        }

        public static async Task UpdatePingTick(this PingComponent b_Component, Session b_Session)
        {
            try
            {
                if (b_Session == null || b_Session.IsDisposed)
                {
                    return;
                }

                long mTicks = b_Component.GetLastReceiveTick(b_Session);
                b_Component.ReceiveTickDic[b_Session.Id] = mTicks;
            }
            catch (Exception e)
            {
                Log.Error(e);
                // TODO 断开这个出问题的session
            }
        }
    }
}
