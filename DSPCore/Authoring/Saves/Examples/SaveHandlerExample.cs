using System.IO;
using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Saves.Auto 是简单状态对象的最短入口。
// - ICoreSaveHandler 是 DSPCore 的完整模组存档处理器接口。
// - Saves.Register 会把某个 modGuid 和处理器实例绑定到 DSPCore 存档系统。
// - 简单存档可以直接使用 BinaryReader/BinaryWriter；可变字段建议结合 SaveBlockFormat。
//
// Usage:
// - 在你的插件 Awake 或功能注册阶段调用 Register。
// - Export/Import 内部应先写版本号，方便未来迁移。
// - IntoOtherSave 用于“加载了其他存档但没有本模组数据”的初始化。
public sealed class SaveHandlerExample : ICoreSaveHandler
{
    private static readonly ExampleState State = Saves.Auto("com.example.auto-mod", new ExampleState());
    private static int counter;

    public static void Register()
    {
        // modGuid 建议使用你的 BepInEx GUID。
        // Use your BepInEx plugin GUID as modGuid.
        Saves.Register(
            modGuid: "com.example.simple-mod",
            export: writer => writer.Write(counter),
            import: reader => counter = reader.ReadInt32(),
            intoOtherSave: () => counter = 0);

        // 复杂状态仍可以用完整 handler class。
        // Complex state can still use a full handler class.
        Saves.Register("com.example.my-mod", new SaveHandlerExample());
    }

    private sealed class ExampleState
    {
        [CoreSaveField("counter")]
        public int Counter { get; set; }

        [CoreSaveField("enabled")]
        public bool Enabled = true;
    }

    public void Export(BinaryWriter writer)
    {
        // 先写格式版本。后续增删字段时用它做迁移。
        // Write a format version first for future migrations.
        writer.Write(1);
    }

    public void Import(BinaryReader reader)
    {
        // 读取版本后根据版本分支处理旧字段。
        // Read the version and branch migration logic from it.
        int version = reader.ReadInt32();
    }

    public void IntoOtherSave()
    {
        // 在这里重置为默认状态，避免上一个存档的数据泄漏到新存档。
        // Reset to defaults here so previous-save state does not leak.
    }
}
