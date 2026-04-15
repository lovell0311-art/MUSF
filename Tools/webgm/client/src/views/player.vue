<template>
    <div class="index">
        <PlayerLeftMenu />
        <div class="content-box" :class="{ 'content-collapse': sidebar.collapse }">
		    <div class="content">
                <router-view v-slot="{ Component }">
                    <transition name="move" mode="out-in">
                        <keep-alive include="[]">
                            <component :is="Component"></component>
                        </keep-alive>
                    </transition>
                    
                </router-view>
            </div>
        </div>
    </div>
    
</template>

<script>
import PlayerLeftMenu from "../components/PlayerLeftMenu"
import {useSidebarStore} from '../store/sidebar'

export default{
    components:{
        PlayerLeftMenu
    },
    setup(){
        const sidebar = useSidebarStore();
        return {
            sidebar
        }
    },
    data(){
        return{

        }
    }
}

</script>

