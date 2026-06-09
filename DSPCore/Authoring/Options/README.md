# 配置项

Options 模块让模组声明简单配置项，由 DSPCore 统一绑定到 BepInEx `ConfigFile`。公开表面包含 `String`、`Bool`、`Int`、`Float` 短入口、底层字符串注册、设置页面描述和设置版本描述。

## 这个模块带来什么便利

- 模组可以在作者侧声明配置项，不需要拿到 DSPCore 插件实例。
- 简单配置可以用 `Options.Bool(...)`、`Options.Int(...)` 这类短入口一次完成注册和读取。
- DSPCore 启动后会绑定已注册配置项；启动后新增的配置项也会立即绑定。
- DSPCore 运行时尚未绑定配置文件时，短入口会返回 descriptor 默认值，不会给作者返回空字符串。
- 设置 UI 可以从 `OptionPageDescriptor` 和带 `PageId` 的 `OptionDescriptor` 生成页面。
- 联机或存档兼容检查可以读取 `OptionVersionDescriptor`。

## 功能：短入口注册并读取

```csharp
bool enabled = Options.Bool("Example", "Enabled", true, "Enable example feature.");
int rowCount = Options.Int("Example", "Rows", 2, "Build bar row count.");
float scale = Options.Float("Example", "Scale", 1.0f, "UI scale.");
string mode = Options.String("Example", "Mode", "Normal", "Example mode.");
```

这些方法会先注册配置项，再返回当前值。适合普通开关、数字和文本配置。

## 边界

- 当前写入 BepInEx 的底层值仍是字符串；布尔、整数和浮点有读取辅助，枚举仍由作者解析。
- 该模块不直接创建具体设置页面。
- 配置 key 应保持稳定，避免玩家本地配置失效。

## 示例

- `Examples/OptionRegistration.md`
- `Examples/OptionRegistrationExample.cs`
