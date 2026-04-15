using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_UnitLeaveView_notice_Handler : AMHandler<G2C_UnitLeaveView_notice>
    {
        protected override void Run(ETModel.Session session, G2C_UnitLeaveView_notice message)
        {
            foreach (var id in message.LeavedUnitId)
            {
                UnitEntityComponent.Instance.Remove(id);
            }
        }
    }
}