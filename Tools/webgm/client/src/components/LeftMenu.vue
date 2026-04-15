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
                                <el-menu-item v-for="(threeItem, i) in subItem.subs" :key="i" :index="threeItem.index" v-permiss="threeItem.permiss">
                                    {{ threeItem.title }}
                                </el-menu-item>
                            </el-sub-menu>
                            <el-menu-item v-else :index="subItem.index" v-permiss="subItem.permiss">
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
import { useRoute } from 'vue-router';
import { onMounted } from 'vue';
import {usePermissStore} from '@/store/permiss'
import { EPermiss } from '../../../common/permiss';

export default{
    name:"leftmenu",
    setup(){
        const sidebar = useSidebarStore();

        const route = useRoute();
        const onRoutes = computed(() => {
            return route.path;
        });

        onMounted(()=>{
            if (document.body.clientWidth < 1500) {
                sidebar.collapse = true;
            }
        });

        return {
            sidebar,
            onRoutes
        };
    },
    data(){
        return {
            items: [
                {
                    icon: 'Odometer',
                    index: '/dashboard_admin',
                    title: '系统首页',
                    permiss: EPermiss.dashboard_admin,
                },
                {
                    icon: 'Odometer',
                    index: '/dashboard_proxy',
                    title: '系统首页',
                    permiss: EPermiss.dashboard_proxy,
                },
                {
                    icon: 'Bicycle',
                    index: '1',
                    title: '游戏管理',
                    permiss: EPermiss.menu_game_manager,
                    subs: [
                        {
                            index: '/search_role',
                            title: '搜索角色',
                            permiss: EPermiss.search_role,
                        },
                        {
                            index: '/search_account',
                            title: '搜索账号',
                            permiss: EPermiss.search_account,
                        },
                        {
                            index: '/server_manager',
                            title: '服务器管理',
                            permiss: EPermiss.server_manager,
                        },
                        {
                            index: '/cdkey_type',
                            title: '兑换码',
                            permiss: EPermiss.cdkey,
                        }
                    ],
                },
                {
                    icon: 'Setting',
                    index: '2',
                    title: '设置',
                    permiss: EPermiss.menu_setting,
                    subs: [
                        {
                            index: '/user_manager',
                            title: '用户管理',
                            permiss: EPermiss.user_manager,
                        },
                        {
                            index: '/config_upload',
                            title: '配置更新',
                            permiss: EPermiss.config_upload,
                        }
                    ],
                }
            ],

        };
    },
    methods:{
        collapseChage(){
            // 侧边栏折叠
            this.sidebar.handleCollapse();
        }
    },
    created(){
    }
    
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
