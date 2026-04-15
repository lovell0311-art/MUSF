using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    public partial class UIWarAllianceComponent
    {
        public void InitPane() 
        {
            ReferenceCollector collector = InitPanel.GetReferenceCollector();
            collector.GetButton("CreatWarBtn").onClick.AddSingleListener(() => 
            {
                Show(E_WarType.Creat);//创建战盟
            });
            collector.GetButton("JoinWarBtn").onClick.AddSingleListener(() => 
            {
                Show(E_WarType.Join);//加入战盟
            });
        }
    }
}
