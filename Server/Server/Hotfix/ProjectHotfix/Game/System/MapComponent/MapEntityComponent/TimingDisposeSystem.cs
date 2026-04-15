using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [Timer(TimerType.TimingDispose)]
    public class TimingDisposeTimer : ATimer<TimingDisposeComponent>
    {
        public override void Run(TimingDisposeComponent self)
        {
            self.Parent?.Dispose();
        }
    }

    
    [EventMethod(typeof(TimingDisposeComponent), EventSystemType.INIT)]
    public class TimingDisposeComponentEventOnInit : ITEventMethodOnInit<TimingDisposeComponent>
    {
        public void OnInit(TimingDisposeComponent b_Component)
        {
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewOnceTimer(Help_TimeHelper.GetNow() + 1000 * 60 * 2, TimerType.TimingDispose, b_Component);
        }
    }

    /*
    [EventMethod(typeof(TimingDisposeComponent), EventSystemType.INIT)]
    public class TimingDisposeComponentEventOnInit2 : ITEventMethodOnInit<TimingDisposeComponent,long>
    {
        public void OnInit(TimingDisposeComponent b_Component,long disposeTime)
        {
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(Help_TimeHelper.GetNow() + disposeTime, TimerType.TimingDispose, b_Component);
        }
    }
    */

    [EventMethod(typeof(TimingDisposeComponent), EventSystemType.DISPOSE)]
    public class TimingDisposeComponentEventOnDispose : ITEventMethodOnDispose<TimingDisposeComponent>
    {
        public override void OnDispose(TimingDisposeComponent b_Component)
        {
            if (b_Component.TimerId != 0)
            {
                ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
            }
        }
    }

    public static class TimingDisposeSystem
    {
    }
}
