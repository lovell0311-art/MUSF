
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;
using System;

namespace ETHotfix
{
    [EventMethod(typeof(MapCellAreaComponent), EventSystemType.INIT)]
    public class MapCellAreaComponentOnInit : ITEventMethodOnInit<MapCellAreaComponent>
    {
        public void OnInit(MapCellAreaComponent b_Component)
        {
            b_Component.OnInit();
        }
    }

    /// <summary>
    /// 对地图进行分域
    /// </summary>
    public static partial class MapCellAreaComponentSystem
    {
        public static void OnInit(this MapCellAreaComponent b_Component)
        {
            b_Component.OnEnable();
        }
    }
}