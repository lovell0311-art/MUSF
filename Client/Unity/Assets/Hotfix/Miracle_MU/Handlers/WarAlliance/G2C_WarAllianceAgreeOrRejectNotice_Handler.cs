using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{

    /// <summary>
    /// 同意或拒绝加入战盟 通知
    /// </summary>
    [MessageHandler]
    public class G2C_WarAllianceAgreeOrRejectNotice_Handler : AMHandler<G2C_WarAllianceAgreeOrRejectNotice>
    {
        protected override void Run(ETModel.Session session, G2C_WarAllianceAgreeOrRejectNotice message)
        {
           
            UIComponent.Instance.VisibleUI(UIType.UIHint,message.Message);
            foreach (WarInfo item in WarAllianceDatas.WarLists)
            {
                if (item.struct_WarAllince.WarAllianceID == message.WAInfo.WarAllianceID)
                {
                    if (message.Message.Contains("同意"))
                    {
                        WarAllianceDatas.IsJoinWar = true;
                        break;
                    }
                    else
                    {
                        WarAllianceDatas.IsJoinWar = false;
                        item.State = 0;
                        break;
                    }
                }
            }
         
        }
    }
}