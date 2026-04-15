const express = require("express");
const path = require("path");
const fileUpload = require('express-fileupload');
const mongoose = require("mongoose");
const bodyParser = require("body-parser");
const passport = require('passport');
const app = express();


const users = require('./routes/api/users');
const player = require('./routes/api/game/player');
const item = require('./routes/api/game/item');
const config = require('./routes/api/game/config');
const server = require('./routes/api/game/server');
const account = require('./routes/api/game/account');
const proxy = require('./routes/api/game/proxy');
const cdkey = require('./routes/api/game/cdkey');



const db = require("./config/db");


app.use(bodyParser.urlencoded({limit: '50mb',extended:false}));
app.use(bodyParser.json({limit: '50mb'}));

app.use(fileUpload({
    createParentPath: true
}));

// passport 初始化
app.use(passport.initialize());
require('./config/passport')(passport);



app.use('/api/users',users);
app.use('/api/game/player',player);
app.use('/api/game/item',item);
app.use('/api/game/config',config);
app.use('/api/game/server',server);
app.use('/api/game/account',account);
app.use('/api/game/proxy',proxy);
app.use('/api/game/cdkey',cdkey);

const clientDistPath = path.join(__dirname, "client", "dist");
app.use(express.static(clientDistPath));
app.get(/^\/(?!api(?:\/|$)).*/, (req, res) => {
    res.sendFile(path.join(clientDistPath, "index.html"));
});


mongoose.set('strictQuery', true);

const port = process.env.PORT || 5000;
/*
let result = '[NumberLong("1698613626615365632"), NumberLong("1704422982200066048"), NumberLong("1698619295972261888"), NumberLong("1698619759828729856"), NumberLong("1678104590841282560"), NumberLong("1704424597107769344"), NumberLong("1704425112503844869"), NumberLong("1704425816878481409"), NumberLong("1704433616539090944"), NumberLong("1704435575044177920"), NumberLong("1704467426521645056"), NumberLong("1704467460881383424"), NumberLong("1704467512420990976"), NumberLong("1704467546780729344"), NumberLong("1704467581140467712"), NumberLong("1704468062176804864"), NumberLong("1704468148076150784"), NumberLong("1704468216795627520"), NumberLong("1704468337054711808")]';
const list = result.match(/[0-9]{19}/g);
for(i = 0;i<list.length;i++)
{
    result = result.replace(`NumberLong("${list[i]}")`,`"${list[i]}"`);
}
console.log(result);

var test = JSON.parse(result);

console.log(test[0]);
*/
const httpServer = app.listen(port,()=>{
    console.log(`Server running on port ${port}`);
    console.log(`Web GM url: http://127.0.0.1:${port}/`);
});

httpServer.on("error", (err) => {
    if (err && err.code === "EADDRINUSE") {
        console.error(`Port ${port} is already in use.`);
        console.error(`If Web GM is already running, open: http://127.0.0.1:${port}/`);
        process.exit(1);
        return;
    }

    console.error(err);
    process.exit(1);
});
