using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ETHotfix
{
    /// <summary>
    /// 检查属性 是否满足条件
    /// </summary>
    public partial class KnapsackDataItem
    {
        /// <summary>
        /// 检查 等级、力量、敏捷、智力、体力、统率 是否满足
        /// </summary>
        /// <returns></returns>
        public bool CheckRequirel() 
        {
            //等级是否满足 
            if (GetProperValue(E_ItemValue.RequireLevel) > roleEntity.Property.GetProperValue(E_GameProperty.Level))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "等级不足");
                return false;
            }
            //力量
            if (GetProperValue(E_ItemValue.RequireStrength) > roleEntity.Property.GetProperValue(E_GameProperty.Property_Strength))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "力量不足");
                return false;
            }
            //敏捷
            if (GetProperValue(E_ItemValue.RequireAgile) > roleEntity.Property.GetProperValue(E_GameProperty.Property_Agility))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "敏捷不足");
                return false;
            }
            //智力
            if (GetProperValue(E_ItemValue.RequireEnergy) > roleEntity.Property.GetProperValue(E_GameProperty.Property_Willpower))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "智力不足");
                return false;
            }
            //体力
            if (GetProperValue(E_ItemValue.RequireVitality) > roleEntity.Property.GetProperValue(E_GameProperty.Property_BoneGas))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "体力不足");
                return false;
            }
            //统率
            if (GetProperValue(E_ItemValue.RequireCommand) > roleEntity.Property.GetProperValue(E_GameProperty.Property_Command))
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "统率不足");
                return false;
            }
            return true;
        }
    }
}
