using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class SynthesisTips_InfoConfigCategory : ACategory<SynthesisTips_InfoConfig>
	{
	}

	///<summary>G功能 </summary>
	public class SynthesisTips_InfoConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>子Id </summary>
		public List<int> SonId;
		 ///<summary>功能名字 </summary>
		public string FunctionName;
		 ///<summary>描述 </summary>
		public string Desk;
	}
}
