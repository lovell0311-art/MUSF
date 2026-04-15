using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{   
    /// <summary>
    /// 左下角 显示物品掉落信息
    /// 所获得 经验信息
    /// </summary>
    public partial class UIMainComponent
    {
        public Transform infoContent;
        public Transform item;
        public void Init_InfoMessage()
        {
            infoContent = ReferenceCollector_Main.GetGameObject("InfoMessage").transform;
            item = infoContent.GetChild(0);
            item.gameObject.SetActive(false);
        }
        /// <summary>
        /// 显示提示信息
        /// </summary>
        /// <param name="msg">显示内容</param>

        public void ShowInfo(string msg)
        {
            Transform item;
            if (infoContent.childCount >= 6)
            {
                item = infoContent.GetChild(0);
            }
            else
            {
                item = UnityEngine.GameObject.Instantiate<Transform>(this.item,infoContent,false);
                item.localScale = Vector3.one;
            }
            item.GetComponent<Text>().text = msg;
            item.SetAsLastSibling();
            item.gameObject.SetActive(true);

        }
    }
}