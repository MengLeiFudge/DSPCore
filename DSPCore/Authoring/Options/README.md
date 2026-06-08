# 配置项

Options 模块让模组声明简单配置项，由 DSPCore 统一绑定到 BepInEx `ConfigFile`。公开表面包含字符串配置、布尔/整数读取辅助、设置页面描述和设置版本描述。

## 这个模块带来什么便利

- 模组可以在作者侧声明配置项，不需要拿到 DSPCore 插件实例。
- DSPCore 启动后会绑定已注册配置项；启动后新增的配置项也会立即绑定。
- 设置 UI 可以从 `OptionPageDescriptor` 和带 `PageId` 的 `OptionDescriptor` 生成页面。
- 联机或存档兼容检查可以读取 `OptionVersionDescriptor`。

## 边界

- 当前写入 BepInEx 的底层值仍是字符串；布尔和整数有读取辅助，枚举仍由作者解析。
- 该模块不直接创建具体设置页面。
- 配置 key 应保持稳定，避免玩家本地配置失效。

## 示例

- `Examples/OptionRegistration.md`
- `Examples/OptionRegistrationExample.cs`
