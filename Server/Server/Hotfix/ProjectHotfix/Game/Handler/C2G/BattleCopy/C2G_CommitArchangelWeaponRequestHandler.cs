using ETModel;
using System;
using CustomFrameWork;
using System.Threading.Tasks;
using CustomFrameWork.Component;

namespace ETHotfix
{
    [MessageHandler(AppType.Game)]
    public class C2G_CommitArchangelWeaponRequestHandler : AMActorRpcHandler<C2G_CommitArchangelWeaponRequest, G2C_CommitArchangelWeaponRequest>
    {
        protected override async Task<bool> BeforeCodeAsync(Session b_Connect, C2G_CommitArchangelWeaponRequest b_Request, G2C_CommitArchangelWeaponRequest b_Response, Action<IMessage> b_Reply)
        {
            using (await CoroutineLockComponent.Instance.Wait(CoroutineLockType.ActorId, b_Request.ActorId))
            {
                return await base.BeforeCodeAsync(b_Connect, b_Request, b_Response, b_Reply);
            }
        }

        protected override async Task<bool> Run(C2G_CommitArchangelWeaponRequest b_Request, G2C_CommitArchangelWeaponRequest b_Response, Action<IMessage> b_Reply)
        {
            await Task.Delay(0);
            C_ServerArea mServerArea = Root.MainFactory.GetCustomComponent<ServerAreaManagerComponent>().GetGameArea((int)b_Request.AppendData);
            if (mServerArea == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(21006);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("当前游戏服务器非目标服务器!");
                b_Reply(b_Response);
                return true;
            }

            BatteCopyManagerComponent batteCopyManagerCpt = mServerArea.GetCustomComponent<BatteCopyManagerComponent>();
            if (batteCopyManagerCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2600);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("提交副本任务道具时,没有找到副本管理组件");
                b_Reply(b_Response);
                return false;
            }

            BattleCopyComponent battleCopyCpt = batteCopyManagerCpt.Get((int)CopyType.RedCastle);
            if (battleCopyCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2601);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("提交副本任务道具时,没有找到副本组件");
                b_Reply(b_Response);
                return false;
            }

            long userId = b_Request.ActorId;
            if (!battleCopyCpt.copyRankDataDic.ContainsKey(userId))
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2602);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("提交副本任务道具时,组件中找不到玩家");
                b_Reply(b_Response);
                return false;
            }

            CopyRankData copyRankData = battleCopyCpt.copyRankDataDic[userId];
            long level = copyRankData.Level;
            int index = copyRankData.Index;
            Log.PLog($"Tijiao Player:{userId} Level:{level}Index:{index}");
            BattleCopyRoom battleCopyRoom = battleCopyCpt.battleCopyRoomDic[level][index];
            if (battleCopyRoom == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2603);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("提交副本任务道具时,攻击副本障碍时，没有找到副本房间");
                b_Reply(b_Response);
                return false;
            }
            //battleCopyRoom.  提交武器
            if (battleCopyRoom.round != (int)CopyRoomState.CommitWeapon)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(2604);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("现在还不能提交大天使武器");
                b_Reply(b_Response);
                return false;
            }

            int mAreaId = (int)(b_Request.AppendData >> 16);
            Player mPlayer = Root.MainFactory.GetCustomComponent<PlayerManageComponent>().Get(mAreaId, b_Request.ActorId);
            if (mPlayer == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(200);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("玩家没有找到!");
                b_Reply(b_Response);
                return false;
            }

            BackpackComponent backpackCpt = mPlayer.GetCustomComponent<BackpackComponent>();
            if (backpackCpt == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(704);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("背包组件异常!");
                b_Reply(b_Response);
                return false;
            }
            long ItemUID = 0;
            var ItemList = backpackCpt.GetAllItemByConfigID(320304);
            if (ItemList != null && ItemList.Count >= 1)
            {
                foreach (var Item in ItemList)
                    ItemUID = Item.Key;
            }
            else
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(705);
                //b_Response.Message = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessage("获取背包物品失败!");
                b_Reply(b_Response);
                return true;
            }
            battleCopyRoom.round = (int)CopyRoomState.Accomplish;
            battleCopyCpt.RedCastleSettlementAsync(battleCopyRoom).Coroutine();

            backpackCpt.UseItem(ItemUID, "血色城堡副本提交任务物品");

            b_Reply(b_Response);
            return true;
        }

    }
}
