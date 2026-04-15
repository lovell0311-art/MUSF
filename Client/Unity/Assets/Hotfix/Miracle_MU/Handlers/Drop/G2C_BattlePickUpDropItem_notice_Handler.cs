using ETModel;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 装备物品拾取 成功通知
    /// </summary>
    [MessageHandler]
    public class G2C_BattlePickUpDropItem_notice_Handler : AMHandler<G2C_BattlePickUpDropItem_notice>
    {
        protected override void Run(ETModel.Session session, G2C_BattlePickUpDropItem_notice message)
        {
           // KnapsackItemsManager.DisDropItemList.Clear();
          //  KnapsackItemsManager.DisDropItemList=message.InstanceId.ToList();
            foreach (var item in message.InstanceId)
            {
                if (UnitEntityComponent.Instance.KnapsackItemEntityDic.ContainsKey(item))
                {
                    UnitEntityComponent.Instance.Remove(item);
                }
              
            }
        }
    }
}
