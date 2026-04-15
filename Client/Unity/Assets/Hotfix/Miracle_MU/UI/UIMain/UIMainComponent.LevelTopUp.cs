using ETModel;
using ILRuntime.Runtime;
using NPOI.SS.Formula.Functions;
using NUnit.Framework.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMainComponent
    {
        public ReferenceCollector levelTopUpCollider;
        public void InitLevelTopUp()
        {
            TimerComponent.Instance.RegisterTimeCallBack(2000, LevelTopUp);
            void LevelTopUp()
            {
                levelTopUpCollider = ReferenceCollector_Main.GetImage("LevelTopUp").gameObject.GetReferenceCollector();
                for (int i = 0, length = levelTopUpCollider.transform.childCount; i < length; i++)
                {
                    int level = levelTopUpCollider.transform.GetChild(i).name.ToInt32();
                    string value = level.ToString() + ',' + UnitEntityComponent.Instance.LocaRoleUUID.ToString();
                    value += "change1";
                    levelTopUpCollider.transform.GetChild(i).GetComponent<Button>().onClick.AddSingleListener(() =>
                    {
                        UIComponent.Instance.VisibleUI(UIType.UILevelTopUp, level, value);
                    });
                    bool show = IsShow(level, value);
                    levelTopUpCollider.transform.GetChild(i).gameObject.SetActive(show);
                }
            }
        }

        public void SetLevelTopUpShow(int level,bool ishow)
        {
            for (int i = 0, length = levelTopUpCollider.transform.childCount; i < length; i++)
            {
                if(levelTopUpCollider.transform.GetChild(i).name.ToInt32() == level)
                {
                    levelTopUpCollider.transform.GetChild(i).gameObject.SetActive(ishow);
                }
            }
        }

        public bool IsShow(int level,string value)
        {
            if (roleEntity.Level >= level)
            {
                return !(PlayerPrefs.GetInt(value) == 1);
            }
            return false;
        }

    }

}

