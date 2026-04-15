using System;
using System.Collections.Generic;

namespace ETHotfix
{  
	/// <summary>
	/// UI面板类型
	/// </summary>
    public static class UIType
    {
	    public const string Root = "Root";
		public const string UILobby = "UILobby";
		public const string UIAddEquipMents = "UIAddEquipMents";//测试添加装备


		public const string UIChouJiang = "UIChouJiang";//抽奖
		/// <summary>
		/// 提示面板
		/// </summary>
		public const string UIHint = "UIHint";
		/// <summary>
		/// 公告面板
		/// </summary>
		public const string UIAnnouncement = "UIAnnouncement";

		//用户协议
		public const string UI_UserAgreement = "UI_UserAgreement";


		/// <summary>
		/// 登录面板
		/// </summary>
	    public const string UILogin = "UILogin";
		/// <summary>
		/// 登录面板
		/// </summary>
	    public const string UILevelTopUp = "UILevelTopUp";
		//XYSDK登录
	    public const string UILogin_XYSDK = "UILogin_XYSDK";
	   /// <summary>
		/// 提示面板
		/// </summary>
	    public const string UIConfirm = "UIConfirm";
		/// <summary>
		///好友请求提示面板
		/// </summary>
		public const string UIAddFirendConfirm = "UIAddFirendConfirm";
		/// <summary>
		/// 服务器选择面板
		/// </summary>
		public const string UISelectServer = "UISelectServer";
		/// <summary>
		/// 角色选择面板
		/// </summary>
	    public const string UIChooseRole = "UIChooseRole";
		/// <summary>
		/// 创建角色面板
		/// </summary>
	    public const string UICreatRole = "UICreatRole";
		/// <summary>
		/// 场景加载面板
		/// </summary>
	    public const string UISceneLoading = "UISceneLoading";
		/// <summary>
		/// 主界面
		/// </summary>
	    public const string UIMainCanvas = "UIMainCanvas";
		/// <summary>
		/// 大地图
		/// </summary>
	    public const string UIBigMap = "UIBigMap";
		/// <summary>
		/// 等级奖励
		/// </summary>
	    public const string UI_RankReward = "UI_RankReward";
		/// <summary>
		/// 地图选择面板
		/// </summary>
	    public const string UISceneTranslate = "UISceneTranslate";
		/// <summary>
		/// 挂机设置面板
		/// </summary>
	    public const string UIOnHookSet = "UIOnHookSet";
		/// <summary>
		/// 自定义相机视角组件
		/// </summary>
	    public const string UISetCameraAtr = "UISetCameraAtr";
		/// <summary>
		/// 游戏设置面板
		/// </summary>
	    public const string UIGameSet = "UIGameSet";
		/// <summary>
		/// 背包面板
		/// </summary>
	    public const string UIKnapsack = "UIKnapsack";
		/// <summary>
		/// 技能面板
		/// </summary>
	    public const string UISkill = "UISkill";
		/// <summary>
		/// 物品简介面板
		/// </summary>
	    public const string UIIntroduction = "UIIntroduction";
		/// <summary>
		/// 聊天面板
		/// </summary>
	    public const string UIChatPanel = "UIChatPanel";
	
		/// <summary>
		/// 好友列表面板
		/// </summary>
		public const string UIFirendList = "UIFirendList"; 

		/// <summary>
		/// 其他玩家的信息面板
		/// </summary>
		public const string UISelectOtherPlayer = "UISelectOtherPlayer"; 
		/// <summary>
		/// 人物属性
		/// </summary>
		public const string UIRoleInfo = "UIRoleInfo"; 
		/// <summary>
		/// 战盟
		/// </summary>
		public const string UIWarAlliance = "UIWarAlliance"; 
		/// <summary>
		/// 组队面板
		/// </summary>
		public const string UITeam = "UITeam"; 
		/// <summary>
		/// 玩家装备面板
		/// </summary>
		public const string UIEquips = "UIEquips"; 
		/// <summary>
		/// 大师
		/// </summary>
		public const string DaShiCanvas = "DaShiCanvas";
        /// <summary>
        /// 宠物
        /// </summary>
        public const string UIPet = "UIPet";
        public const string UIPetNew = "UIPetNew";
        /// <summary>任务</summary>
        public const string UITask = "UITask"; 
		/// <summary>副本</summary>
        public const string UIFuBen = "UIFuBen";
        /// <summary>副本排行榜</summary>
        public const string UIFuBenPaiHangBang = "UIFuBenPaiHangBang"; 
        /// <summary>副本进入时间等级信息</summary>
        public const string UIActiveInfo = "UIActiveInfo"; 
		/// <summary>转职</summary>
        public const string UICareerChangePanel = "UICareerChangePanel";
		/// <summary>
		/// 邮件
		/// </summary>
		public const string UIE_Mail = "UIE_Mail";
		/// <summary>
		/// 商城
		/// </summary>
		public const string UIShop = "UIShop";
		/// <summary>
		/// 首充
		/// </summary>
		public const string UIFirstCharge = "UIFirstCharge";
		/// <summary>
		/// 百级大冲刺
		/// </summary>
		public const string UISprint = "UISprint";
		/// <summary>
		/// 藏宝阁
		/// </summary>
		public const string UITreasureHouse = "UITreasureHouse";
		/// <summary>
		/// 百级大冲刺
		/// </summary>
		public const string UINewYearActivity = "UINewYearActivity";
		/// <summary>
		/// 合成
		/// </summary>
		public const string UISynthesis = "UISynthesis";
		/// <summary>
		/// 在线奖励
		/// </summary>
		public const string UIZaiXian = "UIZaiXian";
		/// <summary>
		/// 线路切换
		/// </summary>
		public const string UIChangeLine = "UIChangeLine";
        /// <summary>
        /// 挑战Boss
        /// </summary>
        public const string UIChallenge = "UIChallenge";

        /// <summary>
        /// 七日充值
        /// </summary>
        public const string TopUp_7_Day = "UITopUp_7_Day";
        /// <summary>
        /// 九日签到
        /// </summary>
        public const string UIQianDao_9_Day = "UIQianDao_9_Day";
        /// <summary>
        /// 累计充值
        /// </summary>
        public const string UITopUpRewards = "UITopUpRewards";
		/// <summary>
		/// 限时充值
		/// </summary>
		public const string UILimitTopUp = "UILimitTopUp";
		/// <summary>
		/// 邀请
		/// </summary>
		public const string UIYaoQingMa = "UIYaoQingMa";

		/// <summary>
		/// 通行证界面
		/// </summary>
		public const string UIPassport = "UIPassport";
		/// <summary>
		/// 称号
		/// </summary>
		public const string UITitle = "UITitle";
		/// <summary>
		/// 藏宝图对话
		/// </summary>
        public const string UITreasureMap = "UITreasureMap";
		/// <summary>
		/// 五一充值活动
		/// </summary>
        public const string UI_51Active = "UI_51Active";
        public const string UI51GoldCard = "UI51GoldCard";
		//重置排行榜
		public const string UICongZhiPaiHangBang = "UICongZhiPaiHangBang";
		//少女安娜 买药
		public const string UIBuyMedicine = "UIBuyMedicine";
        //转生-训鸟师
        public const string UIZhuanSheng = "UIZhuanSheng";
        //实名认证
        public const string UIRealName = "UIRealName";
		//功能引导
		public const string UIFeatureGuide = "UIFeatureGuide";
		//
		public const string UIChaXun = "UIChaXun";
		public const string UI_LimitTopUpActivity = "UI_LimitTopUpActivity";
		//支付
		public const string UIPay = "UIPay";
		public const string UI_HUD = "UI_HUD";
		public const string UIShiLianZhiDi = "UIShiLianZhiDi";
		//抽奖五一活动
		public const string UIWelfare = "UIWelfare";
		public const string UIReclamation = "UIReclamation";

        public const string UIKnapsackNew = "UIKnapsackNew";

		public const string UIGuide = "UIGuide";
        //限购礼包
        public const string UIPurchaseLimit = "UIPurchaseLimit"; 
        //超值礼包
        public const string UITangibleLimit = "UITangibleLimit";
		//新大师
		public const string UIDaShiNew = "UIDaShiNew";
		//月卡界面
		public const string UIMonthCard = "UIMonthCard";
		//坐骑界面
		public const string UIMount = "UIMount";
		//觉醒系统
		public const string UIAwakening = "UIAwakening";
	}
}