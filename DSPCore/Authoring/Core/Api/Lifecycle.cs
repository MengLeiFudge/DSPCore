using System;
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// 作者侧 DSPCore 生命周期事件入口。
/// Author-facing entry point for DSPCore lifecycle events.
/// </summary>
public static class Lifecycle
{
    private static readonly List<Action> StartedHandlers = new();
    private static readonly List<Action> UpdateHandlers = new();
    private static readonly List<Action> DestroyedHandlers = new();
    private static bool started;

    /// <summary>
    /// 注册 DSPCore 运行时启动后的回调。
    /// Registers a callback that runs after the DSPCore runtime has started.
    /// </summary>
    /// <param name="handler">启动后回调。Post-start callback.</param>
    public static void OnStarted(Action handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        if (started)
        {
            InvokeSafely(handler);
            return;
        }

        StartedHandlers.Add(handler);
    }

    /// <summary>
    /// 注册 DSPCore 每帧更新回调。
    /// Registers a callback that runs on every DSPCore update.
    /// </summary>
    /// <param name="handler">每帧回调。Update callback.</param>
    public static void OnUpdate(Action handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        UpdateHandlers.Add(handler);
    }

    /// <summary>
    /// 注册 DSPCore 运行时销毁回调。
    /// Registers a callback that runs when the DSPCore runtime is destroyed.
    /// </summary>
    /// <param name="handler">销毁回调。Destroy callback.</param>
    public static void OnDestroyed(Action handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        DestroyedHandlers.Add(handler);
    }

    internal static void RaiseStarted()
    {
        started = true;
        InvokeAll(StartedHandlers);
        StartedHandlers.Clear();
    }

    internal static void RaiseUpdate()
    {
        InvokeAll(UpdateHandlers);
    }

    internal static void RaiseDestroyed()
    {
        InvokeAll(DestroyedHandlers);
        UpdateHandlers.Clear();
        DestroyedHandlers.Clear();
        started = false;
    }

    private static void InvokeAll(List<Action> handlers)
    {
        foreach (var handler in handlers.ToArray())
        {
            InvokeSafely(handler);
        }
    }

    private static void InvokeSafely(Action handler)
    {
        try
        {
            handler();
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException("DSPCore.Lifecycle", ex);
            DspCore.Logger?.LogError($"DSPCore lifecycle handler failed: {ex}");
        }
    }
}
