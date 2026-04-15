using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class Activity_RewardPropsConfigCategory : ACategory<Activity_RewardPropsConfig>
	{
	}

	///<summary>活动奖励物品 </summary>
	public class Activity_RewardPropsConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>活动id </summary>
		public int Activity;
		 ///<summary>物品id </summary>
		public int ItemId;
		 ///<summary>备注 </summary>
		public string Remark;
		 ///<summary>强化等级 </summary>
		public int Level;
		 ///<summary>数量 </summary>
		public int Quantity;
		 ///<summary>追加列表id </summary>
		public int OptListId;
		 ///<summary>追加等级 </summary>
		public int OptLevel;
		 ///<summary>拥有技能 </summary>
		public int HasSkill;
		 ///<summary>套装id </summary>
		public int SetId;
		 ///<summary>绑定物品 </summary>
		public int IsBind;
		 ///<summary>卓越属性 </summary>
		public List<int> OptionExcellent;
		 ///<summary>自定义属性方法 </summary>
		public string CustomAttrMathod;
	}
}
