using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
    public partial class UIPetNewComponent
    {
        /// <summary>
        /// 放生宠物
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetsReleaseRequest(long petId)
        {
            G2C_PetsReleaseResponse g2C_OpenPets = (G2C_PetsReleaseResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsReleaseRequest()
            {
                PetsID = petId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                

                if (petGameObject != null && petGameObject.activeSelf) ResourcesComponent.Instance.DestoryGameObjectImmediate(petGameObject, petGameObject.name.StringToAB());//回收上一个 角色模型

                InitPetNewList().Coroutine();
                UIComponent.Instance.VisibleUI(UIType.UIHint, "放生成功!");
            }
        }
        
        /// <summary>
        /// 出战宠物
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetsGoToWarRequest(long petsId)
        {

            G2C_PetsGoToWarResponse g2C_OpenPets = (G2C_PetsGoToWarResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsGoToWarRequest()
            {
                PetsID = petsId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                InitPetNewList().Coroutine();
                //lastClickItem.transform.Find("restOrWar").gameObject.SetActive(true);
                SetBtnActive(PetWarState.War);
            }
        }
        /// <summary>
        /// 宠物休息
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetsRestRequest(long petsId)
        {

            G2C_PetsRestResponse g2C_OpenPets = (G2C_PetsRestResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsRestRequest()
            {
                PetsID = petsId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                lastClickItem.transform.Find("restOrWar").gameObject.SetActive(false);
                SetBtnActive(PetWarState.Rest);
            }
        }
        /// <summary>
        /// 宠物放进背包
        /// </summary>
        /// <param name="petsId"></param>
        /// <returns></returns>
        public async ETTask PetPackBackRequest(long petsId)
        {

            G2C_PetsPackBackResponse g2C_OpenPets = (G2C_PetsPackBackResponse)await SessionComponent.Instance.Session.Call(new C2G_PetsPackBackRequest()
            {
                PetsID = petsId
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            else
            {
                if (petGameObject != null && petGameObject.activeSelf) ResourcesComponent.Instance.DestoryGameObjectImmediate(petGameObject, petGameObject.name.StringToAB());//回收上一个 角色模型

                InitPetNewList().Coroutine();
                UIComponent.Instance.VisibleUI(UIType.UIHint, "放入背包成功!");
            }
        }
        /// <summary>
        /// 获得新宠物
        /// </summary>
        /// <param name="petsConfigID"></param>
        /// <returns></returns>
        public async ETTask InsertPetsRequest(int petsConfigID)
        {
            G2C_InsertPetsResponse g2C_OpenPets = (G2C_InsertPetsResponse)await SessionComponent.Instance.Session.Call(new C2G_InsertPetsRequest()
            {
                PetsConfigID = petsConfigID
            });
            if (g2C_OpenPets.Error != 0)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_OpenPets.Error.GetTipInfo());
            }
            //else
            //{
            //    InitPetList().Coroutine();
            //    Log.DebugGreen("获得新宠物成功");
            //}
        }
    }
}
