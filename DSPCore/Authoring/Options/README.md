# 配置项

Options 模块让模组声明简单配置项，由 DSPCore 统一绑定到 BepInEx `ConfigFile`，并提供 DSPCore 自有统一设置窗口。公开表面包含 `String`、`Bool`、`Int`、`Float`、`Enum`、`IntRange`、`FloatRange` 短入口、底层字符串注册、设置页面描述、设置版本描述和 `Options.OpenWindow()`。

## 这个模块带来什么便利

- 模组可以在作者侧声明配置项，不需要拿到 DSPCore 插件实例。
- 简单配置可以用 `Options.Bool(...)`、`Options.IntRange(...)`、`Options.Enum(...)` 这类短入口一次完成注册和读取。
- DSPCore 启动后会绑定已注册配置项；启动后新增的配置项也会立即绑定。
- DSPCore 运行时尚未绑定配置文件时，短入口会返回 descriptor 默认值，不会给作者返回空字符串。
- DSPCore 统一设置窗口会按 `OptionPageDescriptor` 和带 `PageId` 的 `OptionDescriptor` 分组展示配置项。
- 联机或存档兼容检查可以读取 `OptionVersionDescriptor`。

## 功能：短入口注册并读取

```csharp
bool enabled = Options.Bool("Example", "Enabled", true, "Enable example feature.");
int rowCount = Options.Int("Example", "Rows", 2, "Build bar row count.");
float scale = Options.Float("Example", "Scale", 1.0f, "UI scale.");
string mode = Options.String("Example", "Mode", "Normal", "Example mode.");
ExampleMode displayMode = Options.Enum("Example", "DisplayMode", ExampleMode.Normal, "Example display mode.");
int maxRows = Options.IntRange("Example", "MaxRows", 3, "Maximum rows.", minimum: 1, maximum: 6);
float opacity = Options.FloatRange("Example", "Opacity", 0.8f, "Panel opacity.", minimum: 0.2f, maximum: 1.0f, step: 0.05f);
```

这些方法会先注册配置项，再返回当前值。适合普通开关、数字、文本、枚举下拉和范围滑条配置。

## 功能：打开统一设置窗口

```csharp
Options.OpenWindow();
```

统一设置窗口会展示所有已注册配置项：`Bool` 使用复选框，`Enum` 使用下拉框，`IntRange` / `FloatRange` 使用滑条，`String`、`Int` 和 `Float` 使用输入框，按键绑定使用输入框加 Capture 按钮。输入结束或按键捕获后会写回 DSPCore 的 BepInEx 配置文件。按键绑定会在同一 `ConflictGroup` 内提示同键冲突。窗口必须在 `UIRoot` 初始化后打开，通常从模组按钮、快捷键或自己的 UI 回调里调用。

## 边界

- 当前写入 BepInEx 的底层值仍是字符串；布尔、整数、浮点和枚举有读取辅助。
- 当前窗口是 DSPCore 自有窗口，不注入原版设置页。
- `Int` / `Float` / 按键绑定输入非法时会回滚为当前配置值；range 滑条会按最小值、最大值和步进值写回。
- 配置 key 应保持稳定，避免玩家本地配置失效。

## 示例

- `Examples/OptionRegistration.md`
- `Examples/OptionRegistrationExample.cs`
