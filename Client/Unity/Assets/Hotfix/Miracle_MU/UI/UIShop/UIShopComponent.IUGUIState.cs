using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    public partial class UIShopComponent : IUGUIStatus
    {
        public void OnInVisibility()
        {

        }

        public void OnVisible(object[] data)
        {
            if (data.Length > 0)
            { 
             topuptype=int.Parse(data[0].ToString());
            }
        }

        public void OnVisible()
        {

        }


    }
}