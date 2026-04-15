using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UIMainCanvas)]
    public class UIMainComponentFactory : IUIFactory
    {
        private static readonly string[] MainUiDependencies =
        {
            "linestateatlas.unity3d",
            "ui_common_bgss.unity3d",
            "ui_common_btnss.unity3d",
            "ui_common_redpoints.unity3d",
            "ui_common_togbgs.unity3d",
            "ui_dashis.unity3d",
            "ui_main_news.unity3d",
            "ui_mainpanels.unity3d",
            "ui_skills.unity3d",
        };

        public UI Create()
        {
            foreach (string dependency in MainUiDependencies)
            {
                AssetBundleComponent.Instance.LoadBundle(dependency);
            }

            GameObject panel = ResourcesComponent.Instance.LoadGameObject(UIType.UIMainCanvas.StringToAB(),UIType.UIMainCanvas);
            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIMainCanvas,panel,false);
            ui.AddComponent<UIMainComponent>();
            return ui;
        }

      
    }
}
