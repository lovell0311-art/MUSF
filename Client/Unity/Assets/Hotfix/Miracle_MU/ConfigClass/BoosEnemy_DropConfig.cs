using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BoosEnemy_DropConfigCategory : ACategory<BoosEnemy_DropConfig>
	{
	}

	///<summary>特殊掉落 </summary>
	public class BoosEnemy_DropConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>掉落组描述 </summary>
		public string Name;
		 ///<summary>不重复 </summary>
		public int NotRepeat;
	}
}
