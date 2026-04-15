using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;

namespace ETHotfix
{
    [EventMethod(typeof(ItemSocketManager), EventSystemType.INIT)]
    public class ItemSocketManagerInitSystem : ITEventMethodOnInit<ItemSocketManager>
    {
        public void OnInit(ItemSocketManager self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(ItemSocketManager), EventSystemType.LOAD)]
    public class ItemSocketManagerLoadSystem : ITEventMethodOnLoad<ItemSocketManager>
    {
        public override void OnLoad(ItemSocketManager self)
        {
            self.OnInit();
        }
    }


    public static partial class ItemSocketManagerSystem
    {
        public static void OnInit(this ItemSocketManager self)
        {
            self.DropHoleCountSelector.Clear();

            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var jsonDict = readConfig.GetJson<ItemSocket_DropCountConfigJson>().JsonDic;
            foreach(ItemSocket_DropCountConfig v in jsonDict.Values)
            {
                self.DropHoleCountSelector.Add(v.SocketCnt, v.Rate);
            }
        }
    }
}
