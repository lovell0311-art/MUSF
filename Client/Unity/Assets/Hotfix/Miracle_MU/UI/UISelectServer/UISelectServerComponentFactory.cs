using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UISelectServer)]
    public class UISelectServerComponentFactory : IUIFactory
    {
        public UI Create()
        {
            string abName = UIType.UISelectServer.StringToAB();
            Log.Info($"#UISelectServerFactory# load start ab={abName} prefab={UIType.UISelectServer}");
            LoginStageTrace.Append($"UISelectServerFactory load start ab={abName}");
            GameObject gameObject = ResourcesComponent.Instance.LoadGameObject(abName, UIType.UISelectServer);
            if (gameObject == null)
            {
                LoginStageTrace.Append($"UISelectServerFactory prefab null ab={abName}");
                throw new System.Exception($"#UISelectServerFactory# prefab null ab={abName} prefab={UIType.UISelectServer}");
            }

            UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UISelectServer, gameObject, false);
            ui.AddComponent<UISelectServerComponent>();
            Log.Info($"#UISelectServerFactory# create finish ab={abName} prefab={UIType.UISelectServer}");
            LoginStageTrace.Append($"UISelectServerFactory create finish ab={abName}");
            return ui;
        }

      
    }
}
