using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Pets_ExpConfigCategory : ACategory<Pets_ExpConfig>
	{
	}

	///<summary>宠物经验 </summary>
	public class Pets_ExpConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>升级所需经验 </summary>
		public long Exprience;
		 ///<summary>升级累积需要经验 </summary>
		public long ExprenceLevel;
	}
}
