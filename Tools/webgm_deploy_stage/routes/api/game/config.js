const express = require('express');
const router = express.Router();
const db = require('../../../config/db');
const passport = require('passport');

const CreateItemConfigData = require('../../../models/ItemConfigData');
const CreateAttrEntryConfigData = require('../../../models/AttrEntryConfigData');

const mongoose = require('mongoose');
require('mongoose-long')(mongoose);


// @route  POST api/game/config/item_data
// @desc   return gameplayerinfo
// @access Private
router.post(
    '/item_data',
    passport.authenticate('jwt', { session: false }),
    async (req, res) => {
        let data = [];
        try{
            if(req.body.data)data = req.body.data;
     
            const ItemConfigData = CreateItemConfigData(db.mongoURL);
            const result1 = await ItemConfigData.deleteMany({});
            const result2 = await ItemConfigData.insertMany(data);
        }
        catch(err)
        {
            console.log(err);
            return res.status(400).json({success:false,msg:"数据异常"});
        }
        return res.status(200).json({success:true});
    }
  );

// @route  POST api/game/config/attr_entry_data
// @desc   return gameplayerinfo
// @access Private
router.post(
    '/attr_entry_data',
    passport.authenticate('jwt', { session: false }),
    async (req, res) => {
        let data = [];
        try{
            if(req.body.data)data = req.body.data;
     
            const AttrEntryConfigData = CreateAttrEntryConfigData(db.mongoURL);
            const result1 = await AttrEntryConfigData.deleteMany({});
            const result2 = await AttrEntryConfigData.insertMany(data);
        }
        catch(err)
        {
            console.log(err);
            return res.status(400).json({success:false,msg:"数据异常"});
        }
        return res.status(200).json({success:true});
    }
  );

module.exports = router;
