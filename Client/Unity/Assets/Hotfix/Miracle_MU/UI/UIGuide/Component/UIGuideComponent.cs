using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
namespace ETHotfix
{

    public static class GuideType
    {
        //创建角色后的第一步引导 ---寻找幻影导师领取新手祝福
        public const int MainTask = 1;
        public const int Attribute = 2;
        //背包
        public const int Knaspack = 3;
    }

    public class UIGuideComponent:Component
    {
        public UserGuidance guidance;
        public Guide_AllConfig allConfig;
        public Guide_StepConfig stepConfig;
        public Image guideIng;
        public int stepIndex;
        public int[] stepIds;
        public GameObject knapsackdelete;
        public GameObject knapsackwar;
        public GameObject updir;
        public GameObject downdir;
        public float time = 0;
    }
}
