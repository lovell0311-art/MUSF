using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{

    public class GuideComponent:Component
    {
        public static GuideComponent Instance;

        public List<Guide_AllConfig> Guide_AllConfigs;
        public List<Guide_StepConfig> Guide_StepConfigs;
        public Guide_AllConfig CurGuideConfig;
        public bool IsGuideIng;
    }
}