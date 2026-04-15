using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIFriendListNewComponentAwake : AwakeSystem<UIFriendListComponent>
    {
        public override void Awake(UIFriendListComponent self)
        {
            self.Init();
        }
    }
    [ObjectSystem]
    public class UIFriendListNewComponentUpdate : UpdateSystem<UIFriendListComponent>
    {
        public override void Update(UIFriendListComponent self)
        {
            self.AddFriendUpdate();
        }
    }
    public partial class UIFriendListComponent : Component
    {
        ReferenceCollector all_ReferenceCollector;
        ReferenceCollector friendReferenceCollector;
        ReferenceCollector addfriendReferenceCollector;
        ReferenceCollector peopleReferenceCollector;
        ReferenceCollector applyReferenceCollector;
        public void Init()
        {
            all_ReferenceCollector = GetParent<UI>().GameObject.GetReferenceCollector();

            FriendAwake();
            ADDFriendAwake();
            PeopleInfoAwake();
            ApplyAwake();
        }
    }

}
