const express = require('express');
const router = express.Router();
const bcrypt = require('bcryptjs');
const jwt = require('jsonwebtoken');
const passport = require('passport');
const permiss = require('../../common/permiss.js');

const keys = require('../../config/keys')
const db = require('../../config/db')

const CreateUser = require('../../models/User.js')
const { WriteOperateLog,WriteOperateLog2 }= require('../../models/OperateLog.js')

const EPermiss = permiss.EPermiss;


// @route  GET api/users/init
// @desc   返回的请求的json数据
// @access public
router.get('/init', (req, res) => {
  const name = "admin";
  const email = "admin@qq.com";
  const password = "123456";
  const identity = "admin";

  const User = CreateUser(db.mongoURL);

  User.findOne({$or:[{email: email},{name: name}]})
  .then(user=>{
    if(user)
    {
      return res.status(200).json({success:false,msg:'昵称或邮箱已被注册!'});
    }else{
      const newUser = new User({
        name: name,
        email:email,
        password:password,
        identity:identity
      });

      bcrypt.genSalt(10,function(err,salt){
        bcrypt.hash(newUser.password,salt,(err,hash)=>{
          if(err)throw err;

          newUser.password = hash;

          newUser.save()
                .then(user=>res.json({
                  success:true,
                  user:{
                    name:user.name,
                    email:user.email,
                    identity:user.identity
                }}))
                .catch(err=>console.log(err));
        });
      });
    }

  });
  });

  /*
// @route   POST api/users/register
// @desc    注册账号
// @access  public
router.post('/register',(req,res)=>{
  const name = req.body.name;
  const email = req.body.email;
  const password = req.body.password;
  const identity = req.body.identity;

  const User = CreateUser(db.mongoURL);

  User.findOne({$or:[{email: email},{name: name}]})
  .then(user=>{
    if(user)
    {
      return res.status(200).json({success:false,msg:'昵称或邮箱已被注册!'});
    }else{
      const newUser = new User({
        name: name,
        email:email,
        password:password,
        identity:identity
      });

      bcrypt.genSalt(10,function(err,salt){
        bcrypt.hash(newUser.password,salt,(err,hash)=>{
          if(err)throw err;

          newUser.password = hash;

          newUser.save()
                .then(user=>res.json({
                  success:true,
                  user:{
                    name:user.name,
                    email:user.email,
                    identity:user.identity
                }}))
                .catch(err=>console.log(err));
        });
      });
    }

  });
});
*/

// @route   POST api/users/login
// @desc    登录账号
// @access  public
router.post('/login',(req,res)=>{
  const name = req.body.name;
  const password = req.body.password;

  const User = CreateUser(db.mongoURL);
  User.findOne({name:name})
  .then(user=>{
    if(!user)
    {
      return res.status(200).json({success:false,msg:'用户不存在!'});
    }

    bcrypt.compare(password,user.password).then(isMatch=>{
      if(!isMatch)
      {
        return res.status(200).json({success:false,msg:'密码错误!'});
      }

      const rule = {
        id: user.id,
        name: user.name,
        identity: user.identity
      };

      jwt.sign(rule,keys.secretOrKey,{expiresIn:3600 * 24},(err,token)=>{
        if(err)throw err;

        WriteOperateLog2(req,user.name,`Bearer ${token}`,"登录账号");

        res.json({
          success:true,
          token:'Bearer '+token
        });
      });
    });

  });
});

// @route  GET api/users/current
// @desc   return test message
// @access Private
router.post(
  '/current',
  passport.authenticate('jwt', { session: false }),
  (req, res) => {
    return res.json({
      success:true,
      user:{
        name:req.user.name,
        email:req.user.email,
        identity:req.user.identity
      }
    });
  }
);

// @route  POST api/users/upload-avatar
// @desc   return test message
// @access Private
router.post('/upload-avatar',
 (req, res) => {
  try {
      if(!req.files) {
          res.send({
              status: false,
              message: 'No file uploaded'
          });
      } else {
          //Use the name of the input field (i.e. "avatar") to retrieve the uploaded file
          let avatar = req.files.avatar;

          //Use the mv() method to place the file in upload directory (i.e. "uploads")
          avatar.mv('./uploads/' + avatar.name);

          //send response
          res.send({
              status: true,
              message: 'File is uploaded',
              data: {
                  name: avatar.name,
                  mimetype: avatar.mimetype,
                  size: avatar.size
              }
          });
      }
  } catch (err) {
      res.status(500).send(err);
  }
});


// @route   POST api/users/all_users
// @desc    全部用户
// @access  public
router.post('/all_users',
  passport.authenticate('jwt', { session: false }),
  async (req,res)=>{

  try{
    if(!permiss.PermissOkAny(req.user.identity,
      [
        EPermiss.user_manager,
      ]))
    {
        return res.status(403).json({success:false,msg:"权限不足"});
    }
    const User = CreateUser(db.mongoURL);

    const result = await User.find({});
    const newData = [];
    for(const item of result)
    {
      newData.push({
        Id:String(item._id),
        Name:item.name,
        Identity:item.identity,
      });
    }
    return res.status(200).json({success:true,data:newData});
  }
  catch(err)
  {
    return res.status(200).json({success:false,msg:String(err)});
  }
});

// @route   POST api/users/add
// @desc    添加用户
// @access  public
router.post('/add',
  passport.authenticate('jwt', { session: false }),
  async (req,res)=>{

  try{
    if(!permiss.PermissOkAny(req.user.identity,
    [
      EPermiss.user_manager
    ]))
    {
        return res.status(403).json({success:false,msg:"权限不足"});
    }

    let name = "";
    let email = "";
    let password = "";
    let identity = "";
    try{
        if(req.body.name == null) throw null;
        if(req.body.email == null) throw null;
        if(req.body.password == null) throw null;
        if(req.body.identity == null) throw null;

        name = req.body.name;
        email = req.body.email;
        password = req.body.password;
        identity = req.body.identity;
    }
    catch(err)
    {
        return res.status(400).json({success:false,msg:"参数错误"});
    }

    const User = CreateUser(db.mongoURL);

    const result = await User.findOne({$or:[{email: email},{name: name}]});
    if(result != null)
    {
      return res.status(200).json({success:false,msg:"昵称或邮箱已存在!"});
    }

    const newUser = new User({
      name: name,
      email: email,
      password: password,
      identity: identity
    });

    let salt = await bcrypt.genSalt(10);
    let hash = await bcrypt.hash(newUser.password,salt);

    newUser.password = hash;

    await newUser.save();

    WriteOperateLog(req,`添加用户 ${JSON.stringify(req.body)}`);
    res.status(200).json({success:true,msg:"添加成功"});
  }
  catch(err)
  {
    return res.status(200).json({success:false,msg:String(err)});
  }
});

// @route   POST api/users/modify/password
// @desc    修改用户密码
// @access  public
router.post('/modify/password',
  passport.authenticate('jwt', { session: false }),
  async (req,res)=>{

  try{
    if(!permiss.PermissOkAny(req.user.identity,
      [
        EPermiss.user_manager,
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

    let salt = await bcrypt.genSalt(10);
    let hash = await bcrypt.hash(password,salt);

    const User = CreateUser(db.mongoURL);

    const result = await User.updateOne({_id:userId},{password:hash});
    if(result.acknowledged && result.modifiedCount)
    {
        // 修改成功
      WriteOperateLog(req,`修改密码 ${JSON.stringify(req.body)}`);
      return res.status(200).json({success:true,msg:"修改成功"});
    }else{
        return res.status(200).json({success:false,msg:"修改失败"});
    }
  }
  catch(err)
  {
    return res.status(200).json({success:false,msg:String(err)});
  }
});


// @route   POST api/users/modify/identity
// @desc    修改用户权限
// @access  public
router.post('/modify/identity',
  passport.authenticate('jwt', { session: false }),
  async (req,res)=>{

  try{
    if(!permiss.PermissOkAny(req.user.identity,
      [
        EPermiss.user_manager,
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

    const User = CreateUser(db.mongoURL);

    const result = await User.updateOne({_id:userId},{identity:identity});
    if(result.acknowledged && result.modifiedCount)
    {
        // 修改成功
        WriteOperateLog(req,`修改权限 ${JSON.stringify(req.body)}`);
        return res.status(200).json({success:true,msg:"修改成功"});
    }else{
        return res.status(200).json({success:false,msg:"修改失败"});
    }
  }
  catch(err)
  {
    return res.status(200).json({success:false,msg:String(err)});
  }
});

// @route   POST api/users/remove
// @desc    删除用户
// @access  public
router.post('/remove',
  passport.authenticate('jwt', { session: false }),
  async (req,res)=>{
  try{
    if(!permiss.PermissOkAny(req.user.identity,
      [
        EPermiss.user_manager,
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

    const User = CreateUser(db.mongoURL);
    const result = await User.deleteOne({_id:userId});
    if(result.deletedCount > 0)
    {
        // 移除成功
      WriteOperateLog(req,`删除用户 ${JSON.stringify(req.body)}`);

        return res.status(200).json({success:true,msg:"移除成功"});
    }else{
        return res.status(200).json({success:false,msg:"移除失败"});
    }
  }
  catch(err)
  {
    return res.status(200).json({success:false,msg:String(err)});
  }
});

module.exports = router;