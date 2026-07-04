using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using BepInEx;
using UnityEngine;

namespace DSPCore.SmokeMod;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(DSPCorePlugin.PluginGuid)]
public sealed class SmokePlugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.menglei.dsp.core.smoke";
    public const string PluginName = "DSPCore Smoke Mod";
    public const string PluginVersion = "0.1.0";

    private const string PageId = "dspcore.smoke";
    private const string Section = "DSPCore.Smoke";
    private const string PacketId = PluginGuid + ".packet";
    private const string HostRelayId = PluginGuid + ".host-relay";
    private const string PlanetDataId = PluginGuid + ".planet-data";
    private const string SmokeIconId = PluginGuid + ".iron-icon";
    private const string SmokeRecipeTypeId = PluginGuid + ".recipe-type";
    private const string SmokeRecipeStableKey = "smoke-recipe-type-projection-v2";
    private const int IronIngotItemId = 1101;
    private const int CopperIngotItemId = 1104;
    private const int SmokeRecipePreferredId = 11999;
    private static bool contentProjectionEnabled;
    private static AutoNebulaMode autoNebulaMode;
    private static string autoNebulaAddress = "127.0.0.1:8469";
    private static int autoNebulaActionDelaySeconds = 20;

    private static readonly SmokeGlobalState GlobalState = Saves.GlobalAuto<SmokeGlobalState>(
        PluginGuid,
        initialize: state =>
        {
            state.OpenWindowCount = 0;
            state.ErrorReportCount = 0;
        });

    private bool openedFromKey;
    private int pendingContentSmokeChecks;
    private int autoNebulaStartAttempts;
    private int pendingAutoNebulaActionFrames;
    private bool autoNebulaStarted;
    private bool autoNebulaActionsSent;

    private void Awake()
    {
        RegisterLocalization();
        RegisterOptions();
        RegisterBuildBarSmoke();
        if (contentProjectionEnabled)
        {
            RegisterContentSmoke();
        }

        RegisterMultiplayerSmoke();
        RegisterKeyBinds();
        RegisterLifecycleLogs();

        Logger.LogInfo("DSPCore smoke declarations registered.");
    }

    private static void RegisterLocalization()
    {
        ModResources.Text("DSPCore.Smoke.Page", "zhCN", "DSPCore Smoke 验证", PluginGuid);
        ModResources.Text("DSPCore.Smoke.Page", "enUS", "DSPCore Smoke Checks", PluginGuid);
        ModResources.Text("DSPCore.Smoke.Enabled", "zhCN", "启用 Smoke 标记", PluginGuid);
        ModResources.Text("DSPCore.Smoke.Enabled", "enUS", "Enable smoke marker", PluginGuid);
        ModResources.Text("DSPCore.Smoke.Mode", "zhCN", "Smoke 模式", PluginGuid);
        ModResources.Text("DSPCore.Smoke.Mode", "enUS", "Smoke mode", PluginGuid);
        ModResources.Text("DSPCore.Smoke.Level", "zhCN", "Smoke 等级", PluginGuid);
        ModResources.Text("DSPCore.Smoke.Level", "enUS", "Smoke level", PluginGuid);
        ModResources.Text("DSPCore.Smoke.ContentProjection", "zhCN", "启用内容投射 Smoke", PluginGuid);
        ModResources.Text("DSPCore.Smoke.ContentProjection", "enUS", "Enable content projection smoke", PluginGuid);
        ModResources.Text("DSPCore.Smoke.AutoNebulaMode", "zhCN", "自动 Nebula Smoke 模式", PluginGuid);
        ModResources.Text("DSPCore.Smoke.AutoNebulaMode", "enUS", "Auto Nebula smoke mode", PluginGuid);
        ModResources.Text("DSPCore.Smoke.AutoNebulaAddress", "zhCN", "自动 Nebula 连接地址", PluginGuid);
        ModResources.Text("DSPCore.Smoke.AutoNebulaAddress", "enUS", "Auto Nebula join address", PluginGuid);
        ModResources.Text("DSPCore.Smoke.AutoNebulaDelay", "zhCN", "自动 Nebula 发送延迟", PluginGuid);
        ModResources.Text("DSPCore.Smoke.AutoNebulaDelay", "enUS", "Auto Nebula send delay", PluginGuid);
        ModResources.Text("DSPCore.Smoke.OpenGlobalSaves", "zhCN", "打开 DSPCore 全局数据", PluginGuid);
        ModResources.Text("DSPCore.Smoke.OpenGlobalSaves", "enUS", "Open DSPCore global data", PluginGuid);
        ModResources.Text("DSPCore.Smoke.OpenBuildBar", "zhCN", "打开 DSPCore 建造栏绑定", PluginGuid);
        ModResources.Text("DSPCore.Smoke.OpenBuildBar", "enUS", "Open DSPCore build bar bindings", PluginGuid);
        ModResources.Text("DSPCore.Smoke.OpenSettings", "zhCN", "打开 DSPCore Smoke 设置", PluginGuid);
        ModResources.Text("DSPCore.Smoke.OpenSettings", "enUS", "Open DSPCore smoke settings", PluginGuid);
        ModResources.Text("DSPCore.Smoke.ReportError", "zhCN", "生成 DSPCore Smoke 错误报告", PluginGuid);
        ModResources.Text("DSPCore.Smoke.ReportError", "enUS", "Create DSPCore smoke error report", PluginGuid);
        ModResources.Text("DSPCore.Smoke.SendPacket", "zhCN", "发送 DSPCore Smoke 联机包", PluginGuid);
        ModResources.Text("DSPCore.Smoke.SendPacket", "enUS", "Send DSPCore smoke packet", PluginGuid);
        ModResources.Text("DSPCore.Smoke.SendHostRelay", "zhCN", "发送 DSPCore Smoke 主机转发", PluginGuid);
        ModResources.Text("DSPCore.Smoke.SendHostRelay", "enUS", "Send DSPCore smoke host relay", PluginGuid);
        ModResources.Text("DSPCore.Smoke.RequestPlanetData", "zhCN", "请求 DSPCore Smoke 星球数据", PluginGuid);
        ModResources.Text("DSPCore.Smoke.RequestPlanetData", "enUS", "Request DSPCore smoke planet data", PluginGuid);
    }

    private static void RegisterOptions()
    {
        Options.RegisterPage(PageId, PluginGuid, "DSPCore.Smoke.Page", order: -900);
        Options.Bool(
            Section,
            "Enabled",
            true,
            "Smoke bool option; verify checkbox and reset.",
            new OptionUi(PageId, "DSPCore.Smoke.Enabled")
            {
                Order = 10,
                CanReset = true
            });
        Options.Enum(
            Section,
            "Mode",
            SmokeMode.Basic,
            "Smoke enum option; verify combo box.",
            new OptionUi(PageId, "DSPCore.Smoke.Mode")
            {
                Order = 20,
                CanReset = true
            });
        Options.IntRange(
            Section,
            "Level",
            3,
            "Smoke range option; verify slider and reset.",
            0,
            10,
            new OptionUi(PageId, "DSPCore.Smoke.Level")
            {
                Order = 30,
                CanReset = true
            });
        contentProjectionEnabled = Options.Bool(
            Section,
            "EnableContentProjection",
            false,
            "Registers a smoke recipe and icon binding; enable only in dedicated smoke profiles.",
            new OptionUi(PageId, "DSPCore.Smoke.ContentProjection")
            {
                Order = 40,
                CanReset = true
            });
        autoNebulaMode = Options.Enum(
            Section,
            "AutoNebulaMode",
            AutoNebulaMode.Off,
            "Optional smoke automation for Nebula room verification; Off by default.",
            new OptionUi(PageId, "DSPCore.Smoke.AutoNebulaMode")
            {
                Order = 50,
                CanReset = true
            });
        autoNebulaAddress = Options.String(
            Section,
            "AutoNebulaAddress",
            "127.0.0.1:8469",
            "Address used by AutoNebulaMode=Client.",
            new OptionUi(PageId, "DSPCore.Smoke.AutoNebulaAddress")
            {
                Order = 60,
                CanReset = true
            });
        autoNebulaActionDelaySeconds = Options.IntRange(
            Section,
            "AutoNebulaActionDelaySeconds",
            20,
            "Delay after load before the client sends packet, host relay, and planet data smoke requests.",
            minimum: 5,
            maximum: 120,
            ui: new OptionUi(PageId, "DSPCore.Smoke.AutoNebulaDelay")
            {
                Order = 70,
                CanReset = true
            });
    }

    private static void RegisterBuildBarSmoke()
    {
        BuildBar.BindQuickBarWithResult(
            category: 3,
            row: 1,
            index: 11,
            itemId: IronIngotItemId,
            conflictPolicy: BuildBarConflictPolicy.KeepExisting);
        BuildBar.BindQuickBarWithResult(
            category: 3,
            row: 2,
            index: 11,
            itemId: CopperIngotItemId,
            conflictPolicy: BuildBarConflictPolicy.KeepExisting);
    }

    private static void RegisterContentSmoke()
    {
        var iconPath = EnsureSmokeIconFile();
        Icons.BindToProto(
            SmokeIconId,
            PluginGuid,
            iconPath,
            ProtoKind.Item,
            IronIngotItemId);
        ProtoRegistration.DataUpdates(PluginGuid, data =>
        {
            var recipe = new RecipeProto
            {
                ID = SmokeRecipePreferredId,
                Name = "DSPCoreSmokeRecipe",
                name = "DSPCoreSmokeRecipe",
                Type = ERecipeType.Assemble,
                Items = new[] { IronIngotItemId },
                ItemCounts = new[] { 1 },
                Results = new[] { CopperIngotItemId },
                ResultCounts = new[] { 1 },
                TimeSpend = 60,
                GridIndex = GridIndexes.From(3, 2, 12),
                IconTag = "dspcore-smoke-recipe",
                NonProductive = true
            };
            data.RegisterRecipe(
                recipe,
                ProtoStableId.Of(SmokeRecipeStableKey, SmokeRecipePreferredId),
                "Declare DSPCore smoke recipe type projection target");
            GlobalState.SmokeRecipeId = recipe.ID;
            GameEnums.RegisterRecipeType(
                SmokeRecipeTypeId,
                PluginGuid,
                "DSPCore Smoke Recipe Type",
                new[] { recipe.ID },
                new[] { CopperIngotItemId });
        });
    }

    private void RegisterMultiplayerSmoke()
    {
        Multiplayer.RegisterPacket(PacketId, PluginGuid, payload =>
        {
            GlobalState.PacketReceiveCount++;
            GlobalState.LastMultiplayerUtc = DateTime.UtcNow.ToString("O");
            Logger.LogInfo("DSPCore smoke received packet: " + DecodePayload(payload));
            Saves.SaveGlobal();
        });
        Multiplayer.RegisterHostRelay(HostRelayId, PluginGuid, payload =>
        {
            GlobalState.HostRelayReceiveCount++;
            GlobalState.LastMultiplayerUtc = DateTime.UtcNow.ToString("O");
            Logger.LogInfo("DSPCore smoke host handled relay: " + DecodePayload(payload));
            Saves.SaveGlobal();
        });
        Multiplayer.RegisterPlanetData(
            PlanetDataId,
            PluginGuid,
            planetId =>
            {
                GlobalState.PlanetDataExportCount++;
                GlobalState.LastMultiplayerUtc = DateTime.UtcNow.ToString("O");
                Logger.LogInfo("DSPCore smoke exported planet data for planet " + planetId);
                Saves.SaveGlobal();
                return EncodePayload("planet=" + planetId + "; exported=" + GlobalState.PlanetDataExportCount);
            },
            (planetId, payload) =>
            {
                GlobalState.PlanetDataImportCount++;
                GlobalState.LastMultiplayerUtc = DateTime.UtcNow.ToString("O");
                Logger.LogInfo("DSPCore smoke imported planet data for planet " + planetId + ": " + DecodePayload(payload));
                Saves.SaveGlobal();
            });
        Multiplayer.RegisterClientIntoOtherSave(PluginGuid, () =>
        {
            GlobalState.ClientIntoOtherSaveCount++;
            GlobalState.LastMultiplayerUtc = DateTime.UtcNow.ToString("O");
            Logger.LogInfo("DSPCore smoke observed multiplayer client missing-save initializer.");
            Saves.SaveGlobal();
        });
    }

    private void RegisterKeyBinds()
    {
        KeyBinds.Register(
            id: "open-global-saves",
            ownerModGuid: PluginGuid,
            displayName: "DSPCore.Smoke.OpenGlobalSaves",
            defaultKey: "Ctrl+F6",
            callback: OpenGlobalSaves,
            conflictGroup: 8801,
            displayPageId: PageId);
        KeyBinds.Register(
            id: "open-buildbar-editor",
            ownerModGuid: PluginGuid,
            displayName: "DSPCore.Smoke.OpenBuildBar",
            defaultKey: "Ctrl+F7",
            callback: OpenBuildBarEditor,
            conflictGroup: 8801,
            displayPageId: PageId);
        KeyBinds.Register(
            id: "open-smoke-settings",
            ownerModGuid: PluginGuid,
            displayName: "DSPCore.Smoke.OpenSettings",
            defaultKey: "Ctrl+F8",
            callback: OpenSmokeSettings,
            conflictGroup: 8801,
            displayPageId: PageId);
        KeyBinds.Register(
            id: "report-smoke-error",
            ownerModGuid: PluginGuid,
            displayName: "DSPCore.Smoke.ReportError",
            defaultKey: "Ctrl+F9",
            callback: ReportSmokeError,
            conflictGroup: 8801,
            displayPageId: PageId);
        KeyBinds.Register(
            id: "send-smoke-packet",
            ownerModGuid: PluginGuid,
            displayName: "DSPCore.Smoke.SendPacket",
            defaultKey: "Ctrl+F10",
            callback: SendSmokePacket,
            conflictGroup: 8802,
            displayPageId: PageId);
        KeyBinds.Register(
            id: "send-smoke-host-relay",
            ownerModGuid: PluginGuid,
            displayName: "DSPCore.Smoke.SendHostRelay",
            defaultKey: "Ctrl+F11",
            callback: SendSmokeHostRelay,
            conflictGroup: 8802,
            displayPageId: PageId);
        KeyBinds.Register(
            id: "request-smoke-planet-data",
            ownerModGuid: PluginGuid,
            displayName: "DSPCore.Smoke.RequestPlanetData",
            defaultKey: "Ctrl+F12",
            callback: RequestSmokePlanetData,
            conflictGroup: 8802,
            displayPageId: PageId);
    }

    private void RegisterLifecycleLogs()
    {
        Lifecycle.OnStarted(() => Logger.LogInfo("DSPCore smoke observed Lifecycle.OnStarted."));
        Lifecycle.OnNewGame(() => Logger.LogInfo("DSPCore smoke observed Lifecycle.OnNewGame."));
        Lifecycle.OnAfterLoad(() =>
        {
            Logger.LogInfo("DSPCore smoke observed Lifecycle.OnAfterLoad.");
            if (contentProjectionEnabled)
            {
                pendingContentSmokeChecks = 600;
                ProbeContentSmoke(forceLog: false);
            }

            if (autoNebulaMode == AutoNebulaMode.Client)
            {
                pendingAutoNebulaActionFrames = Math.Max(1, autoNebulaActionDelaySeconds) * 60;
                Logger.LogInfo("DSPCore smoke scheduled auto Nebula client sends after load. delaySeconds=" + autoNebulaActionDelaySeconds);
            }
        });
        Lifecycle.OnUpdate(() =>
        {
            ProbeContentSmoke(forceLog: false);
            ProbeAutoNebulaSmoke();
        });
        Lifecycle.OnBeforeSave(saveName => Logger.LogInfo("DSPCore smoke observed Lifecycle.OnBeforeSave: " + saveName));
        Lifecycle.OnDestroyed(() =>
        {
            GlobalState.LastDestroyedUtc = DateTime.UtcNow.ToString("O");
            Saves.SaveGlobal();
        });
    }

    private void OpenGlobalSaves()
    {
        GlobalState.OpenGlobalSavesCount++;
        GlobalState.LastOpenUtc = DateTime.UtcNow.ToString("O");
        Options.OpenGlobalSavesWindow();
        Saves.SaveGlobal();
        Logger.LogInfo("DSPCore smoke opened global saves from key bind. Count=" + GlobalState.OpenGlobalSavesCount);
        LogWindowState("global saves", "dspcore-global-saves-window");
    }

    private void OpenBuildBarEditor()
    {
        GlobalState.OpenBuildBarEditorCount++;
        GlobalState.LastOpenUtc = DateTime.UtcNow.ToString("O");
        BuildBar.OpenEditor();
        Saves.SaveGlobal();
        Logger.LogInfo("DSPCore smoke opened build bar editor from key bind. Count=" + GlobalState.OpenBuildBarEditorCount);
        LogWindowState("build bar editor", "dspcore-buildbar-override-window");
    }

    private void OpenSmokeSettings()
    {
        openedFromKey = true;
        GlobalState.OpenWindowCount++;
        GlobalState.LastOpenUtc = DateTime.UtcNow.ToString("O");
        Options.OpenWindow();
        Saves.SaveGlobal();
        Logger.LogInfo("DSPCore smoke opened settings from key bind. Count=" + GlobalState.OpenWindowCount);
        LogWindowState("settings", "dspcore-options-window");
    }

    private void LogWindowState(string label, string objectName)
    {
        GameObject? found = null;
        foreach (var candidate in Resources.FindObjectsOfTypeAll<GameObject>())
        {
            if (candidate.name == objectName)
            {
                found = candidate;
                break;
            }
        }

        Logger.LogInfo(
            "DSPCore smoke window state " + label +
            ": exists=" + (found != null) +
            ", activeSelf=" + (found != null && found.activeSelf) +
            ", activeInHierarchy=" + (found != null && found.activeInHierarchy));
    }

    private void ReportSmokeError()
    {
        GlobalState.ErrorReportCount++;
        GlobalState.LastErrorUtc = DateTime.UtcNow.ToString("O");
        var context = new ErrorDiagnosticContext(
            PlanetId: GameMain.localPlanet?.id,
            EntityId: null,
            Note: "DSPCore smoke error report triggered by key bind. openedFromKey=" + openedFromKey);
        Errors.Report(
            PluginGuid,
            "SmokeManualReport",
            "Manual DSPCore smoke report.",
            Environment.StackTrace,
            context);
        Saves.SaveGlobal();
        Logger.LogWarning("DSPCore smoke reported a manual diagnostic entry. Count=" + GlobalState.ErrorReportCount);
    }

    private void SendSmokePacket()
    {
        GlobalState.PacketSendCount++;
        GlobalState.LastMultiplayerUtc = DateTime.UtcNow.ToString("O");
        var accepted = Multiplayer.SendPacket(PacketId, EncodePayload("packet-send=" + GlobalState.PacketSendCount));
        Saves.SaveGlobal();
        Logger.LogInfo("DSPCore smoke packet send accepted=" + accepted + ", hasTransport=" + Multiplayer.HasTransport + ", count=" + GlobalState.PacketSendCount);
    }

    private void SendSmokeHostRelay()
    {
        GlobalState.HostRelaySendCount++;
        GlobalState.LastMultiplayerUtc = DateTime.UtcNow.ToString("O");
        var accepted = Multiplayer.SendHostRelay(HostRelayId, EncodePayload("host-relay-send=" + GlobalState.HostRelaySendCount));
        Saves.SaveGlobal();
        Logger.LogInfo("DSPCore smoke host relay send accepted=" + accepted + ", hasTransport=" + Multiplayer.HasTransport + ", count=" + GlobalState.HostRelaySendCount);
    }

    private void RequestSmokePlanetData()
    {
        var planetId = GameMain.localPlanet?.id ?? 1;
        GlobalState.PlanetDataRequestCount++;
        GlobalState.LastMultiplayerUtc = DateTime.UtcNow.ToString("O");
        var accepted = Multiplayer.RequestPlanetData(PlanetDataId, planetId);
        Saves.SaveGlobal();
        Logger.LogInfo("DSPCore smoke planet data request accepted=" + accepted + ", hasTransport=" + Multiplayer.HasTransport + ", planet=" + planetId + ", count=" + GlobalState.PlanetDataRequestCount);
    }

    private void ProbeAutoNebulaSmoke()
    {
        if (autoNebulaMode == AutoNebulaMode.Off)
        {
            return;
        }

        if (!autoNebulaStarted)
        {
            TryStartAutoNebula();
        }

        if (autoNebulaMode != AutoNebulaMode.Client || autoNebulaActionsSent || pendingAutoNebulaActionFrames <= 0)
        {
            return;
        }

        pendingAutoNebulaActionFrames--;
        if (pendingAutoNebulaActionFrames > 0)
        {
            return;
        }

        SendSmokePacket();
        SendSmokeHostRelay();
        RequestSmokePlanetData();
        autoNebulaActionsSent = true;
        Logger.LogInfo("DSPCore smoke auto Nebula client sends completed.");
    }

    private void TryStartAutoNebula()
    {
        if (autoNebulaStartAttempts > 1800)
        {
            return;
        }

        autoNebulaStartAttempts++;
        if (!IsNebulaMenuReady())
        {
            if (autoNebulaStartAttempts == 1800)
            {
                Logger.LogWarning("DSPCore smoke auto Nebula did not start: Nebula main menu was not ready.");
            }

            return;
        }

        try
        {
            if (autoNebulaMode == AutoNebulaMode.Host)
            {
                StartAutoNebulaHost();
            }
            else if (autoNebulaMode == AutoNebulaMode.Client)
            {
                StartAutoNebulaClient();
            }

            autoNebulaStarted = true;
        }
        catch (Exception ex)
        {
            Logger.LogError("DSPCore smoke auto Nebula start failed: " + ex);
            autoNebulaStarted = true;
        }
    }

    private static bool IsNebulaMenuReady()
    {
        var menuType = Type.GetType("NebulaPatcher.Patches.Dynamic.UIMainMenu_Patch, NebulaPatcher");
        var field = menuType?.GetField("mainMenuButtonGroup", BindingFlags.NonPublic | BindingFlags.Static);
        return field?.GetValue(null) != null;
    }

    private void StartAutoNebulaHost()
    {
        Logger.LogInfo("DSPCore smoke auto Nebula host mode is ready. Start the host with Nebula dedicated arguments.");
    }

    private void StartAutoNebulaClient()
    {
        var endpoint = ParseAutoNebulaEndpoint(autoNebulaAddress);
        var clientType = Type.GetType("NebulaNetwork.Client, NebulaNetwork")
            ?? throw new InvalidOperationException("NebulaNetwork.Client type is not available.");
        var multiplayerType = Type.GetType("NebulaWorld.Multiplayer, NebulaWorld")
            ?? throw new InvalidOperationException("NebulaWorld.Multiplayer type is not available.");
        var clientCtor = clientType.GetConstructor(new[] { typeof(IPEndPoint), typeof(string), typeof(string) })
            ?? throw new MissingMethodException("NebulaNetwork.Client..ctor(IPEndPoint, string, string)");
        var joinMethod = multiplayerType.GetMethod(
            "JoinGame",
            BindingFlags.Public | BindingFlags.Static,
            binder: null,
            types: new[] { clientType.GetInterface("NebulaModel.Networking.IClient") ?? clientType },
            modifiers: null)
            ?? multiplayerType.GetMethod("JoinGame", BindingFlags.Public | BindingFlags.Static)
            ?? throw new MissingMethodException("NebulaWorld.Multiplayer.JoinGame");
        var client = clientCtor.Invoke(new object[] { endpoint, "ws", string.Empty });
        Logger.LogInfo("DSPCore smoke auto Nebula client joining " + autoNebulaAddress + ".");
        joinMethod.Invoke(null, new[] { client });
    }

    private static IPEndPoint ParseAutoNebulaEndpoint(string address)
    {
        var host = address;
        var port = 8469;
        var split = address.LastIndexOf(':');
        if (split > 0 && split < address.Length - 1)
        {
            host = address.Substring(0, split);
            if (!int.TryParse(address.Substring(split + 1), out port))
            {
                port = 8469;
            }
        }

        if (!IPAddress.TryParse(host, out var ipAddress))
        {
            ipAddress = Dns.GetHostEntry(host).AddressList[0];
        }

        return new IPEndPoint(ipAddress, port);
    }

    private void ProbeContentSmoke(bool forceLog)
    {
        if (pendingContentSmokeChecks <= 0 && !forceLog)
        {
            return;
        }

        if (LogContentSmokeState(forceLog))
        {
            pendingContentSmokeChecks = 0;
            return;
        }

        if (pendingContentSmokeChecks > 0)
        {
            pendingContentSmokeChecks--;
            if (pendingContentSmokeChecks == 0)
            {
                LogContentSmokeState(forceLog: true);
            }
        }
    }

    private bool LogContentSmokeState(bool forceLog)
    {
        var item = LDB.items.Select(IronIngotItemId);
        var recipe = LDB.recipes.Select(GlobalState.SmokeRecipeId);
        var iconApplied = item != null && item.iconSprite != null;
        var recipeId = recipe == null ? 0 : recipe.ID;
        var recipeTypeId = string.Empty;
        var recipeMapped = false;
        if (recipeId > 0 && GameEnums.TryGetRecipeTypeForRecipe(recipeId, out var recipeType))
        {
            recipeMapped = true;
            recipeTypeId = recipeType.Id;
        }

        var recipeTypeValue = recipe == null ? 0 : (int)recipe.Type;
        var succeeded = iconApplied &&
            recipeMapped &&
            recipeTypeId == SmokeRecipeTypeId &&
            recipeTypeValue == GameEnums.CustomRecipeTypeValue;
        if (!succeeded && !forceLog)
        {
            return false;
        }

        Logger.LogInfo(
            "DSPCore smoke content state: iconApplied=" + iconApplied +
            ", recipeMapped=" + recipeMapped +
            ", recipeId=" + recipeId +
            ", recipeType=" + recipeTypeId +
            ", recipeTypeValue=" + recipeTypeValue);
        return succeeded;
    }

    private static string EnsureSmokeIconFile()
    {
        var directory = Path.Combine(Paths.ConfigPath, "DSPCore", "SmokeAssets");
        Directory.CreateDirectory(directory);
        var path = Path.Combine(directory, "smoke-icon.png");
        if (!File.Exists(path))
        {
            File.WriteAllBytes(path, SmokeIconPng);
        }

        return path;
    }

    private static byte[] EncodePayload(string text)
    {
        return Encoding.UTF8.GetBytes(text + "; utc=" + DateTime.UtcNow.ToString("O"));
    }

    private static string DecodePayload(byte[] payload)
    {
        return payload == null || payload.Length == 0 ? string.Empty : Encoding.UTF8.GetString(payload);
    }

    private static readonly byte[] SmokeIconPng =
    {
        0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A,
        0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
        0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01,
        0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
        0x89, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x44, 0x41,
        0x54, 0x78, 0x9C, 0x63, 0xF8, 0xCF, 0xC0, 0xF0,
        0x1F, 0x00, 0x05, 0x00, 0x01, 0xFF, 0x89, 0x99,
        0x3D, 0x1D, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45,
        0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
    };

    private enum SmokeMode
    {
        Basic,
        Extended,
        Stress
    }

    private enum AutoNebulaMode
    {
        Off,
        Host,
        Client
    }

    private sealed class SmokeGlobalState
    {
        [CoreSaveField("open_window_count")]
        public int OpenWindowCount { get; set; }

        [CoreSaveField("open_global_saves_count")]
        public int OpenGlobalSavesCount { get; set; }

        [CoreSaveField("open_buildbar_editor_count")]
        public int OpenBuildBarEditorCount { get; set; }

        [CoreSaveField("error_report_count")]
        public int ErrorReportCount { get; set; }

        [CoreSaveField("last_open_utc")]
        public string LastOpenUtc { get; set; } = string.Empty;

        [CoreSaveField("last_error_utc")]
        public string LastErrorUtc { get; set; } = string.Empty;

        [CoreSaveField("last_destroyed_utc")]
        public string LastDestroyedUtc { get; set; } = string.Empty;

        [CoreSaveField("packet_send_count")]
        public int PacketSendCount { get; set; }

        [CoreSaveField("packet_receive_count")]
        public int PacketReceiveCount { get; set; }

        [CoreSaveField("host_relay_send_count")]
        public int HostRelaySendCount { get; set; }

        [CoreSaveField("host_relay_receive_count")]
        public int HostRelayReceiveCount { get; set; }

        [CoreSaveField("planet_data_request_count")]
        public int PlanetDataRequestCount { get; set; }

        [CoreSaveField("planet_data_export_count")]
        public int PlanetDataExportCount { get; set; }

        [CoreSaveField("planet_data_import_count")]
        public int PlanetDataImportCount { get; set; }

        [CoreSaveField("client_into_other_save_count")]
        public int ClientIntoOtherSaveCount { get; set; }

        [CoreSaveField("last_multiplayer_utc")]
        public string LastMultiplayerUtc { get; set; } = string.Empty;

        [CoreSaveField("smoke_recipe_id")]
        public int SmokeRecipeId { get; set; }
    }
}
