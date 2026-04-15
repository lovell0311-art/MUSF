const EPermiss = {
    dashboard_admin: 1, // 系统首页 admin
    dashboard_proxy: 2, // 系统首页 proxy
    menu_game_manager:100, // 游戏管理
    search_role:103,  // 搜索角色
    search_account:104, // 搜索账号
    server_manager:105, // 服务器管理
    cdkey:106,  // 兑换码
    menu_setting:200, // 设置
    user_manager:201, // 添加用户
    config_upload:202, // 配置更新

    player_view:1000, // 查看玩家
    account_view:1100, // 查看账号
    account_info_view:1101, // 玩家信息查看
    account_log_view:1102, // 玩家日志查看
    account_log_pay_view:1103, // 玩家日志查看

    role_view:1200, // 查看角色
    role_info_view:1201, // 查看角色信息
    role_equipment_view:1202, // 查看角色装备
    role_backpack_view:1203, // 查看角色背包
    role_warehouse_view:1204, // 查看角色仓库
    role_skill_view:1205, // 查看角色技能
    role_mail_view:1206, // 查看角色邮件
    role_pet_view:1207, // 查看角色宠物
    role_title_view:1208, // 查看角色称号


    modify_xyuid: 2100, // 修改xyuid
    modify_channel_id: 2101, // 修改渠道id
    ban_account: 2102, // 封禁账号
    offlin_account: 2103, // 下线账号
    modify_phone: 2104, // 修改手机号
    modify_identity: 2105, // 修改账号类型
    modify_password: 2106, // 修改密码
    modify_idcard: 2107, // 修改实名信息

    edit_item: 2200, // 编辑物品
    send_mail: 2201, // 发送邮件
    modify_role_data: 2202, // 修改角色数据
    modify_role_title: 2203, // 修改角色称号

}


const permiss2IntList = {
	"root" : [
        EPermiss.dashboard_admin,
        EPermiss.menu_game_manager,
        EPermiss.search_role,
        EPermiss.search_account,
        EPermiss.server_manager,
        EPermiss.cdkey,
        EPermiss.menu_setting,
        EPermiss.user_manager,
        EPermiss.config_upload,

        EPermiss.player_view,
        EPermiss.account_view,
        EPermiss.account_info_view,
        EPermiss.account_log_view,
        EPermiss.account_log_pay_view,

        EPermiss.role_view,
        EPermiss.role_info_view,
        EPermiss.role_equipment_view,
        EPermiss.role_backpack_view,
        EPermiss.role_warehouse_view,
        //EPermiss.role_skill_view,
        EPermiss.role_mail_view,
        //EPermiss.role_pet_view,
        EPermiss.role_title_view,

        EPermiss.modify_xyuid,
        EPermiss.modify_channel_id,
        EPermiss.ban_account,
        EPermiss.offlin_account,
        EPermiss.modify_phone,
        EPermiss.modify_identity,
        EPermiss.modify_password,
        EPermiss.modify_idcard,

        EPermiss.edit_item,
        EPermiss.send_mail,
        EPermiss.modify_role_data,
        EPermiss.modify_role_title,
    ],
	"admin" : [
        EPermiss.dashboard_admin,
        EPermiss.menu_game_manager,
        EPermiss.search_role,
        EPermiss.search_account,
        EPermiss.server_manager,
        EPermiss.cdkey,
        EPermiss.menu_setting,
        EPermiss.user_manager,
        EPermiss.config_upload,

        EPermiss.player_view,
        EPermiss.account_view,
        EPermiss.account_info_view,
        EPermiss.account_log_view,
        EPermiss.account_log_pay_view,

        EPermiss.role_view,
        EPermiss.role_info_view,
        EPermiss.role_equipment_view,
        EPermiss.role_backpack_view,
        EPermiss.role_warehouse_view,
        //EPermiss.role_skill_view,
        EPermiss.role_mail_view,
        //EPermiss.role_pet_view,
        EPermiss.role_title_view,

        EPermiss.modify_xyuid,
        EPermiss.modify_channel_id,
        EPermiss.ban_account,
        EPermiss.offlin_account,
        EPermiss.modify_phone,
        EPermiss.modify_identity,
        EPermiss.modify_password,
        EPermiss.modify_idcard,

        EPermiss.edit_item,
        EPermiss.send_mail,
        EPermiss.modify_role_data,
        EPermiss.modify_role_title,

    ],
	"gm":[
        EPermiss.dashboard_admin,
        EPermiss.search_role,
        EPermiss.search_account,
        EPermiss.menu_game_manager,
        EPermiss.search_account,

        EPermiss.player_view,
        EPermiss.account_view,
        EPermiss.account_info_view,
        EPermiss.account_log_view,
        EPermiss.account_log_pay_view,

        EPermiss.role_view,
        EPermiss.role_info_view,
        EPermiss.role_equipment_view,
        EPermiss.role_backpack_view,
        EPermiss.role_warehouse_view,
        EPermiss.role_skill_view,
        EPermiss.role_mail_view,
        EPermiss.role_pet_view,
        EPermiss.role_title_view,



        EPermiss.modify_xyuid,
        EPermiss.modify_channel_id,
        EPermiss.ban_account,
        EPermiss.offlin_account,
        EPermiss.modify_phone,
        EPermiss.modify_identity,
        EPermiss.modify_password,
        EPermiss.modify_idcard,


        EPermiss.edit_item,
        EPermiss.send_mail,
        EPermiss.modify_role_data,
        EPermiss.modify_role_title,
    ],
    "service":[
        EPermiss.search_role,
        EPermiss.search_account,
        EPermiss.menu_game_manager,
        EPermiss.search_role,
        EPermiss.search_account,

        EPermiss.player_view,
        EPermiss.account_view,
        EPermiss.account_info_view,
        EPermiss.account_log_view,
        EPermiss.account_log_pay_view,

        EPermiss.role_view,
        EPermiss.role_info_view,
        EPermiss.role_equipment_view,
        EPermiss.role_backpack_view,
        EPermiss.role_warehouse_view,
        EPermiss.role_skill_view,
        EPermiss.role_mail_view,
        EPermiss.role_pet_view,
        EPermiss.role_title_view,
    ],
	"proxy":[
        EPermiss.dashboard_proxy,
    ],
	"user":[],
};



module.exports = {
    /**
     *
     * @param {string} self 自己的权限
     * @param {string} val 需要的权限
     */
    PermissOk(self,val)
    {
        if(permiss2IntList[String(self)] == null)return false;
        var intVal = parseInt(val);
        console.log(`val ${intVal} in ${permiss2IntList[String(self)].indexOf(intVal)}`);
        return permiss2IntList[String(self)].indexOf(intVal)>=0;
    },
    /**
     *
     * @param {string} self 自己的权限
     * @param {string} valList 需要的任意权限
     */
    PermissOkAny(self,valList)
    {
        if(permiss2IntList[String(self)] == null)return false;
        return !(permiss2IntList[String(self)].every(item=>{
            return valList.indexOf(item) < 0;
        }));
    },
    EPermiss:EPermiss
};