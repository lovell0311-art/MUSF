using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// Boss …À∫¶≈≈––
    /// </summary>
    [MessageHandler]
    public class G2C_SendDamageRanking_Handler : AMHandler<G2C_SendDamageRanking>
    {
        protected override void Run(ETModel.Session session, G2C_SendDamageRanking message)
        {
            
            //UIMainComponent.Instance?.ShowDamagedRank();
            //UIMainComponent.Instance?.ShowDamageInfo(message.InfoList.ToList());
            
        }
    }
}
