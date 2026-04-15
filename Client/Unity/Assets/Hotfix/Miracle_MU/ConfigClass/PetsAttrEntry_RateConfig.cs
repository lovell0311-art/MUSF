using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class PetsAttrEntry_RateConfigCategory : ACategory<PetsAttrEntry_RateConfig>
	{
	}

	///<summary>宠物获取卓越属性条数概率 </summary>
	public class PetsAttrEntry_RateConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>条数 </summary>
		public int StripCnt;
		 ///<summary>概率(万分比) </summary>
		public int Rate;
	}
}
