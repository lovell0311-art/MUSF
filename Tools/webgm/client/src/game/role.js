const className = [
    [],
    ["魔法师",  "魔导师",   "魔导师",   "神导师",],
    ["剑士",    "骑士",     "骑士",     "神骑士",],
    ["弓箭手",  "圣射手",   "圣射手",   "神射手",],
    ["魔剑士",  "魔剑士",   "魔剑士",   "剑圣",],
    ["圣导师",  "圣导师",   "圣导师",   "祭祀",],
    ["召唤术师","召唤导师", "召唤导师", "召唤巫师",],
    ["格斗家",  "格斗家",   "格斗家",   "格斗大师",],
    ["梦幻骑士","梦幻骑士", "梦幻骑士", "魅影骑士",],
]

export function GetClassName(playerTypeId,occupationLevel){
    if(className[playerTypeId][occupationLevel])
    {
     return className[playerTypeId][occupationLevel];
    }else{
     return `${playerTypeId}(${occupationLevel})`;
    }
 };


class Role{
    constructor(){
        this.NickName = "";
    }

    SetNewData(newData){
        if(newData.NickName)this.NickName = newData.NickName;
    }

    
}