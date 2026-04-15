const mongoose = require('mongoose');
const db = require('../config/db')
const Schema = mongoose.Schema;

// 操作日志
const DBRCodeDataSchema = new Schema({
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
  CodeStr: {
    type: String,
    required: true
  },
  UseCount: {
    type: Number,
    required: true
  },
  UseIds: {
    type: String,
    required: true
  },
  IsDispose: {
    type: Number,
    required: true
  }
});


const DBRCodeData = {
  CreateDBRCodeData: function (url) {
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model(
      "DBRCodeData",
      DBRCodeDataSchema,
      "DBRCodeData"
    );
  }
};

module.exports = DBRCodeData;
