using System;
using System.Collections.Generic;
using System.Text;
using ETModel;
using CustomFrameWork;
using CustomFrameWork.Component;
using CustomFrameWork.Baseic;
using System.Linq;

namespace ETHotfix
{
    [EventMethod(typeof(ItemCustomDropComponent), EventSystemType.INIT)]
    public class ItemCustomDropComponentInitSystem : ITEventMethodOnInit<ItemCustomDropComponent>
    {
        public void OnInit(ItemCustomDropComponent self)
        {
            self.OnInit();
        }
    }

    [EventMethod(typeof(ItemCustomDropComponent), EventSystemType.LOAD)]
    public class ItemCustomDropComponentLoadSystem : ITEventMethodOnLoad<ItemCustomDropComponent>
    {
        public override void OnLoad(ItemCustomDropComponent self)
        {
            self.OnInit();
        }
    }



    public static class ItemCustomDropComponentSystem
    {
        public static void OnInit(this ItemCustomDropComponent self)
        {
            self.ItemDict.Clear();
            self.DropTypeSelector.Clear();

            // 掉落类型权重初始化
            var readConfig = Root.MainFactory.GetCustomComponent<ReadConfigComponent>();
            var dropTypeDict = readConfig.GetJson<ItemCustomDrop_TypeConfigJson>().JsonDic;
            foreach(var config in dropTypeDict.Values)
            {
                self.DropTypeSelector.Add(config.Id, config.DropRate);
            }

            #region 范围查找
            // 怪物id to 索引
            SortedDictionary<RangeKey, int> monstarId2Index = new SortedDictionary<RangeKey, int>();

            List<int> monsterIdList = readConfig.GetJson<Enemy_InfoConfigJson>().JsonDic.Keys.ToList();
            monsterIdList.Sort();
            int monsterIdListCount = monsterIdList.Count;

            for (int index = 0,count = monsterIdList.Count; index < count;++index)
            {
                monstarId2Index.Add(new RangeKey(monsterIdList[index]), index);
            }

            void ForeachMonsterId(int start,int end,Action<int> action)
            {
                RangeKey key = new RangeKey(start, end);
                if(monstarId2Index.TryGetValue(key, out var index))
                {
                    // 有匹配id的怪物
                    for(int i = index; i< monsterIdListCount;++i)
                    {
                        if (monsterIdList[i] > end) break;
                        action(monsterIdList[i]);
                    }
                    for (int i = (index - 1); i >= 0; --i)
                    {
                        if (monsterIdList[i] < start) break;
                        action(monsterIdList[i]);
                    }
                }
            }
            #endregion


            // 等级掉落缓存
            MultiMap<int, ItemCustomDrop_InfoConfig> levelDropCache = new MultiMap<int, ItemCustomDrop_InfoConfig>();

            var dropInfoDict = readConfig.GetJson<ItemCustomDrop_InfoConfigJson>().JsonDic;
            foreach(var config in dropInfoDict.Values)
            {
                if(config.MonsterId > 0)
                {
                    self.AddDropItemToMonster(config.MonsterId, config);
                    continue;
                }
                if(config.MonsterIdRange.Count != 0)
                {
                    // TODO 检查怪物id范围，格式是否正确
                    if (config.MonsterIdRange.Count != 2)
                    {
                        throw new Exception($"CustomItemDrop_InfoConfig 配置错误 MonsterIdRange.Count != 2。config.Id={config.Id}");
                    }
                    if (config.MonsterIdRange[0] > config.MonsterIdRange[1])
                    {
                        throw new Exception($"CustomItemDrop_InfoConfig 配置错误 MonsterIdRange[0] > MonsterIdRange[1]。config.Id={config.Id}");
                    }
                    if (config.MonsterIdRange[0] < 0)
                    {
                        throw new Exception($"CustomItemDrop_InfoConfig 配置错误 MonsterIdRange[0] < 0。config.Id={config.Id}");
                    }

                    ForeachMonsterId(
                        config.MonsterIdRange[0],
                        config.MonsterIdRange[1],
                        (int id)=>{
                            self.AddDropItemToMonster(id, config);
                        });
                }
                else
                {
                    // TODO 检查等级范围，格式是否正确
                    if (config.MonsterLevel.Count != 2)
                    {
                        throw new Exception($"CustomItemDrop_InfoConfig 配置错误 MonsterLevel.Count != 2。config.Id={config.Id}");
                    }
                    if (config.MonsterLevel[0] > config.MonsterLevel[1])
                    {
                        throw new Exception($"CustomItemDrop_InfoConfig 配置错误 MonsterLevel[0] > MonsterLevel[1]。config.Id={config.Id}");
                    }
                    if (config.MonsterLevel[0] < 0)
                    {
                        throw new Exception($"CustomItemDrop_InfoConfig 配置错误 MonsterLevel[0] < 0。config.Id={config.Id}");
                    }

                    // TODO 将包含的等级，添加到 levelDropCache 中
                    for (int level = config.MonsterLevel[0]; level <= config.MonsterLevel[1]; level++)
                    {
                        levelDropCache.Add(level, config);
                    }
                }
            }

            var enemyInfoDict = readConfig.GetJson<Enemy_InfoConfigJson>().JsonDic;
            foreach(var enemyConfig in enemyInfoDict.Values)
            {
                if(levelDropCache.ContainsKey(enemyConfig.Lvl))
                {
                    foreach (var config in levelDropCache[enemyConfig.Lvl])
                    {
                        self.AddDropItemToMonster(enemyConfig.Id, config);
                    }
                }
            }
        }


        public static void AddDropItemToMonster(this ItemCustomDropComponent self,int monsterId, ItemCustomDrop_InfoConfig config)
        {
            if(!self.ItemDict.TryGetValue(monsterId,out var dropType2dropItem))
            {
                dropType2dropItem = new Dictionary<int, RandomSelector<int>>();
                self.ItemDict.Add(monsterId,dropType2dropItem);
            }
            if(!dropType2dropItem.TryGetValue(config.DropType,out var selector))
            {
                selector = new RandomSelector<int>();
                dropType2dropItem.Add(config.DropType, selector);
            }
            selector.Add(config.Id, config.DropRate);
        }

    }
}
