using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BattleIntoInfo_IntoConfigCategory : ACategory<BattleIntoInfo_IntoConfig>
	{
	}

	///<summary>副本条件 </summary>
	public class BattleIntoInfo_IntoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>副本名字 </summary>
		public string BattleName;
		 ///<summary>入场角色等级1 </summary>
		public string IntoRoleLevel1;
		 ///<summary>入场角色等级2 </summary>
		public string IntoRoleLevel2;
		 ///<summary>入场角色等级3 </summary>
		public string IntoRoleLevel3;
		 ///<summary>入场角色等级4 </summary>
		public string IntoRoleLevel4;
		 ///<summary>入场角色等级5 </summary>
		public string IntoRoleLevel5;
		 ///<summary>入场角色等级6 </summary>
		public string IntoRoleLevel6;
		 ///<summary>入场角色等级7 </summary>
		public string IntoRoleLevel7;
	}
}
