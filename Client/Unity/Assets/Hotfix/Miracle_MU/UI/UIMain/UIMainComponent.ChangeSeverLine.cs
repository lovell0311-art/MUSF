using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 切换线路模块
    /// </summary>
    public partial class UIMainComponent
    {
        private SpriteAtlas LinestateAtlas;//服务器状态图集
        GameObject ServerLine;
        Text curseverName;
        public bool IsChangeLine = false;//是否在切换线路
        public void InitChangeSeverLine()
        {
            LinestateAtlas = ReferenceCollector_Main.GetSpriteAtlas("LinestateAtlas");
            ServerLine = ReferenceCollector_Main.GetImage("ServerLine").gameObject;
            // ServerLine.transform.Find("Image").GetComponent<Button>().onClick.AddSingleListener(()=> { UIComponent.Instance.VisibleUI(UIType.UIChangeLine); });
            ReferenceCollector collector = ServerLine.GetReferenceCollector();
            curseverName = collector.GetText("Label");
            collector.GetButton("Image").onClick.AddSingleListener(OpenChangeLinePanel);
            SetCurSeverLine();

        }

        private void OpenChangeLinePanel()
        {
            UIComponent.Instance.VisibleUI(UIType.UIChangeLine);
        }

        public void SetCurSeverLine()
        {
            //GlobalDataManager.EnterZoneID = lastLineInfo.ZoneId;
            //GlobalDataManager.EnterZoneName = lastLineInfo.lineName;
            //GlobalDataManager.EnterLineID = lastLineInfo.LineId;
            //LastLineInfo lastLineInfo = LastLineInfo.GetLastLineInfo();
            //Log.DebugGreen($"{lastLineInfo.lineName},{lastLineInfo.ZoneId}");
            curseverName.text = GlobalDataManager.EnterZoneName;
        }

        /// <summary>
        /// 根据服务器状态 获得对应得sprite
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private Sprite GetSprite(E_LineStateType type)
        {
            switch (type)
            {
                case E_LineStateType.NORMAL:
                    return LinestateAtlas.GetSprite("green");
                case E_LineStateType.HOT:
                    return LinestateAtlas.GetSprite("red");
                default:
                    return LinestateAtlas.GetSprite("yellow");
            }
        }
    }
}
