# Save Handler / 存档处理器

Implement `ICoreSaveHandler` for mod save data.

实现 `ICoreSaveHandler` 来保存模组数据。

```csharp
using System.IO;
using DSPCore;

public sealed class MySaveHandler : ICoreSaveHandler
{
    public void Export(BinaryWriter writer)
    {
        writer.Write(1);
    }

    public void Import(BinaryReader reader)
    {
        int version = reader.ReadInt32();
    }

    public void IntoOtherSave()
    {
    }
}

DspCore.Saves.Register("com.example.my-mod", new MySaveHandler());
```
