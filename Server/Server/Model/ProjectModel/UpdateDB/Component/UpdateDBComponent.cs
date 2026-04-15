using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;

namespace ETModel
{
    public class UpdateDBComponent : TCustomComponent<MainFactory>
    {
        public override void Start()
        {
            Log.Console("开始刷库...");
            t20230228.UpdateDB_ItemData.StartAsync().Coroutine();
            Log.Console("刷库完成");
        }

    }
}
