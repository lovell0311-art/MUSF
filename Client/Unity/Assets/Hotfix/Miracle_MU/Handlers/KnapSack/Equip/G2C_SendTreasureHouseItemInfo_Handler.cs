using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class G2C_SendTreasureHouseItemInfo_Handler : AMHandler<G2C_SendTreasureHouseItemInfo>
    {
        protected override void Run(ETModel.Session session, G2C_SendTreasureHouseItemInfo message)
        {
            KnapsackDataItem knapsackData = new KnapsackDataItem();
            knapsackData.ConfigId = message.AllProperty.ConfigId;
            for (int p = 0, pcount = message.AllProperty.PropList.Count; p < pcount; p++)
            {
                knapsackData.Set(message.AllProperty.PropList[p]);
            }

            for (int e = 0, count = message.AllProperty.ExecllentEntry.Count; e < count; e++)
            {
                knapsackData.SetExecllentEntry(message.AllProperty.ExecllentEntry[e]);
            }
            for (int e = 0, count = message.AllProperty.SpecialEntry.Count; e < count; e++)
            {
                knapsackData.SetExtraEntryDic(message.AllProperty.SpecialEntry[e]);
            }
            for (int q = 0, count = message.AllProperty.ExtraEntry.Count; q < count; q++)
            {
                knapsackData.SetExtraEntryDic(message.AllProperty.ExtraEntry[q]);
            }

            UIComponent.Instance.Get(UIType.UITreasureHouse)?.GetComponent<UITreasureHouseComponent>()?.GetAllAtrs(knapsackData);

        }
    }

}