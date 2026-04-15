using CustomFrameWork;
using CustomFrameWork.Baseic;
using ETModel;
using ETModel.Robot;
using System.Threading.Tasks;
using TencentCloud.Bri.V20190328.Models;

namespace ETHotfix
{
    [Timer(CopyTimerType.CheckCopyTime)]
    public class CheckCopyTimer : ATimer<CopyTime>
    {
        public override void Run(CopyTime self)
        {
            if (self.TimerId == 0 || self.BelongGameUserId ==0 || self.BelongGameNpcId ==0 || self.GameAreaId==0) return;

            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea(self.GameAreaId);
            if (mServerArea != null)
            {
                BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
                if (batteCopyManagerCpt != null)
                {
                    BattleCopyComponent battleCopyCpt = batteCopyManagerCpt.Get((int)CopyType.CangBaoTu);
                    if (battleCopyCpt != null)
                    {
                        if (battleCopyCpt.battleCopyRoomDic.TryGetValue(self.BelongGameNpcId, out var copyRoomList) != false && copyRoomList.Count >0)
                        {
                            if (copyRoomList[0].BelongGameUserId == self.BelongGameUserId)
                            {
                                ////处理玩家道具
                                //int mAreaId = self.GameAreaId >> 16;
                                //Player player = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, self.BelongGameUserId);
                                //if (player != null)
                                //{
                                //    var backpack = player.GetCustomComponent<BackpackComponent>();
                                //    if (backpack != null)
                                //    {
                                //        Item item = backpack.GetItemByUID(copyRoomList[0].BelongItemID);
                                //        if (item != null)
                                //        {
                                //            backpack.UseItem(item, "藏宝图副本关闭使用");
                                //        }
                                //    }
                                //}
                                //处理地图NPC
                                var map = Help_MapHelper.GetMapByMapId(mServerArea, copyRoomList[0].NpcMapId, self.BelongGameUserId);
                                if (map != null)
                                {
                                    var findPos = map.GetFindTheWay2D(copyRoomList[0].BelongGameNpc.X, copyRoomList[0].BelongGameNpc.Y);
                                    map.QuitMap(findPos, copyRoomList[0].BelongGameNpc);
                                    G2C_AttackResult_notice mAttackResultNotice = new G2C_AttackResult_notice();
                                    mAttackResultNotice.AttackTarget = self.BelongGameNpcId;
                                    mAttackResultNotice.HpValue = 0;
                                    map.SendNotice(findPos, mAttackResultNotice);
                                    NpcComponent npcComponent = map.GetCustomComponent<NpcComponent>();
                                    npcComponent.AllNpcDic.Remove(self.BelongGameNpcId);
                                    copyRoomList[0].BelongGameNpc.Dispose();
                                }
                                //处理副本房间
                                copyRoomList[0].copyTime.Dispose();
                                copyRoomList[0].ClearRoom();
                                copyRoomList[0].Dispose();
                                battleCopyCpt.battleCopyRoomDic.Remove(self.BelongGameNpcId);
                                return;
                            }
                            return;
                        }
                        return;
                    }
                    return;
                }
                return;
            }
            return;
        }
    }
}