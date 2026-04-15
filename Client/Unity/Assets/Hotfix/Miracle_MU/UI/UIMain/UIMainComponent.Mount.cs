using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;
using Newtonsoft.Json;
using System;

namespace ETHotfix
{
    /// <summary>
    /// 坐骑 模块
    /// </summary>
    public partial class UIMainComponent
    {
        public Toggle mount_Tog;
        private RectTransform mountToggleRect;
        private bool mountToggleFallbackMouseDown;
        private bool mountToggleFallbackTouchDown;
        private int mountToggleActiveFingerId = -1;
        private Vector2 mountTogglePressScreenPosition;
        private float lastMountToggleActionTime = -10f;
        private Button runtimePetMenuButton;
        private Button runtimeMountMenuButton;
        private const float MountToggleActionDebounceSeconds = 0.25f;
        private const float MountToggleClickMoveThreshold = 35f;

        /// <summary>
        /// 当前坐骑的UUID
        /// </summary>
        public long curMountUUID = -1;
        public long GetEquippedMountId()
        {
            RoleEquipmentComponent equipmentComponent = roleEntity?.GetComponent<RoleEquipmentComponent>();
            if (equipmentComponent != null &&
                equipmentComponent.curWareEquipsData_Dic.TryGetValue(E_Grid_Type.Mounts, out KnapsackDataItem currentMount))
            {
                return currentMount.UUID;
            }

            return 0;
        }
        public void Init_Mount()
        {
            mount_Tog = ReferenceCollector_Main.GetToggle("Mount_Tog");
            mountToggleRect = mount_Tog != null ? mount_Tog.transform as RectTransform : null;
            mount_Tog.onValueChanged.AddSingleListener(UserMount);
        }

        public void EnsureBottomButtonListCanScroll()
        {
            if (BtnList == null)
            {
                return;
            }

            BtnList.horizontal = true;
            BtnList.vertical = false;
            BtnList.inertia = true;
            if (BtnList.scrollSensitivity < 5)
            {
                BtnList.scrollSensitivity = 5;
            }

            EnsureBottomMenuFallbackButtons();
            RefreshBottomButtonListWidth();
        }

        private void EnsureBottomMenuFallbackButtons()
        {
            RectTransform content = BtnList != null ? BtnList.content : null;
            if (content == null)
            {
                return;
            }

            if (runtimePetMenuButton == null && !HasBottomMenuButtonInScroll("PetBtn"))
            {
                runtimePetMenuButton = CreateBottomMenuFallbackButton("PetRuntimeBtn", "\u5BA0\u7269", () =>
                {
                    UIComponent.Instance.VisibleUI(UIType.UIPet);
                });
            }

            if (runtimeMountMenuButton == null && !HasBottomMenuButtonInScroll("MountBtn"))
            {
                runtimeMountMenuButton = CreateBottomMenuFallbackButton("MountRuntimeBtn", "\u5750\u9A91", () =>
                {
                    UIComponent.Instance.VisibleUI(UIType.UIMount);
                });
            }
        }

        private bool HasBottomMenuButtonInScroll(string key)
        {
            if (BtnList == null || BtnList.content == null)
            {
                return false;
            }

            Button button = referenceCollector_BottomCenter != null ? referenceCollector_BottomCenter.GetButton(key) : null;
            if (button == null)
            {
                return false;
            }

            return button.transform.IsChildOf(BtnList.content);
        }

        private Button CreateBottomMenuFallbackButton(string runtimeName, string label, Action onClick)
        {
            RectTransform content = BtnList != null ? BtnList.content : null;
            if (content == null)
            {
                return null;
            }

            Button template = SafeGetBottomButton("SetBtn")
                              ?? SafeGetBottomButton("E_MailBtn")
                              ?? SafeGetBottomButton("MasterBtn")
                              ?? SafeGetBottomButton("AttributeBtn")
                              ?? SafeGetBottomButton("FriendBtn")
                              ?? SafeGetBottomButton("skillBtn")
                              ?? SafeGetBottomButton("knapsackBtn");
            if (template == null)
            {
                return null;
            }

            GameObject clone = UnityEngine.Object.Instantiate(template.gameObject, content);
            clone.name = runtimeName;
            clone.transform.localScale = Vector3.one;
            RectTransform cloneRect = clone.transform as RectTransform;
            if (cloneRect != null)
            {
                cloneRect.anchoredPosition3D = Vector3.zero;
                cloneRect.localRotation = Quaternion.identity;
            }

            Button cloneButton = clone.GetComponent<Button>();
            if (cloneButton == null)
            {
                return null;
            }

            cloneButton.onClick.RemoveAllListeners();
            cloneButton.onClick.AddSingleListener(() => onClick?.Invoke());
            ResetBottomMenuButtonBadges(clone);
            SetBottomMenuButtonLabel(clone, label);

            return cloneButton;
        }

        private static void ResetBottomMenuButtonBadges(GameObject buttonRoot)
        {
            if (buttonRoot == null)
            {
                return;
            }

            foreach (Image image in buttonRoot.GetComponentsInChildren<Image>(true))
            {
                if (image == null || image.transform == buttonRoot.transform)
                {
                    continue;
                }

                string childName = image.gameObject.name;
                if (childName.IndexOf("Red", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    childName.IndexOf("count", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    childName.IndexOf("highlight", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    image.gameObject.SetActive(false);
                }
            }
        }

        private static void SetBottomMenuButtonLabel(GameObject buttonRoot, string label)
        {
            if (buttonRoot == null)
            {
                return;
            }

            Text target = null;
            foreach (Text text in buttonRoot.GetComponentsInChildren<Text>(true))
            {
                if (text == null)
                {
                    continue;
                }

                string currentText = text.text != null ? text.text.Trim() : string.Empty;
                if (!string.IsNullOrEmpty(currentText))
                {
                    target = text;
                    break;
                }

                if (target == null)
                {
                    target = text;
                }
            }

            if (target != null)
            {
                target.text = label;
            }
        }

        private void RefreshBottomButtonListWidth()
        {
            RectTransform content = BtnList != null ? BtnList.content : null;
            if (content == null)
            {
                return;
            }

            HorizontalLayoutGroup layoutGroup = content.GetComponent<HorizontalLayoutGroup>();
            float width = 0f;

            if (layoutGroup != null)
            {
                width += layoutGroup.padding.left + layoutGroup.padding.right;
            }

            int visibleChildCount = 0;
            for (int i = 0; i < content.childCount; ++i)
            {
                RectTransform child = content.GetChild(i) as RectTransform;
                if (child == null || !child.gameObject.activeSelf)
                {
                    continue;
                }

                float childWidth = LayoutUtility.GetPreferredWidth(child);
                if (childWidth <= 0f)
                {
                    childWidth = child.rect.width;
                }
                if (childWidth <= 0f)
                {
                    childWidth = child.sizeDelta.x;
                }

                width += childWidth;
                visibleChildCount++;
            }

            if (layoutGroup != null && visibleChildCount > 1)
            {
                width += layoutGroup.spacing * (visibleChildCount - 1);
            }

            if (BtnList.viewport != null)
            {
                width = Mathf.Max(width, BtnList.viewport.rect.width);
            }

            if (width <= 0f)
            {
                return;
            }

            content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
        }

        public void UpdateMountShortcutFallback()
        {
            if (mount_Tog == null || mountToggleRect == null)
            {
                return;
            }

            if (!mount_Tog.gameObject.activeInHierarchy || !mount_Tog.interactable)
            {
                mountToggleFallbackMouseDown = false;
                mountToggleFallbackTouchDown = false;
                mountToggleActiveFingerId = -1;
                return;
            }

            HandleMountToggleMouseFallback();
            HandleMountToggleTouchFallback();
        }

        private void HandleMountToggleMouseFallback()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 screenPoint = Input.mousePosition;
                if (IsMountToggleScreenPoint(screenPoint))
                {
                    mountToggleFallbackMouseDown = true;
                    mountTogglePressScreenPosition = screenPoint;
                }
            }

            if (!Input.GetMouseButtonUp(0))
            {
                return;
            }

            Vector2 releasePoint = Input.mousePosition;
            bool shouldTrigger = mountToggleFallbackMouseDown && IsMountToggleScreenPoint(releasePoint);
            mountToggleFallbackMouseDown = false;
            if (shouldTrigger)
            {
                TryTriggerMountShortcutFallback(releasePoint);
            }
        }

        private void HandleMountToggleTouchFallback()
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                Vector2 screenPoint = touch.position;
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        if (IsMountToggleScreenPoint(screenPoint))
                        {
                            mountToggleFallbackTouchDown = true;
                            mountToggleActiveFingerId = touch.fingerId;
                            mountTogglePressScreenPosition = screenPoint;
                        }
                        break;
                    case TouchPhase.Canceled:
                        if (touch.fingerId == mountToggleActiveFingerId)
                        {
                            mountToggleFallbackTouchDown = false;
                            mountToggleActiveFingerId = -1;
                        }
                        break;
                    case TouchPhase.Ended:
                        if (touch.fingerId != mountToggleActiveFingerId)
                        {
                            break;
                        }

                        bool shouldTrigger = mountToggleFallbackTouchDown && IsMountToggleScreenPoint(screenPoint);
                        mountToggleFallbackTouchDown = false;
                        mountToggleActiveFingerId = -1;
                        if (shouldTrigger)
                        {
                            TryTriggerMountShortcutFallback(screenPoint);
                        }
                        break;
                }
            }
        }

        private bool IsMountToggleScreenPoint(Vector2 screenPoint)
        {
            return mountToggleRect != null &&
                   RectTransformUtility.RectangleContainsScreenPoint(mountToggleRect, screenPoint, null);
        }

        private void TryTriggerMountShortcutFallback(Vector2 releasePoint)
        {
            if ((releasePoint - mountTogglePressScreenPosition).sqrMagnitude >
                MountToggleClickMoveThreshold * MountToggleClickMoveThreshold)
            {
                return;
            }

            if (Time.unscaledTime - lastMountToggleActionTime < MountToggleActionDebounceSeconds)
            {
                return;
            }

            bool nextState = !mount_Tog.isOn;
            mount_Tog.SetIsOnWithoutNotify(nextState);
            UserMount(nextState);
        }

        /// <summary>
        /// 使用坐骑
        /// </summary>
        /// <param name="isOn"></param>
        public void UserMount(bool isOn)
        {
            if (Time.unscaledTime - lastMountToggleActionTime < MountToggleActionDebounceSeconds)
            {
                return;
            }

            lastMountToggleActionTime = Time.unscaledTime;
            ToggleMount().Coroutine();

            async ETVoid ToggleMount()
            {
                long mountId = curMountUUID;
                long mountConfigId = 0;
                bool isBackpackMount = false;

                if (mountId != -1 && KnapsackItemsManager.KnapsackItems.TryGetValue(mountId, out KnapsackDataItem backpackMount))
                {
                    isBackpackMount = true;
                    mountConfigId = backpackMount.ConfigId;
                }

                if (mountId == -1)
                {
                    if (KnapsackItemsManager.MountUUIDList.Count > 0)
                    {
                        mountId = KnapsackItemsManager.MountUUIDList.First();
                        curMountUUID = mountId;
                        if (KnapsackItemsManager.KnapsackItems.TryGetValue(mountId, out backpackMount))
                        {
                            isBackpackMount = true;
                            mountConfigId = backpackMount.ConfigId;
                        }
                    }
                    else
                    {
                        G2C_OpenTheMountPanelResponse openMountResponse =
                            (G2C_OpenTheMountPanelResponse)await SessionComponent.Instance.Session.Call(
                                new C2G_OpenTheMountPanelRequest() { });
                        if (openMountResponse.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, openMountResponse.Error.GetTipInfo());
                            return;
                        }
                        if (string.IsNullOrEmpty(openMountResponse.MountInfo))
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "没有坐骑可使用");
                            return;
                        }

                        Dictionary<string, int> mountDict =
                            JsonConvert.DeserializeObject<Dictionary<string, int>>(openMountResponse.MountInfo);
                        if (mountDict == null || mountDict.Count == 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "没有坐骑可使用");
                            return;
                        }

                        KeyValuePair<string, int> firstMount = mountDict.First();
                        curMountUUID = long.Parse(firstMount.Key);

                        if (roleEntity.IsSafetyZone && firstMount.Value != 260020)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, "安全区无法使用坐骑");
                            return;
                        }

                        await ToggleMountUseState(curMountUUID);
                        return;
                    }
                }

                if (roleEntity.IsSafetyZone && mountConfigId != 260020)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "安全区无法使用坐骑");
                    return;
                }

                if (isBackpackMount)
                {
                    G2C_PlayerUseItemInTheBackpack useItemResponse =
                        (G2C_PlayerUseItemInTheBackpack)await SessionComponent.Instance.Session.Call(
                            new C2G_PlayerUseItemInTheBackpack { ItemUUID = mountId });
                    if (useItemResponse.Error != 0)
                    {
                        Log.DebugRed($"{useItemResponse.Error}");
                        UIComponent.Instance.VisibleUI(UIType.UIHint, useItemResponse.Error.GetTipInfo());
                    }
                    else
                    {
                        TryUseActivatedMount(mountId);
                    }
                    return;
                }

                await ToggleMountUseState(mountId);
            }

            async ETTask ToggleMountUseState(long mountId)
            {
                G2C_GetMountInfoResponse mountInfoResponse =
                    (G2C_GetMountInfoResponse)await SessionComponent.Instance.Session.Call(
                        new C2G_GetMountInfoRequest() { MountID = mountId });
                if (mountInfoResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, mountInfoResponse.Error.GetTipInfo());
                    return;
                }

                if (roleEntity.IsSafetyZone && mountInfoResponse.ConfigId != 260020)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "安全区无法使用坐骑");
                    return;
                }

                if (mountInfoResponse.IsUsing == 0)
                {
                    long equippedMountId = GetEquippedMountId();
                    if (equippedMountId > 0 && equippedMountId != mountId)
                    {
                        G2C_RecallMountResponse recallMountResponse =
                            (G2C_RecallMountResponse)await SessionComponent.Instance.Session.Call(
                                new C2G_RecallMountRequest() { MountID = equippedMountId });
                        if (recallMountResponse.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, recallMountResponse.Error.GetTipInfo());
                            return;
                        }
                    }

                    G2C_UseMountResponse useMountResponse =
                        (G2C_UseMountResponse)await SessionComponent.Instance.Session.Call(
                            new C2G_UseMountRequest() { MountID = mountId });
                    if (useMountResponse.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, useMountResponse.Error.GetTipInfo());
                    }
                }
                else
                {
                    G2C_RecallMountResponse recallMountResponse =
                        (G2C_RecallMountResponse)await SessionComponent.Instance.Session.Call(
                            new C2G_RecallMountRequest() { MountID = mountId });
                    if (recallMountResponse.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, recallMountResponse.Error.GetTipInfo());
                    }
                }
            }
        }

        public void TryUseActivatedMount(long preferredMountId)
        {
            AutoUseActivatedMount().Coroutine();

            async ETVoid AutoUseActivatedMount()
            {
                if (preferredMountId <= 0)
                {
                    return;
                }

                await TimerComponent.Instance.WaitAsync(100);

                long mountId = preferredMountId;
                G2C_GetMountInfoResponse mountInfoResponse = null;
                for (int i = 0; i < 2; i++)
                {
                    mountInfoResponse =
                        (G2C_GetMountInfoResponse)await SessionComponent.Instance.Session.Call(
                            new C2G_GetMountInfoRequest() { MountID = mountId });
                    if (mountInfoResponse.Error == 0)
                    {
                        break;
                    }

                    await TimerComponent.Instance.WaitAsync(100);
                }

                if (mountInfoResponse == null || mountInfoResponse.Error != 0)
                {
                    G2C_OpenTheMountPanelResponse openMountResponse =
                        (G2C_OpenTheMountPanelResponse)await SessionComponent.Instance.Session.Call(
                            new C2G_OpenTheMountPanelRequest() { });
                    if (openMountResponse.Error == 0 && !string.IsNullOrEmpty(openMountResponse.MountInfo))
                    {
                        Dictionary<string, int> mountDict =
                            JsonConvert.DeserializeObject<Dictionary<string, int>>(openMountResponse.MountInfo);
                        if (mountDict != null && mountDict.Count > 0)
                        {
                            string targetKey = preferredMountId.ToString();
                            if (!mountDict.ContainsKey(targetKey))
                            {
                                foreach (KeyValuePair<string, int> item in mountDict)
                                {
                                    mountId = long.Parse(item.Key);
                                    break;
                                }
                            }

                            mountInfoResponse =
                                (G2C_GetMountInfoResponse)await SessionComponent.Instance.Session.Call(
                                    new C2G_GetMountInfoRequest() { MountID = mountId });
                        }
                    }
                }

                if (mountInfoResponse == null || mountInfoResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "坐骑已激活，可在坐骑界面中使用");
                    return;
                }

                curMountUUID = mountId;

                if (roleEntity.IsSafetyZone && mountInfoResponse.ConfigId != 260020)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "坐骑已激活，安全区无法乘骑");
                    return;
                }

                if (mountInfoResponse.IsUsing == 1)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "坐骑已激活");
                    return;
                }

                long currentMountedMountId = GetEquippedMountId();
                if (currentMountedMountId > 0 && currentMountedMountId != mountId)
                {
                    G2C_RecallMountResponse recallMountResponse =
                        (G2C_RecallMountResponse)await SessionComponent.Instance.Session.Call(
                            new C2G_RecallMountRequest() { MountID = currentMountedMountId });
                    if (recallMountResponse.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, recallMountResponse.Error.GetTipInfo());
                        return;
                    }
                }

                G2C_UseMountResponse useMountResponse =
                    (G2C_UseMountResponse)await SessionComponent.Instance.Session.Call(
                        new C2G_UseMountRequest() { MountID = mountId });
                if (useMountResponse.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, useMountResponse.Error.GetTipInfo());
                }
            }
        }

    }
}
