using ETModel;


namespace ETHotfix
{
    /// <summary>
    /// 仓库金币 变动
    /// </summary>
    [MessageHandler]
    public class G2C_WarehouseGoldChange_notice_Handler : AMHandler<G2C_WarehouseGoldChange_notice>
    {
        protected override void Run(ETModel.Session session, G2C_WarehouseGoldChange_notice message)
        {
           // Log.DebugGreen($"仓库金币 变动：{message.Gold}");
            KnapsackItemsManager.WareGlodCoin = message.Gold;
           // Game.EventManager.BroadCastEvent<long>(EventTypeId.WARE_GOLDCOIN_CHANGE, message.Gold);
            Game.EventCenter.EventTrigger<long>(EventTypeId.WARE_GOLDCOIN_CHANGE, message.Gold);
         
        }
    }
}
