# 错误诊断

Errors 模块收集结构化错误报告，并增强原版致命错误窗口的复制/关闭能力。它让模组作者把自己的异常按归属记录下来，也让 DSPCore 运行时把捕获到的问题集中留痕。

## 这个模块带来什么便利

- 你可以用统一入口记录异常，不需要每个模块自己维护错误列表。
- 错误报告包含归属模组 GUID、错误类型、消息和堆栈，便于后续诊断 UI 或日志工具读取。
- Unity error / exception / assert 日志会被捕获成报告。
- 原版 fatal window 会增加 Copy 和 Close 按钮，方便玩家复制错误文本并关闭窗口。

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

## 调用后 DSPCore 会怎么处理

- `Report(...)` 会把报告追加到内存列表。
- `ReportException(...)` 会用异常类型、消息和堆栈创建 `ErrorReport` 并追加。
- `GetReports()` 返回当前报告快照。
- Unity 日志线程收到 error / exception / assert 时，DSPCore 会以 `UnityLog` 归属记录报告。
- `UIFatalErrorTip` 显示错误或 assertion failed 时，DSPCore 会记录 `DSPCore.FatalWindow` 报告，并确保窗口有 Copy / Close 按钮。

## 这个模块不负责什么

- 不自动判断“哪个模组是根因”；候选问题模组分析、Harmony patch map 和实体定位仍是后续能力。
- 不自动上传日志或发送外部网络请求。
- 不替你的业务逻辑恢复状态；它只记录错误并改善玩家复制错误的流程。

## 示例

当前 Errors 还没有独立 Examples；作者侧入口是 `Errors.Report(...)` 和 `Errors.ReportException(...)`。
