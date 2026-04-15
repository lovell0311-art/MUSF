using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using System.Text;

namespace ETHotfix
{
    /// <summary>
    /// 藏宝图 位置关闭通知
    /// </summary>
    [MessageHandler]
    public class G2C_CangBaoTuPosClose_notice_Handler : AMHandler<G2C_CangBaoTuPosClose_notice>
    {
        protected override void Run(ETModel.Session session, G2C_CangBaoTuPosClose_notice message)
        {
            // 
            TreasureMapComponent.Instance.AllPoint.Remove(message.Id);
        }
    }
}
