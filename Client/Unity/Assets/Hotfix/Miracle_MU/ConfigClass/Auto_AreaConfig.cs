using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Auto_AreaConfigCategory : ACategory<Auto_AreaConfig>
	{
	}

	///<summary>自动合区开区 </summary>
	public class Auto_AreaConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>区名 </summary>
		public string AreaName;
		 ///<summary>开启时间 </summary>
		public string OpeningTime;
		 ///<summary>提前准备时间 </summary>
		public int PreparationTime;
		 ///<summary>线路参数 </summary>
		public string LinesParameter;
		 ///<summary>合并时间 </summary>
		public string MergeTime;
		 ///<summary>合并到几区 </summary>
		public int MergeArea;
		 ///<summary>数据地址 </summary>
		public string DBAddreas;
	}
}
