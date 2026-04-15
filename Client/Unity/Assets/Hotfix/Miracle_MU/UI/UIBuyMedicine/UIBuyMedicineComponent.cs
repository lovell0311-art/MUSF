using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{

    /// <summary>
    /// 药瓶大小 枚举
    /// </summary>
    public enum E_MedicineType 
    {
     Small,
     Medium,
     Large,
    }

    /// <summary>
    /// 药瓶类型 
    /// </summary>
    public enum E_Medicine 
    {
     HP,
     MP
    }

    [ObjectSystem]
    public class UIBuyMedicineComponentAwake : AwakeSystem<UIBuyMedicineComponent>
    {
        public override void Awake(UIBuyMedicineComponent self)
        {
            self.collector = self.GetParent<UI>().GameObject.GetReferenceCollector();
            self.GetParent<UI>().GameObject.GetComponent<Canvas>().planeDistance = 80;
            self.collector.GetButton("Panel").onClick.AddSingleListener(self.Close);
            self.collector.GetToggle("SmallToggle").onValueChanged.AddSingleListener((value) => { self.ChangeMedicineType(value,E_MedicineType.Small); });
            self.collector.GetToggle("MiddleToggle").onValueChanged.AddSingleListener((value) => { self.ChangeMedicineType(value,E_MedicineType.Medium); });
            self.collector.GetToggle("BigToggle").onValueChanged.AddSingleListener((value) => { self.ChangeMedicineType(value,E_MedicineType.Large); });
            self.InitBuyBtns();
        }
    }

    /// <summary>
    /// 少女安娜 买药
    /// </summary>
    public class UIBuyMedicineComponent : Component
    {
        public ReferenceCollector collector;

        public E_MedicineType curMedicineType = E_MedicineType.Small;

        //关闭
        public void Close()
        {
            UIComponent.Instance.Remove(UIType.UIBuyMedicine);
        }

        public void ChangeMedicineType(bool value,E_MedicineType medicineType)
        {
            if (value == false) return;
            curMedicineType = medicineType;
        }

       public void InitBuyBtns() 
        {
            Transform btns = collector.GetGameObject("Btns").transform;
            for (int i = 0, length=btns.childCount; i < length; i++)
            {
                Button button=btns.GetChild(i).GetComponent<Button>();
                var str = button.name.Split('_');
                int count= int.Parse(System.Text.RegularExpressions.Regex.Replace(str[1], @"[^0-9]+", ""));
                E_Medicine medicine = str[0] == "HP" ? E_Medicine.HP: E_Medicine.MP;
                button.onClick.AddSingleListener(()=> 
                {
                    UIConfirmComponent uIConfirm = UIConfirmComponentExtend.GetUIConfirmComponent();
                    uIConfirm.SetTipText($"确定购买<color=red>{GetTip(medicine,count)}</color>?");
                    uIConfirm.AddActionEvent(async () => 
                    {
                        G2C_QuickPurchaseResponse g2C_Quick = (G2C_QuickPurchaseResponse)await SessionComponent.Instance.Session.Call(new C2G_QuickPurchaseRequest
                        {
                            Cnt = count,
                            ItemConfigId = GetConfigId(curMedicineType, medicine)
                        });
                        if (g2C_Quick.Error != 0)
                        {
                            UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_Quick.Error.GetTipInfo());
                        }
                    });
                   
                });

            }
        }

        public string GetTip(E_Medicine medicine,int count)
        {
            
            return GetStr(curMedicineType,medicine);


            string GetStr(E_MedicineType medicineType, E_Medicine medicine) => (medicineType, medicine) switch
            {
                (E_MedicineType.Small, E_Medicine.HP) => $"小瓶治疗药水{count}组",
                (E_MedicineType.Medium, E_Medicine.HP) => $"中瓶治疗药水{count}组",
                (E_MedicineType.Large, E_Medicine.HP) => $"大瓶治疗药水{count}组",
                (E_MedicineType.Small, E_Medicine.MP) => $"小瓶魔力药水{count}组",
                (E_MedicineType.Medium, E_Medicine.MP) => $"中瓶魔力药水{count}组",
                (E_MedicineType.Large, E_Medicine.MP) => $"大瓶魔力药水{count}组",
            };
        }

        /// <summary>
        /// 获取 对应药瓶的ConfigId
        /// </summary>
        /// <param name="medicineType"></param>
        /// <param name="medicine"></param>
        /// <returns></returns>
        int GetConfigId(E_MedicineType medicineType, E_Medicine medicine) =>(medicineType,medicine) switch
        {
         (E_MedicineType.Small,E_Medicine.HP) => 310002,
            (E_MedicineType.Medium, E_Medicine.HP) => 310003,
            (E_MedicineType.Large, E_Medicine.HP) => 310004,
            (E_MedicineType.Small, E_Medicine.MP) => 310005,
            (E_MedicineType.Medium, E_Medicine.MP) => 310006,
            (E_MedicineType.Large, E_Medicine.MP) => 310007,
        };

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
        }



    }
}
