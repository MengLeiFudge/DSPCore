# 错误诊断

Errors 模块收集结构化错误报告，并增强原版致命错误窗口的复制/关闭能力。它让模组作者把自己的异常按归属记录下来，也让 DSPCore 运行时把捕获到的问题集中留痕。

## 这个模块带来什么便利

- 你可以用统一入口记录异常，不需要每个模块自己维护错误列表。
- 错误报告包含归属模组 GUID、错误类型、消息和堆栈，便于后续诊断 UI 或日志工具读取。
- Unity error / exception / assert 日志会被捕获成报告。
- `Errors.BuildDiagnosticText(...)` 会生成可复制诊断文本，包含最近错误、文本命中的候选插件、DSPCore 作者声明和 Harmony patch map 概览。
- 原版 fatal window 会增加 Copy 和 Close 按钮；Copy 会复制增强诊断文本，Close 会关闭窗口。

## 功能：上报模组异常

```csharp
try
{
    DoWork();
}
catch (Exception ex)
{
    Errors.ReportException("com.example.my-mod", ex);
}
```

也可以直接构造 `ErrorReport` 并调用 `Errors.Report(report)`。

## 功能：生成或复制诊断文本

```csharp
string text = Errors.BuildDiagnosticText("current error text");
Errors.CopyDiagnosticText("current error text");
```

诊断文本会包含当前错误文本、最近错误报告、命中文本的候选插件、已注册 feature/module/patch 声明，以及 Harmony 已 patch 方法和 owner 概览。候选插件只是基于 GUID / 名称文本命中，不能等同于根因判断。

## 调用后 DSPCore 会怎么处理

- `Report(...)` 会把报告追加到内存列表。
- `ReportException(...)` 会用异常类型、消息和堆栈创建 `ErrorReport` 并追加。
- `GetReports()` 返回当前报告快照。
- `BuildDiagnosticText(...)` 返回诊断文本；`CopyDiagnosticText(...)` 会同时写入系统剪贴板并返回同一份文本。
- Unity 日志线程收到 error / exception / assert 时，DSPCore 会以 `UnityLog` 归属记录报告。
- `UIFatalErrorTip` 显示错误或 assertion failed 时，DSPCore 会记录 `DSPCore.FatalWindow` 报告，并确保窗口有 Copy / Close 按钮；Copy 使用 `BuildDiagnosticText(...)` 生成增强文本。

## 这个模块不负责什么

- 不自动判断“哪个模组是根因”；候选插件只表示错误文本命中了插件 GUID 或名称。
- 不做实体、星球、工厂对象的深度定位；这仍是后续能力。
- 不自动上传日志或发送外部网络请求。
- 不替你的业务逻辑恢复状态；它只记录错误并改善玩家复制错误的流程。

## 示例

见 `Examples/ErrorDiagnosticsExample.cs`。
