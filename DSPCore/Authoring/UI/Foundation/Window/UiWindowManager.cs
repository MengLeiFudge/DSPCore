#nullable disable
using System.Collections.Generic;

namespace DSPCore;

/// <summary>
/// DSPCore UI 窗口的创建、释放与 Unity 生命周期转发管理器。
/// Manager for creating, freeing, and forwarding Unity lifecycle events to DSPCore UI windows.
/// </summary>
public abstract class UiWindowManager
{
    private static readonly List<ManualBehaviour> Windows = new(4);

    /// <summary>
    /// 指示当前窗口集合是否已接入 UIRoot 生命周期。
    /// Indicates whether the current window set has been initialized against UIRoot.
    /// </summary>
    public static bool Initialized { get; private set; }

    /// <summary>
    /// 初始化 DSPCore UI 控件和窗口模板对象。
    /// Initializes DSPCore UI control and window template objects.
    /// </summary>
    public static void InitBaseObjects()
    {
        UiWindow.InitBaseObject();
        UiCheckButton.InitBaseObject();
        UiCheckBox.InitBaseObject();
        UiComboBox.InitBaseObject();
        UiCornerComboBox.InitBaseObject();
        UiFlatButton.InitBaseObject();
        UiImageButton.InitBaseObject();
    }

    /// <summary>
    /// 创建并登记一个 DSPCore UI 窗口。
    /// Creates and registers a DSPCore UI window.
    /// </summary>
    public static T CreateWindow<T>(string name, string title = "") where T : UiWindow
    {
        var win = UiWindow.Create<T>(name, title);
        if (win)
        {
            Windows.Add(win);
        }

        return win;
    }

    /// <summary>
    /// 释放并销毁一个已登记窗口。
    /// Frees and destroys a registered window.
    /// </summary>
    public static void DestroyWindow(ManualBehaviour win)
    {
        if (win == null)
        {
            return;
        }

        Windows.Remove(win);
        win._Free();
        win._Destroy();
    }

    internal static void InitAllWindows()
    {
        if (Initialized || !UIRoot.instance)
        {
            return;
        }

        InitBaseObjects();
        foreach (var win in Windows)
        {
            win._Init(win.data);
        }

        Initialized = true;
    }

    internal static void FreeAllWindows()
    {
        foreach (var win in Windows)
        {
            win._Free();
            win._Destroy();
        }

        Windows.Clear();
        Initialized = false;
    }

    internal static void UpdateAllWindows()
    {
        if (GameMain.isPaused || !GameMain.isRunning)
        {
            return;
        }

        foreach (var win in Windows)
        {
            win._Update();
        }
    }

    internal static void CloseFunctionalWindows()
    {
        foreach (var win in Windows)
        {
            if (win is UiWindow theWin && theWin.IsWindowFunctional())
            {
                theWin.TryClose();
            }
        }
    }
}
