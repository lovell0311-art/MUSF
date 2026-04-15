using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{

    /// <summary>
    /// 被踢出战盟 通知
    /// </summary>
    [MessageHandler]
    public class G2C_KickOutOfTheWarNotice_Handler : AMHandler<G2C_KickOutOfTheWarNotice>
    {
        protected override void Run(ETModel.Session session, G2C_KickOutOfTheWarNotice message)
        {
            WarAllianceDatas.IsJoinWar = false;
            WarAllianceDatas.Clear();//清理战盟数据
            UIComponent.Instance.VisibleUI(UIType.UIHint,"您已被踢出战盟");
        }
    }
}