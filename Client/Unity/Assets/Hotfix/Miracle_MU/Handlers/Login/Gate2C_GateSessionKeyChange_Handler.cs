using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [MessageHandler]
    public class Gate2C_GateSessionKeyChange_Handler : AMHandler<Gate2C_GateSessionKeyChange>
    {
        protected override void Run(ETModel.Session session, Gate2C_GateSessionKeyChange message)
        {
            // 用于重连的Key变动
            GateLoginKeyRecordComponent gateLoginKeyRecord = session.GetComponent<GateLoginKeyRecordComponent>();
            //gateLoginKeyRecord.GateLoginKey = message.NewKey.ToString();
            GlobalDataManager.GateLoginKey = message.NewKey.ToString();
           // LogCollectionComponent.Instance.Info("#断线重连# KeyChange");
        }
    }
}
