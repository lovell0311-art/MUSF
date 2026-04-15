using Codice.CM.Common;
using ETModel;
using System.Collections.Generic;

namespace ETHotfix
{
    public static class RecycleEquipTools
    {
        private static bool singleExcellence;//单词条卓越
        private static bool whiteSuit;//白装
        private static bool doubleExcellence;//双词条卓越
        private static bool skillBook;//技能书
        private static bool intensifyEquip;//强化装备
        private static bool red_BlueMedicine;//小瓶红、蓝药水
        private static bool autoRecycle;//自动回收
        public static long CurNpcUUid;//自动回收
        public static bool red_SkillId;//带技能
        static RecycleEquipSetting recycleEquipSetting;

        public static bool SingleExcellence { get => singleExcellence; set => singleExcellence = value; }
        public static bool WhiteSuit { get => whiteSuit; set => whiteSuit = value; }
        public static bool DoubleExcellence { get => doubleExcellence; set => doubleExcellence = value; }
        public static bool SkillBook { get => skillBook; set => skillBook = value; }
        public static bool IntensifyEquip { get => intensifyEquip; set => intensifyEquip = value; }
        public static bool Red_BlueMedicine { get => red_BlueMedicine; set => red_BlueMedicine = value; }
        public static bool IsSkillId { get => red_SkillId; set => red_SkillId = value; }
        public static bool AutoRecycle
        {
            get
            {
                //没有特权
                if (UnitEntityComponent.Instance.LocalRole.MinMonthluCardTimeSpan.TotalSeconds <= 0 && UnitEntityComponent.Instance.LocalRole.MaxMonthluCardTimeSpan.TotalSeconds <= 0 && !TitleManager.allTitles.Exists(x => x.TitleId == 60005))
                    return false;

                return autoRecycle;
            }
            set => autoRecycle = value;
        }

        public static void Init() { }
        static List<long> recycleMedicine = new List<long>() { 310002, 310003, 310004, 310005, 310006, 310007, 310045 };
        static RecycleEquipTools()
        {
            recycleEquipSetting = LocalDataJsonComponent.Instance.LoadData<RecycleEquipSetting>(LocalJsonDataKeys.RecycleEquipSetting) ?? new RecycleEquipSetting();
            SingleExcellence = recycleEquipSetting.SingleExcellence;
            DoubleExcellence = recycleEquipSetting.DoubleExcellence;
            IsSkillId = recycleEquipSetting.SkillId;
            WhiteSuit = recycleEquipSetting.WhiteSuit;
            SkillBook = recycleEquipSetting.SkillBook;
            IntensifyEquip = recycleEquipSetting.IntensifyEquip;
            Red_BlueMedicine = recycleEquipSetting.Red_BlueMedicine;
            //AutoRecycle = recycleEquipSetting.AutoRecycle;
            AutoRecycle = true;
        }

        //自动出售
        public static void AutoSell(KnapsackDataItem item)
        {

            //翅膀不能卖
            //  if (item.IsCanSell() == false) return;
            Log.Info("执行自动出售-----------------");

            if (SingleExcellence && item.IsSingleExcellence())
            {
                Releycle().Coroutine();
                return;

            }
            else if (SkillBook && item.IsSkillBook())
            {
                Releycle().Coroutine();
                return;
            }
            else if (IntensifyEquip && item.IsIntensifyEquip())
            {
                Releycle().Coroutine();
                return;
            }
            else if (Red_BlueMedicine && item.IsRed_BlueMedicine())
            {
                Releycle().Coroutine();
                return;
            }
            else if (IsSkillId && item.IsSkill())
            {
                Releycle().Coroutine();
                return;
            }
            else if (WhiteSuit && item.IsWhiteSuit())
            {
                Releycle().Coroutine();
                return;
            }
            //if (WhiteSuit && item.IsWhiteSuit())
            //{
            //    //卓越 幸运、套装、洞转、橙装、追加属性、再生属性、技能

            //    Releycle().Coroutine();
            //    return;

            //}

            //if (DoubleExcellence && item.IsDoubleExcellence())
            //{

            //    Releycle().Coroutine();
            //    return;

            //}

            //if (SkillBook && item.IsSkillBook())
            //{
            //    Releycle().Coroutine();
            //    return;

            //}

            //if (IntensifyEquip && item.IsIntensifyEquip())
            //{
            //    Releycle().Coroutine();
            //    return;

            //}

            //if (Red_BlueMedicine && item.IsRed_BlueMedicine())
            //{

            //    Releycle().Coroutine();
            //    return;

            //}
            //if (IsSkillId && item.IsSkill())
            //{
            //    Releycle().Coroutine();
            //    return;
            //}

            async ETVoid Releycle()
            {
                //确定将 物品 出售
                G2C_SellingItemToNPCShop g2C_SellingItemToNPC = (G2C_SellingItemToNPCShop)await SessionComponent.Instance.Session.Call(new C2G_SellingItemToNPCShop
                {
                    NPCShopID = CurNpcUUid, //商店NPC Id
                    ItemUUID = item.UUID //卖出的物品的 UUID
                });
                if (g2C_SellingItemToNPC.Error != 0)
                {
                    UIComponent.Instance.VisibleUI(UIType.UIHint, g2C_SellingItemToNPC.Error.GetTipInfo());
                }
                /*  else
                  {
                      UIComponent.Instance.VisibleUI(UIType.UIHint, $"回收 {item.item_Info.Name}-");
                  }*/
            }
        }
        public static bool IsCanSell(this KnapsackDataItem item)
        {

            if (item.GetProperValue(E_ItemValue.IsTask) == 1) return false;
            if (item.GetProperValue(E_ItemValue.IsUsing) == 1) return false;
            if (item.GetProperValue(E_ItemValue.IsLocking) == 1) return false;


            switch ((E_ItemType)item.ItemType)
            {
                case E_ItemType.None:
                    return false;
                case E_ItemType.Swords:
                case E_ItemType.Axes:
                case E_ItemType.Maces:
                case E_ItemType.Bows:
                case E_ItemType.Crossbows:
                case E_ItemType.Arrow:
                case E_ItemType.Spears:
                case E_ItemType.Staffs:
                case E_ItemType.MagicBook:
                case E_ItemType.Scepter:
                case E_ItemType.RuneWand:
                case E_ItemType.FistBlade:
                case E_ItemType.MagicSword:
                case E_ItemType.ShortSword:
                case E_ItemType.MagicGun:
                case E_ItemType.Shields:
                case E_ItemType.Helms:
                case E_ItemType.Armors:
                case E_ItemType.Pants:
                case E_ItemType.Gloves:
                case E_ItemType.Boots:
                    return true;
                case E_ItemType.Wing:
                    break;
                case E_ItemType.Necklace:
                    return true;
                case E_ItemType.Rings:
                    return true;
                case E_ItemType.Dangler:
                    return true;
                case E_ItemType.Mounts:
                    break;
                case E_ItemType.FGemstone:
                    break;
                case E_ItemType.Gemstone:
                    break;
                case E_ItemType.SkillBooks:
                    return true;
                case E_ItemType.Guard:
                    break;
                case E_ItemType.Consumables:
                    return true;
                case E_ItemType.Other:
                    break;
                case E_ItemType.Task:
                    break;
                case E_ItemType.QiZhi:
                    return true;
                case E_ItemType.Pet:
                    break;
                case E_ItemType.WristBand:
                    return true;
                default:
                    break;
            }
            return false;
        }
        /// <summary>
        /// 特殊物品不可回收
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool SpecialGood(this KnapsackDataItem item)
        {
            if (item.ItemType == (int)E_ItemType.Pet || item.ItemType == (int)E_ItemType.Mounts || item.ItemType == (int)E_ItemType.Guard || item.ItemType == (int)E_ItemType.Gemstone || item.ItemType == (int)E_ItemType.FGemstone || item.ItemType == (int)E_ItemType.Wing)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// 是否是白装
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsWhiteSuit(this KnapsackDataItem item)
        {
            if (item.ConfigId == 30005 || item.ConfigId == 80007 || item.ConfigId == 40006)//玛雅装备不能回收
            {
                return false;
            }
            if (item.ConfigId == 320097 || item.ConfigId == 320098 || item.ConfigId == 320005 || item.ConfigId == 320004)//回收指定装备
            {
                return true;
            }

            if (SpecialGood(item) == false)
            {
                return false;
            }
            if (item.ItemType == (int)E_ItemType.Consumables || item.ItemType == (int)E_ItemType.SkillBooks) return false;
            if (item.IsCanSell() == false) return false;

            return item.ExecllentEntryDic.Count == 0 && item.GetProperValue(E_ItemValue.LuckyEquip) == 0 && item.GetProperValue(E_ItemValue.SetId) == 0
            && item.GetProperValue(E_ItemValue.FluoreSlotCount) == 0 && item.GetProperValue(E_ItemValue.OptLevel) == 0 && item.IsHaveReginAtr == false && item.GetProperValue(E_ItemValue.SkillId) == 0
            && item.GetProperValue(E_ItemValue.Level) == 0 && item.ItemType != (int)E_ItemType.Other; ;
        }
        /// <summary>
        /// 可交易物品
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsCanTrade(this KnapsackDataItem item)
        {
            if (item.ConfigId == 30005 || item.ConfigId == 80007 || item.ConfigId == 40006)//玛雅装备不能回收
            {
                return false;
            }
            if (item.ConfigId == 320106 || item.ConfigId == 320097 || item.ConfigId == 320098 || item.ConfigId == 320004 || item.ConfigId == 320005 || item.ItemType == (int)E_ItemType.SkillBooks
                || item.ItemType == (int)E_ItemType.QiZhi)
                return true;
            if (item.IsCanSell() == false) return false;
            return item.ExecllentEntryDic.Count == 0 && item.GetProperValue(E_ItemValue.SetId) == 0
            && item.ItemType != (int)E_ItemType.Gemstone && item.ItemType != (int)E_ItemType.Other
            && item.ItemType != (int)E_ItemType.Pet;
        }
        //是否是单条卓越词条
        public static bool IsSingleExcellence(this KnapsackDataItem item)
        {
            if (SpecialGood(item) == false)
            {
                return false;
            }
            return item.ExecllentEntryDic.Count == 1;
        }
        public static bool IsDoubleExcellence(this KnapsackDataItem item)
        {
            if (SpecialGood(item) == false)
            {
                return false;
            }
            return item.ExecllentEntryDic.Count == 2;
        }
        /// <summary>
        /// 回收技能书
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsSkillBook(this KnapsackDataItem item)
        {
            if (SpecialGood(item) == false)
            {
                return false;
            }
            return item.ItemType == (int)E_ItemType.SkillBooks;
        }
        /// <summary>
        /// 回收强化装备
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsIntensifyEquip(this KnapsackDataItem item)
        {
            if (SpecialGood(item) == false)
            {
                return false;
            }
            if (item.ConfigId == 30005 || item.ConfigId == 80007 || item.ConfigId == 40006)//玛雅装备不能回收
            {
                return false;
            }
            return item.GetProperValue(E_ItemValue.Level) > 0;
        }
        public static bool IsRed_BlueMedicine(this KnapsackDataItem item)
        {
            return recycleMedicine.Contains(item.ConfigId);
        }

        /// <summary>
        /// 是否带技能/幸运
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool IsSkill(this KnapsackDataItem item)
        {
            if (SpecialGood(item) == false)
            {
                return false;
            }
            if (item.ConfigId == 30005 || item.ConfigId == 80007 || item.ConfigId == 40006)//玛雅装备不能回收
            {
                return false;
            }
            if (item.ItemType <= (int)E_ItemType.Boots)//属于装备类
            {
                //没有卓越，套装，强化，追加
                if (item.ExecllentEntryDic.Count > 0 || item.GetProperValue(E_ItemValue.SetId) != 0 || item.GetProperValue(E_ItemValue.OptLevel) != 0 || item.GetProperValue(E_ItemValue.Level) != 0)
                {
                    return false;
                }
                if (item.GetProperValue(E_ItemValue.LuckyEquip) == 1 || item.GetProperValue(E_ItemValue.SkillId) != 0)
                {
                    return true;
                }
            }
            return false;
        }
        public static void Sava()
        {
            recycleEquipSetting.SingleExcellence = SingleExcellence;
            recycleEquipSetting.WhiteSuit = WhiteSuit;
            recycleEquipSetting.DoubleExcellence = DoubleExcellence;
            recycleEquipSetting.SkillBook = SkillBook;
            recycleEquipSetting.IntensifyEquip = IntensifyEquip;
            recycleEquipSetting.Red_BlueMedicine = Red_BlueMedicine;
            recycleEquipSetting.AutoRecycle = AutoRecycle;
            recycleEquipSetting.SkillId = IsSkillId;
            LocalDataJsonComponent.Instance.SavaData(recycleEquipSetting, LocalJsonDataKeys.RecycleEquipSetting);

        }

    }
}
