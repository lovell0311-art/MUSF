using UnityEngine;
using ETModel;
using UnityEngine.UI;
using DG.Tweening;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIHintComponentAwake : AwakeSystem<UIHintComponent>
    {
        public override void Awake(UIHintComponent self)
        {
            self.Awake();
        }
    }
    /// <summary>
    /// 提示类型
    /// </summary>
    public class HintType
    {
        // 普通提示
        public const string GENERAL = "general";

        // 商城提示
        public const string MALL = "mall";

    }
    /// <summary>
    /// 提示信息组件
    /// </summary>
    public class UIHintComponent : Component,IUGUIStatus
    {
        private Text commontxt;
        private Text malltxt;
        private GameObject mall;
        private GameObject root;
        private int showVersion;

        ReferenceCollector referenceCollector;
        public void Awake()
        {
            UI ui = GetParent<UI>();
            root = ui?.GameObject;
            if (root == null)
            {
                return;
            }

            referenceCollector = root.GetReferenceCollector();
            if (referenceCollector == null)
            {
                return;
            }

            commontxt = referenceCollector.GetText("commontxt");
            malltxt = referenceCollector.GetText("malltxt");
            mall = referenceCollector.GetGameObject("Mall");

            if (commontxt != null)
            {
                commontxt.transform.localScale = Vector3.zero;
                commontxt.color = Color.red;
            }

            if (mall != null)
            {
                mall.SetActive(false);
            }

            Canvas canvas = referenceCollector.gameObject.GetComponent<Canvas>();
            if (canvas != null)
            {
                canvas.planeDistance = 5;
            }

            HideImmediately();
        }

        public void OnInVisibility()
        {
            CancelTweens();
            HideImmediately();
        }
        public void OnVisible(object[] data)
        {
            if (data == null || data.Length == 0)
            {
                HideImmediately();
                return;
            }

            ++showVersion;
            CancelTweens();
            ResetForShow();

            string message = data[0]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(message))
            {
                HideImmediately();
                return;
            }

            if (data.Length > 1)
            {
                switch (data[1].ToString())
                {
                    case HintType.MALL:
                        ShowMallHint(message, showVersion);
                        break;
                    default:
                        ShowCommonHint(message, showVersion);
                        break;
                }
            }
            else
            {
                ShowCommonHint(message, showVersion);
            }
        }

        public void OnVisible()
        {
        }

        private void ResetForShow()
        {
            if (root != null && !root.activeSelf)
            {
                root.SetActive(true);
            }

            HideHintVisuals();
        }

        private void ShowCommonHint(string message, int version)
        {
            if (commontxt == null || string.IsNullOrWhiteSpace(message))
            {
                HideIfCurrent(version);
                return;
            }

            commontxt.text = message;
            commontxt.transform.gameObject.SetActive(true);
            commontxt.transform.localScale = Vector3.zero;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(commontxt.transform.DOScale(1f, 0.2f));
            sequence.AppendInterval(1.5f);
            sequence.Append(commontxt.transform.DOScale(0f, 0.2f));
            sequence.AppendCallback(() => HideIfCurrent(version));
        }

        private void ShowMallHint(string message, int version)
        {
            if (mall == null || malltxt == null || string.IsNullOrWhiteSpace(message))
            {
                HideIfCurrent(version);
                return;
            }

            malltxt.text = message;
            mall.SetActive(true);
            mall.transform.localScale = Vector3.one;

            Sequence sequence = DOTween.Sequence();
            sequence.AppendInterval(1.7f);
            sequence.AppendCallback(() => HideIfCurrent(version));
        }

        private void HideIfCurrent(int version)
        {
            if (this.IsDisposed || version != showVersion)
            {
                return;
            }

            HideImmediately();
        }

        private void HideImmediately()
        {
            HideHintVisuals();

            UI ui = GetParent<UI>();
            if (ui != null && ui.GameObject != null)
            {
                ui.GameObject.SetActive(false);
            }
        }

        private void HideHintVisuals()
        {
            if (commontxt != null)
            {
                commontxt.text = string.Empty;
                commontxt.transform.localScale = Vector3.zero;
                commontxt.transform.gameObject.SetActive(false);
            }

            if (malltxt != null)
            {
                malltxt.text = string.Empty;
            }

            if (mall != null)
            {
                mall.SetActive(false);
                mall.transform.localScale = Vector3.one;
            }
        }

        private void CancelTweens()
        {
            if (commontxt != null)
            {
                commontxt.transform.DOKill(false);
                commontxt.DOKill(false);
            }

            if (malltxt != null)
            {
                malltxt.DOKill(false);
            }

            if (mall != null)
            {
                mall.transform.DOKill(false);
            }
        }

    }

}
