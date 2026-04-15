using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{
    public static class Help_PlayerHelper
    {
        public static Session GetSessionMGMT(this Player self)
        {
            var allConfig = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.MGMT);
            foreach(var config in allConfig)
            {
                List<int> gameAreaIds = JsonHelper.FromJson<List<int>>(config.RunParameter);
                if ((gameAreaIds[0] >> 16) != self.GameAreaId) continue;

                return Game.Scene.GetComponent<NetInnerComponent>().Get(config.ServerInnerIP);
            }
            return null;
        }
    }
}
