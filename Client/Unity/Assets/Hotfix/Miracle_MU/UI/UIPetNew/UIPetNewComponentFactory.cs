using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [UIFactory(UIType.UIPetNew)]
    public class UIPetNewComponentFactory : IUIFactory
    {
        public UI Create()
        {
            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIPetNew.StringToAB(), UIType.UIPetNew);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIPetNew, panel);
            ui.AddComponent<UIPetNewComponent>();
            return ui;
        }
    }

}
