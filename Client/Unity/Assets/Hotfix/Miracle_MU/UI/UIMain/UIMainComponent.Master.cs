using ETModel;

namespace ETHotfix
{
    public partial class UIMainComponent
    {
        private void TryOpenMasterPanel()
        {
            UnitEnityProperty property = roleEntity?.Property ?? enityProperty;
            if (property == null)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "\u5927\u5e08\u754c\u9762\u521d\u59cb\u5316\u4e2d");
                return;
            }

            if (property.GetProperValue(E_GameProperty.Level) < 400)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "\u7b49\u7ea7\u672a\u8fbe\u5230400\u7ea7\uff0c\u65e0\u6cd5\u4f7f\u7528\u5927\u5e08");
                return;
            }

            if (property.GetProperValue(E_GameProperty.OccupationLevel) < 3)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "\u8bf7\u5148\u5b8c\u6210\u4e09\u8f6c");
                return;
            }

            UIComponent.Instance.VisibleUI(UIType.DaShiCanvas);
        }
    }
}
