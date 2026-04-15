using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{

    [ObjectSystem]
    public class UITreasureMapComponentAwake : AwakeSystem<UITreasureMapComponent>
    {
        public override void Awake(UITreasureMapComponent self)
        {
            self.referenceCollector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.referenceCollector.GetButton("closeBtn").onClick.AddSingleListener(self.Close);
            self.referenceCollector.GetButton("startBtn").onClick.AddSingleListener(self.Start);
            self.Icon = self.referenceCollector.GetImage("Icon");
        }
    }
    /// <summary>
    /// 藏宝图对话
    /// </summary>
    public class UITreasureMapComponent : Component,IUGUIStatus
    {
        public ReferenceCollector referenceCollector;
        private long npcId;
        public Image Icon;
        public void Close() 
        {
            UIComponent.Instance.Remove(UIType.UITreasureMap);
        }
        public void Start() 
        {

            EnterBattle().Coroutine();
         //开启藏宝图
         async ETVoid EnterBattle()
            {
               
                G2C_EnterBattleCopyResponse g2C_EnterBattle = (G2C_EnterBattleCopyResponse)await SessionComponent.Instance.Session.Call(new C2G_EnterBattleCopyRequest 
                {
                 Level=0,
                 Type=4,
                 NPCID= npcId
                });
                if (g2C_EnterBattle.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_EnterBattle.Error.GetTipInfo());
                }
                else
                {
                  
                }
                UIMainComponent.Instance.HideFuBenInfo();
                Close();
            }
        }
        //改变NPCUI
        public void InitNPCIcon(int configID) 
        {
            Log.DebugBrown("npc" + configID);
            Sprite sprite = referenceCollector.GetSprite("huolongwang");
            switch (configID)
            {
                case 10045:
                    sprite = referenceCollector.GetSprite("huolongwang");
                    break;
                case 10046:
                    sprite = referenceCollector.GetSprite("mayahushou");
                    break;
                case 10047:
                    sprite = referenceCollector.GetSprite("mowangkundun");
                    break;
                case 10054:
                    sprite = referenceCollector.GetSprite("Boss_huangjinhuolongwang_UI");
                    break;
                case 10055:
                    sprite = referenceCollector.GetSprite("Boss_bingshuangjuzhu_UI");
                    break;
                case 10056:
                    sprite = referenceCollector.GetSprite("Monster_junzhu_UI");
                    break;
                case 10057:
                    sprite = referenceCollector.GetSprite("Boss_meidusha_UI");
                    break;
                case 10058:
                    sprite = referenceCollector.GetSprite("anheizhihuiguan");
                    break;
                default:
                    break;
            }
            Icon.sprite = sprite;
            Icon.SetNativeSize();
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
        }

        public void OnVisible(object[] data)
        {
            if (data.Length > 0)
            {
                npcId = long.Parse($"{data[0]}");
               
                InitNPCIcon(int.Parse($"{data[1]}"));
            }
        }

        public void OnVisible()
        {
           
        }

        public void OnInVisibility()
        {
         
        }
    }
}
