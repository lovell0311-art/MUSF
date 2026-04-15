<template>
    <div class="index">
        <LeftMenu></LeftMenu>
        <div class="content-box" :class="{ 'content-collapse': sidebar.collapse }">
		    <div class="content">
                <router-view />
            </div>
        </div>
    </div>
    
</template>

<script setup>
import LeftMenu from "../components/LeftMenu"
import {useSidebarStore} from '../store/sidebar'

const sidebar = useSidebarStore();

</script>

