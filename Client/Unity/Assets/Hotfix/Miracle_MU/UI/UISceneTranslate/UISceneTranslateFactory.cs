using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    [UIFactory(UIType.UISceneTranslate)]
    public class UISceneTranslateFactory : IUIFactory
    {
        private static readonly string[] SceneTranslateDependencies =
        {
            "ui_common_inputfilledbgs.unity3d",
            "ui_scenetranslates.unity3d",
        };

        public UI Create()
        {
            foreach (string dependency in SceneTranslateDependencies)
            {
                AssetBundleComponent.Instance.LoadBundle(dependency);
            }

            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UISceneTranslate.StringToAB(),UIType.UISceneTranslate);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UISceneTranslate,panel,false);
            ui.AddComponent<UISceneTranslateComponent>();
            return ui;
        }
    }
}
