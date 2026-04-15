const fs = require('fs');
const path = require('path');

const NEWCONFIG_DIR = 'F:\\MUSF\\Server\\Config\\NewConfig';
const OUTPUT_FILE = 'F:\\MUSF\\Tools\\webgm\\client\\src\\config\\item_config.js';
const PETS_INFO_CONFIG_FILE = path.join(NEWCONFIG_DIR, 'Pets_InfoConfig.json');

const ITEM_CONFIG_FILES = [
    'Item_EquipmentConfig.json',
    'Item_WingConfig.json',
    'Item_NecklaceConfig.json',
    'Item_RingsConfig.json',
    'Item_BraceletConfig.json',
    'Item_DanglerConfig.json',
    'Item_GemstoneConfig.json',
    'Item_MountsConfig.json',
    'Item_FGemstoneConfig.json',
    'Item_SkillBooksConfig.json',
    'Item_GuardConfig.json',
    'Item_ConsumablesConfig.json',
    'Item_OtherConfig.json',
    'Item_FlagConfig.json',
    'Item_PetConfig.json',
    'Item_TaskConfig.json',
];

function readJsonArray(filePath) {
    return JSON.parse(fs.readFileSync(filePath, 'utf8'));
}

function buildPetInfoMap() {
    const rows = readJsonArray(PETS_INFO_CONFIG_FILE);
    const result = new Map();
    for (const row of rows) {
        const petId = normalizeInt(row.Id, 0);
        if (!petId) {
            continue;
        }
        result.set(petId, row);
    }
    return result;
}

function normalizeInt(value, fallback = 0) {
    const parsed = Number(value);
    return Number.isFinite(parsed) ? parsed : fallback;
}

function normalizeArray(value) {
    if (Array.isArray(value)) {
        return value.map(item => normalizeInt(item, item));
    }

    if (typeof value === 'string' && value.trim().length > 0) {
        try {
            const parsed = JSON.parse(value);
            return Array.isArray(parsed) ? parsed.map(item => normalizeInt(item, item)) : [];
        } catch {
            return [];
        }
    }

    return [];
}

function normalizeUseRole(value) {
    let parsed = value;
    if (typeof parsed === 'string' && parsed.trim().length > 0) {
        try {
            parsed = JSON.parse(parsed);
        } catch {
            parsed = {};
        }
    }

    if (parsed == null || typeof parsed !== 'object' || Array.isArray(parsed)) {
        parsed = {};
    }

    const useRole = { 0: 0 };
    for (let roleId = 1; roleId <= 13; roleId += 1) {
        const raw = parsed[String(roleId)];
        useRole[String(roleId)] = Number.isFinite(Number(raw)) ? Number(raw) : 0;
    }
    return useRole;
}

const PET_STAGE_ATTR_ID_MAP = new Map([
    [20105, 1],
    [20106, 2],
    [20107, 3],
]);

const PET_STAGE_BY_ITEM_ID = new Map([
    [350018, 1],
    [350019, 2],
    [350020, 3],
]);

const PET_DEFAULT_BASE_ATTR_BY_CONFIG_ID = new Map([
    [100, 620],
    [104, 621],
    [102, 622],
    [103, 623],
    [101, 624],
]);

function derivePetStage(itemId, appendLevel, baseAttrIds) {
    const stageByItemId = PET_STAGE_BY_ITEM_ID.get(itemId);
    if (stageByItemId != null) {
        return stageByItemId;
    }

    for (const attrId of baseAttrIds) {
        const stageByAttrId = PET_STAGE_ATTR_ID_MAP.get(attrId);
        if (stageByAttrId != null) {
            return stageByAttrId;
        }
    }

    if (!Number.isFinite(appendLevel) || appendLevel <= 0) {
        return 0;
    }
    if (appendLevel <= 7) {
        return 1;
    }
    if (appendLevel <= 9) {
        return 2;
    }
    return 3;
}

function normalizeItem(raw, petInfoMap) {
    const id = normalizeInt(raw.Id, 0);
    if (!id) {
        return null;
    }

    const derivedType = Math.floor(id / 10000);
    const rawType = normalizeInt(raw.Type, 0);
    const type = rawType > 0 ? rawType : derivedType;
    const petId = normalizeInt(raw.PetId, 0);
    const baseAttrIds = normalizeArray(raw.BaseAttrId);
    const petInfo = petInfoMap.get(petId) ?? null;
    const petAppendLevel = petInfo ? normalizeInt(petInfo.AppendLevel, 0) : 0;
    const petStage = type === 35 ? derivePetStage(id, petAppendLevel, baseAttrIds) : 0;
    return {
        Id: id,
        Name: String(raw.Name ?? ''),
        Type: type,
        Slot: normalizeInt(raw.Slot, 0),
        Skill: normalizeInt(raw.Skill, type === 35 ? 2 : 0),
        PetId: petId,
        PetAppendLevel: petAppendLevel,
        PetStage: petStage,
        X: normalizeInt(raw.X, 1),
        Y: normalizeInt(raw.Y, 1),
        StackSize: Math.max(1, normalizeInt(raw.StackSize, 1)),
        Level: normalizeInt(raw.Level, 0),
        QualityAttr: normalizeInt(raw.QualityAttr, 2),
        AppendAttrId: normalizeArray(raw.AppendAttrId),
        ExtraAttrId: normalizeArray(raw.ExtraAttrId),
        SpecialAttrId: normalizeArray(raw.SpecialAttrId),
        UseRole: normalizeUseRole(raw.UseRole),
    };
}

function buildSyntheticPetItem(petInfo) {
    const itemId = normalizeInt(petInfo.BeakId, 0);
    if (!itemId) {
        return null;
    }

    const petConfigId = normalizeInt(petInfo.Id, 0);
    const baseAttrId = PET_DEFAULT_BASE_ATTR_BY_CONFIG_ID.get(petConfigId);
    const baseAttrIds = baseAttrId != null ? [baseAttrId] : [];
    const petAppendLevel = normalizeInt(petInfo.AppendLevel, 0);

    return {
        Id: itemId,
        Name: String(petInfo.Name ?? ''),
        Type: Math.floor(itemId / 10000),
        Slot: 14,
        Skill: 2,
        PetId: petConfigId,
        PetAppendLevel: petAppendLevel,
        PetStage: derivePetStage(itemId, petAppendLevel, baseAttrIds),
        X: 2,
        Y: 2,
        StackSize: 1,
        Level: 0,
        QualityAttr: 12,
        AppendAttrId: [],
        ExtraAttrId: [],
        SpecialAttrId: [],
        UseRole: normalizeUseRole({}),
    };
}

function ensureSyntheticPetItems(itemMap, petInfoMap) {
    for (const petInfo of petInfoMap.values()) {
        const itemId = normalizeInt(petInfo.BeakId, 0);
        if (!itemId) {
            continue;
        }

        const itemKey = String(itemId);
        if (itemMap[itemKey] != null) {
            continue;
        }

        const syntheticItem = buildSyntheticPetItem(petInfo);
        if (syntheticItem != null) {
            itemMap[itemKey] = syntheticItem;
        }
    }
}

function buildItemMap() {
    const itemMap = {};
    const petInfoMap = buildPetInfoMap();

    for (const fileName of ITEM_CONFIG_FILES) {
        const filePath = path.join(NEWCONFIG_DIR, fileName);
        const rows = readJsonArray(filePath);

        for (const row of rows) {
            const item = normalizeItem(row, petInfoMap);
            if (!item) {
                continue;
            }

            itemMap[String(item.Id)] = item;
        }
    }

    ensureSyntheticPetItems(itemMap, petInfoMap);

    const sortedEntries = Object.entries(itemMap).sort((a, b) => Number(a[0]) - Number(b[0]));
    return Object.fromEntries(sortedEntries);
}

function main() {
    const itemMap = buildItemMap();
    const output = `module.exports = ${JSON.stringify(itemMap, null, 2)};\n`;
    fs.writeFileSync(OUTPUT_FILE, output, 'utf8');
    console.log(`Synced ${Object.keys(itemMap).length} items to ${OUTPUT_FILE}`);
}

main();
