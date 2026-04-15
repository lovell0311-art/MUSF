using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{

    public enum E_WarType 
    {
      Init,
      Creat,
      Preview,
      Join,
      WarMain
    }
    /// <summary>
    /// 战盟 系统
    /// </summary>
    public partial class UIWarAllianceComponent : Component,IUGUIStatus
    {
        public ReferenceCollector collector;

        public GameObject InitPanel, CreatPanel, PreviewPanel, JoinPanel, WarMainPanel, collector_Look;

        public Dictionary<int, Sprite> ColorSpriteDic;

        public RoleEntity roleEntity;
        /// <summary>
        /// 显示面板
        /// </summary>
        /// <param name="type">面板的类型</param>

        public void Show(E_WarType type)
        {
            InitPanel.SetActive(type == E_WarType.Init);
            CreatPanel.SetActive(type == E_WarType.Creat);
            PreviewPanel.SetActive(type == E_WarType.Preview);
            JoinPanel.SetActive(type == E_WarType.Join);
            WarMainPanel.SetActive(type == E_WarType.WarMain);
            switch (type)
            {
                case E_WarType.Init:
                    break;
                case E_WarType.Creat:
                    //ResetIcon();//重置徽章
                    break;
                case E_WarType.Preview:
                    Init_WarBadge();
                    break;
                case E_WarType.Join:
                   
                    JoinWarAsync(1).Coroutine();
                    break;
                case E_WarType.WarMain:
                   
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 初始化 颜色精灵
        /// 将颜色精灵 缓存起来 方便使用
        /// </summary>

        public void Init_ColorSpriteDic() 
        {
            ColorSpriteDic = new Dictionary<int, Sprite>();
            for (int i = 0; i < 16; i++)
            {
                int colorIndex = i;
                ColorSpriteDic[colorIndex] = collector.GetSprite(colorIndex.ToString());
            }
        }


        public Sprite GetSprite() 
        {
            if (curChooseColorIndex == -1)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint,"请您先选择一种 颜色");
                return null;
            }
            return ColorSpriteDic[curChooseColorIndex];
        }

        /// <summary>
        /// 更具颜色 精灵 的序号 获取对应的 图片精灵
        /// </summary>
        /// <param name="colorindex"></param>
        /// <returns></returns>
        public Sprite GetSprite(int colorindex)
        {
            if (!ColorSpriteDic.ContainsKey(colorindex))
                return null;
            return ColorSpriteDic[colorindex];
        }

        public void OnVisible(object[] data)
        {
           /* if (data.Length > 0)
            {
                switch (data[0].ToString())
                {
                    case "NO":
                        Show(E_WarType.Init);
                        break;
                    case "YES":
                        OpenWarAsync().Coroutine();
                        break;
                    default:
                        break;
                }
            }
           */
        }

        public void OnVisible()
        {
            OpenWarAsync().Coroutine();
        }

        public void OnInVisibility()
        {
           
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            DisableTheAllianceCache().Coroutine();
            warMemberInfos.Clear();
            WarMemberInfoScrollView.Dispose();
            WarApplyInfos.Clear();
            WarApplyInfoScrollView.Dispose();
            WarJoinInfoScrollView.Dispose();
            ColorSpriteDic.Clear();
            badgeIcon.Clear();

        }

        public async ETTask DisableTheAllianceCache()
        {
            G2C_DisableTheAllianceCache g2C_DisableThe = (G2C_DisableTheAllianceCache)await SessionComponent.Instance.Session.Call(new C2G_DisableTheAllianceCache(){});
            if(g2C_DisableThe.Error != 0) { UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_DisableThe.Error.GetTipInfo()); }

        }
    }
}
