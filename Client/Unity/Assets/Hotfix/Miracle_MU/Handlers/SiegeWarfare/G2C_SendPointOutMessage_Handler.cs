using ETModel;

using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_SendPointOutMessage_Handler : AMHandler<G2C_SendPointOutMessage>
    {
        protected override void Run(ETModel.Session session, G2C_SendPointOutMessage message)
        {
          
            SiegeWarfareData.SiegeWarfareIsStart = message.Status == 0 ? false : true;
            if(message.Status == 2)
            {
                SiegeWarfareData.currole = null;
                SiegeWarfareData.HavePlayer = false;
            }
            string noticeStr = message.Pointout.GetTipInfo();
          
            if (noticeStr.Contains("PlayerName"))
            {
                noticeStr = noticeStr.Replace("PlayerName", message.PlayerName);
            }
            if (noticeStr.Contains("WarName"))
            {
                noticeStr = noticeStr.Replace("WarName", message.WarName);
            }
            if (noticeStr.Contains("Tiem"))
            {
                noticeStr = noticeStr.Replace("Tiem", message.Time.ToString());
            }
            if (noticeStr.Contains("TitleName"))
            {
                TitleConfig_InfoConfig titleConfig = ConfigComponent.Instance.GetItem<TitleConfig_InfoConfig>(message.TitleName);
                noticeStr = noticeStr.Replace("TitleName", titleConfig.Name);
            }
            UIMainComponent.Instance?.ShowSiegeWarfareNotice($"{noticeStr}");

            //玩家离开座位
            if(message.Pointout == 2703)
            {
                if (SiegeWarfareData.CurroleId == 0) return;
                RoleEntity roleEntity = UnitEntityComponent.Instance.Get<RoleEntity>(message.PlayerId);
                if (roleEntity != null)
                {
                    //roleEntity.GetComponent<AnimatorComponent>()?.SetBoolValue("SiegeSitDown", false);
                }
                SiegeWarfareData.CurroleId = 0;
                SiegeWarfareData.currole = null;
                SiegeWarfareData.HavePlayer = false;
            }
            if(message.Pointout == 2705)
            {
                if(SiegeWarfareData.currole != null)
                {
                    //SiegeWarfareData.currole.GetComponent<AnimatorComponent>().SetBoolValue("SiegeSitDown", true);
                    SiegeWarfareData.CurroleId = 0;
                    SiegeWarfareData.currole = null;
                    SiegeWarfareData.HavePlayer = false;
                }
            }

            //玩家坐上座位
            if (message.Pointout == 2701)
            {
               
                if (SiegeWarfareData.CurroleId != 0) return;
                RoleEntity roleEntity = UnitEntityComponent.Instance.Get<RoleEntity>(message.PlayerId);
                if (roleEntity == null) return;
                SiegeWarfareData.CurroleId = message.PlayerId;
                SiegeWarfareData.currole = roleEntity;
                SiegeWarfareData.HavePlayer = true;
                //roleEntity?.GetComponent<AnimatorComponent>().SetBoolValue("SiegeSitDown", true);
               
                //SiegeWarfareData.currole.Game_Object.transform.parent.eulerAngles = new Vector3(0, 90, 0);
                SiegeWarfareData.currole.roleTrs.eulerAngles = new Vector3(0, 90, 0);
            }
        }
    }
}
