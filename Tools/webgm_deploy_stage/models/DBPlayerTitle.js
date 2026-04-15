const mongoose = require('mongoose');
const Schema = mongoose.Schema;
require('mongoose-long')(mongoose);

const DBPlayerTitleSchema = new Schema({
    _id:{
        type:Schema.Types.Long,
        required: true
    },
    UserId:{
        type:Schema.Types.Long,
        required: true
    },
    Type:{
        type:Number,
        required: true
    },
    UseID:{
        type:Number,
        required: true
    },
    TitleID:{
        type:Number,
        required: true
    },
    BingTime:{
        type:Schema.Types.Long,
        required: true
    },
    EndTime:{
        type:Schema.Types.Long,
        required: true
    },
    IsDisabled:{
        type:Number,
        required: true
    },
});


module.exports = CreateDBPlayerTitle = (url)=>{
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model("DBPlayerTitle",DBPlayerTitleSchema,"DBPlayerTitle");
};