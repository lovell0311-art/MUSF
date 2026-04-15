using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class BattleMaster_ALLConfigCategory : ACategory<BattleMaster_ALLConfig>
	{
	}

	///<summary>大师-法师 </summary>
	public class BattleMaster_ALLConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>介绍 </summary>
		public string Describe;
		 ///<summary>解锁魔晶数 </summary>
		public int Unlock;
		 ///<summary>使用职业 </summary>
		public string UseRole;
		 ///<summary>附加数据使用 </summary>
		public int OtherDataUse;
		 ///<summary>等级 </summary>
		public int LayerLevel;
		 ///<summary>前置技能 </summary>
		public List<int> FrontIds;
		 ///<summary>升级消耗 </summary>
		public int Consume;
		 ///<summary>附加数值 </summary>
		public string OtherData;
	}
}
