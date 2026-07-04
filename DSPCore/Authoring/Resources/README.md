# 资源与本地化

Resources 模块记录模组共享资源声明，并把本地化条目写入 DSP 的 `Localization` 数据。它适合放“其他功能块会引用的资源信息”和“多语言文本”，不是具体功能逻辑的运行时。

## 这个模块带来什么便利

- 你可以把资源归属、路径和关键字集中登记，并通过同一套解析/读取入口消费这些资源，避免每个功能块各自维护散落路径。
- 同一模组可以先创建 `ModResources.Pack(...)`，之后注册文本、图标和 Proto 图标绑定时不再重复传 `ownerModGuid` 和根路径。
- 已知目标 Proto 类型时，可以直接用 `pack.ItemIcon(...)`、`RecipeIcon(...)`、`TechIcon(...)`、`TutorialIcon(...)` 或 `SignalIcon(...)`，不用再手写 `ProtoKind`。
- 本地化条目会统一写入 DSP 的 key 索引和语言字符串数组，不需要每个模组自己反射 `Localization`。
- 本地化条目注册后可以立刻通过 `ModResources.Translate(...)` 或 `pack.Translate(...)` 查询，不需要等待 DSP `Localization.LoadLanguage`。
- 玩家或翻译者可以在 BepInEx config 下维护 `DSPCore/Locales/locale-<language>.tsv` 覆盖任意模组的翻译；覆盖层会同时影响实时查询和原版语言数组。
- 中英文或多语言文本可以和功能声明分开维护，减少 UI、Proto、Tabs 里硬编码字符串。

## 功能：用资源包短入口登记资源和文本

```csharp
var pack = ModResources.Pack(
    ownerModGuid: "com.example.my-mod",
    rootPath: "assets",
    assembly: typeof(MyPlugin).Assembly);

pack.Root(id: "example-assets", keyword: "assets");
pack.Text("ExampleMachines", "zhCN", "示例机器");
pack.Text("ExampleMachines", "enUS", "Example Machines");
var title = pack.Translate("ExampleMachines", language: "zhCN");
pack.ItemIcon("example-machine-icon", "icons/example-machine.png", itemId: 9554);
```

`ModResourcePack` 会复用同一个 owner、资源根和默认 assembly。它适合一个模组围绕同一组资源注册多条本地化、图标或 Proto 图标绑定。`ItemIcon` / `RecipeIcon` 等 typed helper 会把相对路径拼到 `RootPath` 下，并把目标类型固定到方法名对应的 `ProtoKind`。

## 功能：登记资源描述

如果只需要登记一个独立资源根，仍可使用低层短入口：

```csharp
ModResources.Root(
    id: "example-icons",
    ownerModGuid: "com.example.my-mod",
    keyword: "icons",
    rootPath: "assets/icons");
```

资源描述会进入统一资源索引。消费方可以用 `DspCore.Resources.ResolvePath(...)`、`TryResolvePath(...)`、`TryOpenRead(...)` 或 `TryReadBytes(...)` 按 root id、owner + keyword 或 owner 的默认资源根解析和读取文件；例如 Icons 的本地 PNG 和 AssetBundle 路径会复用这套解析规则。

## 功能：登记独立本地化条目

```csharp
ModResources.Text(
    key: "ExampleMachines",
    language: "zhCN",
    value: "示例机器",
    ownerModGuid: "com.example.my-mod");
```

`Language` 可以使用 `zhCN`、`enUS`、`frFR`、`deDE`、`esES`、`jaJA`、`koKO`，也可以直接写 DSP 的语言 LCID 数字字符串。

## 功能：实时查询已注册翻译

```csharp
ModResources.Text("ExampleMachines", "zhCN", "示例机器", "com.example.my-mod");

var zh = ModResources.Translate("ExampleMachines", language: "zhCN");
var current = ModResources.Translate("ExampleMachines", fallback: "Example Machines");
```

`Translate(...)` 会直接读取 DSPCore registry 中已经注册的文本。指定 `language` 时会按该语言查询；不指定时按当前游戏语言 LCID 查询。找不到时返回 `fallback`，没有 fallback 则返回 key 本身。这个查询不依赖 DSP 当前 `Localization.currentStrings` 是否已经刷新。

## 功能：玩家 locale 文件覆盖

玩家、翻译者或整合包作者可以在 BepInEx config 目录下创建：

```text
DSPCore/Locales/locale-zhCN.tsv
DSPCore/Locales/locale-enUS.tsv
```

文件每行一条翻译，格式是 `key<TAB>value`：

```text
# 注释行会被忽略
ExampleMachines	示例机器
ExampleDescription	第一行\n第二行
```

语言后缀支持 `zhCN`、`enUS`、`frFR`、`deDE`、`esES`、`jaJA`、`koKO`，也可以直接写 DSP 的语言 LCID 数字字符串。DSPCore 启动时读取这些文件；修改文件后需要重启游戏。玩家覆盖层优先级高于模组作者注册的同 key 同语言文本，所以它可以覆盖任意模组的翻译。

## 调用后 DSPCore 会怎么处理

- 资源描述进入 registry 后可被统一路径解析和文件读取入口消费。
- `ModResourcePack` 会复用同一套资源路径规则，把相对路径解析到自己的 `RootPath` 下；嵌入资源名仍按 manifest resource name 原样传给 Icons。
- Icons 读取本地 PNG 或 AssetBundle 时会先尝试原始路径，找不到时再按图标 owner 的已注册资源根解析。
- 启动时会读取 `DSPCore/Locales/locale-*.tsv`，把玩家覆盖层登记到 DSPCore 自己的本地化索引。
- 本地化 key 会加入 DSP `Localization.namesIndexer`。
- 切换或应用语言时，DSPCore 会按当前语言 LCID 取出匹配条目，并写入 `Localization.strings`；如果玩家文件和作者注册了同一个 key，玩家文件优先。
- `Translate(...)` / `TryTranslate(...)` 查询的是 DSPCore 自己的实时本地化索引，注册后立即可见，且同样优先返回玩家文件覆盖值。
- 如果语言字符串数组长度不足，DSPCore 会扩容对应 strings / floats 数组。

## 这个模块不负责什么

- 不加载图标 sprite；图标加载属于 Icons。
- 不创建 Proto 或 UI；它只提供资源和文本数据。
- `ItemIcon` 等 typed helper 只登记图标和目标绑定，不创建目标 Proto。
- 不自动处理同一个 key 的多模组冲突；作者注册层仍然后写入同语言同 key 的值覆盖先前文本，但玩家 locale 文件始终优先。
- 不在注册时验证资源路径是否真的存在；使用资源的功能块需要处理加载失败，`TryOpenRead(...)` / `TryReadBytes(...)` 会用返回值表示是否成功。
- 不会主动加载资源 DLL；`Pack(..., assembly: ...)` 只记录默认 assembly，资源 DLL 仍必须已经加载到当前 AppDomain。
- 不热重载玩家 locale 文件；修改翻译文件后需要重启游戏。

## 示例

- `Examples/ResourceRegistration.md`
- `Examples/ResourceRegistrationExample.cs`
