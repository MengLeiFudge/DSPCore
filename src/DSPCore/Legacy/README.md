# 旧 API 兼容

## 职责

本功能块保留 obsolete 旧命名空间，并把已覆盖调用映射到 DSPCore 新功能块。

## 公开入口

- `xiaoye97.LDBTool`
- `crecheng.DSPModSave`
- `CommonAPI`
- `BuildBarTool`

兼容 shim 文件放在 `Compat/`。

## 运行时

旧调用会尽量委托给新注册表。运行时行为仍由新功能块负责。

## 边界

旧命名空间不能成为 DSPCore 内部设计语言。
