# 资源与本地化

Resources 模块记录模组共享资源声明，并把本地化条目写入 DSP 的 `Localization` 数据。它适合放“其他功能块会引用的资源信息”和“多语言文本”，不是具体功能逻辑的运行时。

## 这个模块带来什么便利

- 你可以把资源归属、路径和关键字集中登记，避免每个功能块各自维护散落路径。
- 本地化条目会统一写入 DSP 的 key 索引和语言字符串数组，不需要每个模组自己反射 `Localization`。
- 中英文或多语言文本可以和功能声明分开维护，减少 UI、Proto、Tabs 里硬编码字符串。

## 功能：登记资源描述

```csharp
DspCore.Resources.RegisterResource(new ResourceDescriptor(
    Id: "example-icons",
    OwnerModGuid: "com.example.my-mod",
    Keyword: "icons",
    RootPath: "assets/icons",
    BundleName: null));
```

资源描述只记录归属和路径信息。具体怎么加载、何时加载，由消费它的功能块决定；例如图标 sprite 加载由 Icons 负责。

## 功能：登记本地化条目

```csharp
DspCore.Resources.RegisterLocalization(new LocalizationEntry(
    Key: "ExampleMachines",
    Language: "zhCN",
    Value: "示例机器",
    OwnerModGuid: "com.example.my-mod"));
```

`Language` 可以使用 `zhCN`、`enUS`、`frFR`、`deDE`、`esES`、`jaJA`、`koKO`，也可以直接写 DSP 的语言 LCID 数字字符串。

## 调用后 DSPCore 会怎么处理

- 资源描述只进入 registry，等待具体功能块读取。
- 本地化 key 会加入 DSP `Localization.namesIndexer`。
- 切换或应用语言时，DSPCore 会按当前语言 LCID 取出匹配条目，并写入 `Localization.strings`。
- 如果语言字符串数组长度不足，DSPCore 会扩容对应 strings / floats 数组。

## 这个模块不负责什么

- 不加载图标 sprite；图标加载属于 Icons。
- 不创建 Proto 或 UI；它只提供资源和文本数据。
- 不自动处理同一个 key 的多模组冲突；后写入同语言同 key 的值会覆盖数组中的最终文本。
- 不验证资源路径是否真的存在；使用资源的功能块需要处理加载失败。

## 示例

当前 Resources 还没有独立 Examples；可参考 Icons、Tabs、Protos 示例中对图标 ID 和本地化 key 的使用。
