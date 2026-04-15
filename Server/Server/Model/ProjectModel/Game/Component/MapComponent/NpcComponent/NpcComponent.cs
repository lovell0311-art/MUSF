using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public partial class GameNpc
    {
        public Npc_InfoConfig Config { get; set; }
        public NpcShopComponent ShopComponent { get; set; }

        public E_Identity Identity { get; set; } = E_Identity.Npc;
        public int X { get; set; }
        public int Y { get; set; }
        public int Angle { get; set; }
        
        private void Clear()
        {
            Config = null;
            ShopComponent = null;
        }

    }
    public partial class GameNpc : ADataContext<long>
    {
        public long Id { get; private set; }

        public override void ContextAwake(long b_Args)
        {
            Id = b_Args;
        }

        public override void Dispose()
        {
            if (IsDisposeable) return;

            Id = 0;
            Clear();
            base.Dispose();
        }
    }


    public partial class NpcComponent : TCustomComponent<MapComponent>
    {
        public Dictionary<long, GameNpc> AllNpcDic = new Dictionary<long, GameNpc>();

        public override void Dispose()
        {
            if (IsDisposeable) return;

            //TODO 清理数据
            if (AllNpcDic != null && AllNpcDic.Count > 0)
            {
                var mTemp = AllNpcDic.Values.ToList();
                for (int i = 0, len = mTemp.Count; i < len; i++)
                {
                    mTemp[i].Dispose();
                }
                AllNpcDic.Clear();
            }

            base.Dispose();
        }
    }
}