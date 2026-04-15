
using System;
using System.Threading.Tasks;

using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using static CustomFrameWork.Component.ServerManageComponent;
using ETModel;

namespace ETHotfix
{
    [EventMethod(typeof(RealmGateAddressComponent), EventSystemType.INIT)]
    public class RealmGateAddressComponentEventOnInit : ITEventMethodOnInit<RealmGateAddressComponent>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="b_Component"></param>
        public void OnInit(RealmGateAddressComponent b_Component)
        {
            b_Component.OnInit();
        }
    }

    public static class RealmGateAddressComponentEx
    {
        public static void OnInit(this RealmGateAddressComponent b_Component)
        {

        }

        public static C_StartUpInfo GetAddress(this RealmGateAddressComponent self, long userID = 0)
        {
            var mStartUpInfos = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Gate);
            if (mStartUpInfos != null && mStartUpInfos.Length > 0)
            {
                int n = RandomHelper.RandomNumber(0, mStartUpInfos.Length);

                int num = Convert.ToInt32(userID % mStartUpInfos.Length);

                return mStartUpInfos[num];
            }
            return null;
        }

        public static async Task<C_StartUpInfo> GetAddress(this RealmGateAddressComponent self, AppType appType, long userID = 0)
        {
            return self.GetAddress(userID);
            if (OptionComponent.Options.AppType == AppType.AllServer)
            {
                return Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(OptionComponent.Options.AppId);
            }

            return await DispatcherServerHelper.DispatcherServerConfig(appType, userID);
        }

        public static async Task<C_StartUpInfo> GetAddress(this RealmGateAddressComponent self, int gateID, long userID = 0)
        {
            C_StartUpInfo config = null;
            if (userID == 0)
            {
                if (OptionComponent.Options.AppType == AppType.AllServer)
                {
                    //if (StartConfigComponent.Instance.StartConfig.AppId == gateID)
                    {
                        config = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfo(OptionComponent.Options.AppId);
                    }
                }
                else
                {
                    var mStartUpInfos = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(AppType.Gate);
                    foreach (C_StartUpInfo gate in mStartUpInfos)
                    {
                        if (gate.AppId == gateID)
                        {
                            config = gate;
                            break;
                        }
                    }
                }
                if (config == null)
                {
                    config = await self.GetAddress(AppType.Gate, userID);
                }
            }
            else
            {
                config = await self.GetAddress(AppType.Gate, userID);
            }
            return config;
        }

    }
}
