<template>
    <div class="container">
        <el-tabs
            v-model="editableTabsValue"
            type="card"
            class="demo-tabs"
            @tab-click="clickTag"
            >
            <template  v-for="item in editableTabs">
                <el-tab-pane
                v-if="$permissOk(item.permiss)"
                :label="item.title"
                :name="item.name"
                :key="item.name"
                />
            </template>
        </el-tabs>
        <router-view />
    </div>
</template>

<script>
import { computed } from 'vue';
import { useRoute } from 'vue-router';
import { ref } from 'vue'
import {useRoleStore} from '@/store/role'
import { EPermiss } from '../../../../common/permiss';

export default{
    name:"role",
    setup(){
        const role = useRoleStore();
        const editableTabsValue = ref('')
        const editableTabs = ref([
        {
            title: '账号信息',
            name: 'info',
            permiss:EPermiss.account_info_view,
        },
        {
            title: '账号日志',
            name: 'log',
            permiss:EPermiss.account_log_view,
        },
        {
            title: '充值日志',
            name: 'log_pay',
            permiss:EPermiss.account_log_pay_view,
        }
        ]);
        return {
            editableTabsValue,
            editableTabs,
            role,
        }
    },
    date(){
        return{
        }
    },
    created(){
        this.editableTabsValue = this.$route.meta.tags_value;
        window.document.title = this.role.name;
    },
    beforeUpdate(){
        this.editableTabsValue = this.$route.meta.tags_value;
        window.document.title = this.role.name;
    },
    methods:{
        handleTabsEdit(targetName){
            console.log(`targetName=${targetName}`);
        },
        clickTag(pane,ev)
        {
            this.$router.push(`/player/z${this.$route.params.zone_id}/a${this.$route.params.account_uid}/account/${pane.props.name}`);
        }
    }

}
</script>

<style>

</style>