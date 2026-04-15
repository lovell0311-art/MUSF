const item_config = require('../config/item_config');


const EDBItemValue ={
    Durability: 10,
    Level: 12,
    SkillId: 13,
    IsBind: 14,
    IsTask: 15,
    ValidTime: 16,
    MountsLevel: 30,
    MountsExp: 31,
    Quantity: 43,
    OptValue: 48,
    OptLevel: 49,
    LuckyEquip: 51,
    OrecycledID: 56,
    OrecycledLevel: 57,
    SetId: 50,
    FluoreAttr: 70,
    FluoreSlotCount: 71,
    FluoreSlot1: 72,
    FluoreSlot2: 73,
    FluoreSlot3: 74,
    FluoreSlot4: 75,
    FluoreSlot5: 76,
};

const EItemType = {
    None: 0,
    Swords: 1,
    Axes: 2,
    Maces: 3,
    Bows: 4,
    Crossbows: 5,
    Arrow: 6,
    Spears: 7,
    Staffs: 8,
    MagicBook: 9,
    Scepter: 10,
    RuneWand: 11,
    FistBlade: 12,
    MagicSword: 13,
    ShortSword: 14,
    MagicGun: 15,
    Shields: 16,
    Helms: 17,
    Armors: 18,
    Pants: 19,
    Gloves: 20,
    Boots: 21,
    Wing: 22,
    Necklace: 23,
    Rings: 24,
    Dangler: 25,
    Mounts: 26,
    FGemstone: 27,
    Gemstone: 28,
    SkillBooks: 29,
    Guard: 30,
    Consumables: 31,
    Other: 32,
    Task: 33,
    Flag: 34,
    Pet: 35,
    Bracelet: 36,
};

// 品质类型
const EQualityType = {
    Skill: 1,
    Normal: 2,
    Lucky: 4,
    Excellent: 8,
    Set: 16,
    Socket: 32,
    Purple: 64,
};

const EItemInComponent =
{
    None: 0,    // 未知的
    Map: 1,        // 地图
    Backpack: 2,   // 背包
    Equipment: 3,  // 装备栏
    Warehouse: 4,  // 仓库
    Stall: 5,      // 摊位
}

class Item
{
    constructor(itemData){
        if(itemData == null)
        {
            return;
        }
        // 基础属性
        this.Uid = itemData.ItemUid;
        this.Config = item_config[itemData.ConfigID];
        this.ExcellentEntry = itemData.ExcellentEntry;
        this.SpecialEntry = {};
        itemData.SpecialEntry.forEach((item,index)=>{
            this.SpecialEntry[item[0]] = item[1];
        })
        this.PropertyData = {};
        itemData.PropertyData.forEach((item,index)=>{
            this.PropertyData[item[0]] = item[1];
        });
        this.DurabilitySmall = itemData.DurabilitySmall;
        this.GameUserId = itemData.GameUserId;
        this.CreateTimeTick = itemData.CreateTimeTick;
        this.posX = itemData.posX;
        this.posY = itemData.posY;
        this.IsDispose = itemData.IsDispose;
        this.GameAreaId = itemData.GameAreaId;
        // 其他属性
    }

    GetProp(propId)
    {
        return this.PropertyData[propId] != null ? this.PropertyData[propId] : 0;
    }

    SetProp(propId,value)
    {
        if(value != 0)
        {
            this.PropertyData[propId] = value;
        }else{
            delete this.PropertyData[propId];
        }
    }
    // 转为更新用的DB数据
    ToUpdateDBData()
    {
        var data = {};
        data.ConfigID = this.Config.Id;
        data.ExcellentEntry = this.ExcellentEntry;
        data.SpecialEntry = [];
        for(const [key,value] of Object.entries(this.SpecialEntry))
        {
            data.SpecialEntry.push([Number(key),Number(value)]);
        }
        data.PropertyData = [];
        for(const [key,value] of Object.entries(this.PropertyData))
        {
            data.PropertyData.push([Number(key),Number(value)]);
        }
        data.DurabilitySmall = this.DurabilitySmall;
        data.IsDispose = this.IsDispose;
        return data;
    }



}

class ItemCreateAttr
{
    constructor(){
        this.Level = 0;
        this.ForgeLevel =0;
        this.Quantity = 1;
        this.OptListId = 0;
        this.OptLevel = 0;
        this.HaveSkill = false;
        this.HaveLucky = false;
        this.SetId = 0;
        this.IsBind = 0;
        this.IsTask = false;
        this.ValidTime = 0;
        this.FluoreAttr = 0;
        this.FluoreSlotCount = 0;
        this.FluoreSlot = [];
        this.OptionExcellent = [];
        this.CustomAttrMethod = [];
    }
}

class MailItem
{
    constructor(){
        this.ItemConfigID = 0;
        this.ItemID = 0;
        this.CreateAttr = new ItemCreateAttr();
    }
}


module.exports = {
    EDBItemValue : EDBItemValue,
    EItemType: EItemType,
    EQualityType: EQualityType,
    EItemInComponent: EItemInComponent,
    Item: Item,
    ItemCreateAttr: ItemCreateAttr,
    MailItem: MailItem,
}