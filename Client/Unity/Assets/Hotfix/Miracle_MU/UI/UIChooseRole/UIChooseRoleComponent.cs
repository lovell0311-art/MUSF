using System;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIChooseRoleComponentAwake : AwakeSystem<UIChooseRoleComponent>
    {
        public override void Awake(UIChooseRoleComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class UIChooseRoleComponentUpdate : UpdateSystem<UIChooseRoleComponent>
    {
        public override void Update(UIChooseRoleComponent self)
        {
            self.Update();
        }
    }

    /// <summary>
    /// 选择角色组件
    /// </summary>
    public class UIChooseRoleComponent : Component
    {
        private long curSelectPlayerUUID = 0;
        private readonly List<RoleEntity> roleEntityList = new List<RoleEntity>();
        private readonly Dictionary<long, GameObject> selectCircleObjs = new Dictionary<long, GameObject>();
        private bool isChooseRole = false;

        private readonly string selectCircle = "selectCircle";
        private bool isRefresh = false;
        private LastRoelInfo lastRoelInfo = null;

        public bool IsRefresh
        {
            get { return isRefresh; }
            set
            {
                isRefresh = value;
                if (isRefresh)
                {
                    InitRoleMode();
                }
            }
        }

        public void Awake()
        {
            CameraComponent.Instance.MainCamera.cullingMask |= (1 << LayerNames.GetLayerInt(LayerNames.ROLE));

            IsRefresh = true;
            ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
            collector.gameObject.GetComponent<Canvas>().planeDistance = 10;
            collector.GetButton("CreateRoleBtn").onClick.AddSingleListener(OnCreatClick);
            collector.GetButton("DeleteBtn").onClick.AddSingleListener(OnDeletOnClick);
            collector.GetButton("StartBtn").onClick.AddSingleListener(OnStartBtnClick);
            lastRoelInfo = LocalDataJsonComponent.Instance.LoadData<LastRoelInfo>(LocalJsonDataKeys.LastRoleInfo) ?? new LastRoelInfo();
        }

        public void Update()
        {
            if (!GameUtility.TryGetPrimaryPointerDown(out Vector3 screenPosition, out int pointerId))
            {
                return;
            }

            if (RaycastHelper.CastRoleObj(screenPosition, out GameObject role))
            {
                RoleEntity unit = TryGetRoleEntity(role);
                if (unit == null)
                {
                    return;
                }

                SelectRole(unit);
                return;
            }

            if (GameUtility.IsPointerOverGameObject(pointerId))
            {
                return;
            }

            TrySelectRoleByScreenPosition(screenPosition);
        }

        /// <summary>
        /// 初始化角色
        /// </summary>
        private void InitRoleMode()
        {
            ClearRoleList();
            isChooseRole = false;
            curSelectPlayerUUID = 0;
            lastRoelInfo = LocalDataJsonComponent.Instance.LoadData<LastRoelInfo>(LocalJsonDataKeys.LastRoleInfo) ?? new LastRoelInfo();

            int i = 0;
            foreach (RoleArchiveInfo roleInfo in RoleArchiveInfoManager.Instance.GetRoleArchivesInDisplayOrder())
            {
                ShowRoleMode(i, roleInfo);
                i++;
            }

            if (!isChooseRole && roleEntityList.Count > 0)
            {
                SelectRole(roleEntityList[0]);
            }

            if (!isChooseRole && lastRoelInfo != null)
            {
                lastRoelInfo.LastRoleUUID = 0;
            }
        }

        /// <summary>
        /// 显示角色模型
        /// </summary>
        private void ShowRoleMode(int roleIndex, RoleArchiveInfo roleInfos)
        {
            E_RoleType roletype = (E_RoleType)roleInfos.RoleType;
            string resName = roletype.GetRoleResName();
            GameObject role = ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadGameObject(resName.StringToAB(), resName);
            role.SetLayer(LayerNames.ROLE);

            roleIndex += 1;
            int symbol = roleIndex % 2 == 0 ? 1 : -1;
            role.transform.position = new Vector3(5 * ((roleIndex / 2) * symbol - 1), 1.2f, 4);

            int symbolRota = roleIndex % 2 == 0 ? -1 : 1;
            role.transform.rotation = Quaternion.Euler(new Vector3(0, 15 * (roleIndex / 2) * symbolRota, 0));
            role.transform.GetChild(0).transform.localRotation = Quaternion.identity;
            role.transform.GetChild(0).transform.localPosition = Vector3.zero;

            RoleEntity roleEntity = ComponentFactory.CreateWithId<RoleEntity, GameObject>(roleInfos.UUID, role);
            roleEntity.RoleType = (E_RoleType)roleInfos.RoleType;

            UIUnitEntityHpBarComponent hpBarComponent = roleEntity.AddComponent<UIUnitEntityHpBarComponent>();
            hpBarComponent.SetChooseRoleInfo(roleInfos.Name, roletype.GetRoleName(roleInfos.ClassLev), roleInfos.Level);
            roleEntity.AddComponent<AnimatorComponent>();
            RoleEquipmentComponent roleEquipmentComponent = roleEntity.AddComponent<RoleEquipmentComponent>();
            roleEquipmentComponent.UpdateRoleEquipment(roleInfos.struct_ItemIns);

            roleEntityList.Add(roleEntity);

            if (lastRoelInfo != null && lastRoelInfo.LastRoleUUID == roleInfos.UUID)
            {
                SelectRole(roleEntity);
            }
            else
            {
                hpBarComponent.SetChooseRoleSelected(false);
            }
        }

        private RoleEntity TryGetRoleEntity(GameObject hitRole)
        {
            if (hitRole == null)
            {
                return null;
            }

            Transform current = hitRole.transform;
            while (current != null)
            {
                RoleEntity unit = roleEntityList.Find(match =>
                    (match?.Game_Object != null && match.Game_Object.GetInstanceID() == current.gameObject.GetInstanceID()) ||
                    (match?.roleTrs != null && match.roleTrs.gameObject.GetInstanceID() == current.gameObject.GetInstanceID()));
                if (unit != null)
                {
                    return unit;
                }

                current = current.parent;
            }

            return null;
        }

        private bool TrySelectRoleByScreenPosition(Vector3 screenPosition)
        {
            Camera camera = CameraComponent.Instance.MainCamera;
            if (camera == null)
            {
                return false;
            }

            Vector2 pointerPosition = new Vector2(screenPosition.x, screenPosition.y);
            RoleEntity nearestRole = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < roleEntityList.Count; ++i)
            {
                RoleEntity roleEntity = roleEntityList[i];
                if (roleEntity?.Game_Object == null)
                {
                    continue;
                }

                Vector3 roleScreenPoint = GetRoleScreenPoint(camera, roleEntity.Game_Object);
                if (roleScreenPoint.z <= 0f)
                {
                    continue;
                }

                float distance = Vector2.Distance(pointerPosition, new Vector2(roleScreenPoint.x, roleScreenPoint.y));
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestRole = roleEntity;
                }
            }

            if (nearestRole == null || nearestDistance > 420f)
            {
                return false;
            }

            SelectRole(nearestRole);
            return true;
        }

        private Vector3 GetRoleScreenPoint(Camera camera, GameObject roleObject)
        {
            if (camera == null || roleObject == null)
            {
                return Vector3.zero;
            }

            Vector3 worldPoint = roleObject.transform.position + Vector3.up * 2.2f;
            return camera.WorldToScreenPoint(worldPoint);
        }

        private void SelectRole(RoleEntity roleEntity)
        {
            if (roleEntity == null)
            {
                return;
            }

            curSelectPlayerUUID = roleEntity.Id;
            isChooseRole = true;
            RoleArchiveInfoManager.Instance.curSelectRoleUUID = curSelectPlayerUUID;
            ShowSelectCircle(roleEntity);
            RefreshRoleSelectionVisuals();
        }

        private void ShowSelectCircle(RoleEntity roleEntity)
        {
            if (roleEntity?.roleTrs == null)
            {
                return;
            }

            if (!selectCircleObjs.TryGetValue(roleEntity.Id, out GameObject selectCircleObj) || selectCircleObj == null)
            {
                selectCircleObj = ResourcesComponent.Instance.LoadGameObject(selectCircle.StringToAB(), selectCircle);
                if (selectCircleObj == null)
                {
                    return;
                }

                selectCircleObj.SetLayer(LayerNames.ROLE);
                selectCircleObjs[roleEntity.Id] = selectCircleObj;
            }

            selectCircleObj.transform.SetParent(roleEntity.roleTrs, false);
            selectCircleObj.transform.localPosition = new Vector3(0f, 0.02f, 0f);
            selectCircleObj.transform.localRotation = Quaternion.Euler(125f, 0f, 0f);
            selectCircleObj.transform.localScale = Vector3.one * 1.5f;
            selectCircleObj.SetActive(true);
        }

        private void RefreshRoleSelectionVisuals()
        {
            for (int i = 0; i < roleEntityList.Count; ++i)
            {
                RoleEntity roleEntity = roleEntityList[i];
                if (roleEntity == null)
                {
                    continue;
                }

                bool isSelected = roleEntity.Id == curSelectPlayerUUID;
                UIUnitEntityHpBarComponent hpBarComponent = roleEntity.GetComponent<UIUnitEntityHpBarComponent>();
                hpBarComponent?.SetChooseRoleSelected(isSelected);

                if (selectCircleObjs.TryGetValue(roleEntity.Id, out GameObject selectCircleObj) && selectCircleObj != null)
                {
                    selectCircleObj.SetActive(isSelected);
                }
            }
        }

        #region Button点击事件
        private void OnCreatClick()
        {
            isChooseRole = false;
            curSelectPlayerUUID = 0;
            HideAllSelectCircles();

            if (RoleArchiveInfoManager.Instance.roleArchiveInfosDic.Count == 5)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "角色存档已满 不能再新建角色");
                return;
            }

            Game.Scene.GetComponent<UIComponent>().VisibleUI(UIType.UICreatRole);
            Game.Scene.GetComponent<UIComponent>().Remove(UIType.UIChooseRole);
        }

        private void OnStartBtnClick()
        {
            if (!isChooseRole)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请选择你喜爱的角色");
                return;
            }

            StartGameAsync().Coroutine();
        }

        /// <summary>
        /// 进入游戏场景
        /// </summary>
        private async ETVoid StartGameAsync()
        {
            if (curSelectPlayerUUID == 0)
            {
                if (lastRoelInfo.LastRoleUUID == 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, "请先选择一个角色");
                    return;
                }

                curSelectPlayerUUID = lastRoelInfo.LastRoleUUID;
            }

            if (curSelectPlayerUUID == 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请先选择一个角色");
                return;
            }

            G2C_StartGameGamePlayerResponse g2C_GamePlayerStartGameResponse = (G2C_StartGameGamePlayerResponse)await SessionComponent.Instance.Session.Call(new C2G_StartGameGamePlayerRequest
            {
                GameUserId = curSelectPlayerUUID
            });
            if (g2C_GamePlayerStartGameResponse.Error != 0)
            {
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText($"{g2C_GamePlayerStartGameResponse.Error.GetTipInfo()}", true);
                return;
            }

            RoleArchiveInfoManager.Instance.curSelectRoleUUID = curSelectPlayerUUID;
            LogCollectionComponent.Instance.GameUserId = curSelectPlayerUUID;

            UIComponent.Instance.VisibleUI(UIType.UISceneLoading);
            UIComponent.Instance.Remove(UIType.UIChooseRole);
            if (ETModel.Init.instance.e_SDK == E_SDK.XY_SDK)
            {
                SdkCallBackComponent.Instance.sdkUtility.PlayLog(new string[]
                {
                    $"{GlobalDataManager.XYUUID}",
                    $"{GlobalDataManager.EnterZoneID}",
                    $"{GlobalDataManager.EnterZoneName}",
                    $"{curSelectPlayerUUID}",
                    $"{RoleArchiveInfoManager.Instance.roleArchiveInfosDic[curSelectPlayerUUID].Name}",
                    $"{RoleArchiveInfoManager.Instance.roleArchiveInfosDic[curSelectPlayerUUID].Level}",
                    ""
                });
                LogCollectionComponent.Instance.Info("选择角色进入游戏");
            }

            try
            {
                UnitEntityFactory.CreatLocalRole();
                UnitEntityComponent.Instance.LocalRole.Property.ChangeProperValue(E_GameProperty.OccupationLevel, RoleArchiveInfoManager.Instance.curSelectRoleArchiveInfo.ClassLev);
                lastRoelInfo.LastRoleUUID = curSelectPlayerUUID;
                LocalDataJsonComponent.Instance.SavaData(lastRoelInfo, LocalJsonDataKeys.LastRoleInfo);

                UIComponent.Instance.VisibleUI(UIType.UIMainCanvas);

                G2C_ReadyResponse g2C_Ready = (G2C_ReadyResponse)await SessionComponent.Instance.Session.Call(new C2G_ReadyRequest());
                if (g2C_Ready.Error != 0)
                {
                    Log.DebugRed($"{g2C_Ready.Error.GetTipInfo()}");
                    UIComponent.Instance.VisibleUI(UIType.UIHint, $"{g2C_Ready.Error.GetTipInfo()}");
                    return;
                }

                FriendListData.friendChatNewInfos.Clear();
                EnterChatRoom().Coroutine();

                CameraComponent.Instance.MainCamera.farClipPlane = 450;
                CameraComponent.Instance.MainCamera.fieldOfView = 40;
            }
            catch (Exception e)
            {
                Log.DebugRed(e.ToString());
            }

            async ETVoid EnterChatRoom()
            {
                G2C_EnterChatRoom g2C_EnterChatRoom = (G2C_EnterChatRoom)await SessionComponent.Instance.Session.Call(new C2G_EnterChatRoom
                {
                    ChatRoomID = GlobalDataManager.EnterZoneID
                });
                if (g2C_EnterChatRoom.Error != 0)
                {
                    return;
                }

                ChatMessageDataManager.valuePairs[E_ChatType.World] = GlobalDataManager.EnterZoneID;
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        private void OnDeletOnClick()
        {
            if (!isChooseRole)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "请选择你要删除的角色");
                return;
            }

            UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
            RoleArchiveInfo roleArchive = RoleArchiveInfoManager.Instance.GetRoleArchiveInfo(curSelectPlayerUUID);
            uIConfirm.SetTipText($"确定要删除你喜爱的角色 <color=red>{roleArchive.Name}({((E_RoleType)roleArchive.RoleType).GetRoleName(roleArchive.ClassLev)}-Lev:{roleArchive.Level})</color>?", false);
            uIConfirm.AddActionEvent(() =>
            {
                DeleteRoleAsync().Coroutine();
            });
        }

        private async ETVoid DeleteRoleAsync()
        {
            long deletingRoleUUID = curSelectPlayerUUID;
            G2C_LoginSystemDeleteGamePlayerResponse g2C_GamePlayerDeleteResponse = (G2C_LoginSystemDeleteGamePlayerResponse)await SessionComponent.Instance.Session.Call(new C2G_LoginSystemDeleteGamePlayerRequest
            {
                GameId = deletingRoleUUID
            });
            if (g2C_GamePlayerDeleteResponse.Error != 0)
            {
                UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                uIConfirm.SetTipText($"{g2C_GamePlayerDeleteResponse.Error.GetTipInfo()}", true);
                return;
            }

            var stallUpInfo = LocalDataJsonComponent.Instance.LoadData<StallUpInfo>(LocalJsonDataKeys.StallUpName) ?? new StallUpInfo();
            if (!string.IsNullOrEmpty(stallUpInfo.StallName) && stallUpInfo.StallName.Contains($"{deletingRoleUUID}"))
            {
                stallUpInfo.StallName = string.Empty;
            }
            LocalDataJsonComponent.Instance.SavaData(stallUpInfo, LocalJsonDataKeys.StallUpName);

            RoleEntity roleEntity = roleEntityList.Find(r => r != null && r.Id == deletingRoleUUID);
            roleEntity?.Dispose();
            roleEntityList.RemoveAll(r => r == null || r.Id == deletingRoleUUID);
            DestroySelectCircle(deletingRoleUUID);
            RoleArchiveInfoManager.Instance.Remove(deletingRoleUUID);
            RoleArchiveInfoManager.Instance.curSelectRoleUUID = 0;

            curSelectPlayerUUID = 0;
            isChooseRole = false;

            if (roleEntityList.Count > 0)
            {
                SelectRole(roleEntityList[0]);
            }
            else
            {
                HideAllSelectCircles();
            }
        }
        #endregion

        private void ClearRoleList()
        {
            if (roleEntityList.Count != 0)
            {
                for (int i = roleEntityList.Count - 1; i >= 0; i--)
                {
                    roleEntityList[i]?.Dispose();
                }
            }

            roleEntityList.Clear();
            ClearSelectCircles();
        }

        private void HideAllSelectCircles()
        {
            foreach (KeyValuePair<long, GameObject> pair in selectCircleObjs)
            {
                if (pair.Value != null)
                {
                    pair.Value.SetActive(false);
                }
            }
        }

        private void DestroySelectCircle(long roleUUID)
        {
            if (!selectCircleObjs.TryGetValue(roleUUID, out GameObject selectCircleObj))
            {
                return;
            }

            if (selectCircleObj != null)
            {
                ResourcesComponent.Instance.DestoryGameObjectImmediate(selectCircleObj, selectCircle.StringToAB());
            }

            selectCircleObjs.Remove(roleUUID);
        }

        private void ClearSelectCircles()
        {
            foreach (KeyValuePair<long, GameObject> pair in selectCircleObjs)
            {
                if (pair.Value != null)
                {
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(pair.Value, selectCircle.StringToAB());
                }
            }

            selectCircleObjs.Clear();
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
            ClearRoleList();
        }
    }
}
