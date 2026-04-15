const express = require('express');
const router = express.Router();
const db = require('../../../config/db');
const passport = require('passport');
const axios = require('axios');
const permiss = require('../../../common/permiss.js');

const { WriteOperateLog } = require('../../../models/OperateLog.js')

const mongoose = require('mongoose');
require('mongoose-long')(mongoose);

const EPermiss = permiss.EPermiss;

// @route  POST api/game/cdkey/get_type
// @desc   return
// @access Private
router.post(
    '/get_type',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) =>{
        try{

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.cdkey,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let skip = 0;
            let limit = 1;
            try {
                if (req.body.zoneId == null) throw null;
                if (req.body.skip == null) throw null;
                if (req.body.limit == null) throw null;

                zoneId = req.body.zoneId;
                skip = req.body.skip;
                limit = req.body.limit;
                if (limit > 50) limit = 50;
            }
            catch (err) {
                return res.status(400).json({ success: false, msg: "参数错误" });
            }

            const CreateDBRCodeTypeData = require('../../../models/DBRCodeTypeData').CreateDBRCodeTypeData;

            const DBRCodeTypeData = CreateDBRCodeTypeData(db.accountMongoURL);
            let query = {};

            Promise.all([
                DBRCodeTypeData.count(query),
                DBRCodeTypeData.find(query).skip(skip).limit(limit)
            ]).then((args) => {
                const newData = [];
                for (i = 0; i < args[1].length; i++) {
                    const data = args[1][i];
                    newData.push({
                        Id: String(data._id),
                        CodeType: data.CodeType,
                        RewardType: data.RewardType,
                        RewardStr: data.RewardStr,
                        Remark: data.Remark,
                    });
                }
                return res.status(200).json({ success: true, total: args[0], data: newData });
            }).catch(err => {
                console.log(err);
            });
        }
        catch (err)
        {
            return res.status(200).json({ success: false, msg: String(err) });
        }
    }
);

// @route  POST api/game/cdkey/add_type
// @desc   return
// @access Private
router.post(
    '/add_type',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) =>{
        try{

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.cdkey,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let rewardList = [];
            try {
                if (req.body.zoneId == null) throw null;
                if (req.body.rewardList == null) throw null;

                zoneId = req.body.zoneId;
                rewardList = req.body.rewardList;
            }
            catch (err) {
                return res.status(400).json({ success: false, msg: "参数错误" });
            }
            console.log(JSON.stringify(req.body));
            console.log(db.gmUrl + "/api/cdkey/AddType");
            var result = await axios.post(db.gmUrl + "/api/cdkey/AddType", {
                ZoneId: zoneId,
                RewardList: rewardList
            });
            if (result.data.status == false) {
                console.log(result.data.msg);
                return res.status(200).json({ success: false, msg: "服务器错误" });
            }

            WriteOperateLog(req, `添加兑换码类型 ${JSON.stringify(req.body)}`);
            return res.status(200).json({ success: true, data: "" });
        }
        catch (err)
        {
            return res.status(200).json({ success: false, msg: String(err) });
        }
    }
);


// @route  POST api/game/cdkey/set_remark
// @desc   return
// @access Private
router.post(
  '/set_remark',
  passport.authenticate('jwt', { session: false }),
  async (req, res, next) =>{
      try{
          if (!permiss.PermissOkAny(req.user.identity,
              [
                  EPermiss.cdkey,
              ])) {
              return res.status(403).json({ success: false, msg: "权限不足" });
          }

          let id = "";
          let remark = "";
          try {
              if (req.body.id == null) throw null;
              if (req.body.remark == null) throw null;

              id = req.body.id;
              remark = req.body.remark;
          }
          catch (err) {
              return res.status(400).json({ success: false, msg: "参数错误" });
          }
          const CreateDBRCodeTypeData = require('../../../models/DBRCodeTypeData').CreateDBRCodeTypeData;
          const DBRCodeTypeData = CreateDBRCodeTypeData(db.accountMongoURL);
          let data = {
            Remark:remark
          };

          let result = await DBRCodeTypeData.updateOne({_id : mongoose.Types.Long.fromString(id)},{$set:data});
          if(result.acknowledged)
          {
              WriteOperateLog(req,`修改兑换码备注  ${JSON.stringify(req.body)}`);
              return res.status(200).json({success:true});
          }else{
              return res.status(200).json({success:false,msg:"修改失败"});
          }
      }
      catch (err)
      {
          return res.status(200).json({ success: false, msg: String(err) });
      }
  }
);


// @route  POST api/game/cdkey/get_code
// @desc   return
// @access Private
router.post(
    '/get_code',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) =>{
        try{

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.cdkey,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let skip = 0;
            let limit = 1;
            let rewardType = 0;
            let viewUsed = 0;
            try {
                if (req.body.zoneId == null) throw null;
                if (req.body.skip == null) throw null;
                if (req.body.limit == null) throw null;
                if (req.body.rewardType == null) throw null;
                if (req.body.viewUsed == null) throw null;

                zoneId = req.body.zoneId;
                skip = req.body.skip;
                limit = req.body.limit;
                rewardType = req.body.rewardType;
                viewUsed = req.body.viewUsed;
                if (limit > 50) limit = 50;
            }
            catch (err) {
                return res.status(400).json({ success: false, msg: "参数错误" });
            }

            const CreateDBRCodeData = require('../../../models/DBRCodeData').CreateDBRCodeData;

            const DBRCodeData = CreateDBRCodeData(db.accountMongoURL);
            let query = {};
            if(viewUsed == 0)
            {
                query = {RewardType:rewardType};
            }else
            {
                query = {RewardType:rewardType};
            }



            Promise.all([
                DBRCodeData.count(query),
                DBRCodeData.find(query).skip(skip).limit(limit)
            ]).then((args) => {
                const newData = [];
                for (i = 0; i < args[1].length; i++) {
                    const data = args[1][i];
                    newData.push({
                        Id: String(data._id),
                        CodeStr: data.CodeStr,
                        UseCount: data.UseCount,
                        UseIds: data.UseIds,
                    });
                }
                return res.status(200).json({ success: true, total: args[0], data: newData });
            }).catch(err => {
                console.log(err);
            });
        }
        catch (err)
        {
            return res.status(200).json({ success: false, msg: String(err) });
        }
    }
);


// @route  POST api/game/cdkey/add_code
// @desc   return
// @access Private
router.post(
    '/add_code',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) =>{
        try{

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.cdkey,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let rewardType = 0;
            let count = 0;
            try {
                if (req.body.zoneId == null) throw null;
                if (req.body.rewardType == null) throw null;
                if (req.body.count == null) throw null;

                zoneId = req.body.zoneId;
                rewardType = req.body.rewardType;
                count = req.body.count;
            }
            catch (err) {
                return res.status(400).json({ success: false, msg: "参数错误" });
            }

            let result = await axios.post(db.gmUrl + "/api/cdkey/AddCode", {
                ZoneId: zoneId,
                RewardType: rewardType,
                Count: count,
            });
            if (result.data.status == false) {
                console.log(result.data.msg);
                return res.status(200).json({ success: false, msg: "服务器错误" });
            }
            let data = {
                firstId: result.data.data.FirstId,
                lastId: result.data.data.LastId,
            };
            WriteOperateLog(req, `添加兑换码 ${JSON.stringify(req.body)}`);
            return res.status(200).json({ success: true, data: data });
        }
        catch (err)
        {
            return res.status(200).json({ success: false, msg: String(err) });
        }
    }
);

// @route  POST api/game/cdkey/download_cdkey
// @desc   return
// @access Private
router.post(
    '/download_cdkey',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) =>{
        try{

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.cdkey,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let firstId = "";
            let lastId = "";
            try {
                if (req.body.zoneId == null) throw null;
                if (req.body.firstId == null) throw null;
                if (req.body.lastId == null) throw null;

                zoneId = req.body.zoneId;
                firstId = req.body.firstId;
                lastId = req.body.lastId;
            }
            catch (err) {
                return res.status(400).json({ success: false, msg: "参数错误" });
            }

            const CreateDBRCodeData = require('../../../models/DBRCodeData').CreateDBRCodeData;

            const DBRCodeData = CreateDBRCodeData(db.accountMongoURL);
            let query = {_id:{$gte:firstId,$lte:lastId}};

            const dataList = await DBRCodeData.find(query).limit(10000);
            let fileContent = "";
            dataList.forEach((item,index)=>{
                fileContent += item.CodeStr + "\r\n";
            });

            const fs = require('fs');
            const os = require('os');
            const fileName = `cdkey_${firstId}_${lastId}.txt`;
            await fs.writeFileSync(`${os.tmpdir()}//${fileName}`, fileContent);

            return res.status(200).download(`${os.tmpdir()}/${fileName}`,fileName);
        }
        catch (err)
        {
            return res.status(200).json({ success: false, msg: String(err) });
        }
    }
);


module.exports = router;
