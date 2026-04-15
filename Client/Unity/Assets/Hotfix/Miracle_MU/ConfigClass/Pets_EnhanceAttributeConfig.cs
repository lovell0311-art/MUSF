using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Pets_EnhanceAttributeConfigCategory : ACategory<Pets_EnhanceAttributeConfig>
	{
	}

	///<summary>宠物强化属性对照表 </summary>
	public class Pets_EnhanceAttributeConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>增加属性1 </summary>
		public int Enhance;
		 ///<summary>描述 </summary>
		public string Description;
	}
}
