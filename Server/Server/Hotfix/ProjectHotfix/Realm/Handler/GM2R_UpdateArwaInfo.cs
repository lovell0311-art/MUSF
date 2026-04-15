using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class GM2R_UpdateArwaInfoHandler : AMHandler<GM2R_UpdateArwaInfo>
    {
        protected override async Task<bool> Run(GM2R_UpdateArwaInfo b_Request)
        {
            var AutoAreaComponent = Root.MainFactory.GetCustomComponent<AutoAreaComponent>();
            if (AutoAreaComponent == null)
            {
                return false;
            }
            if(!int.TryParse(b_Request.AreaInfo,out var AreaId))
                return false;

            if (!AutoAreaComponent.keyValuePairs.ContainsKey(AreaId))
            {
                var Dic = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<Auto_AreaConfigJson>().JsonDic;
                if (Dic.TryGetValue(AreaId, out var auto_AreaConfig))
                {
                    AutoInfo autoInfo = new AutoInfo();
                    autoInfo.Id = AreaId;
                    autoInfo.Name = auto_AreaConfig.AreaName;
                    autoInfo.State = 1;
                    AutoAreaComponent.keyValuePairs.Add(AreaId,autoInfo);
                }
            }
            return true;
        }
    }
}
