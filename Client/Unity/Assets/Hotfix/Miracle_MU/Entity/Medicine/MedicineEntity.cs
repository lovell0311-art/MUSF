using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ETModel;
using System;
using System.Linq;



namespace ETHotfix
{
    [ObjectSystem]
    public class MedicineEntityAwake : AwakeSystem<MedicineEntity, Transform, E_MidicineType>
    {
        public override void Awake(MedicineEntity self, Transform transform, E_MidicineType medicinetype)
        {
            self.Awake(transform, medicinetype);
        }
    }
    // [ObjectSystem]
    public class MedicineEntityUpdate : UpdateSystem<MedicineEntity>
    {
        public override void Update(MedicineEntity self)
        {
            if (!self.IsMasking) return;
            //等待CD时间
            self.curTime += Time.deltaTime;
            self.mask.fillAmount = 1 - (self.curTime / self.CdTime);
            self.cdTxt.text = Math.Round(self.CdTime - self.curTime, 1).ToString();
            if (self.curTime >= self.CdTime)
            {
                self.cdTxt.text = string.Empty;
                self.curTime = 0;
                self.IsMasking = false;
                self.mask.fillAmount = 0;
                self.button.interactable = true;
            }

        }
    }
    /// <summary>
    /// 药水实体
    /// </summary>
    public class MedicineEntity : Entity
    {
        public Transform medicineBtn;
        public Button button;

        public Image mask;//CD遮罩
        public Text cdTxt;//cd时间
        public bool IsMasking;//是否处于Cd中
        private int num;//药品的数量
        public Text countTxt;


        public List<long> MedicineList = new List<long>();

        //是否使用成功
        public bool IsUseSuccessfully = false;

        public int Num
        {
            get
            {
                return num;
            }
            set
            {
                num = value;
                if (num <= 0) num = 0;
                countTxt.text = num > 99 ? "99" : num.ToString();

            }
        }

        public E_MidicineType midicineType;
        public float curTime = 0;

        public float CdTime = .2f;//Cd时间
        public RoleEntity RoleEntity => UnitEntityComponent.Instance.LocalRole;

        public long curMedicineUUID;

        public void Awake(Transform ts, E_MidicineType medicinetype)
        {
            medicineBtn = ts;
            IsMasking = false;
            this.midicineType = medicinetype;
            mask = medicineBtn.Find("mask").GetComponent<Image>();//cd遮罩
            cdTxt = medicineBtn.Find("Text").GetComponent<Text>();//cd
            cdTxt.text = string.Empty;
            countTxt = medicineBtn.Find("count").GetComponent<Text>();
            countTxt.text = Num.ToString();//设置数量
            IsUseSuccessfully = true;
            button = medicineBtn.GetComponent<Button>();
            button.onClick.AddSingleListener(UserMedicine);
        }

        /// <summary>
        /// 移除 药品
        /// </summary>
        /// <param name="uuid"></param>
        public void Remove(long uuid)
        {
            if (MedicineList.Contains(uuid))
            {
                MedicineList.Remove(uuid);
            }
        }
        /// <summary>
        /// 添加药品
        /// </summary>
        /// <param name="count"></param>
        /// <param name="uuid"></param>
        public void Add(int count, long uuid)
        {
            Num += count;
            if (MedicineList.Contains(uuid) == false)
            {
                MedicineList.Add(uuid);
            }
        }

        public void ChangeNumTxt(int num)
        {
            if (num <= 0) num = 0;
            countTxt.text = num > 99 ? "99" : num.ToString();
            //  Log.DebugRed($"改变数量：{num}  Count:{MedicineList.Count}");
        }

        /// <summary>
        /// 使用药品
        /// </summary>
        public void UserMedicine()
        {

            //请求使用药水
            if (Num == 0 || IsUseSuccessfully == false || this.MedicineList.Count == 0) //数量为0 或者上一次 还未使用成功
            {
                //UIComponent.Instance.VisibleUI(UIType.UIHint, "药品不足 请您去商城购买药品");
                if (this.MedicineList.Count == 0)
                {
                    Num = 0;
                }
                return;
            }
            if (this.midicineType == E_MidicineType.HP && RoleEntity.Property.GetProperValue(E_GameProperty.PROP_HP) == RoleEntity.Property.GetProperValue(E_GameProperty.PROP_HP_MAX))
            {

                return;
            }
            if (this.midicineType == E_MidicineType.MP && RoleEntity.Property.GetProperValue(E_GameProperty.PROP_MP) == RoleEntity.Property.GetProperValue(E_GameProperty.PROP_MP_MAX))
            {

                return;
            }

            UseMedicineAsync().Coroutine();

            async ETVoid UseMedicineAsync()
            {
                //设置数量
                IsUseSuccessfully = false;

                try
                {
                    curMedicineUUID = MedicineList.Last();
                    G2C_PlayerUseItemInTheBackpack g2C_PlayerUseItemIn = (G2C_PlayerUseItemInTheBackpack)await SessionComponent.Instance.Session.Call(new C2G_PlayerUseItemInTheBackpack
                    {
                        ItemUUID = curMedicineUUID
                    }); ;
                    if (g2C_PlayerUseItemIn.Error != 0)
                    {
                        IsUseSuccessfully = true;
                        UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_PlayerUseItemIn.Error.GetTipInfo());
                    }
                    else
                    {
                        IsUseSuccessfully = true;
                    }
                }
                finally
                {
                    IsUseSuccessfully = true;
                }
              
            }
        }
        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            IsUseSuccessfully = true;
            base.Dispose();

        }
    }
}