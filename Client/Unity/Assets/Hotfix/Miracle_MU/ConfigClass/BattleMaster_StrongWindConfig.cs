using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BattleMaster_StrongWindConfigCategory : ACategory<BattleMaster_StrongWindConfig>
	{
	}

	///<summary>大师-疾风 </summary>
	public class BattleMaster_StrongWindConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>介绍 </summary>
		public string Describe;
		 ///<summary>附加数据使用 </summary>
		public int OtherDataUse;
		 ///<summary>等级 </summary>
		public int LayerLevel;
		 ///<summary>前置技能 </summary>
		public List<int> FrontIds;
		 ///<summary>上一阶技能 </summary>
		public List<int> LastIds;
		 ///<summary>升级消耗 </summary>
		public int Consume;
		 ///<summary>附加数值 </summary>
		public string OtherData;
	}
}
