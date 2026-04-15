const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const DBAccountInfoSchema = new Schema({
    _id:{
        type:Schema.Types.Long,
        required: true
    },
    Phone:{
        type:String,
        required: true
    },
    Password:{
        type:String,
        required: true
    },
    RegisterTime:{
        type:Date,
        required: true
    },
    RegisterIP:{
        type:String,
        required: true
    },
    LastLoginTime:{
        type:Date,
        required: true
    },
    LastActiveTime:{
      type:Date,
      required: false
    },
    LastLoginIP:{
        type:String,
        required: false
    },
    Disabled:{
        type:Number,
        required: true
    },
    ChannelId:{
        type:String,
        required: true
    },
    XYAccountNumber:{
        type:String,
        required: false,
        default: ""
    },
    BanTillTime:{
        type:Schema.Types.Long,
        required: false,
        default: 0
    },
    BanReason:{
        type:String,
        required: false,
        default: ""
    },
    Identity:{
      type:Number,
      required: false,
      default: 0
    },
    IdInspect:{
      type:Number,
      required: false,
      default: 0
    },
    IdCard:{
      type:String,
      required: false,
      default: ""
    },
    Name:{
      type:String,
      required: false,
      default: ""
    },
});


module.exports = CreateDBAccountInfo = (url)=>{
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model("DBAccountInfo",DBAccountInfoSchema,"DBAccountInfo");
};