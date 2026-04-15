using ETModel;
using ETModel.Robot;
using ETModel.Robot.EventType;

namespace ETHotfix.Robot
{
    // 分发数值监听
    [Event("NumbericChange")]
    public class NumericChangeEventAsyncNotifyWatcher : AEvent<NumbericChange>
    {
        public override void Run(NumbericChange args)
        {
            NumericWatcherComponent.Instance.Run(args);
        }
    }
}
