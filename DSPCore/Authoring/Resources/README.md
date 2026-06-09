# 资源与本地化

Resources 模块记录模组共享资源声明，并把本地化条目写入 DSP 的 `Localization` 数据。它适合放“其他功能块会引用的资源信息”和“多语言文本”，不是具体功能逻辑的运行时。

## 这个模块带来什么便利

- 你可以把资源归属、路径和关键字集中登记，避免每个功能块各自维护散落路径。
- 同一模组可以先创建 `ModResources.Pack(...)`，之后注册文本、图标和 Proto 图标绑定时不再重复传 `ownerModGuid` 和根路径。
- 本地化条目会统一写入 DSP 的 key 索引和语言字符串数组，不需要每个模组自己反射 `Localization`。
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
```

`ModResourcePack` 会复用同一个 owner、资源根和默认 assembly。它适合一个模组围绕同一组资源注册多条本地化、图标或 Proto 图标绑定。

## 功能：登记资源描述

如果只需要登记一个独立资源根，仍可使用低层短入口：

```csharp
ModResources.Root(
    id: "example-icons",
    ownerModGuid: "com.example.my-mod",
    keyword: "icons",
    rootPath: "assets/icons");
```

资源描述只记录归属和路径信息。具体怎么加载、何时加载，由消费它的功能块决定；例如图标 sprite 加载由 Icons 负责。

## 功能：登记独立本地化条目

```csharp
ModResources.Text(
    key: "ExampleMachines",
    language: "zhCN",
    value: "示例机器",
    ownerModGuid: "com.example.my-mod");
```

`Language` 可以使用 `zhCN`、`enUS`、`frFR`、`deDE`、`esES`、`jaJA`、`koKO`，也可以直接写 DSP 的语言 LCID 数字字符串。

## 调用后 DSPCore 会怎么处理

- 资源描述只进入 registry，等待具体作者能力或系统读取。
- `ModResourcePack` 会把相对路径解析到自己的 `RootPath` 下；嵌入资源名仍按 manifest resource name 原样传给 Icons。
- 本地化 key 会加入 DSP `Localization.namesIndexer`。
- 切换或应用语言时，DSPCore 会按当前语言 LCID 取出匹配条目，并写入 `Localization.strings`。
- 如果语言字符串数组长度不足，DSPCore 会扩容对应 strings / floats 数组。

## 这个模块不负责什么

- 不加载图标 sprite；图标加载属于 Icons。
- 不创建 Proto 或 UI；它只提供资源和文本数据。
- 不自动处理同一个 key 的多模组冲突；后写入同语言同 key 的值会覆盖数组中的最终文本。
- 不验证资源路径是否真的存在；使用资源的功能块需要处理加载失败。
- 不会主动加载资源 DLL；`Pack(..., assembly: ...)` 只记录默认 assembly，资源 DLL 仍必须已经加载到当前 AppDomain。

## 示例

- `Examples/ResourceRegistration.md`
- `Examples/ResourceRegistrationExample.cs`
