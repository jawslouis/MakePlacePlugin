// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.MakePlacePlugin
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Threading;
using Dalamud.Game.Command;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Excel.GeneratedSheets;
using MakePlacePlugin.Objects;
using MakePlacePlugin.Util;

namespace MakePlacePlugin; 

public class MakePlacePlugin : IDalamudPlugin, IDisposable {
    public delegate void SelectItemDelegate(IntPtr housingStruct, IntPtr item);

    public delegate void UpdateYardDelegate(IntPtr housingStruct, ushort index);

    public static List<HousingItem> ItemsToPlace = new();
    private static HookWrapper<SelectItemDelegate> SelectItemHook;
    public static bool CurrentlyPlacingItems;
    public static bool ApplyChange;
    public static SaveLayoutManager LayoutManager;
    public static bool logHousingDetour;
    internal static Location PlotLocation;
    internal static HookWrapper<GetIndexDelegate> GetYardIndexHook;
    internal static HookWrapper<GetObjectDelegate> GetGameObjectHook;
    internal static HookWrapper<GetActiveObjectDelegate> GetObjectFromIndexHook;
    private static HookWrapper<UpdateYardDelegate> UpdateYardObjHook;
    public List<HousingItem> ExteriorItemList = new();
    public List<HousingItem> InteriorItemList = new();
    private HookWrapper<UpdateLayoutDelegate> IsSaveLayoutHook;
    public Layout Layout = new();
    public List<HousingItem> UnusedItemList = new();

    public MakePlacePlugin(DalamudPluginInterface pi) {
        DalamudApi.Initialize(pi);
        if (!(DalamudApi.PluginInterface.GetPluginConfig() is Configuration configuration))
            configuration = new Configuration();
        this.Config = configuration;
        this.Config.Save();
        this.Initialize();
        // ISSUE: method pointer
        DalamudApi.CommandManager.AddHandler("/makeplace", new CommandInfo(this.CommandHandler) {
            HelpMessage = "load config window."
        });
        this.Gui = new PluginUi(this);
        DalamudApi.ClientState.TerritoryChanged += this.TerritoryChanged;
        HousingData.Init(this);
        Memory.Init();
        LayoutManager = new SaveLayoutManager(this, this.Config);
        DalamudApi.PluginLog.Info("MakePlace Plugin v3.2.0 initialized", Array.Empty<object>());
    }

    public string Name => "MakePlace Plugin";

    public PluginUi Gui { get; }

    public Configuration Config { get; }

    public void Dispose() {
        HookManager.Dispose();
        this.Config.PlaceAnywhere = false;
        DalamudApi.ClientState.TerritoryChanged -= this.TerritoryChanged;
        DalamudApi.CommandManager.RemoveHandler("/makeplace");
        this.Gui?.Dispose();
    }

    public void Initialize() {
        this.IsSaveLayoutHook = HookManager.Hook<UpdateLayoutDelegate>(
            "40 53 48 83 ec 20 48 8b d9 48 8b 0d ?? ?? ?? ?? e8 ?? ?? ?? ?? 33 d2 48 8b c8 e8 ?? ?? ?? ?? 84 c0 75 7d 38 83 ?? 01 00 00",
            this.IsSaveLayoutDetour);
        SelectItemHook =
            HookManager.Hook<SelectItemDelegate>("E8 ?? ?? ?? ?? 48 8B CE E8 ?? ?? ?? ?? 48 8B 6C 24 40 48 8B CE",
                SelectItemDetour);
        UpdateYardObjHook = HookManager.Hook<UpdateYardDelegate>(
            "48 89 74 24 18 57 48 83 ec 20 b8 dc 02 00 00 0f b7 f2 48 8b f9 66 3b d0 0f", this.UpdateYardObj);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        GetGameObjectHook = HookManager.Hook<GetObjectDelegate>(
            "48 89 5c 24 08 48 89 74 24 10 57 48 83 ec 20 0f b7 f2 33 db 0f 1f 40 00 0f 1f 84 00 00 00 00 00",
            GetGameObject);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        GetObjectFromIndexHook = HookManager.Hook<GetActiveObjectDelegate>(
            "81 fa 90 01 00 00 75 08 48 8b 81 88 0c 00 00 c3 0f b7 81 90 0c 00 00 3b d0 72 03 33 c0 c3",
            GetObjectFromIndex);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        GetYardIndexHook =
            HookManager.Hook<GetIndexDelegate>(
                "48 89 6c 24 18 56 48 83 ec 20 0f b6 ea 0f b6 f1 84 c9 79 22 0f b6 c1", GetYardIndex);
    }

    internal static ushort GetYardIndex(byte plotNumber, byte inventoryIndex) {
        return GetYardIndexHook.Original(plotNumber, inventoryIndex);
    }

    internal static IntPtr GetObjectFromIndex(IntPtr ObjList, uint index) {
        return GetObjectFromIndexHook.Original(ObjList, index);
    }

    internal static IntPtr GetGameObject(IntPtr ObjList, ushort index) {
        return GetGameObjectHook.Original(ObjList, index);
    }

    private void UpdateYardObj(IntPtr objectList, ushort index) {
        UpdateYardObjHook.Original(objectList, index);
    }

    public static void SelectItemDetour(IntPtr housing, IntPtr item) {
        SelectItemHook.Original(housing, item);
    }

    public static unsafe void SelectItem(IntPtr item) {
        SelectItemDetour((IntPtr)Memory.Instance.HousingStructure, item);
    }

    public void PlaceItems() {
        if (!Memory.Instance.CanEditItem())
            return;
        if (ItemsToPlace.Count == 0)
            return;
        try {
            if (Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Outdoors)
                this.GetPlotLocation();
            while (ItemsToPlace.Count > 0) {
                var rowItem = ItemsToPlace.First();
                ItemsToPlace.RemoveAt(0);
                if (rowItem.ItemStruct != IntPtr.Zero) {
                    if (rowItem.CorrectLocation && rowItem.CorrectRotation) {
                        Log(rowItem.Name + " is already correctly placed");
                    } else {
                        SetItemPosition(rowItem);
                        if (this.Config.LoadInterval > 0)
                            Thread.Sleep(this.Config.LoadInterval);
                    }
                }
            }

            if (ItemsToPlace.Count == 0)
                Log("Finished applying layout");
        } catch (Exception ex) {
            LogError("Error: " + ex.Message, ex.StackTrace);
        }

        CurrentlyPlacingItems = false;
    }

    public static void SetItemPosition(HousingItem rowItem) {
        if (!Memory.Instance.CanEditItem()) {
            LogError("Unable to set position outside of Rotate Layout mode");
        } else {
            if (rowItem.ItemStruct == IntPtr.Zero)
                return;
            Log("Placing " + rowItem.Name);
            var instance = Memory.Instance;
            logHousingDetour = true;
            ApplyChange = true;
            SelectItem(rowItem.ItemStruct);
            var newPosition = new Vector3(rowItem.X, rowItem.Y, rowItem.Z);
            var newRotation = new Vector3();
            newRotation.Y = (float)(rowItem.Rotate * 180.0 / Math.PI);
            if (instance.GetCurrentTerritory() == Memory.HousingArea.Outdoors) {
                var fromAxisAngle =
                    Quaternion.CreateFromAxisAngle(Vector3.UnitY, -PlotLocation.rotation);
                newPosition = Vector3.Transform(newPosition, fromAxisAngle) +
                              PlotLocation.ToVector();
                newRotation.Y = (float)((rowItem.Rotate - (double)PlotLocation.rotation) *
                    180.0 / Math.PI);
            }

            instance.WritePosition(newPosition);
            instance.WriteRotation(newRotation);
            rowItem.CorrectLocation = true;
            rowItem.CorrectRotation = true;
        }
    }

    public void ApplyLayout() {
        if (CurrentlyPlacingItems) {
            Log("Already placing items");
        } else {
            CurrentlyPlacingItems = true;
            var
                interpolatedStringHandler = new DefaultInterpolatedStringHandler(35, 1);
            interpolatedStringHandler.AppendLiteral("Applying layout with interval of ");
            interpolatedStringHandler.AppendFormatted(this.Config.LoadInterval);
            interpolatedStringHandler.AppendLiteral("ms");
            Log(interpolatedStringHandler.ToStringAndClear());
            ItemsToPlace.Clear();
            var collection = new List<HousingItem>();
            List<HousingItem> housingItemList;
            if (Memory.Instance.GetCurrentTerritory() == Memory.HousingArea.Indoors) {
                housingItemList = new List<HousingItem>();
                foreach (var interiorItem in this.InteriorItemList)
                    if (this.IsSelectedFloor(interiorItem.Y))
                        housingItemList.Add(interiorItem);
            } else {
                housingItemList = new List<HousingItem>(this.ExteriorItemList);
            }

            foreach (var housingItem in housingItemList)
                if (housingItem.IsTableOrWallMounted)
                    collection.Add(housingItem);
                else
                    ItemsToPlace.Add(housingItem);

            ItemsToPlace.AddRange(collection);
            new Thread(this.PlaceItems).Start();
        }
    }

    public bool MatchItem(HousingItem item, uint itemKey) {
        return item.ItemStruct == IntPtr.Zero &&
               (int)item.ItemKey == (int)itemKey &&
               this.IsSelectedFloor(item.Y);
    }

    public unsafe bool MatchExactItem(HousingItem item, uint itemKey, HousingGameObject obj) {
        if (!this.MatchItem(item, itemKey) || item.Stain != obj.color)
            return false;
        var materialSlot1 = obj.Item->MaterialManager->MaterialSlot1;
        if (item.MaterialItemKey == 0U && materialSlot1 == 0)
            return true;
        if (item.MaterialItemKey != 0U && materialSlot1 == 0)
            return false;
        uint num;
        return !Wallpaper.Map.TryGetValue(materialSlot1, out num) || (int)num == (int)item.MaterialItemKey;
    }

    public unsafe void MatchLayout() {
        List<HousingGameObject> objects = null;
        var instance = Memory.Instance;
        var rotation1 = new Quaternion();
        var currentTerritory = instance.GetCurrentTerritory();
        switch (currentTerritory) {
            case Memory.HousingArea.Indoors:
                instance.TryGetNameSortedHousingGameObjectList(out objects);
                this.InteriorItemList.ForEach((Action<HousingItem>)(item => item.ItemStruct = IntPtr.Zero));
                break;
            case Memory.HousingArea.Outdoors:
                this.GetPlotLocation();
                objects = instance.GetExteriorPlacedObjects();
                this.ExteriorItemList.ForEach((Action<HousingItem>)(item => item.ItemStruct = IntPtr.Zero));
                rotation1 = Quaternion.CreateFromAxisAngle(Vector3.UnitY,
                    PlotLocation.rotation);
                break;
            case Memory.HousingArea.Island:
                instance.TryGetIslandGameObjectList(out objects);
                this.ExteriorItemList.ForEach((Action<HousingItem>)(item => item.ItemStruct = IntPtr.Zero));
                break;
        }

        var housingGameObjectList = new List<HousingGameObject>();
        foreach (var housingGameObject in objects) {
            var gameObject = housingGameObject;
            if (this.IsSelectedFloor(gameObject.Y)) {
                var housingRowId = gameObject.housingRowId;
                var position = new Vector3(gameObject.X, gameObject.Y, gameObject.Z);
                var rotation2 = gameObject.rotation;
                HousingItem nearestHousingItem;
                if (currentTerritory == Memory.HousingArea.Indoors) {
                    var itemKey = DalamudApi.DataManager.GetExcelSheet<HousingFurniture>().GetRow(housingRowId)
                        .Item.Value.RowId;
                    nearestHousingItem = Utils.GetNearestHousingItem(
                        this.InteriorItemList.Where(
                            (Func<HousingItem, bool>)(item => this.MatchExactItem(item, itemKey, gameObject))),
                        position);
                } else {
                    if (currentTerritory == Memory.HousingArea.Outdoors) {
                        position = Vector3.Transform(
                            position - PlotLocation.ToVector(), rotation1);
                        rotation2 += PlotLocation.rotation;
                    }

                    var itemKey = DalamudApi.DataManager.GetExcelSheet<HousingYardObject>().GetRow(housingRowId)
                        .Item.Value.RowId;
                    nearestHousingItem = Utils.GetNearestHousingItem(
                        this.ExteriorItemList.Where(
                            (Func<HousingItem, bool>)(item => this.MatchExactItem(item, itemKey, gameObject))),
                        position);
                }

                if (nearestHousingItem == null) {
                    housingGameObjectList.Add(gameObject);
                } else {
                    var vector3 = nearestHousingItem.GetLocation() - position;
                    nearestHousingItem.CorrectLocation = vector3.LengthSquared() < 0.0001;
                    nearestHousingItem.CorrectRotation =
                        rotation2 - (double)nearestHousingItem.Rotate < 0.001;
                    nearestHousingItem.ItemStruct = (IntPtr)gameObject.Item;
                }
            }
        }

        this.UnusedItemList.Clear();
        foreach (var housingGameObject in housingGameObjectList) {
            var housingRowId = housingGameObject.housingRowId;
            var position = new Vector3(housingGameObject.X, housingGameObject.Y, housingGameObject.Z);
            var rotation3 = housingGameObject.rotation;
            HousingItem nearestHousingItem;
            Item item;
            switch (currentTerritory) {
                case Memory.HousingArea.Indoors:
                    item = DalamudApi.DataManager.GetExcelSheet<HousingFurniture>().GetRow(housingRowId).Item.Value;
                    nearestHousingItem = Utils.GetNearestHousingItem(
                        this.InteriorItemList.Where(
                            (Func<HousingItem, bool>)(hItem => this.MatchItem(hItem, item.RowId))),
                        new Vector3(housingGameObject.X, housingGameObject.Y, housingGameObject.Z));
                    break;
                case Memory.HousingArea.Outdoors:
                    position = Vector3.Transform(position - PlotLocation.ToVector(),
                        rotation1);
                    rotation3 += PlotLocation.rotation;
                    goto default;
                default:
                    item = DalamudApi.DataManager.GetExcelSheet<HousingYardObject>().GetRow(housingRowId).Item
                        .Value;
                    nearestHousingItem = Utils.GetNearestHousingItem(
                        this.ExteriorItemList.Where(
                            (Func<HousingItem, bool>)(hItem => this.MatchItem(hItem, item.RowId))), position);
                    break;
            }

            if (nearestHousingItem == null) {
                this.UnusedItemList.Add(new HousingItem(item, housingGameObject.color, housingGameObject.X,
                    housingGameObject.Y, housingGameObject.Z, housingGameObject.rotation));
            } else {
                var vector3 = nearestHousingItem.GetLocation() - position;
                nearestHousingItem.CorrectLocation = vector3.LengthSquared() < 0.0001;
                nearestHousingItem.CorrectRotation = rotation3 - (double)nearestHousingItem.Rotate < 0.001;
                nearestHousingItem.DyeMatch = false;
                nearestHousingItem.ItemStruct = (IntPtr)housingGameObject.Item;
            }
        }
    }

    public unsafe void GetPlotLocation() {
        var outdoorTerritory = Memory.Instance.HousingModule->outdoorTerritory;
        var territoryTypeId = Memory.Instance.GetTerritoryTypeId();
        var row = DalamudApi.DataManager.GetExcelSheet<TerritoryType>().GetRow(territoryTypeId);
        if (row == null) {
            LogError("Cannot identify territory");
        } else {
            var key = row.Name.ToString();
            PlotLocation = Plots.Map[key][outdoorTerritory->Plot + 1];
        }
    }

    public unsafe void LoadExterior() {
        SaveLayoutManager.LoadExteriorFixtures();
        this.ExteriorItemList.Clear();
        var outdoorTerritory = Memory.Instance.HousingModule->outdoorTerritory;
        var ObjList1 = (IntPtr)outdoorTerritory + new IntPtr(16);
        var ObjList2 = ObjList1 + new IntPtr(35176);
        var container = Memory.GetContainer((InventoryType)25001);
        this.GetPlotLocation();
        var fromAxisAngle =
            Quaternion.CreateFromAxisAngle(Vector3.UnitY, PlotLocation.rotation);
        switch (PlotLocation.size) {
            case "s":
                this.Layout.houseSize = "Small";
                break;
            case "m":
                this.Layout.houseSize = "Medium";
                break;
            case "l":
                this.Layout.houseSize = "Large";
                break;
        }

        this.Layout.exteriorScale = 1f;
        this.Layout.properties["entranceLayout"] =
            PlotLocation.entranceLayout;
        for (var inventoryIndex = 0; inventoryIndex < container->Size; ++inventoryIndex) {
            var inventorySlot = ((InventoryContainer*)(IntPtr)container)->GetInventorySlot(inventoryIndex);
            if ((IntPtr)inventorySlot != IntPtr.Zero && inventorySlot->ItemID != 0U) {
                var row = DalamudApi.DataManager.GetExcelSheet<Item>().GetRow(inventorySlot->ItemID);
                if (row != null) {
                    var yardIndex =
                        GetYardIndex(outdoorTerritory->Plot, (byte)inventoryIndex);
                    var itemInfo = HousingObjectManager.GetItemInfo(outdoorTerritory, yardIndex);
                    if ((IntPtr)itemInfo != IntPtr.Zero) {
                        var vector3_1 = Vector3.Transform(
                            new Vector3(itemInfo->X, itemInfo->Y, itemInfo->Z) -
                            PlotLocation.ToVector(), fromAxisAngle);
                        var housingItem = new HousingItem(row, inventorySlot->Stain, vector3_1.X,
                            vector3_1.Y, vector3_1.Z,
                            itemInfo->Rotation + PlotLocation.rotation);
                        var housingGameObjectPtr =
                            (HousingGameObject*)GetObjectFromIndex(ObjList2,
                                itemInfo->ObjectIndex);
                        if ((IntPtr)housingGameObjectPtr == IntPtr.Zero) {
                            housingGameObjectPtr =
                                (HousingGameObject*)GetGameObject(ObjList1,
                                    yardIndex);
                            if ((IntPtr)housingGameObjectPtr != IntPtr.Zero) {
                                var vector3_2 = Vector3.Transform(
                                    new Vector3(housingGameObjectPtr->X, housingGameObjectPtr->Y,
                                        housingGameObjectPtr->Z) -
                                    PlotLocation.ToVector(), fromAxisAngle);
                                housingItem.X = vector3_2.X;
                                housingItem.Y = vector3_2.Y;
                                housingItem.Z = vector3_2.Z;
                            }
                        }

                        if ((IntPtr)housingGameObjectPtr != IntPtr.Zero)
                            housingItem.ItemStruct = (IntPtr)housingGameObjectPtr->Item;
                        this.ExteriorItemList.Add(housingItem);
                    }
                }
            }
        }

        this.Config.Save();
    }

    public bool IsSelectedFloor(float y) {
        if (Memory.Instance.GetCurrentTerritory() != Memory.HousingArea.Indoors ||
            Memory.Instance.GetIndoorHouseSize().Equals("Apartment"))
            return true;
        if (y < -0.001)
            return this.Config.Basement;
        if (y >= -0.001 && y < 6.999)
            return this.Config.GroundFloor;
        if (y < 6.999)
            return false;
        return Memory.Instance.HasUpperFloor() ? this.Config.UpperFloor : this.Config.GroundFloor;
    }

    public unsafe void LoadInterior() {
        SaveLayoutManager.LoadInteriorFixtures();
        List<HousingGameObject> objects;
        Memory.Instance.TryGetNameSortedHousingGameObjectList(out objects);
        this.InteriorItemList.Clear();
        foreach (var gameObject in objects) {
            var housingRowId = gameObject.housingRowId;
            var obj = DalamudApi.DataManager.GetExcelSheet<HousingFurniture>().GetRow(housingRowId)?.Item?.Value;
            if (obj != null && obj.RowId != 0U && this.IsSelectedFloor(gameObject.Y)) {
                var housingItem = new HousingItem(obj, gameObject);
                housingItem.ItemStruct = (IntPtr)gameObject.Item;
                if ((IntPtr)gameObject.Item != IntPtr.Zero &&
                    (IntPtr)gameObject.Item->MaterialManager != IntPtr.Zero) {
                    var materialSlot1 = gameObject.Item->MaterialManager->MaterialSlot1;
                    uint num;
                    if (materialSlot1 != 0 && Wallpaper.Map.TryGetValue(materialSlot1, out num))
                        housingItem.MaterialItemKey = num;
                }

                this.InteriorItemList.Add(housingItem);
            }
        }

        this.Config.Save();
    }

    public unsafe void LoadIsland() {
        SaveLayoutManager.LoadIslandFixtures();
        List<HousingGameObject> objects;
        Memory.Instance.TryGetIslandGameObjectList(out objects);
        this.ExteriorItemList.Clear();
        foreach (var gameObject in objects) {
            var housingRowId = gameObject.housingRowId;
            var obj = DalamudApi.DataManager.GetExcelSheet<HousingYardObject>().GetRow(housingRowId)?.Item?.Value;
            if (obj != null && obj.RowId != 0U)
                this.ExteriorItemList.Add(new HousingItem(obj, gameObject) {
                    ItemStruct = (IntPtr)gameObject.Item
                });
        }

        this.Config.Save();
    }

    public void LoadLayout() {
        var currentTerritory = Memory.Instance.GetCurrentTerritory();
        var housingItemList = currentTerritory == Memory.HousingArea.Indoors
            ? this.InteriorItemList
            : this.ExteriorItemList;
        housingItemList.Clear();
        switch (currentTerritory) {
            case Memory.HousingArea.Indoors:
                this.LoadInterior();
                break;
            case Memory.HousingArea.Outdoors:
                this.LoadExterior();
                break;
            case Memory.HousingArea.Island:
                this.LoadIsland();
                break;
        }

        Log(string.Format("Loaded {0} furniture items",
            housingItemList.Count));
        this.Config.HiddenScreenItemHistory = new List<int>();
        this.Config.Save();
    }

    public bool IsSaveLayoutDetour(IntPtr housingStruct) {
        var flag = this.IsSaveLayoutHook.Original(housingStruct);
        if (!ApplyChange)
            return flag;
        ApplyChange = false;
        return true;
    }

    private void TerritoryChanged(ushort e) {
        this.Config.DrawScreen = false;
        this.Config.Save();
    }

    public void CommandHandler(string command, string arguments) {
        var str = arguments.Trim().Replace("\"", string.Empty);
        try {
            if (!string.IsNullOrEmpty(str) && !str.Equals("config", StringComparison.OrdinalIgnoreCase))
                return;
            this.Gui.ConfigWindow.Visible = !this.Gui.ConfigWindow.Visible;
        } catch (Exception ex) {
            LogError(ex.Message, ex.StackTrace);
        }
    }

    public static void Log(string message, string detail_message = "") {
        var str = message ?? "";
        DalamudApi.PluginLog.Info(detail_message == "" ? str : detail_message, Array.Empty<object>());
        DalamudApi.ChatGui.Print(str);
    }

    public static void LogError(string message, string detail_message = "") {
        var str = message ?? "";
        DalamudApi.PluginLog.Error(str, Array.Empty<object>());
        if (detail_message.Length > 0)
            DalamudApi.PluginLog.Error(detail_message, Array.Empty<object>());
        DalamudApi.ChatGui.PrintError(str);
    }

    private delegate bool UpdateLayoutDelegate(IntPtr a1);

    internal delegate ushort GetIndexDelegate(byte type, byte objStruct);

    internal delegate IntPtr GetActiveObjectDelegate(IntPtr ObjList, uint index);

    internal delegate IntPtr GetObjectDelegate(IntPtr ObjList, ushort index);
}
