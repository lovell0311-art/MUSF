namespace ETHotfix
{
    public static partial class ErrorCodeHotfix
    {

        /// <summary>成功</summary>
        public const int ERR_Success = 0;

        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常

        /// <summary>
        /// Mongodb save error
        /// </summary>
        public const int ERR_MongodbSaveError = 110004;
        public const int ERR_MongodbUpdateError = 110005;


        

        /// <summary>
        /// 不处理 网络包认为无效
        /// </summary>
        public const int ERROR_NETPACKAGE_INVALID = 200001;
        /// <summary>
        /// 不处理 网络包认为无效
        /// </summary>
        public const int ERROR_INPUT_INFO_ERROR = 200002;


        /// <summary>
        /// 手机验证码 校验失败
        /// </summary>
        public const int ERR_SMSInspectError = 200101;
        /// <summary>
        /// 账号已存在
        /// </summary>
        public const int ERR_AccountAlreadyExists = 200102;
        /// <summary>
        /// 账号不存在
        /// </summary>
        public const int ERR_AccountNotExists = 200103;
        /// <summary>
        /// 验证码发送失败
        /// </summary>
        public const int ERR_SmsSendError = 200104;
        /// <summary>
        /// 验证码验证失败
        /// </summary>
        public const int ERR_ConnectGateKeyError = 200105;
        /// <summary>
        /// 匹配服务器出现问题
        /// </summary>
        public const int ERR_MatchServerError = 200106;

        /// <summary>
        /// 数据库代理服务器有问题
        /// </summary>
        public const int ERR_DBProxyManageNullError = 200107;

        /// <summary>
        /// 数据库代理服务器有问题
        /// </summary>
        public const int ERR_DBProxyNotFoundError = 200108;

        public const int ERR_MatchError = 200109;

        /// <summary>
        /// 目标玩家不存在
        /// </summary>
        public const int ERR_TargetPlayerNotExists = 200110;

        /// <summary>
        /// 目标组件不存在
        /// </summary>
        public const int ERR_TargetComponentNotExists = 200111;



        //=====================背包(211000-211999)======================
        /// <summary>
        /// 背包组件异常
        /// </summary>
        public const int ERR_BackpackNotFindError = 211000;
        /// <summary>
        /// 背包当前位置装不下物品
        /// </summary>
        public const int ERR_BackpackCantEnterItemError = 211001;
        /// <summary>
        /// 初始化背包物品数据有误
        /// </summary>
        public const int ERR_BackpackInitItemError = 211002;
        /// <summary>
        /// 背包找不到要删除的物品
        /// </summary>
        public const int ERR_BackpackCantDeleteItemError = 211003;
        /// <summary>
        /// 移动物品失败
        /// </summary>
        public const int ERR_BackpackCantMoveItemError = 211004;
        /// <summary>
        /// 物品配置不存在
        /// </summary>
        public const int ERR_ItemConfigIDNotFoundError = 211005;
        /// <summary>
        /// 背包获取物品失败
        /// </summary>
        public const int ERR_ItemNotFoundError = 211006;
        /// <summary>
        /// 使用物品失败：配置表没有配置对应物品的使用事件
        /// </summary>
        public const int ERR_UseMethodNotFoundError = 211007;
        /// <summary>
        /// 使用物品失败：物品不可使用
        /// </summary>
        public const int ERR_ItemCantUseError = 211008;

        //=====================装备(212000-212999)======================
        /// <summary>
        /// 背包组件异常
        /// </summary>
        public const int ERR_EquipmentFindError = 211000;
        /// <summary>
        /// 装备组件初始化失败
        /// </summary>
        public const int ERR_EquipmentInitFail = 212001;
        /// <summary>
        /// 装备穿戴失败
        /// </summary>
        public const int ERR_EquipmentLoadFail = 212002;
        /// <summary>
        /// 装备卸载失败
        /// </summary>
        public const int ERR_EquipmentUnLoadFail = 212003;
        /// <summary>
        /// 装备组件数据异常
        /// </summary>
        public const int ERR_EquipmentDBError = 212004;
        /// <summary>
        /// 装备强化失败
        /// </summary>
        public const int ERR_EquipmentStrengthenFail = 212005;
        /// <summary>
        /// 装备追加失败
        /// </summary>
        public const int ERR_EquipmentAdditionFail = 212006;
        /// <summary>
        /// 装备再生属性添加失败
        /// </summary>
        public const int ERR_EquipmentOrecycledFail = 212007;
        /// <summary>
        /// 装备再生属性进化失败
        /// </summary>
        public const int ERR_EquipmentOrecycledEvoFail = 212008;
        /// <summary>
        /// 装备再生属性还原失败
        /// </summary>
        public const int ERR_EquipmentOrecycledRestoreFail = 212009;

        //=====================聊天(213000-213999)======================
        /// <summary>
        /// 找不到聊天组件
        /// </summary>
        public const int ERR_NotFindChatManagerComponent = 213000;
        /// <summary>
        /// 不允许发送空消息
        /// </summary>
        public const int ERR_ChatMessageIsEmpty = 213001;
        /// <summary>
        /// 找不到房间
        /// </summary>
        public const int ERR_NotFindChatRoom = 213002;

        //=====================组队(214000-214999)======================
        /// <summary>
        /// 找不到聊天组件
        /// </summary>
        public const int ERR_NotFindTeamManagerComponent = 214000;
        /// <summary>
        /// 创建组队失败
        /// </summary>
        public const int ERR_CreateTeamFail = 214001;
        /// <summary>
        /// 邀请组队失败
        /// </summary>
        public const int ERR_InvitePlayerFail = 214002;
        /// <summary>
        /// 加入组队失败
        /// </summary>
        public const int ERR_JoinTeamFail = 214003;
        /// <summary>
        /// 踢出组队失败
        /// </summary>
        public const int ERR_KickThePlayerFail = 214004;
        /// <summary>
        /// 申请组队失败
        /// </summary>
        public const int ERR_ApplyPlayerFail = 214005;
        /// <summary>
        /// 回复申请失败
        /// </summary>
        public const int ERR_ReplyPlayerFail = 214006;
        /// <summary>
        /// 离开组队失败
        /// </summary>
        public const int ERR_LeaveTeamFail = 214007;
        /// <summary>
        /// 让位失败
        /// </summary>
        public const int ERR_HandCaptainFail = 214008;

        //=====================交易(215000-215999)======================
        /// <summary>
        /// 邀请交易失败
        /// </summary>
        public const int ERR_ExchangeInviteFail = 215000;
        /// <summary>
        /// 已经在交易中
        /// </summary>
        public const int ERR_ExchangeIsExist = 215001;
        /// <summary>
        /// 当前不在交易状态
        /// </summary>
        public const int ERR_ExchangeIsNotStart = 215002;
        /// <summary>
        /// 交易终止
        /// </summary>
        public const int ERR_ExchangeIsAborted = 215003;
        /// <summary>
        /// 物品处理异常
        /// </summary>
        public const int ERR_ExchangeItemError = 215004;
        /// <summary>
        /// 已锁定 无法操作
        /// </summary>
        public const int ERR_ExchangeErrorBecauseLock = 215005;


        //=====================好友(216000-216100)======================
        /// <summary>
        /// 获取好友列表失败
        /// </summary>
        public const int ERR_Friendsexist = 216000;
        ///<summary>
        ///检查到好友已经存在
        ///</summary>
        public const int ERR_CheckFriends = 216001;
        ///<summary>
        ///获取好友组件失败
        ///</summary>
        public const int ERR_GetFriendComponent = 216002;
        ///<summary>
        ///添加好友失败
        /// </summary>
        public const int ERR_ADDFriendsFail = 216003;
        ///<summary> 
        ///玩家不在线
        /// </summary>
        public const int ERR_SearchOffLine = 216004;
        ///<summary> 
        ///玩家不存在
        /// </summary>
        public const int ERR_SearchOnExistent = 216005;
        ///<summary>
        ///写入数据库失败
        /// </summary>
        public const int ERR_WriteDBfail = 216006;
        ///<summary>
        ///消息过长
        /// </summary>
        public const int ERR_MessageToLong = 216007;
        /// <summary>
        /// 好友已经在黑名单
        /// </summary>
        public const int ERR_FriendExistenceFoe = 216008;
    }
}