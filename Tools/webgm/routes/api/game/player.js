const express = require('express');
const router = express.Router();
const db = require('../../../config/db');
const passport = require('passport');
const axios = require('axios');
const permiss = require('../../../common/permiss.js');

const CreateDBGamePlayerData = require('../../../models/DBGamePlayerData');
const CreateDBPlayerUnitData = require('../../../models/DBPlayerUnitData');
const CreateDBAccountZoneData = require('../../../models/DBAccountZoneData');
const { WriteOperateLog } = require('../../../models/OperateLog.js')

const mongoose = require('mongoose');
require('mongoose-long')(mongoose);

const EPermiss = permiss.EPermiss;
const EDBItemValue = {
    Level: 12,
    Quantity: 43,
};
const EItemInComponent = {
    Equipment: 3,
    Backpack: 2,
    Warehouse: 4,
};

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

// @route  POST api/game/player/search
// @desc   return gameplayerinfo
// @access Private
router.post(
    '/search',
    passport.authenticate('jwt', { session: false }),
    (req, res) => {

        if (!permiss.PermissOkAny(req.user.identity,
            [
                EPermiss.search_role,
            ])) {
            return res.status(403).json({ success: false, msg: "权限不足" });
        }

        let zoneId = 0;
        let roleName = '';
        let skip = 0;
        let limit = 1;
        try {
            if (req.body.zoneId) zoneId = req.body.zoneId;
            if (req.body.roleName) roleName = req.body.roleName;
            if (req.body.skip) skip = req.body.skip;
            if (req.body.limit) limit = req.body.limit;
            if (limit > 50) limit = 50;
        }
        catch (err) {
            return res.status(400).json({ success: false, msg: "参数错误" });
        }

        const zoneConfig = db.gameMongo[zoneId];
        if (!zoneConfig || !zoneConfig.URL) {
            return res.status(400).json({ success: false, msg: "鍖烘湇閿欒" });
        }

        const keyword = String(roleName ?? '').trim();
        const findUid = parseLongIfNumeric(keyword);
        const query = { IsDisposePlayer: 0 };
        if (keyword.length > 0) {
            const reg = new RegExp(escapeRegex(keyword), 'i');
            query.$or = [{ NickName: reg }];
            if (findUid !== null) {
                query.$or.push({ _id: findUid }, { UserId: findUid });
            }
        }

        const DBGamePlayerData = CreateDBGamePlayerData(zoneConfig.URL);
        console.log(query);
        Promise.all([
            DBGamePlayerData.count(query),
            DBGamePlayerData.find(query).skip(skip).limit(limit)
        ]).then((args) => {
            const newData = [];
            for (let i = 0; i < args[1].length; i++) {
                const data = args[1][i];
                newData.push({
                    GameUserId: String(data._id),
                    UserId: String(data.UserId),
                    GameAreaId: data.GameAreaId,
                    PlayerTypeId: data.PlayerTypeId,
                    NickName: data.NickName,
                    Level: data.Level,
                    OccupationLevel: data.OccupationLevel,
                });
            }
            return res.status(200).json({ success: true, total: args[0], data: newData });
        }).catch(err => {
            console.log(err);
            return res.status(200).json({ success: false, msg: String(err) });
        });
    }
);

// @route  POST api/game/player/archive
// @desc   return gameplayerinfo
// @access Private
router.post(
    '/archive',
    passport.authenticate('jwt', { session: false }),
    (req, res) => {

        if (!permiss.PermissOkAny(req.user.identity,
            [
                EPermiss.role_view,
            ])) {
            return res.status(403).json({ success: false, msg: "权限不足" });
        }

        let zoneId = 0;
        let userId = '';

        if (req.body.zoneId) zoneId = req.body.zoneId;
        if (req.body.userId) userId = req.body.userId;

        const DBGamePlayerData = CreateDBGamePlayerData(db.gameMongo[zoneId].URL);

        DBGamePlayerData.find({ UserId: userId }).then(role => {
            const newData = [];
            for (i = 0; i < role.length; i++) {
                const data = role[i];
                newData.push({
                    GameUserId: String(data._id),
                    UserId: String(data.UserId),
                    GameAreaId: data.GameAreaId,
                    PlayerTypeId: data.PlayerTypeId,
                    NickName: data.NickName,
                    Level: data.Level,
                    OccupationLevel: data.OccupationLevel,
                    IsDisposePlayer: data.IsDisposePlayer,
                });
            }
            return res.status(200).json({ success: true, data: newData });
        }).catch(err => {
            console.log(err);
        });
    });

// @route  POST api/game/player/role_data
// @desc   return gameplayerinfo
// @access Private
router.post(
    '/role_data',
    passport.authenticate('jwt', { session: false }),
    (req, res) => {

        if (!permiss.PermissOkAny(req.user.identity,
            [
                EPermiss.role_info_view,
            ])) {
            return res.status(403).json({ success: false, msg: "权限不足" });
        }

        let zoneId = 0;
        let gameUserId = '';

        if (req.body.zoneId) zoneId = req.body.zoneId;
        if (req.body.gameUserId) gameUserId = req.body.gameUserId;

        const DBGamePlayerData = CreateDBGamePlayerData(db.gameMongo[zoneId].URL);
        const DBPlayerUnitData = CreateDBPlayerUnitData(db.gameMongo[zoneId].URL);
        Promise.all([
            DBGamePlayerData.findOne({ _id: mongoose.Types.Long.fromString(gameUserId) }),
            DBPlayerUnitData.findOne({ GameUserId: mongoose.Types.Long.fromString(gameUserId) })
        ]).then((args) => {
            if (args[0]) {
                const data = args[0];
                newData = {
                    GameUserId: String(data._id),
                    UserId: String(data.UserId),
                    GameAreaId: data.GameAreaId,
                    PlayerTypeId: data.PlayerTypeId,
                    NickName: data.NickName,
                    Level: data.Level,
                    OccupationLevel: data.OccupationLevel,
                    GoldCoin: data.GoldCoin,
                    MiracleCoin: data.MiracleCoin,
                    Strength: data.Strength,
                    Willpower: data.Willpower,
                    Agility: data.Agility,
                    BoneGas: data.BoneGas,
                    Command: data.Command,
                    FreePoint: data.FreePoint,
                    CreateTimeTick: data.CreateTimeTick,
                    IsDisposePlayer: data.IsDisposePlayer,
                    UnitData: {}
                };
                if (args[1]) {
                    const unitData = args[1];
                    newData.UnitData.MapId = unitData.Index;
                    newData.UnitData.X = unitData.X;
                    newData.UnitData.Y = unitData.Y;
                    newData.UnitData.Angle = unitData.Angle;
                    newData.UnitData.Hp = unitData.Hp;
                    newData.UnitData.Mp = unitData.Mp;
                    newData.UnitData.SD = unitData.SD;
                    newData.UnitData.AG = unitData.AG;
                }
                return res.status(200).json({ success: true, data: newData });
            }
            return res.status(200).json({ success: false, msg: '角色不存在' });
        });
    });

// @route  POST api/game/player/account_zone_data
// @desc   return gameplayerinfo
// @access Private
router.post(
  '/account_zone_data',
  passport.authenticate('jwt', { session: false }),
  async (req, res) => {
      try{
        if (!permiss.PermissOkAny(req.user.identity,
          [
              EPermiss.role_info_view,
          ])) {
          return res.status(403).json({ success: false, msg: "权限不足" });
        }

        let zoneId = 0;
        let userId = '';

        if (req.body.zoneId) zoneId = req.body.zoneId;
        if (req.body.userId) userId = req.body.userId;

        const DBAccountZoneData = CreateDBAccountZoneData(db.gameMongo[zoneId].URL);

        var result = await DBAccountZoneData.findOne({ _id: mongoose.Types.Long.fromString(userId) });
        if(result)
        {
          const data = result;
          newData = {
            UserId:String(data._id),
            YuanbaoCoin:data.YuanbaoCoin,
          };
          return res.status(200).json({ success: true, data: newData });
        }
        return res.status(200).json({ success: false, msg: '账号不存在' });
      }
      catch(err)
      {
        console.log(err);
        return res.status(200).json({ success: false, msg: err });
      }
  });

// @route  POST api/game/player/get_login_record
// @desc   return gameplayerinfo
// @access Private
router.post(
    '/get_login_record',
    passport.authenticate('jwt', { session: false }),
    async (req, res) => {
        try {

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.role_info_view,
                    EPermiss.account_info_view,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let userId = '';

            if (req.body.userId) userId = req.body.userId;

            var result = await axios.post(db.gmUrl + "/api/player/GetLoginRecord", {
                UserId: userId,
            });
            if (result.data.status == false) {
                return res.status(200).json({ success: false, msg: "服务器错误" });
            }

            return res.status(200).json({ success: true, data: result.data.data });
        }
        catch (err) {
            console.log(err);
            //next(err);
            return res.status(200).json({ success: false, msg: "GM服务未开启" });
        }
    });


// @route  POST api/game/player/role_data/modify
// @desc   return success
// @access Private
router.post(
    '/role_data/modify',
    passport.authenticate('jwt', { session: false }),
    (req, res) => {

        if (!permiss.PermissOkAny(req.user.identity,
            [
                EPermiss.modify_role_data,
            ])) {
            return res.status(403).json({ success: false, msg: "权限不足" });
        }

        let zoneId = 0;
        let gameUserId = '';
        if (req.body.zoneId) zoneId = req.body.zoneId;
        if (req.body.gameUserId) gameUserId = req.body.gameUserId;
        // 可以修改的值
        let data = {};
        if (req.body.data) {
            if (req.body.data.Level) data.Level = req.body.data.Level;
            if (req.body.data.GoldCoin) data.GoldCoin = req.body.data.GoldCoin;
            if (req.body.data.MiracleCoin) data.MiracleCoin = req.body.data.MiracleCoin;
            if (req.body.data.OccupationLevel) data.OccupationLevel = req.body.data.OccupationLevel;

            if (req.body.data.Strength) data.Strength = req.body.data.Strength;
            if (req.body.data.Willpower) data.Willpower = req.body.data.Willpower;
            if (req.body.data.Agility) data.Agility = req.body.data.Agility;
            if (req.body.data.BoneGas) data.BoneGas = req.body.data.BoneGas;
            if (req.body.data.Command) data.Command = req.body.data.Command;
            if (req.body.data.FreePoint) data.FreePoint = req.body.data.FreePoint;
        }

        const DBGamePlayerData = CreateDBGamePlayerData(db.gameMongo[zoneId].URL);
        console.log(data);
        DBGamePlayerData.updateOne({ _id: mongoose.Types.Long.fromString(gameUserId) }, { $set: data }).then(
            update_res => {
                console.log(update_res);
                if (update_res.acknowledged && update_res.modifiedCount) {
                    WriteOperateLog(req, `修改角色数据 ${JSON.stringify(req.body)}`);
                    return res.status(200).json({ success: true });
                } else {
                    return res.status(200).json({ success: false, msg: "修改失败" });
                }
            }
        ).catch(err => {
            console.log(err);
            return res.status(200).json({ success: false, msg: String(err) });
        });

    }
);

// @route  POST api/game/player/account_zone_data/modify
// @desc   return success
// @access Private
router.post(
  '/account_zone_data/modify',
  passport.authenticate('jwt', { session: false }),
  (req, res) => {

      if (!permiss.PermissOkAny(req.user.identity,
          [
              EPermiss.modify_role_data,
          ])) {
          return res.status(403).json({ success: false, msg: "权限不足" });
      }

      let zoneId = 0;
      let userId = '';
      if (req.body.zoneId) zoneId = req.body.zoneId;
      if (req.body.userId) userId = req.body.userId;
      // 可以修改的值
      let data = {};
      if (req.body.data) {
          if (req.body.data.YuanbaoCoin) data.YuanbaoCoin = req.body.data.YuanbaoCoin;
      }

      const DBAccountZoneData = CreateDBAccountZoneData(db.gameMongo[zoneId].URL);
      console.log(data);
      DBAccountZoneData.updateOne({ _id: mongoose.Types.Long.fromString(userId) }, { $set: data }).then(
          update_res => {
              console.log(update_res);
              if (update_res.acknowledged && update_res.modifiedCount) {
                  WriteOperateLog(req, `修改DBAccountZoneData数据 ${JSON.stringify(req.body)}`);
                  return res.status(200).json({ success: true });
              } else {
                  return res.status(200).json({ success: false, msg: "修改失败" });
              }
          }
      ).catch(err => {
          console.log(err);
          return res.status(200).json({ success: false, msg: String(err) });
      });

  }
);

// @route  POST api/game/player/equipment
// @desc   return success
// @access Private
router.post(
    '/equipment',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.role_equipment_view,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let gameUserId = '';
            if (req.body.zoneId) zoneId = req.body.zoneId;
            if (req.body.gameUserId) gameUserId = req.body.gameUserId;


            const CreateDBItemData = require('../../../models/DBItemData');
            const DBItemData = CreateDBItemData(db.gameMongo[zoneId].URL);

            const itemListRes = await DBItemData.find({
                GameUserId: mongoose.Types.Long.fromString(gameUserId),
                InComponent: EItemInComponent.Equipment,
                IsDispose: 0
            });

            var equipmentList = [];
            itemListRes.forEach((item, index) => {
                var Level = 0;
                var Quantity = 0;
                if (item) {
                    let LevelKV = item.PropertyData.find(s => {
                        return s[0] == EDBItemValue.Level;
                    });
                    Level = LevelKV == null ? 0 : LevelKV[1];

                    let QuantityKV = item.PropertyData.find(s => {
                        return s[0] == EDBItemValue.Quantity;
                    });
                    Quantity = QuantityKV == null ? 0 : QuantityKV[1];
                }

                equipmentList.push({
                    SlotId: item.posId,
                    ItemUid: String(item._id),
                    ConfigId: item ? item.ConfigID : 0,
                    Level: Level,
                    Quantity: Quantity,
                });
            });

            return res.status(200).json({ success: true, data: equipmentList });
        }
        catch (err) {
            console.log(err);
            next(err);
        }
    }
)

// @route  POST api/game/player/backpack
// @desc   return success
// @access Private
router.post(
    '/backpack',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.role_backpack_view,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let gameUserId = '';
            if (req.body.zoneId) zoneId = req.body.zoneId;
            if (req.body.gameUserId) gameUserId = req.body.gameUserId;


            const CreateDBItemData = require('../../../models/DBItemData');
            const DBItemData = CreateDBItemData(db.gameMongo[zoneId].URL);

            const itemListRes = await DBItemData.find({
                GameUserId: mongoose.Types.Long.fromString(gameUserId),
                InComponent: EItemInComponent.Backpack,
                IsDispose: 0
            });

            var equipmentList = [];
            itemListRes.forEach((item, index) => {
                var Level = 0;
                var Quantity = 0;
                if (item) {
                    let LevelKV = item.PropertyData.find(s => {
                        return s[0] == EDBItemValue.Level;
                    });
                    Level = LevelKV == null ? 0 : LevelKV[1];

                    let QuantityKV = item.PropertyData.find(s => {
                        return s[0] == EDBItemValue.Quantity;
                    });
                    Quantity = QuantityKV == null ? 0 : QuantityKV[1];
                }

                equipmentList.push({
                    PosX: item.posX,
                    PosY: item.posY,
                    ItemUid: String(item._id),
                    ConfigId: item.ConfigID,
                    Level: Level,
                    Quantity: Quantity,
                });
            });

            return res.status(200).json({ success: true, data: equipmentList });
        }
        catch (err) {
            console.log(err);
            next(err);
        }
    }
)

// @route  POST api/game/player/warehouse
// @desc   return success
// @access Private
router.post(
    '/warehouse',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.role_warehouse_view,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let userId = '';
            if (req.body.zoneId) zoneId = req.body.zoneId;
            if (req.body.userId) userId = req.body.userId;


            const CreateDBItemData = require('../../../models/DBItemData');
            const DBItemData = CreateDBItemData(db.gameMongo[zoneId].URL);

            const itemListRes = await DBItemData.find({
                UserId: mongoose.Types.Long.fromString(userId),
                InComponent: EItemInComponent.Warehouse,
                IsDispose: 0
            });

            var equipmentList = [];
            itemListRes.forEach((item, index) => {
                var Level = 0;
                var Quantity = 0;
                if (item) {
                    let LevelKV = item.PropertyData.find(s => {
                        return s[0] == EDBItemValue.Level;
                    });
                    Level = LevelKV == null ? 0 : LevelKV[1];

                    let QuantityKV = item.PropertyData.find(s => {
                        return s[0] == EDBItemValue.Quantity;
                    });
                    Quantity = QuantityKV == null ? 0 : QuantityKV[1];
                }

                equipmentList.push({
                    PosX: item.posX,
                    PosY: item.posY,
                    ItemUid: String(item._id),
                    ConfigId: item.ConfigID,
                    Level: Level,
                    Quantity: Quantity,
                });
            });

            return res.status(200).json({ success: true, data: equipmentList });
        }
        catch (err) {
            console.log(err);
            next(err);
        }
    }
)

// @route  POST api/game/player/mail/send
// @desc   return success
// @access Private
router.post(
    '/mail/send',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {
            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.send_mail,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let userId = '';
            let gameUserId = '';
            let name = '';
            let content = '';
            let mailItems = [];
            if (req.body.zoneId) zoneId = req.body.zoneId;
            if (req.body.userId) userId = req.body.userId;
            if (req.body.gameUserId) gameUserId = req.body.gameUserId;
            if (req.body.name) name = req.body.name;
            if (req.body.content) content = req.body.content;
            if (req.body.mailItems) mailItems = req.body.mailItems;


            var result = await axios.post(db.gmUrl + "/api/player/SendMail", {
                ZoneId: zoneId,
                UserId: userId,
                GameUserId: gameUserId,
                Name: name,
                Content: content,
                MailItems: mailItems
            });
            console.log(JSON.stringify(result.data));
            if (result.data.status == false) {
                console.log(result.data.msg);
                return res.status(200).json({ success: false, msg: "服务器错误" });
            }

            WriteOperateLog(req, `发送邮件 ${JSON.stringify(req.body)}`);
            return res.status(200).json({ success: true, data: "" });
        }
        catch (err) {
            console.log(err);
            //next(err);
            return res.status(200).json({ success: false, msg: "GM服务未开启" });
        }
    }
)


// @route  POST api/game/player/mail/send_full
// @desc   return success
// @access Private
router.post(
    '/mail/send_full',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {
            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.send_mail,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let name = '';
            let content = '';
            let mailItems = [];
            if (req.body.zoneId) zoneId = req.body.zoneId;
            if (req.body.name) name = req.body.name;
            if (req.body.content) content = req.body.content;
            if (req.body.mailItems) mailItems = req.body.mailItems;


            var result = await axios.post(db.gmUrl + "/api/player/SendFullMail", {
                ZoneId: zoneId,
                Name: name,
                Content: content,
                MailItems: mailItems
            });

            if (result.data.status == false) {
                return res.status(200).json({ success: false, msg: "服务器错误" });
            }

            WriteOperateLog(req, `发送全服邮件 ${JSON.stringify(req.body)}`);
            return res.status(200).json({ success: true, data: "" });
        }
        catch (err) {
            console.log(err);
            //next(err);
            return res.status(200).json({ success: false, msg: "GM服务未开启" });
        }
    }
)

// @route  POST api/game/player/title
// @desc   return success
// @access Private
router.post(
    '/title',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.role_title_view,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }

            let zoneId = 0;
            let userId = '';
            let gameUserId = '';
            if (req.body.zoneId) zoneId = req.body.zoneId;
            if (req.body.userId) userId = req.body.userId;
            if (req.body.gameUserId) gameUserId = req.body.gameUserId;


            const CreateDBPlayerTitle = require('../../../models/DBPlayerTitle');
            const DBPlayerTitle = CreateDBPlayerTitle(db.gameMongo[zoneId].URL);


            const titleListRes = await DBPlayerTitle.find({
                UserId: {$in:[mongoose.Types.Long.fromString(userId),mongoose.Types.Long.fromString(gameUserId)]},
                IsDisabled: 0
            });

            let data = [];
            titleListRes.forEach((item, index) => {
                data.push({
                    Id: item._id.toString(),
                    TitleId: item.TitleID,
                    UserId: item.UserId.toString(),
                    Type: item.Type,
                    BeginTime: item.BingTime.toString(),
                    EndTime: item.EndTime.toString(),
                });
            });

            return res.status(200).json({ success: true, data: data });
        }
        catch (err) {
            console.log(err);
            next(err);
        }
    }
)


// @route  POST api/game/player/add/title
// @desc   return success
// @access Private
router.post(
    '/add/title',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.modify_role_title,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }
            let zoneId = 0;
            let userId = '';
            let gameUserId = '';
            let titleId = 0;
            let type = 0;
            let endTime = 0;
            if (req.body.zoneId) zoneId = req.body.zoneId;
            if (req.body.userId) userId = req.body.userId;
            if (req.body.gameUserId) gameUserId = req.body.gameUserId;
            if (req.body.titleId) titleId = req.body.titleId;
            if (req.body.type) type = req.body.type;
            if (req.body.endTime) endTime = req.body.endTime;


            var result = await axios.post(db.gmUrl + "/api/player/AddTitle", {
                ZoneId: zoneId,
                UserId: userId,
                GameUserId: gameUserId,
                TitleId: titleId,
                Type: type,
                EndTime: endTime.toString()
            });

            if (result.data.status == false) {
                return res.status(200).json({ success: false, msg: "服务器错误" });
            }

            WriteOperateLog(req, `添加称号 ${JSON.stringify(req.body)}`);
            return res.status(200).json({ success: true ,data:""});
        }
        catch (err) {
            console.log(err);
            next(err);
        }
    }
)

// @route  POST api/game/player/del/title
// @desc   return success
// @access Private
router.post(
    '/del/title',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {

            if (!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.modify_role_title,
                ])) {
                return res.status(403).json({ success: false, msg: "权限不足" });
            }
            console.log(JSON.stringify(req.body));

            let zoneId = 0;
            let userId = '';
            let gameUserId = '';
            let id = '';
            let titleId = 0;
            if (req.body.zoneId) zoneId = req.body.zoneId;
            if (req.body.userId) userId = req.body.userId;
            if (req.body.gameUserId) gameUserId = req.body.gameUserId;
            if (req.body.id) id = req.body.id;
            if (req.body.titleId) titleId = req.body.titleId;

            console.log(JSON.stringify({
                ZoneId: zoneId,
                UserId: userId,
                GameUserId: gameUserId,
                Id: id,
                TitleId: titleId,
            }));

            var result = await axios.post(db.gmUrl + "/api/player/DelTitle", {
                ZoneId: zoneId,
                UserId: userId,
                GameUserId: gameUserId,
                TitleId: titleId,
                DBId: id,
            });

            if (result.data.status == false) {
                return res.status(200).json({ success: false, msg: "服务器错误" });
            }

            WriteOperateLog(req, `删除称号 ${JSON.stringify(req.body)}`);
            return res.status(200).json({ success: true ,data:""});
        }
        catch (err) {
            console.log(err);
            next(err);
        }
    }
)

module.exports = router;
