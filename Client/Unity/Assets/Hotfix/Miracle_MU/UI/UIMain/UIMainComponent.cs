using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ETModel;
using UnityEngine.UI;

namespace ETHotfix
{
   
    /// <summary>
    /// Ö÷Ãæ°å
    /// </summary>
    public partial class UIMainComponent : Component
    {
       
        public ReferenceCollector ReferenceCollector_Main;
        public static UIMainComponent Instance;

        public Text roleId;
      
        public void LoadRedDot()
        {
            RedDotManagerComponent.LoadlocalRedDot();
        }
        public void ChangeRoleID() 
        {
            roleId.text = $"{roleEntity.Id}_{GlobalDataManager.EnterZoneID}_{GlobalDataManager.EnterLineID}";
        }

        public override void Dispose()
        {
            if (this.IsDisposed)
                return;
            base.Dispose();
           
          //  Game.EventManager.RemoveEventHandler(EventTypeId.LOCALROLE_DEAD, delegate { HookTog.isOn = false; });
            Game.EventCenter.RemoveEvent(EventTypeId.LOCALROLE_DEAD, SetMask);
            // Game.EventCenter.RemoveEvent(EventTypeId.LOCALROLE_DEAD, SetMask);

            Instance = null;
            SkillDispos();
            ClearMedicine();
            RemoveSceneName();
            CleanHitText();
        }

    }
}
