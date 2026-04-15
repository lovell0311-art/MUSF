using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace ETHotfix
{
    [EventMethod(typeof(DBMongodbProxySaveManageComponent), EventSystemType.INIT)]
    public class DBMongodbProxySaveManageComponentEventOnInit : ITEventMethodOnInit<DBMongodbProxySaveManageComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(DBMongodbProxySaveManageComponent b_Component)
        {
            b_Component._RunIndex++;
            b_Component.UpdateAsync(b_Component._RunIndex).Coroutine();
        }
    }
    public static partial class DBMongodbProxySaveManageComponentSystem
    {
        public static async Task UpdateAsync(this DBMongodbProxySaveManageComponent b_Component, int b_RunIndex)
        {
            TimerComponent timerComponent = Root.MainFactory.GetCustomComponent<TimerComponent>();
            while (b_Component.IsDisposeable == false)
            {
                // 三秒检查一次
                await timerComponent.WaitAsync(3000);

                if (b_Component.IsDisposeable || b_RunIndex != b_Component._RunIndex)
                {// 可能开了另一个 update
                    return;
                }
                try
                {
                    long mChangeTick = CustomFrameWork.TimeHelper.ClientNowSeconds();

                    if (b_Component._Instance.Count > 0)
                    {
                        var mGameTemplist = b_Component._Instance.Values.ToArray();
                        for (int i = 0, len = mGameTemplist.Length; i < len; i++)
                        {
                            mGameTemplist[i].Update(mChangeTick);
                        }
                    }
                }
                catch(Exception e)
                {
                    Log.Error(e);
                }
            }
        }
    }
}
