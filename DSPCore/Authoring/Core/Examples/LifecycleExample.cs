using DSPCore;

namespace ExampleMod;

// 本文件是文档示例，不参与 DSPCore 编译。
// This file is a documentation example and is excluded from DSPCore compilation.
//
// 用途：
// - Lifecycle.OnStarted 在 DSPCore runtime bridge 装配后触发。
// - Lifecycle.OnUpdate 跟随 DSPCore 插件每帧更新。
// - Lifecycle.OnDestroyed 用于清理挂到 DSPCore 生命周期上的临时状态。
// - Lifecycle 的存档链路事件用于刷新非持久缓存，不替代 Saves 持久化。
//
// Usage:
// - Use this for framework-level setup and cleanup.
// - Use game-specific authoring capabilities for save, proto, planet, factory, or UI lifecycle.
public static class LifecycleExample
{
    private static bool started;

    public static void Register()
    {
        Lifecycle.OnStarted(() => started = true);
        Lifecycle.OnUpdate(UpdateIfStarted);
        Lifecycle.OnDestroyed(() => started = false);
        Lifecycle.OnNewGame(ClearTransientCache);
        Lifecycle.OnBeforeSave(FlushTransientCache);
        Lifecycle.OnBeforeLoad(_ => ClearTransientCache());
        Lifecycle.OnAfterLoad(RebuildTransientCache);
        Lifecycle.OnSaveDeleted(RemoveExternalIndexForSave);
    }

    private static void UpdateIfStarted()
    {
        if (!started)
        {
            return;
        }

        // Keep per-frame work small. Use specific DSPCore capabilities for game object lifecycles.
    }

    private static void ClearTransientCache()
    {
    }

    private static void FlushTransientCache(string saveName)
    {
    }

    private static void RebuildTransientCache()
    {
    }

    private static void RemoveExternalIndexForSave(string saveName)
    {
    }
}
