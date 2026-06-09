# IconSetRegistration

本场景用于注册图标描述，并按需把图标应用到目标 Proto。

## 适用时机

- 模组有共享图标资源，其他功能块需要按稳定 ID 引用。
- 需要把 PNG、Unity Resources、嵌入 PNG 或 AssetBundle 图标绑定到物品、配方、科技等 Proto。
- 同一模组的图标共享 owner、资源根和 assembly，希望减少重复参数。

## 关键参数

- `Id`：图标稳定 ID。
- `OwnerModGuid`：图标归属模组。
- `AssetPath`：PNG 或资源路径。
- `ModResources.Pack(...)`：创建复用 owner、资源根和默认 assembly 的图标短入口。
- `assembly` / `resourceName`：嵌入 PNG 所在程序集和 manifest resource name。
- `bundlePath` / `assetName`：AssetBundle 文件路径和其中的 Sprite / Texture2D 资源名。
- `FallbackIconId`：加载失败时使用的备用图标。
- `TargetKind` / `TargetProtoId`：可选目标 Proto。

## 运行时前提

在图标运行时桥接应用资源前完成注册。图标功能块不创建 Proto；需要目标 Proto 时先通过 ProtoRegistration 注册。使用资源 DLL 时，该 assembly 必须已经加载到当前 AppDomain。使用 AssetBundle 时，文件路径必须在运行时可访问。

## 常见误用

- 不要把图标 ID 设计成临时文件名。
- 不要把资源包 `rootPath` 和嵌入资源名拼在一起；嵌入资源名必须是完整 manifest resource name。
- 不要在 Icons 中写物品或配方创建逻辑。

代码示例见 `IconSetRegistrationExample.cs`。
