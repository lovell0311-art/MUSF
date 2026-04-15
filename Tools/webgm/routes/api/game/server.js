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



// @route  POST api/game/server/game_status
// @desc   return gameplayerinfo
// @access Private
router.post(
    '/game_status',
    passport.authenticate('jwt', { session: false }),
    async (req, res,next) => {
        try{
            if(!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.server_manager
                ]))
            {
                return res.status(403).json({success:false,msg:"权限不足"});
            }
            let serverId = 0;

            if(req.body.serverId)serverId = req.body.serverId;
   
            var result = await axios.post(db.gmUrl + "/api/server/GameStatus",{
                ServerId:serverId
            });
            if(result.data.status == false)
            {
                return res.status(200).json({success:false,msg:"服务器错误"});
            }

            return res.status(200).json({success:true,data:result.data.data});
        }
        catch(err)
        {
            return res.status(200).json({success:false,msg:"GM服务未开启"});
        }
    }
);


// @route  POST api/game/server/run_code
// @desc   return gameplayerinfo
// @access Private
router.post(
    '/run_code',
    passport.authenticate('jwt', { session: false }),
    async (req, res,next) => {
        try{
            if(!permiss.PermissOkAny(req.user.identity,
                [
                    EPermiss.server_manager
                ]))
            {
                return res.status(403).json({success:false,msg:"权限不足"});
            }
            let serverId = 0;
            let code = "";

            if(req.body.serverId)serverId = req.body.serverId;
            if(req.body.code)code = req.body.code;
   
            var result = await axios.post(db.gmUrl + "/api/server/RunCode",{
                ServerId:serverId,
                Code:code
            });
            if(result.data.status == false)
            {
                return res.status(200).json({success:false,msg:"服务器错误"});
            }

            WriteOperateLog(req,`运行Code ${JSON.stringify(req.body)}`);
            return res.status(200).json({success:true,data:result.data.data});
        }
        catch(err)
        {
            return res.status(200).json({success:false,msg:"GM服务未开启"});
        }
    }
);





module.exports = router;
