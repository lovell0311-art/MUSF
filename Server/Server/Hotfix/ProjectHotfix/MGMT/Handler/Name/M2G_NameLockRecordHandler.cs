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
    public class M2G_NameLockRecordHandler : AMHandler<M2G_NameLockRecord>
    {
        protected override async Task<bool> Run(M2G_NameLockRecord b_Request)
        {
            var mNameLockComponent = Root.MainFactory.GetCustomComponent<NameLockComponent>();
            if (mNameLockComponent == null)
            {
                return false;
            }
            
            if (!mNameLockComponent.strings.Contains(b_Request.Name))
            {
                return false;
            }

            mNameLockComponent.strings.Remove(b_Request.Name);
            return true;
        }
    }
}
