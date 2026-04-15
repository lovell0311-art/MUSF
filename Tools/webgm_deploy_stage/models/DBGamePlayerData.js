const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const DBGamePlayerDataSchema = new Schema({
    _id:{
        type:Schema.Types.Long,
        required: true
    },
    UserId:{
        type:Schema.Types.Long,
        required: true
    },
    GameAreaId:{
        type:Number,
        required: true
    },
    PlayerTypeId:{
        type:Number,
        required: true
    },
    NickName:{
        type:String,
        required: true
    },
    Level:{
        type:Number,
        required: true
    },
    Exp:{
        type:Number,
        required: true
    },
    GoldCoin:{
        type:Number,
        required: true
    },
    MiracleCoin:{
      type:Number,
      required: true
    },
    OccupationLevel:{
        type:Number,
        required: true
    },
    Strength:{
        type:Number,
        required: true
    },
    Willpower:{
        type:Number,
        required: true
    },
    Agility:{
        type:Number,
        required: true
    },
    BoneGas:{
        type:Number,
        required: true
    },
    Command:{
        type:Number,
        required: true
    },
    FreePoint:{
        type:Number,
        required: true
    },
    CreateTimeTick:{
        type:Schema.Types.Long,
        required: true
    },
    IsDisposePlayer:{
        type:Number,
        required: true
    },
});


module.exports = CreateDBGamePlayerData = (url)=>{
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model("DBGamePlayerData",DBGamePlayerDataSchema,"DBGamePlayerData");
};