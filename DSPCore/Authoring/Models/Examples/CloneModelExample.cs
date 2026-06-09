using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - ModelProto.CloneAsModel 在调用方已经持有来源模型对象时登记 clone 声明。
// - Models.CloneModel 在调用方只有来源 model index 时登记 clone 声明。
// - ModelDescriptor 路径保留给配置驱动或批量构造。
//
// Usage:
// - Register model clone declarations before DSPCore applies model projection.
// - CloneAsModel records a declaration; it does not immediately create or return the cloned model.
// - Keep the new modelIndex unique across mods.
internal static class CloneModelExample
{
    public static void Register(ModelProto sourceModel)
    {
        // When you already have the source model object, use the object-centric extension.
        sourceModel.CloneAsModel(
            modelIndex: 9554,
            ownerModGuid: "com.example.my-mod",
            configureModel: static model =>
            {
                model.Name = "Example Cloned Model";
            },
            configurePrefab: static prefab =>
            {
                prefab.modelIndex = 9554;
            });

        // If only the source model index is available, the static short entry remains valid.
        Models.CloneModel(
            sourceModelIndex: 230,
            modelIndex: 9555,
            ownerModGuid: "com.example.my-mod");
    }
}
