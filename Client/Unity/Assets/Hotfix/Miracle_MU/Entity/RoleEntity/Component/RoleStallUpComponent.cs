using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class RoleStallUpComponentAwake : AwakeSystem<RoleStallUpComponent>
    {
        public override void Awake(RoleStallUpComponent self)
        {
            self.roleEntity = self.GetParent<RoleEntity>();
            self.Topparent = self.roleEntity.Game_Object.transform.Find("TopPoint");
            self.uIUnitEntityHpBarComponent = self.roleEntity.GetComponent<UIUnitEntityHpBarComponent>();
            self.IsStallUp = false;
            self.StallUpItemDic.Clear();
        }
    }

    [ObjectSystem]
    public class RoleStallUpComponentUpdate : UpdateSystem<RoleStallUpComponent>
    {
        public override void Update(RoleStallUpComponent self)
        {
          
            if (self.StallUpNameObj == null) return;
            self.StallUpNameObj.transform.rotation = CameraComponent.Instance.MainCamera.transform.rotation;
        }
    }

    /// <summary>
    /// 玩家 摆摊组件
    /// </summary>
    public class RoleStallUpComponent : Component
    {
        public RoleEntity roleEntity;
        public bool IsStallUp;//是否处于摆摊中
        

        public  string StallUpResName = "UIUnitEntityStallUp";

        public Transform StallUpNameObj;
        public Transform Topparent;

        public Text stallName;

        public UIUnitEntityHpBarComponent uIUnitEntityHpBarComponent;

        public string curStallUpName=string.Empty;//当前摊位的名字

        /// <summary>
        /// 摊位上的物品
        /// </summary>
        public Dictionary<long, KnapsackDataItem> StallUpItemDic = new Dictionary<long, KnapsackDataItem>();

        /// <summary>
        /// 获取摊位上的物品
        /// </summary>
        /// <param name="itemId"></param>
        /// <returns></returns>
        public KnapsackDataItem GetKnapsackDataItem(long itemId)
        {
            StallUpItemDic.TryGetValue(itemId, out KnapsackDataItem knapsack);
            return knapsack;
        }
        public void Init(UIUnitEntityHpBarComponent hp)
        {
            Topparent = roleEntity.Game_Object.transform.Find("TopPoint");
            uIUnitEntityHpBarComponent = hp;
            if (IsStallUp)
            {
                StartStallUp(curStallUpName);
            }
        }
        /// <summary>
        /// 开始摆摊
        /// </summary>
        /// <param name="name">摊位的昵称</param>
        public void StartStallUp(string name)
        {
            curStallUpName = name;

            uIUnitEntityHpBarComponent.Hide();
            StallUpNameObj = StallUpNameObj != null ? StallUpNameObj : ResourcesComponent.Instance.LoadGameObject(StallUpResName.StringToAB(),StallUpResName).transform;
            stallName = stallName != null ? stallName : StallUpNameObj.GetReferenceCollector().GetText("Name");

            StallUpNameObj.gameObject.SetActive(true);
            stallName.text = name;
            StallUpNameObj.SetParent(Topparent);
            StallUpNameObj.localPosition = Vector3.up*.5f;
            IsStallUp = true;
          
        }
        /// <summary>
        /// 关闭其他玩家的摊位
        /// </summary>
        public void CloseOtherStallUp() 
        {
            StallUpNameObj?.gameObject.SetActive(false);
            uIUnitEntityHpBarComponent ??= roleEntity.GetComponent<UIUnitEntityHpBarComponent>();
            uIUnitEntityHpBarComponent.Show();
            IsStallUp = false;
        }

        public override void Dispose()
        {
            if (this.IsDisposed) return;
            base.Dispose();
            if (StallUpNameObj != null)
            {
                ResourcesComponent.Instance.DestoryGameObjectImmediate(StallUpNameObj.gameObject, StallUpResName.StringToAB());
                StallUpNameObj = null;
            }
            IsStallUp = false;
        }


    }
}