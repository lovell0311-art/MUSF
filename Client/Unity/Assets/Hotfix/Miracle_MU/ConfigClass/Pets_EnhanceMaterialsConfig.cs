using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Pets_EnhanceMaterialsConfigCategory : ACategory<Pets_EnhanceMaterialsConfig>
	{
	}

	///<summary>宠物强化材料对照表 </summary>
	public class Pets_EnhanceMaterialsConfig: IConfig
	{
		public long Id { get; set; }
	}
}
