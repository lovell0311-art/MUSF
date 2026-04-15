using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Text.RegularExpressions;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIUnitEntityHpBarComponentAwake : AwakeSystem<UIUnitEntityHpBarComponent>
    {
        public override void Awake(UIUnitEntityHpBarComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class UIUnitEntityHpBarComponentUpdate : UpdateSystem<UIUnitEntityHpBarComponent>
    {
        public override void Update(UIUnitEntityHpBarComponent self)
        {
            if (self.unitEntity is RoleEntity otherRole && otherRole.Id != UnitEntityComponent.Instance.LocaRoleUUID)
            {
                if (Time.time > self.delayHideRoleSliderTime)
                {
                    self.ShowHp(false);
                }
            }

            self.Update();
        }
    }

    /// <summary>
    /// 实体名字、血条显示组件
    /// </summary>
    public class UIUnitEntityHpBarComponent : Component
    {
        private readonly string resName = "UIUnitEntityHpBar";
        private readonly string npcresName = "UINPCUnitEntityHpBar";

        private static readonly Color ChooseRoleSelectedBackgroundColor = new Color32(255, 194, 76, 235);
        private static readonly Color ChooseRoleIdleBackgroundColor = new Color32(24, 18, 10, 160);
        private static readonly Color ChooseRoleSelectedTypeColor = new Color32(255, 249, 214, 255);
        private static readonly Color ChooseRoleIdleTypeColor = new Color32(235, 235, 235, 255);
        private static readonly Color ChooseRoleSelectedNameColor = new Color32(255, 238, 138, 255);
        private static readonly Color ChooseRoleIdleNameColor = new Color32(176, 176, 176, 255);
        private static readonly Vector3 ChooseRoleSelectedScale = new Vector3(1.12f, 1.12f, 1f);
        private static readonly Vector3 ChooseRoleIdleScale = Vector3.one;

        private GameObject UIUnitEntityHpBar;
        public GameObject ZhuanShen;
        private Transform transform;
        public UnitEntity unitEntity;

        public Transform topPoint;
        public Text nameTxt;
        public Text typeTxt;
        public Image title;
        public Image Background;
        public Slider hpSlider;
        public GameObject Top;
        public GameObject TitleImage;
        public float delayHideRoleSliderTime;

        private Transform maincameraTransform;
        private RectTransform rectTransform;
        private string chooseRoleName = string.Empty;
        private string chooseRoleType = string.Empty;
        private int chooseRoleLevel = 0;
        private bool isChooseRoleSelected = false;

        public void Awake()
        {
            unitEntity = GetParent<UnitEntity>();
            if (unitEntity is NPCEntity || (unitEntity is MonsterEntity monster && monster.monsterConfigId == 547))
            {
                UIUnitEntityHpBar = ResourcesComponent.Instance.LoadGameObject(npcresName.StringToAB(), npcresName);
            }
            else
            {
                UIUnitEntityHpBar = ResourcesComponent.Instance.LoadGameObject(resName.StringToAB(), resName);
            }

            transform = UIUnitEntityHpBar.transform;
            rectTransform = transform.GetComponent<RectTransform>();

            Transform topPointTransform = unitEntity.Game_Object.transform.Find("TopPoint");
            topPoint = topPointTransform != null ? topPointTransform : null;

            ReferenceCollector collectorData = transform.GetReferenceCollector();
            if (unitEntity is NPCEntity)
            {
                nameTxt = collectorData.GetText("Name");
            }
            else
            {
                nameTxt = collectorData.GetText("Name");
                typeTxt = collectorData.GetText("Type");
                title = collectorData.GetImage("Title");
                Background = transform.Find("Background")?.GetComponent<Image>();
                ZhuanShen = collectorData.GetGameObject("ZhuanShen");
                title.gameObject.SetActive(false);
                typeTxt.text = string.Empty;
                nameTxt.text = string.Empty;
                hpSlider = collectorData.GetGameObject("HpBarSlider").GetComponent<Slider>();
                Top = collectorData.GetGameObject("Top");
            }

            maincameraTransform = CameraComponent.Instance.MainCamera.transform;
            Init();
        }

        public void Update()
        {
            if (transform == null || maincameraTransform == null)
            {
                return;
            }

            transform.rotation = maincameraTransform.rotation;
        }

        public void Hide()
        {
            UIUnitEntityHpBar.SetActive(false);
        }

        public void Show()
        {
            if (UIUnitEntityHpBar == null)
            {
                UIUnitEntityHpBar = ResourcesComponent.Instance.LoadGameObject(resName.StringToAB(), resName);
                transform = UIUnitEntityHpBar.transform;
            }

            UIUnitEntityHpBar.SetActive(true);
        }

        private void Init()
        {
            if (topPoint != null)
            {
                transform.SetParent(topPoint);
            }

            transform.localPosition = Vector3.up * .5f;
        }

        public void ChangePos(int x, int z, int y)
        {
            float tempy = Mathf.Clamp(Mathf.Abs(y), 0, 1);
            rectTransform.anchoredPosition3D = new Vector3(x * 2, tempy + 1.5f, z * 2);
        }

        public void SetChooseRoleInfo(string roleName, string roleType, int roleLevel)
        {
            chooseRoleName = roleName ?? string.Empty;
            chooseRoleType = roleType ?? string.Empty;
            chooseRoleLevel = roleLevel;

            if (ZhuanShen != null)
            {
                ZhuanShen.SetActive(false);
            }

            ShowHp(false);
            ApplyChooseRoleVisualState();
        }

        public void SetChooseRoleSelected(bool isSelected)
        {
            isChooseRoleSelected = isSelected;
            ApplyChooseRoleVisualState();
        }

        private void ApplyChooseRoleVisualState()
        {
            if (typeTxt != null)
            {
                Color typeColor = isChooseRoleSelected ? ChooseRoleSelectedTypeColor : ChooseRoleIdleTypeColor;
                string roleNameText = isChooseRoleSelected ? $"【已选】{chooseRoleName}" : chooseRoleName;
                typeTxt.text = $"<color=#{ColorUtility.ToHtmlStringRGB(typeColor)}>{roleNameText}</color>";
            }

            if (nameTxt != null)
            {
                Color nameColor = isChooseRoleSelected ? ChooseRoleSelectedNameColor : ChooseRoleIdleNameColor;
                string careerText = isChooseRoleSelected ? $"<b>{chooseRoleType}  Lev:{chooseRoleLevel}</b>" : $"{chooseRoleType}  Lev:{chooseRoleLevel}";
                nameTxt.text = $"<color=#{ColorUtility.ToHtmlStringRGB(nameColor)}>{careerText}</color>";
            }

            if (Background != null)
            {
                Background.gameObject.SetActive(true);
                Background.color = isChooseRoleSelected ? ChooseRoleSelectedBackgroundColor : ChooseRoleIdleBackgroundColor;
                Background.transform.localScale = isChooseRoleSelected ? ChooseRoleSelectedScale : ChooseRoleIdleScale;
            }
        }

        public void SetEntityName(string roleName, string color = "#FFFFFF")
        {
            ShowHp(false);
            typeTxt.text = string.Empty;
            nameTxt.text = $"<color={color}>{roleName}</color>";
        }

        public void SetElesReincarnation(int reincarnateCnt)
        {
        }

        public void SetReincarnation()
        {
        }

        public void SetEntityWarName(string warName)
        {
            if (string.IsNullOrEmpty(warName))
            {
                return;
            }

            if (warName.Contains("%"))
            {
                typeTxt.text = $"<color=#00FF00>{warName.Split('%')[0]}</color>    {warName.Split('%')[1]}";
            }
            else
            {
                typeTxt.text = $"<color=#00FF00>{warName}</color>";
            }
        }

        public void RefreshTitle(bool isshow)
        {
            if (TitleImage == null || this.unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                return;
            }

            TitleImage.SetActive(isshow);
        }

        public void SetEntityTitle(long titleID)
        {
            if (TitleImage != null)
            {
                ResourcesComponent.Instance.DestoryGameObjectImmediate(TitleImage, TitleImage.name.StringToAB());
            }

            TitleConfig_InfoConfig titleConfig = ConfigComponent.Instance.GetItem<TitleConfig_InfoConfig>((int)titleID);
            if (titleConfig != null)
            {
                TitleImage = ResourcesComponent.Instance.LoadGameObject(titleConfig.AsstetName.StringToAB(), titleConfig.AsstetName);
                TitleImage.transform.parent = title.transform;
                TitleImage.transform.localRotation = Quaternion.identity;
                TitleImage.transform.localScale = Vector3.one * 0.8f;
                TitleImage.transform.localPosition = Vector3.zero;
                TitleImage.transform.Find("UI").gameObject.SetActive(false);
                TitleImage.transform.Find("World").gameObject.SetActive(true);

                title.gameObject.SetActive(true);

                if (unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
                {
                    TitleImage.SetLayer(LayerNames.HIDDEN);
                }
                else
                {
                    TitleImage.SetLayer(LayerNames.ROLE);
                }

                RefreshTitle(GlobalDataManager.IsHideRole);
            }
            else
            {
                title.gameObject.SetActive(false);
            }
        }

        public void ChangeNameColor(string color)
        {
            string nametxt = Regex.Replace(nameTxt.text, "<[^>]*>", "");
            if (this.unitEntity.Id == UnitEntityComponent.Instance.LocaRoleUUID)
            {
                nameTxt.text = $"<color={color}><b>{nametxt}</b></color>";
            }
            else
            {
                nameTxt.text = $"<color={color}>{nametxt}</color>";
            }
        }

        public void SetMonsterName(string monsterName, string color = "#FFFFFF")
        {
            ShowHp(true);
            typeTxt.text = string.Empty;
            nameTxt.text = $"<color={color}>{monsterName}</color>";
        }

        public void ShowHp(bool isShow = true)
        {
            if (hpSlider == null)
            {
                return;
            }

            if (hpSlider.maxValue == 0)
            {
                hpSlider.maxValue = 100;
            }

            hpSlider.value = hpSlider.maxValue;
            hpSlider.gameObject.SetActive(isShow);
        }

        public void ChangeState(bool isHide)
        {
            UIUnitEntityHpBar.SetActive(isHide);
        }

        public void PetHp(float Maxvalue, float value)
        {
            hpSlider.value = (float)value / Maxvalue;
            Log.DebugBrown("value:" + hpSlider.value);
        }

        public void ChangeHp(float Maxvalue, float value)
        {
            if (hpSlider.gameObject.activeSelf == false)
            {
                hpSlider.gameObject.SetActive(true);
            }

            if (UIUnitEntityHpBar.activeSelf == false)
            {
                ChangeState(true);
            }

            if (this.unitEntity is RoleEntity otherRole && otherRole.Id != UnitEntityComponent.Instance.LocaRoleUUID)
            {
                delayHideRoleSliderTime = Time.time + 5;
            }

            hpSlider.maxValue = Maxvalue;
            hpSlider.value = value;

            if (value <= 0)
            {
                if (unitEntity is MonsterEntity monster)
                {
                    monster.Dead();
                }
                else if (unitEntity is RoleEntity role)
                {
                    role.Dead();
                }
                else
                {
                    unitEntity.Dead();
                }
            }
        }

        public void ShowNpcName(string npcName)
        {
            nameTxt.text = npcName;
        }

        public void SetOderLayer(int layer)
        {
            Canvas canvas = transform.GetComponent<Canvas>();
            canvas.sortingOrder = layer;
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
            if (UIUnitEntityHpBar != null)
            {
                if (TitleImage != null)
                {
                    ResourcesComponent.Instance.DestoryGameObjectImmediate(TitleImage, TitleImage.ToString().StringToAB());
                }

                UIUnitEntityHpBar.SetActive(true);
                ResourcesComponent.Instance.RecycleGameObject(UIUnitEntityHpBar);
            }

            transform = null;
        }
    }
}
