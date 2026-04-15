using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class RoleConfig_ExperienceConfigCategory : ACategory<RoleConfig_ExperienceConfig>
	{
	}

	///<summary>角色 </summary>
	public class RoleConfig_ExperienceConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>升级所需经验 </summary>
		public long Exprience;
		 ///<summary>升级累积需要经验 </summary>
		public long ExprenceLevel;
	}
}
