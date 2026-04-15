using System;
using System.Collections.Generic;
using System.Text;

namespace ETModel
{
	// 对应物品配置表Type字段
	public enum EItemType
	{
		None = 0,
		/// <summary>剑</summary>
		Swords = 1,
		/// <summary>斧头</summary>
		Axes = 2,
		/// <summary>棒/槌</summary>
		Maces = 3,
		/// <summary>弓</summary>
		Bows = 4,
		/// <summary>弩</summary>
		Crossbows = 5,
		/// <summary>箭筒/箭</summary>
		Arrow = 6,
		/// <summary>矛/长矛</summary>
		Spears = 7,
		/// <summary>魔杖</summary>
		Staffs = 8,
		/// <summary>异界之书/魔法书</summary>
		MagicBook = 9,
		/// <summary> 权杖 </summary>
		Scepter = 10,
		/// <summary>符文魔棒</summary>
		RuneWand = 11,
		/// <summary>拳刃</summary>
		FistBlade = 12,
		/// <summary>魔剑</summary>
		MagicSword = 13,
		/// <summary>短剑</summary>
		ShortSword = 14,
		/// <summary>魔法枪</summary>
		MagicGun = 15,
		/// <summary>盾</summary>
		Shields = 16,
		/// <summary>头盔</summary>
		Helms = 17,
		/// <summary>盔甲</summary>
		Armors = 18,
		/// <summary>护腿</summary>
		Pants = 19,
		/// <summary>护手</summary>
		Gloves = 20,
		/// <summary>靴子</summary>
		Boots = 21,
		/// <summary>翅膀/披风</summary>
		Wing = 22,
		/// <summary>项链</summary>
		Necklace = 23,
		/// <summary>戒指</summary>
		Rings = 24,
		/// <summary>耳环</summary>
		Dangler = 25,
		/// <summary>坐骑</summary>
		Mounts = 26,
		/// <summary>荧光宝石</summary>
		FGemstone = 27,
		/// <summary>宝石</summary>
		Gemstone = 28,
		/// <summary>技能书</summary>
		SkillBooks = 29,
		/// <summary>守护</summary>
		Guard = 30,
		/// <summary>消耗品(血瓶|药水|实力提升卷轴)</summary>
		Consumables = 31,
		/// <summary> 其他 </summary>
		Other = 32,
		/// <summary> 任务物品 </summary>
		Task = 33,
        /// <summary> 旗帜 </summary>
        Flag = 34,
		/// <summary>宠物进背包物品/// </summary>
		Pets= 35,
		/// <summary> 手环 </summary>
		Bracelet = 36,
    }
}
