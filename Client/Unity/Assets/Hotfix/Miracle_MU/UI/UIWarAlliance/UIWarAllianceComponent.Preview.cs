using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{

    /// <summary>
    /// 战盟 预览 模块
    /// </summary>
    public partial class UIWarAllianceComponent
    {

        ReferenceCollector collector_Preview;

        Text WarLeaderName, WarName;
        public void Init_Preview()
        {
            collector_Preview = PreviewPanel.GetReferenceCollector();
            WarLeaderName = collector_Preview.GetText("WarLeaderName");
            WarName = collector_Preview.GetText("WarName");
            ///请求 创建战盟
            collector_Preview.GetButton("NextStepBtn").onClick.AddSingleListener(() => 
            {
                CreatWar().Coroutine();
            });
            ///返回上一步 修改战盟
            collector_Preview.GetButton("LastStepBtn").onClick.AddSingleListener(() => 
            {
                Show(E_WarType.Creat);
            });
        }

        /// <summary>
        /// 初始化 战盟徽章
        /// </summary>
        public void Init_WarBadge()
        {
            Transform badge = collector_Preview.GetGameObject("Icons").transform;
            Transform temp = badge.GetChild(0).transform;
            int childCount = badge.childCount;
            for (int i = 0; i < 64; i++)
            {
                Transform transform;
                if (i < childCount)
                {
                    transform = badge.GetChild(i).transform;
                }
                else
                {
                    transform = GameObject.Instantiate<Transform>(temp, badge);
                }
                transform.GetComponent<Image>().enabled = DrawIconDic[i] != 0;
                transform.GetComponent<Image>().sprite = GetSprite(DrawIconDic[i]);
            }
            WarLeaderName.text = roleEntity.RoleName;
            WarName.text = warName;
        }
    }
}
