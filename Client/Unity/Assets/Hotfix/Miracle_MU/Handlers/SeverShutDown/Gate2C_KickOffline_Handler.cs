using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System;


namespace ETHotfix
{
    [MessageHandler]
    public class Gate2C_KickOffline_Handler : AMHandler<Gate2C_KickOffline>
    {
        protected override void Run(ETModel.Session session, Gate2C_KickOffline message)
        {

            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
            uIConfirm.AddCancelEventAction(()=> Application.Quit());
            uIConfirm.AddActionEvent(()=> Application.Quit());
            string str=string.Empty;
            GlobalDataManager.IsOFFLINE = true;
            switch (message.DisconnectType)
            {
                case 0://顶号
                    str = "该账号已在其他设备上登陆 如非本人操作，请您及时修改密码";
                   
                    CameraFollowComponent.Instance.followTarget = null;
                    break;
                case 1://服务器关闭
                    str = "服务器已关闭";
                    break;
                case 2://GM踢下线
                    str = $"GM踢下线\n{message.Reason}";
                    break;
                case 3://封号
                  
                    TimeSpan timeSpan = GetSpacingTime_Seconds(message.BanTillTime);
                   
                    DateTime dateTime = TimeHelper.GetDateTime_Milliseconds(message.BanTillTime);
                    str = $"您的账号存在存在异常行为({message.Reason})\n被封禁{timeSpan.Days}天{timeSpan.Hours}小时{timeSpan.Minutes}\n解封时间：{dateTime.Year}年{dateTime.Month}月{dateTime.Hour}:{dateTime.Minute}";
                  
                    break;
               
            }
            uIConfirm.SetTipText(str,true);

           
            TimeSpan GetSpacingTime_Seconds(long Seconds)
            {
                //获取时间戳 秒
             
                DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds((double)Seconds);
                DateTime curdateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddMilliseconds((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000);
                TimeSpan timeSpan = new TimeSpan(startTime.Ticks);
                TimeSpan CurtimeSpan = new TimeSpan(curdateTime.Ticks);
                return timeSpan.Subtract(CurtimeSpan).Duration();
            }

            string GetTime(long Seconds) 
            {
                long time = Seconds - TimeHelper.GetNowSecond();
                 return $"{Mathf.Floor(time / (60 * 60 * 24))}天{Mathf.Floor((time % (60 * 60 * 24)) / (60 * 60))}时{Mathf.Floor((time % (60 * 60)) / 60)}分";
            }

            string GetOffine() => message.DisconnectType switch
            {
                0 => "顶号",
                1 => "服务器关闭",
                2 => "GM踢下线",
                3 => "封号",
                _ => ""
            };
        }

        
    }
}
