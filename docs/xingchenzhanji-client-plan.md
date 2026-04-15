# 星辰战纪客户端实施方案

## 固定基线

- 功能源码基线：`E:\MUNEW\Client`
- 当前开发工程：`F:\MUSF\Client\Unity`
- UI 布局基线：`F:\MUSF\20260310pjzr.apk`
- UI 冻结清单：`F:\MUSF\apk-ui-baseline-manifest.json`
- Unity 版本：`2020.3.49f1c1`

## 硬约束

- 角色选择站位必须以 `E:\MUNEW\Client` 的老逻辑为准。
- 主 UI 布局不得偏离 `20260310pjzr.apk`。
- 宠物与坐骑功能以原始客户端结构为主，但需要同步当前客户端已确认的增量数据。
- 客户端所有游戏品牌统一为“星辰战纪”。
- 应用图标统一使用 `F:\MUSF\星辰战纪.png`。

## 已完成

- `ProjectSettings.productName` 已改为“星辰战纪”。
- 热更稳定目录名已改为“星辰战纪”。
- 系统公告已替换为“星辰战纪”文案。
- 主图标资源已切到 `Assets/Res/UI_FuGu/UI_Login/星辰战纪.png`。
- 登录资源目录中旧品牌素材已归档，新增：
  - `Assets/Res/UI_FuGu/UI_Login/星辰战纪login.png`
  - `Assets/Res/UI_FuGu/UI_Login/星辰战纪标题.png`
- 登录页历史标题图与宣传图已按原尺寸重绘为“星辰战纪”版本，并保留 `brandgen-20260414-0226` 备份。
- 当前选角站位已确认使用老客户端硬编码逻辑。

## 宠物与坐骑结论

- `UIPetComponent.cs`、`UIPetNewComponent.cs`、`UIMountComponent.cs` 在两边客户端中一致。
- `Assets/Bundles/Pet` 与 `Assets/Bundles/Mounts` 文件数量一致。
- `Item_MountsConfig.txt` 等坐骑配置一致。
- 当前确认有差异的核心文件只有：
  - `Assets/Res/Config/Item_PetConfig.txt`

## 已确认的宠物差异

- `Id=350018`
  - 原始基线：`PetId=101`, `ResName=Pet_ShuangTouLong_beibaoUI`
  - 当前客户端：`PetId=101`, `ResName=Pet_Dragon_beibaoUI`
- `Id=350019`
  - 原始基线：`PetId=101`, `ResName=Pet_ShuangTouLong_beibaoUI`
  - 当前客户端：`PetId=105`, `ResName=Pet_ShuangTouLong_beibaoUI`
- `Id=350020`
  - 原始基线：`PetId=101`, `ResName=Pet_ShuangTouLong_beibaoUI`
  - 当前客户端：`PetId=107`, `ResName=Pet_JiJiaShuangTouLong_beibaoUI`

## 后续执行顺序

1. 继续清理登录、启动、公告、配置中的历史品牌残留。
2. 以 `E:\MUNEW\Client` 为逻辑母版复核选角、登录、创建角色、主城进入流程。
3. 保持 UI 冻结包不动，只允许替换品牌素材与功能数据。
4. 将 `Item_PetConfig.txt` 的当前增量作为宠物同步基线保留。
5. 完成后重新构建 Android 包并做冒烟测试。

## 发布前检查

- 选角站位是否与老客户端一致。
- `uimaincanvas` / `ui_hud` / `ui_mainpanels` / `ui_skills` / `minmap` 是否仍匹配 APK 基线。
- 登录页和启动图是否完全显示“星辰战纪”。
- 宠物 `350018/350019/350020` 的显示资源与 PetId 是否正确。
- 热更目录是否写入 `星辰战纪`。
