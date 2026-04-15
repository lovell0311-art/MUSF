using CustomFrameWork;
using ETModel;
using CustomFrameWork.Component;
using System.Linq;

namespace ETHotfix
{
    [FriendOf(typeof(RechargeGiftBundleComponent))]
    public static class RechargeGiftBundleComponentSystem
    {
        public static void OnInit(this RechargeGiftBundleComponent self)
        {
            if(self.allGiftBundle.Count != 0)
            {
                RechargeGiftBundle[] allGiftBundle = self.allGiftBundle.Values.ToArray();
                foreach(RechargeGiftBundle giftBundle in allGiftBundle)
                {
                    giftBundle.Dispose();
                }
                self.allGiftBundle.Clear();
            }

            ReadConfigComponent readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var typeConfigJsonDic = readConfig.GetJson<RechargeGiftBundle_TypeConfigJson>().JsonDic;
            var itemConfigJsonDic = readConfig.GetJson<RechargeGiftBundle_ItemInfoConfigJson>().JsonDic;

            foreach(var config in typeConfigJsonDic.Values)
            {
                RechargeGiftBundle giftBundle = Root.CreateBuilder.GetInstance<RechargeGiftBundle>();
                giftBundle.Awake(null);
                giftBundle.Init(config);
                self.allGiftBundle.Add(config.Id, giftBundle);
            }

            foreach (var config in itemConfigJsonDic.Values)
            {
                self.giftBundleId2ConfigId.Add(config.GiftBundleId,config.Id);
            }
        }


    }
}
