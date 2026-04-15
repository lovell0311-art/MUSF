using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    [ObjectSystem]
    public class UIMainComponentUpdate : UpdateSystem<UIMainComponent>
    {
        public override void Update(UIMainComponent self)
        {
            if (self.mapid != 102 && self.mapid != 112)
            {
                return;
            }

            self.stop += Time.deltaTime;
            if (self.stop < 1f)
            {
                return;
            }

            self.stop = 0f;
            self.chargetime += 1f;

            if (self.IconCollector == null)
            {
                return;
            }

            Image display = self.IconCollector.GetImage("display");
            if (display == null || display.transform.childCount == 0)
            {
                return;
            }

            Text timeTxt = display.transform.GetChild(0).GetComponent<Text>();
            if (timeTxt != null)
            {
                timeTxt.text = TimeHelper.ConvertSecondsToTime((int)self.chargetime);
            }
        }
    }

    public partial class UIMainComponent
    {
        public ReferenceCollector IconCollector;
        public int mapid = 0, inttype = 0;
        public float chargetime = 0, stop = 0;

        public void InitIcon()
        {
            GameObject iconRoot = ReferenceCollector_Main.GetGameObject("Icon");
            if (iconRoot == null)
            {
                return;
            }

            IconCollector = iconRoot.GetReferenceCollector();
            if (IconCollector == null)
            {
                return;
            }

            Text nameTxt = IconCollector.GetText("NameTxt");
            if (nameTxt != null)
            {
                nameTxt.text = roleEntity.RoleName;
            }

            Text lvTxt = IconCollector.GetText("LVTxt");
            if (lvTxt != null)
            {
                lvTxt.text = $"LV:{roleEntity.Level}";
            }

            Image iconImage = IconCollector.GetImage("icon");
            if (iconImage != null)
            {
                int roleTypeIndex = (int)roleEntity.RoleType - 1;
                if (roleTypeIndex >= 0 && roleTypeIndex < iconImage.transform.childCount)
                {
                    iconImage.transform.GetChild(roleTypeIndex).gameObject.SetActive(true);
                }
            }

            Game.EventCenter.EventListenner<int>(EventTypeId.CHARGE_MAP, CHARGE_MAP);

            Button concealBtn = IconCollector.GetButton("Btn_conceal");
            if (concealBtn != null)
            {
                concealBtn.onClick.AddSingleListener(() =>
                {
                    inttype++;
                    GameObject concealRoot = IconCollector.GetGameObject("yingchang");
                    if (concealRoot != null)
                    {
                        concealRoot.SetActive(inttype % 2 == 0);
                    }
                });
            }
        }

        public void CHARGE_MAP(int name)
        {
            mapid = name;
            Log.DebugBrown("获取的地图信息" + name);
            if (IconCollector == null)
            {
                return;
            }

            Image display = IconCollector.GetImage("display");
            if (display != null)
            {
                display.gameObject.SetActive(mapid == 102 || mapid == 112);
            }

            chargetime = 0;
        }

        public void SetLV(long lv)
        {
            if (IconCollector == null)
            {
                return;
            }

            Text lvTxt = IconCollector.GetText("LVTxt");
            if (lvTxt != null)
            {
                lvTxt.text = $"LV:{lv}";
            }
        }
    }
}
