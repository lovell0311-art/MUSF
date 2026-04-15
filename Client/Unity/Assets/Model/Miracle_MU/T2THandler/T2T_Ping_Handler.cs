using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETModel
{
    [MessageHandler]
    public class T2T_Ping_Handler : AMHandler<T2T_Ping>
    {
        protected override void Run(Session session, T2T_Ping message)
        {
            //Log.DebugRed($"{session==null} ·¢ËÍÐÄÌø°ü");
            session?.Send(new T2T_Ping { });
        }
    }
}
