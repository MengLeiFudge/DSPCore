# 成就策略

## 职责

本功能块汇总所有模组的成就策略声明，并把最终决策提供给运行时补丁。

## 公开入口

- `Achievements`：作者侧短入口。
- `AchievementPolicyDeclaration`：单个模组的策略声明。
- `AchievementPolicyRegistry`：全局策略注册表。
- `AchievementMetadataMode`：元数据保留级别。

## 示例

- `Examples/AchievementPolicyExample.cs`

## 运行时

`Runtime/AchievementRuntime.cs` 会 patch 异常检查、本地成就变更、Milky Way 上传、排行榜上传和平台成就调用。

## 边界

本功能块不允许每个模组各自 patch 成就。模组只声明意图，DSPCore 执行一个聚合策略。
