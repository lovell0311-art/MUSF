using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// 好易充值
    /// </summary>
    public partial class UIShopComponent
    {
        public void Init_TopUp_HaoYi()
        {
            Transform TopUps = collector.GetGameObject("TopUps").transform;
            TopUps.gameObject.SetActive(false);

            Button urlBrn = collector.GetButton("TopUp_Url");
            urlBrn.gameObject.SetActive(true);
            urlBrn.onClick.AddSingleListener(() => 
            {
                //开好易充值
                GlobalDataManager.HaoYiTopUp();
            });
            }
        }
}
