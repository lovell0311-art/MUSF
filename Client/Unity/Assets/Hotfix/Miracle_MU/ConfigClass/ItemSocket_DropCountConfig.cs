using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ItemSocket_DropCountConfigCategory : ACategory<ItemSocket_DropCountConfig>
	{
	}

	///<summary>镶嵌孔洞掉落数 </summary>
	public class ItemSocket_DropCountConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>孔数 </summary>
		public int SocketCnt;
		 ///<summary>概率(权重) </summary>
		public int Rate;
	}
}
