using System;
using System.Collections.Generic;
using System.Text;
using CustomFrameWork;
using CustomFrameWork.Baseic;
using CustomFrameWork.Component;
using ETModel;

namespace ETHotfix
{

    [EventMethod(typeof(ItemPriceComponent), EventSystemType.INIT)]
    public class ItemPriceComponentAwakeSystem : ITEventMethodOnInit<ItemPriceComponent>
    {
        public void OnInit(ItemPriceComponent self)
        {
            self.Load();
        }
    }

    [EventMethod(typeof(ItemPriceComponent), EventSystemType.LOAD)]
    public class ItemPriceComponentLoadSystem : ITEventMethodOnLoad<ItemPriceComponent>
    {
        public override void OnLoad(ItemPriceComponent self)
        {
            self.Load();
        }
    }

    public static class ItemPriceComponentSystem
    {
        public static bool Load(this ItemPriceComponent self)
        {
            var values = Root.MainFactory.GetCustomComponent<ReadConfigComponent>().GetJson<ItemCustom_PriceConfigJson>().JsonDic.Values;

            self.__ItemPriceDict.Clear();

            foreach (var conf in values)
            {
                var itemPrice = new ItemPrice();
                itemPrice.UseFormula = (conf.UseFormula == 1);

                {
                    try
                    {
                        itemPrice.Formula = new Maths.MathParser();
                        itemPrice.Formula.SetLocalVar("强化等级", 0);
                        itemPrice.Formula.SetLocalVar("配置等级", 0);
                        itemPrice.Formula.SetLocalVar("耐久", 0);
                        itemPrice.Formula.SetLocalVar("最大耐久", 0);
                        itemPrice.Formula.SetLocalVar("数量", 0);
                        itemPrice.Formula.SetLocalVar("单组数量", 0);
                        itemPrice.Formula.SetLocalVar("默认价格", 0);
                        itemPrice.Formula.SetLocalVar("Value", 0);

                        itemPrice.Formula.SetLocalVar("有卓越属性", 0);
                        itemPrice.Formula.SetLocalVar("有套装属性", 0);
                        itemPrice.Formula.SetLocalVar("有镶嵌孔洞", 0);

                        if (!itemPrice.Formula.Parse(conf.Formula))
                        {
                            Log.Error($"'ItemCustom_PriceConfig' 解析 Formula 出错，ConfigId = {conf.Id}  Formula = {conf.Formula}");
                            return false;
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Error($"'ItemCustom_PriceConfig' 解析 Formula 出错，ConfigId = {conf.Id}  Formula = {conf.Formula}\n{e}");
                        return false;
                    }
                }
                { 
                    itemPrice.Value = new List<int>(16);
                    itemPrice.Value.Add(conf.Value0);
                    itemPrice.Value.Add(conf.Value1);
                    itemPrice.Value.Add(conf.Value2);
                    itemPrice.Value.Add(conf.Value3);
                    itemPrice.Value.Add(conf.Value4);
                    itemPrice.Value.Add(conf.Value5);
                    itemPrice.Value.Add(conf.Value6);
                    itemPrice.Value.Add(conf.Value7);
                    itemPrice.Value.Add(conf.Value8);
                    itemPrice.Value.Add(conf.Value9);
                    itemPrice.Value.Add(conf.Value10);
                    itemPrice.Value.Add(conf.Value11);
                    itemPrice.Value.Add(conf.Value12);
                    itemPrice.Value.Add(conf.Value13);
                    itemPrice.Value.Add(conf.Value14);
                    itemPrice.Value.Add(conf.Value15);
                }

                self.__ItemPriceDict.Add(conf.Id, itemPrice);
            }

            return true;
        }

        public static ItemPrice Get(this ItemPriceComponent self,int configId)
        {
            self.__ItemPriceDict.TryGetValue(configId, out var conf);
            return conf;
        }
    }
}
