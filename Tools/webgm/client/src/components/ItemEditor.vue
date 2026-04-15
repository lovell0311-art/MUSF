<template>
    <el-drawer class="item-editor" v-model="drawer.value" direction="rtl">
        <template #header>
        <h4 v-if="IsEdit">编辑物品({{ form.Item.Uid }})</h4>
        <h4 v-else="IsEdit">添加物品</h4>
        </template>
        <template #default>
            <el-divider content-position="left">物品</el-divider>
            <el-form :model="form" label-width="100px">
                <el-form-item label="使用职业">
                    <el-select v-model="form.SelectedRoleType" class="m-2" placeholder="Select" @change="OnSelectChangeRefreshItemOptions" :disabled="IsEdit">
                    <el-option
                    v-for="item in RoleType"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                    />
                    </el-select>
                </el-form-item>
                <el-form-item label="装备部位">
                    <el-select v-model="form.SelectedSlotId" class="m-2" placeholder="Select"  @change="OnSelectChangeRefreshItemOptions" :disabled="IsEdit">
                    <el-option
                    v-for="item in SlotOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                    />
                    </el-select>
                </el-form-item>
                <el-form-item v-if="form.SelectedSlotId === 14" label="宠物阶段">
                    <el-select v-model="form.SelectedPetStage" class="m-2" placeholder="Select" @change="OnSelectChangeRefreshItemOptions" :disabled="IsEdit">
                    <el-option
                    v-for="item in PetStageOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                    />
                    </el-select>
                </el-form-item>
                <el-form-item label="过滤">
                <el-input v-model="form.ItemNameFilter" class="m-2 w-214" placeholder="关键词" @input="OnBlurRefreshItemOptions" :disabled="IsEdit">
                    <template #prefix>
                    <el-icon class="el-input__icon"><search /></el-icon>
                    </template>
                </el-input>
                </el-form-item>
                <el-form-item label="物品名">
                    <el-select-v2
                    v-model="form.SelectedItemOptionKey"
                    :options="ItemOptions"
                    placeholder="没选择物品"
                    @change="OnSelectItemChangeRefresh"
                    @keyup.enter="OnSelectItemChangeRefresh"
                    :disabled="IsEdit"
                    />
                </el-form-item>
                <el-divider content-position="left">自定义属性</el-divider>
                <el-form-item label="自定义属性">
                    <el-select multiple v-model="form.Item.CustomAttrMethod" class="m-2" placeholder="Select" :disabled="DisabledCustomAttrMethod()">
                    <el-option
                    v-for="item in CustomAttrMethodOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                    />
                    </el-select>
                </el-form-item>
                <el-divider content-position="left">附加属性</el-divider>
                <el-form-item label="强化等级">
                    <el-input-number v-model="form.Item.Level" :min="0" :max="15" controls-position="right" />
                </el-form-item>
                <el-form-item label="锻造等级">
                    <el-input-number v-model="form.Item.ForgeLevel" :min="0" :max="15" controls-position="right" />
                </el-form-item>
                <el-form-item label="物品数量">
                    <el-input-number v-model="form.Item.Quantity" :min="1" :max="form.config.StackSize" controls-position="right" />
                </el-form-item>
                <el-form-item label="套装">
                    <el-select v-model="form.Item.SetId" class="m-2" placeholder="没选择套装" :disabled="DisabledSet()">
                    <el-option
                    v-for="item in ItemSetOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                    />
                    </el-select>
                </el-form-item>
                <el-form-item label="追加">
                    <el-select v-model="form.Item.AppendEntryId" class="m-2" placeholder="Select">
                    <el-option
                    v-for="item in ItemAppendOptions"
                    :key="item.value"
                    :label="item.label"
                    :value="item.value"
                    />
                    </el-select>
                    <el-input-number v-model="form.Item.AppendLevel" :min="0" :max="4" style="width:100px" controls-position="right" :disabled="form.Item.AppendEntryId == 0"/>
                </el-form-item>
                <el-form-item v-if="Item != null" label="到期时间">
                    <el-date-picker
                        v-model="form.Item.ValidTime.Date"
                        type="datetime"
                        placeholder="无时限道具"
                    />
                </el-form-item>
                <el-form-item v-else label="有效时间">
                    <el-space :size="4">
                        <el-input-number v-model="form.Item.ValidTime.Day" size="small" style="width:64px" :min="0" :max="3650" :controls="false"/>
                        <el-text class="mx-1">天</el-text>
                        <el-input-number v-model="form.Item.ValidTime.Hour" size="small" style="width:64px" :min="0" :max="24" :controls="false"/>
                        <el-text class="mx-1">时</el-text>
                        <el-input-number v-model="form.Item.ValidTime.Minute" size="small" style="width:64px" :min="0" :max="60" :controls="false"/>
                        <el-text class="mx-1">分</el-text>
                    </el-space>
                </el-form-item>
                <el-form-item label="属性">
                    <el-checkbox-group v-model="form.Item.option1">
                        <el-checkbox :label="13" name="type" :disabled="form.disabled.Skill" >技能</el-checkbox>
                        <el-checkbox :label="51" name="type" :disabled="form.disabled.Lucky" >幸运</el-checkbox>
                    </el-checkbox-group>
                </el-form-item>
                <el-form-item label="物品类型">
                    <el-radio-group v-model="form.Item.option2">
                        <el-radio :label="0" >普通</el-radio>
                        <el-radio :label="14001" >绑定账号</el-radio>
                        <el-radio :label="14002" >绑定角色</el-radio>
                        <el-radio :label="15" >任务</el-radio>
                    </el-radio-group>
                </el-form-item>

                <el-tabs type="border-card" >
                    <el-tab-pane label="卓越" >
                        <!-- 卓越 -->
                        <el-table
                            ref="ExcMultipleTableRef"
                            :data="ExcEntryTableData"
                            style="width: 100%"
                            @selection-change="OnHandleExcellentSelectionChange"
                            :show-header="false"
                            border
                        >
                            <el-table-column type="selection" width="55" :selectable="CheckboxExcellent"/>
                            <el-table-column property="Name" label="Name"/>
                        </el-table>
                        <!-- 卓越 End -->
                    </el-tab-pane>
                    <el-tab-pane label="再生">Config</el-tab-pane>
                    <el-tab-pane label="镶嵌">
                        <el-form-item label="卡槽1">
                            <el-select v-model="form.Item.FluoreSlot1" class="m-2" placeholder="" :disabled="DisabledSocket()">
                            <el-option
                            v-for="item in ItemSocketOptions"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value"
                            />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="卡槽2">
                            <el-select v-model="form.Item.FluoreSlot2" class="m-2" placeholder="" :disabled="DisabledSocket() || form.Item.FluoreSlot1 == -1">
                            <el-option
                            v-for="item in ItemSocketOptions"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value"
                            />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="卡槽3">
                            <el-select v-model="form.Item.FluoreSlot3" class="m-2" placeholder=""
                            :disabled="
                            DisabledSocket() ||
                            form.Item.FluoreSlot1 == -1 ||
                            form.Item.FluoreSlot2 == -1">
                            <el-option
                            v-for="item in ItemSocketOptions"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value"
                            />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="卡槽4">
                            <el-select v-model="form.Item.FluoreSlot4" class="m-2" placeholder=""
                            :disabled="
                            Boolean(DisabledSocket() ||
                            form.Item.FluoreSlot1 == -1 ||
                            form.Item.FluoreSlot2 == -1 ||
                            form.Item.FluoreSlot3 == -1)">
                            <el-option
                            v-for="item in ItemSocketOptions"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value"
                            />
                            </el-select>
                        </el-form-item>
                        <el-form-item label="卡槽5">
                            <el-select v-model="form.Item.FluoreSlot5" class="m-2" placeholder=""
                            :disabled="
                            DisabledSocket() ||
                            form.Item.FluoreSlot1 == -1 ||
                            form.Item.FluoreSlot2 == -1 ||
                            form.Item.FluoreSlot3 == -1 ||
                            form.Item.FluoreSlot4 == -1">
                            <el-option
                            v-for="item in ItemSocketOptions"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value"
                            />
                            </el-select>
                        </el-form-item>
                    </el-tab-pane>
                    <el-tab-pane label="特殊(翅膀)" >
                        <!-- 翅膀特殊词条 -->
                        <el-table
                            ref="SpecialEntryMultipleTableRef"
                            :data="SpecialEntryTableData"
                            style="width: 100%"
                            @selection-change="OnHandleSpecialEntrySelectionChange"
                            :show-header="false"
                            border
                        >
                            <el-table-column type="selection" width="55"/>
                            <el-table-column property="Name" label="Name">
                                <template #default="scope">
                                    {{SpecialEntryTableName(scope)}}
                                </template>
                            </el-table-column>
                            <el-table-column
                                align='center'
                                fixed="right"
                                label="操作"
                                width="100">
                                <template #default="scope">
                                    <el-button-group>
                                        <el-button type="primary" size="small" @click="OnClick_SpecialEntry_Reduce(scope)">
                                            -
                                        </el-button>
                                        <el-button type="primary" size="small" @click="OnClick_SpecialEntry_Add(scope)">
                                            +
                                        </el-button>
                                    </el-button-group>
                                </template>
                            </el-table-column>
                        </el-table>
                        <!-- 翅膀特殊词条 End -->
                    </el-tab-pane>
                </el-tabs>
            </el-form>
        </template>
        <template #footer>

        <div style="flex: auto" v-if="IsEdit">
            <el-button @click="drawer.value=false">取消</el-button>
            <el-button type="primary" v-on:click="$emit('modify-click')">修改</el-button>
        </div>
        <div style="flex: auto" v-else="IsEdit">
            <el-button @click="drawer.value=false">取消</el-button>
            <el-button type="primary" v-on:click="$emit('add-click')">添加</el-button>
        </div>

        </template>
    </el-drawer>
</template>

<script>
import { ref } from 'vue';
import {Item,EItemType,EQualityType,EDBItemValue,ItemCreateAttr} from '@/game/item';
import { Select } from '@element-plus/icons-vue';
import item_config from '@/config/item_config';
import itemset_config from '@/config/itemset_config';
import attrentry_config from '@/config/attrentry_config';
import attrsocket_config from '@/config/attrsocket_config';

export default {
    props:{
        drawer: {value:false},
        drawer2: {type:String},
        IsEdit:{value:false},   /**编辑模式 */
    },
    setup(){

    },
    data(){
        return {
            form:{
                Item: {
                    Uid: '',    /* 物品Uid */
                    ConfigId: 0,
                    Level: 0,
                    ForgeLevel:0,
                    Quantity: 1,
                    SetId: 0,    /* 套装id number */
                    option1: [],    /* 技能 幸运 */
                    option2: 0,    /* 普通 绑定账号 绑定角色 任务 */
                    AppendEntryId: 0, /* 追加词条id number */
                    AppendLevel: 0, /* 追加等级 */
                    ExcellentEntryId: [],   /* 卓越词条id */
                    SpecialEntry:{},  /* 特殊词条id { Id:Level } */
                    ValidTime:{ /* 有效时间 { Id:Level } */
                        Day: 0,
                        Hour: 0,
                        Minute: 0,
                        Date: Date()
                    },
                    FluoreSlot1: -1,    /* 镶嵌宝石 */
                    FluoreSlot2: -1,
                    FluoreSlot3: -1,
                    FluoreSlot4: -1,
                    FluoreSlot5: -1,
                    FluoreAttr: 0,  /* 荧光宝石属性 */
                    CustomAttrMethod: [],   /* 自定义属性方法 */
                },
                SelectedRoleType: 0,
                SelectedSlotId: -1,
                SelectedPetStage: 0,
                SelectedItemOptionKey: null,
                ItemNameFilter: '',
                config:{
                    StackSize: 255,   // 单组最大数量
                },
                disabled:{
                    Skill:true,
                    Lucky:true,
                    Excellent: true,
                    Set:true,
                    Socket:true,
                    CustomAttrMethod:false,
                }
            },
            RoleType:[
                {
                    value: 0,
                    label: "所有职业"
                },
                {
                    value: 1,
                    label: "魔法师"
                },
                {
                    value: 2,
                    label: "剑士"
                },
                {
                    value: 3,
                    label: "弓箭手"
                },
                {
                    value: 4,
                    label: "魔剑士"
                },
                {
                    value: 5,
                    label: "圣导师"
                },
                {
                    value: 6,
                    label: "召唤术士"
                },
                {
                    value: 7,
                    label: "格斗家"
                },
                {
                    value: 8,
                    label: "梦幻骑士"
                },
                {
                    value: 9,
                    label: "符文法师"
                },
                {
                    value: 10,
                    label: "疾风"
                },
                {
                    value: 11,
                    label: "枪手"
                },
                {
                    value: 12,
                    label: "白魔"
                },
                {
                    value: 13,
                    label: "女魔"
                },
            ],
            SlotOptions:[
                {
                    value: -1,
                    label: "所有部位"
                },
                {
                    value: 1,
                    label: "武器"
                },
                {
                    value: 2,
                    label: "盾牌"
                },
                {
                    value: 3,
                    label: "头盔"
                },
                {
                    value: 4,
                    label: "铠甲"
                },
                {
                    value: 5,
                    label: "护腿"
                },
                {
                    value: 6,
                    label: "护手"
                },
                {
                    value: 7,
                    label: "靴子"
                },
                {
                    value: 8,
                    label: "翅膀"
                },
                {
                    value: 9,
                    label: "守护"
                },
                {
                    value: 10,
                    label: "项链"
                },
                {
                    value: 11,
                    label: "左戒指"
                },
                {
                    value: 12,
                    label: "右戒指"
                },
                {
                    value: 13,
                    label: "旗帜"
                },
                {
                    value: 14,
                    label: "宠物"
                },
                {
                    value: 15,
                    label: "手环"
                },
                {
                    value: 0,
                    label: "其他物品"
                },
            ],
            CustomAttrMethodOptions: /* 自定义属性方法 */
            [],
            PetStageOptions:[
                {
                    value: 0,
                    label: "全部阶段"
                },
                {
                    value: 1,
                    label: "一阶"
                },
                {
                    value: 2,
                    label: "二阶"
                },
                {
                    value: 3,
                    label: "三阶"
                },
            ],
            ItemOptions:[],
            ItemSetOptions:[],  /* 套装列表 */
            ItemAppendOptions:[],   /* 追加列表 */
            ExcEntryTableData:[],   /* 卓越词条列表 */
            ItemSocketOptions:[],   /* 镶嵌 */
            SpecialEntryTableData:[],  /* 特殊词条列表 */
            Item: null,
            ItemCreateAttr: null,
        }
    },
    methods:{
        BuildItemOptionValue(configId, petStage = 0)
        {
            if(petStage > 0)
            {
                return `pet:${configId}:${petStage}`;
            }
            return `item:${configId}`;
        },
        ParseItemOptionValue(optionValue)
        {
            const text = String(optionValue ?? '').trim();
            if(text === '')
            {
                return { configId: 0, petStage: 0 };
            }

            const parts = text.split(':');
            if(parts[0] === 'pet' && parts.length >= 3)
            {
                return {
                    configId: Number(parts[1]) || 0,
                    petStage: Number(parts[2]) || 0,
                };
            }
            if(parts[0] === 'item' && parts.length >= 2)
            {
                return {
                    configId: Number(parts[1]) || 0,
                    petStage: 0,
                };
            }

            return {
                configId: Number(text) || 0,
                petStage: 0,
            };
        },
        ApplySelectedItemOption()
        {
            const parsed = this.ParseItemOptionValue(this.form.SelectedItemOptionKey);
            this.form.Item.ConfigId = parsed.configId > 0 ? parsed.configId : null;
        },
        SyncSelectedItemOptionFromConfig()
        {
            const config = item_config[String(this.form.Item.ConfigId)];
            if(config == null)
            {
                this.form.SelectedItemOptionKey = null;
                return;
            }

            let petStage = 0;
            if(config.Slot == 14)
            {
                petStage = this.form.SelectedPetStage > 0
                    ? this.form.SelectedPetStage
                    : (config.PetStage > 0 ? config.PetStage : 1);
            }

            this.form.SelectedItemOptionKey = this.BuildItemOptionValue(config.Id, petStage);
        },
        GetPetStageLabel(stage)
        {
            switch(stage)
            {
                case 1:
                    return '一阶';
                case 2:
                    return '二阶';
                case 3:
                    return '三阶';
                default:
                    return '';
            }
        },
        GetItemOptionLabel(value, petStage = 0)
        {
            if(value == null)
            {
                return '';
            }
            const displayPetStage = petStage > 0 ? petStage : value.PetStage;
            if(value.Slot == 14 && displayPetStage > 0)
            {
                return `[${this.GetPetStageLabel(displayPetStage)}] ${value.Name}`;
            }
            return value.Name;
        },
        RefreshtableItemOptions(){
            // 刷新物品列表
            this.ItemOptions = [];
                console.log(`this.form.SelectedSlotId=${this.form.SelectedSlotId} this.form.SelectedRoleType=${this.form.SelectedRoleType}`);
            for(const [key,value] of Object.entries(item_config))
            {
                if(this.form.SelectedSlotId != -1 && value.Slot != this.form.SelectedSlotId)
                {
                    continue;
                }
                if(value.UseRole[String(this.form.SelectedRoleType)] < 0)
                {
                    continue;
                }
                if(this.form.ItemNameFilter != '' && value.Name.indexOf(this.form.ItemNameFilter) < 0)
                {
                    continue;
                }
                if(this.form.SelectedSlotId == 14 && value.Slot == 14)
                {
                    for(let petStage = 1; petStage <= 3; petStage += 1)
                    {
                        if(this.form.SelectedPetStage > 0 && petStage != this.form.SelectedPetStage)
                        {
                            continue;
                        }

                        this.ItemOptions.push({
                            value: this.BuildItemOptionValue(value.Id, petStage),
                            label: this.GetItemOptionLabel(value, petStage),
                        });
                    }
                    continue;
                }

                this.ItemOptions.push({
                    value: this.BuildItemOptionValue(value.Id),
                    label: this.GetItemOptionLabel(value),
                });
            }
        },
        SelectFirstItemOption()
        {
            // 选择物品列表第一个
            if(this.ItemOptions.length <= 0)
            {
                this.form.Item.ConfigId = null;
                this.form.SelectedItemOptionKey = null;
                return;
            }

            if(this.form.SelectedItemOptionKey != null)
            {
                const existsCurrentOption = this.ItemOptions.find((item)=>item.value === this.form.SelectedItemOptionKey);
                if(existsCurrentOption != null)
                {
                    this.ApplySelectedItemOption();
                    return;
                }
            }

            if(this.form.Item.ConfigId != null)
            {
                const preferredStage = this.form.SelectedSlotId == 14
                    ? (this.form.SelectedPetStage > 0
                        ? this.form.SelectedPetStage
                        : (this.ParseItemOptionValue(this.form.SelectedItemOptionKey).petStage || 1))
                    : 0;
                const preferredOptionValue = this.BuildItemOptionValue(this.form.Item.ConfigId, preferredStage);
                const preferredOption = this.ItemOptions.find((item)=>item.value === preferredOptionValue);
                if(preferredOption != null)
                {
                    this.form.SelectedItemOptionKey = preferredOption.value;
                    this.ApplySelectedItemOption();
                    return;
                }

                const fallbackByConfigId = this.ItemOptions.find((item)=>this.ParseItemOptionValue(item.value).configId === this.form.Item.ConfigId);
                if(fallbackByConfigId != null)
                {
                    this.form.SelectedItemOptionKey = fallbackByConfigId.value;
                    this.ApplySelectedItemOption();
                    return;
                }
            }

            this.form.SelectedItemOptionKey = this.ItemOptions[0].value;
            this.ApplySelectedItemOption();
        },
        OnSelectChangeRefreshItemOptions()
        {
            this.RefreshtableItemOptions();
            this.SelectFirstItemOption();
            this.OnSelectItemChangeRefresh();
        },
        OnBlurRefreshItemOptions()
        {
            this.RefreshtableItemOptions();
            this.SelectFirstItemOption();
            this.OnSelectItemChangeRefresh();
        },
        OnSelectItemChangeRefresh() /** 选择的物品变了，刷新一些属性 */
        {
            this.ApplySelectedItemOption();
            this.SelectedRoleType = 0;
            this.SelectedSlotId = -1;
            this.ItemNameFilter = '';
            this.RefreshItemSetOptions();
            this.RefreshItemAppendOptions();
            this.RefreshExcellentOptions();
            this.RefreshSocketOptions();
            this.RefreshSpecialEntryOptions();
            this.RefreshCustomAttrMethodOptions();

            this.RefreshSelectedItemRestriction();
        },
        OnHandleExcellentSelectionChange(val)    /** 卓越词条选项变动 */
        {
            this.form.Item.ExcellentEntryId = [];
            this.form.Item.ExcellentEntryId = val.map((item,index)=>{
                return item.Id;
            });
            console.log(JSON.stringify(this.form.Item));
        },
        OnHandleSpecialEntrySelectionChange(val)    /** 特殊(翅膀)词条选项变动 */
        {
            this.form.Item.SpecialEntry = {};
            val.forEach((item,index)=>{
                this.form.Item.SpecialEntry[item.Id] = item.Level;
            });
            console.log(JSON.stringify(this.form.Item));
        },

        RefreshItemSetOptions()
        {
            // 刷新套装列表
            this.ItemSetOptions = [];
            if(this.form.Item.ConfigId == null)return;
            this.ItemSetOptions.push({
                value:0,
                label: " - "
            })
            for(const [key,value] of Object.entries(itemset_config))
            {
                if(value.ItemsId.indexOf(this.form.Item.ConfigId)<0)
                {
                    continue;
                }
                this.ItemSetOptions.push({
                    value: value.Id,
                    label: value.SetName,
                });
            }
            this.form.Item.SetId = 0;
        },
        RefreshItemAppendOptions()
        {
            // 刷新追加列表
            this.ItemAppendOptions = [];
            if(this.form.Item.ConfigId == null)return;
            this.ItemAppendOptions = item_config[String(this.form.Item.ConfigId)].AppendAttrId.map((item,index)=>{
                return {
                    value: item,
                    label: attrentry_config[800000 + item] != null ? attrentry_config[800000 + item].Name : `[${item}] Unknown`,
                };
            });
            this.ItemAppendOptions.unshift({
                value: 0,
                label: " - ",
            })
            if(this.ItemAppendOptions.length > 1)
            {
                this.form.Item.AppendEntryId = this.ItemAppendOptions[1].value;
            }else{
                this.form.Item.AppendEntryId = 0;
            }
            this.form.Item.AppendLevel = 0;
        },
        RefreshCustomAttrMethodOptions()
        {
          this.form.Item.CustomAttrMethod = [];
            if(this.form.Item.ConfigId == null)return;
          var QualityType = item_config[String(this.form.Item.ConfigId)].QualityAttr != null ? item_config[String(this.form.Item.ConfigId)].QualityAttr : EQualityType.Normal;
          this.CustomAttrMethodOptions = [];
          if((QualityType & EQualityType.Excellent) == EQualityType.Excellent)
          {
            // 可以添加卓越
            this.CustomAttrMethodOptions.push({
                value: "ItemAddExcAttr",
                label: "添加卓越属性(默认概率)",
            });
            this.CustomAttrMethodOptions.push({
                value: "ItemRandAddExcAttr_3",
                label: "添加3条卓越属性"
            });
            this.CustomAttrMethodOptions.push({
                value: "ItemRandAddExcAttr_3_6",
                label: "添加3-6条卓越属性"
            });

          }
          if((QualityType & EQualityType.Purple) == EQualityType.Purple)
          {
            // 可以添加橙装
            this.CustomAttrMethodOptions.push({
                value: "ItemAddPurple",
                label: "添加橙光属性(默认概率)",
            });
            this.CustomAttrMethodOptions.push({
                value: "ItemAddPurple_3_6",
                label: "添加3-6条橙光属性"
            });
          }
          if((QualityType & EQualityType.Set) == EQualityType.Set)
          {
            // 可以添加套装
            this.CustomAttrMethodOptions.push({
                value: "ItemTryRandSetId",
                label: "尝试随机添加套装Id",
            });
          }
          if((QualityType & EQualityType.Socket) == EQualityType.Socket)
          {
            // 可以添加镶嵌
            this.CustomAttrMethodOptions.push({
                value: "ItemTryAddSocketHoleCount",
                label: "尝试随机镶嵌孔洞数(默认概率)",
            });
            this.CustomAttrMethodOptions.push({
                value: "ItemAddSocketHoleCountFull",
                label: "添加镶嵌孔洞数(满洞)",
            });
          }

        },
        RefreshExcellentOptions()
        {
            // 刷新卓越列表
            this.ExcEntryTableData = [];
            if(this.form.Item.ConfigId == null)return;

            var selfEntryType = 1;

            switch(Math.floor(this.form.Item.ConfigId / 10000))
            {
                case EItemType.Swords:// 武器
                case EItemType.Axes:
                case EItemType.Maces:
                case EItemType.Bows:
                case EItemType.Crossbows:
                case EItemType.Spears:
                case EItemType.Staffs:
                case EItemType.MagicBook:
                case EItemType.Scepter:
                case EItemType.RuneWand:
                case EItemType.FistBlade:
                case EItemType.MagicSword:
                case EItemType.ShortSword:
                case EItemType.MagicGun:

                case EItemType.Necklace:// 项链
                    selfEntryType = 1;
                break;
                case EItemType.Shields: // 防具
                case EItemType.Helms:
                case EItemType.Armors:
                case EItemType.Pants:
                case EItemType.Gloves:
                case EItemType.Boots:

                case EItemType.Rings:// 戒指
                case EItemType.Flag:// 旗帜
                case EItemType.Bracelet:// 手环
                case EItemType.Guard:// 守护
                    selfEntryType = 2;
                break;

                case EItemType.Pet:// 宠物
                    selfEntryType = 0;
                break;
                default:
                    return;
                    break;
            }

            var QualityType = item_config[String(this.form.Item.ConfigId)].QualityAttr != null ? item_config[String(this.form.Item.ConfigId)].QualityAttr : EQualityType.Normal;
            if((QualityType & EQualityType.Excellent) != EQualityType.Excellent)
            {
                // 不正常卓越
                return;
            }

            console.log(`ConfigId=${this.form.Item.ConfigId} selfEntryType = ${selfEntryType}`);
            for(const [key,value] of Object.entries(attrentry_config))
            {
                if(selfEntryType == 0)
                {
                    if(value.EntryType != 1 && value.EntryType != 2)
                    {
                        continue;
                    }
                }else if(value.EntryType != selfEntryType)
                {
                    continue;
                }

                this.ExcEntryTableData.push({
                    Id: value.Id,
                    Name: value.Name,
                });
            }
        },
        RefreshSocketOptions()
        {
            // 刷新镶嵌列表
            this.ItemSocketOptions = [];
            this.ItemSocketOptions.push({
                value: -1,
                label: "未开孔"
            });
            this.ItemSocketOptions.push({
                value: 0,
                label: "开孔"
            });
            for(var level = 0;level < 5;level++)
            {
                for(const [key,value] of Object.entries(attrsocket_config))
                {
                    var name = value.Info.replace("{0:G}",String(value.Level[level]/10000)).replace("{0:P}",`${String(value.Level[level]/100)}%`);
                    switch(value.Fluore)
                    {
                        case 270008:
                            name = `[火] lv.${level} ${name}`;
                            break;
                        case 270009:
                            name = `[水] lv.${level} ${name}`;
                            break;
                        case 270010:
                            name = `[冰] lv.${level} ${name}`;
                            break;
                        case 270011:
                            name = `[风] lv.${level} ${name}`;
                            break;
                        case 270012:
                            name = `[雷] lv.${level} ${name}`;
                            break;
                        case 270013:
                            name = `[土] lv.${level} ${name}`;
                            break;
                        default:
                            name = `[空] lv.${level} ${name}`;
                            break;
                    }
                    this.ItemSocketOptions.push({
                        value: value.Id * 100 + level,
                        label: name,
                    });
                }
            }

            this.form.Item.FluoreSlot1 = -1;
            this.form.Item.FluoreSlot2 = -1;
            this.form.Item.FluoreSlot3 = -1;
            this.form.Item.FluoreSlot4 = -1;
            this.form.Item.FluoreSlot5 = -1;
        },
        RefreshSpecialEntryOptions()
        {
            this.SpecialEntryTableData = [];
            if(this.form.Item.ConfigId == null)return;
            const conf = item_config[String(this.form.Item.ConfigId)];

            if(conf.SpecialAttrId.length == 0)return;
            for(const [key,value] of Object.entries(conf.SpecialAttrId))
            {
                const entryConf = attrentry_config[String(value)];
                if(entryConf == null)continue;
                this.SpecialEntryTableData.push({
                    Id: entryConf.Id,
                    Name: entryConf.Name,
                    Level: 0,
                });
            }


        },
        RefreshSelectedItemRestriction()   /** 刷新选择的物品属性限制 */
        {
            if(this.form.Item.ConfigId == null)
            {
              this.form.disabled.Skill = false;
              this.form.disabled.Lucky = false;
              this.form.disabled.Excellent = false;
              this.form.disabled.Set = false;
              this.form.disabled.Socket = false;
              return;
            }
            var QualityType = item_config[String(this.form.Item.ConfigId)].QualityAttr != null ? item_config[String(this.form.Item.ConfigId)].QualityAttr : EQualityType.Normal;
            if((QualityType & EQualityType.Skill) == EQualityType.Skill)
            {
                // 有技能
                this.form.disabled.Skill = false;
            }else{
                this.form.disabled.Skill = true;
            }
            if((QualityType & EQualityType.Lucky) == EQualityType.Lucky)
            {
                // 幸运
                this.form.disabled.Lucky = false;
            }else{
                this.form.disabled.Lucky = true;
            }
            if((QualityType & EQualityType.Excellent) == EQualityType.Excellent)
            {
                // 卓越
                this.form.disabled.Excellent = false;
            }else{
                this.form.disabled.Excellent = true;
            }
            if((QualityType & EQualityType.Set) == EQualityType.Set)
            {
                // 套装
                this.form.disabled.Set = false;
            }else{
                this.form.disabled.Set = true;
            }
            if((QualityType & EQualityType.Socket) == EQualityType.Socket)
            {
                // 镶嵌
                this.form.disabled.Socket = false;
            }else{
                this.form.disabled.Socket = true;
            }


            // 数值限制
            this.form.config.StackSize = item_config[String(this.form.Item.ConfigId)].StackSize;
            this.form.Item.Quantity = 1;

        },
        /**OnClick */
        OnClick_SpecialEntry_Reduce(scope)
        {
            let level = this.SpecialEntryTableData[scope.$index].Level;
            level -= 1;
            if(level < 0) level = 0;
            this.SpecialEntryTableData[scope.$index].Level = level;

            if(this.form.Item.SpecialEntry[this.SpecialEntryTableData[scope.$index].Id] != null)
            {
                this.form.Item.SpecialEntry[this.SpecialEntryTableData[scope.$index].Id] = level;
            }
        },
        OnClick_SpecialEntry_Add(scope)
        {
            let level = this.SpecialEntryTableData[scope.$index].Level;
            level += 1;
            if(level > 3) level = 3;
            this.SpecialEntryTableData[scope.$index].Level = level;

            if(this.form.Item.SpecialEntry[this.SpecialEntryTableData[scope.$index].Id] != null)
            {
                this.form.Item.SpecialEntry[this.SpecialEntryTableData[scope.$index].Id] = level;
            }
        },

        /**ToString */
        SpecialEntryTableName(scope)
        {
            const entryConf = attrentry_config[String(scope.row.Id)];
            return `[lv.${scope.row.Level}]`+scope.row.Name.replace("{0:G}",entryConf.Value[scope.row.Level]);
        },


        /**Disabled */
        DisabledSet()
        {
            return this.form.disabled.Set;
            //return this.form.disabled.Set || this.form.Item.ExcellentEntryId.length != 0 || this.form.Item.FluoreSlot1 != -1;
        },
        DisabledExcellent()
        {
            return this.form.disabled.Excellent;
            //return this.form.disabled.Excellent || this.form.Item.SetId != 0 || this.form.Item.FluoreSlot1 != -1;
        },
        DisabledSocket()
        {
            return this.form.disabled.Socket;
            //return this.form.disabled.Socket || this.form.Item.ExcellentEntryId.length != 0 || this.form.Item.SetId != 0;
        },
        DisabledCustomAttrMethod()
        {
            return this.form.disabled.CustomAttrMethod || this.IsEdit;
        },
        CheckboxExcellent(row,index)
        {
            return !this.DisabledExcellent();
        },

        /**Item to form */
        Item2Form(item)
        {
            this.Item = item;
            this.form.Item.Uid = this.Item.Uid;
            this.form.Item.ConfigId = this.Item.Config.Id;
            this.SyncSelectedItemOptionFromConfig();
            this.OnSelectItemChangeRefresh();
            this.form.Item.Level = this.Item.GetProp(EDBItemValue.Level);
            this.form.Item.Quantity = this.Item.GetProp(EDBItemValue.Quantity);
            this.form.Item.SetId = this.Item.GetProp(EDBItemValue.SetId);
            this.form.Item.AppendEntryId = this.Item.GetProp(EDBItemValue.OptValue);
            this.form.Item.AppendLevel = this.Item.GetProp(EDBItemValue.OptLevel);
            this.form.Item.FluoreAttr = this.Item.GetProp(EDBItemValue.FluoreAttr);
            // 镶嵌
            if(this.Item.GetProp(EDBItemValue.FluoreSlotCount) >= 1)
                this.form.Item.FluoreSlot1 = this.Item.GetProp(EDBItemValue.FluoreSlot1);
            if(this.Item.GetProp(EDBItemValue.FluoreSlotCount) >= 2)
                this.form.Item.FluoreSlot2 = this.Item.GetProp(EDBItemValue.FluoreSlot2);
            if(this.Item.GetProp(EDBItemValue.FluoreSlotCount) >= 3)
                this.form.Item.FluoreSlot3 = this.Item.GetProp(EDBItemValue.FluoreSlot3);
            if(this.Item.GetProp(EDBItemValue.FluoreSlotCount) >= 4)
                this.form.Item.FluoreSlot4 = this.Item.GetProp(EDBItemValue.FluoreSlot4);
            if(this.Item.GetProp(EDBItemValue.FluoreSlotCount) >= 5)
                this.form.Item.FluoreSlot5 = this.Item.GetProp(EDBItemValue.FluoreSlot5);
            // 技能，幸运
            if(this.Item.GetProp(EDBItemValue.SkillId) > 0) // 技能
                this.form.Item.option1.push(EDBItemValue.SkillId);
            if(this.Item.GetProp(EDBItemValue.LuckyEquip) > 0)   // 幸运
                this.form.Item.option1.push(EDBItemValue.LuckyEquip);
            else
                this.form.Item.option1 = [];

            // 普通，绑定账号，绑定角色，任务
            if(this.Item.GetProp(EDBItemValue.IsBind) == 1)
                this.form.Item.option2 = EDBItemValue.IsBind * 1000 + 1;
            else if(this.Item.GetProp(EDBItemValue.IsBind) == 2)
                this.form.Item.option2 = EDBItemValue.IsBind * 1000 + 2;
            else if(this.Item.GetProp(EDBItemValue.IsTask) > 0)
                this.form.Item.option2 = EDBItemValue.IsTask;
            else
                this.form.Item.option2 = 0;

            // 设置有效时间
            if(this.Item.GetProp(EDBItemValue.ValidTime) > 0)
            {
                this.form.Item.ValidTime.Date = new Date();
                this.form.Item.ValidTime.Date.setTime(this.Item.GetProp(EDBItemValue.ValidTime) * 1000);
            }
            else
            {
                this.form.Item.ValidTime.Date = null;
            }

            this.$nextTick(()=>{
                this.$nextTick(()=>{
                    // 卓越
                    this.ExcEntryTableData.forEach((row)=>{
                        if(this.Item.ExcellentEntry.indexOf(row.Id)<0)
                        {
                            this.$refs['ExcMultipleTableRef'].toggleRowSelection(row,false);
                        }else{
                            this.$refs['ExcMultipleTableRef'].toggleRowSelection(row,true);
                        }
                    });

                    // 特殊-翅膀
                    this.SpecialEntryTableData.forEach((row)=>{
                        if(this.Item.SpecialEntry[row.Id] == null)
                        {
                            this.$refs['SpecialEntryMultipleTableRef'].toggleRowSelection(row,false);
                        }else{
                            row.Level = this.Item.SpecialEntry[row.Id];
                            this.$refs['SpecialEntryMultipleTableRef'].toggleRowSelection(row,true);
                        }
                    });
                });
            });
        },
        Form2Item()
        {
            this.Item.Config = item_config[String(this.form.Item.ConfigId)];
            this.Item.SetProp(EDBItemValue.Level,this.form.Item.Level);
            this.Item.SetProp(EDBItemValue.Quantity,this.form.Item.Quantity);
            this.Item.SetProp(EDBItemValue.SetId,this.form.Item.SetId);
            this.Item.SetProp(EDBItemValue.OptValue,this.form.Item.AppendEntryId);
            this.Item.SetProp(EDBItemValue.OptLevel,this.form.Item.AppendLevel);
            this.Item.SetProp(EDBItemValue.FluoreAttr,this.form.Item.FluoreAttr);
            // 镶嵌
            var fluoreSlotCount = 0;
            if(this.form.Item.FluoreSlot1 >= 0)
            {
                this.Item.SetProp(EDBItemValue.FluoreSlot1,this.form.Item.FluoreSlot1);
                fluoreSlotCount = 1;
            }
            if(this.form.Item.FluoreSlot2 >= 0)
            {
                this.Item.SetProp(EDBItemValue.FluoreSlot2,this.form.Item.FluoreSlot2);
                fluoreSlotCount = 2;
            }
            if(this.form.Item.FluoreSlot3 >= 0)
            {
                this.Item.SetProp(EDBItemValue.FluoreSlot3,this.form.Item.FluoreSlot3);
                fluoreSlotCount = 3;
            }
            if(this.form.Item.FluoreSlot4 >= 0)
            {
                this.Item.SetProp(EDBItemValue.FluoreSlot4,this.form.Item.FluoreSlot4);
                fluoreSlotCount = 4;
            }
            if(this.form.Item.FluoreSlot5 >= 0)
            {
                this.Item.SetProp(EDBItemValue.FluoreSlot5,this.form.Item.FluoreSlot5);
                fluoreSlotCount = 5;
            }
            this.Item.SetProp(EDBItemValue.FluoreSlotCount,fluoreSlotCount);

            // 技能，幸运
            if(this.form.Item.option1.indexOf(EDBItemValue.SkillId)<0)    /**技能 */
            {
                this.Item.SetProp(EDBItemValue.SkillId,0);
            }else{
                this.Item.SetProp(EDBItemValue.SkillId,this.Item.Config.Skill != null?this.Item.Config.Skill:0);
            }
            if(this.form.Item.option1.indexOf(EDBItemValue.LuckyEquip)<0)    /**幸运 */
            {
                this.Item.SetProp(EDBItemValue.LuckyEquip,0);
            }else{
                this.Item.SetProp(EDBItemValue.LuckyEquip,1);
            }
            // 普通，绑定账号，绑定角色，任务
            if(this.form.Item.option2 == 0) /**普通 */
            {
                this.Item.SetProp(EDBItemValue.IsBind,0);
                this.Item.SetProp(EDBItemValue.IsTask,0);
            }else if(this.form.Item.option2 == EDBItemValue.IsBind * 1000 + 1) /**绑定账号 */
            {
                this.Item.SetProp(EDBItemValue.IsBind,1);
                this.Item.SetProp(EDBItemValue.IsTask,0);
            }else if(this.form.Item.option2 == EDBItemValue.IsBind * 1000 + 2) /**绑定角色 */
            {
                this.Item.SetProp(EDBItemValue.IsBind,2);
                this.Item.SetProp(EDBItemValue.IsTask,0);
            }else if(this.form.Item.option2 == EDBItemValue.IsTask) /**任务 */
            {
                this.Item.SetProp(EDBItemValue.IsBind,0);
                this.Item.SetProp(EDBItemValue.IsTask,1);
            }

            // 设置有效时间
            if(this.form.Item.ValidTime.Date == null)
            {
                this.Item.SetProp(EDBItemValue.ValidTime,0);
            }else{
                this.Item.SetProp(EDBItemValue.ValidTime,Math.floor(this.form.Item.ValidTime.Date.getTime() / 1000));
            }

            // 卓越
            this.Item.ExcellentEntry = this.form.Item.ExcellentEntryId;
            // 特殊属性
            this.Item.SpecialEntry = this.form.Item.SpecialEntry;
            return this.Item;
        },
        ItemCreateAttr2Form(createAttr)
        {
            this.ItemCreateAttr = createAttr

            this.form.Item.Uid = 0;
            //this.form.Item.ConfigId = this.Item.Config.Id;
            var config = item_config[this.form.Item.ConfigId];
            this.SyncSelectedItemOptionFromConfig();
            this.OnSelectItemChangeRefresh();
            this.form.Item.Level = this.ItemCreateAttr.Level;
            this.form.Item.Quantity = this.ItemCreateAttr.Quantity;
            this.form.Item.SetId = this.ItemCreateAttr.SetId;
            if(config.AppendAttrId.length > 0)
            {
                this.form.Item.AppendEntryId = config.AppendAttrId[this.ItemCreateAttr.OptListId];
            }else{
                this.form.Item.AppendEntryId = 0;
            }
            this.form.Item.AppendLevel = this.ItemCreateAttr.OptLevel;
            this.form.Item.FluoreAttr = this.ItemCreateAttr.FluoreAttr;
            // 镶嵌
            if(this.ItemCreateAttr.FluoreSlotCount >= 1)
                this.form.Item.FluoreSlot1 = this.ItemCreateAttr.FluoreSlot[0];
            if(this.ItemCreateAttr.FluoreSlotCount >= 2)
                this.form.Item.FluoreSlot2 = this.ItemCreateAttr.FluoreSlot[1];
            if(this.ItemCreateAttr.FluoreSlotCount >= 3)
                this.form.Item.FluoreSlot3 = this.ItemCreateAttr.FluoreSlot[2];
            if(this.ItemCreateAttr.FluoreSlotCount >= 4)
                this.form.Item.FluoreSlot4 = this.ItemCreateAttr.FluoreSlot[3];
            if(this.ItemCreateAttr.FluoreSlotCount >= 5)
                this.form.Item.FluoreSlot5 = this.ItemCreateAttr.FluoreSlot[4];
            // 技能，幸运
            if(this.ItemCreateAttr.HaveSkill == true) // 技能
                this.form.Item.option1.push(EDBItemValue.SkillId);
            if(this.ItemCreateAttr.HaveLucky == true)   // 幸运
                this.form.Item.option1.push(EDBItemValue.LuckyEquip);
            else
                this.form.Item.option1 = [];

            // 普通，绑定账号，绑定角色，任务
            if(this.ItemCreateAttr.IsBind == 1)
                this.form.Item.option2 = EDBItemValue.IsBind * 1000 + 1;
            else if(this.ItemCreateAttr.IsBind == 2)
                this.form.Item.option2 = EDBItemValue.IsBind * 1000 + 2;
            else if(this.ItemCreateAttr.IsTask == true)
                this.form.Item.option2 = EDBItemValue.IsTask;
            else
                this.form.Item.option2 = 0;

            // 有效时间
            this.form.Item.ValidTime.Day = Math.floor(this.ItemCreateAttr.ValidTime / (60 * 60 * 24));
            this.form.Item.ValidTime.Hour = Math.floor((this.ItemCreateAttr.ValidTime % (60 * 60 * 24)) / (60 * 60));
            this.form.Item.ValidTime.Minute = Math.floor((this.ItemCreateAttr.ValidTime % (60 * 60)) / 60);

            this.$nextTick(()=>{
                this.$nextTick(()=>{
                    // 卓越
                    this.ExcEntryTableData.forEach((row)=>{
                        if(this.ItemCreateAttr.OptionExcellent.indexOf(row.Id)<0)
                        {
                            this.$refs['ExcMultipleTableRef'].toggleRowSelection(row,false);
                        }else{
                            this.$refs['ExcMultipleTableRef'].toggleRowSelection(row,true);
                        }
                    });

                    // 特殊-翅膀
                    this.SpecialEntryTableData.forEach((row)=>{
                        if(this.ItemCreateAttr.OptionSpecial[row.Id] == null)
                        {
                            this.$refs['SpecialEntryMultipleTableRef'].toggleRowSelection(row,false);
                        }else{
                            row.Level = this.ItemCreateAttr.OptionSpecial[row.Id];
                            this.$refs['SpecialEntryMultipleTableRef'].toggleRowSelection(row,true);
                        }
                    });
                });
            });
            // 自定义属性
            this.form.Item.CustomAttrMethod = this.ItemCreateAttr.CustomAttrMethod;
        },
        Form2ItemCreateAttr()
        {
            if(this.ItemCreateAttr == null)
            {
                this.ItemCreateAttr = new ItemCreateAttr();
            }
            this.ItemCreateAttr.Level = this.form.Item.Level;
            this.ItemCreateAttr.ForgeLevel = this.form.Item.ForgeLevel;
            this.ItemCreateAttr.Quantity = this.form.Item.Quantity;
            this.ItemCreateAttr.SetId = this.form.Item.SetId;
            // 荧光宝石属性
            this.ItemCreateAttr.FluoreAttr = this.form.Item.FluoreAttr;
            // 追加属性
            this.ItemAppendOptions.forEach((item,index)=>{
                if(item.value == this.form.Item.AppendEntryId)
                {
                    this.ItemCreateAttr.OptListId = index == 0 ? 0 : index - 1;
                    this.ItemCreateAttr.OptLevel = this.form.Item.AppendLevel;
                    return;
                }
            });
            // 镶嵌
            var fluoreSlotCount = 0;
            if(this.form.Item.FluoreSlot1 >= 0)
            {
                this.ItemCreateAttr.FluoreSlot.push(this.form.Item.FluoreSlot1);
                fluoreSlotCount = 1;
            }
            if(this.form.Item.FluoreSlot2 >= 0)
            {
                this.ItemCreateAttr.FluoreSlot.push(this.form.Item.FluoreSlot2);
                fluoreSlotCount = 2;
            }
            if(this.form.Item.FluoreSlot3 >= 0)
            {
                this.ItemCreateAttr.FluoreSlot.push(this.form.Item.FluoreSlot3);
                fluoreSlotCount = 3;
            }
            if(this.form.Item.FluoreSlot4 >= 0)
            {
                this.ItemCreateAttr.FluoreSlot.push(this.form.Item.FluoreSlot4);
                fluoreSlotCount = 4;
            }
            if(this.form.Item.FluoreSlot5 >= 0)
            {
                this.ItemCreateAttr.FluoreSlot.push(this.form.Item.FluoreSlot5);
                fluoreSlotCount = 5;
            }
            this.ItemCreateAttr.FluoreSlotCount = fluoreSlotCount;
            // 技能，幸运
            if(this.form.Item.option1.indexOf(EDBItemValue.SkillId)<0)    /**技能 */
            {
                this.ItemCreateAttr.HaveSkill = false;
            }else{
                this.ItemCreateAttr.HaveSkill = true;
            }
            if(this.form.Item.option1.indexOf(EDBItemValue.LuckyEquip)<0)    /**幸运 */
            {
                this.ItemCreateAttr.HaveLucky = false;
            }else{
                this.ItemCreateAttr.HaveLucky = true;
            }
            // 普通，绑定，任务
            if(this.form.Item.option2 == 0) /**普通 */
            {
                this.ItemCreateAttr.IsBind = 0;
                this.ItemCreateAttr.IsTask = false;
            }else if(this.form.Item.option2 == EDBItemValue.IsBind * 1000 + 1) /**绑定账号 */
            {
                this.ItemCreateAttr.IsBind = 1;
                this.ItemCreateAttr.IsTask = false;
            }else if(this.form.Item.option2 == EDBItemValue.IsBind * 1000 + 2) /**绑定角色 */
            {
                this.ItemCreateAttr.IsBind = 2;
                this.ItemCreateAttr.IsTask = false;
            }else if(this.form.Item.option2 == EDBItemValue.IsTask) /**任务 */
            {
                this.ItemCreateAttr.IsBind = 0;
                this.ItemCreateAttr.IsTask = true;
            }
            // 有效时间
            this.ItemCreateAttr.ValidTime = this.form.Item.ValidTime.Day * (60 * 60 * 24);
            this.ItemCreateAttr.ValidTime += this.form.Item.ValidTime.Hour * (60 * 60);
            this.ItemCreateAttr.ValidTime += this.form.Item.ValidTime.Minute * 60;
            // 卓越
            this.ItemCreateAttr.OptionExcellent = this.form.Item.ExcellentEntryId;
            // 特殊
            this.ItemCreateAttr.OptionSpecial = this.form.Item.SpecialEntry;
            // 自定义属性
            this.ItemCreateAttr.CustomAttrMethod = this.form.Item.CustomAttrMethod;
            return this.ItemCreateAttr;
        },
        Init(){
            this.form.Item.Uid = 0;
            this.form.Item.ConfigId = 0;
            this.form.SelectedPetStage = 0;
            this.form.SelectedItemOptionKey = null;
            this.form.Item.Level = 0;
            this.form.Item.Quantity = 1;
            this.form.Item.SetId = 0;
            this.form.Item.AppendEntryId = 0;
            this.form.Item.AppendLevel = 0;
            this.form.Item.FluoreAttr = 0;
            // 镶嵌
            this.form.Item.FluoreSlot1 = -1;
            this.form.Item.FluoreSlot2 = -1;
            this.form.Item.FluoreSlot3 = -1;
            this.form.Item.FluoreSlot4 = -1;
            this.form.Item.FluoreSlot5 = -1;
            // 技能，幸运
            this.form.Item.option1 = [];

            // 普通，绑定，任务
            this.form.Item.option2 = 0;

            // 有效时间
            this.form.Item.ValidTime.Day = 0;
            this.form.Item.ValidTime.Hour = 0;
            this.form.Item.ValidTime.Minute = 0;
            this.form.Item.ValidTime.Date = null;

            // 卓越
            this.form.Item.ExcellentEntryId = [];
            this.$nextTick(()=>{
                this.$nextTick(()=>{
                    this.ExcEntryTableData.forEach((row)=>{
                        if(this.form.Item.ExcellentEntryId.indexOf(row.Id)<0)
                        {
                            this.$refs['ExcMultipleTableRef'].toggleRowSelection(row,false);
                        }else{
                            this.$refs['ExcMultipleTableRef'].toggleRowSelection(row,true);
                        }
                    });
                });
            });

            this.Item = null;
            this.ItemCreateAttr = null;

            this.RefreshtableItemOptions();
            this.SelectFirstItemOption();
            this.OnSelectItemChangeRefresh();
        }

    },
    created(){

    },


}

</script>

<style>
.item-editor{
    width: 480px !important;
}

.w-214{
    width: 214px;
}
</style>
