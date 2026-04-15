using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIOnHookSetComponentAwake : AwakeSystem<UIOnHookSetComponent>
    {
        public override void Awake(UIOnHookSetComponent self)
        {
            self.Awake();
        }
    }


    /// <summary>
    /// 挂机设置组件
    /// </summary>
    public class UIOnHookSetComponent : Component
    {
        Transform Togs;
        GameObject panel_0, panel_1, panel_2;
        ReferenceCollector reference_panel_0, reference_panel_1, reference_panel_2;
        Toggle IsAuto_30, IsAuto_50, IsAuto_80, IsReturnOrigin, IsOriginOnHook,IsTargetMonster, IsUseAttack, IsUseSkill, IsPickUpLuck, IsPickUpSkill;
        Toggle IsDiyPickUp, IsPickUpAllEquip, IsPickUpGem, IsPickUpMountMat, IsPickUpSuitEquip, IsPickUpYuMao;
        Toggle IsAcceptTeam, IsAutoAcceptTeam;
        Text value;
        Slider rangslider;
        InputField diyInputField;

        GameObject BufferCdObj;
        Toggle IsAutoBufferCd_10, IsAutoBufferCd_20, IsAutoBufferCd_30;


        public void Awake()
        {
            ReferenceCollector referenceCollector = GetParent<UI>().GameObject.GetReferenceCollector();
            referenceCollector.GetButton("saveBtn").onClick.AddSingleListener(SaveClose);
            referenceCollector.GetButton("resterBtn").onClick.AddSingleListener(ResetInfo);
            Togs = referenceCollector.GetGameObject("Togs").transform;
            panel_0 = referenceCollector.GetGameObject("panel_0");
            panel_1 = referenceCollector.GetGameObject("panel_1");
            panel_2 = referenceCollector.GetGameObject("panel_2");
            reference_panel_0 = panel_0.GetReferenceCollector();
            reference_panel_1 = panel_1.GetReferenceCollector();
            reference_panel_2 = panel_2.GetReferenceCollector();
            //panel-0
            IsAuto_30 = reference_panel_0.GetToggle("IsAuto_30");
            IsAuto_50 = reference_panel_0.GetToggle("IsAuto_50");
            IsAuto_80 = reference_panel_0.GetToggle("IsAuto_80");
            IsReturnOrigin = reference_panel_0.GetToggle("IsReturnOrigin");
            IsTargetMonster = reference_panel_0.GetToggle("IsTargetMonster");
            IsOriginOnHook = reference_panel_0.GetToggle("IsOriginOnHook");
            IsUseAttack = reference_panel_0.GetToggle("IsUseAttack");
            IsUseSkill = reference_panel_0.GetToggle("IsUseSkill");
            value = reference_panel_0.GetText("value");
            rangslider = reference_panel_0.GetGameObject("Slider").GetComponent<Slider>();
            rangslider.onValueChanged.AddSingleListener(ChangeValue);
            //BufferCd
            BufferCdObj = reference_panel_0.GetText("AutoUseBuffer").gameObject;
            IsAutoBufferCd_10 = BufferCdObj.transform.Find("IsAutoBuffer_10").GetComponent<Toggle>();
            IsAutoBufferCd_20 = BufferCdObj.transform.Find("IsAutoBuffer_20").GetComponent<Toggle>();
            IsAutoBufferCd_30 = BufferCdObj.transform.Find("IsAutoBuffer_30").GetComponent<Toggle>();
            BufferCdObj.SetActive(UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Archer);//弓箭手 才显示bufferCd
            //panel-1
            IsDiyPickUp = reference_panel_1.GetToggle("IsDiyPickUp");
            IsPickUpAllEquip = reference_panel_1.GetToggle("IsPickUpAllEquip");
            IsPickUpGem = reference_panel_1.GetToggle("IsPickUpGem");
            IsPickUpMountMat = reference_panel_1.GetToggle("IsPickUpMountMat");
            IsPickUpSuitEquip = reference_panel_1.GetToggle("IsPickUpSuitEquip");
            IsPickUpYuMao = reference_panel_1.GetToggle("IsPickUpYuMao");
            IsPickUpSkill = reference_panel_1.GetToggle("IsPickUpSkill");
            IsPickUpLuck = reference_panel_1.GetToggle("IsPickUpLuck");
            diyInputField = reference_panel_1.GetInputField("DiyInputField");


            //panel-2
            IsAcceptTeam = reference_panel_2.GetToggle("IsAcceptTeam");
            IsAutoAcceptTeam = reference_panel_2.GetToggle("IsAutoAcceptTeam");
            InitTogs();

            InitValue();
            AddEvent();
            referenceCollector.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIOnHookSet));
        }

        public void AddEvent() 
        {
            IsAuto_30.onValueChanged.AddSingleListener((value) => { OnHookSetInfoTools.IsAuto_30 = value; });
            IsAuto_50.onValueChanged.AddSingleListener((value) => { OnHookSetInfoTools.IsAuto_50 = value; });
            IsAuto_80.onValueChanged.AddSingleListener((value) => { OnHookSetInfoTools.IsAuto_80 = value; });

            IsAutoBufferCd_10.onValueChanged.AddSingleListener((value) => OnHookSetInfoTools.IsAutoBufferCd_10 = value) ;
            IsAutoBufferCd_20.onValueChanged.AddSingleListener((value) => OnHookSetInfoTools.IsAutoBufferCd_20 = value) ;
            IsAutoBufferCd_30.onValueChanged.AddSingleListener((value) => OnHookSetInfoTools.IsAutoBufferCd_30 = value) ;

            IsReturnOrigin.onValueChanged.AddSingleListener((value) =>OnHookSetInfoTools.IsReturnOrigin = value);
            IsOriginOnHook.onValueChanged.AddSingleListener((value) =>OnHookSetInfoTools.IsOriginOnHook = value); 
            IsTargetMonster.onValueChanged.AddSingleListener((value) => OnHookSetInfoTools.IsTargetMonster = value);
            IsUseAttack.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsUseAttack=value);
            IsUseSkill.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsUseSkill = value);


            IsDiyPickUp.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsDiyPicUp = value);
            IsPickUpAllEquip.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsPickUpAllEquip = value);
            IsPickUpGem.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsPickUpGems = value);
            IsPickUpSuitEquip.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsPickUpSuitEquip = value);
            IsPickUpYuMao.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsPickUpYuMao = value);
            IsPickUpMountMat.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsPickUpMountMat = value);
            IsPickUpLuck.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsPickUpLuck = value);
            IsPickUpSkill.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsPickUpSkill = value);

            diyInputField.onEndEdit.AddSingleListener(value=>OnHookSetInfoTools.DiyPickUpName=value);

            IsAcceptTeam.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsAutoAccpet=value);
            IsAutoAcceptTeam.onValueChanged.AddSingleListener(value=>OnHookSetInfoTools.IsAutoAccpetTeam = value);
        }
        public void InitValue()
        {
            IsAuto_30.isOn = OnHookSetInfoTools.IsAuto_30;
            IsAuto_50.isOn = OnHookSetInfoTools.IsAuto_50;
            IsAuto_80.isOn = OnHookSetInfoTools.IsAuto_80;

            IsAutoBufferCd_10.isOn = OnHookSetInfoTools.IsAutoBufferCd_10;
            IsAutoBufferCd_20.isOn = OnHookSetInfoTools.IsAutoBufferCd_20;
            IsAutoBufferCd_30.isOn = OnHookSetInfoTools.IsAutoBufferCd_30;

            IsReturnOrigin.isOn = OnHookSetInfoTools.IsReturnOrigin;
            IsOriginOnHook.isOn = OnHookSetInfoTools.IsOriginOnHook;
            IsTargetMonster.isOn = OnHookSetInfoTools.IsTargetMonster;
            IsUseAttack.isOn = OnHookSetInfoTools.IsUseAttack;
            IsUseSkill.isOn = OnHookSetInfoTools.IsUseSkill;
            value.text = OnHookSetInfoTools.Range.ToString();
            rangslider.value = OnHookSetInfoTools.Range;

            IsDiyPickUp.isOn = OnHookSetInfoTools.IsDiyPicUp;
            IsPickUpAllEquip.isOn = OnHookSetInfoTools.IsPickUpAllEquip;
            IsPickUpGem.isOn = OnHookSetInfoTools.IsPickUpGems;
            IsPickUpSuitEquip.isOn = OnHookSetInfoTools.IsPickUpSuitEquip;
            IsPickUpYuMao.isOn = OnHookSetInfoTools.IsPickUpYuMao;
            IsPickUpMountMat.isOn = OnHookSetInfoTools.IsPickUpMountMat;
            IsPickUpLuck.isOn = OnHookSetInfoTools.IsPickUpLuck;
            IsPickUpSkill.isOn = OnHookSetInfoTools.IsPickUpSkill;
            diyInputField.text = OnHookSetInfoTools.DiyPickUpName;
          
            IsAcceptTeam.isOn = OnHookSetInfoTools.IsAutoAccpet;
            IsAutoAcceptTeam.isOn = OnHookSetInfoTools.IsAutoAccpetTeam;
        }
        public void InitTogs()
        {
            int length = Togs.childCount;
            for (int i = 0; i < length; i++)
            {
                int index = i;
                Toggle toggle = Togs.GetChild(i).GetComponent<Toggle>();
                toggle.onValueChanged.AddSingleListener((value) => { OnToggleEvent(index); });
            }
        }
        public void ChangeValue(float value)
        {
            this.value.text = value.ToString();
        }

        public void OnToggleEvent(int index)
        {
            panel_0.SetActive(index == 0);
            panel_1.SetActive(index == 1);
            panel_2.SetActive(index == 2);
        }
        /// <summary>
        /// 恢复默认状态
        /// </summary>
        public void ResetInfo()
        {
            InitValue();
        }
        /// <summary>
        /// 保存当前设置面板中的数据
        /// </summary>
        public void ChangeInfoData()
        {
           
            OnHookSetInfoTools.IsAuto_30 = IsAuto_30.isOn;
            OnHookSetInfoTools.IsAuto_50 = IsAuto_50.isOn;
            OnHookSetInfoTools.IsAuto_80 = IsAuto_80.isOn;
            OnHookSetInfoTools.IsAutoBufferCd_10 = IsAutoBufferCd_10.isOn;
            OnHookSetInfoTools.IsAutoBufferCd_20 = IsAutoBufferCd_20.isOn;
            OnHookSetInfoTools.IsAutoBufferCd_30 = IsAutoBufferCd_30.isOn;

            OnHookSetInfoTools.IsReturnOrigin = IsReturnOrigin.isOn;
            OnHookSetInfoTools.IsOriginOnHook = IsOriginOnHook.isOn;
            OnHookSetInfoTools.IsTargetMonster = IsTargetMonster.isOn;
            OnHookSetInfoTools.IsUseAttack = IsUseAttack.isOn;
            OnHookSetInfoTools.IsUseSkill = IsUseSkill.isOn;
            OnHookSetInfoTools.Range = int.Parse(value.text);


            OnHookSetInfoTools.IsDiyPicUp = IsDiyPickUp.isOn;
            OnHookSetInfoTools.IsPickUpAllEquip = IsPickUpAllEquip.isOn;
            OnHookSetInfoTools.IsPickUpGems = IsPickUpGem.isOn;
            OnHookSetInfoTools.IsPickUpSuitEquip = IsPickUpSuitEquip.isOn;
            OnHookSetInfoTools.IsPickUpYuMao = IsPickUpYuMao.isOn;
            OnHookSetInfoTools.IsPickUpMountMat = IsPickUpMountMat.isOn;
            OnHookSetInfoTools.IsPickUpLuck = IsPickUpLuck.isOn;
            OnHookSetInfoTools.IsPickUpSkill = IsPickUpSkill.isOn;
            OnHookSetInfoTools.DiyPickUpName = diyInputField.textComponent.text;

            OnHookSetInfoTools.IsAutoAccpet = IsAcceptTeam.isOn;
            OnHookSetInfoTools.IsAutoAccpetTeam = IsAutoAcceptTeam.isOn;

           
          
        }
        public void SaveClose()
        {
           // ChangeInfoData();
            OnHookSetInfoTools.Save();
            UIComponent.Instance.Remove(UIType.UIOnHookSet);
         
        }
    }
}
