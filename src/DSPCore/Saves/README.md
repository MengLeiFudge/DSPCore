# 存档

## 职责

本功能块声明模组存档处理器和 tagged block 保存辅助。

## 公开入口

- `ICoreSaveHandler`
- `SaveRegistry`
- `SaveRegistration`
- `CoreLoadOrder`
- `SaveBlock`
- `SaveBlockFormat`

## 示例

- `Examples/SaveHandlerExample.cs`
- `Examples/SaveBlocksExample.cs`

## 运行时

`Runtime/SaveRuntime.cs` 会读写 `.dspcore` 独立存档，按加载顺序调用处理器，并桥接已覆盖的旧 DSPModSave 处理器。

## 边界

处理器可以使用原始 `BinaryReader`/`BinaryWriter` API 或 tagged block。会随版本增删的字段优先使用 tagged block。
