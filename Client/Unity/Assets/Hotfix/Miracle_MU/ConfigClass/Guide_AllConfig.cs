using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Guide_AllConfigCategory : ACategory<Guide_AllConfig>
	{
	}

	///<summary>宝箱类型 </summary>
	public class Guide_AllConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>引导描述 </summary>
		public string Desc;
		 ///<summary>引导开启条件1主线任务 </summary>
		public int OpenGuideType;
		 ///<summary>为任务类型时  当此列表中id任务id接受 可以直接切换到此引导 </summary>
		public string Value;
		 ///<summary>在引导类型中的具体类型 </summary>
		public int OpType;
		 ///<summary>等级要求 </summary>
		public int Level;
		 ///<summary>当前引导流程完成后 进入下一个引导 </summary>
		public int NextGuidId;
		 ///<summary>引导步骤 </summary>
		public string Steps;
	}
}
