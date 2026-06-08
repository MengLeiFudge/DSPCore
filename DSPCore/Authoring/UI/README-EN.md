# UI Common Framework

The UI block provides common Unity UI scaffolding for DSPCore: window lifecycle helpers, tabbed windows, base controls, declarative grid layout, themed cards, and small descriptor objects.

It does not provide concrete business pages. A mod's own pages, navigation, unlock rules, save state, and business buttons remain owned by that mod or by the relevant authoring capability.

## What This Block Gives You

- Shared window creation, initialization, update, close, and destroy flow, so each mod does not need to patch `UIRoot` again.
- Reusable controls such as check boxes, combo boxes, sliders, flat buttons, image buttons, and image button groups.
- `GridDsl`, `LayoutGrid`, `LayoutTrack`, and `LayoutInsets` for fixed-size UI layout without hand-written coordinate math in every page.
- `UiPageLayout` and `UiRoundedSpriteFactory` for consistent rounded cards, headers, footers, scroll cards, and state borders.
- Lightweight descriptor objects such as `UiButtonDescriptor` and `UiNodeFactory` for sharing UI metadata across capabilities.

## Capability: Window Lifecycle

`UiWindow` is the common window base class. It reuses a vanilla window template and provides title, fixed window size, and common control creation helpers.

`UiWindowManager.CreateWindow<T>(name, title)` creates and registers a window. At runtime, DSPCore wires `UiWindowRuntimePatches` into `UIRoot`:

- After `UIRoot._OnOpen`, registered windows are initialized.
- After `UIRoot._OnUpdate`, window updates are forwarded.
- After `UIGame.ShutAllFunctionWindow`, functional windows are closed.
- After `UIRoot._OnDestroy`, windows are freed and destroyed.

Mods still decide when to create a window, when to open it, and what the window displays.

## Capability: Tabbed Windows

`UiTabbedWindow` provides a common tabbed window shell:

- Tab buttons.
- Tab content containers.
- Selection, visibility, and refresh hooks.

It only handles generic tabs inside a custom window. It is not the same as item, recipe, or replicator tabs in the game UI; those remain owned by the `Tabs` capability.

## Capability: Base Controls

Current control wrappers include:

- `UiCheckButton`
- `UiCheckBox`
- `UiComboBox`
- `UiCornerComboBox`
- `UiFlatButton`
- `UiImageButton`
- `UiImageButtonGroup`
- `UiSideSlider`
- `UiSlider`

These controls wrap Unity/DSP UI object creation, size, text, events, and common states. They do not contain mod-specific business logic.

## Capability: Declarative Grid Layout

`GridDsl` provides short factory methods:

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

`GridLayoutRuntime.BuildRoot(window, parent, root)` builds the layout nodes into a `RectTransform` hierarchy.

## Capability: Theme Cards

`UiPageLayout` owns common page structure:

- Page headers.
- Content cards.
- Strong cards.
- Footers.
- Scrollable content cards.
- Empty-state hints.
- Selected, hover, and normal card borders.

`UiRoundedSpriteFactory` creates rounded fill and border sprites so pages do not need to generate those textures repeatedly.

## Capability: Descriptors

`UiNodeFactory` can still create lightweight descriptor objects:

```csharp
var button = new UiNodeFactory().Button(
    id: "example.copy",
    title: "Copy",
    tooltip: "Copy error text");
```

The return value is a `UiButtonDescriptor` containing only `Id`, `Title`, and `Tooltip`. It is useful for cross-feature metadata and is not automatically attached to a game window.

## What This Block Does Not Own

- It does not register concrete pages.
- It does not decide which game entry point opens a page.
- It does not own item, recipe, icon, or picker content.
- It does not save business window state.
- It does not handle a mod's own unlock conditions, categories, tasks, markets, or other business navigation.
- It does not replace capabilities that already have clear ownership, such as `Tabs`, `PickerSurfaces`, and `ErrorWindow`.
