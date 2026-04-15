const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const DBEquipmentItemSchema = new Schema({
    _id:{
        type:Schema.Types.Long,
        required: true
    },
    GameUserId:{
        type:Schema.Types.Long,
        required: true
    },
    ItemList:{
        type:String,
        required: true
    }
});


module.exports = CreateDBEquipmentItem = (url)=>{
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model("DBEquipmentItem",DBEquipmentItemSchema,"DBEquipmentItem");
};