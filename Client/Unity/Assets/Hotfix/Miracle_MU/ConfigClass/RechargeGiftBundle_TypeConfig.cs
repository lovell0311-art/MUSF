using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class RechargeGiftBundle_TypeConfigCategory : ACategory<RechargeGiftBundle_TypeConfig>
	{
	}

	///<summary>充值礼包 </summary>
	public class RechargeGiftBundle_TypeConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>价格 </summary>
		public int Price;
		 ///<summary>原价 </summary>
		public int OriginPrice;
		 ///<summary>上架时间 </summary>
		public string StartTime;
		 ///<summary>持续时间(秒) </summary>
		public int DurationTime;
		 ///<summary>购买次数 </summary>
		public int BuyCount;
		 ///<summary>购买次数绑定类型(0.账号 1.角色) </summary>
		public int BindType;
	}
}
