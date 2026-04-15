const mongoose = require('mongoose');
const db = require('../config/db')
const Schema = mongoose.Schema;

// 操作日志
const OperateLogSchema = new Schema({
  user: {
    type: String,
    required: true
  },
  ip: {
    type: String,
    required: true
  },
  token: {
    type: String,
    required: true
  },
  log: {
    type: String,
    required: true
  },
  date: {
    type: Date,
    default: Date.now
  }
});


const operateLog = {
  CreateOperateLog: function (url) {
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model(
      db.CollectionName("OperateLog"),
      OperateLogSchema,
      db.CollectionName("OperateLog")
    );
  },
  WriteOperateLog: async function (req, log) {
    const OperateLog = operateLog.CreateOperateLog(db.mongoURL);
    const newOperateLog = new OperateLog({
      user: req.user.name,
      ip: req.connection.remoteAddress,
      token: req.headers.authorization,
      log: log
    });
    await newOperateLog.save();
  },
  WriteOperateLog2: async function (req, user, token, log) {
    const OperateLog = operateLog.CreateOperateLog(db.mongoURL);
    const newOperateLog = new OperateLog({
      user: user,
      ip: req.connection.remoteAddress,
      token: token,
      log: log
    });
    await newOperateLog.save();
  }
};

module.exports = operateLog;
