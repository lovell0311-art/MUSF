const express = require('express');
const router = express.Router();
const db = require('../../../config/db');
const passport = require('passport');
const axios = require('axios');
const permiss = require('../../../common/permiss.js');
const fs = require('fs');
const path = require('path');

const CreateDBItemData = require('../../../models/DBItemData');
const CreateDBGamePlayerData = require('../../../models/DBGamePlayerData');
const { WriteOperateLog } = require('../../../models/OperateLog.js');

const mongoose = require('mongoose');
require('mongoose-long')(mongoose);

const EPermiss = permiss.EPermiss;
const STARTUP_CONFIG_PATH = path.resolve(__dirname, '../../../../../Server/Config/StartUpConfig/StartUp_ServerConfig.json');
const RUN_CODE_SUCCESS_PREFIX = 'GM_BACKPACK_ADD_SUCCESS';
const RUN_CODE_ERROR_PREFIX = 'GM_BACKPACK_ADD_ERROR';

let cachedStartUpConfig = null;

function getStartUpConfig() {
    if (cachedStartUpConfig != null) {
        return cachedStartUpConfig;
    }

    const raw = fs.readFileSync(STARTUP_CONFIG_PATH, 'utf8').replace(/^\uFEFF/, '');
    cachedStartUpConfig = JSON.parse(raw);
    return cachedStartUpConfig;
}

function getZoneGameServerIds(zoneId) {
    return getStartUpConfig()
        .filter(item => item.AppType === 'Game' && Number(item.ZoneId) === Number(zoneId))
        .map(item => Number(item.AppId))
        .filter(item => Number.isFinite(item) && item > 0)
        .sort((a, b) => a - b);
}

function getZoneMongoUrl(zoneId) {
    const zoneConfig = db.gameMongo[zoneId];
    if (!zoneConfig || !zoneConfig.URL) {
        return '';
    }
    return zoneConfig.URL;
}

function normalizeInt(value, fallback = 0) {
    const parsed = Number.parseInt(value, 10);
    return Number.isFinite(parsed) ? parsed : fallback;
}

function normalizeBoolean(value) {
    return value === true || value === 1 || value === '1';
}

function normalizeIntArray(value) {
    if (!Array.isArray(value)) {
        return [];
    }

    return value
        .map(item => normalizeInt(item, NaN))
        .filter(item => Number.isFinite(item));
}

function normalizeSpecialMap(value) {
    const result = {};
    if (value == null || typeof value !== 'object' || Array.isArray(value)) {
        return result;
    }

    Object.entries(value).forEach(([key, val]) => {
        const normalizedKey = normalizeInt(key, NaN);
        const normalizedValue = normalizeInt(val, NaN);
        if (Number.isFinite(normalizedKey) && Number.isFinite(normalizedValue)) {
            result[normalizedKey] = normalizedValue;
        }
    });

    return result;
}

function normalizeStringArray(value) {
    if (!Array.isArray(value)) {
        return [];
    }

    return value
        .map(item => String(item ?? '').trim())
        .filter(item => item.length > 0);
}

function normalizeNumericText(value) {
    const text = String(value ?? '').trim();
    return /^\d+$/.test(text) ? text : '';
}

function toLongOrNull(value) {
    const text = normalizeNumericText(value);
    if (!text) {
        return null;
    }
    return mongoose.Types.Long.fromString(text);
}

function normalizeItemCreateAttr(input) {
    const createAttr = input && typeof input === 'object' ? input : {};
    return {
        Level: Math.max(0, normalizeInt(createAttr.Level, 0)),
        Quantity: Math.max(1, normalizeInt(createAttr.Quantity, 1)),
        OptListId: Math.max(0, normalizeInt(createAttr.OptListId, 0)),
        OptLevel: Math.max(0, normalizeInt(createAttr.OptLevel, 0)),
        HaveSkill: normalizeBoolean(createAttr.HaveSkill),
        HaveLucky: normalizeBoolean(createAttr.HaveLucky),
        SetId: Math.max(0, normalizeInt(createAttr.SetId, 0)),
        IsBind: Math.max(0, normalizeInt(createAttr.IsBind, 0)),
        IsTask: normalizeBoolean(createAttr.IsTask),
        ValidTime: Math.max(0, normalizeInt(createAttr.ValidTime, 0)),
        ExpireTimestamp: Math.max(0, normalizeInt(createAttr.ExpireTimestamp, 0)),
        FluoreAttr: Math.max(0, normalizeInt(createAttr.FluoreAttr, 0)),
        FluoreSlotCount: Math.max(0, normalizeInt(createAttr.FluoreSlotCount, 0)),
        FluoreSlot: normalizeIntArray(createAttr.FluoreSlot),
        OptionExcellent: normalizeIntArray(createAttr.OptionExcellent),
        OptionSpecial: normalizeSpecialMap(createAttr.OptionSpecial),
        CustomAttrMethod: normalizeStringArray(createAttr.CustomAttrMethod),
    };
}

function toCSharpStringLiteral(value) {
    return `"${String(value ?? '').replace(/\\/g, '\\\\').replace(/"/g, '\\"')}"`;
}

function appendAttrCode(lines, createAttr) {
    lines.push('var itemCreateAttr = new ItemCreateAttr();');
    lines.push(`itemCreateAttr.Level = ${createAttr.Level};`);
    lines.push(`itemCreateAttr.Quantity = ${createAttr.Quantity};`);
    lines.push(`itemCreateAttr.OptListId = ${createAttr.OptListId};`);
    lines.push(`itemCreateAttr.OptLevel = ${createAttr.OptLevel};`);
    lines.push(`itemCreateAttr.HaveSkill = ${createAttr.HaveSkill ? 'true' : 'false'};`);
    lines.push(`itemCreateAttr.HaveLucky = ${createAttr.HaveLucky ? 'true' : 'false'};`);
    lines.push(`itemCreateAttr.SetId = ${createAttr.SetId};`);
    lines.push(`itemCreateAttr.IsBind = ${createAttr.IsBind};`);
    lines.push(`itemCreateAttr.IsTask = ${createAttr.IsTask ? 'true' : 'false'};`);
    lines.push(`itemCreateAttr.ValidTime = ${createAttr.ValidTime};`);
    lines.push(`itemCreateAttr.ExpireTimestamp = ${createAttr.ExpireTimestamp};`);
    lines.push(`itemCreateAttr.FluoreAttr = ${createAttr.FluoreAttr};`);
    lines.push(`itemCreateAttr.FluoreSlotCount = ${createAttr.FluoreSlotCount};`);

    createAttr.FluoreSlot.forEach(item => {
        lines.push(`itemCreateAttr.FluoreSlot.Add(${item});`);
    });

    createAttr.OptionExcellent.forEach(item => {
        lines.push(`itemCreateAttr.OptionExcellent.Add(${item});`);
    });

    Object.entries(createAttr.OptionSpecial).forEach(([key, val]) => {
        lines.push(`itemCreateAttr.OptionSpecial[${Number(key)}] = ${Number(val)};`);
    });

    createAttr.CustomAttrMethod.forEach(item => {
        lines.push(`itemCreateAttr.CustomAttrMethod.Add(${toCSharpStringLiteral(item)});`);
    });
}

function buildBackpackAddRunCode(zoneId, gameUserId, configId, createAttr) {
    const lines = [
        `const string successPrefix = "${RUN_CODE_SUCCESS_PREFIX}";`,
        `const string errorPrefix = "${RUN_CODE_ERROR_PREFIX}";`,
        `int zoneId = ${Number(zoneId)};`,
        `long gameUserId = ${String(gameUserId)}L;`,
        `int configId = ${Number(configId)};`,
        'void PrintSuccess(string mode, Item item)',
        '{',
        '    Print($"{successPrefix}|{mode}|{item.ItemUID}|{item.data.posX}|{item.data.posY}|{item.ConfigID}");',
        '}',
        'void PrintError(string msg)',
        '{',
        '    Print($"{errorPrefix}|{msg}");',
        '}',
        'try',
        '{',
    ];

    appendAttrCode(lines, createAttr);

    lines.push(
        '    var itemConfigManager = Root.MainFactory.GetCustomComponent<ItemConfigManagerComponent>();',
        '    if (itemConfigManager == null)',
        '    {',
        '        PrintError("ITEM_CONFIG_MANAGER_NOT_FOUND");',
        '        return null;',
        '    }',
        '    var itemConfig = itemConfigManager.Get(configId);',
        '    if (itemConfig == null)',
        '    {',
        '        PrintError("ITEM_CONFIG_NOT_FOUND");',
        '        return null;',
        '    }',
        '    var playerManage = Root.MainFactory.GetCustomComponent<PlayerManageComponent>();',
        '    var player = playerManage?.Get(zoneId, gameUserId);',
        '    if (player != null)',
        '    {',
        '        var backpack = player.GetCustomComponent<BackpackComponent>();',
        '        if (backpack == null)',
        '        {',
        '            PrintError("BACKPACK_COMPONENT_NOT_FOUND");',
        '            return null;',
        '        }',
        '        var newItem = ItemFactory.Create(itemConfig, player.GameAreaId, itemCreateAttr);',
        '        if (!backpack.AddItem(newItem, "webgm_add_backpack_item", false))',
        '        {',
        '            newItem.Dispose();',
        '            PrintError("BACKPACK_FULL");',
        '            return null;',
        '        }',
        '        PrintSuccess("online", newItem);',
        '        return null;',
        '    }',
        '    var dBProxyManager = Root.MainFactory.GetCustomComponent<DBProxyManagerComponent>();',
        '    if (dBProxyManager == null)',
        '    {',
        '        PrintError("DB_PROXY_MANAGER_NOT_FOUND");',
        '        return null;',
        '    }',
        '    var dBProxy = dBProxyManager.GetZoneDB(DBType.Core, zoneId);',
        '    if (dBProxy == null)',
        '    {',
        '        PrintError("ZONE_DB_NOT_FOUND");',
        '        return null;',
        '    }',
        '    var playerList = await dBProxy.Query<DBGamePlayerData>(p => p.Id == gameUserId && p.IsDisposePlayer == 0);',
        '    if (playerList == null || playerList.Count == 0)',
        '    {',
        '        PrintError("ROLE_NOT_FOUND");',
        '        return null;',
        '    }',
        '    var dbRole = playerList[0] as DBGamePlayerData;',
        '    if (dbRole == null)',
        '    {',
        '        PrintError("ROLE_DATA_LOAD_FAILED");',
        '        return null;',
        '    }',
        '    var allItems = await dBProxy.Query<DBItemData>(p => p.GameUserId == gameUserId && p.InComponent == EItemInComponent.Backpack && p.IsDispose == 0);',
        '    var itemBox = new ItemsBoxStatus();',
        '    itemBox.Init(BackpackComponent.I_PackageWidth, BackpackComponent.I_PackageWidth * BackpackComponent.I_PackageHigh);',
        '    if (allItems != null)',
        '    {',
        '        foreach (var raw in allItems)',
        '        {',
        '            var dbItem = raw as DBItemData;',
        '            if (dbItem == null)',
        '            {',
        '                continue;',
        '            }',
        '            var existConfig = itemConfigManager.Get(dbItem.ConfigID);',
        '            if (existConfig == null)',
        '            {',
        '                continue;',
        '            }',
        '            itemBox.AddItem(existConfig.X, existConfig.Y, dbItem.posX, dbItem.posY);',
        '        }',
        '    }',
        '    int posX = 0;',
        '    int posY = 0;',
        '    if (!itemBox.CheckStatus(itemConfig.X, itemConfig.Y, ref posX, ref posY))',
        '    {',
        '        PrintError("BACKPACK_FULL");',
        '        return null;',
        '    }',
        '    var offlineItem = ItemFactory.Create(itemConfig, dbRole.GameAreaId, itemCreateAttr);',
        '    offlineItem.data.GameUserId = dbRole.Id;',
        '    offlineItem.data.UserId = dbRole.UserId;',
        '    offlineItem.data.InComponent = EItemInComponent.Backpack;',
        '    offlineItem.data.posId = 0;',
        '    offlineItem.data.posX = posX;',
        '    offlineItem.data.posY = posY;',
        '    await offlineItem.data.SaveDBNow();',
        '    PrintSuccess("offline", offlineItem);',
        '    offlineItem.Dispose();',
        '}',
        'catch (Exception ex)',
        '{',
        '    PrintError("EXCEPTION:" + ex.Message);',
        '}',
        'return null;'
    );

    return lines.join('\n');
}

function parseRunCodeResult(output) {
    const text = String(output ?? '');
    const lines = text.split(/\r?\n/).map(line => line.trim()).filter(Boolean);

    for (const line of lines) {
        if (!line.startsWith(`${RUN_CODE_SUCCESS_PREFIX}|`)) {
            continue;
        }

        const parts = line.split('|');
        return {
            ok: true,
            data: {
                Mode: parts[1] ?? '',
                ItemUid: parts[2] ?? '',
                PosX: normalizeInt(parts[3], 0),
                PosY: normalizeInt(parts[4], 0),
                ConfigId: normalizeInt(parts[5], 0),
            }
        };
    }

    for (const line of lines) {
        if (!line.startsWith(`${RUN_CODE_ERROR_PREFIX}|`)) {
            continue;
        }

        return {
            ok: false,
            msg: line.slice((`${RUN_CODE_ERROR_PREFIX}|`).length) || 'add backpack item failed',
        };
    }

    return {
        ok: false,
        msg: text.trim() || 'add backpack item failed',
    };
}

function isTerminalRunCodeError(msg) {
    const text = String(msg ?? '').trim();
    if (!text) {
        return false;
    }

    return (
        text === 'ITEM_CONFIG_MANAGER_NOT_FOUND' ||
        text === 'ITEM_CONFIG_NOT_FOUND' ||
        text === 'BACKPACK_COMPONENT_NOT_FOUND' ||
        text === 'DB_PROXY_MANAGER_NOT_FOUND' ||
        text === 'ZONE_DB_NOT_FOUND' ||
        text === 'ROLE_NOT_FOUND' ||
        text === 'ROLE_DATA_LOAD_FAILED' ||
        text === 'BACKPACK_FULL' ||
        text.startsWith('EXCEPTION:')
    );
}

async function resolveRoleUserId(zoneId, gameUserId, userId) {
    const directUserId = normalizeNumericText(userId);
    if (directUserId) {
        return directUserId;
    }

    const zoneMongoUrl = getZoneMongoUrl(zoneId);
    if (!zoneMongoUrl) {
        return '';
    }

    const DBGamePlayerData = CreateDBGamePlayerData(zoneMongoUrl);
    const role = await DBGamePlayerData.findOne({
        _id: mongoose.Types.Long.fromString(String(gameUserId)),
        IsDisposePlayer: 0,
    });

    if (role == null) {
        return '';
    }

    return String(role.UserId ?? '');
}

async function isGameServerReachable(serverId) {
    try {
        const result = await axios.post(db.gmUrl + '/api/server/GameStatus', {
            ServerId: Number(serverId),
        });
        return (
            result?.data?.status !== false &&
            normalizeInt(result?.data?.data?.ServerStatus, 0) === 1
        );
    } catch (err) {
        return false;
    }
}

async function resolveCandidateGameServers(zoneId, userId, gameUserId) {
    const zoneServerIds = [...new Set(getZoneGameServerIds(zoneId))];

    try {
        const loginRecord = await axios.post(db.gmUrl + '/api/player/GetLoginRecord', {
            UserId: String(userId),
        });
        const loginData = loginRecord?.data;
        const record = loginData?.data;
        const gameServerId = normalizeInt(record?.GameServerId, 0);
        if (
            loginData &&
            loginData.status !== false &&
            record &&
            String(record.GameUserId ?? '') === String(gameUserId) &&
            gameServerId > 0
        ) {
            if (await isGameServerReachable(gameServerId)) {
                return {
                    serverIds: [gameServerId],
                    onlineLocked: true,
                };
            }

            const fallbackServerIds = zoneServerIds.filter(serverId => serverId !== gameServerId);
            return {
                serverIds: fallbackServerIds.length > 0 ? fallbackServerIds : [gameServerId],
                onlineLocked: false,
            };
        }
    } catch (err) {
        // Ignore and fall back to zone defaults.
    }

    return {
        serverIds: zoneServerIds,
        onlineLocked: false,
    };
}

async function executeBackpackAdd(zoneId, userId, gameUserId, configId, createAttr) {
    const candidateInfo = await resolveCandidateGameServers(zoneId, userId, gameUserId);
    if (candidateInfo.serverIds.length === 0) {
        return {
            ok: false,
            msg: 'no game server available for this zone',
        };
    }

    const code = buildBackpackAddRunCode(zoneId, gameUserId, configId, createAttr);
    let lastError = 'add backpack item failed';

    for (const serverId of candidateInfo.serverIds) {
        try {
            const result = await axios.post(db.gmUrl + '/api/server/RunCode', {
                ServerId: serverId,
                Code: code,
            });

            if (result?.data?.status === false) {
                lastError = result.data.msg || lastError;
                continue;
            }

            const parsed = parseRunCodeResult(result?.data?.data?.Return);
            if (parsed.ok) {
                parsed.data.ServerId = serverId;
                return parsed;
            }

            if (isTerminalRunCodeError(parsed.msg)) {
                return parsed;
            }

            lastError = parsed.msg || lastError;
        } catch (err) {
            lastError = err?.message || String(err);
        }
    }

    return {
        ok: false,
        msg: lastError,
    };
}

// @route  POST api/game/item/get
// @desc   get item detail
// @access Private
router.post(
    '/get',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {
            if (!permiss.PermissOkAny(req.user.identity, [EPermiss.edit_item])) {
                return res.status(403).json({ success: false, msg: 'permission denied' });
            }

            const zoneId = normalizeInt(req.body.zoneId, 0);
            const itemUid = normalizeNumericText(req.body.itemUid);
            if (!zoneId || !itemUid) {
                return res.status(400).json({ success: false, msg: 'invalid params' });
            }

            const zoneMongoUrl = getZoneMongoUrl(zoneId);
            if (!zoneMongoUrl) {
                return res.status(400).json({ success: false, msg: 'invalid zone' });
            }

            const DBItemData = CreateDBItemData(zoneMongoUrl);
            const itemRes = await DBItemData.findOne({ _id: mongoose.Types.Long.fromString(itemUid) });
            if (itemRes == null) {
                return res.status(200).json({ success: false, msg: 'item not found' });
            }

            return res.status(200).json({
                success: true,
                data: {
                    ItemUid: String(itemRes._id),
                    GameUserId: String(itemRes.GameUserId),
                    GameAreaId: itemRes.GameAreaId,
                    IsDispose: itemRes.IsDispose,
                    CreateTimeTick: itemRes.CreateTimeTick,
                    ConfigID: itemRes.ConfigID,
                    posX: itemRes.posX,
                    posY: itemRes.posY,
                    DurabilitySmall: itemRes.DurabilitySmall,
                    PropertyData: itemRes.PropertyData,
                    ExcellentEntry: itemRes.ExcellentEntry,
                    SpecialEntry: itemRes.SpecialEntry,
                }
            });
        } catch (err) {
            next(err);
        }
    }
);

// @route  POST api/game/item/modify
// @desc   update item data
// @access Private
router.post(
    '/modify',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {
            if (!permiss.PermissOkAny(req.user.identity, [EPermiss.edit_item])) {
                return res.status(403).json({ success: false, msg: 'permission denied' });
            }

            const zoneId = normalizeInt(req.body.zoneId, 0);
            const itemUid = normalizeNumericText(req.body.itemUid);
            const data = req.body.data && typeof req.body.data === 'object' ? req.body.data : {};

            if (!zoneId || !itemUid) {
                return res.status(400).json({ success: false, msg: 'invalid params' });
            }

            const zoneMongoUrl = getZoneMongoUrl(zoneId);
            if (!zoneMongoUrl) {
                return res.status(400).json({ success: false, msg: 'invalid zone' });
            }

            const DBItemData = CreateDBItemData(zoneMongoUrl);
            const itemRes = await DBItemData.updateOne(
                { _id: mongoose.Types.Long.fromString(itemUid) },
                { $set: data }
            );

            if (itemRes.acknowledged) {
                WriteOperateLog(req, `modify item ${JSON.stringify(req.body)}`);
                return res.status(200).json({ success: true });
            }

            return res.status(200).json({ success: false, msg: 'modify failed' });
        } catch (err) {
            next(err);
        }
    }
);

// @route  POST api/game/item/delete
// @desc   soft delete item
// @access Private
router.post(
    '/delete',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {
            if (!permiss.PermissOkAny(req.user.identity, [EPermiss.edit_item])) {
                return res.status(403).json({ success: false, msg: 'permission denied' });
            }

            const zoneId = normalizeInt(req.body.zoneId, 0);
            const itemUid = normalizeNumericText(req.body.itemUid);
            if (!zoneId || !itemUid) {
                return res.status(400).json({ success: false, msg: 'invalid params' });
            }

            const zoneMongoUrl = getZoneMongoUrl(zoneId);
            if (!zoneMongoUrl) {
                return res.status(400).json({ success: false, msg: 'invalid zone' });
            }

            const DBItemData = CreateDBItemData(zoneMongoUrl);
            const itemRes = await DBItemData.updateOne(
                { _id: mongoose.Types.Long.fromString(itemUid) },
                { $set: { IsDispose: Math.round(new Date() / 1000) } }
            );

            if (itemRes.acknowledged) {
                WriteOperateLog(req, `delete item ${JSON.stringify(req.body)}`);
                return res.status(200).json({ success: true });
            }

            return res.status(200).json({ success: false, msg: 'delete failed' });
        } catch (err) {
            next(err);
        }
    }
);

// @route  POST api/game/item/backpack/add
// @desc   add a new item into backpack
// @access Private
router.post(
    '/backpack/add',
    passport.authenticate('jwt', { session: false }),
    async (req, res, next) => {
        try {
            if (!permiss.PermissOkAny(req.user.identity, [EPermiss.edit_item])) {
                return res.status(403).json({ success: false, msg: 'permission denied' });
            }

            const zoneId = normalizeInt(req.body.zoneId, 0);
            const gameUserId = normalizeNumericText(req.body.gameUserId);
            const configId = normalizeInt(req.body.configId, 0);
            const createAttr = normalizeItemCreateAttr(req.body.createAttr);

            if (!zoneId || !gameUserId || !configId) {
                return res.status(400).json({ success: false, msg: 'invalid params' });
            }

            const zoneMongoUrl = getZoneMongoUrl(zoneId);
            if (!zoneMongoUrl) {
                return res.status(400).json({ success: false, msg: 'invalid zone' });
            }

            const userId = await resolveRoleUserId(zoneId, gameUserId, req.body.userId);
            if (!userId) {
                return res.status(200).json({ success: false, msg: 'role user id not found' });
            }

            const executeResult = await executeBackpackAdd(zoneId, userId, gameUserId, configId, createAttr);
            if (!executeResult.ok) {
                return res.status(200).json({ success: false, msg: executeResult.msg });
            }

            WriteOperateLog(req, `add backpack item ${JSON.stringify(req.body)}`);
            return res.status(200).json({ success: true, data: executeResult.data });
        } catch (err) {
            next(err);
        }
    }
);

module.exports = router;
