using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class LevelTopUp_TypeConfigCategory : ACategory<LevelTopUp_TypeConfig>
	{
	}

	///<summary>等级充值 </summary>
	public class LevelTopUp_TypeConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>等级 </summary>
		public int Level;
		 ///<summary>充值金额 </summary>
		public int RechargeAmount;
		 ///<summary>职业类型 </summary>
		public int RoleType;
		 ///<summary>赠送物品ID </summary>
		public int ConfigId;
		 ///<summary>物品名 </summary>
		public string Name;
		 ///<summary>强化等级 </summary>
		public int EnhanceLevel;
		 ///<summary>数量 </summary>
		public int Quantity;
		 ///<summary>拥有技能 </summary>
		public int HasSkill;
		 ///<summary>有幸运属性 </summary>
		public int HasLucky;
		 ///<summary>绑定物品 </summary>
		public int IsBind;
		 ///<summary>自定义属性方法 </summary>
		public string CustomAttrMathod;
	}
}
