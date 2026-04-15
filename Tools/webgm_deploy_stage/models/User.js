const mongoose = require('mongoose');
const db = require('../config/db')
const Schema = mongoose.Schema;

const UserSchema = new Schema({
    name:{
        type:String,
        required: true
    },
    email:{
        type:String,
        required: true
    },
    password:{
        type:String,
        required: true
    },
    /** root、admin、gm、、proxy */
    identity:{
        type:String,
        required: true
    },
    date:{
        type:Date,
        default: Date.now
    }
});


module.exports = CreateUser = (url)=>{
    let CreateConnect = require('./mongo').CreateConnect;
    var connect = CreateConnect(url);
    return connect.model(db.CollectionName("Users"),UserSchema,db.CollectionName("Users"));
};