using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ETHotfix
{
    [ObjectSystem]
    public class ClickSelectUnitEntityComponentStart : StartSystem<ClickSelectUnitEntityComponent>
    {
        public override void Start(ClickSelectUnitEntityComponent self)
        {
            ClickSelectUnitEntityComponent.Instance = self;
            LoginStageTrace.Append("MoveClick component-start");
        }
    }

    [ObjectSystem]
    public class ClickSelectUnitEntityComponentUpdate : UpdateSystem<ClickSelectUnitEntityComponent>
    {
        private static bool hasLoggedUpdateReady;
        private static bool hasActiveTouch;

        public override void Update(ClickSelectUnitEntityComponent self)
        {
            if (!hasLoggedUpdateReady)
            {
                hasLoggedUpdateReady = true;
                LoginStageTrace.Append("MoveClick component-update-ready");
            }

            if (!TryGetPointerDown(out Vector3 screenPosition, out string pointerSource))
            {
                return;
            }

            LoginStageTrace.Append($"MoveClick pointer source={pointerSource} pos={screenPosition.x:0.0},{screenPosition.y:0.0} touchCount={Input.touchCount}");

            bool hitUi = GameUtility.GetPointerOverGameObject(out GameObject pressObject);
            LoginStageTrace.Append($"MoveClick uiHit={hitUi} path={GetObjectPath(pressObject)}");

            if (hitUi)
            {
                string pressPath = GetObjectPath(pressObject);
                if (!ShouldAllowGroundMoveThroughUi(pressObject))
                {
                    LoginStageTrace.Append($"MoveClick blockedByUI path={pressPath}");
                    Log.Debug($"MoveClick blockedByUI path={pressPath}");
                    return;
                }

                LoginStageTrace.Append($"MoveClick passThroughUI path={pressPath}");
                Log.Debug($"MoveClick passThroughBrokenUI path={pressPath}");
            }

            RoleEntity localRole = UnitEntityComponent.Instance?.LocalRole;
            if (localRole == null)
            {
                LoginStageTrace.Append("MoveClick localRole-null");
                return;
            }

            RoleMoveControlComponent moveControl = localRole.GetComponent<RoleMoveControlComponent>();
            if (moveControl == null)
            {
                LoginStageTrace.Append("MoveClick moveControl-null");
                return;
            }

            LoginStageTrace.Append($"MoveClick localRole-ok node={localRole.CurrentNodePos.x:0.0},{localRole.CurrentNodePos.z:0.0}");

            if (TryHandleNpcClick(screenPosition))
            {
                LoginStageTrace.Append($"MoveClick npcHandled screen={screenPosition.x:0.0},{screenPosition.y:0.0}");
                Log.Debug($"MoveClick npcHandled screen={screenPosition.x:F1},{screenPosition.y:F1}");
                return;
            }

            LoginStageTrace.Append("MoveClick npcMiss");

            if (TryHandleUnitSelection(self, screenPosition))
            {
                LoginStageTrace.Append($"MoveClick unitHandled screen={screenPosition.x:0.0},{screenPosition.y:0.0}");
                Log.Debug($"MoveClick unitHandled screen={screenPosition.x:F1},{screenPosition.y:F1}");
                return;
            }

            LoginStageTrace.Append("MoveClick unitMiss");
            LoginStageTrace.Append($"MoveClick ground screen={screenPosition.x:0.0},{screenPosition.y:0.0}");
            moveControl.ClickGroundMove(screenPosition);
        }

        private static bool TryGetPointerDown(out Vector3 screenPosition, out string pointerSource)
        {
            if (Input.touchCount > 0)
            {
                for (int i = 0; i < Input.touchCount; ++i)
                {
                    Touch touch = Input.GetTouch(i);
                    if (touch.phase == TouchPhase.Began)
                    {
                        hasActiveTouch = true;
                        screenPosition = touch.position;
                        pointerSource = $"touch:{touch.fingerId}";
                        return true;
                    }
                }

                Touch primaryTouch = Input.GetTouch(0);
                if (!hasActiveTouch)
                {
                    hasActiveTouch = true;
                    screenPosition = primaryTouch.position;
                    pointerSource = $"touch:{primaryTouch.fingerId}:synthetic";
                    return true;
                }
            }
            else
            {
                hasActiveTouch = false;
            }

            if (Input.GetMouseButtonDown(0))
            {
                screenPosition = Input.mousePosition;
                pointerSource = "mouse";
                return true;
            }

            screenPosition = Vector3.zero;
            pointerSource = "none";
            return false;
        }

        private static bool ShouldAllowGroundMoveThroughUi(GameObject pressObject)
        {
            if (pressObject == null)
            {
                return false;
            }

            bool sawGraphic = false;
            Transform current = pressObject.transform;
            while (current != null)
            {
                if (HasInteractiveComponent(current))
                {
                    return false;
                }

                Image image = current.GetComponent<Image>();
                if (image != null)
                {
                    sawGraphic = true;
                    if (IsBlockingImage(image))
                    {
                        return false;
                    }
                }

                RawImage rawImage = current.GetComponent<RawImage>();
                if (rawImage != null)
                {
                    sawGraphic = true;
                    if (IsBlockingRawImage(rawImage))
                    {
                        return false;
                    }
                }

                Text text = current.GetComponent<Text>();
                if (text != null)
                {
                    sawGraphic = true;
                    if (IsBlockingText(text))
                    {
                        return false;
                    }
                }

                current = current.parent;
            }

            return sawGraphic;
        }

        private static bool HasInteractiveComponent(Transform transform)
        {
            return transform.GetComponent<Selectable>() != null ||
                   transform.GetComponent<ScrollRect>() != null ||
                   transform.GetComponent<EventTrigger>() != null;
        }

        private static bool IsBlockingImage(Image image)
        {
            if (image == null || !image.enabled || !image.gameObject.activeInHierarchy || !image.raycastTarget)
            {
                return false;
            }

            if (image.color.a <= 0.05f)
            {
                return false;
            }

            if (image.sprite != null || image.overrideSprite != null)
            {
                return true;
            }

            return !IsNearWhite(image.color);
        }

        private static bool IsBlockingRawImage(RawImage image)
        {
            if (image == null || !image.enabled || !image.gameObject.activeInHierarchy || !image.raycastTarget)
            {
                return false;
            }

            if (image.color.a <= 0.05f)
            {
                return false;
            }

            if (image.texture != null)
            {
                return true;
            }

            return !IsNearWhite(image.color);
        }

        private static bool IsBlockingText(Text text)
        {
            return text != null &&
                   text.enabled &&
                   text.gameObject.activeInHierarchy &&
                   text.raycastTarget &&
                   text.color.a > 0.05f &&
                   !string.IsNullOrWhiteSpace(text.text);
        }

        private static bool IsNearWhite(Color color)
        {
            return color.r >= 0.92f &&
                   color.g >= 0.92f &&
                   color.b >= 0.92f &&
                   color.a >= 0.35f;
        }

        private static string GetObjectPath(GameObject obj)
        {
            if (obj == null)
            {
                return "null";
            }

            List<string> names = new List<string>();
            Transform current = obj.transform;
            while (current != null)
            {
                names.Add(current.name);
                current = current.parent;
            }

            names.Reverse();
            return string.Join("/", names);
        }

        private static bool TryHandleUnitSelection(ClickSelectUnitEntityComponent self, Vector3 screenPosition)
        {
            if (!TryCastUnitObject(screenPosition, out GameObject obj) || obj == null)
            {
                return false;
            }

            foreach (KeyValuePair<long, UnitEntity> unit in UnitEntityComponent.Instance.AllUnitEntityDic)
            {
                if (unit.Value == null || unit.Value.Game_Object == null)
                {
                    continue;
                }

                if (unit.Value is KnapsackItemEntity || unit.Value is NPCEntity)
                {
                    continue;
                }

                if (!IsHitOnEntityObject(unit.Value.Game_Object, obj))
                {
                    continue;
                }

                if (unit.Value is RoleEntity roleEntity && unit.Key != UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    HandleRoleSelection(self, roleEntity);
                    return true;
                }

                if (unit.Value is MonsterEntity monster)
                {
                    self.curSelectUnit = monster;
                    self.SetSelectEffect();
                    return true;
                }
            }

            return true;
        }

        private static bool TryCastUnitObject(Vector3 screenPosition, out GameObject obj)
        {
            obj = null;

            Camera camera = Camera.main;
            if (camera == null)
            {
                return false;
            }

            int monsterLayer = LayerMask.NameToLayer(LayerNames.MONSTER);
            int roleLayer = LayerMask.NameToLayer(LayerNames.ROLE);
            int mask = 0;

            if (monsterLayer >= 0)
            {
                mask |= 1 << monsterLayer;
            }

            if (roleLayer >= 0)
            {
                mask |= 1 << roleLayer;
            }

            if (mask == 0)
            {
                return false;
            }

            Ray ray = camera.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 500f, mask) || hit.collider == null)
            {
                return false;
            }

            obj = hit.collider.gameObject;
            return obj != null;
        }

        private static void HandleRoleSelection(ClickSelectUnitEntityComponent self, RoleEntity roleEntity)
        {
            RoleStallUpComponent stall = roleEntity.GetComponent<RoleStallUpComponent>();
            if (stall != null && stall.IsStallUp)
            {
                self.curSelectUnit = roleEntity;
                UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Stallup_OtherPlayer);
                return;
            }

            if (GlobalDataManager.BattleModel == E_BattleType.Peace)
            {
                if (GlobalDataManager.IsBeatBack)
                {
                    if (self.curSelectUnit != null && roleEntity.Id == self.curSelectUnit.Id)
                    {
                        roleEntity.GetComponent<UIUnitEntityHpBarComponent>()?.ShowHp(false);
                        self.ClearSelectUnit();
                    }
                    else
                    {
                        self.curSelectUnit = roleEntity;
                        roleEntity.GetComponent<UIUnitEntityHpBarComponent>()?.ShowHp();
                        self.SetSelectEffect();
                    }

                    return;
                }

                self.curSelectUnit = roleEntity;
                if (SiegeWarfareData.SiegeWarfareIsStart &&
                    SceneComponent.Instance.CurrentSceneName == SceneName.BingFengGu.ToString())
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "\u653b\u57ce\u6218\u671f\u95f4\u51b0\u98ce\u8c37\u7981\u6b62\u4ea4\u6613");
                    return;
                }

                UIComponent.Instance.VisibleUI(UIType.UISelectOtherPlayer);
                UI selfPlayerUi = UIComponent.Instance.Get(UIType.UISelectOtherPlayer);
                if (selfPlayerUi != null)
                {
                    UISelectOtherPlayerComponent component = selfPlayerUi.GetComponent<UISelectOtherPlayerComponent>();
                    component?.SetCurSelectRole(roleEntity);
                }

                self.SetSelectEffect();
                return;
            }

            if (GlobalDataManager.BattleModel == E_BattleType.Whole)
            {
                if (self.curSelectUnit != null && roleEntity.Id == self.curSelectUnit.Id)
                {
                    roleEntity.GetComponent<UIUnitEntityHpBarComponent>()?.ShowHp(false);
                    self.ClearSelectUnit();
                }
                else
                {
                    self.curSelectUnit = roleEntity;
                    roleEntity.GetComponent<UIUnitEntityHpBarComponent>()?.ShowHp();
                    self.SetSelectEffect();
                }

                return;
            }

            if (roleEntity.Property != null &&
                roleEntity.Property.GetProperValue(E_GameProperty.Level) < GlobalDataManager.Battlelev)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, $"等级不足，达到{GlobalDataManager.Battlelev}级后才能PK");
                self.curSelectUnit = null;
                return;
            }

            long selectedId = roleEntity.Id;
            if (TeamDatas.OtherTeamMemberStatusList.Exists(r => r.GameUserId == selectedId))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "目标是队友，无法攻击");
                return;
            }

            if (FriendListData.FriendList.Exists(r => r.UUID == selectedId))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "目标是好友，无法攻击");
                return;
            }

            if (WarAllianceDatas.WarMemberList.Exists(r => r.UUID == selectedId))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "目标是盟友，无法攻击");
                return;
            }

            self.curSelectUnit = roleEntity;
            self.SetSelectEffect();
        }

        private static bool TryHandleNpcClick(Vector3 screenPosition)
        {
            if (!TryCastNpcObject(screenPosition, out GameObject npc) || npc == null)
            {
                if (!TryFindNearbyNpc(screenPosition, out NPCEntity nearbyNpc))
                {
                    return false;
                }

                nearbyNpc.ClickEvent();
                Log.Debug($"MoveClick npcHandled fallback screen={screenPosition.x:F1},{screenPosition.y:F1} npc={nearbyNpc.configId}");
                return true;
            }

            foreach (KeyValuePair<long, NPCEntity> pair in UnitEntityComponent.Instance.NPCEntityDic)
            {
                if (pair.Value == null || pair.Value.Game_Object == null)
                {
                    continue;
                }

                if (IsHitOnEntityObject(pair.Value.Game_Object, npc))
                {
                    pair.Value.ClickEvent();
                    return true;
                }
            }

            return true;
        }

        private static bool TryFindNearbyNpc(Vector3 screenPosition, out NPCEntity npcEntity)
        {
            npcEntity = null;

            Camera camera = Camera.main;
            RoleEntity localRole = UnitEntityComponent.Instance?.LocalRole;
            Dictionary<long, NPCEntity> npcDic = UnitEntityComponent.Instance?.NPCEntityDic;
            if (camera == null || localRole == null || localRole.CurrentNodePos == null || npcDic == null)
            {
                return false;
            }

            const float maxTapDistanceSqr = 180f * 180f;
            float bestDistanceSqr = maxTapDistanceSqr;

            foreach (KeyValuePair<long, NPCEntity> pair in npcDic)
            {
                NPCEntity candidate = pair.Value;
                if (candidate == null || candidate.Game_Object == null || !candidate.Game_Object.activeInHierarchy || candidate.CurrentNodePos == null)
                {
                    continue;
                }

                if (PositionHelper.Distance2D(candidate.CurrentNodePos, localRole.CurrentNodePos) >= 9)
                {
                    continue;
                }

                if (!TryGetNpcScreenPoint(candidate.Game_Object, camera, out Vector3 npcScreenPoint))
                {
                    continue;
                }

                float distanceSqr = (npcScreenPoint - screenPosition).sqrMagnitude;
                if (distanceSqr >= bestDistanceSqr)
                {
                    continue;
                }

                bestDistanceSqr = distanceSqr;
                npcEntity = candidate;
            }

            return npcEntity != null;
        }

        private static bool TryGetNpcScreenPoint(GameObject npcObject, Camera camera, out Vector3 screenPoint)
        {
            screenPoint = Vector3.zero;
            if (npcObject == null || camera == null)
            {
                return false;
            }

            Transform npcTransform = npcObject.transform;
            if (npcTransform == null)
            {
                return false;
            }

            Vector3 worldPoint = npcTransform.position + Vector3.up * 1.2f;

            screenPoint = camera.WorldToScreenPoint(worldPoint);
            return screenPoint.z > 0f;
        }

        private static bool TryCastNpcObject(Vector3 screenPosition, out GameObject npc)
        {
            npc = null;

            if (Camera.main == null)
            {
                return false;
            }

            int npcLayer = LayerMask.NameToLayer(LayerNames.NPC);
            if (npcLayer < 0)
            {
                return false;
            }

            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, 500, 1 << npcLayer))
            {
                return false;
            }

            npc = hit.collider != null ? hit.collider.gameObject : null;
            return npc != null;
        }

        private static bool IsHitOnEntityObject(GameObject entityObject, GameObject hitObject)
        {
            if (entityObject == null || hitObject == null)
            {
                return false;
            }

            if (entityObject.GetInstanceID() == hitObject.GetInstanceID())
            {
                return true;
            }

            Transform entityTransform = entityObject.transform;
            Transform hitTransform = hitObject.transform;
            return hitTransform.IsChildOf(entityTransform) || entityTransform.IsChildOf(hitTransform);
        }
    }

    public class ClickSelectUnitEntityComponent : Component
    {
        public static ClickSelectUnitEntityComponent Instance;

        public UnitEntity curSelectUnit;

        private GameObject selectEffect;

        private string selectEffectResName = "selectCircle";

        public bool hasOneClock = false;

        public float doubleClickInterval = 0.5f;

        public float timer = 0;

        public Dictionary<long, List<Struct_Property>> OtherStallUpItemProDic =
            new Dictionary<long, List<Struct_Property>>();

        public void ClearSelectUnit()
        {
            curSelectUnit = null;
            if (selectEffect == null)
            {
                return;
            }

            ResourcesComponent.Instance.RecycleGameObject(selectEffect);
            selectEffect = null;
        }

        public void SetSelectEffect()
        {
            if (curSelectUnit == null || curSelectUnit.Game_Object == null)
            {
                return;
            }

            selectEffect = selectEffect != null
                ? selectEffect
                : ResourcesComponent.Instance.LoadGameObject(selectEffectResName.StringToAB(), selectEffectResName);
            if (selectEffect == null)
            {
                return;
            }

            selectEffect.transform.SetParent(curSelectUnit.Game_Object.transform);
            selectEffect.transform.localPosition = Vector3.up * 0.5f;
            float s = selectEffect.transform.localScale.x;
            selectEffect.transform.localScale = new Vector3(s, s * 3, s);
            selectEffect.SetActive(true);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
            Instance = null;
        }
    }
}
