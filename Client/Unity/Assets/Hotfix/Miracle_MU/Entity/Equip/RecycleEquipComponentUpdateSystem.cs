using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETHotfix
{
    [ObjectSystem]
    public class RecycleEquipComponentAwakeSystem : AwakeSystem<RecycleEquipComponent>
    {
        public override void Awake(RecycleEquipComponent self)
        {

        }
    }


    [ObjectSystem]
    public class RecycleEquipComponentUpdateSystem : UpdateSystem<RecycleEquipComponent>
    {
        public override void Update(RecycleEquipComponent self)
        {
            //Log.Info("------------------------------------");
            if (UIMainComponent.Instance != null)
            {
                if (UIMainComponent.Instance.HookTog.isOn && RecycleEquipTools.AutoRecycle)
                {
                    self.recycleTime -= Time.deltaTime;
                    if (self.recycleTime < 0)
                    {
                        self.recycleTime = 3;
                        List<KnapsackDataItem> RecycleEquipList = new List<KnapsackDataItem>();

                        var list = KnapsackItemsManager.KnapsackItems.Values.ToList();
                        foreach (var item in list)
                        {
                            if (RecycleEquipTools.SingleExcellence && item.IsSingleExcellence()) // 是否是单条卓越词条
                            {
                                RecycleEquipList.Add(item);
                            }

                            if (RecycleEquipTools.WhiteSuit && item.IsWhiteSuit()) //是否是白装
                            {
                                RecycleEquipList.Add(item);
                            }

                            if (RecycleEquipTools.DoubleExcellence && item.IsDoubleExcellence()) //双词条卓越
                            {
                                RecycleEquipList.Add(item);
                            }

                            if (RecycleEquipTools.SkillBook && item.IsSkillBook()) //技能书
                            {
                                RecycleEquipList.Add(item);
                            }

                            if (RecycleEquipTools.IntensifyEquip && item.IsIntensifyEquip()) //强化装备
                            {
                                RecycleEquipList.Add(item);
                            }

                            if (RecycleEquipTools.Red_BlueMedicine && item.IsRed_BlueMedicine()) //小瓶红、蓝药水
                            {
                                RecycleEquipList.Add(item);
                            }
                            if (RecycleEquipTools.IsSkillId && item.IsSkill()) //小瓶红、蓝药水
                            {
                                RecycleEquipList.Add(item);
                            }
                        }

                        Log.Info("挂机状态中 需要回收的数量--- " + RecycleEquipList.Count);
                        //查询白装是否开启自动回收
                        RecycleEquipList.ForEach((item) =>
                        {
                            RecycleEquipTools.AutoSell(item);
                        });

                        RecycleEquipList.Clear();
                    }
                }

            }
        }
    }
}
