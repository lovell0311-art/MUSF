using UnityEngine;
using ETModel;
using UnityEngine.UI;
using System.Linq;

namespace ETHotfix
{
    /// <summary>
    /// 药水类型
    /// </summary>
    public enum E_MidicineType 
    {
    HP,
    MP,
    }
    /// <summary>
    /// 药水模块
    /// </summary>
    public partial class UIMainComponent
    {

        Button HpMedicineBtn, MpMedicineBtn;
        public MedicineEntity medicineEntity_Hp, medicineEntity_Mp;

        private static readonly System.Object MedicineLock=new System.Object();
        public void Init_Medicine()
        {
            ReferenceCollector collector = ReferenceCollector_Main.GetGameObject("Medicine").GetReferenceCollector();
            HpMedicineBtn = collector.GetButton("Hp");//生命药水
            MpMedicineBtn = collector.GetButton("Mp");//魔力药水

           // HpMedicineBtn.transform.Find("icon").GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_Medicine, "hp");//设置图标
          //  MpMedicineBtn.transform.Find("icon").GetComponent<Image>().sprite = SpriteUtility.Instance.GetAtlasSprite(AtalsType.UI_Medicine, "mp");//设置图标
            HpMedicineBtn.transform.Find("count").GetComponent<Text>().text = 0.ToString();
            MpMedicineBtn.transform.Find("count").GetComponent<Text>().text = 0.ToString();

            CreatMedicineEntity();
            InitMedicne();


            void InitMedicne()
            {
                var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
                foreach (var item in list)
                {
                    if(KnapsackItemsManager.KnapsackItems.ContainsKey(item.Id)==false) continue;
                    if (KnapsackItemsManager.MedicineHpIdList.Contains(item.ConfigId))
                    {
                        medicineEntity_Hp.Add(item.GetProperValue(E_ItemValue.Quantity), item.UUID);

                    }
                    if (KnapsackItemsManager.MedicineMpIdList.Contains(item.ConfigId))
                    {

                        medicineEntity_Mp.Add(item.GetProperValue(E_ItemValue.Quantity), item.UUID);

                    }
                }
            }

            void CreatMedicineEntity()
            {
                medicineEntity_Hp ??= ComponentFactory.Create<MedicineEntity, Transform,E_MidicineType>(HpMedicineBtn.transform,E_MidicineType.HP);
                medicineEntity_Mp ??= ComponentFactory.Create<MedicineEntity, Transform,E_MidicineType>(MpMedicineBtn.transform,E_MidicineType.MP);
            }

        }



        /// <summary>
        /// 使用或丢弃后 
        /// 更新药水显示
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="isRemove"></param>
        public void ChangeNum(KnapsackDataItem dataItem,bool isRemove=true)
        {
           // lock (MedicineLock)
            {
                //生命药水
                if (KnapsackItemsManager.MedicineHpIdList.Contains(dataItem.ConfigId))
                {
                    medicineEntity_Hp.IsUseSuccessfully = true;
                    if (isRemove)
                    {
                        //从背包中丢弃
                        medicineEntity_Hp.Num -= dataItem.GetProperValue(E_ItemValue.Quantity);
                        //队列中 移除
                        medicineEntity_Hp.Remove(dataItem.Id);
                    }
                    else
                    {
                          Log.DebugWhtie($"数量：medicineEntity_Hp.curMedicineUUID：{medicineEntity_Hp.curMedicineUUID} dataItem.Id：{dataItem.Id}");
                        if (dataItem.Id != medicineEntity_Hp.curMedicineUUID) return;

                        medicineEntity_Hp.Num -= 1;
                    }

                }
                //魔力药水
                else if (KnapsackItemsManager.MedicineMpIdList.Contains(dataItem.ConfigId))
                {
                    medicineEntity_Mp.IsUseSuccessfully = true;
                    if (isRemove)
                    {
                        medicineEntity_Mp.Num -= dataItem.GetProperValue(E_ItemValue.Quantity);
                        medicineEntity_Mp.Remove(dataItem.Id);
                    }
                    else
                    {
                        if (dataItem.Id != medicineEntity_Mp.curMedicineUUID) return;

                        medicineEntity_Mp.Num -= 1;
                    }

                }
            }
        }

        public void AddNum(KnapsackDataItem dataItem, int count)
        {
            //lock (MedicineLock)
            {
                //生命药水
                if (KnapsackItemsManager.MedicineHpIdList.Contains(dataItem.ConfigId))
                {
                    medicineEntity_Hp.Add(count, dataItem.Id);

                }
                //魔力药水
                else if (KnapsackItemsManager.MedicineMpIdList.Contains(dataItem.ConfigId))
                {
                    medicineEntity_Mp.Add(count, dataItem.Id);

                }
            }
        }
      
        public void ClearMedicine()
        {
            if (medicineEntity_Hp != null)
                medicineEntity_Hp?.Dispose();
            if (medicineEntity_Mp != null)
                medicineEntity_Mp?.Dispose();
        }
    }
}
