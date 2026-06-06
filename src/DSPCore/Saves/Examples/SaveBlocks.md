# SaveBlocks

本场景用于使用标签化分块格式保存可演进字段。

## 适用时机

- 模组存档字段未来可能增删。
- 希望旧存档遇到新字段、新版本遇到未知字段时仍能跳过处理。

## 关键参数

- `Tag`：字段块稳定标签。
- `Write`：写入该块的逻辑。
- `Read`：读取该块的逻辑。

## 运行时前提

在 `ICoreSaveHandler.Export` 中调用 `SaveBlockFormat.WriteBlocks`，在 `Import` 中调用 `ReadBlocks`。未知 Tag 会被跳过。

## 常见误用

- 不要复用同一个 Tag 表示不同语义。
- 不要在 `Read` 中假设所有旧字段一定存在。

代码示例见 `SaveBlocksExample.cs`。
