using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIFuBenComponentAwake : AwakeSystem<UIFuBenComponent>
    {
        public override void Awake(UIFuBenComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.ChooseFuBenPanel = self.collector.GetImage("ChooseFuBenPanel").gameObject;
            self.EMoGuangChangPanel = self.collector.GetImage("EMoGuangChangPanel").gameObject;
            self.XueSeChengBaoPanel = self.collector.GetImage("XueSeChengBaoPanel").gameObject;
            self.collector.GetButton("XueSeBtn").onClick.AddSingleListener(() =>
            {
                self.ShowEMoGuangChang(false,true);
            });
            self.collector.GetButton("EMoBtn").onClick.AddSingleListener(() =>
            {
                self.ShowEMoGuangChang();
            });
            self.collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIFuBen));//关闭副本选择
            self.ShowEMoGuangChang(false);
            self.Init_EMoGuangChang();
            self.Init_XueSeChengBao();
        }
    }
    /// <summary>
    /// 副本
    /// </summary>
    public partial class UIFuBenComponent : Component
    {
        public ReferenceCollector collector;
        public GameObject ChooseFuBenPanel, EMoGuangChangPanel, XueSeChengBaoPanel;

        /// <summary>
        /// 显示恶魔广场
        /// </summary>
        /// <param name="isShow"></param>
        public void ShowEMoGuangChang(bool isShow = true,bool isShowXueSe = false)
        {
            ChooseFuBenPanel.SetActive(!isShow && !isShowXueSe);
            EMoGuangChangPanel.SetActive(isShow);
            XueSeChengBaoPanel.SetActive(isShowXueSe);
        }
        /// <summary>
        /// 恶魔广场 层数选择
        /// </summary>

        public void Init_EMoGuangChang()
        {
            ReferenceCollector collector = EMoGuangChangPanel.GetReferenceCollector();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIFuBen));
            GameObject FubenLayer = collector.GetGameObject("FubenLayer");
            //注册恶魔广场 进入事件
            for (int i = 0, length = FubenLayer.transform.childCount; i < length; i++)
            {
                int lev = i + 1;
                Button button = FubenLayer.transform.GetChild(i).GetComponent<Button>();
                button.onClick.AddSingleListener(() =>
                {
                    Log.DebugGreen($"请求进入恶魔广场第->{lev}<-层");
                    //进入恶魔广场
                    EnterBattleCopyResponse(lev,1).Coroutine();
                });
            }
        }
        /// <summary>
        /// 血色城堡 层数选择
        /// </summary>

        public void Init_XueSeChengBao()
        {
            ReferenceCollector collector = XueSeChengBaoPanel.GetReferenceCollector();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIFuBen));
            GameObject FubenLayer = collector.GetGameObject("FubenLayer");
            //注册恶魔广场 进入事件
            for (int i = 0, length = FubenLayer.transform.childCount; i < length; i++)
            {
                int lev = i + 1;
                Button button = FubenLayer.transform.GetChild(i).GetComponent<Button>();
                button.onClick.AddSingleListener(() =>
                {
                  //  Log.DebugGreen($"请求进入恶魔广场第->{lev}<-层");
                    //进入恶魔广场
                    EnterBattleCopyResponse(lev, 2).Coroutine();
                });
            }
        }
        public async ETTask EnterBattleCopyResponse(int level,int type)
        {
            G2C_EnterBattleCopyResponse g2C_EnterBattleCopy = (G2C_EnterBattleCopyResponse)await SessionComponent.Instance.Session.Call(new C2G_EnterBattleCopyRequest
            {
                Level = level,
                Type = type,
                PlayerID=0
            });
            if (g2C_EnterBattleCopy.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_EnterBattleCopy.Error.GetTipInfo());
               // Log.DebugRed($"{g2C_EnterBattleCopy.Error}");
            }
            else
            {
                UIMainComponent.Instance.FuBenLevel = level;
                UIMainComponent.Instance.FuBenType = type;
                UIMainComponent.Instance.ChangEMoGuangChangState(false);//恶魔广场 
                UIMainComponent.Instance.ChangXueSeChengBaoState(false);//血色城堡
                switch (type)
                {
                    case 1:
                        UIMainComponent.Instance.SetEmoTips();
                        break;
                    case 2:
                        UIMainComponent.Instance.SeXueseTips();
                        break;
                    default:
                        break;
                }
                UIComponent.Instance.Remove(UIType.UIFuBen);
            }
        }
            


    }
}
