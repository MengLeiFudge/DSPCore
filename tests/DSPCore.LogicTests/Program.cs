using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DSPCore;

namespace DSPCore.LogicTests;

internal static class Program
{
    private static readonly List<string> Failures = new();

    private static int Main()
    {
        Run("BuildBar conflicts and overrides", BuildBarConflictsAndOverrides);
        Run("Stable proto ID mapping", StableProtoIdMapping);
        Run("Options import and export", OptionsImportAndExport);
        Run("SaveBlock and AutoSave", SaveBlockAndAutoSave);
        Run("Global save overview", GlobalSaveOverview);
        Run("Localization fallback and overrides", LocalizationFallbackAndOverrides);
        Run("Resource roots resolve and read files", ResourceRootsResolveAndReadFiles);
        Run("Icon runtime loading and binding", IconRuntimeLoadingAndBinding);
        Run("Game enum type declarations", GameEnumTypeDeclarations);
        Run("Proto cache rebuild continues after failed step", ProtoCacheRebuildContinuesAfterFailedStep);
        Run("Proto cache rebuild filters items without prefab desc", ProtoCacheRebuildFiltersItemsWithoutPrefabDesc);
        Run("Picker layout fallback", PickerLayoutFallback);
        Run("Blueprint tagged parameter blocks", BlueprintTaggedParameterBlocks);
        Run("Component and planet lifecycle callbacks", ComponentAndPlanetLifecycleCallbacks);
        Run("Galaxy and star lifecycle callbacks", GalaxyAndStarLifecycleCallbacks);
        Run("Model clone projection", ModelCloneProjection);

        if (Failures.Count == 0)
        {
            Console.WriteLine("DSPCore logic tests passed.");
            return 0;
        }

        Console.Error.WriteLine("DSPCore logic tests failed:");
        foreach (var failure in Failures)
        {
            Console.Error.WriteLine("- " + failure);
        }

        return 1;
    }

    private static void Run(string name, Action test)
    {
        try
        {
            DspCore.ResetForTests();
            test();
            Console.WriteLine("[PASS] " + name);
        }
        catch (Exception ex)
        {
            Failures.Add(name + ": " + ex.Message);
        }
    }

    private static void BuildBarConflictsAndOverrides()
    {
        var slot = new BuildBarSlot(3, 2, 5);
        var first = DspCore.BuildBar.BindQuickBarWithResult(slot, 1001);
        Equal(BuildBarBindStatus.Applied, first.Status, "first bind status");
        True(first.Applied, "first bind applied");

        var duplicate = DspCore.BuildBar.BindQuickBarWithResult(slot, 1001);
        Equal(BuildBarBindStatus.AlreadyBound, duplicate.Status, "duplicate bind status");
        True(duplicate.Applied, "duplicate bind applied");

        var occupied = DspCore.BuildBar.BindQuickBarWithResult(slot, 1002);
        Equal(BuildBarBindStatus.Occupied, occupied.Status, "occupied status");
        False(occupied.Applied, "occupied applied");
        Equal(1001, occupied.ExistingItemId, "occupied existing item");

        var replaced = DspCore.BuildBar.BindQuickBarWithResult(slot, 1002, BuildBarConflictPolicy.ReplaceExisting);
        Equal(BuildBarBindStatus.Replaced, replaced.Status, "replaced status");
        Equal(1002, DspCore.BuildBar.GetAll()[slot], "replaced item");

        True(DspCore.BuildBar.SetPlayerOverride(slot, 0), "empty override accepted");
        False(DspCore.BuildBar.GetEffectiveBindings().ContainsKey(slot), "empty override removes effective binding");

        True(DspCore.BuildBar.SetPlayerOverride(slot, 2001), "item override accepted");
        Equal(2001, DspCore.BuildBar.GetEffectiveBindings()[slot], "item override wins");
    }

    private static void StableProtoIdMapping()
    {
        ResetStableProtoIds();
        var preferred = StableProtoIdRuntime.ResolveForRegistration(
            ProtoKind.Item,
            "mod.alpha",
            ProtoStableId.Of("item-a", preferredId: 42),
            currentId: 0,
            registeredIds: Array.Empty<int>());
        Equal(42, preferred, "preferred id is accepted");

        var reused = StableProtoIdRuntime.ResolveForRegistration(
            ProtoKind.Item,
            "mod.alpha",
            ProtoStableId.Of("item-a", preferredId: 77),
            currentId: 0,
            registeredIds: Array.Empty<int>());
        Equal(42, reused, "existing mapping wins over new preferred id");

        var allocated = StableProtoIdRuntime.ResolveForRegistration(
            ProtoKind.Item,
            "mod.alpha",
            ProtoStableId.Of("item-b", preferredId: 42),
            currentId: 0,
            registeredIds: new[] { 42 });
        Equal(12000, allocated, "occupied preferred id allocates above 12000");

        ResetStableProtoIds();
        Directory.CreateDirectory(Path.Combine(BepInEx.Paths.ConfigPath, "DSPCore"));
        File.WriteAllText(
            Path.Combine(BepInEx.Paths.ConfigPath, "DSPCore", "StableProtoIds.tsv"),
            "# kind\townerModGuid\tstableKey\truntimeId\nItem\tmod.alpha\told-key\t13000\n",
            Encoding.UTF8);
        var migrated = StableProtoIdRuntime.ResolveForRegistration(
            ProtoKind.Item,
            "mod.alpha",
            ProtoStableId.Of("new-key", aliases: "old-key"),
            currentId: 0,
            registeredIds: Array.Empty<int>());
        Equal(13000, migrated, "alias migrates existing id");
        Contains(
            File.ReadAllText(Path.Combine(BepInEx.Paths.ConfigPath, "DSPCore", "StableProtoIds.tsv"), Encoding.UTF8),
            "Item\tmod.alpha\tnew-key\t13000",
            "alias writes current key mapping");
    }

    private static void OptionsImportAndExport()
    {
        Options.Register("Alpha", "Text", "default", "description");
        Options.Register("Alpha", "Number", "2", "description", kind: OptionValueKind.Int);

        Equal("default", Options.GetString("Alpha", "Text"), "unbound default value");
        Equal(2, Options.GetInt("Alpha", "Number"), "unbound int default");

        OptionRuntime.BindForTests();
        var report = Options.ImportValues(new[]
        {
            new OptionValueSnapshot("Alpha", "Text", "changed"),
            new OptionValueSnapshot("Missing", "Key", "value")
        });
        Equal(1, report.AppliedCount, "applied import count");
        Equal(1, report.SkippedCount, "skipped import count");
        Equal("changed", Options.GetString("Alpha", "Text"), "imported value");

        var text = Options.ExportText();
        Contains(text, Convert.ToBase64String(Encoding.UTF8.GetBytes("Alpha")), "export contains encoded section");

        var malformed = Options.ImportText("not-base64\tbad\tline\n");
        Equal(0, malformed.AppliedCount, "malformed applied count");
        Equal(1, malformed.SkippedCount, "malformed skipped count");
    }

    private static void SaveBlockAndAutoSave()
    {
        var readKnown = false;
        var unknownSkipped = true;
        byte[] data;
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                SaveBlockFormat.WriteBlocks(writer, new[]
                {
                    new SaveBlock("known", blockWriter => blockWriter.Write(42), _ => { }),
                    new SaveBlock("unknown", blockWriter => blockWriter.Write("ignored"), _ => { unknownSkipped = false; })
                });
            }

            data = stream.ToArray();
        }

        using (var stream = new MemoryStream(data))
        using (var reader = new BinaryReader(stream, Encoding.UTF8))
        {
            SaveBlockFormat.ReadBlocks(reader, new[]
            {
                new SaveBlock("known", _ => { }, blockReader =>
                {
                    Equal(42, blockReader.ReadInt32(), "known block value");
                    readKnown = true;
                })
            });
        }

        True(readKnown, "known block read");
        True(unknownSkipped, "unknown block skipped");

        var state = new TestState { Count = 7, Name = "before", Mode = TestMode.Second };
        var migratedFrom = -1;
        var handler = new AutoSaveHandler<TestState>("logic.test", state, version: 3, migrate: (version, target) =>
        {
            migratedFrom = version;
            target.Name += ":migrated";
        }, intoOtherSave: target => target.Name = "initialized");

        byte[] save;
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
            {
                handler.Export(writer);
            }

            save = stream.ToArray();
        }

        state.Count = 0;
        state.Name = "mutated";
        state.Mode = TestMode.First;
        using (var stream = new MemoryStream(save))
        using (var reader = new BinaryReader(stream, Encoding.UTF8))
        {
            handler.Import(reader);
        }

        Equal(7, state.Count, "auto save count");
        Equal("before:migrated", state.Name, "auto save name and migration");
        Equal(TestMode.Second, state.Mode, "auto save enum");
        Equal(3, migratedFrom, "auto save migration version");

        handler.IntoOtherSave();
        Equal("initialized", state.Name, "into other save callback");
    }

    private static void GlobalSaveOverview()
    {
        var savedState = new TestState { Count = 11, Name = "global", Mode = TestMode.Second };
        DspCore.GlobalSaves.Register(
            "mod.saved",
            new AutoSaveHandler<TestState>("mod.saved", savedState, version: 1, migrate: null, intoOtherSave: null),
            CoreLoadOrder.Postload);

        GlobalSaveRuntime.Initialize();
        GlobalSaveRuntime.Save();

        var overview = GlobalSaveRuntime.CreateOverview();
        True(overview.IsLoaded, "global save runtime loaded");
        Equal(1, overview.RegisteredCount, "registered global save count");
        Equal(1, overview.FileBlockCount, "file global save block count");
        Contains(overview.Path, "GlobalSaves.dspcore", "global save path");
        var savedBlock = overview.Blocks.Single(item => item.ModGuid == "mod.saved");
        True(savedBlock.IsRegistered, "saved block registered");
        True(savedBlock.IsLoadedFromFile, "saved block loaded");
        True(savedBlock.ByteCount > 0, "saved block has bytes");

        var path = Path.Combine(BepInEx.Paths.ConfigPath, "DSPCore", "GlobalSaves.dspcore");
        using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
        {
            writer.Write('D');
            writer.Write('S');
            writer.Write('P');
            writer.Write('G');
            writer.Write(1);
            writer.Write(1);
            writer.Write("mod.fileonly");
            writer.Write(3);
            writer.Write(new byte[] { 1, 2, 3 });
        }

        DspCore.GlobalSaves.Register(
            "mod.initialized",
            new AutoSaveHandler<TestState>("mod.initialized", new TestState(), version: 1, migrate: null, intoOtherSave: null),
            CoreLoadOrder.Postload);

        GlobalSaveRuntime.Initialize();
        overview = GlobalSaveRuntime.CreateOverview();
        Equal(2, overview.RegisteredCount, "registered plus initialized count");
        Equal(1, overview.FileBlockCount, "file only count");
        var fileOnlyBlock = overview.Blocks.Single(item => item.ModGuid == "mod.fileonly");
        False(fileOnlyBlock.IsRegistered, "file-only block is not registered");
        True(fileOnlyBlock.IsLoadedFromFile, "file-only block loaded");
        Equal(3, fileOnlyBlock.ByteCount, "file-only byte count");
        var initializedBlock = overview.Blocks.Single(item => item.ModGuid == "mod.initialized");
        True(initializedBlock.IsRegistered, "initialized block registered");
        False(initializedBlock.IsLoadedFromFile, "initialized block not loaded from file");
        Equal(0, initializedBlock.ByteCount, "initialized byte count");
    }

    private static void LocalizationFallbackAndOverrides()
    {
        DspCore.Resources.RegisterLocalization(new LocalizationEntry("hello", "zhCN", "你好", "mod"));
        DspCore.Resources.RegisterLocalization(new LocalizationEntry("hello", "enUS", "hello", "mod"));
        Localization.CurrentLanguageLCID = Localization.LCID_ZHCN;

        True(DspCore.Resources.TryTranslate("hello", out var value), "zh translation exists");
        Equal("你好", value, "zh translation");
        Equal("hello", DspCore.Resources.Translate("hello", "enUS"), "explicit en translation");
        Equal("fallback", DspCore.Resources.Translate("missing", fallback: "fallback"), "missing fallback");

        DspCore.Resources.RegisterLocalizationOverride(new LocalizationEntry("hello", "zhCN", "玩家覆盖", "override"));
        Equal("玩家覆盖", DspCore.Resources.Translate("hello"), "override wins");
        Equal(Localization.LCID_ZHCN.ToString(), LocalizationLanguages.Normalize("zhCN"), "language normalization");
    }

    private static void ResourceRootsResolveAndReadFiles()
    {
        var root = Path.Combine(BepInEx.Paths.ConfigPath, "ExampleMod", "assets");
        Directory.CreateDirectory(root);
        var filePath = Path.Combine(root, "icons", "machine.txt");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        File.WriteAllBytes(filePath, Encoding.UTF8.GetBytes("resource bytes"));

        DspCore.Resources.RegisterResource(new ResourceDescriptor(
            "example-assets",
            "mod.example",
            "assets",
            root,
            "bundles/icons"));

        True(DspCore.Resources.TryGetResource("example-assets", out var descriptor), "resource root is registered");
        Equal(
            Path.Combine(root, "icons", "machine.txt").Replace('\\', '/'),
            descriptor.ResolvePath("icons/machine.txt"),
            "descriptor resolves relative path");

        True(
            DspCore.Resources.TryResolvePath("mod.example", "assets", "icons/machine.txt", out var resolved),
            "owner keyword path resolves");
        Equal(Path.Combine(root, "icons", "machine.txt").Replace('\\', '/'), resolved, "registry resolved path");

        True(
            DspCore.Resources.TryReadBytes("mod.example", "icons/machine.txt", out var data, out var readPath),
            "resource bytes are read through owner root");
        Equal("resource bytes", Encoding.UTF8.GetString(data), "resource bytes content");
        Equal(Path.Combine(root, "icons", "machine.txt").Replace('\\', '/'), readPath, "resource bytes resolved path");

        True(
            DspCore.Resources.TryResolveExistingFile("mod.example", "icons/machine.txt", out var existingPath),
            "existing resource file resolves");
        Equal(Path.Combine(root, "icons", "machine.txt").Replace('\\', '/'), existingPath, "existing resource file path");

        True(DspCore.Resources.TryResolveBundlePath("example-assets", out var bundlePath), "bundle path resolves");
        Equal(Path.Combine(root, "bundles", "icons").Replace('\\', '/'), bundlePath, "bundle path");

        var absolute = Path.Combine(root, "absolute.txt").Replace('\\', '/');
        Equal(absolute, descriptor.ResolvePath(absolute), "absolute path stays absolute");
        False(
            DspCore.Resources.TryReadBytes("mod.example", "icons/missing.txt", out _, out _),
            "missing resource file returns false");
    }

    private static void IconRuntimeLoadingAndBinding()
    {
        ResetIconRuntime();
        var root = Path.Combine(BepInEx.Paths.ConfigPath, "IconMod", "assets");
        Directory.CreateDirectory(Path.Combine(root, "icons"));
        File.WriteAllBytes(Path.Combine(root, "icons", "machine.png"), Encoding.UTF8.GetBytes("png bytes"));
        File.WriteAllBytes(Path.Combine(root, "bundle.ab"), Encoding.UTF8.GetBytes("bundle bytes"));
        DspCore.Resources.RegisterResource(new ResourceDescriptor(
            "icon-assets",
            "mod.icon",
            "assets",
            root));

        LDB.items.dataArray = new[] { new ItemProto { ID = 1001, Name = "item-icon-target" } };
        LDB.recipes.dataArray = new[] { new RecipeProto { ID = 2001, Name = "recipe-icon-target" } };

        Icons.Register(new IconDescriptor(
            Id: "local",
            OwnerModGuid: "mod.icon",
            AssetPath: "icons/machine.png",
            TargetKind: ProtoKind.Item,
            TargetProtoId: 1001));
        Icons.Register(new IconDescriptor(
            Id: "fallback",
            OwnerModGuid: "mod.icon",
            AssetPath: "missing.png",
            FallbackIconId: "local",
            TargetKind: ProtoKind.Recipe,
            TargetProtoId: 2001));

        var embeddedPath = IconAssetPaths.Embedded(typeof(Program).Assembly, "DSPCore.LogicTests.Fixtures.embedded-icon.bytes");
        Icons.Register(new IconDescriptor("embedded", "mod.icon", embeddedPath));
        var bundlePath = IconAssetPaths.AssetBundle("bundle.ab", "sprite");
        Icons.Register(new IconDescriptor("bundle", "mod.icon", bundlePath));

        var localSprite = IconRuntime.ResolveSprite(Icons.GetAll().Single(item => item.Id == "local"));
        True(localSprite != null, "local icon loads through resource root");
        True(ReferenceEquals(localSprite, IconRuntime.ResolveSprite(Icons.GetAll().Single(item => item.Id == "local"))), "local icon is cached");
        True(ReferenceEquals(localSprite, IconRuntime.ResolveSprite(Icons.GetAll().Single(item => item.Id == "fallback"))), "fallback icon resolves to cached local sprite");
        True(IconRuntime.ResolveSprite(Icons.GetAll().Single(item => item.Id == "embedded")) != null, "embedded icon loads");
        True(IconRuntime.ResolveSprite(Icons.GetAll().Single(item => item.Id == "bundle")) != null, "asset bundle icon loads through resource root");

        IconRuntime.ApplyIcons();
        True(ReferenceEquals(localSprite, LDB.items.Select(1001)!.IconSprite), "item proto icon is bound");
        True(ReferenceEquals(localSprite, LDB.recipes.Select(2001)!.IconSprite), "recipe proto fallback icon is bound");
    }

    private static void GameEnumTypeDeclarations()
    {
        LDB.items.dataArray = new[]
        {
            new ItemProto { ID = 1001, Name = "item-a" },
            new ItemProto { ID = 1002, Name = "item-b" }
        };

        GameEnums.RegisterItemType(
            id: "example.custom-items",
            ownerModGuid: "mod.example",
            displayName: "Example Items",
            itemIds: new[] { 1001, 9999 });
        Equal(1, GameEnums.GetItemTypes().Count, "registered item type count");
        Equal(1, GameEnums.GetOrAssignItemTypeRuntimeId("example.custom-items"), "item type runtime id");
        Equal(1, GameEnums.GetOrAssignItemTypeRuntimeId("example.custom-items"), "item type runtime id is stable");

        ItemTypeRuntime.Apply();
        Equal(GameEnums.CustomItemTypeValue, (int)LDB.items.Select(1001)!.Type, "declared item gets custom type");
        Equal(0, (int)LDB.items.Select(1002)!.Type, "undeclared item keeps type");
        True(GameEnums.TryGetItemTypeForItem(1001, out var itemType), "item type query succeeds");
        Equal("example.custom-items", itemType.Id, "item type query id");
        False(GameEnums.TryGetItemTypeForItem(1002, out _), "undeclared item type query fails");

        var direct = new ItemProto { ID = 2001, Name = "direct" };
        True(direct.SetCustomItemType(), "direct item marker succeeds");
        True(direct.IsCustomItemType(), "direct item marker query");

        GameEnums.RegisterRecipeType(
            id: "example.custom-recipes",
            ownerModGuid: "mod.example",
            displayName: "Example Recipes",
            recipeIds: new[] { 3001, 3999 },
            assemblerItemIds: new[] { 1001 });
        Equal(1, GameEnums.GetRecipeTypes().Count, "registered recipe type count");
        Equal(1, GameEnums.GetOrAssignRecipeTypeRuntimeId("example.custom-recipes"), "recipe type runtime id");
        False(GameEnums.TryGetRecipeTypeForRecipe(3001, out _), "recipe type runtime query is empty before runtime apply");

        LDB.recipes.dataArray = new[]
        {
            new RecipeProto { ID = 3001, Name = "recipe-a" },
            new RecipeProto { ID = 3002, Name = "recipe-b" }
        };
        RecipeTypeRuntime.Apply();
        Equal(GameEnums.CustomRecipeTypeValue, (int)LDB.recipes.Select(3001)!.Type, "declared recipe gets custom type");
        Equal(0, (int)LDB.recipes.Select(3002)!.Type, "undeclared recipe keeps type");
        True(GameEnums.TryGetRecipeTypeForRecipe(3001, out var recipeType), "recipe type query succeeds after runtime apply");
        Equal("example.custom-recipes", recipeType.Id, "recipe type query id");
        False(GameEnums.TryGetRecipeTypeForRecipe(3999, out _), "missing recipe is skipped");

        var factory = CreateFactory(planetId: 7, entityId: 3, protoId: 1001, modelIndex: 0);
        GameMain.localPlanet = factory.planet;
        True(GameEnums.CanAssemblerUseRecipe(3, 3001), "allowed assembler item can use recipe");
        factory.entityPool[3].protoId = 1002;
        False(GameEnums.CanAssemblerUseRecipe(3, 3001), "wrong assembler item cannot use recipe");
        True(GameEnums.CanAssemblerUseRecipe(3, 3002), "undeclared recipe remains usable");
        GameMain.localPlanet = null;
        True(GameEnums.CanAssemblerUseRecipe(3, 3001), "missing local factory does not block recipe");

        var factorySystem = new FactorySystem
        {
            factory = factory,
            assemblerPool = new AssemblerComponent[2]
        };
        factory.entityPool[3].protoId = 1001;
        factorySystem.assemblerPool[1] = new AssemblerComponent { id = 1, entityId = 3 };
        GameMain.localPlanet = factory.planet;
        RecipeTypeRuntime.SetCurrentAssembler(new UIAssemblerWindow
        {
            assemblerId = 1,
            factory = factory,
            factorySystem = factorySystem
        });
        True(RecipeTypeRuntime.CanCurrentAssemblerUseRecipe(3001), "current assembler context allows recipe");
        factory.entityPool[3].protoId = 1002;
        False(RecipeTypeRuntime.CanCurrentAssemblerUseRecipe(3001), "current assembler context rejects recipe");
        RecipeTypeRuntime.ClearCurrentAssembler();
        True(RecipeTypeRuntime.CanCurrentAssemblerUseRecipe(3001), "cleared assembler context does not block recipe");
    }

    private static void ProtoCacheRebuildContinuesAfterFailedStep()
    {
        ItemProto.ThrowOnInitFighterIndices = true;
        LDB.models.dataArray = new[]
        {
            new ModelProto
            {
                ID = 1,
                Name = "model",
                prefabDesc = new PrefabDesc { modelIndex = 1 }
            }
        };
        LDB.recipes.dataArray = new[]
        {
            new RecipeProto
            {
                ID = 1,
                Name = "recipe",
                Items = new[] { 1001 },
                ItemCounts = new[] { 1 },
                Results = new[] { 1002 },
                ResultCounts = new[] { 1 },
                TimeSpend = 10
            }
        };
        GameMain.iconSet = new GameIconSet();

        ProtoRegistrationRuntime.RebuildDerivedCaches();

        Equal(1, DspCore.Errors.Reports.Count, "failed cache step reported once");
        Equal(1, ItemProto.InitPowerFacilityIndicesCalls, "later item cache step still ran");
        Equal(1, ItemProto.InitProductionMaskCalls, "production mask still ran");
        Equal(1, ModelProto.InitMaxModelIndexCalls, "model max index still ran");
        Equal(1, ModelProto.InitModelIndicesCalls, "model indices still ran");
        Equal(1, ModelProto.InitModelOrdersCalls, "model orders still ran");
        Equal(1, PlanetFactory.InitPrefabDescArrayCalls, "prefab desc rebuild still ran");
        Equal(1, RecipeProto.InitRecipeItemsCalls, "recipe items still ran");
        Equal(1, RecipeProto.InitFractionatorNeedsCalls, "fractionator needs still ran");
        Equal(1, RecipeProto.recipeExecuteData.Count, "recipe execute data still rebuilt");
        Equal(1, SignalProtoSet.InitSignalKeyIdPairsCalls, "signal key-id pairs still ran");
        Equal(1, GameMain.iconSet.CreateCalls, "game icon set still rebuilt");
        False(GameMain.iconSet.loaded, "game icon set was marked dirty before rebuild");
    }

    private static void ProtoCacheRebuildFiltersItemsWithoutPrefabDesc()
    {
        var missingPrefab = new ItemProto { ID = 1001, Name = "no-prefab" };
        var withPrefab = new ItemProto { ID = 1002, Name = "with-prefab", prefabDesc = new PrefabDesc() };
        LDB.items.dataArray = new[] { missingPrefab, withPrefab };
        LDB.recipes.dataArray = new[]
        {
            new RecipeProto
            {
                ID = 2001,
                Name = "uses-no-prefab",
                Type = ERecipeType.Assemble,
                Items = new[] { missingPrefab.ID },
                ItemCounts = new[] { 1 },
                Results = new[] { withPrefab.ID },
                ResultCounts = new[] { 1 },
                TimeSpend = 60
            }
        };
        ItemProto.ThrowOnNullPrefabDescInCacheSteps = true;

        ProtoRegistrationRuntime.RebuildDerivedCaches();

        Equal(0, DspCore.Errors.Reports.Count, "null prefab desc items are patched before prefab-dependent cache steps");
        Equal(2, LDB.items.dataArray.Length, "original item array is restored after cache step");
        True(ReferenceEquals(missingPrefab, LDB.items.dataArray[0]), "missing-prefab item is restored");
        True(ReferenceEquals(withPrefab, LDB.items.dataArray[1]), "with-prefab item is restored");
        True(missingPrefab.prefabDesc == null, "missing prefab desc is restored after cache step");
        Equal(1, missingPrefab.consumptionMask, "production mask still sees no-prefab item through recipe input");
        Equal(1, withPrefab.productionMask, "production mask still sees recipe output");
        Equal(1, ItemProto.InitPowerFacilityIndicesCalls, "power facility step still ran");
        Equal(1, ItemProto.InitProductionMaskCalls, "production mask step still ran");
    }

    private static void PickerLayoutFallback()
    {
        var entries = new[]
        {
            new PickerLayoutEntry<string>(1101, 1, "a"),
            new PickerLayoutEntry<string>(1101, 2, "b"),
            new PickerLayoutEntry<string>(1305, 3, "c"),
            new PickerLayoutEntry<string>(0, 4, "invalid")
        };

        var layout = PickerLayoutPlanner.Plan(entries, defaultColumns: 2, defaultRows: 1);
        Equal(5, layout.Metrics.Columns, "layout expands columns");
        Equal(3, layout.Metrics.Rows, "layout expands rows");
        Equal(3, layout.Entries.Count, "invalid grid skipped");
        Equal(0, layout.Entries[0].Slot, "first preferred slot");
        Equal(1, layout.Entries[1].Slot, "duplicate fallback slot");
        Equal(14, layout.Entries[2].Slot, "far grid slot");
        True(PickerLayoutPlanner.IsOnPage(1305, 1), "grid page check");
        True(PickerLayoutPlanner.TryGetCell(1305, out var row, out var column), "grid cell parsed");
        Equal(2, row, "grid row");
        Equal(4, column, "grid column");
    }

    private static void BlueprintTaggedParameterBlocks()
    {
        var factory = new PlanetFactory();
        var copiedObjectIds = new List<int>();
        var pastedPayloads = new List<int[]>();
        var prebuildPayloads = new List<int[]>();
        var canPasteCalls = 0;

        Blueprints.Register(
            blockId: "logic.block",
            ownerModGuid: "mod.logic",
            copy: (_, objectId) =>
            {
                copiedObjectIds.Add(objectId);
                return new[] { objectId, 7, 9 };
            },
            paste: (_, entityId, data) => pastedPayloads.Add(new[] { entityId }.Concat(data).ToArray()),
            canPaste: (_, entityId, data) =>
            {
                canPasteCalls++;
                return entityId == 77 && data.SequenceEqual(new[] { 42, 7, 9 });
            },
            applyPrebuild: (_, entityId, data) => prebuildPayloads.Add(new[] { entityId }.Concat(data).ToArray()));

        var parameters = new BuildingParameters { parameters = new[] { 1, 2, 3 } };
        BuildingParameterRuntime.AddCopiedBlocks(ref parameters, objectId: 42, factory);
        Equal(1, copiedObjectIds.Count, "copy callback count");
        Equal(42, copiedObjectIds[0], "copy object id");
        True(parameters.parameters.Length > 3, "encoded block appended");
        Equal(1, parameters.parameters[0], "vanilla parameter 0 preserved");
        Equal(2, parameters.parameters[1], "vanilla parameter 1 preserved");
        Equal(3, parameters.parameters[2], "vanilla parameter 2 preserved");

        var imported = new BuildingParameters { parameters = new[] { 99 } };
        BuildingParameterRuntime.PreserveBlocksFromParamsArray(ref imported, parameters.parameters);
        Equal(99, imported.parameters[0], "preserve keeps new vanilla params");
        True(imported.parameters.Length > 1, "preserve carries DSPCore block");

        var raw = new[] { 5, 6 };
        var count = raw.Length;
        BuildingParameterRuntime.AppendBlocksToParamsArray(imported, ref raw, ref count);
        Equal(raw.Length, count, "param count tracks appended raw length");
        Equal(5, raw[0], "to params keeps raw parameter 0");
        Equal(6, raw[1], "to params keeps raw parameter 1");

        False(BuildingParameterRuntime.CanPaste(imported, objectId: 12, factory), "can paste rejects entity with descriptor decision");
        Equal(1, canPasteCalls, "can paste reject call count");
        True(BuildingParameterRuntime.CanPaste(imported, objectId: 77, factory), "can paste accepts matching entity");
        Equal(2, canPasteCalls, "can paste accept call count");

        BuildingParameterRuntime.Paste(imported, objectId: 77, factory);
        Equal(1, pastedPayloads.Count, "paste callback count");
        SequenceEqual(new[] { 77, 42, 7, 9 }, pastedPayloads[0], "paste payload");

        BuildingParameterRuntime.ApplyPrebuild(entityId: 88, imported.parameters, factory);
        Equal(1, prebuildPayloads.Count, "prebuild callback count");
        SequenceEqual(new[] { 88, 42, 7, 9 }, prebuildPayloads[0], "prebuild payload");

        True(BuildingParameterRuntime.CanPaste(new BuildingParameters { parameters = Array.Empty<int>() }, 77, factory), "empty params do not call descriptors and remain pasteable");
    }

    private static void ComponentAndPlanetLifecycleCallbacks()
    {
        var factory = CreateFactory(planetId: 5, entityId: 2, protoId: 1001, modelIndex: 2001);
        var desc = new PrefabDesc { modelIndex = 2001 };
        var createdComponents = new List<TestComponent>();

        Components.Register(new ComponentDescriptor(
            componentId: "logic.component",
            ownerModGuid: "mod.logic",
            factory: (planetFactory, entityId, prefabDesc, prebuildId) =>
            {
                var component = new TestComponent();
                component.Events.Add("factory:" + (planetFactory == factory));
                component.Events.Add("prebuild:" + prebuildId);
                createdComponents.Add(component);
                return component;
            },
            itemId: 1001,
            modelIndex: 2001,
            predicate: prefabDesc => prefabDesc.modelIndex == 2001));

        EntityLifecycleRuntime.AttachComponents(factory, entityId: 2, desc, prebuildId: 9);
        Equal(1, createdComponents.Count, "component created once");
        var component = createdComponents[0];
        Equal("logic.component", component.ComponentId, "component context id");
        Equal(2, component.EntityId, "component context entity id");
        True(ReferenceEquals(factory, component.Factory), "component context factory");
        Contains(string.Join(",", component.Events), "added:9", "component added callback");
        True(Components.TryGet<TestComponent>(factory, 2, out var fetched), "component can be fetched");
        True(ReferenceEquals(component, fetched), "fetched component matches");

        EntityLifecycleRuntime.PowerUpdate(factory, time: 10, isActive: true);
        EntityLifecycleRuntime.Update(factory, time: 11, isActive: false);
        EntityLifecycleRuntime.PostUpdate(factory, time: 12);
        Contains(string.Join(",", component.Events), "power:10:True", "component power update");
        Contains(string.Join(",", component.Events), "update:11:False", "component update");
        Contains(string.Join(",", component.Events), "post:12", "component post update");

        EntityLifecycleRuntime.RemoveComponents(factory, entityId: 2);
        Contains(string.Join(",", component.Events), "removed", "component removed callback");
        False(Components.TryGet<TestComponent>(factory, 2, out _), "component removed from lookup");

        var factoryWithoutMatch = CreateFactory(planetId: 6, entityId: 2, protoId: 9999, modelIndex: 2001);
        EntityLifecycleRuntime.AttachComponents(factoryWithoutMatch, entityId: 2, desc, prebuildId: 1);
        Equal(1, createdComponents.Count, "non-matching item does not create component");

        var createdPlanetSystems = new List<TestPlanetSystem>();
        PlanetSystems.Register(new PlanetSystemDescriptor(
            systemId: "logic.planet",
            ownerModGuid: "mod.logic",
            factory: planetFactory =>
            {
                var system = new TestPlanetSystem();
                system.Events.Add("factory:" + (planetFactory == factory));
                createdPlanetSystems.Add(system);
                return system;
            }));

        PlanetLifecycleRuntime.EnsureFactory(factory);
        Equal(1, createdPlanetSystems.Count, "planet system created once");
        var planetSystem = createdPlanetSystems[0];
        Equal("logic.planet", planetSystem.SystemId, "planet system context id");
        True(ReferenceEquals(factory, planetSystem.Factory), "planet system context factory");
        Contains(string.Join(",", planetSystem.Events), "into-other-save", "planet system into other save callback");
        Contains(string.Join(",", planetSystem.Events), "init", "planet system init callback");
        True(PlanetSystems.TryGet<TestPlanetSystem>(factory, "logic.planet", out var fetchedSystem), "planet system can be fetched");
        True(ReferenceEquals(planetSystem, fetchedSystem), "fetched planet system matches");

        PlanetLifecycleRuntime.DrawUpdate(factory);
        PlanetLifecycleRuntime.PowerUpdate(factory, time: 20, isActive: true);
        PlanetLifecycleRuntime.PreUpdate(factory, time: 21, isActive: true);
        PlanetLifecycleRuntime.Update(factory, time: 22, isActive: false);
        PlanetLifecycleRuntime.PostUpdate(factory, time: 23);
        Contains(string.Join(",", planetSystem.Events), "draw", "planet draw update");
        Contains(string.Join(",", planetSystem.Events), "power:20:True", "planet power update");
        Contains(string.Join(",", planetSystem.Events), "pre:21:True", "planet pre update");
        Contains(string.Join(",", planetSystem.Events), "update:22:False", "planet update");
        Contains(string.Join(",", planetSystem.Events), "post:23", "planet post update");

        PlanetLifecycleRuntime.EnsureFactory(factory);
        Equal(1, createdPlanetSystems.Count, "ensure factory is idempotent for planet systems");
    }

    private static void GalaxyAndStarLifecycleCallbacks()
    {
        GalaxyLifecycleRuntime.Initialize();
        var galaxy = new GalaxyData
        {
            stars = new[]
            {
                new StarData { id = 1 },
                new StarData { id = 2 }
            }
        };
        var createdGalaxySystems = new List<TestGalaxySystem>();
        var createdStarSystems = new List<TestStarSystem>();

        GalaxySystems.RegisterGalaxy(new GalaxySystemDescriptor(
            SystemId: "logic.galaxy",
            OwnerModGuid: "mod.logic",
            Factory: input =>
            {
                var system = new TestGalaxySystem();
                system.Events.Add("factory:" + ReferenceEquals(input, galaxy));
                createdGalaxySystems.Add(system);
                return system;
            }));
        GalaxySystems.RegisterStar(new StarSystemDescriptor(
            SystemId: "logic.star",
            OwnerModGuid: "mod.logic",
            Factory: star =>
            {
                var system = new TestStarSystem();
                system.Events.Add("factory:" + star.id);
                createdStarSystems.Add(system);
                return system;
            }));

        GalaxyLifecycleRuntime.EnsureGalaxy(galaxy);
        Equal(1, createdGalaxySystems.Count, "galaxy system created once");
        Equal(2, createdStarSystems.Count, "star system created for every star");
        var galaxySystem = createdGalaxySystems[0];
        Equal("logic.galaxy", galaxySystem.SystemId, "galaxy system context id");
        True(ReferenceEquals(galaxy, galaxySystem.Galaxy), "galaxy system context galaxy");
        Contains(string.Join(",", galaxySystem.Events), "into-other-save", "galaxy system into other save callback");
        Contains(string.Join(",", galaxySystem.Events), "init", "galaxy system init callback");
        Equal("logic.star", createdStarSystems[0].SystemId, "star system context id");
        Equal(1, createdStarSystems[0].Star!.id, "first star context");
        Equal(2, createdStarSystems[1].Star!.id, "second star context");
        Contains(string.Join(",", createdStarSystems[0].Events), "into-other-save", "star system into other save callback");
        Contains(string.Join(",", createdStarSystems[0].Events), "init", "star system init callback");

        GalaxyLifecycleRuntime.Update(galaxy, time: 55);
        Contains(string.Join(",", galaxySystem.Events), "update:55", "galaxy system update");
        Contains(string.Join(",", createdStarSystems[0].Events), "update:55", "first star system update");
        Contains(string.Join(",", createdStarSystems[1].Events), "update:55", "second star system update");

        GalaxyLifecycleRuntime.EnsureGalaxy(galaxy);
        Equal(1, createdGalaxySystems.Count, "ensure galaxy is idempotent for galaxy systems");
        Equal(2, createdStarSystems.Count, "ensure galaxy is idempotent for star systems");

        galaxySystem.ExportValue = 101;
        createdStarSystems[0].ExportValue = 201;
        createdStarSystems[1].ExportValue = 202;
        var saveData = ExportGalaxyLifecycleSave();

        createdGalaxySystems.Clear();
        createdStarSystems.Clear();
        var restoredGalaxy = new GalaxyData
        {
            stars = new[]
            {
                new StarData { id = 1 },
                new StarData { id = 2 }
            }
        };
        ImportGalaxyLifecycleSave(saveData);
        GalaxyLifecycleRuntime.EnsureGalaxy(restoredGalaxy);
        Equal(1, createdGalaxySystems.Count, "import creates restored galaxy system");
        Equal(2, createdStarSystems.Count, "import creates restored star systems");
        Contains(string.Join(",", createdGalaxySystems[0].Events), "import:101", "galaxy system imports saved state");
        Contains(string.Join(",", createdStarSystems[0].Events), "import:201", "first star system imports saved state");
        Contains(string.Join(",", createdStarSystems[1].Events), "import:202", "second star system imports saved state");
        False(string.Join(",", createdGalaxySystems[0].Events).Contains("into-other-save", StringComparison.Ordinal), "imported galaxy system does not use into other save");
        False(string.Join(",", createdStarSystems[0].Events).Contains("into-other-save", StringComparison.Ordinal), "imported star system does not use into other save");
    }

    private static void ModelCloneProjection()
    {
        ResetModelRuntime();
        var source = new ModelProto
        {
            ID = 100,
            Name = "source-model",
            name = "source-model-name",
            sid = "source-model-sid",
            customValue = 7,
            prefabDesc = new PrefabDesc
            {
                modelIndex = 100,
                customValue = 9
            }
        };
        var existingTarget = new ModelProto
        {
            ID = 200,
            Name = "old-target",
            prefabDesc = new PrefabDesc
            {
                modelIndex = 200,
                customValue = 1
            }
        };
        LDB.models.dataArray = new[] { source, existingTarget };

        Models.Register(new ModelDescriptor(
            ownerModGuid: "mod.logic",
            sourceModelIndex: 100,
            modelIndex: 200,
            configureModel: model =>
            {
                model.Name = "configured-model";
                model.customValue = 70;
            },
            configurePrefab: prefab =>
            {
                prefab.customValue = 90;
            }));

        ModelRuntime.Apply();
        Equal(2, LDB.models.dataArray.Length, "existing target model is replaced");
        True(ReferenceEquals(source, LDB.models.Select(100)), "source model remains original");
        var clone = LDB.models.Select(200)!;
        False(ReferenceEquals(existingTarget, clone), "target model instance is replaced");
        False(ReferenceEquals(source, clone), "clone is a new model instance");
        Equal(200, clone.ID, "clone model id");
        Equal("configured-model", clone.Name, "configure model can change name");
        Equal("source-model", clone.name, "clone public field copied before configure");
        Equal("source-model", clone.sid, "clone sid follows source SID before configure");
        Equal(70, clone.customValue, "configure model applied");
        True(clone.prefabDesc != null, "clone prefab exists");
        False(ReferenceEquals(source.prefabDesc, clone.prefabDesc), "prefab desc is cloned");
        Equal(200, clone.prefabDesc!.modelIndex, "clone prefab model index follows target");
        Equal(90, clone.prefabDesc.customValue, "configure prefab applied");

        ModelRuntime.Apply();
        Equal(2, LDB.models.dataArray.Length, "model apply is idempotent");

        PlanetFactory.PrefabDescByModelIndex = new[] { new PrefabDesc { modelIndex = 0 } };
        PlanetFactory.InitPrefabDescArrayCalls = 0;
        ModelRuntime.RebuildPrefabDescArray();
        Equal(1, PlanetFactory.InitPrefabDescArrayCalls, "prefab desc array rebuilt once");
        True(PlanetFactory.PrefabDescByModelIndex != null, "prefab desc array exists");
        True(ReferenceEquals(clone.prefabDesc, PlanetFactory.PrefabDescByModelIndex![200]), "prefab desc array contains cloned prefab");
    }

    private static void Equal<T>(T expected, T actual, string message)
    {
        if (!EqualityComparer<T>.Default.Equals(expected, actual))
        {
            throw new InvalidOperationException($"{message}: expected {expected}, got {actual}");
        }
    }

    private static void True(bool value, string message)
    {
        if (!value)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void False(bool value, string message)
    {
        if (value)
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void Contains(string text, string expected, string message)
    {
        if (!text.Contains(expected, StringComparison.Ordinal))
        {
            throw new InvalidOperationException(message);
        }
    }

    private static void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message)
    {
        if (!expected.SequenceEqual(actual))
        {
            throw new InvalidOperationException($"{message}: expected [{string.Join(", ", expected)}], got [{string.Join(", ", actual)}]");
        }
    }

    private static void ResetStableProtoIds()
    {
        var type = typeof(StableProtoIdRuntime);
        var mappings = (System.Collections.IDictionary)type.GetField("Mappings", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        mappings.Clear();
        type.GetField("loaded", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, false);
        type.GetField("dirty", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, false);
    }

    private static void ResetModelRuntime()
    {
        typeof(ModelRuntime).GetField("applied", BindingFlags.Static | BindingFlags.NonPublic)!.SetValue(null, false);
    }

    private static void ResetIconRuntime()
    {
        ((System.Collections.IDictionary)typeof(IconRuntime).GetField("SpriteCache", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!).Clear();
        ((System.Collections.IDictionary)typeof(IconRuntime).GetField("AssetBundleCache", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!).Clear();
    }

    private sealed class TestState
    {
        [CoreSaveField("count", 2)]
        public int Count { get; set; }

        [CoreSaveField("name", 1)]
        public string Name { get; set; } = string.Empty;

        [CoreSaveField("mode", 3)]
        public TestMode Mode { get; set; }
    }

    private enum TestMode
    {
        First = 1,
        Second = 2
    }

    private static PlanetFactory CreateFactory(int planetId, int entityId, int protoId, int modelIndex)
    {
        var factory = new PlanetFactory
        {
            planet = new PlanetData { id = planetId },
            entityPool = new EntityData[Math.Max(entityId + 1, 4)]
        };
        factory.planet.factory = factory;
        factory.entityPool[entityId] = new EntityData
        {
            id = entityId,
            protoId = protoId,
            modelIndex = modelIndex
        };
        return factory;
    }

    private static byte[] ExportGalaxyLifecycleSave()
    {
        var handler = CreateGalaxyLifecycleSaveHandler();
        using var stream = new MemoryStream();
        using (var writer = new BinaryWriter(stream, Encoding.UTF8, true))
        {
            handler.Export(writer);
        }

        return stream.ToArray();
    }

    private static void ImportGalaxyLifecycleSave(byte[] data)
    {
        var handler = CreateGalaxyLifecycleSaveHandler();
        using var stream = new MemoryStream(data);
        using var reader = new BinaryReader(stream, Encoding.UTF8);
        handler.Import(reader);
    }

    private static ICoreSaveHandler CreateGalaxyLifecycleSaveHandler()
    {
        var type = typeof(GalaxyLifecycleRuntime).GetNestedType("SaveHandler", BindingFlags.NonPublic)!;
        return (ICoreSaveHandler)Activator.CreateInstance(type)!;
    }

    private sealed class TestComponent : CoreFactoryComponent
    {
        public List<string> Events { get; } = new();

        public override void OnAdded(int prebuildId)
        {
            Events.Add("added:" + prebuildId);
        }

        public override void OnRemoved()
        {
            Events.Add("removed");
        }

        public override void PowerUpdate(long time, bool isActive)
        {
            Events.Add("power:" + time + ":" + isActive);
        }

        public override void Update(long time, bool isActive)
        {
            Events.Add("update:" + time + ":" + isActive);
        }

        public override void PostUpdate(long time)
        {
            Events.Add("post:" + time);
        }
    }

    private sealed class TestPlanetSystem : CorePlanetSystem
    {
        public List<string> Events { get; } = new();

        public override void Init()
        {
            Events.Add("init");
        }

        public override void IntoOtherSave()
        {
            Events.Add("into-other-save");
        }

        public override void DrawUpdate()
        {
            Events.Add("draw");
        }

        public override void PowerUpdate(long time, bool isActive)
        {
            Events.Add("power:" + time + ":" + isActive);
        }

        public override void PreUpdate(long time, bool isActive)
        {
            Events.Add("pre:" + time + ":" + isActive);
        }

        public override void Update(long time, bool isActive)
        {
            Events.Add("update:" + time + ":" + isActive);
        }

        public override void PostUpdate(long time)
        {
            Events.Add("post:" + time);
        }
    }

    private sealed class TestGalaxySystem : CoreGalaxySystem
    {
        public List<string> Events { get; } = new();

        public int ExportValue { get; set; }

        public override void Init()
        {
            Events.Add("init");
        }

        public override void IntoOtherSave()
        {
            Events.Add("into-other-save");
        }

        public override void Update(long time)
        {
            Events.Add("update:" + time);
        }

        public override void Export(BinaryWriter writer)
        {
            writer.Write(ExportValue);
        }

        public override void Import(BinaryReader reader)
        {
            Events.Add("import:" + reader.ReadInt32());
        }
    }

    private sealed class TestStarSystem : CoreStarSystem
    {
        public List<string> Events { get; } = new();

        public int ExportValue { get; set; }

        public override void Init()
        {
            Events.Add("init");
        }

        public override void IntoOtherSave()
        {
            Events.Add("into-other-save");
        }

        public override void Update(long time)
        {
            Events.Add("update:" + time);
        }

        public override void Export(BinaryWriter writer)
        {
            writer.Write(ExportValue);
        }

        public override void Import(BinaryReader reader)
        {
            Events.Add("import:" + reader.ReadInt32());
        }
    }
}
