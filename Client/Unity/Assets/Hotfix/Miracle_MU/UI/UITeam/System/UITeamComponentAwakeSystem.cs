using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class UITeamComponentAwakeSystem : AwakeSystem<UITeamComponent>
    {
        public override void Awake(UITeamComponent self)
        {
            self.Init();
            self.Init_NearTeam();
            self.Init_NearPlayer();
            self.Init_Apply();
        }
    }
}
