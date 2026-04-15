using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod("NSMapEntity.Destory")]
    public class NSMapEntityDestory_LevelMap : ITEventMethodOnRun<ETModel.EventType.NSMapEntity.Destory>
    {
        public void OnRun(ETModel.EventType.NSMapEntity.Destory args)
        {
            if(args.self.Map == null)
            {
                return;
            }
            args.self.Map.MapEntityLeave(args.self);
            args.self.Parent = null;
        }
    }

    public static class MapEntitySystem
    {

    }
}
