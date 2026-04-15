using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIRoleInfoComponent
    {
        public static Image TitleImage;
        public GameObject TitleImageShow;
        public void SelectTitle()
        {
            TitleImage = collector.GetImage("TitleImage");
            collector.GetButton("ChangeTitleBtn").onClick.AddSingleListener(() =>
            {
                UIComponent.Instance.VisibleUI(UIType.UITitle);
                TitleImage.gameObject.SetActive(false);
            });
            ChangeTitle();
        }
        public void ChangeTitle()
        {
            if (TitleImageShow != null)
                ResourcesComponent.Instance.RecycleGameObject(TitleImageShow);

            //ResourcesComponent.Instance.DestoryGameObjectImmediate(TitleImageShow, TitleImageShow.name.StringToAB());
            TitleConfig_InfoConfig titleConfig = ConfigComponent.Instance.GetItem<TitleConfig_InfoConfig>(TitleManager.useID);
            if (titleConfig != null)
            {

                TitleImageShow = ResourcesComponent.Instance.LoadGameObject(titleConfig.AsstetName.StringToAB(), titleConfig.AsstetName);
                TitleImageShow.transform.SetParent(TitleImage.transform);
                TitleImageShow.transform.localRotation = Quaternion.identity;
                TitleImageShow.transform.localScale = Vector3.one * 0.2f;
                TitleImageShow.transform.GetComponent<RectTransform>().transform.localPosition = new Vector3(0, -55, 0);
                TitleImage.gameObject.SetActive(true);
            }
            else
            {
                TitleImage.gameObject.SetActive(false);
            }
        }
    }

}
