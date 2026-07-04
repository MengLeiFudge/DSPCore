# 存档

Saves 模块让模组把自己的状态保存到 DSPCore 独立的 `.dspcore` 旁路存档里，并在新游戏、读档、删档和自动存档轮换时跟随原版流程处理。它也提供 `Saves.Global...` 入口保存跨存档数据。

## 这个模块带来什么便利

- 你不需要自己 patch `GameSave`、管理旁路文件名、处理自动存档轮换或删档同步。
- 每个模组按 GUID 拥有自己的存档段，导入/导出异常会记录到 Errors，不会要求所有模组共享一个脆弱的二进制布局。
- `IntoOtherSave()` 会在新游戏或当前存档没有本模组数据时调用，减少“上一份存档状态泄漏到另一份存档”的风险。
- 已覆盖的旧 DSPModSave 处理器会桥接进同一套 SaveRegistry，便于迁移。
- `Saves.Auto<TState>(...)` 可以直接创建并保存带 `[CoreSaveField]` 标记的简单状态对象；需要先填默认值或注入依赖时，也可以传入已有实例。
- `Saves.GlobalAuto<TState>(...)` / `GlobalRegister(...)` 保存跨存档数据，适合成就统计、全局解锁记录或玩家 profile 级状态；它不是 Config，DSPCore 只提供只读 metadata 页面，不提供编辑 UI。
- tagged block 工具可以让可演进字段按标签读写，未知字段会跳过，降低版本升级成本。

## 功能：自动 schema 保存简单状态

```csharp
private sealed class ExampleState
{
    [CoreSaveField("counter")]
    public int Counter { get; set; }

    [CoreSaveField("enabled")]
    public bool Enabled = true;
}

private static readonly ExampleState State = Saves.Auto<ExampleState>("com.example.my-mod");
```

`Saves.Auto<TState>(...)` 会用无参构造函数创建状态对象，并注册成存档处理器。导出时写入 schema version 和每个 `[CoreSaveField]` 成员；导入时先恢复注册时的默认值，再按 tag 读取已有字段，缺失字段保持默认值。需要迁移旧版本时传入 `migrate: (version, state) => { ... }`。

如果状态对象需要构造参数、依赖注入或注册前先填充默认值，使用实例重载：`Saves.Auto("com.example.my-mod", state)`。

当前自动 schema 只支持 `bool`、`int`、`long`、`float`、`double`、`string` 和 enum。复杂集合、字典、嵌套对象和 Unity 类型仍使用委托、完整 handler 或 tagged block。

## 功能：保存跨存档全局数据

跨存档数据使用同一套 `[CoreSaveField]` 和 `ICoreSaveHandler` 规则，但写入 BepInEx config 下的 `DSPCore/GlobalSaves.dspcore`，不会跟随某一个游戏存档轮换或删除。

```csharp
private sealed class GlobalState
{
    [CoreSaveField("total_runs")]
    public int TotalRuns { get; set; }
}

private static readonly GlobalState Global = Saves.GlobalAuto<GlobalState>(
    "com.example.my-mod",
    initialize: state => state.TotalRuns = 0);
```

委托写法：

```csharp
Saves.GlobalRegister(
    modGuid: "com.example.my-mod",
    export: writer => writer.Write(totalRuns),
    import: reader => totalRuns = reader.ReadInt32(),
    initialize: () => totalRuns = 0);
```

全局数据会在 DSPCore 启动时导入；如果某个模组稍后才注册 global handler，DSPCore 会立刻对该 handler 导入已有数据或调用初始化。插件销毁时会自动保存一次；需要立即落盘时可以调用 `Saves.SaveGlobal()`。玩家可以从 DSPCore 统一设置窗口打开只读“全局数据”页，查看已注册项、文件中残留的块和字节数，但不能在 UI 中修改内容。

## 功能：注册存档处理器

简单状态可以直接用委托注册，不必单独声明一个 handler class：

```csharp
Saves.Register(
    modGuid: "com.example.my-mod",
    export: writer => writer.Write(counter),
    import: reader => counter = reader.ReadInt32(),
    intoOtherSave: () => counter = 0);
```

复杂状态可以实现 `ICoreSaveHandler`，然后在启动或功能注册阶段调用：

```csharp
Saves.Register("com.example.my-mod", handler, CoreLoadOrder.Postload);
```

`CoreLoadOrder.Preload` 会在游戏主要加载流程前导入，`CoreLoadOrder.Postload` 会在主要加载流程后导入。多数普通模组状态使用 `Postload`。

同一个 `modGuid` 后一次注册会覆盖前一次注册。`modGuid` 不能为空。

## 调用后 DSPCore 会怎么处理

- 保存成功后，DSPCore 会写入同名 `.dspcore` 文件，文件内按注册的 `modGuid` 保存每个处理器的数据区间。
- 全局保存使用 `DSPCore/GlobalSaves.dspcore`，按 `modGuid` 保存每个全局处理器的数据块；统一设置窗口只显示块 metadata，不读取或展示二进制内容。
- 读档前会打开 `.dspcore` 文件并读取 header，然后按 `CoreLoadOrder` 调用对应处理器的 `Import`。
- 如果当前存档没有该模组的数据，DSPCore 会调用该处理器的 `IntoOtherSave()`。
- 新游戏会对所有处理器调用 `IntoOtherSave()`。
- 自动存档会同步轮换 `_autosave_0`、`_autosave_1`、`_autosave_2`、`_autosave_3` 的 `.dspcore` 文件。
- 删除原版存档时，DSPCore 会删除对应 `.dspcore` 文件。

## 功能：使用 tagged block 保存可演进字段

如果你的存档字段未来可能增加、删除或改为可选，优先在 `Export` / `Import` 内使用：

```csharp
SaveBlockFormat.WriteBlocks(writer, blocks);
SaveBlockFormat.ReadBlocks(reader, blocks, onBlockError);
```

每个 `SaveBlock` 有一个稳定 `Tag`。读取时未知 tag 会被跳过；单个 block 读取异常可以交给 `onBlockError` 处理。

## 功能：兼容旧 DSPModSave

旧 `crecheng.DSPModSave.IModCanSave`、`ModSaveSettingsAttribute` 和 `DSPModSavePlugin.AddModSaveManually(...)` 仍保留为 obsolete 兼容入口。DSPCore 会把已覆盖的旧处理器适配成 `ICoreSaveHandler`。

新代码应直接使用 `DSPCore.ICoreSaveHandler` 和 `Saves.Register(...)`。

## 这个模块不负责什么

- 不替你设计二进制格式版本；复杂数据仍应自己写版本号或使用 tagged block。
- 不保证旧 DSPModSave 的所有边缘行为完全复刻；兼容目标是把已覆盖处理器桥接进 DSPCore 生命周期。
- 不在 `Import` 失败时自动恢复业务状态；异常会记录，状态恢复策略仍由模组处理器负责。
- 不会把 `.dspcore` 内容写入原版 `.dsv` 文件；它是独立旁路文件。
- `Saves.Global...` 不等于 Config：它不暴露可编辑 UI，玩家不应直接修改全局二进制文件。

## 示例

- `Examples/SaveHandler.md`
- `Examples/SaveHandlerExample.cs`
- `Examples/SaveBlocks.md`
- `Examples/SaveBlocksExample.cs`
