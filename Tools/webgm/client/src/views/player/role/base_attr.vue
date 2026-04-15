<template>
    <el-alert title="当前界面需要玩家下线后，'状态' 显示为离线时，修改才会生效!" type="error" :closable="false" />
    <br>
    <el-row>
        <el-col :sm="12" :xs="24">
            <el-form :model="roleData" label-width="100px" :disabled="!$permissOk(EPermiss.modify_role_data)">
                <el-form-item label="账号ID:">
                    <el-text class="mx-1">{{ roleData.UserId }}</el-text>
                    <el-link type="primary" @click="OnCopy(roleData.UserId)">复制</el-link>
                </el-form-item>
                <el-form-item label="角色ID:">
                    <el-text class="mx-1">{{ roleData.GameUserId }}</el-text>
                    <el-link type="primary" @click="OnCopy(roleData.GameUserId)">复制</el-link>
                </el-form-item>
                <el-form-item label="状态:">
                    <el-tag v-if="roleData.OnlineStatus == EOnlineStatus.Online" class="ml-2" type="success">角色在线</el-tag>
                    <el-tag v-else-if="roleData.OnlineStatus == EOnlineStatus.AccountOnline" class="ml-2" type="success">账号在线</el-tag>
                    <el-tag v-else-if="roleData.OnlineStatus == EOnlineStatus.Offline" class="ml-2" type="info">离线</el-tag>
                    <el-tag v-else class="ml-2" type="info">获取中</el-tag>

                    <template v-if="$permissOk(EPermiss.offlin_account)">
                        <el-button
                        v-if="roleData.OnlineStatus == EOnlineStatus.Online ||
                        roleData.OnlineStatus == EOnlineStatus.AccountOnline"
                        size="small"
                        @click="OnClickKick()"
                        >
                            下线
                        </el-button>
                    </template>
                </el-form-item>
                <el-form-item label="角色名:">
                    <el-text class="mx-1">{{ roleData.NickName }}</el-text>
                </el-form-item>
                <el-form-item label="职业:">
                    <el-text class="mx-1">{{ roleData.ClassName }}</el-text>
                </el-form-item>
                <el-form-item label="转职等级:">
                    <el-select v-model="roleData.OccupationLevel" class="class-level-select" placeholder="Select" size="large" @change="OnSelectChangeFromOccupationLevel">
                        <el-option :key="0" label="未转职" :value="0" />
                        <el-option :key="1" label="一转" :value="1" />
                        <el-option :key="2" label="二转" :value="2" />
                        <el-option :key="3" label="三转" :value="3" />
                    </el-select>
                </el-form-item>
                <el-form-item label="等级:">
                    <el-input-number v-model="roleData.Level" :min="0" :max="800" />
                </el-form-item>
                <el-form-item label="背包金币:">
                    <el-input-number v-model="roleData.GoldCoin" :min="0" :max="2100000000" :controls="false" />
                </el-form-item>
                <el-form-item label="奇迹币:">
                    <el-input-number v-model="roleData.MiracleCoin" :min="0" :max="2100000000" :controls="false" />
                </el-form-item>
            </el-form>
        </el-col>
        <el-col :sm="12" :xs="24">
            <el-col :sm="24" :lg="12">
                <div class="radius" :style="{borderRadius:'No Radius'}">
                    <h3>基础属性：</h3>
                    <el-divider />
                    <div class="base_attr">
                        <el-form :model="roleData" :disabled="!$permissOk(EPermiss.modify_role_data)">
                            <el-form-item label="力量:">
                                <el-input-number v-model="roleData.Strength" :min="0" :max="1000000" :controls="false"/>
                            </el-form-item>
                            <el-form-item label="敏捷:">
                                <el-input-number v-model="roleData.Agility" :min="0" :max="1000000" :controls="false" />
                            </el-form-item>
                            <el-form-item label="智力:">
                                <el-input-number v-model="roleData.Willpower" :min="0" :max="1000000" :controls="false" />
                            </el-form-item>
                            <el-form-item label="体力:">
                                <el-input-number v-model="roleData.BoneGas" :min="0" :max="1000000" :controls="false" />
                            </el-form-item>
                            <el-form-item label="统帅:">
                                <el-input-number v-model="roleData.Command" :min="0" :max="1000000" :controls="false" />
                            </el-form-item>
                            <el-form-item label="属性点:">
                                <el-input-number v-model="roleData.FreePoint" :min="0" :max="1000000" :controls="false" />
                            </el-form-item>
                        </el-form>
                    </div>
                </div>

                <div class="radius" :style="{borderRadius:'No Radius'}">
                    <h3>账号数据：</h3>
                    <el-divider />
                    <div class="base_attr">
                        <el-form :model="roleData" :disabled="!$permissOk(EPermiss.modify_role_data)">
                            <el-form-item label="魔晶:">
                                <el-input-number v-model="accountZoneData.YuanbaoCoin" :min="0" :max="2100000000" :controls="false"/>
                            </el-form-item>
                            <el-form-item>
                                <el-button type="primary" @click="OnModifyAccountZoneData" v-permiss="EPermiss.modify_role_data">修改账号数据</el-button>
                            </el-form-item>
                        </el-form>
                    </div>
                </div>
            </el-col>
        </el-col>
        <el-button type="primary" @click="OnModifyRoleData" v-permiss="EPermiss.modify_role_data">修改角色数据</el-button>
    </el-row>
</template>

<script>
import { reactive } from 'vue';
import { ElMessage,ElLoading } from 'element-plus';
import clipboard from 'clipboardy';
import {GetClassName} from '../../../game/role';
import { EPermiss } from '../../../../../common/permiss';

export default{
    setup(){
        console.log("base_attr setup");
        const roleData = reactive({
            UserId:'',
            GameUserId:'',
            OnlineStatus: -1,    /**0.离线 1.角色在线 2.账号在线 */
        });

        const accountZoneData = reactive({
          YuanbaoCoin:0,
        });
        return {
            roleData,
            accountZoneData
        };
    },
    data(){
        console.log("base_attr data");
        return {
            EPermiss:EPermiss,
            EOnlineStatus:{
                Offline: 0, /**离线 */
                Online: 1, /**在线 */
                AccountOnline: 2, /**账号在线 */
            }
        };
    },
    methods:{
        OnCopy(value)
        {
            console.log(`copy value = ${value}`);
            clipboard.write(value);
            ElMessage.success("复制成功");
        },
        async RefreshRoleData(){
            // 刷新角色数据
            var res = await this.$axios.post('/api/game/player/role_data',{zoneId:this.$route.params.zone_id,gameUserId:this.$route.params.role_uid});
            if(res.data.success == false)
            {
                ElMessage.error("角色不存在");
                return;
            }

            const data = res.data.data;
            this.roleData.UserId = data.UserId;
            this.roleData.GameUserId = data.GameUserId;
            this.roleData.NickName = data.NickName;
            this.roleData.ClassName = GetClassName(data.PlayerTypeId,data.OccupationLevel);
            this.roleData.PlayerTypeId = data.PlayerTypeId;
            this.roleData.OccupationLevel = data.OccupationLevel;
            this.roleData.Level = data.Level;
            this.roleData.GoldCoin = data.GoldCoin;
            this.roleData.MiracleCoin = data.MiracleCoin;

            // 基础属性
            this.roleData.Strength = data.Strength;
            this.roleData.Willpower = data.Willpower;
            this.roleData.Agility = data.Agility;
            this.roleData.BoneGas = data.BoneGas;
            this.roleData.Command = data.Command;
            this.roleData.FreePoint = data.FreePoint;

            this.roleData.OnlineStatus = -1;

            console.log(this.roleData);


            var res = await this.$axios.post('/api/game/player/account_zone_data',{zoneId:this.$route.params.zone_id,userId:this.$route.params.account_uid});
            if(res.data.success == false)
            {
                ElMessage.warning(res.data.msg);
            }else{
              this.accountZoneData.YuanbaoCoin = res.data.data.YuanbaoCoin;
            }


            // 获取角色有没有在线
            res = await this.$axios.post('/api/game/player/get_login_record',{userId:this.roleData.UserId});
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            if(res.data.data.Online == false)
            {
                this.roleData.OnlineStatus = this.EOnlineStatus.Offline;
                return;
            }

            if(res.data.data.GameUserId == this.roleData.GameUserId)
            {
                // 角色在线
                this.roleData.OnlineStatus = this.EOnlineStatus.Online;
            }else{
                // 账号在线
                this.roleData.OnlineStatus = this.EOnlineStatus.AccountOnline;
            }

        },
        OnSelectChangeFromOccupationLevel()
        {
            this.roleData.ClassName = GetClassName(this.roleData.PlayerTypeId,this.roleData.OccupationLevel);
        },
        OnModifyRoleData()
        {
            const data = {
                OccupationLevel:this.roleData.OccupationLevel,
                Level:this.roleData.Level,
                GoldCoin:this.roleData.GoldCoin,
                MiracleCoin:this.roleData.MiracleCoin,

                Strength:this.roleData.Strength,
                Willpower:this.roleData.Willpower,
                Agility:this.roleData.Agility,
                BoneGas:this.roleData.BoneGas,
                Command:this.roleData.Command,
                FreePoint:this.roleData.FreePoint,
            };
            this.$axios.post('/api/game/player/role_data/modify',{zoneId:this.$route.params.zone_id,gameUserId:this.$route.params.role_uid,data:data}).then(
                res=>{
                    if(res.data.success == false)
                    {
                        ElMessage.error(res.data.msg);
                    }else{
                        ElMessage.success("修改成功");
                    }
                }
            );

        },
        OnModifyAccountZoneData()
        {
            const data = {
                YuanbaoCoin:this.accountZoneData.YuanbaoCoin,
            };
            this.$axios.post('/api/game/player/account_zone_data/modify',{zoneId:this.$route.params.zone_id,userId:this.$route.params.account_uid,data:data}).then(
                res=>{
                    if(res.data.success == false)
                    {
                        ElMessage.error(res.data.msg);
                    }else{
                        ElMessage.success("修改成功");
                    }
                }
            );

        },
        async OnClickKick()
        {
            // 踢账号下线
            var res = await this.$axios.post('/api/game/account/kick',{
                userId: this.roleData.UserId,
                reason: "被 GM 踢下线"
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("下线成功");
        },
    },
    created(){
        console.log("created data");
        this.RefreshRoleData();
    },
    beforeUpdate(){
        console.log("beforeUpdate data");
        this.RefreshRoleData();
    }
}

</script>


<style scoped>
.radius {
    padding:20px;
    border: 1px solid var(--el-border-color);
    border-radius: 0;
}

.class-level-select{
    width: 100px;
}

.handle-input{
    width:120px;
}

</style>