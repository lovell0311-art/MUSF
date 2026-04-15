using ETModel;
using System;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UITreasureHouseComponentAwake : AwakeSystem<UITreasureHouseComponent>
    {
        public override void Awake(UITreasureHouseComponent self)
        {
            self.Awake();
        }
    }

    public partial class UITreasureHouseComponent : Component
    {
        public ReferenceCollector collector;
        public void Awake()
        {
            collector = GetParent<UI>().GameObject.GetReferenceCollector();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() =>
            {
                DeletePlayerTreasureHouse().Coroutine();
                UIComponent.Instance.Remove(UIType.UITreasureHouse);
            });
            //搜索
            Search();
            //筛选
            Filtrate();
            //物品信息/交易记录
            Right();
            //翻页
            PageTurning();
            //物品列表
            ItelList();
            //左部列表
            ItemOrder();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            ModelHide();
            uICircular_Item.Dispose();
        }
    }

}
