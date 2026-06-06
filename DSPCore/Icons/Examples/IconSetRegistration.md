# IconSetRegistration

本场景用于注册图标描述，并按需把图标应用到目标 Proto。

## 适用时机

- 模组有共享图标资源，其他功能块需要按稳定 ID 引用。
- 需要把 PNG 或 Unity Resources 图标绑定到物品、配方、科技等 Proto。

## 关键参数

- `Id`：图标稳定 ID。
- `OwnerModGuid`：图标归属模组。
- `AssetPath`：PNG 或资源路径。
- `FallbackIconId`：加载失败时使用的备用图标。
- `TargetKind` / `TargetProtoId`：可选目标 Proto。

## 运行时前提

在图标运行时桥接应用资源前完成注册。图标功能块不创建 Proto；需要目标 Proto 时先通过 ProtoRegistration 注册。

## 常见误用

- 不要把图标 ID 设计成临时文件名。
- 不要在 Icons 中写物品或配方创建逻辑。

代码示例见 `IconSetRegistrationExample.cs`。
