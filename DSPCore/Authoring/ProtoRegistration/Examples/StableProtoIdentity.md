# StableProtoIdentity

本场景用于给新增 Proto 声明稳定身份，并把运行时 int ID 分配、旧 key 迁移和链式后续配置放在同一个作者工作流里。

## 适用时机

- 模组新增物品、配方、科技或教程，并希望避免和其他模组抢同一个 int ID。
- 作者希望提供一个 preferred int ID，但允许 DSPCore 在冲突时自动换到可用 ID。
- 某个版本之后稳定 key 改名，需要让旧版本映射迁移到新 key。
- 注册后还要继续设置 `GridIndex`、建造栏、图标或其他 proto 字段。

## 关键参数

- `key`：模组内稳定 key。不要使用玩家可见翻译文本，使用不会随语言变化的机器名。
- `preferredId`：优先候选 int ID。它只是候选；如果被原版或其他模组占用，DSPCore 会分配其他可用 ID。
- `aliases`：旧版本 key 列表。DSPCore 会先尝试复用 alias 已有映射，再写入新 key。
- `purpose`：注册目的说明，便于错误报告和日志定位。

## 运行时前提

在 DSPCore 应用 Proto 阶段前注册。推荐在 `ProtoRegistration.Data(...)` 里声明基础 Proto，在 `DataUpdates` 或 `DataFinalFixes` 中处理跨模组调整。

稳定 key 到最终 int ID 的映射写入 BepInEx config 下的 `DSPCore/StableProtoIds.tsv`。游戏运行时、LDB、数组索引和原版存档仍使用最终 int ID。

## 常见误用

- 不要把 `preferredId` 当成强制 ID；它被占用时应允许 DSPCore 改分配。
- 不要为了换翻译或展示名改 stable key；只有真实身份迁移才改 key，并把旧 key 放入 alias。
- 不要用 stable key 替代 `GridIndex`、BuildBar 位置或本地化 key；它只解决 Proto 运行时 int ID 稳定性。

代码示例见 `StableProtoIdentityExample.cs`。
