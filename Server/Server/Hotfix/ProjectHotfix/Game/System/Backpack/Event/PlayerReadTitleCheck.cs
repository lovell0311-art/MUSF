using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.EventType;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using TencentCloud.Tcr.V20190924.Models;

namespace ETHotfix
{

//     [EventMethod("PlayerReadyComplete")]
//     public class PlayerReadyTitleCheck : ITEventMethodOnRun<PlayerReadyComplete>
//     {
//         public void OnRun(PlayerReadyComplete args)
//         {
//             CheckTitle(args.player).Coroutine();
//         }
// 
//         private async Task CheckTitle(Player player)
//         {
//             var Info = player.GetCustomComponent<PlayerTitle>();
//             if (Info != null) 
//             {
//                 if (Info.CheckTitle(60007))
//                 {
//                     Info.AddTitle(60007,1);
//                     Info.SendTitle();
//                 }
//             }
//         }
//     }
}
