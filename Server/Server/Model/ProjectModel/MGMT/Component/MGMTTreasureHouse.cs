using CustomFrameWork.Baseic;
using CustomFrameWork;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
    public class MGMTTreasureHouse : TCustomComponent<MainFactory>
    {
        public Dictionary<int, Dictionary<int, List<THItemInfo>>> keyValuePairs;

        public Dictionary<long, Dictionary<int,List<THItemInfo>>> TemporaryPlayer;

        public List<THItemInfo> Dlelist;
        public List<int> DBID;
        public override void Awake()
        {
            keyValuePairs = new Dictionary<int, Dictionary<int, List<THItemInfo>>> ();
            TemporaryPlayer = new Dictionary<long, Dictionary<int, List<THItemInfo>>>();
            Dlelist = new List<THItemInfo>();
            DBID = new List<int>();
        }

        public override void Dispose()
        {
            keyValuePairs.Clear ();
            TemporaryPlayer.Clear ();
            DBID.Clear();
        }
    }
}
