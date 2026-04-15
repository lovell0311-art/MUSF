const mongoose = require('mongoose');
const db = require('../config/db')
const Schema = mongoose.Schema;

// 操作日志
const DBOnlineCountSchema = new Schema({
  Count: {
    type: String,
    required: true
  },
  CreateTime: {
    type: Schema.Types.Long,
    required: true
  }
});


const DBOnlineCount = {
  CreateDBOnlineCount: function (url) {
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model(
      "DBOnlineCount",
      DBOnlineCountSchema,
      "DBOnlineCount"
    );
  }
};

module.exports = DBOnlineCount;
