using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    internal static class VisualRepairCompatibility
    {
        // The published Android client now locks all core UI bundles to the
        // LieFeng baseline. Runtime visual "repair" heuristics must stay out
        // of the way, otherwise they restyle valid legacy assets and drift the
        // layout again on login/select-role/main UI.
        private const bool PreserveLieFengUiBaseline = true;

        private static readonly string[] BuiltinPlaceholderSpriteNames =
        {
            "Background",
            "UISprite",
            "InputFieldBackground",
            "Knob",
            "Checkmark",
            "DropdownArrow",
            "UIMask"
        };

        public static bool ShouldRepairLogin(
            GameObject uiRoot,
            GameObject loginPanel,
            InputField accountField,
            InputField passwordField,
            params Button[] buttons)
        {
            if (PreserveLieFengUiBaseline)
            {
                return false;
            }

            if (HasZeroScale(uiRoot?.transform) || HasZeroScale(loginPanel?.transform))
            {
                return true;
            }

            Image loginBg = FindDescendantImage(uiRoot?.transform, "LoginBg");
            if (NeedsCriticalImageRepair(loginBg))
            {
                return true;
            }

            if (NeedsInputFieldRepair(accountField) || NeedsInputFieldRepair(passwordField))
            {
                return true;
            }

            return AnyButtonNeedsRepair(buttons);
        }

        public static bool ShouldRepairSelectServer(
            GameObject panelRoot,
            GameObject zoneList,
            GameObject lineContent,
            params Button[] buttons)
        {
            if (PreserveLieFengUiBaseline)
            {
                return false;
            }

            if (HasZeroScale(panelRoot?.transform) || HasZeroScale(zoneList?.transform) || HasZeroScale(lineContent?.transform))
            {
                return true;
            }

            Transform root = panelRoot?.transform;
            if (NeedsCriticalImageRepair(FindDescendantImage(root, "selectSever")) ||
                NeedsCriticalImageRepair(FindDescendantImage(root, "SeverScrollView")))
            {
                return true;
            }

            if (HasLargeBrokenWhiteGraphic(root))
            {
                return true;
            }

            return AnyButtonNeedsRepair(buttons);
        }

        public static bool ShouldRepairChooseRole(params Button[] buttons)
        {
            if (PreserveLieFengUiBaseline)
            {
                return false;
            }

            return AnyButtonNeedsRepair(buttons);
        }

        public static bool ShouldRepairMain(Transform root)
        {
            if (PreserveLieFengUiBaseline)
            {
                return false;
            }

            if (root == null)
            {
                return false;
            }

            foreach (Image image in root.GetComponentsInChildren<Image>(true))
            {
                if (NeedsBrokenMainImageRepair(image))
                {
                    return true;
                }
            }

            foreach (RawImage image in root.GetComponentsInChildren<RawImage>(true))
            {
                if (NeedsBrokenMainRawImageRepair(image))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool AnyButtonNeedsRepair(Button[] buttons)
        {
            if (buttons == null)
            {
                return false;
            }

            foreach (Button button in buttons)
            {
                if (NeedsButtonRepair(button))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool NeedsInputFieldRepair(InputField field)
        {
            if (field == null)
            {
                return false;
            }

            if (HasZeroScale(field.transform))
            {
                return true;
            }

            Image image = field.targetGraphic as Image;
            if (NeedsCriticalImageRepair(image))
            {
                return true;
            }

            if (field.textComponent == null || field.placeholder == null)
            {
                return true;
            }

            return false;
        }

        private static bool NeedsButtonRepair(Button button)
        {
            if (button == null)
            {
                return false;
            }

            if (HasZeroScale(button.transform))
            {
                return true;
            }

            if (HasUsableButtonVisual(button))
            {
                return false;
            }

            return true;
        }

        private static bool HasUsableButtonVisual(Button button)
        {
            if (button == null)
            {
                return false;
            }

            if (HasUsableImage(button.targetGraphic as Image))
            {
                return true;
            }

            foreach (Image image in button.GetComponentsInChildren<Image>(true))
            {
                if (image == null || image.transform == button.transform)
                {
                    continue;
                }

                if (!image.enabled || image.color.a <= 0.05f)
                {
                    continue;
                }

                if (HasUsableImage(image))
                {
                    return true;
                }
            }

            foreach (Text text in button.GetComponentsInChildren<Text>(true))
            {
                if (text == null || text.name == "CodexLabel" || !text.enabled)
                {
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(text.text))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool NeedsCriticalImageRepair(Image image)
        {
            if (image == null)
            {
                return false;
            }

            if (HasZeroScale(image.transform))
            {
                return true;
            }

            if (!HasUsableImage(image))
            {
                return true;
            }

            if (image.color.a < 0.05f)
            {
                return true;
            }

            return false;
        }

        private static bool HasUsableImage(Image image)
        {
            if (image == null || !image.enabled)
            {
                return false;
            }

            Sprite sprite = image.overrideSprite != null ? image.overrideSprite : image.sprite;
            return sprite != null && !IsBuiltinPlaceholderSprite(sprite);
        }

        private static bool IsBuiltinPlaceholderSprite(Sprite sprite)
        {
            if (sprite == null)
            {
                return true;
            }

            string spriteName = sprite.name;
            for (int i = 0; i < BuiltinPlaceholderSpriteNames.Length; ++i)
            {
                if (spriteName == BuiltinPlaceholderSpriteNames[i])
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasZeroScale(Transform root)
        {
            if (root == null)
            {
                return false;
            }

            Vector3 scale = root.localScale;
            return Mathf.Approximately(scale.x, 0f) ||
                   Mathf.Approximately(scale.y, 0f) ||
                   Mathf.Approximately(scale.z, 0f);
        }

        private static Image FindDescendantImage(Transform root, string name)
        {
            Transform target = FindDescendant(root, name);
            return target != null ? target.GetComponent<Image>() : null;
        }

        private static Transform FindDescendant(Transform root, string name)
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
                Transform found = FindDescendant(child, name);
                if (found != null)
                {
                    return found;
                }
            }

            return null;
        }

        private static bool HasLargeBrokenWhiteGraphic(Transform root)
        {
            if (root == null)
            {
                return false;
            }

            foreach (Image image in root.GetComponentsInChildren<Image>(true))
            {
                if (image == null || !image.enabled)
                {
                    continue;
                }

                RectTransform rect = image.rectTransform;
                if (rect == null || rect.rect.width < 220f || rect.rect.height < 120f)
                {
                    continue;
                }

                if (HasUsableImage(image))
                {
                    continue;
                }

                Color color = image.color;
                if (color.a >= 0.3f && color.r >= 0.92f && color.g >= 0.92f && color.b >= 0.92f)
                {
                    return true;
                }
            }

            return false;
        }

        private static bool NeedsBrokenMainImageRepair(Image image)
        {
            if (image == null || !image.gameObject.activeInHierarchy || !image.enabled)
            {
                return false;
            }

            if (image.sprite != null || image.overrideSprite != null)
            {
                return false;
            }

            return IsBrokenWhiteGraphic(image.color);
        }

        private static bool NeedsBrokenMainRawImageRepair(RawImage image)
        {
            if (image == null || !image.gameObject.activeInHierarchy || !image.enabled)
            {
                return false;
            }

            if (image.texture != null)
            {
                return false;
            }

            return IsBrokenWhiteGraphic(image.color);
        }

        private static bool IsBrokenWhiteGraphic(Color color)
        {
            return color.a >= 0.35f &&
                   color.r >= 0.92f &&
                   color.g >= 0.92f &&
                   color.b >= 0.92f;
        }
    }
}
