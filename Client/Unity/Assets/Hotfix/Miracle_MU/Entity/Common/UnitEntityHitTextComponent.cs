using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using DG.Tweening;
using UnityEngine.UI;

using System.Text;

namespace ETHotfix
{
    [ObjectSystem]
    public class UnitEntityHitTextComponentAwake : AwakeSystem<UnitEntityHitTextComponent>
    {
        public override void Awake(UnitEntityHitTextComponent self)
        {
            self.unitEntity = self.GetParent<UnitEntity>();
            self.curColor = ColorTools.GetColorHtmlString(Color.yellow);

            self.ui = UIComponent.Instance.Get(UIType.UIMainCanvas);
            self.parent = self.ui?.GameObject.GetComponent<Canvas>();
        }
    }

    /// <summary>
    /// 实体显示被击文字组件
    /// </summary>
    public class UnitEntityHitTextComponent : Component
    {
        private const string UIUnitTakeDemageText = "UIUnitTakeDamageText";

        public UnitEntity unitEntity;
        public string curColor;

        public UI ui;
        public Canvas parent;
        public Vector3 UnitPos => new Vector3(unitEntity.Position.x, 5, unitEntity.Position.z);

        public void SetColor(Color color)
        {
            curColor = ColorTools.GetColorHtmlString(color);
        }

        public void SetColor(string color)
        {
            curColor = color;
        }

        public void ShowHitText(string damageValue, Vector3 textScale)
        {
            if (ui == null)
            {
                ui = UIComponent.Instance.Get(UIType.UIMainCanvas);
            }

            if (ui == null)
            {
                return;
            }

            if (parent == null)
            {
                parent = ui.GameObject.GetComponent<Canvas>();
            }

            if (parent == null || CameraComponent.Instance?.MainCamera == null || CameraComponent.Instance?.UICamera == null)
            {
                return;
            }

            GameObject obj = UIMainComponent.Instance.ObjDequeue();
            if (obj == null)
            {
                return;
            }

            obj.SetActive(true);

            Vector3 scrPos = CameraComponent.Instance.MainCamera.WorldToScreenPoint(UnitPos);
            scrPos = CameraComponent.Instance.UICamera.ScreenToWorldPoint(scrPos);
            obj.transform.position = scrPos;

            Text text = obj.GetComponent<Text>();
            if (text == null)
            {
                UIMainComponent.Instance.Recycle(obj);
                return;
            }

            // Reset pooled hit text state before reuse.
            ResetTextVisualState(text);
            text.raycastTarget = false;
            text.transform.localScale = Vector3.one;
            text.text = $"<color={curColor}>{damageValue}</color>";

            obj.transform.SetAsLastSibling();
            text.transform.localScale = textScale * 0.75f;

            text.transform.DOBlendableLocalMoveBy(Vector3.up * 300, 3f);

            Sequence seq = DOTween.Sequence();
            seq.Append(text.transform.DOScale(Vector3.one, 0.4f));
            seq.AppendInterval(0.7f);
            seq.Append(text.transform.DOScale(Vector3.one * 1.2f, 0.4f));

            UIMainComponent.Instance?.ScheduleRecycle(obj, 3000);
        }

        private static void ResetTextVisualState(Text text)
        {
            if (text == null)
            {
                return;
            }

            text.transform.DOKill();
            text.DOKill();

            Color color = text.color;
            color.a = 1f;
            text.color = color;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
        }
    }
}
