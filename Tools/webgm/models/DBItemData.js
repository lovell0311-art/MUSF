const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const DBItemDataSchema = new Schema({
    _id:{
        type:Schema.Types.Long,
        required: true
    },
    UserId:{
        type:Schema.Types.Long,
        required: true
    },
    GameUserId:{
        type:Schema.Types.Long,
        required: true
    },
    PropertyData:{
        type:Array,
        required: true
    },
    ExcellentEntry:{
        type:Array,
        required: false
    },
    SpecialEntry:{
        type:Array,
        required: false
    },
    ConfigID:{
        type:Number,
        required: true
    },
    InComponent:{
        type:Number,
        required: true
    },
    posId:{
        type:Number,
        required: true
    },
    posX:{
        type:Number,
        required: true
    },
    posY:{
        type:Number,
        required: true
    },
    CreateTimeTick:{
        type:Schema.Types.Long,
        required: true
    },
    IsDispose:{
        type:Schema.Types.Long,
        required: true
    },
    GameAreaId:{
        type:Number,
        required: true
    },
    DurabilitySmall:{
        type:Number,
        required: false
    },
});


module.exports = CreateDBItemData = (url)=>{
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model("DBItemData",DBItemDataSchema,"DBItemData");
};