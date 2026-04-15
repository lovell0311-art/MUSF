# MU手游二开记录

## 1. 文档目的

本文档用于整理 `MUSF` 项目截至目前的二开、部署、联调、打包、真机测试、UI 克隆与问题排查过程。

重点说明：

- 服务端从“可启动”到“可联机”的配置过程
- 安卓客户端从“可打包”到“可真机联调”的完整排查过程
- 使用 `20260310.apk` 做界面/UI 对齐时遇到的问题与解决思路
- 当前仍未完全收敛的问题与后续建议

本文档不是最终发行说明，而是项目技术排障与二开经验记录。

---

## 2. 项目结构与关键目录

根目录：`F:\MUSF`

主要目录：

- `F:\MUSF\Client`
  客户端 Unity 工程、Android 打包、热更代码、资源包
- `F:\MUSF\Server`
  服务端工程、配置、运行文件、日志、发布目录
- `F:\MUSF\Server\Release\Android\StreamingAssets`
  真正对外给客户端拉取热更资源的目录
- `F:\MUSF\Release_Test\Android\StreamingAssets`
  中间构建输出目录，不等于真实发布目录
- `F:\MUSF\Tools`
  nginx、webgm、Unity 打包日志、UI 克隆中间工具
- `F:\MUSF\20260310.apk`
  用于参考和提取 UI 素材的目标 APK
- `F:\MUSF\开服部署文档.md`
  原始部署文档
- `F:\MUSF\提取APP素材.txt`
  提取 APK 素材的说明

---

## 3. 阶段性工作总览

本次二开大致分为 6 个阶段：

1. 服务端基础环境恢复
2. 文件服、GM、Nginx、Mongo 与游戏主进程联通
3. 客户端 Android 打包链路恢复
4. 真机联调登录、选服、选角、进图
5. 以 `20260310.apk` 为参考进行 UI 克隆
6. 修正 UI 克隆后带来的资源依赖与显示问题

---

## 4. 服务端部分整理

### 4.1 初始状态与问题

最开始部署完成后，基础配套服务与游戏主服务状态不一致：

- MongoDB、webgm、nginx 基本可用
- 游戏主服务部分端口未监听
- 文件服未成功启动
- 多处配置仍保留旧内网 IP 或旧外网地址

主要症状：

- `65001` 没开
- `8080/8088` 文件服不可用
- `Realm/Gate/Game/DB` 等端口未全部监听
- 数据库和服务端配置中混用了多个历史地址

### 4.2 服务端配置调整

做过的核心调整：

- 将服务端配置统一为当前测试环境可用地址
- 把 Mongo 地址、服务监听地址、外网下发地址重新梳理
- 清理旧的 `192.168.11.x`、历史公网 IP 等残留配置

关键文件：

- `F:\MUSF\Server\Config\StartUpConfig\StartUp_ServerConfig.json`
- `F:\MUSF\Server\Config\StartUpConfig\Server_DataConfig.json`
- `F:\MUSF\Server\Config\NewConfig`
- `F:\MUSF\Server\运行.bat`

### 4.3 文件服恢复

文件服最早无法启动，原因是运行时依赖缺失：

- `FileServer.dll` 依赖 `Microsoft.AspNetCore.App 2.1.1`
- 机器上只有较新的 ASP.NET Core 运行环境

处理方式：

- 安装 `ASP.NET Core Runtime 2.1`
- 验证 `8080` 与 `8088` 监听
- 验证 `http://127.0.0.1:8080` 与 `http://127.0.0.1:8088` 返回正常

### 4.4 服务端完整启动

在修正配置后，主服务成功监听了以下关键端口：

- 外网 UDP：
  - `10002`
  - `10004-10008`
- 内网 TCP：
  - `20002`
  - `20004-20008`
  - `20010`
  - `20011`
  - `20021`
  - `20026`
  - `20028`
  - `30257`
  - `30259`
  - `30262`
  - `30278`
  - `30279`
  - `30280`
  - `65001`

### 4.5 GM 与网页管理

GM 相关服务恢复步骤：

- 确认 Node 后端监听 `5000`
- 确认 nginx 反代 `6013`
- 修正 GM 前端入口路由
- 修正首页空白页与缓存问题

关键目录：

- `F:\MUSF\Tools\webgm`
- `F:\MUSF\Tools\webgm\config\db.js`

网页入口：

- `http://127.0.0.1:6013`
- 局域网可用时也可通过 `http://10.10.10.192:6013`

### 4.6 账号数据问题

服务端账号系统最初存在两个典型问题：

1. 客户端和服务端曾一度都把账号当手机号处理
2. 密码 MD5 大小写不一致

最终处理：

- 放开字母账号登录限制
- 修正 Realm 登录处理
- 把 `blaike / 123456` 调整成客户端当前提交格式可通过的状态

---

## 5. 客户端开发与二开重点

这一部分是本次二开中最复杂、最容易反复踩坑的环节。

### 5.1 Unity 打包环境恢复

#### 5.1.1 编辑器与模块问题

客户端最初无法直接出安卓包，主要问题有：

- 本机缺 Unity 编辑器
- 缺 Android Build Support
- Unity License 未激活
- China 版 Unity 与 Android SDK/NDK 识别存在兼容问题

最终采取的处理路线：

- 安装 Unity China 版 `2020.3.49f1c1`
- 安装 Android Build Support
- 安装/配置 JDK、Android SDK、NDK
- 调整构建脚本适配 China 版编辑器

#### 5.1.2 SDK/NDK 识别问题

遇到的典型问题：

- `platform-tools version 0.0`
- `Failed to update Android SDK`
- 外部 NDK 路径被判定无效
- China 版 Unity 对 NDK 版本要求非常苛刻

最终收敛方案：

- `cmdline-tools` 退回到 Unity 可识别的版本
- 选用与当前编辑器兼容的 NDK 版本
- 调整批量构建脚本，改用 Unity 认可的配置写入方式

相关文件：

- `F:\MUSF\Client\Unity\Assets\Editor\BatchBuildAndroid.cs`
- `F:\MUSF\Client\Unity\Assets\Editor\BuildHelp\ProjectBuildForAndroid.cs`
- `F:\MUSF\Client\Unity\ProjectSettings\ProjectVersion.txt`

### 5.2 APK 内置原生库问题

#### 5.2.1 `libkcp.so` 缺失

这是安卓客户端联机阶段最关键的一次排查。

现象：

- 服务端 UDP 握手正常
- `pktmon` 已经证实：
  - 手机 `SYN` 到了服务器
  - 服务器 `ACK` 回到了手机
- 但客户端仍然报 `10060`

最终通过真机 `adb logcat` 抓到根因：

- `System.DllNotFoundException: kcp`
- `libkcp.so not found`

结论：

- 网络并没有真正不通
- 客户端收到 Realm 回包后，在本地创建 KCP 实例时失败
- 所以后续协议流程无法继续，表现成超时

处理方式：

- 把 `libkcp.so` 正确打进安卓包
- 重新签名并安装

这一步是从“假网络问题”定位成“真实客户端打包问题”的典型案例。

### 5.3 热更资源与文件服问题

#### 5.3.1 首次卡在 `0B/0B`

最早安卓首装会卡在：

- “正在为您检查/更新游戏资源… 0B/0B 0%”

原因不是一个，而是多层叠加：

1. 文件服没有正确发布安卓热更目录
2. 客户端读取 `Version.txt` 时路径命中错误
3. Android 对 `http://` 明文请求有限制

做过的修复：

- 把客户端 `StreamingAssets` 发布到文件服真实目录
- 补齐 `AndroidManifest.xml` 中的 `android:usesCleartextTraffic="true"`
- 修正首装校验逻辑，不再错误把 APK 内资源视为已完整就绪

关键文件：

- `F:\MUSF\Client\Unity\Assets\Model\Module\AssetsBundle\BundleDownloaderComponent.cs`
- `F:\MUSF\Client\Unity\Assets\Plugins\Android\AndroidManifest.xml`
- `F:\MUSF\Server\Release\Android\StreamingAssets\Version.txt`

#### 5.3.2 `Version.txt` BOM 问题

这是一次非常典型的“文件内容看着对、客户端却不认”的问题。

现象：

- 文件服能正常返回 `Version.txt`
- 手机上也能打开
- 但客户端热更解析失败

根因：

- `Version.txt` 被写成了带 BOM 的 UTF-8
- 客户端解析 JSON 时遇到了不可见字符 `﻿`

处理方式：

- 改成无 BOM 编码重新发布
- 真机强制拉取新版本

经验：

- 热更描述文件不能只看“浏览器能打开”
- 一定要确认编码格式

### 5.4 登录、选服、选角、进图联调

#### 5.4.1 登录链路排查方法

本项目登录问题的排查不能只靠客户端弹窗，要同时看：

- 真机 `adb logcat`
- 服务端 Realm 日志
- 必要时抓 UDP 包

关键方法：

1. `pktmon` 验证 UDP 是否到达服务端
2. `adb logcat` 抓 Unity/KCP/Session 错误
3. Realm 日志确认账号查询、密码校验、Gate 下发是否成功

这样才能区分：

- 真网络问题
- 客户端 KCP 初始化失败
- 服务端 Realm/DB 问题
- Gate 后续会话断开

#### 5.4.2 `10060` 与 `102008`

这两个错误在实际过程中都曾频繁出现。

`10060` 含义：

- 初看像网络超时
- 实际曾对应过多种原因：
  - UDP 被怀疑拦截
  - `libkcp.so` 缺失
  - Realm 未启动

`102008` 含义：

- 客户端统一显示为“连接被对端断开”
- 实际根因曾包括：
  - Realm 查询异常
  - 账号不存在
  - 密码不匹配
  - 登录链路某一步会话被主动关闭

经验：

- 客户端错误码不一定等于真实根因
- 必须同时对服务端日志

#### 5.4.3 选服界面空白

曾出现过“进到海面场景但没有区服和线路”的问题。

原因主要有两类：

1. 客户端选服页初始化没触发
2. 服务端大区/线路返回为空，或客户端没真正拉到新热更

做过的修复：

- 给选服页加首次兜底逻辑
- 在服务端 Gate 侧加临时线路兜底
- 强制重新发布 `code.unity3d`
- 真机清缓存/重新拉热更

#### 5.4.4 选角异常

选角阶段出现过多种问题：

- 角色显示不全
- 点击角色没反应
- 点击“开始”卡很久
- 进入场景后显示异常

其中一次关键根因是 `ILRuntime` 兼容问题：

- `Bounds.Encapsulate(Bounds)`
- `CapsuleCollider.center`

这些在热更环境下会导致选角逻辑中断，进而表现为：

- `roleEntityList` 没建立完整
- 点角色不响应
- 点“开始”后不进入游戏

处理方式：

- 在 `UIChooseRoleComponent.cs` 中避开 ILRuntime 不兼容调用
- 重新编译热更代码

### 5.5 APK 资源与热更资源是两套来源

这是整个客户端二开里最容易出大事故的一点。

结论：

- 游戏启动早期、登录页、部分核心资源，优先从 APK 自带 `assets` 读取
- 并不是所有资源都会优先走手机外置热更目录
- 所以只改 `F:\MUSF\Server\Release\Android\StreamingAssets` 并不一定能立刻影响登录页

从日志可以明确看到：

- `LoadOneBundle path [streaming] jar:file:///.../base.apk!/assets/...`

这意味着：

- 如果登录页、首屏、代码包有问题，很多时候必须重打 APK
- 不能只依赖“把资源推到文件服/手机缓存”

### 5.6 `StreamingAssets.manifest` 的重要性

这是后期 UI 克隆阶段踩得最深的坑之一。

现象：

- 登录页变成白板
- 预制体结构还在
- 文本、Toggle、根节点能创建
- 但背景、输入框、按钮贴图大量丢失

根因：

- 全局 `StreamingAssets.manifest` 被错误覆盖
- 清单中依赖记录不完整
- `uilogin.unity3d` 的依赖包没有按预期加载

经验：

- 单独替换 `code.unity3d`、`config.unity3d` 或某几个 UI 包后
- 一定要确认全局 `StreamingAssets.manifest` 仍然完整可用

---

## 6. 使用 `20260310.apk` 做 UI 对齐的过程

### 6.1 可行性评估

对 `20260310.apk` 做素材提取后，发现：

- 与当前工程高度同源
- 大量资源包同名
- UI 资源迁移可行
- 不适合直接全量替换所有逻辑相关包

所以采用了“安全混合”的迁移策略：

- 优先对齐 UI/界面资源
- 保留当前可运行的登录、选服、选角、角色、代码包

### 6.2 UI 克隆的实际策略

做法不是直接全替换，而是分层处理：

1. 先比对目标 APK 与当前包的资源命名和尺寸
2. 筛出 UI 相关资源
3. 对关键逻辑包做保留名单
4. 只覆盖安全 UI 资源
5. 再逐步验证登录页、选服页、角色页、HUD

相关脚本：

- `C:\Users\ZM\Documents\New project\compare_musf_ui_assets.ps1`
- `C:\Users\ZM\Documents\New project\apply_musf_ui_clone.ps1`
- `C:\Users\ZM\Documents\New project\repack_musf_test_ui_clone.ps1`
- `C:\Users\ZM\Documents\New project\build_musf_test_ui_clone_safehybrid_fullassets.ps1`

### 6.3 真实发布目录与构建目录不一致

这一点导致过一次很大的误判。

中间构建目录：

- `F:\MUSF\Release_Test\Android\StreamingAssets`

真实发布目录：

- `F:\MUSF\Server\Release\Android\StreamingAssets`

如果只更新前者而不更新后者，就会出现：

- 本地看资源已替换
- 真机实际上还在拉旧资源

经验：

- 所有最终对外测试都应以 `F:\MUSF\Server\Release\Android\StreamingAssets` 为准

### 6.4 白板登录页问题

这是 UI 克隆阶段最典型的资源依赖错配问题。

表现：

- 标题在
- 底部协议文字在
- 大白板在
- 输入框与按钮背景不正常

本质：

- `uilogin.unity3d` 本体存在
- 但依赖的 `ui_common_bgss`、`ui_common_btnss`、`ui_common_inputfilledbgs`、`ui_common_textss`、`ui_common_togbgs`、`ui_logins` 资源族错配或清单引用异常

处理思路：

- 对登录资源族整体回退，而不是只换单个 `uilogin.unity3d`
- 必须连同依赖包和对应 `.manifest` 一起回退
- 最终还要重打 APK，因为启动时优先吃 APK 资源

---

## 7. 真机联调过程中常用的方法

### 7.1 ADB

常用命令：

```powershell
C:\Android\Sdk\platform-tools\adb.exe devices
C:\Android\Sdk\platform-tools\adb.exe -s AN6B024B15003471 logcat -c
C:\Android\Sdk\platform-tools\adb.exe -s AN6B024B15003471 logcat -v time
C:\Android\Sdk\platform-tools\adb.exe -s AN6B024B15003471 shell screencap -p /sdcard/Download/test.png
C:\Android\Sdk\platform-tools\adb.exe -s AN6B024B15003471 pull /sdcard/Download/test.png .
```

### 7.2 Windows 侧端口确认

```powershell
netstat -ano -p udp | Select-String ':10002|:10004|:10005|:10006|:10007|:10008'
netstat -ano -p tcp | Select-String ':8080|:8088|:58030|:5000|:6013'
```

### 7.3 UDP 包到达验证

使用 `pktmon` 验证：

- 手机是否真的把 UDP 发到了服务器
- 服务器是否真的回了 ACK

这一步对于区分“真网络问题”和“客户端本地 KCP 初始化失败”非常关键。

---

## 8. 目前已修改过的重要客户端文件

以下是二开过程中涉及过的典型关键文件：

- `F:\MUSF\Client\Unity\Assets\Hotfix\Miracle_MU\UI\UILogin\Component\UILoginComponent.cs`
- `F:\MUSF\Client\Unity\Assets\Hotfix\Miracle_MU\UI\UISelectServer\UISelectServerComponent.cs`
- `F:\MUSF\Client\Unity\Assets\Hotfix\Miracle_MU\UI\UIChooseRole\UIChooseRoleComponent.cs`
- `F:\MUSF\Client\Unity\Assets\Hotfix\Miracle_MU\Handlers\Login\G2C_LoadingComplete_notice_Handler.cs`
- `F:\MUSF\Client\Unity\Assets\Model\Module\AssetsBundle\BundleDownloaderComponent.cs`
- `F:\MUSF\Client\Unity\Assets\Model\Helper\BundleHelper.cs`
- `F:\MUSF\Client\Unity\Assets\Plugins\Android\AndroidManifest.xml`
- `F:\MUSF\Client\Unity\Assets\Editor\BatchBuildAndroid.cs`
- `F:\MUSF\Client\Unity\Assets\Editor\BatchBuildCodeBundle.cs`

---

## 9. 当前项目状态

### 9.1 已确认跑通的部分

- 服务端基础环境已可正常启动
- Mongo、nginx、webgm、文件服都可运行
- Realm/Gate/Game/DB 主链路已拉起
- 安卓 APK 可成功打包并安装到真机
- 真机登录、选服、选角、进图曾经都跑通过
- 使用 `20260310.apk` 做 UI 对齐是可行的

### 9.2 当前仍未完全收敛的问题

截至本文档编写时，客户端仍存在以下未完全稳定的问题：

1. UI 克隆后的登录资源族仍容易被错误覆盖
2. 选角页和进图后的场景/相机/显示问题曾多次反复
3. “点击开始后卡很久再进游戏”说明加载链路仍需继续细查
4. “进游戏后显示不正常”说明场景资源、相机跟随或渲染相关资源仍需进一步核对

换句话说：

- 项目已经从“完全不能跑”走到“可启动、可打包、可真机联调、可部分进图”
- 但距离“稳定、完整、视觉一致”的成品状态还需要继续收口

---

## 10. 后续开发建议

### 10.1 客户端必须分稳定线与实验线

建议至少保留两套发布：

- 稳定可联机包
- UI 克隆实验包

不要在唯一真机测试包上直接不断叠加实验改动。

### 10.2 每次替换资源都做资源族级别备份

不能只备份单个包。

例如登录页必须把以下资源作为一个整体备份：

- `uilogin`
- `ui_logins`
- `ui_common_bgss`
- `ui_common_btnss`
- `ui_common_inputfilledbgs`
- `ui_common_textss`
- `ui_common_togbgs`

### 10.3 每次重打包都验证 3 个层级

1. APK 内 `assets` 是否正确
2. 文件服发布目录是否正确
3. 真机本地缓存是否已切到新版本

只看其中一个层级很容易误判。

### 10.4 优先保证逻辑可用，再做视觉 1:1 还原

对于这类老项目二开，不建议一开始就追求：

- UI、场景、角色、特效、逻辑全部同时替换

正确顺序应当是：

1. 登录可用
2. 选服可用
3. 选角可用
4. 进图可用
5. 再逐页对齐视觉

### 10.5 真机永远比模拟器更重要

本次已经证明：

- 模拟器能跑不代表真机能跑
- 真机上的 `libkcp.so`、清单、输入法、系统安全策略、ADB 安装确认都可能产生独有问题

所以最终验证必须以真机为准。

---

## 11. 一句话结论

本项目的二开已经完成了最难的前半段：服务端恢复、安卓打包、真机联调、UI 克隆可行性验证都已经做通。

当前最需要继续投入的方向，不再是“能不能跑起来”，而是：

- 把客户端资源族管理做稳定
- 把登录页、选角页、进图后的显示问题逐步收口
- 在保证逻辑稳定的前提下，继续向 `20260310.apk` 的界面与体验靠拢

