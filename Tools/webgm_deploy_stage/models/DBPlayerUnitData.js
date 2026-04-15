const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const DBPlayerUnitDataSchema = new Schema({
    GameUserId:{
        type:Schema.Types.Long,
        required: true
    },
    Index:{
        type:Number,
        required: true
    },
    X:{
        type:Number,
        required: true
    },
    Y:{
        type:Number,
        required: true
    },
    Angle:{
        type:Number,
        required: true
    },
    Hp:{
        type:Number,
        required: true
    },
    Mp:{
        type:Number,
        required: true
    },
    SD:{
        type:Number,
        required: true
    },
    AG:{
        type:Number,
        required: true
    },
});


module.exports = CreateDBPlayerUnitData = (url)=>{
  let CreateConnect = require('./mongo').CreateConnect;
  var connect = CreateConnect(url);
    return connect.model("DBPlayerUnitData",DBPlayerUnitDataSchema,"DBPlayerUnitData");
};