using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    /// <summary>
    /// 取消摊位 通知
    /// </summary>
    [MessageHandler]
    public class G2C_BaiTanClose_notice_Handler : AMHandler<G2C_BaiTanClose_notice>
    {
        protected override void Run(ETModel.Session session, G2C_BaiTanClose_notice message)
        {
            for (int i = 0, length=message.BaiTanInstanceId.Count; i < length; i++)
            {
                if (UnitEntityComponent.Instance.Get<RoleEntity>(message.BaiTanInstanceId[i]) is RoleEntity role)
                {
                    role.GetComponent<RoleStallUpComponent>().CloseOtherStallUp();
                }
            }
            
        }
    }
}
