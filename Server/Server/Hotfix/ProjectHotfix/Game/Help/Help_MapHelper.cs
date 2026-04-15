using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CustomFrameWork;
using CustomFrameWork.Component;

namespace ETHotfix
{

    public static class Help_MapHelper
    {
        public static MapComponent GetMapByMapId(C_ServerArea b_ServerArea, int b_MapId,long GameUserId)
        {
            var mReadConfigComponent = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();

            if (mReadConfigComponent.GetJson<Map_InfoConfigJson>().JsonDic.TryGetValue(b_MapId, out var mMapConfig) == false)
            {
                return null;
            }

            if (mMapConfig.IsCopyMap == 0)
            {
                return b_ServerArea.GetCustomComponent<MapManageComponent>().GetMapByMapIndex(b_MapId);
            }
            else
            {
                return b_ServerArea.GetCustomComponent<BatteCopyManagerComponent>().GetRoomMapComponent(b_MapId, GameUserId); 
            }
        }
    }
}