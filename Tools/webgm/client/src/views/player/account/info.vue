<template>
    <el-row :gutter="20">
        <el-col :span="12">
            <el-form :model="AccountData" label-width="100px">
                <el-divider content-position="left">账号信息</el-divider>
                <el-form-item label="账号ID">
                    <el-text class="mx-1">{{ AccountData.UserId }}</el-text>
                    <el-link type="primary" @click="OnCopy(AccountData.UserId)">复制</el-link>
                </el-form-item>
                <el-form-item label="账号类型">
                    <el-select
                    v-model="AccountData.Identity"
                    placeholder="账号类型"
                    style="width: 240px"
                    :disabled="!$permissOk(EPermiss.modify_identity)"
                    @change="OnClickModifyIdentity()">
                        <el-option :key="0" label="普通用户" :value="0"></el-option>
                        <el-option :key="1" label="测试账号" :value="1" ></el-option>
                        <el-option :key="2" label="工作室" :value="2" ></el-option>
                    </el-select>
                </el-form-item>
                <el-form-item label="XYUID">
                    <el-input v-model="AccountData.XYUID" :disabled="!$permissOk(EPermiss.modify_xyuid)" style="width:240px"/>
                    <el-button @click="OnClickModifyXYUID()" v-permiss="EPermiss.modify_xyuid">换绑</el-button>
                </el-form-item>
                <el-form-item label="手机号">
                    <el-input v-model="AccountData.Phone" :disabled="!$permissOk(EPermiss.modify_phone)" style="width:240px"/>
                    <el-button @click="OnClickModifyPhone()" v-permiss="EPermiss.modify_phone">换绑</el-button>
                </el-form-item>
                <el-form-item label="渠道id">
                    <el-input v-model="AccountData.ChannelId" :disabled="!$permissOk(EPermiss.modify_channel_id)" style="width:240px"/>
                    <el-button @click="OnClickModifyChannelId()" v-permiss="EPermiss.modify_channel_id">修改</el-button>
                </el-form-item>

                <el-form-item label="状态">
                    <el-tag v-if="this.AccountData.OnlineStatus == 1" class="ml-2" type="success">在线</el-tag>
                    <el-tag v-else-if="this.AccountData.OnlineStatus == 0" class="ml-2" type="info">离线</el-tag>
                    <el-tag v-else class="ml-2" type="info">获取中</el-tag>

                    <el-button v-if="this.AccountData.OnlineStatus == 1" size="small" @click="OnClickKick()" v-permiss="EPermiss.offlin_account">下线</el-button>
                </el-form-item>
                <el-form-item label="最后登录时间">
                    <el-text class="mx-1">{{ AccountData.LastLoginTimeString }}</el-text>
                </el-form-item>
                <el-form-item label="最后登录IP">
                    <el-text class="mx-1">{{ AccountData.LastLoginIP }}</el-text>
                </el-form-item>
                <el-form-item label="注册时间">
                    <el-text class="mx-1">{{ AccountData.RegisterTimeString }}</el-text>
                </el-form-item>
                <el-form-item label="注册IP">
                    <el-text class="mx-1">{{ AccountData.RegisterIP }}</el-text>
                </el-form-item>

                <el-divider content-position="left">账号管理</el-divider>
                <el-form-item label="解封时间">
                    <el-date-picker
                        v-model="AccountData.BanTillTime"
                        type="datetime"
                        placeholder="无封禁"
                        :disabled="!$permissOk(EPermiss.ban_account)"
                    />
                    <el-button @click="OnClickModifyBanTillTime()" v-permiss="EPermiss.ban_account">修改</el-button>
                </el-form-item>
                <el-form-item label="封禁时间">
                    <el-space :size="4">
                        <el-input-number v-model="AccountData.BanTime.Day" size="small" style="width:64px" :min="0" :max="3650" :controls="false" :disabled="!$permissOk(EPermiss.ban_account)"/>
                        <el-text class="mx-1">天</el-text>
                        <el-input-number v-model="AccountData.BanTime.Hour" size="small" style="width:64px" :min="0" :max="24" :controls="false" :disabled="!$permissOk(EPermiss.ban_account)"/>
                        <el-text class="mx-1">时</el-text>
                        <el-input-number v-model="AccountData.BanTime.Minute" size="small" style="width:64px" :min="0" :max="60" :controls="false" :disabled="!$permissOk(EPermiss.ban_account)"/>
                        <el-text class="mx-1">分</el-text>
                    </el-space>
                </el-form-item>
                <el-form-item label="封禁原因">
                    <el-select v-model="AccountData.BanReason" placeholder="封禁原因" style="width: 240px" :disabled="!$permissOk(EPermiss.ban_account)">
                        <el-option
                            v-for="item in form.BanReason.Options"
                            :key="item.value"
                            :label="item.label"
                            :value="item.value"
                            />
                    </el-select>
                    <el-button @click="OnClickBan()" v-permiss="EPermiss.ban_account">封禁</el-button>
                </el-form-item>
                <el-form-item label="封禁原因">
                    <el-input
                        v-model="AccountData.BanReason"
                        :rows="2"
                        type="textarea"
                        placeholder="自定义封禁原因"
                        style="width: 240px"
                        :disabled="!$permissOk(EPermiss.ban_account)"
                    />
                </el-form-item>
            </el-form>
            <el-tabs
                  v-model="form.tabsActiveName"
                  type="border-card"
                  class="modify-tabs"
                  @tab-click="handleClick"
                >
                  <el-tab-pane label="修改密码" name="modify_password">
                    <el-form :model="AccountData" label-width="100px">
                      <el-form-item label="密码(密文)">
                        <el-input v-model="AccountData.Password" :disabled="true" style="width:240px"/>
                      </el-form-item>
                      <el-form-item label="密码(明文)">
                        <el-input v-model="AccountData.Password2" :disabled="!$permissOk(EPermiss.modify_password)" style="width:240px"/>
                        <el-button @click="OnClickModifyPassword()" v-permiss="EPermiss.modify_password">修改</el-button>
                      </el-form-item>
                    </el-form>
                  </el-tab-pane>
                  <el-tab-pane label="实名认证" name="identity">
                    <el-form :model="AccountData" label-width="100px">
                      <el-form-item label="身份证">
                        <el-input v-model="AccountData.IdCard" :disabled="!$permissOk(EPermiss.modify_idcard)" style="width:240px"/>
                      </el-form-item>
                      <el-form-item label="姓名">
                        <el-input v-model="AccountData.Name" :disabled="!$permissOk(EPermiss.modify_idcard)" style="width:240px"/>
                      </el-form-item>
                      <el-form-item>
                        <el-button type="primary" @click="OnClickModifyIdCard()" v-permiss="EPermiss.modify_idcard">修改</el-button>
                      </el-form-item>
                    </el-form>
                  </el-tab-pane>
                </el-tabs>
          </el-col>
        <el-col :span="12">

        </el-col>
    </el-row>
</template>

<script>
import { reactive } from 'vue';
import { ElMessage,ElLoading } from 'element-plus';
import clipboard from 'clipboardy';
import moment from 'moment';
import md5 from 'md5-node';
import { EPermiss } from '../../../../../common/permiss';

export default{
    data(){
        return {
            EPermiss:EPermiss,
            AccountData:{
                UesrId: "",
                RegisterTime: 0,
                RegisterTimeString: "",
                RegisterIP: "",
                ChannelId: "",
                XYUID: "",
                LastLoginTime: 0,
                LastLoginTimeString: "",
                LastLoginIP: "",
                BanTillTime: null,
                BanReason: "",
                BanTime:{
                    Day: 0,
                    Hour: 0,
                    Minute: 0,
                },
                OnlineStatus: 0,
                Identity:0,
                IdCard:"",
                Name:"",
                Password:"",
                Password2:"",
            },
            form:{
                BanReason:{
                    Options:[
                        {
                            value:"账号异常",
                            label:"账号异常",
                        },
                        {
                            value:"使用外挂",
                            label:"使用外挂",
                        },
                        {
                            value:"违规刷屏",
                            label:"违规刷屏",
                        },
                        {
                            value:"传播违规内容",
                            label:"传播违规内容",
                        }
                    ]
                },
                tabsActiveName:"modify_password",
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
        async RefreshAccountInfo()
        {
            let res = await this.$axios.post('/api/game/account/info',{userId:this.$route.params.account_uid});
            if(res.data.success == false)
            {
                ElMessage.error("账号不存在");
                return;
            }
            let date = new Date();
            let formatTime = 'yyyy年MM月DD日 HH时mm分ss秒';

            let data = res.data.data;
            this.AccountData.UserId = data.UserId;
            this.AccountData.RegisterTime = data.RegisterTime;
            date.setTime(data.RegisterTime);
            this.AccountData.RegisterTimeString = moment(date).format(formatTime);
            this.AccountData.RegisterIP = data.RegisterIP;
            this.AccountData.ChannelId = data.ChannelId;
            this.AccountData.XYUID = data.XYAccountNumber;
            this.AccountData.Phone = data.Phone;
            this.AccountData.LastLoginTime = data.LastLoginTime;
            date.setTime(data.LastLoginTime);
            this.AccountData.LastLoginTimeString = moment(date).format(formatTime);
            this.AccountData.LastLoginIP = data.LastLoginIP;
            this.AccountData.Password = data.Password;
            this.AccountData.Password2 = "";
            this.AccountData.Identity = data.Identity;
            this.AccountData.IdCard = data.IdCard;
            this.AccountData.Name = data.Name;
            let banTillTimestamp = parseInt(data.BanTillTime);
            if(banTillTimestamp != 0)
            {
                this.AccountData.BanTillTime = new Date();
                this.AccountData.BanTillTime.setTime(banTillTimestamp);
            }else{
                this.AccountData.BanTillTime = null;
            }
            this.AccountData.BanReason = data.BanReason;

            this.AccountData.BanTime.Day = 0;
            this.AccountData.BanTime.Hour = 0;
            this.AccountData.BanTime.Minute = 0;

            // 在线状态获取
            res = await this.$axios.post('/api/game/player/get_login_record',{userId:this.AccountData.UserId});
            if(res.data.success == false)
            {
                this.AccountData.OnlineStatus = -1;
                ElMessage.error(res.data.msg);
                return;
            }

            this.AccountData.OnlineStatus = (res.data.data.Online == true) ? 1 : 0;
        },
        async OnClickBan()
        {
            // 封禁账号
            let banTillTime = Math.floor(new Date().getTime());
            banTillTime += ((1000 * 60) * this.AccountData.BanTime.Minute);
            banTillTime += ((1000 * 60 * 60) * this.AccountData.BanTime.Hour);
            banTillTime += ((1000 * 60 * 60 * 24) * this.AccountData.BanTime.Day);

            var res = await this.$axios.post('/api/game/account/ban',{
                userId: this.AccountData.UserId,
                banTillTime: String(banTillTime),
                banReason: this.AccountData.BanReason
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("封禁成功");
        },
        async OnClickModifyIdentity()
        {
          var res = await this.$axios.post('/api/game/account/identity/modify',{
                userId: this.AccountData.UserId,
                identity: this.AccountData.Identity
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("修改成功");
        },
        async OnClickModifyPassword(){
          let password = md5(this.AccountData.Password2).toUpperCase();
          var res = await this.$axios.post('/api/game/account/password/modify',{
                userId: this.AccountData.UserId,
                password: password,
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            this.AccountData.Password = password;
            ElMessage.success("修改成功");
        },
        async OnClickModifyIdCard(){
          var res = await this.$axios.post('/api/game/account/idcard/modify',{
                userId: this.AccountData.UserId,
                idcard: this.AccountData.IdCard,
                name: this.AccountData.Name,
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("修改成功");
        },
        async OnClickModifyBanTillTime()
        {
            // 封禁封禁时间
            let banTillTime = 0;
            if(this.AccountData.BanTillTime != null)
            {
                banTillTime = this.AccountData.BanTillTime.getTime();
            }

            var res = await this.$axios.post('/api/game/account/ban',{
                userId: this.AccountData.UserId,
                banTillTime: String(banTillTime),
                banReason: this.AccountData.BanReason
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("修改成功");
        },
        async OnClickKick()
        {
            // 踢账号下线
            var res = await this.$axios.post('/api/game/account/kick',{
                userId: this.AccountData.UserId,
                reason: "被 GM 踢下线"
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("下线成功");
        },
        async OnClickModifyXYUID()
        {
            var res = await this.$axios.post('/api/game/account/xyuid/modify',{
                userId: this.AccountData.UserId,
                xyuid: this.AccountData.XYUID
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("换绑成功");
        },
        async OnClickModifyPhone()
        {
            var res = await this.$axios.post('/api/game/account/phone/modify',{
                userId: this.AccountData.UserId,
                phone: this.AccountData.Phone
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("换绑成功");
        },
        async OnClickModifyChannelId()
        {
            var res = await this.$axios.post('/api/game/account/channel_id/modify',{
                userId: this.AccountData.UserId,
                channelId: this.AccountData.ChannelId
            });
            if(res.data.success == false)
            {
                ElMessage.error(res.data.msg);
                return;
            }
            ElMessage.success("修改成功");
        }


    },
    created(){
            console.log("created data");
            this.RefreshAccountInfo();
    },
    beforeUpdate(){
        console.log("beforeUpdate data");
        this.RefreshAccountInfo();
    }
}
</script>