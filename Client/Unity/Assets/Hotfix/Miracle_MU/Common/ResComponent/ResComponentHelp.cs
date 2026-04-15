using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
namespace ETHotfix
{
    public static class ResComponentHelp
    {
        /// <summary>
        /// 根据等级加载对应的装备
        /// </summary>
        /// <param name="infoConfig"></param>
        /// <param name="levle"></param>
        /// <returns></returns>
        public static GameObject LoadEquipMent(Item_infoConfig infoConfig,int levle)
        {
            if (string.IsNullOrEmpty(infoConfig.ResName))
            {
                Log.DebugRed($"{infoConfig.Name} 的模型资源为空");
                return null;
            }
            string resName = infoConfig.ResName;

            GameObject obj = ResourcesComponent.Instance.LoadGameObject(resName.StringToAB(),resName);
            return obj;
        }
    }
}
