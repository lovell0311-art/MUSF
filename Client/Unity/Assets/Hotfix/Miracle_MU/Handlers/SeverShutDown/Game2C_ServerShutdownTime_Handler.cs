using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Game2C_ServerShutdownTime_Handler : AMHandler<Game2C_ServerShutdownTime>
    {
        protected override void Run(ETModel.Session session, Game2C_ServerShutdownTime message)
        {
            var timer = TimeHelper.GetSpacingTime_Milliseconds(message.ShutdownTillTime);
            //重启服务器倒计时
            UIMainComponent.Instance?.ShowNotice($"<color=red>{(int)timer.TotalSeconds} 秒 后 将 重 启 服 务 器</color>");
        }

       
    }
}
