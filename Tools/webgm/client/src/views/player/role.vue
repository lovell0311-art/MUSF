<template>
    <div class="container">
        <el-tabs
            v-model="editableTabsValue"
            type="card"
            class="demo-tabs"
            @tab-click="clickTag"
            >
            <template v-for="item in editableTabs">
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
            title: '属性',
            name: 'base_attr',
            permiss:EPermiss.role_info_view,
        },
        {
            title: '装备栏',
            name: 'equipment',
            permiss:EPermiss.role_equipment_view,
        },
        {
            title: '背包',
            name: 'backpack',
            permiss:EPermiss.role_backpack_view,
        },
        {
            title: '仓库',
            name: 'warehouse',
            permiss:EPermiss.role_warehouse_view,
        },
        {
            title: '技能',
            name: 'skill',
            permiss:EPermiss.role_skill_view,
        },
        {
            title: '邮件',
            name: 'mail',
            permiss:EPermiss.role_mail_view,
        },
        {
            title: '宠物',
            name: 'pet',
            permiss:EPermiss.role_pet_view,
        },
        {
            title: '称号',
            name: 'title',
            permiss:EPermiss.role_title_view,
        },
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
            this.$router.push(`/player/z${this.$route.params.zone_id}/a${this.$route.params.account_uid}/r${this.$route.params.role_uid}/${pane.props.name}`);
        }
    }

}
</script>

<style>

</style>