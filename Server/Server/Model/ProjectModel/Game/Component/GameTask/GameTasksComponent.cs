using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class GameTasksComponent : TCustomComponent<Player>
    {
        public DBGameTasksData data;

        public override void Dispose()
        {
            if (IsDisposeable) return;



            base.Dispose();
        }

    }
}
