const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const DBAccountZoneDataSchema = new Schema({
    _id:{
        type:Schema.Types.Long,
        required: true
    },
    YuanbaoCoin:{
        type:Number,
        required: true
    }
});


module.exports = CreateDBAccountZoneData = (url)=>{
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model("DBAccountZoneData",DBAccountZoneDataSchema,"DBAccountZoneData");
};