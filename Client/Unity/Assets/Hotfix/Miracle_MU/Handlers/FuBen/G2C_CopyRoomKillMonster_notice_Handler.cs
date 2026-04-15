using ETModel;
using System.Diagnostics;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_CopyRoomKillMonster_notice_Handler : AMHandler<G2C_CopyRoomKillMonster_notice>
    {
        protected override void Run(ETModel.Session session, G2C_CopyRoomKillMonster_notice message)
        {
           // Log.DebugGreen($"怪物---> ：( {message.KilledNumber}/{message.MaxNumber} ) 当前状态：{UIMainComponent.Instance.xueSeStata}");
            UIMainComponent.Instance.SetSkillCount($"怪物 ：( {message.KilledNumber}/{message.MaxNumber} )");
        }
    }
}

