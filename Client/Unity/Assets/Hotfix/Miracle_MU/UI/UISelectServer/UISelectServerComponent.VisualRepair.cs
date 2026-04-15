using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UISelectServerComponent
    {
        private const string ServerSelectEnterGameText = "\u5f00\u59cb";
        private const string ServerSelectOpenText = "\u9009\u62e9\u533a\u670d";
        private const string ServerSelectCloseText = "\u5173\u95ed";

        private static readonly Color ServerBackdropFillColor = new Color32(16, 14, 12, 214);
        private static readonly Color ServerRootFillColor = new Color32(24, 23, 20, 238);
        private static readonly Color ServerScrollFillColor = new Color32(32, 30, 27, 240);
        private static readonly Color ServerButtonFillColor = new Color32(66, 62, 56, 255);
        private static readonly Color ServerButtonTextColor = new Color32(238, 230, 214, 255);
        private static readonly Color ServerZoneFillColor = new Color32(59, 49, 39, 244);
        private static readonly Color ServerZoneSelectedFillColor = new Color32(90, 69, 45, 252);
        private static readonly Color ServerZoneSpriteColor = new Color32(212, 201, 183, 236);
        private static readonly Color ServerZoneSelectedSpriteColor = new Color32(255, 255, 255, 255);
        private static readonly Color ServerZoneTextColor = new Color32(225, 213, 191, 255);
        private static readonly Color ServerZoneSelectedTextColor = new Color32(251, 231, 176, 255);
        private static readonly Color ServerZoneIndicatorColor = new Color32(236, 190, 82, 255);
        private static readonly Color ServerLineFillColor = new Color32(46, 42, 38, 248);
        private static readonly Color ServerLineTextColor = new Color32(241, 235, 224, 255);
        private static readonly Color ServerCurrentLineTextColor = new Color32(251, 239, 207, 255);
        private static readonly Color ServerDividerColor = new Color32(143, 128, 104, 180);
        private static readonly Color ServerNormalStateColor = new Color32(88, 196, 116, 255);
        private static readonly Color ServerHotStateColor = new Color32(228, 86, 64, 255);
        private static readonly Color ServerNewStateColor = new Color32(236, 190, 82, 255);
        private static readonly Color ServerSpriteVisibleColor = new Color32(255, 255, 255, 255);

        private static Font cachedServerSelectFont;

        private void RepairServerSelectVisuals()
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (!VisualRepairCompatibility.ShouldRepairSelectServer(SelectSeverPanel, ZonList, Content, openSelectSevBtn, EnterGameBtn, AgeBtn, AnnouncBtn))
            {
                return;
            }

            EnsureScale(SelectSeverPanel?.transform);
            EnsureScale(Content?.transform);
            EnsureScale(ZonList?.transform);

            RepairPanelLayout();
            RepairPanelBackgrounds();
            RepairCurrentLineDisplay();
            RepairZoneList();
            RepairLineList();
            RepairActionButtons();
            RepairAuxiliaryPanels();
        }

        private void RepairPanelLayout()
        {
            RectTransform panelRoot = SelectSeverPanel != null ? SelectSeverPanel.transform as RectTransform : null;
            if (panelRoot == null)
            {
                return;
            }

            panelRoot.anchorMin = Vector2.zero;
            panelRoot.anchorMax = Vector2.one;
            panelRoot.pivot = new Vector2(0.5f, 0.5f);
            panelRoot.anchoredPosition = Vector2.zero;
            panelRoot.sizeDelta = Vector2.zero;
            panelRoot.offsetMin = Vector2.zero;
            panelRoot.offsetMax = Vector2.zero;

            RectTransform mainPanel = ResolveComponent<RectTransform>(panelRoot, "selectSever");
            if (mainPanel != null)
            {
                mainPanel.anchorMin = new Vector2(0.5f, 0.5f);
                mainPanel.anchorMax = new Vector2(0.5f, 0.5f);
                mainPanel.pivot = new Vector2(0.5f, 0.5f);
                mainPanel.anchoredPosition = Vector2.zero;
                mainPanel.sizeDelta = new Vector2(1200f, 800f);
            }

            RectTransform zoneListRect = ZonList != null ? ZonList.transform as RectTransform : null;
            if (zoneListRect != null)
            {
                zoneListRect.anchorMin = new Vector2(0f, 0.5f);
                zoneListRect.anchorMax = new Vector2(0f, 0.5f);
                zoneListRect.pivot = new Vector2(0f, 0.5f);
                zoneListRect.anchoredPosition = Vector2.zero;
                zoneListRect.sizeDelta = new Vector2(326.5f, 800f);
            }

            RectTransform dividerRect = ResolveComponent<RectTransform>(panelRoot, "Line");
            if (dividerRect != null)
            {
                dividerRect.anchorMin = new Vector2(0.5f, 0f);
                dividerRect.anchorMax = new Vector2(0.5f, 1f);
                dividerRect.pivot = new Vector2(0.5f, 0.5f);
                dividerRect.anchoredPosition = new Vector2(-271f, 0.30648804f);
                dividerRect.sizeDelta = new Vector2(5f, -7.3823f);
            }

            RectTransform serverScrollRect = ResolveComponent<RectTransform>(panelRoot, "SeverScrollView");
            if (serverScrollRect != null)
            {
                serverScrollRect.anchorMin = new Vector2(0.5f, 0.5f);
                serverScrollRect.anchorMax = new Vector2(0.5f, 0.5f);
                serverScrollRect.pivot = new Vector2(0.5f, 0.5f);
                serverScrollRect.anchoredPosition = new Vector2(150.74634f, 0f);
                serverScrollRect.sizeDelta = new Vector2(800.0927f, 726.7614f);
            }
        }

        private void ScheduleServerSelectVisualRepairPasses()
        {
            if (!VisualRepairCompatibility.ShouldRepairSelectServer(SelectSeverPanel, ZonList, Content, openSelectSevBtn, EnterGameBtn, AgeBtn, AnnouncBtn))
            {
                LoginStageTrace.Append("RepairServerSelectVisuals skipped baseline-healthy");
                return;
            }

            ScheduleServerSelectVisualRepairPass(180, "180ms");
            ScheduleServerSelectVisualRepairPass(720, "720ms");
            ScheduleServerSelectVisualRepairPass(1500, "1500ms");
        }

        private void ScheduleServerSelectVisualRepairPass(long delay, string tag)
        {
            TimerComponent.Instance?.RegisterTimeCallBack(delay, () =>
            {
                if (this.IsDisposed)
                {
                    return;
                }

                try
                {
                    RepairServerSelectVisuals();
                    LoginStageTrace.Append($"RepairServerSelectVisuals delayed={tag}");
                }
                catch (System.Exception e)
                {
                    LoginStageTrace.Append($"RepairServerSelectVisuals failed={tag} type={e.GetType().Name} message={e.Message}");
                }
            });
        }

        private void RepairPanelBackgrounds()
        {
            Transform panelRoot = SelectSeverPanel?.transform;
            if (panelRoot == null)
            {
                return;
            }

            EnsurePanelBackdrop(panelRoot);
            RepairImagePresentation(ResolveComponent<Image>(panelRoot, "selectSever"), ServerRootFillColor);
            RepairImagePresentation(ResolveComponent<Image>(panelRoot, "SeverScrollView"), ServerScrollFillColor);
            RepairImagePresentation(ResolveComponent<Image>(panelRoot, "Line"), ServerDividerColor);

            foreach (Image image in panelRoot.GetComponentsInChildren<Image>(true))
            {
                if (!ShouldNormalizePanelImage(image))
                {
                    continue;
                }

                RepairImagePresentation(image, ServerScrollFillColor);
            }

            RepairInvisibleOverlayButton(CloseBtn);
        }

        private void RepairCurrentLineDisplay()
        {
            if (curLineTxt != null)
            {
                curLineTxt.font = ResolveServerSelectFont(SelectSeverPanel?.transform);
                curLineTxt.color = ServerCurrentLineTextColor;
                curLineTxt.fontStyle = FontStyle.Bold;
            }

            if (stateimag != null)
            {
                EnsureStateIndicator(stateimag, lastLineInfo.linestate);
            }
        }

        private void RepairZoneList()
        {
            if (ZonList == null)
            {
                return;
            }

            for (int i = 0; i < ZonList.transform.childCount; i++)
            {
                Transform zoneItem = ZonList.transform.GetChild(i);
                EnsureScale(zoneItem);

                Toggle toggle = zoneItem.GetComponent<Toggle>();
                if (toggle != null)
                {
                    RepairZoneToggle(toggle);
                }

                Text label = ResolveComponent<Text>(zoneItem, "Label");
                if (label != null)
                {
                    RepairZoneLabel(label, toggle != null && toggle.isOn, zoneItem);
                }
            }
        }

        private static void RepairZoneToggle(Toggle toggle)
        {
            if (toggle == null)
            {
                return;
            }

            toggle.transition = Selectable.Transition.None;

            Image background = toggle.targetGraphic as Image;
            RepairZoneBackgroundPresentation(background, toggle.isOn);
            if (background != null)
            {
                background.raycastTarget = true;
            }

            RepairZoneSelectionIndicator(toggle.graphic as Image, toggle.isOn);
        }

        private static void RepairZoneBackgroundPresentation(Image image, bool isSelected)
        {
            if (image == null)
            {
                return;
            }

            image.enabled = true;
            image.material = null;

            if (HasSprite(image))
            {
                image.color = isSelected ? ServerZoneSelectedSpriteColor : ServerZoneSpriteColor;
                return;
            }

            RepairSolidFillPresentation(image, isSelected ? ServerZoneSelectedFillColor : ServerZoneFillColor);
        }

        private void RepairZoneLabel(Text label, bool isSelected, Transform zoneRoot)
        {
            if (label == null)
            {
                return;
            }

            label.font = ResolveServerSelectFont(zoneRoot);
            label.color = isSelected ? ServerZoneSelectedTextColor : ServerZoneTextColor;
            label.fontStyle = FontStyle.Bold;

            Outline outline = label.GetComponent<Outline>() ?? label.gameObject.AddComponent<Outline>();
            outline.effectColor = isSelected ? new Color32(34, 27, 18, 255) : new Color32(16, 12, 9, 224);
            outline.effectDistance = new Vector2(1f, -1f);
            outline.useGraphicAlpha = true;
        }

        private static void RepairZoneSelectionIndicator(Image image, bool isSelected)
        {
            if (image == null)
            {
                return;
            }

            image.enabled = true;
            image.raycastTarget = false;
            image.sprite = null;
            image.overrideSprite = null;
            image.material = null;
            image.type = Image.Type.Simple;
            image.color = isSelected ? ServerZoneIndicatorColor : new Color(0f, 0f, 0f, 0f);

            RectTransform rect = image.rectTransform;
            if (rect == null)
            {
                return;
            }

            rect.anchorMin = new Vector2(0f, 0.16f);
            rect.anchorMax = new Vector2(0f, 0.84f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(12f, 0f);
            rect.sizeDelta = new Vector2(8f, 0f);
            rect.localScale = Vector3.one;
            rect.localRotation = Quaternion.identity;
        }

        private void RepairLineList()
        {
            if (Content == null)
            {
                return;
            }

            for (int i = 0; i < Content.transform.childCount; i++)
            {
                Transform lineItem = Content.transform.GetChild(i);
                EnsureScale(lineItem);

                Button button = lineItem.GetComponent<Button>();
                if (button != null)
                {
                    button.transition = Selectable.Transition.None;
                    RepairImagePresentation(button.targetGraphic as Image, ServerLineFillColor);
                }

                Text text = ResolveComponent<Text>(lineItem, "Text");
                if (text != null)
                {
                    text.font = ResolveServerSelectFont(lineItem);
                    text.color = ServerLineTextColor;
                    text.fontStyle = FontStyle.Normal;
                }

                Image state = ResolveComponent<Image>(lineItem, "state");
                if (state != null)
                {
                    EnsureStateIndicator(state, InferLineState(text != null ? text.text : string.Empty));
                }
            }
        }

        private void RepairActionButtons()
        {
            RepairButton(openSelectSevBtn, ServerSelectOpenText);
            RepairButton(EnterGameBtn, ServerSelectEnterGameText);
            RepairButton(collector?.GetButton("close"), ServerSelectCloseText);
            RepairButton(AgeBtn, "\u9002\u9f84\u63d0\u793a");
            RepairButton(AnnouncBtn, "\u516c\u544a");
        }

        private void RepairAuxiliaryPanels()
        {
            RepairImagePresentation(collector?.GetImage("AgePanel"), ServerRootFillColor);
            RepairImagePresentation(collector?.GetImage("AnnouncPanel"), ServerRootFillColor);
        }

        private void RepairButton(Button button, string fallbackText)
        {
            if (button == null)
            {
                return;
            }

            EnsureScale(button.transform);

            if (button.name == "closeBtn")
            {
                RepairInvisibleOverlayButton(button);
                SetCodexLabelVisible(button.transform, false);
                return;
            }

            if (ShouldPreferSolidButtonBackground(button))
            {
                RepairSolidFillPresentation(button.targetGraphic as Image, ServerButtonFillColor);
            }
            else
            {
                RepairImagePresentation(button.targetGraphic as Image, ServerButtonFillColor);
            }

            button.transition = Selectable.Transition.None;

            bool preserveLegacyLabel = HasVisibleLegacyButtonLabel(button.transform);
            bool createFallbackLabel = !preserveLegacyLabel && ShouldCreateFallbackLabel(button);
            SetCodexLabelVisible(button.transform, createFallbackLabel);

            if (createFallbackLabel)
            {
                DisableLegacyButtonBadge(button.transform);
                DisableLegacyButtonText(button.transform);

                Text label = EnsureButtonText(button.transform, fallbackText);
                if (label == null)
                {
                    return;
                }

                label.text = fallbackText;
                label.color = ServerButtonTextColor;
                label.fontStyle = FontStyle.Bold;
                return;
            }

            EnableLegacyButtonVisuals(button.transform);
        }

        private static void RepairInvisibleOverlayButton(Button button)
        {
            Image image = button != null ? button.targetGraphic as Image : null;
            if (image == null)
            {
                return;
            }

            image.enabled = true;
            image.sprite = null;
            image.overrideSprite = null;
            image.material = null;
            image.type = Image.Type.Simple;
            image.color = new Color(0f, 0f, 0f, 0.01f);
            image.raycastTarget = true;
        }

        private void EnsureStateIndicator(Image image, int lineState)
        {
            if (image == null)
            {
                return;
            }

            image.enabled = true;
            image.material = null;
            image.raycastTarget = false;

            Sprite sprite = image.sprite;
            if (sprite == null)
            {
                image.type = Image.Type.Simple;
                image.color = GetStateFallbackColor(lineState);
                return;
            }

            image.color = Color.white;
        }

        private static Color GetStateFallbackColor(int lineState)
        {
            if (lineState == (int)E_LineStateType.HOT)
            {
                return ServerHotStateColor;
            }

            if (lineState == (int)E_LineStateType.NEW)
            {
                return ServerNewStateColor;
            }

            return ServerNormalStateColor;
        }

        private static int InferLineState(string lineName)
        {
            if (string.IsNullOrEmpty(lineName))
            {
                return (int)E_LineStateType.NORMAL;
            }

            string lower = lineName.ToLowerInvariant();
            if (lower.Contains("hot") || lower.Contains("\u706b\u7206"))
            {
                return (int)E_LineStateType.HOT;
            }

            if (lower.Contains("new") || lower.Contains("\u65b0"))
            {
                return (int)E_LineStateType.NEW;
            }

            return (int)E_LineStateType.NORMAL;
        }

        private static bool ShouldNormalizePanelImage(Image image)
        {
            if (image == null)
            {
                return false;
            }

            string name = image.name;
            if (name == "closeBtn" || name == "state" || name == "stateimag" || name == "CodexBackdrop")
            {
                return false;
            }

            RectTransform rect = image.rectTransform;
            if (rect == null)
            {
                return false;
            }

            if (rect.rect.width < 260f && rect.rect.height < 120f)
            {
                return false;
            }

            Color color = image.color;
            bool bright = color.a > 0.2f && color.r > 0.85f && color.g > 0.85f && color.b > 0.85f;
            return bright || name == "Viewport" || name == "Mask";
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
                rectTransform.offsetMin = new Vector2(12f, 6f);
                rectTransform.offsetMax = new Vector2(-12f, -6f);
            }
            else
            {
                labelObject = labelRoot.gameObject;
            }

            labelObject.transform.SetAsLastSibling();

            Text text = labelObject.GetComponent<Text>();
            text.font = ResolveServerSelectFont(buttonRoot);
            text.supportRichText = false;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 16;
            text.resizeTextMaxSize = 34;
            text.fontSize = 28;
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

        private static bool ShouldPreferSolidButtonBackground(Button button)
        {
            if (button == null)
            {
                return false;
            }

            switch (button.name)
            {
                case "selectSeverBtn":
                case "EnterGameBtn":
                    return true;
            }

            return false;
        }

        private static bool ShouldCreateFallbackLabel(Button button)
        {
            if (button == null)
            {
                return false;
            }

            switch (button.name)
            {
                case "AgeBtn":
                case "AnnouncBtn":
                case "CloseBtn":
                case "close":
                case "closeBtn":
                case "Close":
                    return false;
            }

            if (button.targetGraphic is Image image && HasSprite(image))
            {
                return false;
            }

            return true;
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
                if (image == null || image.transform == buttonRoot || image.name != "Text")
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
                text.color = ServerButtonTextColor;
            }

            foreach (Image image in buttonRoot.GetComponentsInChildren<Image>(true))
            {
                if (image == null || image.transform == buttonRoot || image.name != "Text")
                {
                    continue;
                }

                image.enabled = true;
                image.raycastTarget = false;
                RepairImagePresentation(image, ServerButtonFillColor);
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

        private Font ResolveServerSelectFont(Transform root)
        {
            if (cachedServerSelectFont != null)
            {
                return cachedServerSelectFont;
            }

            Text sample = curLineTxt
                ?? ResolveComponent<Text>(root, "Label")
                ?? ResolveComponent<Text>(root, "Text");

            if (sample?.font != null)
            {
                cachedServerSelectFont = sample.font;
                return cachedServerSelectFont;
            }

            cachedServerSelectFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
            return cachedServerSelectFont;
        }

        private static void RepairSolidFillPresentation(Image image, Color fillColor)
        {
            if (image == null)
            {
                return;
            }

            image.enabled = true;
            image.sprite = null;
            image.overrideSprite = null;
            image.material = null;
            image.type = Image.Type.Simple;
            image.color = fillColor;
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

        private static bool HasSprite(Image image)
        {
            if (image == null)
            {
                return false;
            }

            Sprite sprite = image.overrideSprite != null ? image.overrideSprite : image.sprite;
            return sprite != null && !IsBuiltinPlaceholderSprite(sprite);
        }

        private static void EnsureSpriteVisible(Image image)
        {
            if (image == null)
            {
                return;
            }

            if (image.color.a < 0.2f)
            {
                image.color = ServerSpriteVisibleColor;
                return;
            }

            if (ShouldForceOpaqueSprite(image) && image.color.a < 0.92f)
            {
                image.color = ServerSpriteVisibleColor;
                return;
            }

            if (image.color.r < 0.1f && image.color.g < 0.1f && image.color.b < 0.1f)
            {
                image.color = ServerSpriteVisibleColor;
            }
        }

        private static bool ShouldForceOpaqueSprite(Image image)
        {
            if (image == null)
            {
                return false;
            }

            switch (image.name)
            {
                case "selectSever":
                case "AnnouncBtn":
                case "AgeBtn":
                    return true;
            }

            return false;
        }

        private static bool IsBuiltinPlaceholderSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                return true;
            }

            switch (sprite.name)
            {
                case "Background":
                case "UISprite":
                case "InputFieldBackground":
                case "Knob":
                case "Checkmark":
                case "DropdownArrow":
                case "UIMask":
                    return true;
            }

            return false;
        }

        private static void EnsurePanelBackdrop(Transform panelRoot)
        {
            Transform targetPanel = FindChild(panelRoot, "selectSever");
            RectTransform targetRect = targetPanel as RectTransform;
            if (targetRect == null)
            {
                return;
            }

            Transform backdropRoot = FindChild(panelRoot, "CodexBackdrop");
            GameObject backdropObject;
            if (backdropRoot == null)
            {
                backdropObject = new GameObject("CodexBackdrop", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
                backdropObject.transform.SetParent(panelRoot, false);
                backdropRoot = backdropObject.transform;
            }
            else
            {
                backdropObject = backdropRoot.gameObject;
            }

            RectTransform backdropRect = backdropObject.GetComponent<RectTransform>();
            backdropRect.anchorMin = targetRect.anchorMin;
            backdropRect.anchorMax = targetRect.anchorMax;
            backdropRect.pivot = targetRect.pivot;
            backdropRect.anchoredPosition = targetRect.anchoredPosition;
            backdropRect.sizeDelta = targetRect.sizeDelta;
            backdropRect.localScale = Vector3.one;
            backdropRect.localRotation = Quaternion.identity;

            Image backdropImage = backdropObject.GetComponent<Image>();
            backdropImage.enabled = true;
            backdropImage.sprite = null;
            backdropImage.overrideSprite = null;
            backdropImage.material = null;
            backdropImage.type = Image.Type.Simple;
            backdropImage.color = ServerBackdropFillColor;
            backdropImage.raycastTarget = false;

            int siblingIndex = 0;
            for (int i = 0; i < panelRoot.childCount; ++i)
            {
                Transform child = panelRoot.GetChild(i);
                if (child == backdropRoot)
                {
                    continue;
                }

                if (child.name == "close")
                {
                    siblingIndex = i + 1;
                    break;
                }
            }

            backdropRoot.SetSiblingIndex(Mathf.Clamp(siblingIndex, 0, panelRoot.childCount - 1));
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
