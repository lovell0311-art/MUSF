using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static CustomFrameWork.Component.ServerManageComponent;

namespace ETHotfix
{



    /// <summary>
    /// 分发服务器
    /// </summary>
    public static class DispatcherServerHelper
    {
        /// <summary>
        /// 根据USERID自动分发服务器配置
        /// </summary>
        /// <param name="appType"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task<C_StartUpInfo> DispatcherServerConfig(AppType appType, long userID = 0)
        {
            //手动扩容
            //await ScalingHelper.Scaling(appType);
            var configList = Root.MainFactory.GetCustomComponent<ServerManageComponent>().GetStartUpInfos(appType);
            if (configList == null || configList.Length == 0)
            {
                return null;
            }

            int num = 0;
            List<C_StartUpInfo> list = new List<C_StartUpInfo>(configList);
            DBComponent dbComponent = Root.MainFactory.GetCustomComponent<DBComponent>();
            while (list.Count > 0)
            {
                if (userID == 0)
                {
                    num = Help_RandomHelper.Range(0, list.Count);
                }
                else
                {
                    num = Convert.ToInt32(userID % list.Count);
                }
                //检查服务是否可用
                var dateNow = DateTime.UtcNow.AddSeconds(-15);
                if (await dbComponent.GetCount<DBServiceRegistryInfo>(f => f.GameServerId == list[num].AppId
                                                                         && dateNow < f.UpdateTime2) > 0)
                {
                    return list[num];
                }
                else
                {
                    list.Remove(list[num]);
                }
            }

            return null;
        }
    }
}
