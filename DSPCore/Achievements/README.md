# 成就策略

Achievements 模块让模组声明“我是否要求禁用成就”，然后由 DSPCore 汇总所有模组的声明，统一处理原版异常检查、本地成就变更、Milky Way / 排行榜上传，以及 Steam / RAIL / XGP 平台成就调用。

## 这个模块带来什么便利

- 你只需要声明自己的模组 GUID 和 `disableAchievements`，不用自己 patch `AchievementSystem`、`GameAbnormalityData_0925`、`MilkyWayWebClient`、`STEAMX`、`RAILX` 或 `XGPX`。
- 多个模组同时存在时，你不需要和其他模组协调成就策略；DSPCore 会用一个全局策略处理冲突。
- DSPCore 会把“是否禁用成就”同时应用到异常检查、成就解锁/进度、上传和平台元数据调用，避免每个模组各自补一套不一致的 patch。

## 怎么使用

在插件 `Awake` 或功能注册阶段调用一次：

```csharp
using DSPCore;

Achievements.Declare("com.example.my-mod", disableAchievements: true);
```

`modGuid` 建议使用稳定的 BepInEx 插件 GUID。`disableAchievements` 表示你的模组是否要求 DSPCore 全局禁用成就。

## 调用后 DSPCore 会怎么处理

DSPCore 会按 `modGuid` 记录声明；同一个 `modGuid` 后一次声明会覆盖前一次声明。

运行时会计算所有声明：

- 只要任意模组声明 `disableAchievements: true`，最终策略就是禁用成就。
- 没有任何模组声明 true 时，最终策略就是不禁用成就。

最终策略会影响这些游戏行为：

- 原版异常检查：未禁用成就时，DSPCore 会屏蔽异常检查并清理异常运行时数据；已禁用成就时，不再屏蔽异常检查。
- 本地成就变更：已禁用成就时，阻断 `AchievementSystem.UnlockAchievement` 和 `SetAchievementProgress`。
- 上传：已禁用成就时，阻断 Milky Way 上传和 Steam 排行榜上传。
- 平台调用：已禁用成就时，阻断 Steam / RAIL / XGP 的平台成就和平台元数据调用。

## 常见情况

- 不调用 `Achievements.Declare(...)`：你的模组不提出禁用请求。只要其他模组也没有声明 true，DSPCore 会保持成就可用，并屏蔽原版异常检查。
- 声明 `disableAchievements: false`：明确表示你的模组不要求禁用成就。行为与不声明基本相同，但这条声明会进入 `GetDeclarations()` 返回的快照。
- 声明 `disableAchievements: true`：要求全局禁用成就。任意一个模组这样声明后，DSPCore 都会阻断成就变更、上传和平台调用。
- 多个模组同时声明：任意 true 胜出；false 不能抵消其他模组的 true。
- 同一个 GUID 重复声明：后一次覆盖前一次。不要在每帧 `Update` 中重复声明。

## 不提供什么

- 不提供“只禁用上传但保留平台成就”之类的独立开关。
- 不允许每个模组各自决定一套成就 patch 结果；DSPCore 只执行一个汇总后的全局策略。
- 不替你判断模组是否应该禁用成就；这个判断仍由模组作者根据自己的玩法影响声明。

## 示例

- `Examples/AchievementPolicy.md`
- `Examples/AchievementPolicyExample.cs`
