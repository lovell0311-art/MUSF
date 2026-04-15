using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ETHotfix
{

    /// <summary>
    /// 澶у湴鍥?
    /// </summary>
    public partial class UIMainComponent
    {
        public RectTransform LimitContainer;//闄愬埗鍖哄煙
        public Canvas canvas;

        bool isOpenBigMap = false;

        public MiniMap bigMap;
        /// <summary>鏈湴瑙掕壊 澶у湴鍥綢con瀵硅薄</summary>
        private MiniMap.IconRole bigMap_RoleIcon;

        private MiniMap.IconTargetPoint bigMap_TargetPointIcon;

        private LinkedList<MiniMap.IconNavPoint> bigMap_NavIconList = new LinkedList<MiniMap.IconNavPoint>();
        /// <summary>鐢ㄦ潵鎭㈠Icon鏁版嵁鐨?/summary>
        private LinkedList<Vector2> bigMap_NavList = new LinkedList<Vector2>();
        private const float DefaultBigMapZoomScale = 3f;
        private const float DefaultBigMapMinZoomScale = 0.25f;
        private const float AbsoluteBigMapMinZoomScale = 0.05f;
        private const float DefaultBigMapMaxZoomScale = 10f;
        private const float AdaptiveBigMapScaleOffsetThreshold = 2.4f;
        private const float AdaptiveBigMapOpenZoomMultiplier = 2f;
        private const float AdaptiveLargeMapTargetFinalMinScale = 0.24f;

        private bool TryGetAdaptiveBigMapCalibrationScale(int sceneId, out float calibrationScale, out float configuredScale)
        {
            calibrationScale = 1f;
            configuredScale = 1f;

            Map_InfoConfig mapInfo = ConfigComponent.Instance.GetItem<Map_InfoConfig>(sceneId);
            if (mapInfo == null)
            {
                return false;
            }

            if (mapInfo.ScaleOffset <= 0f || float.IsNaN(mapInfo.ScaleOffset) || float.IsInfinity(mapInfo.ScaleOffset))
            {
                return false;
            }

            configuredScale = mapInfo.ScaleOffset;
            if (configuredScale < AdaptiveBigMapScaleOffsetThreshold)
            {
                return false;
            }

            calibrationScale = configuredScale;
            float legacyBundleFactor = GetLegacyMinimapBundleFactor();
            if (legacyBundleFactor > 1.001f)
            {
                calibrationScale *= legacyBundleFactor;
            }

            return true;
        }

        private float GetBigMapMinZoomScale(int sceneId)
        {
            if (TryGetAdaptiveBigMapCalibrationScale(sceneId, out float calibrationScale, out _))
            {
                float adaptiveMinZoomScale = AdaptiveLargeMapTargetFinalMinScale / Mathf.Max(0.001f, calibrationScale);
                return Mathf.Clamp(adaptiveMinZoomScale, AbsoluteBigMapMinZoomScale, DefaultBigMapMinZoomScale);
            }

            return DefaultBigMapMinZoomScale;
        }

        private float GetBigMapOpenZoomScale(int sceneId)
        {
            if (TryGetAdaptiveBigMapCalibrationScale(sceneId, out _, out _))
            {
                float minZoomScale = GetBigMapMinZoomScale(sceneId);
                return Mathf.Clamp(minZoomScale * AdaptiveBigMapOpenZoomMultiplier, minZoomScale, DefaultBigMapZoomScale);
            }

            return DefaultBigMapZoomScale;
        }

        private bool ShouldOpenBigMapFitWholeMap(int sceneId)
        {
            return false;
        }

        private Vector2 GetBigMapOpenCenterPosition(int sceneId)
        {
            if (ShouldOpenBigMapFitWholeMap(sceneId))
            {
                AstarComponent astar = AstarComponent.Instance;
                if (astar != null && astar.Width > 0 && astar.Height > 0)
                {
                    return new Vector2((astar.Width - 1) * 0.5f, (astar.Height - 1) * 0.5f);
                }
            }

            return bigMap_RoleIcon != null ? bigMap_RoleIcon.Position : Vector2.zero;
        }

        private void EnsureBigMapRootLayout()
        {
            if (this.BigMap == null)
            {
                return;
            }

            RectTransform rectTransform = this.BigMap.transform as RectTransform;
            if (rectTransform != null)
            {
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.pivot = new Vector2(0.5f, 0.5f);
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.sizeDelta = Vector2.zero;
                rectTransform.localScale = Vector3.one;
                rectTransform.localRotation = Quaternion.identity;
            }

            this.BigMap.transform.SetAsLastSibling();
        }

        //鏄剧ず澶у湴鍥?
        public void ShowBigMap()
        {
            BigMap.SetActive(true);
            EnsureBigMapRootLayout();
            isOpenBigMap = true;

            if (bigMap == null)
            {
                bigMap = new MiniMap();
            }
            bigMap.Init(BigMap.transform);
            bigMap.SetViewportCenterOffset(Vector2.zero);
            bigMap.SetTrackCenterOffset(Vector2.zero);
            bigMap.CanDrag = true;
            bigMap.CanZoom = true;
            bigMap.MinZoomScale = GetBigMapMinZoomScale(curSceneId);
            bigMap.MaxZoomScale = DefaultBigMapMaxZoomScale;
            bigMap.MiniMap_Icon = miniMap.MiniMap_Icon;
            bigMap.OffsetTranform.SetSiblingIndex(0);

            SwitchMiniMap(bigMap, curSceneId);
            LoadTreasurePoint(bigMap, curSceneId);

            float openZoomScale = GetBigMapOpenZoomScale(curSceneId);
            bigMap.MapScale = new Vector3(openZoomScale, openZoomScale, openZoomScale);
            LoginStageTrace.Append(
                $"BigMap open sceneId={curSceneId} openZoom={openZoomScale:0.###} minZoom={bigMap.MinZoomScale:0.###} maxZoom={bigMap.MaxZoomScale:0.###}");
            if (TryGetAdaptiveBigMapCalibrationScale(curSceneId, out float adaptiveScale, out float configuredScale))
            {
                LoginStageTrace.Append(
                    $"BigMap adaptive sceneId={curSceneId} scaleConfigured={configuredScale:0.###} " +
                    $"scaleEffective={adaptiveScale:0.###} openZoom={openZoomScale:0.###} minZoom={bigMap.MinZoomScale:0.###}");
            }

            bigMap_RoleIcon = bigMap.Create<MiniMap.IconRole>("role", 5);
            // 璁剧疆瑙掕壊浣嶇疆銆佹棆杞柟鍚?
            bigMap_RoleIcon.Position = miniMap_RoleIcon.Position;
            bigMap_RoleIcon.Rotation = miniMap_RoleIcon.Rotation;


            bigMap.Update();

            bool fitWholeMap = ShouldOpenBigMapFitWholeMap(curSceneId);
            if (!fitWholeMap)
            {
                // 璺熻釜Icon
                bigMap.TrackIcon = bigMap_RoleIcon;
                bigMap.SetCenterPosition(bigMap_RoleIcon.Position);
            }
            else
            {
                bigMap.TrackIcon = null;
                Vector2 centerPosition = GetBigMapOpenCenterPosition(curSceneId);
                bigMap.SetCenterPosition(centerPosition);
                LoginStageTrace.Append(
                    $"BigMap ancient fit-wholemap sceneId={curSceneId} center={centerPosition.x:0.0},{centerPosition.y:0.0} " +
                    $"grid={AstarComponent.Instance?.Width ?? 0}x{AstarComponent.Instance?.Height ?? 0} zoom={openZoomScale:0.###}");
            }

            {
                // 鎭㈠瀵昏矾Icon
                if (bigMap_NavList.Count != 0)
                {
                    bigMap_TargetPointIcon = bigMap.Create<MiniMap.IconTargetPoint>("target_point", 1);
                    bigMap_TargetPointIcon.Position = bigMap_NavList.Last();
                    foreach (Vector2 navPos in bigMap_NavList)
                    {
                        MiniMap.IconNavPoint navIcon = bigMap.Create<MiniMap.IconNavPoint>("nav_point", 1);
                        navIcon.Position = navPos;
                        bigMap_NavIconList.AddLast(navIcon);
                    }
                }

            }

            unitEntityPathComponent ??= this.roleEntity.GetComponent<UnitEntityPathComponent>();
            bigMap.OnClickEvent = pos =>
            {
                var navtarget = AstarComponent.Instance.GetNodeVector(pos.x, pos.y);
                Log.DebugBrown("鐐瑰嚮涓嬪湴鍥剧殑淇℃伅" + pos.x + ":::" + pos.y);
                if (navtarget == null) return;
                if (navtarget.isWalkable == false)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "鐩爣鐐逛负涓嶅彲琛岃蛋鍖哄煙");
                }
                else
                {
                    this.navtarget = navtarget;
                    unitEntityPathComponent.NavTarget(navtarget, ClearBigMapNavigationIcons);
                    QueueBigMapNavigationPreview(navtarget);
                }
            };

        }

        private void QueueBigMapNavigationPreview(AstarNode target)
        {
            if (target == null)
            {
                return;
            }

            // Let the actual navigation request enter the A* worker before asking for the preview path.
            TimerComponent.Instance.RegisterTimeCallBack(100, () =>
            {
                if (!this.isOpenBigMap || this.bigMap == null || this.navtarget == null)
                {
                    return;
                }

                if (this.navtarget.x != target.x || this.navtarget.z != target.z)
                {
                    return;
                }

                if (this.roleEntity?.CurrentNodePos == null)
                {
                    return;
                }

                AstarComponent.Instance.FindPath(this.roleEntity.CurrentNodePos, target, UpdateBigMapNavigationIcons);
            });
        }

        public void HideBigMap()
        {
            this.BigMap.SetActive(false);
            isOpenBigMap = false;
            bigMap_RoleIcon = null;
            bigMap.Clear();
            bigMap_NavIconList.Clear();
            // 涓嬫鎵撳紑鍦板浘鏄剧ず
            //bigMap_NavList.Clear();
            bigMap_TargetPointIcon = null;
        }
    }
}
