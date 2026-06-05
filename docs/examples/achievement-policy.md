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
