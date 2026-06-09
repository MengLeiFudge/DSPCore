# 作者声明诊断

Diagnostics 模块让 DSPCore 在启动期检查作者已经注册的声明，并把可疑项写入日志和可复制诊断文本。公开表面包含 `Diagnostics.Info(...)`、`Diagnostics.Warn(...)`、`Diagnostics.Error(...)`、`Diagnostics.RunBuiltInChecks()` 和 `Diagnostics.GetIssues()`。

## 这个模块带来什么便利

- 模组作者不需要自己遍历 DSPCore 的各个 registry 来找常见声明错误。
- DSPCore 启动时会自动运行内置检查；发现 warning 或 error 时，会写入 BepInEx 日志，并进入 `Errors.BuildDiagnosticText(...)` 的 Diagnostics 段。
- 作者可以用短入口把自己的业务检查结果汇入同一份诊断文本。
- 诊断只报告问题，不阻止加载，不改变 Proto、Tab、Option、Resource 或 UI runtime 的实际行为。

## 功能：手动报告作者声明问题

```csharp
Diagnostics.Warn(
    ownerModGuid: "com.example.my-mod",
    code: "example.recipe.unreachable",
    message: "Recipe is registered but no machine can craft it.",
    subject: "recipe=9554");
```

`code` 应保持稳定，便于作者和玩家搜索日志。`subject` 写具体对象，例如 `item=9554`、`tab=com.example.machines` 或 `recipe=9554`。

## 功能：内置检查

启动期会自动运行一次内置检查，也可以在测试或调试代码中手动读取：

```csharp
IReadOnlyList<DiagnosticIssue> issues = Diagnostics.RunBuiltInChecks();
```

当前内置检查覆盖：

- 已注册 Proto 的 ID 是否缺失或重复。
- 已注册物品和配方的 `GridIndex` 是否重复。
- 自定义 `GridIndex` 是否指向未注册的 DSPCore tab。
- Tab 声明是否缺图标或引用未注册图标。
- Option 是否引用未注册设置页。
- 同一个本地化 key 是否缺少中文或英文基础语言条目。

## 边界

- 诊断是作者提示，不是强制校验；DSPCore 不会因为这些诊断直接阻止游戏加载。
- 内置检查只使用当前已经注册到 DSPCore 的声明快照；如果模组在更晚阶段才注册声明，应在注册后自行调用 `Diagnostics.RunBuiltInChecks()` 或手动报告。
- `GridIndex` 检查只覆盖已通过 DSPCore 注册的物品和配方；直接改原版 LDB 且未经过 DSPCore registry 的对象不在本模块范围内。
- 本地化检查只要求常见中文/英文基础条目，不判断文本质量或所有语言覆盖率。
- 运行时错误报告仍由 ErrorWindow / Errors 负责；Diagnostics 负责作者声明质量。

## 示例

- `Examples/DeclarationDiagnostics.md`
- `Examples/DeclarationDiagnosticsExample.cs`
