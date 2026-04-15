using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    /// <summary>
    /// Boss 血条
    /// </summary>
    public partial class UIMainComponent
    {
        GameObject BossHp;
        Image HpImage;
        Text bossName;
        public void InitBossHp()
        {
            BossHp = ReferenceCollector_Main.GetImage("BossHp").gameObject;
            HpImage = BossHp.GetReferenceCollector().GetImage("hp");
            bossName = BossHp.GetReferenceCollector().GetText("bossName");
            BossHp.SetActive(false);
        }
        //显示Boss血条
        public void ShowBossHp(string bossName)
        {
            this.bossName.text = bossName;
            HpImage.fillAmount = 1;
            BossHp.SetActive(true);
        }

        //改变Boss的血量
        public void ChangeBossHp(float value)
        {
            HpImage.fillAmount = value;
        }

        //隐藏Boss血条
        public void HideBossHp() 
        {
         BossHp.SetActive(false);
        }
    }
}