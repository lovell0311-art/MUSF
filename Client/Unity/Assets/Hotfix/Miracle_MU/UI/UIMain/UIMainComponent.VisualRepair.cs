using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMainComponent
    {
        private static readonly Color MainBrokenGraphicTint = new Color(1f, 1f, 1f, 0.02f);

        public void ScheduleMainVisualRepairPasses()
        {
            if (!VisualRepairCompatibility.ShouldRepairMain(ReferenceCollector_Main != null ? ReferenceCollector_Main.transform : null))
            {
                LoginStageTrace.Append("RepairMainVisualWhiteBlocks skipped baseline-healthy");
                return;
            }

            ScheduleMainVisualRepairPass(180, "180ms");
            ScheduleMainVisualRepairPass(720, "720ms");
            ScheduleMainVisualRepairPass(1800, "1800ms");
        }

        private void ScheduleMainVisualRepairPass(long delay, string tag)
        {
            TimerComponent.Instance?.RegisterTimeCallBack(delay, () =>
            {
                if (this.IsDisposed)
                {
                    return;
                }

                try
                {
                    RepairMainVisualWhiteBlocks();
                    LoginStageTrace.Append($"RepairMainVisualWhiteBlocks delayed={tag}");
                }
                catch (System.Exception e)
                {
                    LoginStageTrace.Append($"RepairMainVisualWhiteBlocks failed={tag} type={e.GetType().Name} message={e.Message}");
                }
            });
        }

        private void RepairMainVisualWhiteBlocks()
        {
            Transform root = ReferenceCollector_Main != null ? ReferenceCollector_Main.transform : null;
            if (root == null || !VisualRepairCompatibility.ShouldRepairMain(root))
            {
                return;
            }

            int repairedImages = 0;
            int repairedRawImages = 0;

            foreach (Image image in root.GetComponentsInChildren<Image>(true))
            {
                if (TryNeutralizeBrokenMainImage(image))
                {
                    repairedImages++;
                }
            }

            foreach (RawImage image in root.GetComponentsInChildren<RawImage>(true))
            {
                if (TryNeutralizeBrokenMainRawImage(image))
                {
                    repairedRawImages++;
                }
            }

            if (repairedImages > 0 || repairedRawImages > 0)
            {
                LoginStageTrace.Append($"RepairMainVisualWhiteBlocks repaired image={repairedImages} raw={repairedRawImages}");
            }
        }

        private static bool TryNeutralizeBrokenMainImage(Image image)
        {
            if (!ShouldNeutralizeBrokenMainImage(image, out bool keepRaycast))
            {
                return false;
            }

            image.enabled = true;
            image.material = null;
            image.sprite = null;
            image.overrideSprite = null;
            image.type = Image.Type.Simple;
            image.color = MainBrokenGraphicTint;
            image.raycastTarget = keepRaycast;
            return true;
        }

        private static bool TryNeutralizeBrokenMainRawImage(RawImage image)
        {
            if (!ShouldNeutralizeBrokenMainRawImage(image, out bool keepRaycast))
            {
                return false;
            }

            image.enabled = true;
            image.material = null;
            image.texture = null;
            image.color = MainBrokenGraphicTint;
            image.raycastTarget = keepRaycast;
            return true;
        }

        private static bool ShouldNeutralizeBrokenMainImage(Image image, out bool keepRaycast)
        {
            keepRaycast = false;
            if (image == null || !image.gameObject.activeInHierarchy || !image.enabled)
            {
                return false;
            }

            if (image.sprite != null || image.overrideSprite != null)
            {
                return false;
            }

            if (!IsBrokenWhiteGraphic(image.color))
            {
                return false;
            }

            keepRaycast = HasInteractiveOwner(image.transform);
            return true;
        }

        private static bool ShouldNeutralizeBrokenMainRawImage(RawImage image, out bool keepRaycast)
        {
            keepRaycast = false;
            if (image == null || !image.gameObject.activeInHierarchy || !image.enabled)
            {
                return false;
            }

            if (image.texture != null)
            {
                return false;
            }

            if (!IsBrokenWhiteGraphic(image.color))
            {
                return false;
            }

            keepRaycast = HasInteractiveOwner(image.transform);
            return true;
        }

        private static bool HasInteractiveOwner(Transform transform)
        {
            Transform current = transform;
            while (current != null)
            {
                if (current.GetComponent<Selectable>() != null || current.GetComponent<ScrollRect>() != null)
                {
                    return true;
                }

                current = current.parent;
            }

            return false;
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
