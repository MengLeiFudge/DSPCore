# AchievementPolicy

本场景用于模组声明自己的成就禁用需求。

## 适用时机

- 模组会改变游戏平衡、跳过原版限制或生成非原版进度时，声明 `disableAchievements: true`。
- 模组只是 UI、诊断或纯显示增强时，通常声明 `disableAchievements: false` 或不声明。

## 关键参数

- `modGuid`：稳定模组 GUID，建议使用 BepInEx 插件 GUID。
- `disableAchievements`：是否要求 DSPCore 全局禁用成就。

## 运行时前提

在插件 `Awake` 或功能注册阶段调用一次。DSPCore 会汇总所有声明；只要任意声明要求禁用，最终策略就是禁用成就。

## 常见误用

- 不要在 `Update` 中重复声明。
- 不要自行 patch 成就系统；把策略声明交给 DSPCore 聚合。

代码示例见 `AchievementPolicyExample.cs`。
