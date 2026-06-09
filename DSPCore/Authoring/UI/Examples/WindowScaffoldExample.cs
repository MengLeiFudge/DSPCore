using BepInEx.Configuration;
using UnityEngine;

namespace DSPCore.Examples;

/// <summary>
/// 展示如何用 DSPCore UI 框架创建一个自定义窗口。
/// Demonstrates how to create a custom window with the DSPCore UI framework.
/// </summary>
public sealed class WindowScaffoldExample
{
    private readonly ConfigEntry<bool> enableFeatureConfig;
    private ExampleWindow? window;

    public WindowScaffoldExample(ConfigEntry<bool> enableFeatureConfig)
    {
        this.enableFeatureConfig = enableFeatureConfig;
    }

    /// <summary>
    /// 创建并登记窗口；调用方仍然负责选择合适的打开时机。
    /// Creates and registers the window; the caller still chooses when to open it.
    /// </summary>
    public void CreateWindow()
    {
        window = UiWindowManager.CreateWindow<ExampleWindow>("example-window", "Example");
        window.Configure(enableFeatureConfig);
    }

    /// <summary>
    /// 从模组自己的按钮、快捷键或菜单回调里打开窗口。
    /// Opens the window from the mod's own button, key bind, or menu callback.
    /// </summary>
    public void OpenWindow()
    {
        window?.Open();
    }

    private sealed class ExampleWindow : UiWindow
    {
        private ConfigEntry<bool>? enableFeatureConfig;

        public void Configure(ConfigEntry<bool> config)
        {
            enableFeatureConfig = config;
        }

        protected override void _OnCreate()
        {
            base._OnCreate();
            RectTransform root = GetComponent<RectTransform>();
            var statusRows = new[]
            {
                new UiFormRowDescriptor("Feature", enableFeatureConfig?.Value.ToString() ?? "No config bound"),
                new UiFormRowDescriptor("Owner", "ExampleMod"),
            };

            var layout = GridDsl.Grid(
                rows: [GridDsl.Px(72f), GridDsl.Fr(1f), GridDsl.Px(56f)],
                cols: [GridDsl.Fr(1f)],
                rowGap: UiPageLayout.Gap,
                children:
                [
                    GridDsl.Header("Example", "DSPCore UI scaffold", row: 0, col: 0),
                    GridDsl.FormCard("Settings", statusRows, row: 1, col: 0),
                    GridDsl.StatusFooter("Concrete behavior remains in the owning mod.", "Close", Close, row: 2, col: 0)
                ]);

            GridDsl.BuildLayout(this, root, layout);
        }
    }
}
