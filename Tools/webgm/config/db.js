
const mongoose = require("mongoose");

module.exports = {
    mongoURL: "mongodb://127.0.0.1:58030/Test2",
    accountMongoURL: "mongodb://127.0.0.1:58030/ET",
    gameMongo: [
        {
            ZoneName: "留空",
            URL: "mongodb://127.0.0.1:58030/ET0",
            Valid: false,
        },
        {
            ZoneName: "一区",
            URL: "mongodb://127.0.0.1:58030/ET1",
            Valid: true,
        },
        {
            ZoneName: "二区",
            URL: "mongodb://127.0.0.1:58030/ET2",
            Valid: true,
        }
    ],
    gameLogMongo: [
        {
            ZoneName: "留空",
            URL: "mongodb://127.0.0.1:58030/ETLog",
            Valid: false,
        },
        {
            ZoneName: "一区",
            URL: "mongodb://127.0.0.1:58030/ET1Log",
            Valid: true,
        },
        {
            ZoneName: "二区",
            URL: "mongodb://127.0.0.1:58030/ET2Log",
            Valid: true,
        }
    ],
    CollectionName(name) {
        return "Web_" + name;
    },
    gmUrl: "http://127.0.0.1:65001"
}
