using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIWarAllianceAwakeSystem : AwakeSystem<UIWarAllianceComponent>
    {
        public override void Awake(UIWarAllianceComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.roleEntity = UnitEntityComponent.Instance.LocalRole;
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(()=>UIComponent.Instance.Remove(UIType.UIWarAlliance));//注册关闭事件

            self.InitPanel = self.collector.GetGameObject("InitPanel");//默认界面
            self.CreatPanel = self.collector.GetGameObject("CreatWar");//创建战盟
            self.PreviewPanel = self.collector.GetGameObject("PreviewWar");//预览战盟
            self.JoinPanel = self.collector.GetGameObject("JoinWar");//加入战盟
            self.WarMainPanel = self.collector.GetGameObject("WarMain");//战盟主界面
            self.collector_Look = self.collector.GetGameObject("LookWarInfoPanel");//战盟主界面

            self.Init_ColorSpriteDic();
            self.InitPane();
            self.InitCreat();
            self.Init_Preview();
            self.Init_WarMainInfo();
            self.Init_PostSet();
            self.Init_Join();
        }
    }
}