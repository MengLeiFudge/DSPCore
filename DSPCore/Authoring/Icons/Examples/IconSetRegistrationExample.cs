using System.Reflection;
using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Icons 只注册图标描述和目标绑定。
// - 图标文件可以是 Unity Resources、本地 PNG、嵌入 PNG 或 AssetBundle 资源。
// - 如果提供 TargetKind 和 TargetProtoId，运行时会把图标应用到对应 Proto。
//
// Usage:
// - Register icons before the proto/icon runtime bridge applies resources.
// - Keep icon ids stable so other features can reference them as fallback icons.
public static class IconSetRegistrationExample
{
    public static void Register()
    {
        var pack = ModResources.Pack(
            ownerModGuid: "com.example.my-mod",
            rootPath: "assets/icons",
            assembly: Assembly.GetExecutingAssembly());

        // Id 是 DSPCore 内部查找图标的稳定键。
        // Id is the stable key used by DSPCore and other capabilities.
        pack.IconFromResources(
            id: "default-machine",
            resourcesPath: "default-machine");

        pack.IconFromEmbedded(
            id: "embedded-machine",
            resourceName: "ExampleMod.Assets.embedded-machine.png",
            fallbackIconId: "default-machine");

        pack.IconFromAssetBundle(
            id: "bundle-machine",
            bundlePath: "example-icons",
            assetName: "example-machine",
            fallbackIconId: "default-machine");

        pack.ItemIcon(
            id: "example-machine",

            // assetPath 可以指向本地 PNG，也可以按后续资源规则指向 Unity 资源。
            // assetPath may point to a local PNG or a Unity resource path.
            assetPath: "example-machine.png",

            // ItemIcon 已经表达目标类型，只需要给物品 ID。
            // ItemIcon already names the target kind; only the item id is needed.
            itemId: 9554,
            fallbackIconId: "default-machine");
    }
}
