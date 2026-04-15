using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;
using UnityEngine.UI;
using ILRuntime.Runtime;

namespace ETHotfix
{
    /// <summary>
    /// 荧光宝石抽取
    /// </summary>
    public partial class UIKnapsackComponent
    {
        ReferenceCollector collector_YingGuangBaoShiChouQu;
        readonly int Glod_ChouQu = 1_000_000;
        readonly int Success_ChouQu = 90;

        Transform Atrs;
        ToggleGroup AtrtoggleGroup;
        public void Init_YingGuangBaoShiChouQu()
        {
            collector_YingGuangBaoShiChouQu = YingGuangBaoShiChouQuPanel.GetReferenceCollector();
            InitItemGrid(collector_YingGuangBaoShiChouQu, 4, E_InlayType.YingGuangBaoShiChouQu, Glod_ChouQu, Success_ChouQu);

            Atrs = collector_YingGuangBaoShiChouQu.GetGameObject("Atr").transform;
            InitAtr();

            ///初始化镶嵌属性条数
            void InitAtr()
            {
                AtrtoggleGroup = Atrs.GetComponent<ToggleGroup>();
                AtrtoggleGroup.allowSwitchOff = true;
                for (int i = 0, length = Atrs.childCount; i < length; i++)
                {
                    Toggle toggle = Atrs.GetChild(i).GetComponent<Toggle>();
                    int index = i+1;
                    toggle.GetComponentInChildren<Text>().text = $"镶宝{index}：";
                    toggle.isOn = false;
                    toggle.interactable = false;
                    toggle.onValueChanged.AddSingleListener(value => 
                    {
                        if (!value) return;

                        if(AtrtoggleGroup.allowSwitchOff) AtrtoggleGroup.allowSwitchOff = false;

                        if (InlayItemDic.TryGetValue(1,out List<long> ItemList))
                        {
                            if (ItemList.Count == 2)
                            {
                                ItemList[1] = index;
                            }
                            else
                            {
                                ItemList.Add(index);
                            }
                        }
                    });
                   
                }
            }
        }
        /// <summary>
        /// 重置 装备的镶嵌属性
        /// </summary>
        void ResetAtrs() 
        {
            AtrtoggleGroup.allowSwitchOff = true;
            for (int i = 0, length = Atrs.childCount; i < length; i++)
            {
                Toggle toggle = Atrs.GetChild(i).GetComponent<Toggle>();
                toggle.GetComponentInChildren<Text>().text = $"镶宝{i+1}：";
                toggle.isOn=false;
                toggle.interactable = false;
            }
        }
        /// <summary>
        /// 设置卡槽的荧光宝石属性
        /// </summary>
        /// <param name="index"></param>
        /// <param name="PropId"></param>
        void SetEquipAtr(KnapsackDataItem dataItem,int index,long PropId)
        {
            //id:value/100  level:value%100
            FluoreSet_AttrConfig fluoreSet_AttrConfig = ConfigComponent.Instance.GetItem<FluoreSet_AttrConfig>((PropId / 100).ToInt32());//获取荧光宝石 属性配置表
            Atrs.GetChild(index).GetComponentInChildren<Text>().text = $"镶宝{index + 1}:{dataItem.GetYingGuangBaoShiAtr(fluoreSet_AttrConfig.fluore)}({string.Format(fluoreSet_AttrConfig.Info, dataItem.GetAtrValue(fluoreSet_AttrConfig, (PropId % 100).ToInt32()))})";
            Atrs.GetChild(index).GetComponent<Toggle>().interactable = true;
        }
        

    }
    
}
