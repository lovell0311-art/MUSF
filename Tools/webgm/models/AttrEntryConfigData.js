const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const AttrEntryConfigDataSchema = new Schema({
    _id:{
        type:Number,
        required: true
    },
    Name:{
        type:String,
        required: true
    },
    Value0:{
        type:Number,
        required: false
    },
    Value1:{
        type:Number,
        required: false
    },
    Value2:{
        type:Number,
        required: false
    },
    Value3:{
        type:Number,
        required: false
    },
    Value4:{
        type:Number,
        required: false
    },
    Value5:{
        type:Number,
        required: false
    },
    Value6:{
        type:Number,
        required: false
    },
    Value7:{
        type:Number,
        required: false
    },
    Value8:{
        type:Number,
        required: false
    },
    Value9:{
        type:Number,
        required: false
    },
    Value10:{
        type:Number,
        required: false
    },
    Value11:{
        type:Number,
        required: false
    },
    Value12:{
        type:Number,
        required: false
    },
    Value13:{
        type:Number,
        required: false
    },
    Value14:{
        type:Number,
        required: false
    },
    Value15:{
        type:Number,
        required: false
    },
});


module.exports = CreateAttrEntryConfigData = (url)=>{
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model("AttrEntryConfigData",AttrEntryConfigDataSchema,"AttrEntryConfigData");
};