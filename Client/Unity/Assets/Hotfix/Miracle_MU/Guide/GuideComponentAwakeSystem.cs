using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{

    [ObjectSystem]
    public class GuideComponentAwakeSystem : AwakeSystem<GuideComponent>
    {
        public override void Awake(GuideComponent self)
        {
            GuideComponent.Instance = self;

            self.Awake();
        }
    }



}
