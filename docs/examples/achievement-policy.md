# Achievement Policy / 成就策略

DSPCore does not make each mod patch achievements independently. Mods declare their policy, and DSPCore aggregates the final result.

DSPCore 不让每个模组各自 patch 成就系统。模组只声明策略，DSPCore 汇总最终结果。

```csharp
using DSPCore;

DspCore.Achievements.Declare(new AchievementPolicyDeclaration(
    ModGuid: "com.example.balance",
    DisableAchievements: true,
    Reason: "Changes balance",
    SourceVersion: "1.0.0"));
```

If any declaration disables achievements, the final result disables achievements.

只要任意声明要求禁用成就，最终结果就是禁用成就。

Runtime policy:

运行时策略：

- When no mod disables achievements, DSPCore blocks vanilla abnormality checks so local achievements can still progress.
- 没有模组禁用成就时，DSPCore 会屏蔽原版异常检查，让本地成就继续推进。

- When any mod disables achievements, DSPCore lets vanilla abnormality checks run and blocks local achievement progress/unlock.
- 任意模组禁用成就时，DSPCore 会允许原版异常检查运行，并阻断本地成就进度/解锁。

- Milky Way uploads, leaderboard uploads, and platform achievement sync are blocked by default unless the framework policy explicitly allows them.
- Milky Way 上传、排行榜上传和平台成就同步默认阻断，除非框架策略显式允许。

```csharp
DspCore.Achievements.AllowPlatformAchievements = false;
DspCore.Achievements.AllowMilkyWayUpload = false;
DspCore.Achievements.MetadataMode = AchievementMetadataMode.DeclarationsOnly;
```
