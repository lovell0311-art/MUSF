const express = require('express');
const router = express.Router();
const db = require('../../../config/db');
const passport = require('passport');
const axios = require('axios');
const permiss = require('../../../common/permiss.js');

const mongoose = require('mongoose');
require('mongoose-long')(mongoose);

const EPermiss = permiss.EPermiss;


// @route  POST api/game/proxy/account_info
// @desc   return
// @access Private
router.post(
  '/account_info',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
    try {
      let channelId = "";
      let skip = 0;
      let limit = 1;
      try {
        if (req.body.skip == null) throw null;
        if (req.body.limit == null) throw null;

        channelId = req.user.name;
        skip = req.body.skip;
        limit = req.body.limit;

        if (limit > 50) limit = 50;
      }
      catch (err) {
        return res.status(400).json({ success: false, msg: "参数错误" });
      }


      let query = { ChannelId: channelId };

      const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
      const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

      let args = await Promise.all([
        DBAccountInfo.count(query),
        DBAccountInfo.find(query).skip(skip).limit(limit)
      ]);

      const newData = [];
      for (i = 0; i < args[1].length; i++) {
        const data = args[1][i];
        let singData = {
          UserId: String(data._id).replace(/^(\w{3}).*(\w{3})$/, "$1****$2"),
          LastLoginTime: data.LastLoginTime.getTime(),
          RegisterTime: data.RegisterTime.getTime(),
        };
        if (data.XYAccountNumber != null && data.XYAccountNumber !== '') singData.XYAccountNumber = data.XYAccountNumber.replace(/^(\w{3}).*(\w{3})$/, "$1****$2");
        if (data.Phone != null && data.Phone !== '') singData.Phone = data.Phone.replace(/^(\w{3}).*(\w{3})$/, "$1****$2");

        newData.push(singData);
      }
      return res.status(200).json({ success: true, total: args[0], data: newData });
    }
    catch (err) {
      return res.status(200).json({ success: false, msg: String(err) });
    }
  }
);

// @route  POST api/game/proxy/account_reg_count
// @desc   return
// @access Private
router.post(
  '/account_reg_count',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
    try {
      let channelId = req.user.name;
      let beginTime = 0;
      let endTime = 1;
      try {
        if (req.body.beginTime == null) throw null;
        if (req.body.endTime == null) throw null;

        beginTime = req.body.beginTime;
        endTime = req.body.endTime;

      }
      catch (err) {
        return res.status(400).json({ success: false, msg: "参数错误" });
      }
      let beginDate = new Date();
      let endDate = new Date();
      beginDate.setTime(beginTime);
      endDate.setTime(endTime);

      let query = {
        ChannelId: channelId,
        RegisterTime: { $gte: beginDate, $lte: endDate }
      };

      const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
      const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

      let regCount = await DBAccountInfo.count(query);

      return res.status(200).json({ success: true, count: regCount });
    }
    catch (err) {
      console.log(err);
      return res.status(200).json({ success: false, msg: String(err) });
    }
  }
);


// @route  POST api/game/proxy/all/account_reg_count
// @desc   return
// @access Private
router.post(
  '/all/account_reg_count',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
      try{
          if(!permiss.PermissOkAny(req.user.identity,
            [
              EPermiss.dashboard_admin,
            ]))
          {
              return res.status(403).json({success:false,msg:"权限不足"});
          }

        let beginTime = 0;
        let endTime = 1;
        try{
            if(req.body.beginTime == null) throw null;
            if(req.body.endTime == null) throw null;

            beginTime = req.body.beginTime;
            endTime = req.body.endTime;

        }
        catch(err)
        {
            return res.status(400).json({success:false,msg:"参数错误"});
        }
        let beginDate = new Date();
        let endDate = new Date();
        beginDate.setTime(beginTime);
        endDate.setTime(endTime);

        let query = {
          RegisterTime:{$gte:beginDate,$lte:endDate}
        };

        const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
        const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

        let regCount = await DBAccountInfo.count(query);

        return res.status(200).json({success:true,count:regCount});
    }
    catch(err)
    {
        return res.status(200).json({success:false,msg:String(err)});
    }
  }
);

// @route  POST api/game/proxy/all/total_reg_count
// @desc   return
// @access Private
router.post(
  '/all/total_reg_count',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
    try {
      if (!permiss.PermissOkAny(req.user.identity,
        [
          EPermiss.dashboard_admin,
        ])) {
        return res.status(403).json({ success: false, msg: "权限不足" });
      }

      let query = {
      };

      const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
      const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

      let regCount = await DBAccountInfo.count(query);

      return res.status(200).json({ success: true, count: regCount });
    }
    catch (err) {
      return res.status(200).json({ success: false, msg: String(err) });
    }
  }
);


// @route  POST api/game/proxy/account_login_count
// @desc   return
// @access Private
router.post(
  '/account_login_count',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
    try {
      let channelId = req.user.name;
      let beginTime = 0;
      let endTime = 1;
      try {
        if (req.body.beginTime == null) throw null;
        if (req.body.endTime == null) throw null;

        beginTime = req.body.beginTime;
        endTime = req.body.endTime;

      }
      catch (err) {
        return res.status(400).json({ success: false, msg: "参数错误" });
      }
      let beginDate = new Date();
      let endDate = new Date();
      beginDate.setTime(beginTime);
      endDate.setTime(endTime);

      let query = {
        ChannelId: channelId,
        LastLoginTime: { $gte: beginDate, $lte: endDate }
      };

      const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
      const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

      let regCount = await DBAccountInfo.count(query);

      return res.status(200).json({ success: true, count: regCount });
    }
    catch (err) {
      return res.status(200).json({ success: false, msg: String(err) });
    }
  }
);

// @route  POST api/game/proxy/all/account_login_count
// @desc   return
// @access Private
router.post(
  '/all/account_login_count',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
    try {
      if (!permiss.PermissOkAny(req.user.identity,
        [
          EPermiss.dashboard_admin,
        ])) {
        return res.status(403).json({ success: false, msg: "权限不足" });
      }

      let beginTime = 0;
      let endTime = 1;
      try {
        if (req.body.beginTime == null) throw null;
        if (req.body.endTime == null) throw null;

        beginTime = req.body.beginTime;
        endTime = req.body.endTime;

      }
      catch (err) {
        return res.status(400).json({ success: false, msg: "参数错误" });
      }
      let beginDate = new Date();
      let endDate = new Date();
      beginDate.setTime(beginTime);
      endDate.setTime(endTime);

      let query = {
        LastLoginTime: { $gte: beginDate, $lte: endDate }
      };

      const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
      const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

      let regCount = await DBAccountInfo.count(query);

      return res.status(200).json({ success: true, count: regCount });
    }
    catch (err) {
      return res.status(200).json({ success: false, msg: String(err) });
    }
  }
);


// @route  POST api/game/proxy/total_rec_amount
// @desc   时间段的充值金额
// @access Private
router.post(
  '/total_rec_amount',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
    try {
      let channelId = req.user.name;
      let beginTime = 0;
      let endTime = 1;
      try {
        if (req.body.beginTime == null) throw null;
        if (req.body.endTime == null) throw null;

        beginTime = req.body.beginTime;
        endTime = req.body.endTime;

      }
      catch (err) {
        return res.status(400).json({ success: false, msg: "参数错误" });
      }
      beginTime = Math.floor(beginTime / 1000);
      endTime = Math.floor(endTime / 1000);

      const CreateDBPlayerPayOrderInfo = require('../../../models/DBPlayerPayOrderInfo');

      let total = 0;

      for (const item of db.gameMongo) {
        if (item.Valid == false) continue;
        const DBPlayerPayOrderInfo = CreateDBPlayerPayOrderInfo(item.URL);
        const result = await DBPlayerPayOrderInfo.aggregate([
          {
            $match: {
              ChannelId: channelId,
              SuccessTime: {
                $gt: 0
              },
              Time: {
                $gte: beginTime,
                $lte: endTime
              }
            }
          },
          {
            $group: {
              _id: null,
              totalMoney: { $sum: "$Money" }
            }
          }
        ]);
        if (result.length > 0) {
          total += result[0].totalMoney;
        }
      }

      return res.status(200).json({ success: true, total: total });
    }
    catch (err) {
      return res.status(200).json({ success: false, msg: String(err) });
    }
  }
);

// @route  POST api/game/proxy/find/total_rec_amount
// @desc   时间段的充值金额
// @access Private
router.post(
  '/find/total_rec_amount',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
    try {
      if (!permiss.PermissOkAny(req.user.identity,
        [
          EPermiss.dashboard_admin,
        ])) {
        return res.status(403).json({ success: false, msg: "权限不足" });
      }

      let channelId = "";
      let beginTime = 0;
      let endTime = 1;
      try {
        if (req.body.channelId == null) throw null;
        if (req.body.beginTime == null) throw null;
        if (req.body.endTime == null) throw null;

        beginTime = req.body.beginTime;
        endTime = req.body.endTime;
        channelId = req.body.channelId;
      }
      catch (err) {
        return res.status(400).json({ success: false, msg: "参数错误" });
      }
      beginTime = Math.floor(beginTime / 1000);
      endTime = Math.floor(endTime / 1000);

      const CreateDBPlayerPayOrderInfo = require('../../../models/DBPlayerPayOrderInfo');

      let total = 0;

      for (const item of db.gameMongo) {
        if (item.Valid == false) continue;
        const DBPlayerPayOrderInfo = CreateDBPlayerPayOrderInfo(item.URL);
        const result = await DBPlayerPayOrderInfo.aggregate([
          {
            $match: {
              ChannelId: channelId,
              SuccessTime: {
                $gt: 0
              },
              Time: {
                $gte: beginTime,
                $lte: endTime
              }
            }
          },
          {
            $group: {
              _id: null,
              totalMoney: { $sum: "$Money" }
            }
          }
        ]);
        if (result.length > 0) {
          total += result[0].totalMoney;
        }
      }

      return res.status(200).json({ success: true, total: total });
    }
    catch (err) {
      return res.status(200).json({ success: false, msg: String(err) });
    }
  }
);


// @route  POST api/game/proxy/find/online_count
// @desc   时间段的在线人数
// @access Private
router.post(
  '/find/online_count',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
    try {
      if (!permiss.PermissOkAny(req.user.identity,
        [
          EPermiss.dashboard_admin,
        ])) {
        return res.status(403).json({ success: false, msg: "权限不足" });
      }

      let beginTime = 0;
      let endTime = 1;
      try {
        if (req.body.beginTime == null) throw null;
        if (req.body.endTime == null) throw null;

        beginTime = req.body.beginTime;
        endTime = req.body.endTime;
      }
      catch (err) {
        return res.status(400).json({ success: false, msg: "参数错误" });
      }
      beginTime = Math.floor(beginTime);
      endTime = Math.floor(endTime);

      const CreateDBOnlineCount = require('../../../models/DBOnlineCount').CreateDBOnlineCount;
      const DBOnlineCountInfo = CreateDBOnlineCount(db.gameLogMongo[0].URL);

      const result = await DBOnlineCountInfo.find({CreateTime: {
        $gte: beginTime,
        $lte: endTime
      }});
      let data = [];
      let pre = 1000 * 60;
      if((endTime - beginTime) > (1000 * 60 * 60 * 24 * 2))
      {
        pre *= 60;
      }
      let lastData = {
        Count: 0,
        CreateTime: 0,
        PreCreateTime: 0,
      };
      for (const item of result)
      {
        let preCreateTime = Math.floor(Number(item.CreateTime) / pre) * pre;
        if(lastData.PreCreateTime == preCreateTime)
        {
          if(lastData.Count < item.Count)
          {
            lastData.Count = item.Count;
            lastData.CreateTime = item.CreateTime;
          }
        }else
        {
          if(lastData.PreCreateTime != 0){
            data.push({
              Count : lastData.Count,
              CreateTime : Number(lastData.CreateTime)
            });
          }
          lastData.Count = item.Count;
          lastData.CreateTime = item.CreateTime;
          lastData.PreCreateTime = preCreateTime;
        }

      }
      return res.status(200).json({ success: true, data: data });
    }
    catch (err) {
      return res.status(200).json({ success: false, msg: String(err) });
    }
  }
);


// @route  POST api/game/proxy/retain
// @desc   留存
// @access Private
router.post(
  '/retain',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) => {
    try {
      if (!permiss.PermissOkAny(req.user.identity,
        [
          EPermiss.dashboard_admin,
        ])) {
        return res.status(403).json({ success: false, msg: "权限不足" });
      }

      let channelId = "";
      let regBeginTime = new Date();
      let regEndTime = new Date();
      let loginBeginTime = new Date();
      let loginEndTime = new Date();
      try {
        if (req.body.channelId == null) throw null;
        if (req.body.regBeginTime == null) throw null;
        if (req.body.regEndTime == null) throw null;
        if (req.body.loginBeginTime == null) throw null;
        if (req.body.loginEndTime == null) throw null;

        regBeginTime.setTime(req.body.regBeginTime);
        regEndTime.setTime(req.body.regEndTime);
        loginBeginTime.setTime(req.body.loginBeginTime);
        loginEndTime.setTime(req.body.loginEndTime);
        channelId = req.body.channelId;
      }
      catch (err) {
        return res.status(400).json({ success: false, msg: "参数错误" });
      }

      const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
      const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

      let query = {
        RegisterTime: { $gte: regBeginTime, $lte: regEndTime },
        LastActiveTime: { $gte: loginBeginTime }
      };

      let loginCount = await DBAccountInfo.count(query);
      query = {
        RegisterTime: { $gte: regBeginTime, $lte: regEndTime }
      };
      let regCount = await DBAccountInfo.count(query);
      return res.status(200).json({ success: true, regCount: regCount, loginCount: loginCount });
    }
    catch (err) {
      return res.status(200).json({ success: false, msg: String(err) });
    }
  }
);

module.exports = router;