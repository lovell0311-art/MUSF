using ETModel;
using CustomFrameWork;

namespace ETHotfix
{
    /// <summary>
    /// 翅膀
    /// </summary>
    public static partial class ItemSystem
    {
        /// <summary>
        /// 随机特殊词条
        /// </summary>
        /// <param name="self"></param>
        /// <param name="entryCount"></param>
        public static void RandSpecialEntry(this Item self,int entryCount)
        {
			self.data.SpecialEntry.Clear();
			if (self.ConfigData.SpecialAttrId.Count == 0) return;
			using ListComponent<int> entryList = ListComponent<int>.Create();
			entryList.AddRange(self.ConfigData.SpecialAttrId);
			for (int i = 0; i < entryList.Count; i++)
			{
				int randInt = RandomHelper.RandomNumber(0, entryList.Count);
				(entryList[i], entryList[randInt]) = (entryList[randInt], entryList[i]);
			}

			RandomSelector<int> selector = new RandomSelector<int>();
			selector.Add(0, 25);
			selector.Add(1, 100);
			selector.Add(2, 50);
			selector.Add(3, 10);

			for (int i = 0; i < entryList.Count && i < entryCount; i++)
			{
				selector.TryGetValue(out int randLevel);
				self.data.SpecialEntry.Add(entryList[i], randLevel);
			}
		}
    }
}
