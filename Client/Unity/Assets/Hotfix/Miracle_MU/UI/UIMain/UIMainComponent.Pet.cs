using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIMainComponent 
    {
        ReferenceCollector referenceCollector_Pet;
        public GameObject petBG1;
        public GameObject petMp, petHp;
        public Text PetName;
        public Text DeadTxt;
        public Button PutBtnOpen, PutBtnClose;

        public Image petMp_, petHp_, petIcon;
        public void Init_Pet()
        {
            referenceCollector_Pet = ReferenceCollector_Main.GetImage("PetHpMpvalue").gameObject.GetReferenceCollector();

            petMp_ = referenceCollector_Pet.GetImage("Mp");
            petHp_ = referenceCollector_Pet.GetImage("Mp");
            petIcon = referenceCollector_Pet.GetImage("Icon");
           /* PutBtnOpen = referenceCollector_Pet.GetButton("PutBtnOpen");
            PutBtnClose = referenceCollector_Pet.GetButton("PutBtnClose");
            petBG1 = referenceCollector_Pet.GetImage("BG1").gameObject;
            petMp = referenceCollector_Pet.GetImage("Mp").gameObject;
            petHp = referenceCollector_Pet.GetImage("Hp").gameObject;
            PetName = referenceCollector_Pet.GetText("PetName");
            DeadTxt = referenceCollector_Pet.GetText("DeadTxt");*/

            /* PutBtnOpen.onClick.AddSingleListener(delegate ()
             {
                 petBG1.SetActive(true);
                 PetName.gameObject.SetActive(true);
                 petMp.SetActive(true);
                 petHp.SetActive(true);

                 PutBtnOpen.gameObject.SetActive(false);
                 PutBtnClose.gameObject.SetActive(true);
             });
             PutBtnClose.onClick.AddSingleListener(delegate ()
             {
                 petBG1.SetActive(false);
                // PetName.gameObject.SetActive(false);
                 petMp.SetActive(false);
                 petHp.SetActive(false);

                 PutBtnOpen.gameObject.SetActive(true);
                 PutBtnClose.gameObject.SetActive(false);
             });*/
            referenceCollector_Pet.gameObject.SetActive(false);

          
        }

        public void SetPetHpMpValue(int isDead,string petName = "³èÎï",float curHp = 0f, float maxHp = 0f, float curMp = 0f, float maxMp = 0f)
        {
           // ShowPetPanle();

           // petMp_.fillAmount = curMp / maxMp;
           // petHp_.fillAmount = curHp / maxHp;

        }
        public void SetPetHpValue(float curHp = 0f, float maxHp = 0f)
        {
          //  ShowPetPanle();
          //  petHp_.fillAmount = curHp / maxHp;
          
        }

        public void SetPetMpValue(float curMp = 0f, float maxMp = 0f)
        {
          //  ShowPetPanle();
          //  petMp_.fillAmount = curMp / maxMp;
        }
        
        public async ETVoid GetWarPetData()
        {
            
            G2C_AttributeChangeResponse g2C_AttributeChange = (G2C_AttributeChangeResponse)await SessionComponent.Instance.Session.Call(new C2G_AttributeChangeRequest());
        }

        public void ShowPetPanle()
        {
           // if (referenceCollector_Pet.gameObject.activeSelf) return;
           // referenceCollector_Pet.gameObject.SetActive(true);
        }
        public void HidePetPanle()
        {
           // if (!referenceCollector_Pet.gameObject.activeSelf) return;
           // referenceCollector_Pet.gameObject.SetActive(false);
        }
    }

}
