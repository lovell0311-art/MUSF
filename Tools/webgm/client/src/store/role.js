import { defineStore } from 'pinia';


export const useRoleStore = defineStore('role', {
	state: () => {
		return {
			role: false,
			name:'Unknown',
		};
	},
	getters: {},
	actions: {},
    persist:{
        enabled: true
    }
});
