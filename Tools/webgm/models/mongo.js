const mongoose = require('mongoose');

mongoose.Promise = global.Promise;

function getMongodbConfig() {
    let options = {
        useNewUrlParser: true,
        maxPoolSize: 5, // 连接池中维护的连接数
        wtimeoutMS: 120,
    }
    return options;
}

let mongoClient = null;

let AllConnect = {};
/**
 * 关闭 Mongo 连接
 */
function close() {
    mongoClient.close();
}

function CreateConnect(url){
    //console.log(`count = ${count}`);

    if(AllConnect[url] == null)
    {
        mongoClient = mongoose.createConnection(url, getMongodbConfig());
        mongoClient.set("strictQuery", false);

        mongoClient.on('connected', function() {
            //console.log(new Date().getTime())
            //console.log('Mongoose连接至 ：' + url);
        });

        /**
         * Mongo 连接失败回调
         */
        mongoClient.on('error', function(err) {
            //console.log('Mongoose 连接失败，原因: ' + err);
        });
        /**
         * Mongo 关闭连接回调
         */
        mongoClient.on('disconnected', function() {
            //console.log('Mongoose 连接关闭');
        });
        AllConnect[url] = mongoClient;

    }else{
        mongoClient = AllConnect[url];
    }

    return mongoClient;
};

module.exports = {
    CreateConnect : CreateConnect,
    close: close,
}