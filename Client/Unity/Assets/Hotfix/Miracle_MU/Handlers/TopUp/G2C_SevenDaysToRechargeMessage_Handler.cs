using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 扩展为额外充值信息
    /// </summary>
    [MessageHandler]
    public class G2C_SevenDaysToRechargeMessage_Handler : AMHandler<G2C_SevenDaysToRechargeMessage>
    {
        protected override void Run(ETModel.Session session, G2C_SevenDaysToRechargeMessage message)
        {
            //2023/4/24 0:00:00,1,6,False->日期 第几天 充值金额 领取状态
            //Dic<(string,int),(int,bool)> 日期key，int值，bool领奖状态
            //Log.DebugGreen($"七天充值信息：{message.Info}");

          //  Log.DebugGreen($"充值状态：{message.Info}");
            if (message.Info.Contains(";"))
            {
                string[] infos = message.Info.Split(';');
                for (int i = 0; i < infos.Length; i++)
                {
                    string[] st = infos[i].Split(',');
                    GlobalDataManager.SevenDaysToRechargeDic[int.Parse(st[1])] = bool.Parse(st[3]);

                    string[] time = st[0].Split(' ');
                    GlobalDataManager.SevenDaysToRechargeDic2[int.Parse(st[1])] = time[0].ToString();

                    TopUp_7_DayComponent.Instance?.RegisterReceiveRequest(int.Parse(st[1]));
                
                }

            }
            else
            {
                string[] st = message.Info.Split(',');
                GlobalDataManager.SevenDaysToRechargeDic[int.Parse(st[1])] = bool.Parse(st[3]);

                string[] time = st[0].Split(' ');
                GlobalDataManager.SevenDaysToRechargeDic2[int.Parse(st[1])] = time[0].ToString();

                TopUp_7_DayComponent.Instance?.RegisterReceiveRequest(int.Parse(st[1]));
             
            }
        }
    }
}
