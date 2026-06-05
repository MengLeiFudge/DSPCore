# 错误诊断

## 职责

本功能块收集结构化错误报告，并提供给诊断 UI 和运行时代码。

## 公开入口

- `Errors`：作者侧短入口。
- `ErrorReport`
- `ErrorReporter`

## 运行时

`ErrorRuntime.cs` 捕获 Unity 日志，并给致命错误窗口添加关闭/复制能力。

## 边界

候选问题模组分析、Harmony patch map 和实体定位后续实现时归入本功能块。
