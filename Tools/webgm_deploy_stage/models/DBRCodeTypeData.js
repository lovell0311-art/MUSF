const mongoose = require('mongoose');
const db = require('../config/db')
const Schema = mongoose.Schema;

// 操作日志
const DBRCodeTypeDataSchema = new Schema({
  _id:{
    type:Schema.Types.Long,
    required: true
  },
  CodeType: {
    type: Number,
    required: true
  },
  RewardType: {
    type: Number,
    required: true
  },
  RewardStr: {
    type: String,
    required: true
  },
  Remark:{
    type: String,
    required: false
  },
  IsDispose: {
    type: Number,
    required: true
  }
});


const DBRCodeTypeData = {
  CreateDBRCodeTypeData: function (url) {
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model(
      "DBRCodeTypeData",
      DBRCodeTypeDataSchema,
      "DBRCodeTypeData"
    );
  }
};

module.exports = DBRCodeTypeData;
