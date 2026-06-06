# UI 通用框架

UI 功能块提供 DSPCore 级别的通用 Unity UI 架子：窗口生命周期、标签页窗口、基础控件、声明式网格布局、主题卡片和少量描述对象。

它不提供具体业务页面。模组自己的页面、导航、解锁条件、存档状态和业务按钮仍由模组或对应功能块负责。

## 这个模块带来什么便利

- 统一窗口创建、初始化、更新、关闭和销毁流程，减少每个模组重复 patch `UIRoot`。
- 复用常见控件封装，例如勾选框、下拉框、滑条、扁平按钮、图片按钮和图片按钮组。
- 用 `GridDsl`、`LayoutGrid`、`LayoutTrack` 和 `LayoutInsets` 描述固定尺寸 UI 布局，避免每个页面手写坐标计算。
- 用 `UiPageLayout` 和 `UiRoundedSpriteFactory` 创建一致的圆角卡片、页头、页脚、滚动卡片和状态边框。
- 继续保留轻量描述对象，例如 `UiButtonDescriptor` 和 `UiNodeFactory`，供跨功能块共享 UI 元数据。

## 功能：窗口生命周期

`UiWindow` 是通用窗口基类，负责复用原版窗口模板、标题、固定窗口尺寸和常见控件创建方法。

`UiWindowManager.CreateWindow<T>(name, title)` 会创建并登记窗口。DSPCore 在运行时通过 `UiWindowRuntimePatches` 接入 `UIRoot`：

- `UIRoot._OnOpen` 后初始化已登记窗口。
- `UIRoot._OnUpdate` 后转发窗口更新。
- `UIGame.ShutAllFunctionWindow` 后关闭功能窗口。
- `UIRoot._OnDestroy` 后释放并销毁窗口。

模组仍需要自己决定什么时候创建窗口、打开窗口，以及窗口内部显示什么内容。

## 功能：标签页窗口

`UiTabbedWindow` 提供一个通用的页签窗口壳：

- 左侧/顶部页签按钮。
- 页签内容容器。
- 页签选中、隐藏和刷新入口。

它只处理窗口里的通用标签页交互，不等同于游戏物品/配方/制造器分页。后者仍属于 `Tabs` 功能块。

## 功能：基础控件

当前控件封装包括：

- `UiCheckButton`
- `UiCheckBox`
- `UiComboBox`
- `UiCornerComboBox`
- `UiFlatButton`
- `UiImageButton`
- `UiImageButtonGroup`
- `UiSideSlider`
- `UiSlider`

这些控件只封装 Unity/DSP UI 对象创建、尺寸、文本、事件和常见状态。它们不包含任何具体模组业务逻辑。

## 功能：声明式网格布局

`GridDsl` 提供短工厂方法：

```csharp
var root = GridDsl.Grid(
    rows: [GridDsl.Px(72f), GridDsl.Fr(1f)],
    cols: [GridDsl.Fr(1f)],
    rowGap: 16f,
    children:
    [
        GridDsl.Header("Title", "Summary", row: 0, col: 0),
        GridDsl.ContentCard(row: 1, col: 0)
    ]);
```

`GridLayoutRuntime.BuildRoot(window, parent, root)` 会把布局节点构造成 `RectTransform` 层级。

## 功能：主题卡片

`UiPageLayout` 负责常见页面骨架：

- 页头。
- 内容卡片。
- 强调卡片。
- 页脚。
- 滚动内容卡片。
- 空态提示。
- 卡片选中/悬停/普通状态边框。

`UiRoundedSpriteFactory` 负责生成圆角填充和描边 sprite，避免每个页面重复生成贴图。

## 功能：描述对象

`UiNodeFactory` 仍可创建轻量描述对象：

```csharp
var button = new UiNodeFactory().Button(
    id: "example.copy",
    title: "Copy",
    tooltip: "Copy error text");
```

返回值是 `UiButtonDescriptor`，只包含 `Id`、`Title` 和 `Tooltip`。它适合做跨功能块元数据，不会自动挂到游戏窗口。

## 这个模块不负责什么

- 不注册具体页面。
- 不决定页面出现在游戏哪个入口。
- 不负责物品、配方、图标或选择器内容。
- 不保存窗口业务状态。
- 不处理模组自己的解锁条件、分类、任务、市场或其他业务导航。
- 不替代 `Tabs`、`Pickers`、`Errors` 等已经有明确归属的功能块。
