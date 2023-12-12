// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Memory
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using Lumina.Excel.GeneratedSheets;

namespace MakePlacePlugin; 

public class Memory {
    public unsafe delegate InventoryContainer* GetInventoryContainerDelegate(
        IntPtr inventoryManager,
        InventoryType inventoryType);

    public enum HousingArea {
        Indoors,
        Outdoors,
        Island,
        None
    }

    public static GetInventoryContainerDelegate GetInventoryContainer;

    private Memory() {
        try {
            this.HousingModulePtr = DalamudApi.SigScanner.GetStaticAddressFromSig(
                "40 53 48 83 EC 20 33 DB 48 39 1D ?? ?? ?? ?? 75 2C 45 33 C0 33 D2 B9 ?? ?? ?? ?? E8 ?? ?? ?? ?? 48 85 C0 74 11 48 8B C8 E8 ?? ?? ?? ?? 48 89 05 ?? ?? ?? ?? EB 07");
            this.LayoutWorldPtr =
                DalamudApi.SigScanner.GetStaticAddressFromSig(
                    "48 8B 0D ?? ?? ?? ?? 48 85 C9 74 ?? 48 8B 49 40 E9 ?? ?? ?? ??");
            GetInventoryContainer =
                Marshal.GetDelegateForFunctionPointer<GetInventoryContainerDelegate>(
                    DalamudApi.SigScanner.ScanText("E8 ?? ?? ?? ?? 8B 55 BB"));
        } catch (Exception ex) {
            DalamudApi.PluginLog.Error(ex, "Could not load housing memory!!", Array.Empty<object>());
        }
    }

    public static Memory Instance { get; private set; }

    private IntPtr HousingModulePtr { get; }

    private IntPtr LayoutWorldPtr { get; }

    public unsafe HousingModule* HousingModule => this.HousingModulePtr == IntPtr.Zero
        ? null
        : (HousingModule*)Marshal.ReadIntPtr(this.HousingModulePtr);

    public unsafe LayoutWorld* LayoutWorld => this.LayoutWorldPtr == IntPtr.Zero
        ? null
        : (LayoutWorld*)Marshal.ReadIntPtr(this.LayoutWorldPtr);

    public unsafe HousingObjectManager* CurrentManager => this.HousingModule->currentTerritory;

    public unsafe HousingStructure* HousingStructure => this.LayoutWorld->HousingStruct;

    public static void Init() {
        Instance = new Memory();
    }

    public static unsafe InventoryContainer* GetContainer(InventoryType inventoryType) {
        return ((InventoryManager*)(IntPtr)InventoryManager.Instance())->GetInventoryContainer(inventoryType);
    }

    public uint GetTerritoryTypeId() {
        LayoutManager manager;
        return !this.GetActiveLayout(out manager) ? 0U : manager.TerritoryTypeId;
    }

    public bool HasUpperFloor() {
        var indoorHouseSize = this.GetIndoorHouseSize();
        return indoorHouseSize.Equals("Medium") || indoorHouseSize.Equals("Large");
    }

    public string GetIndoorHouseSize() {
        var territoryTypeId = Instance.GetTerritoryTypeId();
        var row = DalamudApi.DataManager.GetExcelSheet<TerritoryType>().GetRow(territoryTypeId);
        if (row == null)
            return null;
        switch (row.Name.ToString().Substring(1, 3)) {
            case "1i1":
                return "Small";
            case "1i2":
                return "Medium";
            case "1i3":
                return "Large";
            case "1i4":
                return "Apartment";
            default:
                return null;
        }
    }

    public float GetInteriorLightLevel() {
        LayoutManager manager;
        return this.GetCurrentTerritory() != HousingArea.Indoors || !this.GetActiveLayout(out manager) ||
               !manager.IndoorAreaData.HasValue
            ? 0.0f
            : manager.IndoorAreaData.Value.LightLevel;
    }

    public CommonFixture[] GetInteriorCommonFixtures(int floorId) {
        if (this.GetCurrentTerritory() != HousingArea.Indoors)
            return new CommonFixture[0];
        LayoutManager manager;
        if (!this.GetActiveLayout(out manager))
            return new CommonFixture[0];
        if (!manager.IndoorAreaData.HasValue)
            return new CommonFixture[0];
        var floor = manager.IndoorAreaData.Value.GetFloor(floorId);
        var interiorCommonFixtures = new CommonFixture[5];
        for (var index = 0; index < 5; ++index) {
            var part = floor.GetPart(index);
            Item obj;
            if (!HousingData.Instance.TryGetItem((uint)part, out obj))
                HousingData.Instance.IsUnitedExteriorPart((uint)part, out obj);
            interiorCommonFixtures[index] = new CommonFixture(false, index, part, null, obj);
        }

        return interiorCommonFixtures;
    }

    public CommonFixture[] GetExteriorCommonFixtures(int plotId) {
        if (this.GetCurrentTerritory() != HousingArea.Outdoors)
            return new CommonFixture[0];
        HousingController controller;
        if (!this.GetHousingController(out controller))
            return new CommonFixture[0];
        var houseCustomize = controller.Houses(plotId);
        if (houseCustomize.Size == -1)
            return new CommonFixture[0];
        if (houseCustomize.GetPart(0).Category == -1)
            return new CommonFixture[0];
        var exteriorCommonFixtures = new CommonFixture[8];
        for (var type = 0; type < 8; ++type) {
            Stain stain;
            HousingData.Instance.TryGetStain(houseCustomize.GetPart(type).Color, out stain);
            Item obj;
            HousingData.Instance.TryGetItem(houseCustomize.GetPart(type).FixtureKey, out obj);
            exteriorCommonFixtures[type] = new CommonFixture(true, houseCustomize.GetPart(type).Category,
                houseCustomize.GetPart(type).FixtureKey, stain, obj);
        }

        return exteriorCommonFixtures;
    }

    public unsafe List<HousingGameObject> GetExteriorPlacedObjects() {
        var outdoorTerritory = Instance.HousingModule->outdoorTerritory;
        var ObjList1 = (IntPtr)outdoorTerritory + new IntPtr(16);
        var ObjList2 = ObjList1 + new IntPtr(35176);
        var container = GetContainer((InventoryType)25001);
        if ((IntPtr)container == IntPtr.Zero)
            throw new Exception("Unable to get inventory for exterior");
        var exteriorPlacedObjects = new List<HousingGameObject>();
        for (var inventoryIndex = 0; inventoryIndex < container->Size; ++inventoryIndex) {
            var inventorySlot = ((InventoryContainer*)(IntPtr)container)->GetInventorySlot(inventoryIndex);
            if ((IntPtr)inventorySlot != IntPtr.Zero && inventorySlot->ItemID != 0U) {
                var yardIndex = MakePlacePlugin.GetYardIndex(outdoorTerritory->Plot, (byte)inventoryIndex);
                var itemInfo = HousingObjectManager.GetItemInfo(outdoorTerritory, yardIndex);
                if ((IntPtr)itemInfo != IntPtr.Zero) {
                    var housingGameObjectPtr =
                        (HousingGameObject*)MakePlacePlugin.GetObjectFromIndex(ObjList2, itemInfo->ObjectIndex);
                    if ((IntPtr)housingGameObjectPtr == IntPtr.Zero)
                        housingGameObjectPtr = (HousingGameObject*)MakePlacePlugin.GetGameObject(ObjList1, yardIndex);
                    if ((IntPtr)housingGameObjectPtr != IntPtr.Zero)
                        exteriorPlacedObjects.Add(*housingGameObjectPtr);
                }
            }
        }

        return exteriorPlacedObjects;
    }

    public unsafe bool TryGetIslandGameObjectList(out List<HousingGameObject> objects) {
        objects = new List<HousingGameObject>();
        var furnitureManager = ((MjiManagerExtended*)MJIManager.Instance())->ObjectManager->FurnitureManager;
        for (var index = 0; index < 200; ++index) {
            var housingGameObjectPtr = (HousingGameObject*)furnitureManager->Objects[index];
            if ((IntPtr)housingGameObjectPtr != IntPtr.Zero)
                objects.Add(*housingGameObjectPtr);
        }

        return true;
    }

    public unsafe bool TryGetNameSortedHousingGameObjectList(out List<HousingGameObject> objects) {
        objects = null;
        if ((IntPtr)this.HousingModule == IntPtr.Zero ||
            (IntPtr)this.HousingModule->GetCurrentManager() == IntPtr.Zero ||
            (IntPtr)this.HousingModule->GetCurrentManager()->Objects == IntPtr.Zero)
            return false;
        objects = new List<HousingGameObject>();
        for (var index = 0; index < 400; ++index) {
            var num = this.HousingModule->GetCurrentManager()->Objects[index];
            if (num != 0UL) {
                var housingGameObject = *(HousingGameObject*)num;
                objects.Add(housingGameObject);
            }
        }

        objects.Sort((obj1, obj2) => {
            var strA = "";
            var strB = "";
            HousingFurniture furniture1;
            if (HousingData.Instance.TryGetFurniture(obj1.housingRowId, out furniture1))
                strA = furniture1.Item.Value.Name.ToString();
            HousingFurniture furniture2;
            if (HousingData.Instance.TryGetFurniture(obj2.housingRowId, out furniture2))
                strB = furniture2.Item.Value.Name.ToString();
            return string.Compare(strA, strB, StringComparison.Ordinal);
        });
        return true;
    }

    public unsafe bool GetActiveLayout(out LayoutManager manager) {
        manager = new LayoutManager();
        if ((IntPtr)this.LayoutWorld == IntPtr.Zero || (IntPtr)this.LayoutWorld->ActiveLayout == IntPtr.Zero)
            return false;
        manager = *this.LayoutWorld->ActiveLayout;
        return true;
    }

    public bool GetHousingController(out HousingController controller) {
        controller = new HousingController();
        LayoutManager manager;
        if (!this.GetActiveLayout(out manager) || !manager.HousingController.HasValue)
            return false;
        controller = manager.HousingController.Value;
        return true;
    }

    public unsafe HousingArea GetCurrentTerritory() {
        var row = DalamudApi.DataManager.GetExcelSheet<TerritoryType>().GetRow(this.GetTerritoryTypeId());
        if (row == null) {
            MakePlacePlugin.LogError("Cannot identify territory");
            return HousingArea.None;
        }

        if (row.Name.ToString().Equals("h1m2"))
            return HousingArea.Island;
        if ((IntPtr)this.HousingModule == IntPtr.Zero)
            return HousingArea.None;
        return this.HousingModule->IsOutdoors() ? HousingArea.Outdoors : HousingArea.Indoors;
    }

    public unsafe bool IsHousingMode() {
        return (IntPtr)this.HousingStructure != IntPtr.Zero && this.HousingStructure->Mode != 0;
    }

    public unsafe bool CanEditItem() {
        return (IntPtr)this.HousingStructure != IntPtr.Zero && this.HousingStructure->Mode == HousingLayoutMode.Rotate;
    }

    public unsafe void WritePosition(Vector3 newPosition) {
        if (!this.CanEditItem())
            return;
        try {
            var activeItem = this.HousingStructure->ActiveItem;
            if ((IntPtr)activeItem == IntPtr.Zero)
                return;
            activeItem->Position = newPosition;
        } catch (Exception ex) {
            DalamudApi.PluginLog.Error(ex, "Error occured while writing position!", Array.Empty<object>());
        }
    }

    public unsafe void WriteRotation(Vector3 newRotation) {
        if (!this.CanEditItem())
            return;
        try {
            var activeItem = this.HousingStructure->ActiveItem;
            if ((IntPtr)activeItem == IntPtr.Zero)
                return;
            activeItem->Rotation = MoveUtil.ToQ(newRotation);
        } catch (Exception ex) {
            DalamudApi.PluginLog.Error(ex, "Error occured while writing rotation!", Array.Empty<object>());
        }
    }
}
