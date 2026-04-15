<template>
    <div class="sidebar">
        <el-menu
            class="sidebar-el-menu"
            :default-active="onRoutes"
            :collapse="sidebar.collapse"
            background-color="#324157"
            text-color="#bfcbd9"
            active-text-color="#20a0ff"
            unique-opened
            router
        >
                <!-- 折叠按钮 -->
                <div class="collapse-btn"
                    @click="collapseChage">
                    <el-icon v-if="sidebar.collapse"><Expand /></el-icon>
                    <el-icon v-else><Fold /></el-icon>
                </div>
            <template v-for="item in items">
                <template v-if="item.subs">
                    <el-sub-menu :index="item.index" :key="item.index" v-permiss="item.permiss">
                        <template #title>
                            <el-icon>
                                <component :is="item.icon"></component>
                            </el-icon>
                            <span>{{ item.title }}</span>
                        </template>
                        <template v-for="subItem in item.subs">
                            <el-sub-menu
                                v-if="subItem.subs"
                                :index="subItem.index"
                                :key="subItem.index"
                                v-permiss="item.permiss"
                            >
                                <template #title>{{ subItem.title }}</template>
                                <el-menu-item v-for="(threeItem, i) in subItem.subs" :key="i" :index="threeItem.index" @click="clickEnum(subItem)">
                                    {{ threeItem.title }}
                                </el-menu-item>
                            </el-sub-menu>
                            <el-menu-item v-else :index="subItem.index" v-permiss="item.permiss" @click="clickEnum(subItem)">
                                {{ subItem.title }}
                            </el-menu-item>
                        </template>
                    </el-sub-menu>
                </template>
                <template v-else>
                    <el-menu-item :index="item.index" :key="item.index" v-permiss="item.permiss">
                        <el-icon>
                            <component :is="item.icon"></component>
                        </el-icon>
                        <template #title>{{ item.title }}</template>
                    </el-menu-item>
                </template>
            </template>
        </el-menu>
    </div>
</template>

<script>
import { computed } from 'vue';
import {useSidebarStore} from '../store/sidebar'
import {useRoleStore} from '@/store/role'
import { useRoute } from 'vue-router';
import { GetClassName } from '../game/role';
import { onMounted } from 'vue';
import { EPermiss } from '../../../common/permiss';

export default{
    name:"leftmenu",
    setup(){
        const sidebar = useSidebarStore();
        const role = useRoleStore();
        const route = useRoute();
        const onRoutes = computed(() => {
            return route.path;
        });

        const items = [
                {
                    icon: 'User',
                    index: 'info',
                    title: '账号信息',
                    permiss: EPermiss.account_view,
                },
                {
                    icon: 'TakeawayBox',
                    index: '1',
                    title: '角色存档',
                    permiss: EPermiss.role_view,
                    subs: [],
                }
            ];

        onMounted(()=>{
            if (document.body.clientWidth < 1500) {
                sidebar.collapse = true;
            }
        });


        return {
            sidebar,
            onRoutes,
            items,
            role
        };
    },
    
    data(){
        return {
        };
    },
    methods:{
        collapseChage(){
            // 侧边栏折叠
            this.sidebar.handleCollapse();
        },
        clickEnum(item){
            console.log("clickEnum "+item.index);
        }

    },
    created(){
        console.log("PlayerLeftMenu created");

        this.$axios.post('/api/game/player/archive',{zoneId:this.$route.params.zone_id,userId:this.$route.params.account_uid}).then(
            res=>{
                if(res.data.data.length == 0)
                {
                    this.items[1].title = "角色存档 [空]"
                }else{
                    res.data.data.forEach(ares=>{
                        if(ares.IsDisposePlayer == 0)
                        {
                            this.items[1].subs.push({
                                index:`/player/z${this.$route.params.zone_id}/a${this.$route.params.account_uid}/r${ares.GameUserId}/base_attr`,
                                title:`[${GetClassName(ares.PlayerTypeId,ares.OccupationLevel)}] ${ares.NickName} lv.${ares.Level}`,
                                permiss:EPermiss.role_info_view,
                                roleData: ares,
                            });
                        }
                    });
                }

                this.items[1].subs.forEach((item,index)=>{
                    if(item.roleData.GameUserId == this.$route.params.role_uid)
                    {
                        this.role.name = item.roleData.NickName;
                        window.document.title = this.role.name;
                        return;
                    }
                });
            }
        )
        this.items[0].index = `/player/z${this.$route.params.zone_id}/a${this.$route.params.account_uid}/account`;

    },
    beforeUpdate(){
        this.items[1].subs.forEach((item,index)=>{
                    if(item.roleData.GameUserId == this.$route.params.role_uid)
                    {
                        this.role.name = item.roleData.NickName;
                        window.document.title = this.role.name;
                        //this.items[1].index = item.index;
                        return;
                    }
                });
    },
    
};

</script>

<style scoped>
.sidebar {
    display: block;
    position: absolute;
    left: 0;
    top: 0;
    bottom: 0;
    overflow-y: scroll;
}
.sidebar::-webkit-scrollbar {
    width: 0;
}
.sidebar-el-menu:not(.el-menu--collapse) {
    width: 200px;
}
.sidebar > ul {
    height: 100%;
}
.collapse-btn {
	display: flex;
	justify-content: center;
	align-items: center;
    width: 100%;
    height: 56px;
	cursor: pointer;
    color:rgb(191,203,217);
    font-size:18px;
}
</style>
