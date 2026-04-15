using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{

    [ObjectSystem]
    public class UITeamComponentStartSystem : StartSystem<UITeamComponent>
    {
        public override void Start(UITeamComponent self)
        {
            self.Init_Show();
        }
    }

}
