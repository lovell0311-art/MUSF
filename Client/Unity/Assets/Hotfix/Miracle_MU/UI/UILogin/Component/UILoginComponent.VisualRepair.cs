using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UILoginComponent
    {
        private const string LoginAccountPlaceholderText = "\u8bf7\u8f93\u5165\u8d26\u53f7";
        private const string LoginPasswordPlaceholderText = "\u8bf7\u8f93\u5165\u5bc6\u7801";
        private const string LoginEnterGameText = "\u8fdb\u5165\u6e38\u620f";
        private const string LoginRegisterText = "\u6ce8\u518c\u8d26\u53f7";
        private const string LoginResetPasswordText = "\u91cd\u7f6e\u5bc6\u7801";

        private static readonly Color LoginPanelFallbackColor = new Color32(24, 22, 20, 235);
        private static readonly Color LoginFieldFallbackColor = new Color32(44, 42, 40, 255);
        private static readonly Color LoginButtonFillColor = new Color32(66, 62, 56, 255);
        private static readonly Color LoginLabelColor = new Color32(212, 191, 144, 255);
        private static readonly Color LoginInputTextColor = new Color32(244, 239, 228, 255);
        private static readonly Color LoginPlaceholderColor = new Color32(214, 205, 182, 160);
        private static readonly Color LoginButtonTextColor = new Color32(238, 230, 214, 255);
        private static readonly Color LoginToggleBackgroundColor = new Color32(54, 50, 46, 255);
        private static readonly Color LoginToggleCheckColor = new Color32(238, 230, 214, 255);
        private static readonly Color LoginSpriteVisibleColor = new Color32(255, 255, 255, 255);

        private static Font cachedLoginFont;

        private void SetLoginPanelsDefaultState()
        {
            if (LoginPanel != null)
            {
                LoginPanel.SetActive(true);
                LoginPanel.transform.SetAsLastSibling();
            }

            if (RegisterPanel != null)
            {
                RegisterPanel.SetActive(false);
            }

            if (ResetPassWorld != null)
            {
                ResetPassWorld.SetActive(false);
            }
        }

        private void RepairLoginVisuals(GameObject uiRoot)
        {
            if (!VisualRepairCompatibility.ShouldRepairLogin(uiRoot, LoginPanel, loginaccount, loginpassWord, loginBtn, opeenregisterBtn, ResterBtn))
            {
                return;
            }

            EnsureScale(uiRoot?.transform);
            EnsureScale(LoginPanel?.transform);
            EnsureScale(RegisterPanel?.transform);
            EnsureScale(ResetPassWorld?.transform);

            Image loginBg = ResolveComponent<Image>(uiRoot?.transform, "LoginBg");
            LoginStageTrace.Append($"RepairLoginVisuals loginBgFound={loginBg != null} loginPanelFound={LoginPanel != null}");
            RepairImagePresentation(loginBg, LoginPanelFallbackColor);

            ApplyLabelFallback(LoginPanel?.transform, "LoginAccountTxt");
            ApplyLabelFallback(LoginPanel?.transform, "PassWordTxt");
            RepairInputField(loginaccount, LoginAccountPlaceholderText);
            RepairInputField(loginpassWord, LoginPasswordPlaceholderText);
            RepairButton(loginBtn, LoginEnterGameText);
            RepairButton(opeenregisterBtn, LoginRegisterText);
            RepairButton(ResterBtn, LoginResetPasswordText);
            RepairAgreementToggle(isReadTog);
        }

        private void RepairInputField(InputField field, string fallbackPlaceholder)
        {
            if (field == null)
            {
                return;
            }

            EnsureScale(field.transform);
            RepairImagePresentation(field.targetGraphic as Image, LoginFieldFallbackColor);

            if (field.textComponent != null)
            {
                field.textComponent.font = ResolveLoginFont(field.transform);
                field.textComponent.color = LoginInputTextColor;
                field.textComponent.fontStyle = FontStyle.Normal;
            }

            if (field.placeholder is Text placeholder)
            {
                placeholder.font = ResolveLoginFont(field.transform);
                placeholder.color = LoginPlaceholderColor;
                placeholder.fontStyle = FontStyle.Normal;
                if (string.IsNullOrWhiteSpace(placeholder.text))
                {
                    placeholder.text = fallbackPlaceholder;
                }
            }
        }

        private void RepairButton(Button button, string fallbackText)
        {
            if (button == null)
            {
                return;
            }

            EnsureScale(button.transform);
            RepairImagePresentation(button.targetGraphic as Image, LoginButtonFillColor);
            button.transition = Selectable.Transition.None;

            bool preserveLegacyLabel = HasVisibleLegacyButtonLabel(button.transform);
            SetCodexLabelVisible(button.transform, !preserveLegacyLabel);

            if (!preserveLegacyLabel)
            {
                DisableLegacyButtonBadge(button.transform);
                DisableLegacyButtonText(button.transform);

                Text buttonText = EnsureButtonText(button.transform, fallbackText);
                if (buttonText == null)
                {
                    LoginStageTrace.Append($"RepairButton missingText name={button.name}");
                    return;
                }

                buttonText.gameObject.SetActive(true);
                buttonText.enabled = true;
                buttonText.text = fallbackText ?? buttonText.text;
                buttonText.color = LoginButtonTextColor;
                buttonText.fontStyle = FontStyle.Bold;
                buttonText.raycastTarget = false;
                return;
            }

            EnableLegacyButtonVisuals(button.transform);
        }

        private void RepairAgreementToggle(Toggle toggle)
        {
            if (toggle == null)
            {
                return;
            }

            EnsureScale(toggle.transform);
            toggle.transition = Selectable.Transition.None;

            if (toggle.targetGraphic is Image background)
            {
                RepairImagePresentation(background, LoginToggleBackgroundColor);
                background.raycastTarget = true;
            }

            if (toggle.graphic is Image checkmark)
            {
                checkmark.enabled = true;
                if (HasSprite(checkmark))
                {
                    EnsureSpriteVisible(checkmark);
                }
                else
                {
                    checkmark.sprite = null;
                    checkmark.overrideSprite = null;
                    checkmark.material = null;
                    checkmark.type = Image.Type.Simple;
                    checkmark.color = LoginToggleCheckColor;
                }
                checkmark.raycastTarget = false;

                RectTransform rectTransform = checkmark.rectTransform;
                rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                rectTransform.sizeDelta = new Vector2(18f, 18f);
                rectTransform.anchoredPosition = Vector2.zero;
            }
        }

        private void ApplyLabelFallback(Transform root, string name)
        {
            Text label = ResolveComponent<Text>(root, name);
            if (label == null)
            {
                return;
            }

            label.font = ResolveLoginFont(root);
            label.color = LoginLabelColor;
            label.fontStyle = FontStyle.Bold;
        }

        private static void RepairImagePresentation(Image image, Color fallbackColor)
        {
            if (image == null)
            {
                return;
            }

            image.enabled = true;
            if (HasSprite(image))
            {
                EnsureSpriteVisible(image);
                return;
            }

            image.sprite = null;
            image.overrideSprite = null;
            image.material = null;
            image.type = Image.Type.Simple;
            image.color = fallbackColor;
        }

        private Text EnsureButtonText(Transform buttonRoot, string fallbackText)
        {
            if (buttonRoot == null)
            {
                return null;
            }

            Transform labelRoot = FindChild(buttonRoot, "CodexLabel");
            GameObject labelObject;
            if (labelRoot == null)
            {
                labelObject = new GameObject("CodexLabel", typeof(RectTransform), typeof(Text));
                labelObject.transform.SetParent(buttonRoot, false);

                RectTransform rectTransform = labelObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMin = new Vector2(16f, 8f);
                rectTransform.offsetMax = new Vector2(-16f, -8f);
            }
            else
            {
                labelObject = labelRoot.gameObject;
            }

            labelObject.transform.SetAsLastSibling();

            Text text = labelObject.GetComponent<Text>();
            text.font = ResolveLoginFont(buttonRoot);
            text.supportRichText = false;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 18;
            text.resizeTextMaxSize = 34;
            text.fontSize = 30;
            text.raycastTarget = false;
            text.text = fallbackText;

            Outline outline = labelObject.GetComponent<Outline>() ?? labelObject.AddComponent<Outline>();
            outline.effectColor = new Color32(30, 26, 22, 255);
            outline.effectDistance = new Vector2(1f, -1f);
            outline.useGraphicAlpha = true;

            return text;
        }

        private static bool HasVisibleLegacyButtonLabel(Transform buttonRoot)
        {
            if (buttonRoot == null)
            {
                return false;
            }

            foreach (Text text in buttonRoot.GetComponentsInChildren<Text>(true))
            {
                if (text == null || text.name == "CodexLabel")
                {
                    continue;
                }

                if (text.enabled && !string.IsNullOrWhiteSpace(text.text))
                {
                    return true;
                }
            }

            foreach (Image image in buttonRoot.GetComponentsInChildren<Image>(true))
            {
                if (image == null || image.transform == buttonRoot || image.name != "Text")
                {
                    continue;
                }

                if (HasSprite(image) && image.enabled && image.color.a > 0.05f)
                {
                    return true;
                }
            }

            return false;
        }

        private static void DisableLegacyButtonText(Transform buttonRoot)
        {
            if (buttonRoot == null)
            {
                return;
            }

            foreach (Text text in buttonRoot.GetComponentsInChildren<Text>(true))
            {
                if (text == null || text.name == "CodexLabel")
                {
                    continue;
                }

                text.enabled = false;
            }
        }

        private static void DisableLegacyButtonBadge(Transform buttonRoot)
        {
            if (buttonRoot == null)
            {
                return;
            }

            foreach (Image image in buttonRoot.GetComponentsInChildren<Image>(true))
            {
                if (image == null || image.transform == buttonRoot)
                {
                    continue;
                }

                if (image.name != "Text")
                {
                    continue;
                }

                if (image.GetComponent<Text>() != null)
                {
                    continue;
                }

                image.enabled = false;
                image.raycastTarget = false;
                image.color = new Color(0f, 0f, 0f, 0f);
            }
        }

        private static void EnableLegacyButtonVisuals(Transform buttonRoot)
        {
            if (buttonRoot == null)
            {
                return;
            }

            foreach (Text text in buttonRoot.GetComponentsInChildren<Text>(true))
            {
                if (text == null || text.name == "CodexLabel")
                {
                    continue;
                }

                text.enabled = true;
                text.color = LoginLabelColor;
            }

            foreach (Image image in buttonRoot.GetComponentsInChildren<Image>(true))
            {
                if (image == null || image.transform == buttonRoot)
                {
                    continue;
                }

                if (image.name != "Text")
                {
                    continue;
                }

                image.enabled = true;
                image.raycastTarget = false;
                RepairImagePresentation(image, LoginButtonFillColor);
            }
        }

        private static void SetCodexLabelVisible(Transform buttonRoot, bool visible)
        {
            Transform labelRoot = FindChild(buttonRoot, "CodexLabel");
            if (labelRoot == null)
            {
                return;
            }

            labelRoot.gameObject.SetActive(visible);
        }

        private static bool HasSprite(Image image)
        {
            return image != null && (image.overrideSprite != null || image.sprite != null);
        }

        private static void EnsureSpriteVisible(Image image)
        {
            if (image == null)
            {
                return;
            }

            if (image.color.a < 0.2f)
            {
                image.color = LoginSpriteVisibleColor;
                return;
            }

            if (image.color.r < 0.1f && image.color.g < 0.1f && image.color.b < 0.1f)
            {
                image.color = LoginSpriteVisibleColor;
            }
        }

        private Font ResolveLoginFont(Transform root)
        {
            if (cachedLoginFont != null)
            {
                return cachedLoginFont;
            }

            Text sample = ResolveComponent<Text>(root, "LoginAccountTxt")
                ?? ResolveComponent<Text>(root, "PassWordTxt")
                ?? loginaccount?.textComponent
                ?? loginpassWord?.textComponent;

            if (sample?.font != null)
            {
                cachedLoginFont = sample.font;
                return cachedLoginFont;
            }

            cachedLoginFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return cachedLoginFont;
        }

        private static T ResolveComponent<T>(Transform root, string name) where T : UnityEngine.Component
        {
            Transform target = FindChild(root, name);
            return target != null ? target.GetComponent<T>() : null;
        }

        private static Transform FindChild(Transform root, string name)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == name)
            {
                return root;
            }

            foreach (Transform child in root)
            {
                Transform found = FindChild(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static void EnsureScale(Transform target)
        {
            if (target == null)
            {
                return;
            }

            if (target.localScale == Vector3.zero)
            {
                target.localScale = Vector3.one;
            }
        }
    }
}
