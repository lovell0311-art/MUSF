using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BattleCopyConfig_OpenConfigCategory : ACategory<BattleCopyConfig_OpenConfig>
	{
	}

	///<summary>副本开放 </summary>
	public class BattleCopyConfig_OpenConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>副本名 </summary>
		public string Name;
		 ///<summary>开启时间1 </summary>
		public string OpenTime1;
		 ///<summary>开启时间2 </summary>
		public string OpenTime2;
		 ///<summary>开启时间3 </summary>
		public string OpenTime3;
		 ///<summary>开启时间4 </summary>
		public string OpenTime4;
		 ///<summary>开启时间5 </summary>
		public string OpenTime5;
		 ///<summary>开启时间6 </summary>
		public string OpenTime6;
		 ///<summary>开启时间7 </summary>
		public string OpenTime7;
		 ///<summary>开启时间8 </summary>
		public string OpenTime8;
		 ///<summary>开启时间9 </summary>
		public string OpenTime9;
		 ///<summary>开启时间10 </summary>
		public string OpenTime10;
		 ///<summary>开启时间11 </summary>
		public string OpenTime11;
		 ///<summary>开启时间12 </summary>
		public string OpenTime12;
		 ///<summary>开启时间13 </summary>
		public string OpenTime13;
		 ///<summary>开启时间14 </summary>
		public string OpenTime14;
		 ///<summary>开启时间15 </summary>
		public string OpenTime15;
		 ///<summary>开启时间16 </summary>
		public string OpenTime16;
		 ///<summary>开启时间17 </summary>
		public string OpenTime17;
		 ///<summary>开启时间18 </summary>
		public string OpenTime18;
		 ///<summary>开启时间19 </summary>
		public string OpenTime19;
		 ///<summary>开启时间20 </summary>
		public string OpenTime20;
		 ///<summary>开启时间21 </summary>
		public string OpenTime21;
		 ///<summary>开启时间22 </summary>
		public string OpenTime22;
		 ///<summary>开启时间23 </summary>
		public string OpenTime23;
		 ///<summary>开启时间24 </summary>
		public string OpenTime24;
		 ///<summary>持续时间（分钟） </summary>
		public string Duration;
	}
}
