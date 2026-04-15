import { defineStore } from 'pinia';
import { PermissOk } from '../../../common/permiss';

export const usePermissStore = defineStore('permiss', {
	state: () => {
		return {
			identity: "user",
			channelId: "",
		};
	},
	actions: {
		handleSet(val) {
			this.identity = val;
		},
		handleSetChannelId(val) {
			this.channelId = val;
		},
		Ok(val){
			return PermissOk(this.identity,val);
		}
	},
	persist:{
        enabled: true
    }
});
