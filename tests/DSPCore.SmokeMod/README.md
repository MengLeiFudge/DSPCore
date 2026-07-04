# DSPCore Smoke Mod

这是一个手动游戏内验证插件，不进入 Thunderstore 正式包。它把当前无法由仓库内纯逻辑测试证明的 UI / 点击 / 运行时入口集中到一个最小 BepInEx 插件里。

## 构建

```bash
dotnet build tests/DSPCore.SmokeMod/DSPCore.SmokeMod.csproj -p:ProfileDir=/mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/Default -p:DSPGameDir="/mnt/d/Steam/steamapps/common/Dyson Sphere Program"
```

输出：

```text
tests/DSPCore.SmokeMod/bin/Debug/DSPCore.SmokeMod.dll
```

## 安装到本地测试 profile

先构建，再执行：

```bash
tests/install-smoke-mod.sh /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/Default
```

也可以手动复制下列 DLL。

复制下列 DLL 到同一个 BepInEx profile：

- `DSPCore/bin/Debug/DSPCore.dll` -> `BepInEx/plugins/DSPCore/DSPCore.dll`
- `DSPCore.Preloader/bin/Debug/DSPCore.Preloader.dll` -> `BepInEx/patchers/DSPCore.Preloader.dll`
- `tests/DSPCore.SmokeMod/bin/Debug/DSPCore.SmokeMod.dll` -> `BepInEx/plugins/DSPCore.SmokeMod/DSPCore.SmokeMod.dll`
- 可选：`DSPCore.NebulaAdapter/bin/Debug/DSPCore.NebulaAdapter.dll` -> `BepInEx/plugins/DSPCore/DSPCore.NebulaAdapter.dll`

## 游戏内检查项

1. 启动游戏后日志应出现 `DSPCore smoke declarations registered.`。
2. 原版设置窗口应出现 DSPCore 分页，并能看到 `DSPCore Smoke 验证` 页面。
3. Smoke 页面应显示 bool、enum、int range 和内容投射开关；七个可重绑定按键应出现在原版按键页。
4. 按 `Ctrl+F6` 应刷新 GlobalSaves 并写入日志，不打开玩家窗口。
5. 按 `Ctrl+F7` 应记录建造栏绑定改为使用原版建造栏热键交互，不打开编辑器窗口。
6. 按 `Ctrl+F8` 应打开原版设置窗口并切到 DSPCore 分页。
7. 按 `Ctrl+F9` 应写入一条 `SmokeManualReport`，再触发 fatal/error copy 时诊断文本应包含该报告。
8. 建造栏 category 3 的第 1 行第 11 格应尝试绑定铁块；第 2 行第 11 格应尝试绑定铜块。若被其他模组占位，日志/绑定结果用于确认冲突策略。
9. 按住建造栏重绑键点击槽位应打开物品 picker 并写入覆盖；按住清空键悬停槽位应显式清空。
10. 专用内容投射 smoke profile 中，把 `DSPCore.Smoke.EnableContentProjection` 配置为 `true` 后启动/读档，日志应出现 `DSPCore smoke content state: iconApplied=True`，证明 PNG 图标加载并绑定到原版铁块；同一行还应有 `recipeMapped=True` 和 `recipeTypeValue=20`，证明 smoke 自注册配方被标记到 DSPCore 自定义配方类型。该开关默认关闭，避免普通测试 profile 中的导出器或其他工具把 smoke 自定义配方当成真实业务内容处理。

## Nebula 双端检查项

两端都安装 DSPCore、DSPCore_NebulaAdapter、NebulaMultiplayerModApi、NebulaMultiplayerMod 和本 smoke 插件后，再做这组检查。

如果只有一个现成 profile，可以先复制出隔离的主机/客户端 smoke profile。这个脚本会拒绝覆盖已有目标目录，并且只在复制出来的 profile 中把 Nebula 本体、IlLine 和 NebulaCompatibilityAssist 的 `.old` 文件恢复为启用状态：

```bash
tests/prepare-nebula-smoke-profiles.sh \
  /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/Default \
  /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/DSPCoreSmokeHost \
  /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/DSPCoreSmokeClient
```

如果隔离 profile 已经存在，用下列命令更新当前构建产物和自动模式配置：

```bash
tests/update-nebula-smoke-profile.sh /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/DSPCoreSmokeHost Host
tests/update-nebula-smoke-profile.sh /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/DSPCoreSmokeClient Client 127.0.0.1:8469
```

开始前分别检查主机和客户端 profile：

```bash
tests/verify-nebula-profile-ready.sh /path/to/host-profile
tests/verify-nebula-profile-ready.sh /path/to/client-profile
```

如果不通过 r2modman 启动 profile，可以用启动辅助脚本临时切换游戏目录的 Doorstop 配置。脚本会先备份 `doorstop_config.ini`，启动游戏后恢复原文件：

```bash
tests/start-dsp-profile.sh /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/DSPCoreSmokeHost
tests/start-dsp-profile.sh /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/DSPCoreSmokeClient
tests/start-dsp-profile.sh /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/DSPCoreSmokeHost /mnt/d/Steam/steamapps/common/Dyson\ Sphere\ Program -- -nebula-server -batchmode -newgame 24681357 32 1
```

1. 启动后日志应出现 `DSPCore Nebula adapter is initialized.`，并且 smoke 设置页仍能打开。
2. 客户端按 `Ctrl+F10`，另一端日志应出现 `DSPCore smoke received packet`；`Ctrl+F6` 应只刷新 GlobalSaves 并写入日志，不打开玩家窗口。
3. 客户端按 `Ctrl+F11`，主机日志应出现 `DSPCore smoke host handled relay`。
4. 客户端进入一个星球后按 `Ctrl+F12`，主机日志应出现 `DSPCore smoke exported planet data`，客户端日志应出现 `DSPCore smoke imported planet data`。
5. 没装 DSPCore_NebulaAdapter 或当前不在联机房间时，这三个按键应只记录 `accepted=False`，不会抛异常。

也可以用 opt-in 自动模式减少手动点击：

- Host profile：把 `DSPCore.Smoke.AutoNebulaMode` 配置为 `Host`，并通过 Nebula 官方 dedicated 参数启动主机，例如 `tests/start-dsp-profile.sh /path/to/host /path/to/game -- -nebula-server -batchmode -newgame 24681357 32 1`；smoke 插件只记录 host-ready 日志，不在插件内部模拟建房。
- Client profile：把 `DSPCore.Smoke.AutoNebulaMode` 配置为 `Client`，并保持 `DSPCore.Smoke.AutoNebulaAddress = 127.0.0.1:8469`，启动后 smoke 插件会反射构造 `NebulaNetwork.Client` 并调用 `NebulaWorld.Multiplayer.JoinGame(...)`，读档完成后自动发送 packet、host relay 和 planet data request。
- 默认值是 `Off`；普通 smoke profile 不会自动建房、加房或发送联机数据。

## 日志检查

启动游戏并执行对应按键后，可用脚本检查 BepInEx 日志：

```bash
tests/verify-smoke-evidence.sh /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/Default/BepInEx/LogOutput.log startup ui-pages error offline-multiplayer
tests/verify-smoke-evidence.sh /path/to/content-smoke-profile/BepInEx/LogOutput.log startup content
tests/verify-smoke-evidence.sh /mnt/c/Users/MLJ/AppData/Roaming/r2modmanPlus-local/DysonSphereProgram/profiles/Default/BepInEx/LogOutput.log nebula
tests/verify-smoke-evidence.sh /path/to/host/BepInEx/LogOutput.log startup auto-nebula-host
tests/verify-smoke-evidence.sh /path/to/client/BepInEx/LogOutput.log startup auto-nebula-client
tests/verify-nebula-smoke-evidence.sh /path/to/host/BepInEx/LogOutput.log /path/to/client/BepInEx/LogOutput.log
```

## 边界

- 这个插件提供 Nebula 双端 packet route、host relay 和 planet data request/response 的固定验证入口；真正完成验证仍需要双端日志或截图证据。
- 内容投射 smoke 默认关闭；启用后会注册一个测试配方并把它标记为自定义配方类型，只应放在专用 smoke profile 验证。
- `tests/verify-smoke-evidence.sh ... nebula` 只适合单日志聚合或本机回环场景；真实双端房间验收应使用 `tests/verify-nebula-smoke-evidence.sh <host-log> <client-log>`。
- 这个插件不替代截图证据；最终验收仍需要真实游戏内截图、点击记录或日志片段。
- 这个插件默认不注册新 Proto，避免把普通 smoke 目标变成物品字段调参；只有开启内容投射 smoke 选项时才注册测试配方。
