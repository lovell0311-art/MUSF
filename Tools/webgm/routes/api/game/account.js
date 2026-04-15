const express = require('express');
const router = express.Router();
const db = require('../../../config/db');
const passport = require('passport');
const axios = require('axios');
const permiss = require('../../../common/permiss.js');
const { WriteOperateLog }= require('../../../models/OperateLog.js')

const mongoose = require('mongoose');
require('mongoose-long')(mongoose);

const EPermiss = permiss.EPermiss;

function escapeRegex(value) {
    return String(value).replace(/[.*+?^${}()|[\]\\]/g, '\\$&');
}

function parseLongIfNumeric(value) {
    const text = String(value ?? '').trim();
    if (!/^-?\d+$/.test(text)) {
        return null;
    }
    return mongoose.Types.Long.fromString(text);
}

// @route  POST api/game/account/info
// @desc   return
// @access Private
router.post(
    '/info',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try{

            if(!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.account_info_view,
                ]))
            {
                return res.status(403).json({success:false,msg:"权限不足"});
            }
            let userId = "";
            try{
                if(req.body.userId == null) throw null;

                userId = req.body.userId;
            }
            catch(err)
            {
                return res.status(400).json({success:false,msg:"参数错误"});
            }
            const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
            const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

            let result = await DBAccountInfo.findOne({_id:mongoose.Types.Long.fromString(userId)});

            if(result)
            {
                const data = result;
                newData = {
                    UserId:String(data._id),
                    Phone:data.Phone,
                    Password:data.Password,
                    RegisterTime:data.RegisterTime.getTime(),
                    RegisterIP:data.RegisterIP,
                    LastLoginTime:data.LastLoginTime.getTime(),
                    LastLoginIP:data.LastLoginIP,
                    ChannelId:data.ChannelId,
                    XYAccountNumber:data.XYAccountNumber,
                    BanTillTime:String(data.BanTillTime),
                    BanReason:data.BanReason,
                    Identity:data.Identity,
                    IdCard:data.IdCard,
                    Name:data.Name,
                };

                return res.status(200).json({success:true,data:newData});
            }
            return res.status(200).json({success:false,msg:'账号不存在'});
        }
        catch(err)
        {
            next(err);
        }
    }
  );



// @route  POST api/game/account/ban
// @desc   return success
// @access Private
router.post(
    '/ban',
    passport.authenticate('jwt', { session: false }),
    async (req, res,next) => {
        try{
            if(!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.ban_account,
                ]))
            {
                return res.status(403).json({success:false,msg:"权限不足"});
            }

            let userId = "";
            let banTillTime = "";
            let banReason = "";
            try{
                if(req.body.userId == null) throw null;
                if(req.body.banTillTime == null) throw null;
                if(req.body.banReason == null) throw null;

                userId = req.body.userId;
                banTillTime = req.body.banTillTime;
                banReason = req.body.banReason;
            }
            catch(err)
            {
                return res.status(400).json({success:false,msg:"参数错误"});
            }

            var result = await axios.post(db.gmUrl + "/api/account/Ban",{
                UserId:userId,
                BanTillTime:banTillTime,
                BanReason:banReason,
            });
            console.log(JSON.stringify(result.data));
            if(result.data.status == false)
            {
                console.log(result.data.msg);
                return res.status(200).json({success:false,msg:"服务器错误"});
            }
            WriteOperateLog(req,`封禁账号 ${JSON.stringify(req.body)}`);
            return res.status(200).json({success:true,data:""});
        }
        catch(err)
        {
            console.log(err);
            //next(err);
            return res.status(200).json({success:false,msg:"GM服务未开启"});
        }
    }
)

// @route  POST api/game/account/kick
// @desc   return success
// @access Private
router.post(
    '/kick',
    passport.authenticate('jwt', { session: false }),
    async (req, res,next) => {
        try{
            if(!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.offlin_account,
                ]))
            {
                return res.status(403).json({success:false,msg:"权限不足"});
            }

            let userId = "";
            let reason = "";
            try{
                if(req.body.userId == null) throw null;
                if(req.body.reason == null) throw null;

                userId = req.body.userId;
                reason = req.body.reason;
            }
            catch(err)
            {
                return res.status(400).json({success:false,msg:"参数错误"});
            }

            var result = await axios.post(db.gmUrl + "/api/account/Kick",{
                UserId:userId,
                Reason:reason,
            });
            console.log(JSON.stringify(result.data));
            if(result.data.status == false)
            {
                console.log(result.data.msg);
                return res.status(200).json({success:false,msg:"服务器错误"});
            }

            WriteOperateLog(req,`踢玩家下线 ${JSON.stringify(req.body)}`);
            return res.status(200).json({success:true,data:""});
        }
        catch(err)
        {
            console.log(err);
            //next(err);
            return res.status(200).json({success:false,msg:"GM服务未开启"});
        }
    }
)


// @route  POST api/game/account/xyuid/modify
// @desc   return success
// @access Private
router.post(
    '/xyuid/modify',
    passport.authenticate('jwt', { session: false }),
    async (req, res,next) => {
        try{
            if(!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.modify_xyuid,
                ]))
            {
                return res.status(403).json({success:false,msg:"权限不足"});
            }

            let userId = "";
            let xyuid = "";
            try{
                if(req.body.userId == null) throw null;
                if(req.body.xyuid == null) throw null;

                userId = req.body.userId;
                xyuid = req.body.xyuid;
            }
            catch(err)
            {
                return res.status(400).json({success:false,msg:"参数错误"});
            }
            const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
            const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

            if(xyuid != "")
            {
                let result = await DBAccountInfo.findOne({XYAccountNumber:xyuid});

                if(result)
                {
                    // XYUID 已经绑定账号
                    return res.status(200).json({success:false,msg:"XYUID 已经被绑定，请解绑后设置"});
                }
            }
            let updateRes = await DBAccountInfo.updateOne({_id:mongoose.Types.Long.fromString(userId)},{$set:{XYAccountNumber:xyuid}})
            if(updateRes.acknowledged)
            {
                // 修改成功
                WriteOperateLog(req,`XYUID修改 ${JSON.stringify(req.body)}`);
                return res.status(200).json({success:true,msg:"绑定成功"});
            }else{
                return res.status(200).json({success:false,msg:"绑定失败"});
            }
        }
        catch(err)
        {
            return res.status(200).json({success:false,msg:String(err)});
        }
    }
)

// @route  POST api/game/account/phone/modify
// @desc   return success
// @access Private
router.post(
    '/phone/modify',
    passport.authenticate('jwt', { session: false }),
    async (req, res,next) => {
        try{
            if(!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.modify_phone,
                ]))
            {
                return res.status(403).json({success:false,msg:"权限不足"});
            }

            let userId = "";
            let phone = "";
            try{
                if(req.body.userId == null) throw null;
                if(req.body.phone == null) throw null;

                userId = req.body.userId;
                phone = req.body.phone;
            }
            catch(err)
            {
                return res.status(400).json({success:false,msg:"参数错误"});
            }
            const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
            const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

            if(phone != "")
            {
                let result = await DBAccountInfo.findOne({Phone:phone});

                if(result)
                {
                    // XYUID 已经绑定账号
                    return res.status(200).json({success:false,msg:"手机号 已经被绑定，请解绑后设置"});
                }
            }
            let updateRes = await DBAccountInfo.updateOne({_id:mongoose.Types.Long.fromString(userId)},{$set:{Phone:phone}})
            if(updateRes.acknowledged)
            {
                // 修改成功
                WriteOperateLog(req,`手机号修改 ${JSON.stringify(req.body)}`);
                return res.status(200).json({success:true,msg:"绑定成功"});
            }else{
                return res.status(200).json({success:false,msg:"绑定失败"});
            }
        }
        catch(err)
        {
            return res.status(200).json({success:false,msg:String(err)});
        }
    }
)



// @route  POST api/game/account/channel_id/modify
// @desc   return success
// @access Private
router.post(
    '/channel_id/modify',
    passport.authenticate('jwt', { session: false }),
    async (req, res,next) => {
        try{
            if(!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.modify_channel_id,
                ]))
            {
                return res.status(403).json({success:false,msg:"权限不足"});
            }

            let userId = "";
            let channelId = "";
            try{
                if(req.body.userId == null) throw null;
                if(req.body.channelId == null) throw null;

                userId = req.body.userId;
                channelId = req.body.channelId;
            }
            catch(err)
            {
                return res.status(400).json({success:false,msg:"参数错误"});
            }
            const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
            const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

            let updateRes = await DBAccountInfo.updateOne({_id:mongoose.Types.Long.fromString(userId)},{$set:{ChannelId:channelId}})
            if(updateRes.acknowledged && updateRes.modifiedCount)
            {
                // 修改成功
                WriteOperateLog(req,`ChannelId修改 ${JSON.stringify(req.body)}`);
                return res.status(200).json({success:true,msg:"绑定成功"});
            }else{
                return res.status(200).json({success:false,msg:"绑定失败"});
            }
        }
        catch(err)
        {
            return res.status(200).json({success:false,msg:String(err)});
        }
    }
)

// @route  POST api/game/account/identity/modify
// @desc   return success
// @access Private
router.post(
  '/identity/modify',
  passport.authenticate('jwt', { session: false }),
  async (req, res,next) => {
      try{
          if(!permiss.PermissOkAny(req.user.identity,
              [
                  EPermiss.modify_identity,
              ]))
          {
              return res.status(403).json({success:false,msg:"权限不足"});
          }

          let userId = "";
          let identity = "";
          try{
              if(req.body.userId == null) throw null;
              if(req.body.identity == null) throw null;

              userId = req.body.userId;
              identity = req.body.identity;
          }
          catch(err)
          {
              return res.status(400).json({success:false,msg:"参数错误"});
          }
          const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
          const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

          let updateRes = await DBAccountInfo.updateOne({_id:mongoose.Types.Long.fromString(userId)},{$set:{Identity:identity}})
          if(updateRes.acknowledged && updateRes.modifiedCount)
          {
              // 修改成功
              WriteOperateLog(req,`Identity修改 ${JSON.stringify(req.body)}`);

              await axios.post(db.gmUrl + "/api/player/UpdateAccountIdentity", {
                  UserId: userId,
              });

              return res.status(200).json({success:true,msg:"修改成功"});
          }else{
              return res.status(200).json({success:false,msg:"修改失败"});
          }
      }
      catch(err)
      {
          return res.status(200).json({success:false,msg:String(err)});
      }
  }
)

// @route  POST api/game/account/idcard/modify
// @desc   return success
// @access Private
router.post(
  '/idcard/modify',
  passport.authenticate('jwt', { session: false }),
  async (req, res,next) => {
      try{
          if(!permiss.PermissOkAny(req.user.identity,
              [
                  EPermiss.modify_idcard,
              ]))
          {
              return res.status(403).json({success:false,msg:"权限不足"});
          }

          let userId = "";
          let idcard = "";
          let name = "";
          try{
              if(req.body.userId == null) throw null;
              if(req.body.idcard == null) throw null;
              if(req.body.name == null) throw null;

              userId = req.body.userId;
              idcard = req.body.idcard;
              name = req.body.name;
          }
          catch(err)
          {
              return res.status(400).json({success:false,msg:"参数错误"});
          }
          const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
          const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

          let idInspect = 0;
          if(idcard != "")
          {
              let result = await DBAccountInfo.findOne({IdCard:idcard});

              if(result)
              {
                  // 身份证 已经绑定账号
                  return res.status(200).json({success:false,msg:"身份证 已经被绑定"});
              }
              idInspect = 1;
          }
          let updateRes = await DBAccountInfo.updateOne({_id:mongoose.Types.Long.fromString(userId)},{$set:{IdCard:idcard,Name:name,IdInspect:idInspect}})
          if(updateRes.acknowledged)
          {
              // 修改成功
              WriteOperateLog(req,`实名信息修改 ${JSON.stringify(req.body)}`);
              return res.status(200).json({success:true,msg:"实名信息修改成功"});
          }else{
              return res.status(200).json({success:false,msg:"实名信息修改失败"});
          }
      }
      catch(err)
      {
          return res.status(200).json({success:false,msg:String(err)});
      }
  }
)

// @route  POST api/game/account/password/modify
// @desc   return success
// @access Private
router.post(
  '/password/modify',
  passport.authenticate('jwt', { session: false }),
  async (req, res,next) => {
      try{
          if(!permiss.PermissOkAny(req.user.identity,
              [
                  EPermiss.modify_password,
              ]))
          {
              return res.status(403).json({success:false,msg:"权限不足"});
          }

          let userId = "";
          let password = "";
          try{
              if(req.body.userId == null) throw null;
              if(req.body.password == null) throw null;

              userId = req.body.userId;
              password = req.body.password;
          }
          catch(err)
          {
              return res.status(400).json({success:false,msg:"参数错误"});
          }
          const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
          const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

          let updateRes = await DBAccountInfo.updateOne({_id:mongoose.Types.Long.fromString(userId)},{$set:{Password:password}})
          if(updateRes.acknowledged && updateRes.modifiedCount)
          {
              // 修改成功
              WriteOperateLog(req,`Password修改 ${JSON.stringify(req.body)}`);
              return res.status(200).json({success:true,msg:"修改成功"});
          }else{
              return res.status(200).json({success:false,msg:"修改失败"});
          }
      }
      catch(err)
      {
          return res.status(200).json({success:false,msg:String(err)});
      }
  }
)

// @route  POST api/game/account/search
// @desc   return gameplayerinfo
// @access Private
router.post(
    '/search',
    passport.authenticate('jwt', { session: false }),
    async (req, res,next) => {
        try{

            if(!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.search_account,
                ]))
            {
                return res.status(403).json({success:false,msg:"权限不足"});
            }

            let search = '';
            let skip = 0;
            let limit = 1;
            try{
                if(req.body.search == null) throw null;
                if(req.body.skip == null) throw null;
                if(req.body.limit == null) throw null;

                search = req.body.search;
                skip = req.body.skip;
                limit = req.body.limit;

                if(limit > 50) limit = 50;
            }
            catch(err)
            {
                return res.status(400).json({success:false,msg:"参数错误"});
            }

            const keyword = String(search ?? '').trim();
            const findUid = parseLongIfNumeric(keyword);

            let query = {};
            if(keyword.length > 0)
            {
                const reg = new RegExp(escapeRegex(keyword), 'i');
                query = {$or:[{ChannelId:reg},{XYAccountNumber:reg},{Phone:reg}]};
                if (findUid !== null) {
                    query.$or.push({_id:findUid});
                }
            }
            console.log(query);

            const CreateDBAccountInfo = require('../../../models/DBAccountInfo');
            const DBAccountInfo = CreateDBAccountInfo(db.accountMongoURL);

            let args = await Promise.all([
                DBAccountInfo.count(query),
                DBAccountInfo.find(query).skip(skip).limit(limit)
            ]);

            const newData = [];
            for(let i = 0; i < args[1].length; i++)
            {
                const data = args[1][i];
                newData.push({
                    UserId: String(data._id),
                    XYAccountNumber: data.XYAccountNumber,
                    Phone: data.Phone,
                    ChannelId: data.ChannelId,
                    LastLoginTime: data.LastLoginTime.getTime(),
                });
            }
            return res.status(200).json({success:true,total:args[0],data:newData});
        }
        catch(err)
        {
            return res.status(200).json({success:false,msg:String(err)});
        }
    }
  );




module.exports = router;
