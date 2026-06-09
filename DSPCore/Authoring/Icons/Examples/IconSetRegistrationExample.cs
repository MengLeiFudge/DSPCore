using System.Reflection;
using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Icons 只注册图标描述和目标绑定。
// - 图标文件可以是 Unity Resources 路径，也可以是本地 PNG 路径。
// - 如果提供 TargetKind 和 TargetProtoId，运行时会把图标应用到对应 Proto。
//
// Usage:
// - Register icons before the proto/icon runtime bridge applies resources.
// - Keep icon ids stable so other features can reference them as fallback icons.
public static class IconSetRegistrationExample
{
    public static void Register()
    {
        // Id 是 DSPCore 内部查找图标的稳定键。
        // Id is the stable key used by DSPCore and other capabilities.
        Icons.FromResources(
            id: "default-machine",
            ownerModGuid: "com.example.my-mod",
            resourcesPath: "icons/default-machine");

        Icons.FromEmbedded(
            id: "embedded-machine",
            ownerModGuid: "com.example.my-mod",
            assembly: Assembly.GetExecutingAssembly(),
            resourceName: "ExampleMod.Assets.embedded-machine.png",
            fallbackIconId: "default-machine");

        Icons.BindToProto(
            id: "example-machine",
            ownerModGuid: "com.example.my-mod",

            // assetPath 可以指向本地 PNG，也可以按后续资源规则指向 Unity 资源。
            // assetPath may point to a local PNG or a Unity resource path.
            assetPath: "assets/icons/example-machine.png",

            // TargetKind/TargetProtoId 表示加载成功后写入哪个 Proto。
            // TargetKind/TargetProtoId select the proto that receives the icon.
            targetKind: ProtoKind.Item,
            targetProtoId: 9554,
            fallbackIconId: "default-machine");
    }
}
