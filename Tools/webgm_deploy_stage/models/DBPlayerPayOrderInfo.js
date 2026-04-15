const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const DBPlayerPayOrderInfoSchema = new Schema({
    _id:{
        type:Schema.Types.Long,
        required: true
    },
    Ordef_id:{
        type:String,
        required: true
    },
    App_Ordef_id:{
        type:Schema.Types.Long,
        required: true
    },
    Gid:{
        type:Schema.Types.Long,
        required: true
    },
    Uid:{
        type:Schema.Types.Long,
        required: true
    },
    GUid:{
        type:Schema.Types.Long,
        required: true
    },
    Rid:{
        type:Schema.Types.Long,
        required: true
    },
    Product_id:{
        type:Number,
        required: true
    },
    Money:{
        type:Number,
        required: true
    },
    Time:{
        type:Schema.Types.Long,
        required: true
    },
    RName:{
        type:String,
        required: true
    },
    SuccessTime:{
        type:Schema.Types.Long,
        required: true
    },
    Effective:{
        type:Boolean,
        required: true
    },
    Success:{
        type:Boolean,
        required: true
    },
    ChannelId:{
        type:String,
        required: false
    },
});


module.exports = CreateDBPlayerPayOrderInfo = (url)=>{
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model("DBPlayerPayOrderInfo",DBPlayerPayOrderInfoSchema,"DBPlayerPayOrderInfo");
};