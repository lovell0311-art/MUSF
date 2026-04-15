using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class GameTask_DropItemConfigCategory : ACategory<GameTask_DropItemConfig>
	{
	}

	///<summary>任务物品掉落 </summary>
	public class GameTask_DropItemConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>怪物id </summary>
		public int MonsterId;
		 ///<summary>任务id </summary>
		public int TaskId;
		 ///<summary>任务目标id </summary>
		public int TaskTargetId;
		 ///<summary>备注 </summary>
		public string Remark;
		 ///<summary>物品id </summary>
		public int ItemId;
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
		 ///<summary>任务物品 </summary>
		public int IsTask;
		 ///<summary>卓越属性 </summary>
		public List<int> OptionExcellent;
		 ///<summary>自定义属性方法 </summary>
		public string CustomAttrMathod;
		 ///<summary>掉落率 </summary>
		public int DropRate;
	}
}
