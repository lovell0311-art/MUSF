using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;


namespace ETHotfix
{
    /// <summary>
    /// 实体属性
    /// </summary>
    public partial class UnitEnityProperty
    {
       /// <summary>
       /// 实体属性字典
       /// </summary>
        public Dictionary<int,long> PropertyDic=new Dictionary<int,long>();

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public long GetProperValue(E_GameProperty property)
        {
            if (PropertyDic.ContainsKey((int)property))
            {
                return PropertyDic[(int)property];
            }
            return 0;
        }
        /// <summary>
        /// 改变属性的值
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public void ChangeProperValue(E_GameProperty property,long value) 
        {
            PropertyDic[(int)property] = value;
        }

        /// <summary>
        /// 设置实体的属性
        /// </summary>
        /// <param name="propertyData"></param>
        public void Set(G2C_BattleKVData propertyData)
        {
            PropertyDic[propertyData.Key] = propertyData.Value;
        }

       

        /// <summary>
        /// 清除属性
        /// </summary>
        public void Clean()
        {
            PropertyDic.Clear();
        }
    }
}