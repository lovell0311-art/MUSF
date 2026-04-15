using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UITreasureHouseComponent
    {
        public ReferenceCollector rightListCollector;
        public Text GoldCoinTxt;
        public void Right()
        {
            rightListCollector = collector.GetImage("RightPanel").gameObject.GetReferenceCollector();
            GoldCoinTxt = rightListCollector.GetText("GoldCoinTxt");
            SetGoldCount();
            ItemInfo();
            Records();
        }
        public void SetGoldCount()
        {
            GoldCoinTxt.text = $"{UnitEntityComponent.Instance.LocalRole.Property.GetProperValue(E_GameProperty.MoJing)}";
        }
    }
}
