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
    /// <summary>
    /// 这种写法麻烦点，但是支持热重载
    /// </summary>
    [Timer(TimerType.BattleComponentUpdateFrame)]
    public class BattleComponentUpdateFrameTimer : ATimer<BattleComponent>
    {
        public override void Run(BattleComponent self)
        {
            try
            {
                long mRunCodeTimeOnEnd = Help_TimeHelper.GetNow();
                self.UpdateFrameLogic(20, mRunCodeTimeOnEnd);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }


    [EventMethod(typeof(BattleComponent), EventSystemType.INIT)]
    public class BattleComponentEventOnInit : ITEventMethodOnInit<BattleComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(BattleComponent b_Component)
        {
            b_Component.OnInit();
            b_Component.TimerId = ETModel.ET.TimerComponent.Instance.NewRepeatedTimer(20, TimerType.BattleComponentUpdateFrame, b_Component);
        }
    }

    [EventMethod(typeof(BattleComponent), EventSystemType.DISPOSE)]
    public class BattleComponentEventOnDispose : ITEventMethodOnDispose<BattleComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public override void OnDispose(BattleComponent b_Component)
        {
            ETModel.ET.TimerComponent.Instance.Remove(ref b_Component.TimerId);
        }
    }

    public static partial class BattleComponentSystem
    {
        public static void OnInit(this BattleComponent b_Component)
        {
            b_Component.SyncTimerInit();

            b_Component.OnEnable();
            //b_Component.UpdateFrame();
        }
    }
}