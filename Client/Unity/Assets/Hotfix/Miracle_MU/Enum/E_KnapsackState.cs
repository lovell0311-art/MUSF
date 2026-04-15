using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    /// <summary>
    /// 背包状态类型
    /// </summary>
    public enum E_KnapsackState : byte
    {
        /// <summary>背包</summary>
        KS_Knapsack = 0,
        /// <summary>装备</summary>
        KS_Equipment=1,
        /// <summary>合成</summary>
        KS_Gem_Merge=2,
        /// <summary>仓库 </summary>
        KS_Ware_House=3,
        /// <summary>商城 </summary>
        KS_Shop=4,
        /// <summary>摆摊</summary>
        KS_Stallup=5,
        /// <summary>其他玩家的摊位</summary>
        KS_Stallup_OtherPlayer=6,
        /// <summary>赠送金币</summary>
        KS_GiveCoin=7,
        /// <summary>赠送物品</summary>
        KS_GiveGoods=8,
        /// <summary>物品寄售</summary>
        KS_Consignment=9,
        /// <summary>属性还原</summary>
        KS_Reduction=10,
        /// <summary>镶嵌</summary>
        KS_Inlay=11,
        /// <summary>交易</summary>
        KS_Trade = 12,
        /// <summary>装备回收</summary>
        KS_Recycle = 13,
        /// <summary>强化，再生、还原</summary>
        KS_Revision = 14,
    }
}
