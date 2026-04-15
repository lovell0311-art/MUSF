using Codice.Client.Common.FsNodeReaders.ChangeTrackerService;
using ETModel;
using ILRuntime.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{

    public class PassportInfo
    {
        public PassportTask_PassportConfig config;

        public long rewardItem;
        public string item;
        public int money;
        public List<int> listid = new List<int>();//奖励物品的集合
        public string reward = null;
    }

    [ObjectSystem]
    public class UIPassportComponentAwakeSystem : AwakeSystem<UIPassportComponent>
    {
        public override void Awake(UIPassportComponent self)
        {
            ReferenceCollector rc = self.GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
            self.scrollRect1 = rc.GetImage("scroll1").GetComponent<ScrollRect>();
            self.scrollRect2 = rc.GetImage("scroll2").GetComponent<ScrollRect>();
            self.scrollRect3 = rc.GetImage("scroll3").GetComponent<ScrollRect>();
            self.scrollRect4 = rc.GetImage("scroll4").GetComponent<ScrollRect>();
            self.scrollRect5 = rc.GetImage("scroll5").GetComponent<ScrollRect>();
            self.taskProgess1 = rc.GetText("taskProgess1");
            self.taskProgess2 = rc.GetText("taskProgess2");
            self.actitiyTxt = rc.GetText("actitiyTxt");

            self.scrollViewInfo1 = ComponentFactory.Create<UICircularScrollView<PassportInfo>>(); // 生成奖励信息的循环滚动视图1
            self.scrollViewInfo1.ItemInfoCallBack = self.InitInfoCallBack; // 循环信息内容
            self.scrollViewInfo1.InitInfo(E_Direction.Horizontal, 1, 12, 0); // 水平滑动，1排，12个子对象，间隙为0
            self.scrollViewInfo1.IninContent(rc.GetGameObject("content1"), self.scrollRect1);// 初始化滑动信息

            self.scrollViewInfo2 = ComponentFactory.Create<UICircularScrollView<PassportInfo>>(); // 生成奖励信息的循环滚动视图2
            self.scrollViewInfo2.ItemInfoCallBack = self.InitInfoCallBack; // 循环信息内容
            self.scrollViewInfo2.InitInfo(E_Direction.Horizontal, 1, 12, 0); // 水平滑动，1排，12个子对象，间隙为0
            self.scrollViewInfo2.IninContent(rc.GetGameObject("content2"), self.scrollRect2);// 初始化滑动信息

            self.scrollViewInfo3 = ComponentFactory.Create<UICircularScrollView<PassportInfo>>(); // 生成奖励信息的循环滚动视图3
            self.scrollViewInfo3.ItemInfoCallBack = self.InitInfoCallBack; // 循环信息内容
            self.scrollViewInfo3.InitInfo(E_Direction.Horizontal, 1, 12, 0); // 水平滑动，1排，12个子对象，间隙为0
            self.scrollViewInfo3.IninContent(rc.GetGameObject("content3"), self.scrollRect3);// 初始化滑动信息

            self.scrollViewInfo4 = ComponentFactory.Create<UICircularScrollView<PassportInfo>>(); // 生成奖励信息的循环滚动视图4
            self.scrollViewInfo4.ItemInfoCallBack = self.InitInfoCallBack; // 循环信息内容
            self.scrollViewInfo4.InitInfo(E_Direction.Horizontal, 1, 12, 0); // 水平滑动，1排，12个子对象，间隙为0
            self.scrollViewInfo4.IninContent(rc.GetGameObject("content4"), self.scrollRect4);// 初始化滑动信息

            self.scrollViewInfo5 = ComponentFactory.Create<UICircularScrollView<PassportInfo>>(); // 生成奖励信息的循环滚动视图
            self.scrollViewInfo5.ItemInfoCallBack = self.InitInfoCallBack; // 循环信息内容
            self.scrollViewInfo5.InitInfo(E_Direction.Horizontal, 1, 12, 0); // 水平滑动，1排，12个子对象，间隙为0
            self.scrollViewInfo5.IninContent(rc.GetGameObject("content5"), self.scrollRect5);// 初始化滑动信息

            self.shop328Btn = rc.GetButton("ShopBtn328");
            self.shop688Btn = rc.GetButton("ShopBtn688");
            rc.GetButton("zhuanhuan").onClick.AddSingleListener(() =>
            {
                if (self.intType>=5)
                {
                    self.intType = 1;
                }
                else
                {

                    self.intType += 1;
                }
                Debug.Log("当前的数据type" + self.intType);
                self.Show(self.intType);
            });
            rc.GetButton("CloseBtn").onClick.AddSingleListener(() => UIComponent.Instance.Remove(UIType.UIPassport));// 关闭等级奖励面板
            //rc.GetButton("GetAllBtn").onClick.AddSingleListener(() =>
            //{
            //    self.OnOneAllRewardClick().Coroutine();
            //}); //监听一键领取按钮

            self.shop328Btn.onClick.AddSingleListener(() =>
            {
                self.OnShopClick(328).Coroutine();
            }); //监听一键领取按钮

            self.shop688Btn.onClick.AddSingleListener(() =>
            {
                self.OnShopClick(688).Coroutine();
            }); //监听一键领取按钮

            List<IConfig> rewards = ConfigComponent.Instance.GetAll(typeof(GameTask_RewardItemConfig)).ToList();

            List<PassportInfo> list1 = new List<PassportInfo>();
            List<PassportInfo> list2 = new List<PassportInfo>();
            List<PassportInfo> list3 = new List<PassportInfo>();
            List<PassportInfo> list4 = new List<PassportInfo>();
            List<PassportInfo> list5 = new List<PassportInfo>();
            for (int i = 1; i < 50; i++)
            {
                //获取便宜的配置
                PassportTask_PassportConfig config1 = ConfigComponent.Instance.GetItem<PassportTask_PassportConfig>(700000 + i);
                if (config1 != null)
                {
                   
                    if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magician)
                    {
                        if (config1.Spell==0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config1;
                            list1.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config1.Id;
                                info1.reward = config1.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Swordsman)
                    {
                        if (config1.Swordsman == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config1;
                            list1.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config1.Id;
                                info1.reward = config1.RewardItems;
                              //  Log.Debug("111这是记录的数据" + config1.RewardItems + ":::" + config1.Id);
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Archer)
                    {
                        if (config1.Archer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config1;
                            list1.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config1.Id;
                                info1.reward = config1.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magicswordsman)
                    {
                        if (config1.Spellsword == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config1;
                            list1.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config1.Id;
                                info1.reward = config1.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Holymentor)
                    {
                        if (config1.Holyteacher == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config1;
                            list1.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config1.Id;
                                info1.reward = config1.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Summoner)
                    {
                        if (config1.SummonWarlock == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config1;
                            list1.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config1.Id;
                                info1.reward = config1.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Gladiator)
                    {
                        if (config1.Combat == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config1;
                            list1.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config1.Id;
                                info1.reward = config1.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.GrowLancer)
                    {
                        if (config1.GrowLancer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config1;
                            list1.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                            if (gameTask != null)
                            {
                                info1.reward = config1.RewardItems;
                                info1.rewardItem = config1.Id;
                            }
                        }
                    }
                }

                //获取便宜的配置
                PassportTask_PassportConfig config2 = ConfigComponent.Instance.GetItem<PassportTask_PassportConfig>(700999 + i);
                if (config2 != null)
                {

                    if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magician)
                    {
                        if (config2.Spell == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config2;
                            list2.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config2.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config2.Id;
                                info1.reward = config2.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Swordsman)
                    {
                        if (config2.Swordsman == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config2;
                            list2.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config2.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config2.Id;
                                info1.reward = config2.RewardItems;

                           //     Log.Debug("这是记录的数据" + config2.RewardItems + ":::" + config2.Id);
                            }
                            else
                            {
                                info1.reward = "";
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Archer)
                    {
                        if (config2.Archer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config2;
                            list2.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config2.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config2.Id;
                                info1.reward = config2.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magicswordsman)
                    {
                        if (config2.Spellsword == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config2;
                            list2.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config2.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config2.Id;
                                info1.reward = config2.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Holymentor)
                    {
                        if (config2.Holyteacher == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config2;
                            list2.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config2.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config2.Id;
                                info1.reward = config2.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Summoner)
                    {
                        if (config2.SummonWarlock == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config2;
                            list2.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config2.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config2.Id;
                                info1.reward = config2.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Gladiator)
                    {
                        if (config2.Combat == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config2;
                            list2.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config2.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config2.Id;
                                info1.reward = config2.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.GrowLancer)
                    {
                        if (config2.GrowLancer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config2;
                            list2.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config2.Id);
                            if (gameTask != null)
                            {
                                info1.reward = config2.RewardItems;
                                info1.rewardItem = config2.Id;
                            }
                        }
                    }
                }


                //获取便宜的配置
                PassportTask_PassportConfig config3 = ConfigComponent.Instance.GetItem<PassportTask_PassportConfig>(701999 + i);
                if (config3 != null)
                {

                    if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magician)
                    {
                        if (config3.Spell == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config3;
                            list3.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config3.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config3.Id;
                                info1.reward = config3.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Swordsman)
                    {
                        if (config3.Swordsman == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config3;
                            list3.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config3.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config3.Id;
                                info1.reward = config3.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Archer)
                    {
                        if (config3.Archer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config3;
                            list3.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config3.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config3.Id;
                                info1.reward = config3.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magicswordsman)
                    {
                        if (config3.Spellsword == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config3;
                            list3.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config3.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config3.Id;
                                info1.reward = config3.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Holymentor)
                    {
                        if (config3.Holyteacher == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config3;
                            list3.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config3.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config3.Id;
                                info1.reward = config3.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Summoner)
                    {
                        if (config3.SummonWarlock == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config3;
                            list3.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config3.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config3.Id;
                                info1.reward = config3.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Gladiator)
                    {
                        if (config3.Combat == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config3;
                            list3.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config3.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config3.Id;
                                info1.reward = config3.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.GrowLancer)
                    {
                        if (config3.GrowLancer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config3;
                            list3.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config3.Id);
                            if (gameTask != null)
                            {
                                info1.reward = config3.RewardItems;
                                info1.rewardItem = config3.Id;
                            }
                        }
                    }
                }


                //获取便宜的配置
                PassportTask_PassportConfig config4 = ConfigComponent.Instance.GetItem<PassportTask_PassportConfig>(702999 + i);
                if (config4 != null)
                {

                    if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magician)
                    {
                        if (config4.Spell == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config4;
                            list4.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config4.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config4.Id;
                                info1.reward = config4.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Swordsman)
                    {
                        if (config4.Swordsman == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config4;
                            list4.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config4.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config4.Id;
                                info1.reward = config4.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Archer)
                    {
                        if (config4.Archer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config4;
                            list4.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config4.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config4.Id;
                                info1.reward = config4.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magicswordsman)
                    {
                        if (config4.Spellsword == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config4;
                            list4.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config4.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config4.Id;
                                info1.reward = config4.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Holymentor)
                    {
                        if (config4.Holyteacher == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config4;
                            list4.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config4.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config4.Id;
                                info1.reward = config4.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Summoner)
                    {
                        if (config4.SummonWarlock == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config4;
                            list4.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config4.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config4.Id;
                                info1.reward = config4.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Gladiator)
                    {
                        if (config4.Combat == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config4;
                            list4.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config4.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config4.Id;
                                info1.reward = config4.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.GrowLancer)
                    {
                        if (config4.GrowLancer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config4;
                            list4.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config4.Id);
                            if (gameTask != null)
                            {
                                info1.reward = config4.RewardItems;
                                info1.rewardItem = config4.Id;
                            }
                        }
                    }
                }


                //获取便宜的配置
                PassportTask_PassportConfig config5 = ConfigComponent.Instance.GetItem<PassportTask_PassportConfig>(703999 + i);
                if (config5 != null)
                {

                    if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magician)
                    {
                        if (config5.Spell == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config5;
                            list5.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config5.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config5.Id;
                                info1.reward = config5.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Swordsman)
                    {
                        if (config5.Swordsman == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config5;
                            list5.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config5.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config5.Id;
                                info1.reward = config5.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Archer)
                    {
                        if (config5.Archer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config5;
                            list5.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config5.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config5.Id;
                                info1.reward = config5.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magicswordsman)
                    {
                        if (config5.Spellsword == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config5;
                            list5.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config5.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config5.Id;
                                info1.reward = config5.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Holymentor)
                    {
                        if (config5.Holyteacher == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config5;
                            list5.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config5.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config5.Id;
                                info1.reward = config5.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Summoner)
                    {
                        if (config5.SummonWarlock == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config5;
                            list5.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config5.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config5.Id;
                                info1.reward = config5.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Gladiator)
                    {
                        if (config5.Combat == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config5;
                            list5.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config5.Id);
                            if (gameTask != null)
                            {
                                info1.rewardItem = config5.Id;
                                info1.reward = config5.RewardItems;
                            }
                        }
                    }
                    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.GrowLancer)
                    {
                        if (config5.GrowLancer == 0)
                        {
                            PassportInfo info1 = new PassportInfo();
                            info1.config = config5;
                            list5.Add(info1);
                            info1.money = 328;
                            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config5.Id);
                            if (gameTask != null)
                            {
                                info1.reward = config5.RewardItems;
                                info1.rewardItem = config5.Id;
                            }
                        }
                    }
                }


                ////获取便宜的配置
                //PassportTask_PassportConfig config1 = ConfigComponent.Instance.GetItem<PassportTask_PassportConfig>(700000 + i);
                //if (config1 != null)
                //{

                //    if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magician)
                //    {
                //        if (config1.Spell == 0)
                //        {
                //            PassportInfo info1 = new PassportInfo();
                //            info1.config = config1;
                //            list1.Add(info1);
                //            info1.money = 328;
                //            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                //            if (gameTask != null)
                //            {
                //                info1.rewardItem = config1.Id;
                //                info1.reward = config1.RewardItems;
                //            }
                //        }
                //    }
                //    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Swordsman)
                //    {
                //        if (config1.Swordsman == 0)
                //        {
                //            PassportInfo info1 = new PassportInfo();
                //            info1.config = config1;
                //            list1.Add(info1);
                //            info1.money = 328;
                //            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                //            if (gameTask != null)
                //            {
                //                info1.rewardItem = config1.Id;
                //                info1.reward = config1.RewardItems;
                //            }
                //        }
                //    }
                //    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Archer)
                //    {
                //        if (config1.Archer == 0)
                //        {
                //            PassportInfo info1 = new PassportInfo();
                //            info1.config = config1;
                //            list1.Add(info1);
                //            info1.money = 328;
                //            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                //            if (gameTask != null)
                //            {
                //                info1.rewardItem = config1.Id;
                //                info1.reward = config1.RewardItems;
                //            }
                //        }
                //    }
                //    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Magicswordsman)
                //    {
                //        if (config1.Spellsword == 0)
                //        {
                //            PassportInfo info1 = new PassportInfo();
                //            info1.config = config1;
                //            list1.Add(info1);
                //            info1.money = 328;
                //            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                //            if (gameTask != null)
                //            {
                //                info1.rewardItem = config1.Id;
                //                info1.reward = config1.RewardItems;
                //            }
                //        }
                //    }
                //    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Holymentor)
                //    {
                //        if (config1.Holyteacher == 0)
                //        {
                //            PassportInfo info1 = new PassportInfo();
                //            info1.config = config1;
                //            list1.Add(info1);
                //            info1.money = 328;
                //            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                //            if (gameTask != null)
                //            {
                //                info1.rewardItem = config1.Id;
                //                info1.reward = config1.RewardItems;
                //            }
                //        }
                //    }
                //    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Summoner)
                //    {
                //        if (config1.SummonWarlock == 0)
                //        {
                //            PassportInfo info1 = new PassportInfo();
                //            info1.config = config1;
                //            list1.Add(info1);
                //            info1.money = 328;
                //            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                //            if (gameTask != null)
                //            {
                //                info1.rewardItem = config1.Id;
                //                info1.reward = config1.RewardItems;
                //            }
                //        }
                //    }
                //    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.Gladiator)
                //    {
                //        if (config1.Combat == 0)
                //        {
                //            PassportInfo info1 = new PassportInfo();
                //            info1.config = config1;
                //            list1.Add(info1);
                //            info1.money = 328;
                //            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                //            if (gameTask != null)
                //            {
                //                info1.rewardItem = config1.Id;
                //                info1.reward = config1.RewardItems;
                //            }
                //        }
                //    }
                //    else if (UnitEntityComponent.Instance.LocalRole.RoleType == E_RoleType.GrowLancer)
                //    {
                //        if (config1.GrowLancer == 0)
                //        {
                //            PassportInfo info1 = new PassportInfo();
                //            info1.config = config1;
                //            list1.Add(info1);
                //            info1.money = 328;
                //            GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config1.Id);
                //            if (gameTask != null)
                //            {
                //                info1.reward = config1.RewardItems;
                //                info1.rewardItem = config1.Id;
                //            }
                //        }
                //    }
                //}

                ////获取贵的配置
                //PassportTask_PassportConfig config2 = ConfigComponent.Instance.GetItem<PassportTask_PassportConfig>(701000 + i);
                //if (config2 != null)
                //{
                //    PassportInfo info2 = new PassportInfo();
                //    info2.config = config2;
                //    list2.Add(info2);

                //    GameTask_RewardItemConfig gameTask = (GameTask_RewardItemConfig)rewards.Find(p => ((GameTask_RewardItemConfig)p).TaskId == config2.Id);
                //    if (gameTask != null)
                //    {
                //        info2.rewardItem = gameTask.Id;
                //    }
                //    info2.money = 688;
                //}
            }
            self.scrollViewInfo1.Items = list1;
            self.scrollViewInfo2.Items = list2;
            self.scrollViewInfo3.Items = list3;
            self.scrollViewInfo4.Items = list4;
            self.scrollViewInfo5.Items = list5;
            self.UpdateView();
            self.SyncActivityTime().Coroutine();
        }
    }

    public static class UIPassportComponentSystem
    {

        public static async ETTask SyncActivityTime(this UIPassportComponent self)
        {
            int activityId = 13;
            var response = (G2C_OpenMiracleActivitiesResponse)await SessionComponent.Instance.Session.Call(new C2G_OpenMiracleActivitiesRequest { MiracleActivitiesID = activityId });
            Log.DebugBrown("通行证的错误码" + response.Error);
            if (response.Error != ErrorCode.ERR_Success)
            {
                self.Open = false;
                UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
                UIComponent.Instance.Remove(UIType.UIHint);
            }
            else
            {
                if (response.Info == null)
                {
                    UIComponent.Instance.Remove(UIType.UIPassport);
                    return;
                }
                self.Open = true;
                //Activity_InfoConfig config = ConfigComponent.Instance.GetItem<Activity_InfoConfig>(activityId.ToInt32());
                //self.actitiyTxt.text = "活动开始时间：" + config.OpenTime + "  结束时间：" + config.EndTime;
            }
        }

        public static void Show(this UIPassportComponent self,int index)
        {
            self.scrollRect1.gameObject.SetActive(index == 1);
            self.scrollRect2.gameObject.SetActive(index == 2);
            self.scrollRect3.gameObject.SetActive(index == 3);
            self.scrollRect4.gameObject.SetActive(index == 4);
            self.scrollRect5.gameObject.SetActive(index == 5);
            if (index==1)
            {
                self.actitiyTxt.text = "大天使武器";
            }
            else if (index==2)
            {
                self.actitiyTxt.text = "大天使项链";
            }
            else if (index == 3)
            {
                self.actitiyTxt.text = "大天使戒指";
            }
            else if (index == 4)
            {
                self.actitiyTxt.text = "大天使手环";
            }
            else if (index == 5)
            {
                self.actitiyTxt.text = "大天使旗帜";
            }
        }
        public static void UpdateView(this UIPassportComponent self)
        {
            //获取当前任务
            self.shop328Btn.gameObject.SetActive(TaskDatas.PassportTaskInfo328 == null);
            // self.shop688Btn.gameObject.SetActive(TaskDatas.PassportTaskInfo688 == null);

            self.scrollViewInfo1.Items = self.scrollViewInfo1.Items;
            self.scrollViewInfo2.Items = self.scrollViewInfo2.Items;
            self.scrollViewInfo3.Items = self.scrollViewInfo3.Items;
            self.scrollViewInfo4.Items = self.scrollViewInfo4.Items;
            self.scrollViewInfo5.Items = self.scrollViewInfo5.Items;
            self.taskProgess1.text = "";
            self.taskProgess2.text = "";
            if (TaskDatas.PassportTaskInfo328 != null)
            {
                self.taskProgess1.text = TaskDatas.PassportTaskInfo328.TaskName + $"\t{TaskDatas.PassportTaskInfo328.KillMonsterCount}/{TaskDatas.PassportTaskInfo328.TaskTargetCounts}";
            }
            if (TaskDatas.PassportTaskInfo688 != null)
            {
                self.taskProgess2.text = TaskDatas.PassportTaskInfo688.TaskName + $"\t{TaskDatas.PassportTaskInfo688.KillMonsterCount}/{TaskDatas.PassportTaskInfo688.TaskTargetCounts}";
            }
        }

        public static async ETTask OnShopClick(this UIPassportComponent self, int money)
        {
            int passId = 700001;
            if (self.intType==1)
            {
                passId = 700001;
            }
            else if (self.intType == 2)
            {
                passId = 701000;
            }
            else if (self.intType == 3)
            {
                passId = 702000;
            }
            else if (self.intType == 4)
            {
                passId = 703000;
            }
            else if (self.intType == 5)
            {
                passId = 704000;
            }
            if (money == 688)
            {
                passId = 701001;
            }
            if (self.Open == false)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "13该活动尚未开启");
                return;
            }
            var response = (G2C_PurchaseOfTradeCard)await SessionComponent.Instance.Session.Call(new C2G_PurchaseOfTradeCard
            {
                PassId = passId
            });
            Log.Info(JsonHelper.ToJson(response));
            if (response.Error != ErrorCode.ERR_Success)
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, response.Error.GetTipInfo());
            }
            else
            {
                UIComponent.Instance.VisibleUI(UIType.UIHint, "购买成功！");
            }
            Log.Info("ONShopClick");
            self.UpdateView();
        }

        public static async ETTask OnOneAllRewardClick(this UIPassportComponent self)
        {

            await ETTask.CompletedTask;
        }


        public static void InitInfoCallBack(this UIPassportComponent self, GameObject go, PassportInfo Info)
        {

            go.transform.Find("name").GetComponent<Text>().text = Info.config.TaskName;
            go.transform.Find("freename").GetComponent<Text>().text = Info.config.TaskDes;
            GameObject freeobj = go.transform.Find("freeobj").gameObject;
            freeobj.transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            freeobj.transform.GetChild(0).GetChild(3).GetComponent<Text>().text = Info.config.RewardCoin.ToString();

            freeobj.transform.GetChild(1).gameObject.SetActive(true);
            freeobj.transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
            freeobj.transform.GetChild(1).GetChild(3).GetComponent<Text>().text = Info.config.RewardExp.ToString();

            //freeobj.transform.GetChild(2).gameObject.SetActive(true);
            //freeobj.transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
            //freeobj.transform.GetChild(2).GetChild(3).GetComponent<Text>().text = Info.config.RewardExp.ToString();
            freeobj.transform.GetChild(2).gameObject.SetActive(false);
            freeobj.transform.GetChild(3).gameObject.SetActive(false);
            for (int i = 0; i < freeobj.transform.GetChild(3).childCount; i++)
            {
                if (i>3)
                {
                    GameObject.Destroy(freeobj.transform.GetChild(3).GetChild(i).gameObject);
                }
            }
            for (int i = 0; i < freeobj.transform.GetChild(2).childCount; i++)
            {
                if (i > 3)
                {
                    GameObject.Destroy(freeobj.transform.GetChild(2).GetChild(i).gameObject);
                }
            }
           
            IConfig[] pack = ConfigComponent.Instance.GetAll<GameTask_RewardItemConfig>();
            int index = 2;

            if (Info.config.RewardItems.Length>0)
            {
                string[] arr = Info.config.RewardItems.Split(',');
                for (int i = 0; i < arr.Length; i++)
                {
                    foreach (var items in pack.Cast<GameTask_RewardItemConfig>())
                    {
                        if (items.Id == int.Parse(arr[i]))
                        {
                            if (items.Id != 0)
                            {
                                GameTask_RewardItemConfig config = ConfigComponent.Instance.GetItem<GameTask_RewardItemConfig>((int)items.Id);
                                freeobj.transform.GetChild(index).gameObject.SetActive(true);
                                ((long)config.ItemId).GetItemInfo_Out(out Item_infoConfig item_Info);
                                //Log.DebugBrown("通行证的奖励" + Info.rewardItem + ":::");
                                //Log.DebugBrown("通行证的奖励任务id" + item_Info.Id + "::奖励id" + config.ItemId);
                                GameObject g = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
                                if (g == null)
                                {
                                    g = ResourcesComponent.Instance.LoadGameObject("Weapon_borenjian".StringToAB(), "Weapon_borenjian");
                                }
                                g.SetUI();
                                g.transform.parent = freeobj.transform.GetChild(index);
                                g.transform.localPosition = Vector3.zero;
                                g.transform.rotation = Quaternion.identity;
                                g.transform.localScale = new Vector3(60, 60, 60);
                                freeobj.transform.GetChild(index).GetChild(3).GetComponent<Text>().text = item_Info.Name + "X1";
                            }
                            else
                            {
                                freeobj.transform.GetChild(index).gameObject.SetActive(false);
                            }
                            index += 1;
                        }
                    }
                }
            }
            //foreach (var items in pack.Cast<GameTask_RewardItemConfig>())
            //{
            //    if (items.TaskId == Info.rewardItem)
            //    {
            //       // freeobj.transform.GetChild(index).gameObject.SetActive(true);

            //        if (items.Id != 0)
            //        {
            //            GameTask_RewardItemConfig config = ConfigComponent.Instance.GetItem<GameTask_RewardItemConfig>((int)items.Id);
            //            freeobj.transform.GetChild(index).gameObject.SetActive(true);
            //            ((long)config.ItemId).GetItemInfo_Out(out Item_infoConfig item_Info);
            //            Log.DebugBrown("通行证的奖励" + Info.rewardItem + ":::");
            //            Log.DebugBrown("通行证的奖励任务id" + item_Info.Id + "::奖励id" + config.ItemId);
            //            GameObject g = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
            //            if (g == null)
            //            {
            //                g = ResourcesComponent.Instance.LoadGameObject("Weapon_borenjian".StringToAB(), "Weapon_borenjian");
            //            }
            //            g.SetUI();
            //            g.transform.parent = freeobj.transform.GetChild(index);
            //            g.transform.localPosition = Vector3.zero;
            //            g.transform.rotation = Quaternion.identity;
            //            g.transform.localScale = new Vector3(60, 60, 60);
            //            freeobj.transform.GetChild(index).GetChild(3).GetComponent<Text>().text = item_Info.Name + "X1";
            //        }
            //        else
            //        {
            //            freeobj.transform.GetChild(index).gameObject.SetActive(false);
            //        }
            //        //freeobj.transform.GetChild(index).GetChild(0).gameObject.SetActive(true);
            //        //freeobj.transform.GetChild(index).GetChild(3).GetComponent<Text>().text = Info.config.RewardExp.ToString();
            //        index += 1;
            //        Log.DebugBrown("kk" +items.Id);
            //    }
            //}
            //在奖励表中查询--是否存在奖励物品
            //if (Info.rewardItem != 0)
            //{
            //    GameTask_RewardItemConfig config = ConfigComponent.Instance.GetItem<GameTask_RewardItemConfig>((int)Info.rewardItem);
            //    freeobj.transform.GetChild(3).gameObject.SetActive(true);
            //    ((long)config.ItemId).GetItemInfo_Out(out Item_infoConfig item_Info);
            //    Log.DebugBrown("通行证的奖励" + Info.rewardItem + ":::");
            //    Log.DebugBrown("通行证的奖励任务id" + item_Info.Id + "::奖励id"+ config.ItemId);
            //    GameObject g = ResourcesComponent.Instance.LoadGameObject(item_Info.ResName.StringToAB(), item_Info.ResName);
            //    if (g == null)
            //    {
            //        g = ResourcesComponent.Instance.LoadGameObject("Weapon_borenjian".StringToAB(), "Weapon_borenjian");
            //    }
            //    g.SetUI();
            //    g.transform.parent = freeobj.transform.GetChild(3);
            //    g.transform.localPosition = Vector3.zero;
            //    g.transform.rotation = Quaternion.identity;
            //    g.transform.localScale = new Vector3(60, 60, 60);
            //    freeobj.transform.GetChild(3).GetChild(3).GetComponent<Text>().text = item_Info.Name+ "X1";
            //}
            //else
            //{
            //    freeobj.transform.GetChild(3).gameObject.SetActive(false);
            //}

            var mask = go.transform.Find("mask");
            var reward = go.transform.Find("reward");
            var rewardBtn = go.transform.Find("rewardBtn");
            if (mask)
            {
                TaskInfo taskInfo = TaskDatas.PassportTaskInfo328;
                if (Info.config.Id / 1000 == 701)
                {
                    taskInfo = TaskDatas.PassportTaskInfo688;
                }

                if (taskInfo != null && taskInfo.Id / 1000 == Info.config.Id / 1000)
                {

                    if (taskInfo.Id == Info.config.Id)
                    {
                        //|| taskInfo.State == TaskStatus.Faulted
                     //   mask.gameObject.SetActive(taskInfo.State != 1);
                        reward.gameObject.SetActive(taskInfo.State == 3);
                        rewardBtn.gameObject.SetActive(taskInfo.State == 2);
                        rewardBtn.GetComponent<Button>().onClick.AddSingleListener(async () =>
                        {
                            G2C_ReceiveTaskReward reward = (G2C_ReceiveTaskReward)await SessionComponent.Instance.Session.Call(new C2G_ReceiveTaskReward() { TaskId = (int)Info.config.Id });
                            if (reward.Error == ErrorCode.ERR_Success)
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, "领取成功！");
                                self.UpdateView();
                            }
                            else
                            {
                                UIComponent.Instance.VisibleUI(UIType.UIHint, reward.Error.GetTipInfo());
                            }
                        });
                    }
                    else
                    {
                       // mask.gameObject.SetActive(true);
                        reward.gameObject.SetActive(Info.config.Id < taskInfo.Id);
                        rewardBtn.gameObject.SetActive(false);
                    }
                }
                else
                {
                    rewardBtn.gameObject.SetActive(false);
                    //mask.gameObject.SetActive(true);
                    reward.gameObject.SetActive(false);
                }
            }
        }

    }
}
