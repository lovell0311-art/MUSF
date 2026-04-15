using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Guide_StepConfigCategory : ACategory<Guide_StepConfig>
	{
	}

	///<summary>宝箱类型 </summary>
	public class Guide_StepConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>引导描述 </summary>
		public string Desc;
		 ///<summary>1点击类型2拖拽 </summary>
		public int ClickType;
		 ///<summary>引导类型1为任务类型 </summary>
		public int GuideType;
		 ///<summary>在引导类型中的具体类型 </summary>
		public int OpType;
		 ///<summary>是否需要屏蔽界面中的所有操作1为需要屏蔽 </summary>
		public int PingBi;
		 ///<summary>箭头图标方向1上2下 </summary>
		public int Dir;
		 ///<summary>拖拽的终点 </summary>
		public string DragEndTarget;
		 ///<summary>页面中可点击区域路径 </summary>
		public string ClickTarget;
	}
}
