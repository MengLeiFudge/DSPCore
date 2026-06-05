# 兼容补丁声明

## 职责

本功能块把兼容补丁声明记录为功能级元数据。

## 公开入口

- `Compatibility`：作者侧短入口。
- `CompatibilityPatchDescriptor`
- `CompatibilityPatchRegistry`

## 示例

- `Examples/CompatibilityPatchExample.cs`

## 运行时

运行时应用由需要补丁的具体功能块负责。本注册表是声明和报告入口。

## 边界

不要把无关修复藏在通用 fixer 名称下。修复应按 tutorial、build bar、save、UI 等具体功能拆分。
