using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using CustomFrameWork;

namespace ETModel
{

    public class C_ServerArea : CustomFrameWork.Baseic.TCustomComponent<ServerAreaManagerComponent, int, List<int>>
    {
        public int SourceId { get; private set; }
        public int GameAreaId { get; private set; }
        public int GameAreaRouteId { get; private set; }

        public List<int> VirtualIdlist { get; private set; }

        public override void Awake(int b_AreaId, List<int> b_VirtualIdlist)
        {
            this.SourceId = b_AreaId;
            this.GameAreaId = b_AreaId >> 16;
            this.GameAreaRouteId = b_AreaId & 0xffff;

            AddCustomComponent<BatteCopyManagerComponent>();

            if (b_VirtualIdlist == null)
            {
                VirtualIdlist = new List<int>() { this.GameAreaId };
                Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Add(this.GameAreaId);
            }
            else
            {
                for (int i = 0, len = b_VirtualIdlist.Count; i < len; i++)
                {
                    var mTemp = b_VirtualIdlist[i];

                    var mNewId = mTemp >> 16;
                    Root.MainFactory.GetCustomComponent<DBMongodbProxySaveManageComponent>().Add(mNewId);
                }
                VirtualIdlist = new List<int>(b_VirtualIdlist);
            }

            AddCustomComponent<MapManageComponent>();
            AddCustomComponent<ActivitiesComponent>();//»î¶¯

            
            AddCustomComponent<CitySiegeActivities>();
        }

        public override void Dispose()
        {
            if (IsDisposeable) return;

            GameAreaId = 0;
            GameAreaRouteId = 0;
            if (VirtualIdlist != null)
            {
                VirtualIdlist.Clear();
                VirtualIdlist = null;
            }
            base.Dispose();
        }
    }
}