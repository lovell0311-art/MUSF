using ETModel;
using NPOI.Util;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMainComponent
    {
        public ReferenceCollector SynthesisRefer;
        public Button QiangHua, OpenBtn, CloseBtn;
        public GameObject SynthesisMerger,BG, BtnListSystem;
        public Toggle QiangHuaToggle, BagToggle;
        public void InitSynthesis()
        {
            SynthesisRefer = ReferenceCollector_Main.GetGameObject("MergerBtns").gameObject.GetReferenceCollector();
            QiangHua = SynthesisRefer.GetButton("qianghua");
            BG = SynthesisRefer.GetImage("BG").gameObject;
            OpenBtn = SynthesisRefer.GetButton("OpenBtn");
            CloseBtn = SynthesisRefer.GetButton("CloseBtn");
            //CloseBtn.gameObject.SetActive(true);
            //BG.gameObject.SetActive(true);
            //OpenBtn.gameObject.SetActive(true);
            SynthesisMerger = SynthesisRefer.GetGameObject("SynthesisMerger");
            SynthesisMerger.gameObject.SetActive(false);
            //SynthesisRefer.gameObject.SetActive(false);

            QiangHuaToggle = SynthesisRefer.GetToggle("QiangHuaToggle");
            BagToggle = SynthesisRefer.GetToggle("BagToggle");
            BtnListSystem = SynthesisRefer.GetImage("BtnList").gameObject;
            BtnListSystem.SetActive(true);
            SynthesisBtnMerger();
        }

        public void SynthesisBtnMerger()
        {
            /*
            CloseBtn.onClick.AddSingleListener(() =>
            {
                CloseBtn.gameObject.SetActive(false);
                OpenBtn.gameObject.SetActive(true);
                BG.gameObject.SetActive(false);
                SynthesisMerger.gameObject.SetActive(false);
            });
            OpenBtn.onClick.AddSingleListener(() =>
            {
                CloseBtn.gameObject.SetActive(true);
                BG.gameObject.SetActive(true);
                OpenBtn.gameObject.SetActive(false);
                SynthesisMerger.gameObject.SetActive(true);
            });*/

            QiangHuaToggle.onValueChanged.AddSingleListener(OnQiangHuaToggleChanged);
            BagToggle.onValueChanged.AddSingleListener(OnBagToggleChanged);

            QiangHua.onClick.AddSingleListener(OpenStrengthenSynthesis);
            SynthesisRefer.GetButton("zuoqi").onClick.AddSingleListener(OpenMountSynthesis);
            SynthesisRefer.GetButton("zhuoyue").onClick.AddSingleListener(OpenExcellentRandomSynthesis);
            SynthesisRefer.GetButton("xiangqian").onClick.AddSingleListener(OpenInlaySynthesis);
            SynthesisRefer.GetButton("hecheng").onClick.AddSingleListener(OpenNormalSynthesis);
            SynthesisRefer.GetButton("fenjie").onClick.AddSingleListener(OpenMoJingShiSynthesis);
            SynthesisRefer.GetButton("chibang").onClick.AddSingleListener(OpenWingSynthesis);
            SynthesisRefer.GetButton("jinjie").onClick.AddSingleListener(OpenDarkAdvanceSynthesis);
        }

        private void OnQiangHuaToggleChanged(bool isOn)
        {
            SynthesisMerger.gameObject.SetActive(isOn);
        }

        private void OnBagToggleChanged(bool isOn)
        {
            BtnListSystem.SetActive(isOn);
            if (isOn)
            {
                QiangHuaToggle.isOn = false;
                if (SynthesisMerger.gameObject.activeSelf)
                {
                    SynthesisMerger.gameObject.SetActive(false);
                }
            }
        }

        private void OpenStrengthenSynthesis()
        {
            UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge, SynthesisData.ZhuangBeiQiangHua);
        }

        private void OpenMountSynthesis()
        {
            UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge, SynthesisData.ZuoQiSynthesis);
        }

        private void OpenExcellentRandomSynthesis()
        {
            UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge, SynthesisData.ZhuoYueAttributesRandom);
        }

        private void OpenInlaySynthesis()
        {
            UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Inlay);
        }

        private void OpenNormalSynthesis()
        {
            UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge, SynthesisData.YiBanSynthesis);
        }

        private void OpenMoJingShiSynthesis()
        {
            UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge, SynthesisData.MoJingShi);
        }

        private void OpenWingSynthesis()
        {
            UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge, SynthesisData.CiBangSynthesis);
        }

        private void OpenDarkAdvanceSynthesis()
        {
            UIComponent.Instance.VisibleUI(UIType.UIKnapsack, E_KnapsackState.KS_Gem_Merge, SynthesisData.YouAnYiBanSynthesis);
        }
    }

}
