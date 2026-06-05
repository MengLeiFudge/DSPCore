# Tagged Save Blocks / 标签化存档块

Tagged blocks allow authors to add or remove fields without breaking older saves.

标签化存档块允许作者增删字段，而不破坏旧存档。

```csharp
using DSPCore;

SaveBlock[] blocks =
[
    new SaveBlock(
        Tag: "Settings",
        Write: writer => writer.Write(enabled),
        Read: reader => enabled = reader.ReadBoolean()),
    new SaveBlock(
        Tag: "Counter",
        Write: writer => writer.Write(counter),
        Read: reader => counter = reader.ReadInt32())
];

SaveBlockFormat.WriteBlocks(writer, blocks);
SaveBlockFormat.ReadBlocks(reader, blocks);
```
