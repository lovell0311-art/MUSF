using CustomFrameWork.Baseic;
using CustomFrameWork;
using ETModel;
using System;
using System.Linq;

namespace ETHotfix
{

    [FriendOf(typeof(RechargeGiftBundle))]
    public static class RechargeGiftBundleSystem
    {
        public static void Init(this RechargeGiftBundle self, RechargeGiftBundle_TypeConfig config)
        {
            self.config = config;

            self.startTime = 0;
            self.endTime = 0;

            try
            {
                self.startTime = Help_TimeHelper.DateConversionTime(Convert.ToDateTime(config.StartTime));
            }
            catch (Exception e)
            {
                Log.Error($"'RechargeGiftBundle_TypeConfig' 配置错误，无法解析 'StartTime' 字段。config.Id={config.Id}");
                return;
            }

            self.endTime = self.startTime + config.DurationTime;
        }

        public static void OnDispose(this RechargeGiftBundle self)
        {
            if(self.configId2Item.Count != 0)
            {
                Item[] allItem = self.configId2Item.Values.ToArray();
                foreach(Item item in allItem)
                {
                    item.Dispose();
                }
                self.configId2Item.Clear();
            }
        }

    }
}
