using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ETHotfix
{
    public partial class UITaskComponent : IUGUIStatus
    {
        public void OnInVisibility()
        {

        }

        public void OnVisible(object[] data)
        {
            if (data.Length == 0) return;
            curTaskType = (E_TaskType)Enum.Parse(typeof(E_TaskType), data[0].ToString());
            if (data.Length == 2)
            {
                curTaskStatus = (E_TaskStatus)Enum.Parse(typeof(E_TaskStatus), data[1].ToString());
                // ChangeBtnStatus(curTaskStatus);

            }
        }

        public void OnVisible()
        {

        }


    }
}