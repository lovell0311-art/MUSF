using System;
using System.Collections;
using System.Diagnostics;

using System.Linq;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{
    public static partial class BattleComponentSystem
    {
        /*
        /// <summary>
        /// 战斗循环处理
        /// </summary>
        /// <param name="b_Component"></param>
        private static async void UpdateFrame(this BattleComponent b_Component)
        {
            TimerComponent mTimerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            while (b_Component.IsDisposeable == false && b_Component.IsRunUpdate)
            {
                var mRunCodeTimeOnBefore = Help_TimeHelper.GetNow();

                // 20毫秒更新一次逻辑
                int mFrameTime = 20;
                long mFrameTimeCompensate = mFrameTime - b_Component.UpdateTimeCompensate;
                // 最少需要等待1帧，防止卡死在这个函数中，让进程去处理其他逻辑
                if (mFrameTimeCompensate < 1) mFrameTimeCompensate = 1;

                await mTimerComponent.WaitAsync(mFrameTimeCompensate);
                if (b_Component.IsDisposeable || b_Component.IsRunUpdate == false) break;

                var mRunCodeTimeOnEnd = Help_TimeHelper.GetNow();
                int mInterval = (int)(mRunCodeTimeOnEnd - mRunCodeTimeOnBefore);
                if (mInterval <= 0) mInterval = mFrameTime;

                try
                {
                    b_Component.UpdateFrameLogic(mInterval, mRunCodeTimeOnEnd);
                }
                catch (Exception e)
                {
                    Log.Error(e);
                }

                mRunCodeTimeOnEnd = Help_TimeHelper.GetNow();
                b_Component.UpdateTimeCompensate = mRunCodeTimeOnEnd - mRunCodeTimeOnBefore - mFrameTimeCompensate;

                //if (b_Component.Parent.MapId == 1)
                //    LogToolComponent.FileLog("time", $" 差值时间:{mInterval} 等待时间:{mFrameTimeCompensate} 补偿时间:{b_Component.UpdateTimeCompensate} mapid:{b_Component.Parent.MapId} areaid:{b_Component.Parent.Parent.Parent.SourceId}", false);
            }
        }
        */

        /// <summary>
        /// 每一帧处理
        /// </summary>
        /// <param name="b_Component"></param>
        public static void UpdateFrameLogic(this BattleComponent b_Component, long b_UpdateFrame, long b_CurrentFrame)
        {
            if (b_Component.IsDisposeable || b_Component.IsRunUpdate == false) return;

            if (b_Component.UpdateTick >= long.MaxValue - 1) b_Component.UpdateTick = 0;
            b_Component.UpdateTick++;

            b_Component.CurrentTimeTick = b_CurrentFrame;
            var mCurrentFrame = b_CurrentFrame;

            //Console.WriteLine(DateTime.Now.ToString());
            //MapComponent mapComponent = b_Component.Parent;

            CombatSourceRecycleComponent.Instance.ClearCombatSource();

            // 逻辑
            b_Component.RunSyncAction();
        }
    }
}