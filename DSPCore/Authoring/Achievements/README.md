# 成就策略

Achievements 模块让模组声明“我是否要求阻止竞争性上传”，然后由 DSPCore 汇总所有模组的声明，统一处理原版异常检查、Milky Way / 排行榜上传，以及 Steam / RAIL / XGP 平台成就调用。

## 这个模块带来什么便利

- 你只需要声明自己的模组 GUID 和是否阻止竞争性上传，不用自己 patch `GameAbnormalityData_0925`、`MilkyWayWebClient`、`STEAMX`、`RAILX` 或 `XGPX`。
- 多个模组同时存在时，你不需要和其他模组协调成就策略；DSPCore 会用一个全局策略处理冲突。
- DSPCore 会始终屏蔽原版异常检查以保留本地成就；有模组声明阻止竞争性上传时，只阻断 Milky Way / Steam 排行榜这类上传。

## 怎么使用

在插件 `Awake` 或功能注册阶段调用一次：

```csharp
using DSPCore;

Achievements.BlockCompetitiveUpload("com.example.my-mod");
```

`modGuid` 建议使用稳定的 BepInEx 插件 GUID。`BlockCompetitiveUpload(...)` 表示你的模组要求 DSPCore 阻止 Milky Way / 排行榜上传。

## 调用后 DSPCore 会怎么处理

DSPCore 会按 `modGuid` 记录声明；同一个 `modGuid` 后一次声明会覆盖前一次声明。

运行时会计算所有声明：

- 只要任意模组声明阻止竞争性上传，最终策略就是阻止 Milky Way / 排行榜上传。
- 没有任何模组声明阻止时，最终策略就是允许上传。

最终策略会影响这些游戏行为：

- 原版异常检查：DSPCore 始终屏蔽异常检查并清理异常运行时数据，让本地成就保持可用。
- 本地成就变更：DSPCore 不阻断 `AchievementSystem.UnlockAchievement` 和 `SetAchievementProgress`。
- 上传：有任意模组声明阻止竞争性上传时，阻断 Milky Way 上传和 Steam 排行榜上传。
- 平台调用：DSPCore 不阻断 Steam / RAIL / XGP 的平台成就和平台元数据调用。

## 常见情况

- 不调用 `Achievements.BlockCompetitiveUpload(...)`：你的模组不提出上传阻断请求。
- 调用 `Achievements.BlockCompetitiveUpload(modGuid)`：要求阻止竞争性上传。任意一个模组这样声明后，DSPCore 都会阻断 Milky Way / 排行榜上传。
- 调用 `Achievements.BlockCompetitiveUpload(modGuid, blockUpload: false)`：明确表示你的模组不要求阻止上传。行为与不声明基本相同，但这条声明会进入 `GetDeclarations()` 返回的快照。
- 多个模组同时声明：任意 true 胜出；false 不能抵消其他模组的 true。
- 同一个 GUID 重复声明：后一次覆盖前一次。不要在每帧 `Update` 中重复声明。

`Achievements.Declare(modGuid, disableAchievements)` 和 `AchievementPolicyDeclaration.DisableAchievements` 作为兼容入口保留；在当前策略中，这个 bool 表示“是否阻止竞争性上传”，不再表示阻断本地或平台成就。

## 不提供什么

- 不提供“阻断本地成就但允许上传”之类的独立开关。
- 不允许每个模组各自决定一套成就 patch 结果；DSPCore 只执行一个汇总后的全局策略。
- 不替你判断模组是否应该阻止竞争性上传；这个判断仍由模组作者根据自己的玩法影响声明。

## 示例

- `Examples/AchievementPolicy.md`
- `Examples/AchievementPolicyExample.cs`
