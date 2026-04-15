using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 转职完成 通知
    /// </summary>
    [MessageHandler]
    public class G2C_CareerChangeComplete_notice_Handler : AMHandler<G2C_CareerChangeComplete_notice>
    {
        protected override void Run(ETModel.Session session, G2C_CareerChangeComplete_notice message)
        {
          
            if (UnitEntityComponent.Instance.Get<RoleEntity>(message.GameUserId) is RoleEntity role)
            {
              
                role.Property.ChangeProperValue(E_GameProperty.OccupationLevel,message.OccupationLevel);//更新转职等级属性
            }
        }
    }
}