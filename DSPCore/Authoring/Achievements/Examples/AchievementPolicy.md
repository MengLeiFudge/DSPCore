# AchievementPolicy

本场景用于声明你的模组是否要求 DSPCore 全局禁用成就。

## 什么时候声明 true

当你的模组会明显改变游戏平衡、跳过原版限制、生成非原版进度，或让玩家获得原版本不应获得的成就条件时，声明：

```csharp
Achievements.Declare("com.example.my-mod", disableAchievements: true);
```

调用后，只要没有后续同 GUID 覆盖为 false，DSPCore 会把全局策略切到“禁用成就”，并阻断本地成就变更、Milky Way / 排行榜上传和平台成就/元数据调用。

## 什么时候声明 false 或不声明

当你的模组只是 UI、诊断、显示增强、配置整理，或不改变成就条件时，通常可以不声明。

如果你希望明确留下“本模组不要求禁用成就”的声明，可以调用：

```csharp
Achievements.Declare("com.example.my-mod", disableAchievements: false);
```

声明 false 不会抵消其他模组的 true。它只表示你的模组自己不请求禁用，并会出现在 `GetDeclarations()` 的声明快照里。

## 参数

- `modGuid`：稳定模组 GUID，建议使用 BepInEx 插件 GUID。
- `disableAchievements`：你的模组是否要求 DSPCore 全局禁用成就。

## 调用位置

在插件 `Awake` 或功能注册阶段调用一次。不要在每帧 `Update` 中重复声明；同一个 GUID 的后一次声明会覆盖前一次。

## 你不需要自己做的事

- 不需要自行 patch 原版异常检查。
- 不需要自行 patch 成就解锁、进度、上传或平台接口。
- 不需要和其他模组手动协调谁来决定最终成就策略。

代码示例见 `AchievementPolicyExample.cs`。
