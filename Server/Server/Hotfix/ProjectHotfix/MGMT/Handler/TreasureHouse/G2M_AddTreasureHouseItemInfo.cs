using CustomFrameWork;
using CustomFrameWork.Component;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Ecm.V20190719.Models;

namespace ETHotfix
{
    [MessageHandler(AppType.MGMT)]
    public class G2M_AddTreasureHouseItemInfoHandler : AMActorRpcHandler<G2M_AddTreasureHouseItemInfo, M2G_AddTreasureHouseItemInfo>
    {
        protected override async Task<bool> Run(G2M_AddTreasureHouseItemInfo b_Request, M2G_AddTreasureHouseItemInfo b_Response, Action<IMessage> b_Reply)
        {
            var component = Root.MainFactory.GetCustomComponent<MGMTTreasureHouse>();
            if (component == null)
            {
                b_Response.Error = Root.MainFactory.GetCustomComponent<LanguageComponent>().GetMessageId(3305);
                b_Reply(b_Response);
                return false;
            }

            int mAreaId = (int)(b_Request.AppendData >> 16);
            THItemInfo tHItemInfo = new THItemInfo();
            tHItemInfo.Id = IdGeneraterNew.Instance.GenerateUnitId(mAreaId);
            tHItemInfo.Uid = b_Request.ItemInfo.Uid;//道具ID
            tHItemInfo.UserID = b_Request.ItemInfo.UserID;//出售者ID
            tHItemInfo.Name = b_Request.ItemInfo.Name;//道具名称
            tHItemInfo.Class = b_Request.ItemInfo.Class;//职业类型
            tHItemInfo.Excellent = b_Request.ItemInfo.Excellent;//卓越条数
            tHItemInfo.Enhance = b_Request.ItemInfo.Enhance;//强化等级
            tHItemInfo.Readdition = b_Request.ItemInfo.Readdition;//追加等级
            tHItemInfo.Price = b_Request.ItemInfo.Price;//价格
            tHItemInfo.Page = 0;//页签
            tHItemInfo.MaxType = b_Request.MaxType;
            tHItemInfo.MinType = b_Request.MinType;
            tHItemInfo.mAreaId = b_Request.ItemInfo.AreaId;
            tHItemInfo.ListingTime = Help_TimeHelper.GetNowSecond() + 604800;//上架时间
            tHItemInfo.ConfigId = b_Request.ItemInfo.ConfigID;
            tHItemInfo.IsDispose = 0;//是否有效
            tHItemInfo.ClassList = Help_JsonSerializeHelper.DeSerialize<Dictionary<int,int>>(tHItemInfo.Class);
            tHItemInfo.Cnt = b_Request.ItemInfo.Cnt;

            await component.AddItem(tHItemInfo);

            b_Response.DBid = tHItemInfo.Id;
            b_Reply(b_Response);
            return true;

        }
    }
}