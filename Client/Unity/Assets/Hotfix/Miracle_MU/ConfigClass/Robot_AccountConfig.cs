using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Robot_AccountConfigCategory : ACategory<Robot_AccountConfig>
	{
	}

	///<summary>机器人账号 </summary>
	public class Robot_AccountConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>手机号 </summary>
		public string Phone;
		 ///<summary>密码 </summary>
		public string Passwd;
		 ///<summary>区id </summary>
		public int ZoneId;
		 ///<summary>线id </summary>
		public int LineId;
	}
}
