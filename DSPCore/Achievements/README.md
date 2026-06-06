# 成就策略

## 职责

本功能块汇总所有模组的成就禁用声明，并把最终决策提供给运行时补丁。

## 公开入口

- `Api/Achievements.cs`：作者侧短入口。
- `Api/AchievementPolicyDeclaration.cs`：单个模组的 `ModGuid` + `DisableAchievements` 声明。
- `Api/AchievementPolicyRegistry.cs`：全局策略注册表。

## 示例

- `Examples/AchievementPolicy.md`
- `Examples/AchievementPolicyExample.cs`

## 运行时

`Runtime/AchievementRuntime.cs` 会 patch 异常检查、本地成就变更、Milky Way 上传、排行榜上传和平台成就/元数据调用。

规则只有一个：任意模组声明 `DisableAchievements = true` 时禁用成就相关能力；否则屏蔽游戏异常报错，并允许获取成就、上传排行榜和获取平台元数据。

## 边界

本功能块不允许每个模组各自 patch 成就，也不提供单独的上传或平台开关。模组只声明 GUID 和是否禁用成就，DSPCore 执行一个聚合策略。
