using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [UIFactory(UIType.UICreatRole)]
    public class UICreatComponentFactory : IUIFactory
    {
        public UI Create()
        {
            string abName = UIType.UICreatRole.StringToAB();
            AssetBundleComponent.Instance.LoadBundle("ui_common_textss.unity3d");
            LoginStageTrace.Append($"UICreatRoleFactory load start ab={abName}");
            GameObject gameObject = ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadGameObject(abName,UIType.UICreatRole);
            if (gameObject == null)
            {
                LoginStageTrace.Append($"UICreatRoleFactory prefab null ab={abName}");
                throw new System.Exception($"UICreatRoleFactory prefab null ab={abName}");
            }
            UI uI = ComponentFactory.Create<UI, string, GameObject>(UIType.UICreatRole,gameObject,false);
            uI.AddComponent<UICreatComponent>();
            uI.AddComponent<RenderTextureComponent>();
            LoginStageTrace.Append($"UICreatRoleFactory create finish ab={abName}");
            return uI;
        }
    }
}
