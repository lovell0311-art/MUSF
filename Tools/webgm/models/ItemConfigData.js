const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const ItemConfigDataSchema = new Schema({
    _id:{
        type:Number,
        required: true
    },
    Name:{
        type:String,
        required: true
    },
    Type:{
        type:Number,
        required: false
    },
    Slot:{
        type:Number,
        required: false
    },
    Skill:{
        type:Number,
        required: false
    },
    X:{
        type:Number,
        required: false
    },
    Y:{
        type:Number,
        required: false
    },
    StackSize:{
        type:Number,
        required: false
    },
    QualityAttr:{
        type:Number,
        required: false
    },
    Level:{
        type:Number,
        required: false
    },
    AppendAttrId:{
        type:String,
        required: false
    },
    ExtraAttrId:{
        type:String,
        required: false
    },
    TwoHand:{
        type:Number,
        required: false
    },
    SpecialAttrId:{
        type:String,
        required: false
    },
    Durable:{
        type:Number,
        required: false
    },

    UseRole_Spell:{
        type:Number,
        required: false
    },
    UseRole_Swordsman:{
        type:Number,
        required: false
    },
    UseRole_Archer:{
        type:Number,
        required: false
    },
    UseRole_Spellsword:{
        type:Number,
        required: false
    },
    UseRole_Holyteacher:{
        type:Number,
        required: false
    },
    UseRole_SummonWarlock:{
        type:Number,
        required: false
    },
    UseRole_Combat:{
        type:Number,
        required: false
    },
    UseRole_GrowLancer:{
        type:Number,
        required: false
    },
});


module.exports = CreateItemConfigData = (url)=>{
    var connect = mongoose.createConnection(url);
    connect.set("strictQuery", false);
    return connect.model("ItemConfigData",ItemConfigDataSchema,"ItemConfigData");
};