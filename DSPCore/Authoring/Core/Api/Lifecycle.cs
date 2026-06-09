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
    private static readonly List<Action> NewGameHandlers = new();
    private static readonly List<Action<string>> BeforeSaveHandlers = new();
    private static readonly List<Action<string>> BeforeLoadHandlers = new();
    private static readonly List<Action> AfterLoadHandlers = new();
    private static readonly List<Action<string>> SaveDeletedHandlers = new();
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

    /// <summary>
    /// 注册新游戏开始后的回调。
    /// Registers a callback that runs after a new game starts.
    /// </summary>
    /// <param name="handler">新游戏回调。New-game callback.</param>
    public static void OnNewGame(Action handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        NewGameHandlers.Add(handler);
    }

    /// <summary>
    /// 注册保存当前游戏前的回调。
    /// Registers a callback that runs before the current game is saved.
    /// </summary>
    /// <param name="handler">保存前回调，参数是存档名。Before-save callback with the save name.</param>
    public static void OnBeforeSave(Action<string> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        BeforeSaveHandlers.Add(handler);
    }

    /// <summary>
    /// 注册读取当前游戏前的回调。
    /// Registers a callback that runs before the current game is loaded.
    /// </summary>
    /// <param name="handler">读取前回调，参数是存档名。Before-load callback with the save name.</param>
    public static void OnBeforeLoad(Action<string> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        BeforeLoadHandlers.Add(handler);
    }

    /// <summary>
    /// 注册读取当前游戏后的回调。
    /// Registers a callback that runs after the current game has loaded.
    /// </summary>
    /// <param name="handler">读取后回调。After-load callback.</param>
    public static void OnAfterLoad(Action handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        AfterLoadHandlers.Add(handler);
    }

    /// <summary>
    /// 注册删除存档后的回调。
    /// Registers a callback that runs after a save is deleted.
    /// </summary>
    /// <param name="handler">删除后回调，参数是存档名。After-delete callback with the save name.</param>
    public static void OnSaveDeleted(Action<string> handler)
    {
        if (handler == null)
        {
            throw new ArgumentNullException(nameof(handler));
        }

        SaveDeletedHandlers.Add(handler);
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
        NewGameHandlers.Clear();
        BeforeSaveHandlers.Clear();
        BeforeLoadHandlers.Clear();
        AfterLoadHandlers.Clear();
        SaveDeletedHandlers.Clear();
        started = false;
    }

    internal static void RaiseNewGame()
    {
        InvokeAll(NewGameHandlers);
    }

    internal static void RaiseBeforeSave(string saveName)
    {
        InvokeAll(BeforeSaveHandlers, saveName);
    }

    internal static void RaiseBeforeLoad(string saveName)
    {
        InvokeAll(BeforeLoadHandlers, saveName);
    }

    internal static void RaiseAfterLoad()
    {
        InvokeAll(AfterLoadHandlers);
    }

    internal static void RaiseSaveDeleted(string saveName)
    {
        InvokeAll(SaveDeletedHandlers, saveName);
    }

    private static void InvokeAll(List<Action> handlers)
    {
        foreach (var handler in handlers.ToArray())
        {
            InvokeSafely(handler);
        }
    }

    private static void InvokeAll<T>(List<Action<T>> handlers, T value)
    {
        foreach (var handler in handlers.ToArray())
        {
            InvokeSafely(handler, value);
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

    private static void InvokeSafely<T>(Action<T> handler, T value)
    {
        try
        {
            handler(value);
        }
        catch (Exception ex)
        {
            DspCore.Errors.ReportException("DSPCore.Lifecycle", ex);
            DspCore.Logger?.LogError($"DSPCore lifecycle handler failed: {ex}");
        }
    }
}
