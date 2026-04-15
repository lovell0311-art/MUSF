using System.Collections.Generic;
using System.Linq;

namespace ETModel
{
    public partial class C_Stall
    {
        public string StallName { get; private set; }
        public void SetStallName(string b_StallName)
        {
            StallName = b_StallName;
        }

        public int PosX { get; set; }
        public int PosY { get; set; }

        public bool IsStalling { get; set; }

        public Dictionary<long, Item> keyValuePairs = new Dictionary<long, Item>();

        public ItemsBoxStatus mItemBox;

        private void AfterClear()
        {
            StallName = null;
            PosX = 0;
            PosY = 0;
            IsStalling = false;

            if (keyValuePairs.Count > 0)
            {
                var mTemplist = keyValuePairs.Values.ToArray();
                for (int i = 0, len = mTemplist.Length; i < len; i++)
                {
                    mTemplist[i].Dispose();
                }
                keyValuePairs.Clear();
            }
            if (mItemBox != null)
            {
                mItemBox.Dispose();
                mItemBox = null;
            }
        }
    }
    public partial class C_Stall : CustomFrameWork.ADataContext<long>
    {
        public long StallId { get; private set; }
        public override void ContextAwake(long b_StallId)
        {
            StallName = "";
            StallId = b_StallId;
            IsStalling = false;
            mItemBox = new ItemsBoxStatus();
            mItemBox.Init(8, 8 * 11);
        }
        public override void Dispose()
        {
            if (IsDisposeable) return;

            AfterClear();
            StallId = default;

            base.Dispose();
        }
    }
}