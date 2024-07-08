using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using Dalamud.Game;
using Dalamud.Logging;
using Dalamud.Plugin;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using Lumina.Excel.GeneratedSheets;
using static DisPlacePlugin.DisPlacePlugin;

namespace DisPlacePlugin
{
    public unsafe class Memory
    {

        public static GetInventoryContainerDelegate GetInventoryContainer;
        public delegate InventoryContainer* GetInventoryContainerDelegate(IntPtr inventoryManager, InventoryType inventoryType);

        private Memory()
        {
            try
            {
                housingModulePtr = DalamudApi.SigScanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 8B 52");
                LayoutWorldPtr = DalamudApi.SigScanner.GetStaticAddressFromSig("48 8B D1 48 8B 0D ?? ?? ?? ?? 48 85 C9 74 0A", 3);


                var getInventoryContainerPtr = DalamudApi.SigScanner.ScanText("E8 ?? ?? ?? ?? 40 38 78 10");
                GetInventoryContainer = Marshal.GetDelegateForFunctionPointer<GetInventoryContainerDelegate>(getInventoryContainerPtr);

            }
            catch (Exception e)
            {
                DalamudApi.PluginLog.Error(e, "Could not load housing memory!!");
            }
        }

        public static Memory Instance { get; private set; }

        public IntPtr housingModulePtr { get; }
        public IntPtr LayoutWorldPtr { get; }

        public unsafe HousingModule* HousingModule => housingModulePtr != IntPtr.Zero ? (HousingModule*) Marshal.ReadIntPtr(housingModulePtr) : null;
        public unsafe LayoutWorld* LayoutWorld => LayoutWorldPtr != IntPtr.Zero ? (LayoutWorld*)Marshal.ReadIntPtr(LayoutWorldPtr) : null;

        public unsafe HousingObjectManager* CurrentManager => HousingModule->currentTerritory;
        public unsafe HousingStructure* HousingStructure => LayoutWorld->HousingStruct;


        public static void Init()
        {
            Instance = new Memory();
        }

        public static InventoryContainer* GetContainer(InventoryType inventoryType)
        {
            return InventoryManager.Instance()->GetInventoryContainer(inventoryType);
        }

        public uint GetTerritoryTypeId()
        {
            if (!GetActiveLayout(out var manager)) return 0;
            return manager.TerritoryTypeId;
        }

        public bool HasUpperFloor()
        {
            var houseSize = GetIndoorHouseSize();
            return houseSize.Equals("Medium") || houseSize.Equals("Large");
        }

        public string GetIndoorHouseSize()
        {
            var territoryId = Memory.Instance.GetTerritoryTypeId();
            var row = DalamudApi.DataManager.GetExcelSheet<TerritoryType>().GetRow(territoryId);

            if (row == null) return null;

            var placeName = row.Name.ToString();
            var sizeName = placeName.Substring(1, 3);

            switch (sizeName)
            {
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

        public float GetInteriorLightLevel()
        {

            if (GetCurrentTerritory() != HousingArea.Indoors) return 0f;
            if (!GetActiveLayout(out var manager)) return 0f;
            if (!manager.IndoorAreaData.HasValue) return 0f;
            return manager.IndoorAreaData.Value.LightLevel;
        }

        public CommonFixture[] GetInteriorCommonFixtures(int floorId)
        {
            if (GetCurrentTerritory() != HousingArea.Indoors) return new CommonFixture[0];
            if (!GetActiveLayout(out var manager)) return new CommonFixture[0];
            if (!manager.IndoorAreaData.HasValue) return new CommonFixture[0];
            var floor = manager.IndoorAreaData.Value.GetFloor(floorId);

            var ret = new CommonFixture[IndoorFloorData.PartsMax];
            for (var i = 0; i < IndoorFloorData.PartsMax; i++)
            {
                var key = floor.GetPart(i);
                if (!HousingData.Instance.TryGetItem(unchecked((uint)key), out var item))
                    HousingData.Instance.IsUnitedExteriorPart(unchecked((uint)key), out item);

                ret[i] = new CommonFixture(
                    false,
                    i,
                    key,
                    null,
                    item);
            }

            return ret;
        }

        public CommonFixture[] GetExteriorCommonFixtures(int plotId)
        {
            if (GetCurrentTerritory() != HousingArea.Outdoors) return new CommonFixture[0];
            if (!GetHousingController(out var controller)) return new CommonFixture[0];
            var home = controller.Houses(plotId);

            if (home.Size == -1) return new CommonFixture[0];
            if (home.GetPart(0).Category == -1) return new CommonFixture[0];

            var ret = new CommonFixture[HouseCustomize.PartsMax];
            for (var i = 0; i < HouseCustomize.PartsMax; i++)
            {
                var colorId = home.GetPart(i).Color;
                HousingData.Instance.TryGetStain(colorId, out var stain);
                HousingData.Instance.TryGetItem(home.GetPart(i).FixtureKey, out var item);

                ret[i] = new CommonFixture(
                    true,
                    home.GetPart(i).Category,
                    home.GetPart(i).FixtureKey,
                    stain,
                    item);
            }

            return ret;
        }

        public unsafe List<HousingGameObject> GetExteriorPlacedObjects()
        {

            var mgr = Memory.Instance.HousingModule->outdoorTerritory;

            var outdoorMgrAddr = (IntPtr)mgr;
            var objectListAddr = outdoorMgrAddr + 0x10;
            var activeObjList = objectListAddr + 0x8968;


            var exteriorItems = Memory.GetContainer(InventoryType.HousingExteriorPlacedItems);

            if (exteriorItems == null) throw new Exception("Unable to get inventory for exterior");

            var placedObjects = new List<HousingGameObject>();

            for (int i = 0; i < exteriorItems->Size; i++)
            {
                var item = exteriorItems->GetInventorySlot(i);
                if (item == null || item->ItemId == 0) continue;

                var itemInfoIndex = GetYardIndex(mgr->Plot, (byte)i);
                var itemInfo = HousingObjectManager.GetItemInfo(mgr, itemInfoIndex);

                if (itemInfo == null) continue;

                var gameObj = (HousingGameObject*)GetObjectFromIndex(activeObjList, itemInfo->ObjectIndex);

                if (gameObj == null)
                {
                    gameObj = (HousingGameObject*)GetGameObject(objectListAddr, itemInfoIndex);
                }

                if (gameObj != null)
                {
                    placedObjects.Add(*gameObj);
                }
            }



            return placedObjects;
        }

        public unsafe bool TryGetIslandGameObjectList(out List<HousingGameObject> objects)
        {
            objects = new List<HousingGameObject>();

            var manager = (MjiManagerExtended*)MJIManager.Instance();
            var objectManager = manager->ObjectManager;
            var furnManager = objectManager->FurnitureManager;

            for (int i = 0; i < 200; i++)
            {
                var objPtr = (HousingGameObject*)furnManager->Objects[i];
                if (objPtr == null) continue;
                objects.Add(*objPtr);
            }
            return true;
        }

        public unsafe bool TryGetNameSortedHousingGameObjectList(out List<HousingGameObject> objects)
        {
            objects = null;
            if (HousingModule == null ||
                HousingModule->GetCurrentManager() == null ||
                HousingModule->GetCurrentManager()->Objects == null)
                return false;

            objects = new List<HousingGameObject>();

            for (var i = 0; i < 400; i++)
            {
                var oPtr = HousingModule->GetCurrentManager()->Objects[i];
                if (oPtr == 0)
                    continue;

                var o = *(HousingGameObject*)oPtr;

                objects.Add(o);
            }

            objects.Sort(
                (obj1, obj2) =>
                {
                    string name1 = "", name2 = "";
                    if (HousingData.Instance.TryGetFurniture(obj1.housingRowId, out var furniture1))
                        name1 = furniture1.Item.Value.Name.ToString();

                    if (HousingData.Instance.TryGetFurniture(obj2.housingRowId, out var furniture2))
                        name2 = furniture2.Item.Value.Name.ToString();

                    return string.Compare(name1, name2, StringComparison.Ordinal);
                });
            return true;
        }


        public unsafe bool GetActiveLayout(out LayoutManager manager)
        {
            manager = new LayoutManager();
            if (LayoutWorld == null ||
                LayoutWorld->ActiveLayout == null)
                return false;
            manager = *LayoutWorld->ActiveLayout;
            return true;
        }

        public bool GetHousingController(out HousingController controller)
        {
            controller = new HousingController();
            if (!GetActiveLayout(out var manager) ||
                !manager.HousingController.HasValue)
                return false;

            controller = manager.HousingController.Value;
            return true;
        }

        public enum HousingArea
        {
            Indoors,
            Outdoors,
            Island,
            None
        }

        public unsafe HousingArea GetCurrentTerritory()
        {
            var territoryRow = DalamudApi.DataManager.GetExcelSheet<TerritoryType>().GetRow(GetTerritoryTypeId());
            if (territoryRow == null)
            {
                LogError("Mem Cannot identify territory");
                return HousingArea.None;
            }

            if (territoryRow.Name.ToString().Equals("h1m2"))
            {
                return HousingArea.Island;
            }

            if (HousingModule == null) return HousingArea.None;

            if (HousingModule->IsOutdoors()) return HousingArea.Outdoors;
            else return HousingArea.Indoors;
        }

        public unsafe bool IsHousingMode()
        {
            if (HousingStructure == null)
                return false;
            
            return HousingStructure->Mode != HousingLayoutMode.None;
        }

        /// <summary>
        /// Checks if you can edit a housing item, specifically checks that rotate mode is active.
        /// </summary>
        /// <returns>Boolean state if housing menu is on or off.</returns>
        public unsafe bool CanEditItem()
        {
            if (HousingStructure == null)
                return false;

            // Rotate mode only.
            return HousingStructure->Mode == HousingLayoutMode.Rotate;
        }

        /// <summary>
        /// Writes the position vector to memory.
        /// </summary>
        /// <param name="newPosition">Position vector to write.</param>
        public unsafe void WritePosition(Vector3 newPosition)
        {
            // Don't write if housing mode isn't on.
            if (!CanEditItem())
                return;

            try
            {
                var item = HousingStructure->ActiveItem;
                if (item == null)
                    return;

                // Set the position.
                item->Position = newPosition;
            }
            catch (Exception ex)
            {
                DalamudApi.PluginLog.Error(ex, "Error occured while writing position!");
            }
        }

        public unsafe void WriteRotation(Vector3 newRotation)
        {
            // Don't write if housing mode isn't on.
            if (!CanEditItem())
                return;

            try
            {
                var item = HousingStructure->ActiveItem;
                if (item == null)
                    return;

                // Convert into a quaternion.
                item->Rotation = MoveUtil.ToQ(newRotation);
            }
            catch (Exception ex)
            {
                DalamudApi.PluginLog.Error(ex, "Error occured while writing rotation!");
            }
        }
    }
}