
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;
using System.Text;
using ILRuntime.Runtime;

namespace ETHotfix
{

    public enum E_SkillType 
    {
     ActiveSkill=1,//主动技能
     SubSkill=2//被动技能
     }

    public class SkillInfo
    {
        public int skill_ConfigIndex;//技能id
        public string skill_Icon_Name;//技能图标名字
        public string skill_Name;//技能名
        public bool IsOwn;//是否学习了该技能
        public int skillType;//技能类型 1 主动 2被动
    }
    [ObjectSystem]
    public class UISkillComponentAwake : AwakeSystem<UISkillComponent>
    {
        public override void Awake(UISkillComponent self)
        {
            self.Awake();
            self.LoadAllSkills();
            self.InitUICircularScrollView();
            self.InitSubSkillUICircularScrollView();

            self.LoadSkill(self.Skills,E_SkillType.ActiveSkill);//加载主动技能
            self.LoadSkill(self.SubSkills,E_SkillType.SubSkill);//加载被动技能
            self.GetHaveBeenStudingSkills();
        }
    }
    /// <summary>
    /// 技能选择 
    /// </summary>
    public class UISkillComponent : Component
    {
        private int curSlotIndex = -1;//当前技能卡槽的索引
        private int curSlotType = -1;//当前技能卡槽的类型
        private const float SkillIntroTopMargin = 108f;
        private const float SkillIntroBottomGap = 20f;
        private const float SkillIntroMinHeight = 420f;
        private const float SkillIntroPreferredHeight = 500f;
        private const float SkillIntroMinRootHeight = 960f;
        private const float SkillIntroViewportInset = 12f;
        private const int SkillIntroTextFontSize = 19;

        private readonly string dusterMaterResName = "ChangeDust";//技能遮罩材质球名
        Material dustMater = null;//技能遮罩 材质球
        List<SkillInfo> basisskillInfos;//全部主动技能集合
        List<SkillInfo> subskillInfos;//全部被动技能集合

        RoleEntity roleEntity;//本地玩家

        UICircularScrollView<SkillInfo> uICircularScrollView,SubSKillUiCircularScrollView;
        public RectTransform SkillContent,SubSkillContent;
        public ScrollRect SkillscrollRect,SubSKillScrollRect;



        public Transform Skills,SubSkills;//技能
        Skillconfiguration skillconfiguration;//技能配置信息

        SkillInfo curDropSkillinfo;//当前正在拖拽的技能 信息
        Image dropImage;
        RectTransform rectTransform;

        public Text page;//页数
        int maxPage;//最大页数

        Text skillName, info;//技能简介信息
        StringBuilder skillInfoBuilder;//技能简介字符串
        ScrollRect introScrollRect;

        public Button UIBeginnerGuide;
        private Button saveButton;
        private Button cancelButton;

        public void Awake()
        {
            basisskillInfos = new List<SkillInfo>();
            subskillInfos = new List<SkillInfo>();
            roleEntity = UnitEntityComponent.Instance.LocalRole;

            //加载缓存的技能
            skillconfiguration = LocalDataJsonComponent.Instance.LoadData<Skillconfiguration>(roleEntity.LocalSkillFillName) ?? new Skillconfiguration();

            ReferenceCollector collector = GetParent<UI>().GameObject.GetReferenceCollector();
            collector.GetButton("CloseBtn").onClick.AddSingleListener(Close);
            SkillContent = collector.GetGameObject("SkillContent").GetComponent<RectTransform>();
            SkillscrollRect = collector.GetImage("SkillScrollView").GetComponent<ScrollRect>();

            SubSkillContent = collector.GetGameObject("SubSkillContent").GetComponent<RectTransform>();
            SubSKillScrollRect = collector.GetImage("SubSkillScrollView").GetComponent<ScrollRect>();

            //加载Ui遮罩 材质球
            AssetBundleComponent.Instance.LoadBundle(dusterMaterResName.StringToAB());
            dustMater = (Material)AssetBundleComponent.Instance.GetAsset(dusterMaterResName.StringToAB(), dusterMaterResName);


           /* //基本技能 与特殊技能 切换
            collector.GetToggle("mainSkillTog").onValueChanged.AddSingleListener((value)=> { ChangeSkillType(value, true); });
            collector.GetToggle("subSkillTog").onValueChanged.AddSingleListener((value)=> { ChangeSkillType(value, false); });*/

            //技能简介信息
            skillInfoBuilder = new StringBuilder();
            skillName = collector.GetText("skillName");
            info = collector.GetText("info");
            introScrollRect = info != null ? info.GetComponentInParent<ScrollRect>() : null;
          
            //当前装备的技能卡槽 父对象
            Skills = collector.GetGameObject("Skills").transform;
            SubSkills = collector.GetGameObject("SubSkills").transform;

            collector.GetButton("skillleftBtn").onClick.AddSingleListener(() => { TrunThePage(E_SkillType.ActiveSkill, 1); });
            collector.GetButton("skillrightBtn").onClick.AddSingleListener(() => { TrunThePage(E_SkillType.ActiveSkill, -1); });

            collector.GetButton("subskillleftBtn").onClick.AddSingleListener(() => { TrunThePage(E_SkillType.SubSkill, 1); });
            collector.GetButton("subskillrightBtn").onClick.AddSingleListener(() => { TrunThePage(E_SkillType.SubSkill, -1); });

            rectTransform = collector.gameObject.GetComponent<RectTransform>();
            dropImage = collector.GetImage("DropImage");
            dropImage.gameObject.SetActive(false);
            saveButton = collector.GetButton("SaveBtn");
            cancelButton = collector.GetButton("CancleBtn");

            ConfigureSkillIntroLayout();
            ClearSkillInfo();

            //保存更改
            saveButton.onClick.AddSingleListener(async delegate 
            {
             /*   //新手引导（配置技能）
                if (TaskDatas.MainTaskInfo.TaskActionType == 103)
                {
                    if (skillconfiguration.SKilDic.Count == 0) return;
                    G2C_UpdateTaskProgress c_UpdateTaskProgress = (G2C_UpdateTaskProgress)await SessionComponent.Instance.Session.Call(new C2G_UpdateTaskProgress 
                    {
                    TaskId= TaskDatas.MainTaskInfo.Id.ToInt32(),
                    ProgressId=0,
                    Value=1
                    });
                    if (c_UpdateTaskProgress.Error != 0)
                    {
                        UIComponent.Instance.VisibleUI(UIType.UIHint, c_UpdateTaskProgress.Error.GetTipInfo());
                    }
                   
                }*/

                LocalDataJsonComponent.Instance.SavaData(skillconfiguration, roleEntity.LocalSkillFillName);
              
                UIMainComponent.Instance.LoadSkills();

                RoleOnHookComponent.Instance.ChangeSkillInfo();
               
            });
            //取消
            cancelButton.onClick.AddSingleListener(Close);

            UIBeginnerGuide = collector.GetButton("UIBeginnerGuide");
            UIBeginnerGuide.onClick.AddSingleListener(() =>
            {
                if (BeginnerGuideData.IsCompleteTrigger(56, 53))
                {
                    BeginnerGuideData.SetBeginnerGuide(56);
                }
                UIMainComponent.Instance.BtnList.horizontal = true;
                UIBeginnerGuide.gameObject.SetActive(false);
            });
            if (BeginnerGuideData.IsCompleteTrigger(55, 53))
            {
                BeginnerGuideData.SetBeginnerGuide(55);
                UIBeginnerGuide.gameObject.SetActive(true);
            }
           
        }

        /// <summary>
        /// 技能换页
        /// </summary>
        /// <param name="skillType"></param>
        /// <param name="value">左 1，右 -1</param>
        public void TrunThePage(E_SkillType skillType,int value)
        {
            RectTransform skilltrs = skillType == E_SkillType.ActiveSkill ? SkillContent : SubSkillContent;
            float pos_x = skilltrs.anchoredPosition.x+ 540 * value;
            
            if (pos_x > 0)
            {
                pos_x = 0;
            }
            skilltrs.anchoredPosition = new Vector3(pos_x, 0, 0);
        }


       public void InitUICircularScrollView()
        {
            uICircularScrollView = ComponentFactory.Create<UICircularScrollView<SkillInfo>>();
            uICircularScrollView.ItemInfoCallBack = InitSkillInfoCallBack;
            uICircularScrollView.ItemClickCallBack = ShowSkillIntroInfo;
            uICircularScrollView.ItemBeginDragEventBack = BeginDragEvent;
            uICircularScrollView.ItemDragEventBack = DragEvent;
            uICircularScrollView.ItemEndDragEventBack = EndDragEvent;
            uICircularScrollView.InitInfo(E_Direction.Horizontal, 1, 35, 0);
            uICircularScrollView.IninContent(SkillContent.gameObject, SkillscrollRect);
            SkillscrollRect.horizontal = true;//禁止滑动
            SkillscrollRect.vertical = false;
            uICircularScrollView.Items = basisskillInfos;
        }
       public void InitSubSkillUICircularScrollView()
        {
            SubSKillUiCircularScrollView = ComponentFactory.Create<UICircularScrollView<SkillInfo>>();
            SubSKillUiCircularScrollView.ItemInfoCallBack = InitSkillInfoCallBack;
            SubSKillUiCircularScrollView.ItemClickCallBack = ShowSkillIntroInfo;
            SubSKillUiCircularScrollView.ItemBeginDragEventBack = BeginDragEvent;
            SubSKillUiCircularScrollView.ItemDragEventBack = DragEvent;
            SubSKillUiCircularScrollView.ItemEndDragEventBack = EndDragEvent;
            SubSKillUiCircularScrollView.InitInfo(E_Direction.Horizontal, 1, 30, 20);
            SubSKillUiCircularScrollView.IninContent(SubSkillContent.gameObject, SubSKillScrollRect);
            SubSKillScrollRect.horizontal = true;//禁止滑动
            SubSKillScrollRect.vertical = false;
            SubSKillUiCircularScrollView.Items = subskillInfos;
        }
        private void ChangeSkillType(bool isOn,bool isBasisSkill)
        {
            if (!isOn)
            {
                return;
            }
            ClearSkillInfo();

            uICircularScrollView.Items = isBasisSkill ? basisskillInfos: subskillInfos;

            if(isBasisSkill)
                maxPage = Mathf.CeilToInt((float)basisskillInfos.Count / 10);
            else
                maxPage = Mathf.CeilToInt((float)subskillInfos.Count / 10);
            page.text = 1.ToString();

        }
        /// <summary>
        /// 获取已经学习的技能
        /// </summary>
        /// <returns></returns>
        public async ETVoid GetHaveBeenStudingSkills() 
        {
            G2C_OpenSkillGroupResponse g2C_OpenSkill = (G2C_OpenSkillGroupResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenSkillGroupRequest());
            if (g2C_OpenSkill.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenSkill.Error.GetTipInfo());
             
            }
            else
            {
                foreach (var item in g2C_OpenSkill.SkillIds)
                {
                    if (roleEntity.OwnSkills.Contains(item))
                        continue;
                    roleEntity.OwnSkills.Add(item);
                }

                RefreshOwnSkillState();
            }
        }

        public void RefreshOwnSkillState()
        {
            LoadAllSkills();
            ClearSkillInfo();

            if (uICircularScrollView != null)
            {
                uICircularScrollView.Items = basisskillInfos;
            }

            if (SubSKillUiCircularScrollView != null)
            {
                SubSKillUiCircularScrollView.Items = subskillInfos;
            }
        }

        /// <summary>
        /// 初始化技能 图标 
        /// </summary>
        /// <param name="game"></param>
        /// <param name="info"></param>
        public void InitSkillInfoCallBack(GameObject game, SkillInfo info)
        {
            game.transform.Find("icon").GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, info.skill_Icon_Name);
            //是否学习了该技能
            game.transform.Find("icon").GetComponent<Image>().material = info.IsOwn ? null : dustMater;
            Debug.Log("技能icon" + info.skill_Icon_Name);
        }
        /// <summary>
        /// 显示技能简介 信息
        /// </summary>
        /// <param name="game"></param>
        /// <param name="info"></param>
        public void ShowSkillIntroInfo(GameObject game, SkillInfo skillInfo)
        {
            ConfigureSkillIntroLayout();
            HideLegacySkillNamePlaceholder();

            //显示技能简介
            skillInfoBuilder.Clear();
            skillInfo.skill_ConfigIndex.GetSkillInfos_RoleType_Out(roleEntity.RoleType,out SkillInfos skillconfigInfo);
            skillInfoBuilder.AppendFormat("<size=22><color=#CEA75A>{0}</color></size>\n\n", skillInfo.skill_Name);
            skillInfoBuilder.Append($"{skillconfigInfo.Describe}\n\n");
            skillInfoBuilder.Append($"<color=yellow>魔法值：</color>{skillconfigInfo.Consume.GetValue(1)}\n");
            skillInfoBuilder.Append($"<color=yellow>AG：</color>{skillconfigInfo.Consume.GetValue(2)}\n");
            skillInfoBuilder.Append($"<color=yellow>攻击距离：</color>{skillconfigInfo.Distance}\n");
            skillInfoBuilder.Append($"<color=yellow>基础技能攻击力：</color>{skillconfigInfo.OtherData.GetValue(1)}\n");
            skillInfoBuilder.Append($"<color=yellow>技能冷却时间：</color>{skillconfigInfo.CoolTime / 1000}");

            this.info.text = skillInfoBuilder.ToString();
            ResetSkillIntroScroll();
            
        }

        

        /// <summary>
        /// 重置 技能简介信息
        /// </summary>
        public void ClearSkillInfo()
        {
            HideLegacySkillNamePlaceholder();
            info.text = string.Empty;
            skillInfoBuilder.Clear();
            ResetSkillIntroScroll();
        }

        private void ConfigureSkillIntroLayout()
        {
            HideLegacySkillNamePlaceholder();

            if (info == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();
            if (rectTransform != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
            }

            RectTransform infoRect = info.rectTransform;
            RectTransform contentRect = infoRect.parent as RectTransform;
            RectTransform viewportRect = contentRect?.parent as RectTransform;
            RectTransform scrollRectRect = introScrollRect != null ? introScrollRect.GetComponent<RectTransform>() : null;
            RectTransform introPanelRect = scrollRectRect?.parent as RectTransform;
            RectTransform introRootRect = introPanelRect?.parent as RectTransform;
            RectTransform saveButtonRect = saveButton != null ? saveButton.transform as RectTransform : null;

            if (introRootRect != null && introPanelRect != null && saveButtonRect != null && saveButtonRect.parent == introRootRect)
            {
                float parentHeight = GetEffectiveRectHeight(introRootRect);
                float buttonTop = saveButtonRect.anchoredPosition.y + saveButtonRect.rect.height;
                float bottomMargin = buttonTop + SkillIntroBottomGap;
                float maxHeight = Mathf.Max(SkillIntroMinHeight, parentHeight - SkillIntroTopMargin - bottomMargin);
                float targetHeight = Mathf.Clamp(SkillIntroPreferredHeight, SkillIntroMinHeight, maxHeight);
                float targetCenterFromBottom = bottomMargin + targetHeight * 0.5f;
                float anchoredY = targetCenterFromBottom - parentHeight * 0.5f;
                introPanelRect.anchoredPosition = new Vector2(introPanelRect.anchoredPosition.x, anchoredY);
                introPanelRect.sizeDelta = new Vector2(introPanelRect.sizeDelta.x, targetHeight);
            }

            if (scrollRectRect != null && introPanelRect != null)
            {
                scrollRectRect.sizeDelta = new Vector2(scrollRectRect.sizeDelta.x, Mathf.Max(360f, introPanelRect.sizeDelta.y - 14f));
                scrollRectRect.anchoredPosition = new Vector2(scrollRectRect.anchoredPosition.x, -4f);
            }

            if (viewportRect != null)
            {
                viewportRect.anchorMin = Vector2.zero;
                viewportRect.anchorMax = Vector2.one;
                viewportRect.pivot = new Vector2(0.5f, 0.5f);
                viewportRect.anchoredPosition = Vector2.zero;
                viewportRect.sizeDelta = new Vector2(-SkillIntroViewportInset * 2f, -SkillIntroViewportInset * 2f);
            }

            if (contentRect != null)
            {
                ContentSizeFitter contentFitter = contentRect.GetComponent<ContentSizeFitter>();
                if (contentFitter != null)
                {
                    contentFitter.enabled = true;
                    contentFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
                    contentFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                }

                VerticalLayoutGroup contentLayout = contentRect.GetComponent<VerticalLayoutGroup>();
                if (contentLayout != null)
                {
                    contentLayout.enabled = true;
                    contentLayout.padding = new RectOffset(16, 16, 12, 18);
                    contentLayout.childAlignment = TextAnchor.UpperLeft;
                    contentLayout.childControlWidth = true;
                    contentLayout.childControlHeight = true;
                    contentLayout.childForceExpandWidth = true;
                    contentLayout.childForceExpandHeight = false;
                    contentLayout.spacing = 0;
                }

                contentRect.anchorMin = new Vector2(0f, 1f);
                contentRect.anchorMax = new Vector2(1f, 1f);
                contentRect.pivot = new Vector2(0f, 1f);
                contentRect.anchoredPosition = Vector2.zero;
                contentRect.sizeDelta = new Vector2(0f, 0f);
            }

            info.alignment = TextAnchor.UpperLeft;
            info.horizontalOverflow = HorizontalWrapMode.Wrap;
            info.verticalOverflow = VerticalWrapMode.Overflow;
            info.supportRichText = true;
            info.fontSize = SkillIntroTextFontSize;
            info.lineSpacing = 1.05f;

            infoRect.anchorMin = new Vector2(0f, 1f);
            infoRect.anchorMax = new Vector2(1f, 1f);
            infoRect.pivot = new Vector2(0f, 1f);
            infoRect.anchoredPosition = Vector2.zero;
            infoRect.sizeDelta = new Vector2(0f, 0f);

            if (introScrollRect != null)
            {
                introScrollRect.horizontal = false;
                introScrollRect.vertical = true;
                introScrollRect.movementType = ScrollRect.MovementType.Clamped;
                introScrollRect.scrollSensitivity = 22f;
            }

            if (contentRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(contentRect);
            }

            if (scrollRectRect != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(scrollRectRect);
            }
        }

        private void HideLegacySkillNamePlaceholder()
        {
            if (skillName != null && string.Equals(skillName.gameObject.name, "skillName"))
            {
                skillName.text = string.Empty;
                skillName.enabled = false;
                skillName.gameObject.SetActive(false);
            }

            UI ui = GetParent<UI>();
            if (ui == null || ui.GameObject == null)
            {
                return;
            }

            Text[] allTexts = ui.GameObject.GetComponentsInChildren<Text>(true);
            for (int i = 0; i < allTexts.Length; ++i)
            {
                Text placeholder = allTexts[i];
                if (placeholder == null || !string.Equals(placeholder.gameObject.name, "skillName"))
                {
                    continue;
                }

                placeholder.text = string.Empty;
                placeholder.enabled = false;
                placeholder.gameObject.SetActive(false);
            }
        }

        private void ResetSkillIntroScroll()
        {
            if (introScrollRect == null)
            {
                return;
            }

            Canvas.ForceUpdateCanvases();
            introScrollRect.StopMovement();
            introScrollRect.verticalNormalizedPosition = 1f;
        }

        private float GetEffectiveRectHeight(RectTransform targetRect)
        {
            if (targetRect != null && targetRect.rect.height > 1f)
            {
                return targetRect.rect.height;
            }

            if (rectTransform != null && rectTransform.rect.height > 1f)
            {
                return rectTransform.rect.height;
            }

            return Mathf.Max(Screen.height, SkillIntroMinRootHeight);
        }
        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="game"></param>
        /// <param name="info"></param>
        public void DragEvent(GameObject game, SkillInfo info)
        {
            if (dropImage.sprite != null)
            {
                RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, Input.mousePosition, CameraComponent.Instance.UICamera, out Vector3 localPos);
                dropImage.transform.position = localPos;
            }
        }

        public void BeginDragEvent(GameObject game, SkillInfo info)
        {
            if (!info.IsOwn)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "该技能还未学习 无法使用");
                return;
            }
            curDropSkillinfo = info;
            dropImage.sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, info.skill_Icon_Name);
            dropImage.gameObject.SetActive(true);
        }
        public void EndDragEvent(GameObject game, SkillInfo info)
        {
            dropImage.sprite = null;
            dropImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示角色的所拥有技能
        /// </summary>
        public void LoadAllSkills()
        {
            roleEntity.RoleType.GetAllSkillConfig(out List<SkillInfos> skillinfos);
            basisskillInfos.Clear();
            subskillInfos.Clear();
            foreach (SkillInfos info in skillinfos)
            {
                SkillInfo skillInfo = new SkillInfo
                {
                    skill_Icon_Name = info.Icon,
                    skill_ConfigIndex = (int)info.Id,
                    skill_Name = info.Name,
                    IsOwn = IsOwnSkill((int)info.Id),
                    skillType = info.skillType
                };
                if (info.skillType == 1)
                    basisskillInfos.Add(skillInfo);
                else if (info.skillType == 2)
                {
                    subskillInfos.Add(skillInfo);
                }
            }
         
        }
       
        /// <summary>
        /// 是否拥有该技能
        /// </summary>
        /// <returns></returns>
        private bool IsOwnSkill(int skillIndex)
        {
            return roleEntity.OwnSkills.Contains(skillIndex);
        }
        /// <summary>
        /// 获取使用类型
        /// </summary>
        /// <param name="configID"></param>
        /// <returns></returns>
        public string GetUserType(int configID)
        {
            configID.GetSkillInfos__Out(out SkillInfos skillInfos);
            return roleEntity.RoleType.GetItemUserName(skillInfos.LearnStandard);

        }
        /// <summary>
        ///  加载已经放入技能卡槽的技能
        /// </summary>
        /// <param name="isSubSkills">是否是辅助技能</param>
        public void LoadSkill(Transform Skills,E_SkillType skillType)
        {
            //主动技能0~3 被动技能 4~6
            for (int i = skillType==E_SkillType.ActiveSkill?0:4,count= skillType == E_SkillType.ActiveSkill ? 4 :7; i < count; i++)
            {
                int sortIndex = i;
                Transform skill = Skills.Find($"skill_{i}");
                var skilltypeKey = $"{i}";
                if (skillconfiguration.SKilDic.ContainsKey(skilltypeKey))
                {
                    skillconfiguration.SKilDic[skilltypeKey].GetSkillInfos__Out(out SkillInfos skillInfos);
                    skill.Find("icon").GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, skillInfos.Icon);
                    skill.Find("icon").GetComponent<Image>().type = Image.Type.Simple;
                    skill.Find("icon").GetComponent<Image>().enabled = true;
                 
                }
                else
                {
                    //skill.Find("icon").GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, "+");
                    skill.Find("icon").GetComponent<Image>().enabled = false;
                }
                //注册事件OnPointerEnterEvent
                skill.GetComponent<UGUITriggerProxy>().OnPointerEnterEvent = delegate
                 {
                     if (curDropSkillinfo is null) return;

                     if (curDropSkillinfo.skillType != (int)skillType) return;

                     curSlotIndex = sortIndex;
                     //判断是否已经装备 该技能
                     for (int i = 0; i < skillconfiguration.SKilDic.Count; i++)
                     {
                         if (skillconfiguration.SKilDic.ElementAt(i).Value == curDropSkillinfo?.skill_ConfigIndex)
                         {
                             Image image = Skills.Find($"skill_{int.Parse(skillconfiguration.SKilDic.ElementAt(i).Key)}").Find("icon").GetComponent<Image>();
                             image.enabled = false;
                            // image.sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, "+");
                             RemoveSkill(skillconfiguration.SKilDic.ElementAt(i).Key);//移除已经添加的技能
                         }
                     }
                     AddSkill(curSlotIndex, curDropSkillinfo.skill_ConfigIndex);//加入到技能栏
                     curDropSkillinfo = null;
                 };
                //点击移除 已装备的技能
                skill.GetComponent<UGUITriggerProxy>().OnPointerClickEvent = delegate
                {
                  //  skill.Find("icon").GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, "+");
                    skill.Find("icon").GetComponent<Image>().enabled=false;
                    RemoveSkill(sortIndex.ToString());
                };

            }
        }
        /// <summary>
        /// 添加技能到技能卡槽
        /// </summary>
        /// <param name="slotIndex">卡槽索引</param>
        /// <param name="skillConfigIndex">技能配置表id</param>
        /// <param name="isSubSkill">是否是辅助技能</param>
        public void AddSkill(int slotIndex, int skillConfigIndex)
        {
          
            skillConfigIndex.GetSkillInfos__Out(out SkillInfos skillInfos);
            Transform skill =skillInfos.skillType==1? Skills.Find($"skill_{slotIndex}"):SubSkills.Find($"skill_{slotIndex}");
            int index = slotIndex;
            skill.Find("icon").GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.skillIconAtlasName, skillInfos.Icon);
            skill.Find("icon").GetComponent<Image>().type = Image.Type.Simple;
            skill.Find("icon").GetComponent<Image>().enabled = true;
            skillconfiguration.SKilDic[$"{index}"] = skillConfigIndex;//key：技能类型_技能卡槽位置 value：杰尼龟配置表ID
            curSlotIndex = -1;
            curSlotType = -1;
        }


        /// <summary>
        /// 移除本地缓存的技能配置信息
        /// </summary>
        /// <param name="slotIndex">技能卡槽</param>
        public void RemoveSkill(string slotIndex)
        {
            if (skillconfiguration.SKilDic.ContainsKey(slotIndex))
            {
                skillconfiguration.SKilDic.Remove(slotIndex);
            }
        }
        /// <summary>
        /// 关闭 技能面板
        /// </summary>
        public void Close()
        {
      
            LocalDataJsonComponent.Instance.SavaData(skillconfiguration, roleEntity.LocalSkillFillName);//保存 技能 配置信息
            UIMainComponent.Instance.LoadSkills();//更新 主界面的技能信息
          
            UIComponent.Instance.Remove(UIType.UIIntroduction);
            UIComponent.Instance.Remove(UIType.UISkill);
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
            uICircularScrollView.Dispose();
            SubSKillUiCircularScrollView.Dispose();
            AssetBundleComponent.Instance.UnloadBundle(dusterMaterResName.StringToAB());
        }
    }
}
