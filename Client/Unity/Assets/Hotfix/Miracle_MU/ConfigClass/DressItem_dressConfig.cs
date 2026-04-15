using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class DressItem_dressConfigCategory : ACategory<DressItem_dressConfig>
	{
	}

	///<summary>时装 </summary>
	public class DressItem_dressConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>名字 </summary>
		public string Name;
		 ///<summary>资源名 </summary>
		public string ResName;
		 ///<summary>时装资源名 </summary>
		public string FashionResName;
		 ///<summary>追加属性 </summary>
		public List<int> AppendAttrId;
		 ///<summary>基础属性 </summary>
		public List<int> BaseAttrId;
		 ///<summary>额外属性 </summary>
		public List<int> ExtraAttrId;
		 ///<summary>需求等级 </summary>
		public int ReqLvl;
		 ///<summary>强化所需材料 </summary>
		public string Material1;
		 ///<summary>使用职业 </summary>
		public string UseRole;
		 ///<summary>提示 </summary>
		public string Prompt;
		 ///<summary>获取途径 </summary>
		public string GetWay;
	}
}
