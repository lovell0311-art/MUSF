<template>
    <el-button v-if="$permissOk(EPermiss.send_mail)" ref="MailSend" type="primary" @click="OnClickSendMail">发送邮件</el-button>
    <el-button v-if="$permissOk(EPermiss.send_mail)" ref="MailSend" type="primary" @click="OnClickSendFullServiceMail">发送全服邮件</el-button>
    
    <MailSend v-if="$permissOk(EPermiss.send_mail)" ref="MailSend" v-bind:dialogFormVisible="dialogFormVisible" />
</template>

<script>

import MailSend from '@/components/MailSend';
import { ref,reactive } from 'vue';
import { EPermiss } from '../../../../../common/permiss';


export default{
    components:{
        MailSend
    },
    data(){
        return{
            EPermiss:EPermiss,
            dialogFormVisible:{
                value:false,
            }
        }
    },
    methods:{
        OnClickSendMail(){
            this.$refs.MailSend.IsFullServiceMail = false;
            this.$refs.MailSend.Clear();
            this.dialogFormVisible.value = true
        },
        OnClickSendFullServiceMail(){
            this.$refs.MailSend.IsFullServiceMail = true;
            this.$refs.MailSend.Clear();
            this.dialogFormVisible.value = true
        }
    }
}

</script>