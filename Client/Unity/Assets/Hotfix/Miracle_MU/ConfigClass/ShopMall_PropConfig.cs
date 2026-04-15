using System.Collections.Generic;
using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Game))]
	public partial class ShopMall_PropConfigCategory : ACategory<ShopMall_PropConfig>
	{
	}

	///<summary>商城道具 </summary>
	public class ShopMall_PropConfig: IConfig
	{
		public long Id { get; set; }
		 ///<summary>商城类型 </summary>
		public int ShopMall;
		 ///<summary>商品分类 </summary>
		public int ShopType;
		 ///<summary>道具ID </summary>
		public int ItemID;
		 ///<summary>物品名称 </summary>
		public string ItemName;
		 ///<summary>物品标识 </summary>
		public string ItemIcon;
		 ///<summary>物品介绍 </summary>
		public string Introduce;
		 ///<summary>物品到期时间 </summary>
		public int ItemTime;
		 ///<summary>宝石种类 </summary>
		public int Gemtypes;
		 ///<summary>初始单价 </summary>
		public int Price;
		 ///<summary>限时起点 </summary>
		public string StartTime;
		 ///<summary>限时时长 </summary>
		public long Duration;
		 ///<summary>折扣 </summary>
		public int Discount;
		 ///<summary>折扣限时起点 </summary>
		public string DiscountstartTime;
		 ///<summary>折扣限时时长 </summary>
		public long Discountduration;
		 ///<summary>最大组数限制 </summary>
		public int BuyMaxlimit;
		 ///<summary>最小组数限制 </summary>
		public int BuyMinlimit;
		 ///<summary>单组数量 </summary>
		public int UnitQuantity;
		 ///<summary>套装ID </summary>
		public int SetID;
		 ///<summary>限量 </summary>
		public int Limit;
		 ///<summary>限量时间(秒) </summary>
		public int LimitTime;
		 ///<summary>自定义属性方法 </summary>
		public string CustomAttrMathod;
		 ///<summary>库存数量 </summary>
		public int InventoryMax;
		 ///<summary>绑定物品 </summary>
		public int IsBind;
	}
}
