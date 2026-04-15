using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace ETHotfix
{
    public class UIPassportComponent : Component
    {
        public ScrollRect scrollRect1;// 声明
        public ScrollRect scrollRect2;// 声明
        public ScrollRect scrollRect3;// 声明
        public ScrollRect scrollRect4;// 声明
        public ScrollRect scrollRect5;// 声明
        public UICircularScrollView<PassportInfo> scrollViewInfo1; //声明奖励活动信息
        public UICircularScrollView<PassportInfo> scrollViewInfo2; //声明奖励活动信息
        public UICircularScrollView<PassportInfo> scrollViewInfo3; //声明奖励活动信息
        public UICircularScrollView<PassportInfo> scrollViewInfo4; //声明奖励活动信息
        public UICircularScrollView<PassportInfo> scrollViewInfo5; //声明奖励活动信息
        public Button shop328Btn, shop688Btn;
        public Text taskProgess1, taskProgess2, actitiyTxt;
        public bool Open = false;
        public int intType = 1;//任务当前类型
    }
}
