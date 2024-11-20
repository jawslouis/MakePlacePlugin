using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;

using static MakePlacePlugin.MakePlacePlugin;

namespace MakePlacePlugin
{
    public unsafe class Memory
    {
        // Pointers to modify assembly to enable place anywhere.
        public IntPtr placeAnywhere;
        public IntPtr wallAnywhere;
        public IntPtr wallmountAnywhere;

        public delegate InventoryContainer* GetInventoryContainerDelegate(IntPtr inventoryManager, InventoryType inventoryType);

        private Memory()
        {
            try
            {
                placeAnywhere = DalamudApi.SigScanner.ScanText("C6 ?? ?? ?? 00 00 00 8B FE 48 89") + 6;
                wallAnywhere = DalamudApi.SigScanner.ScanText("48 85 C0 74 ?? C6 87 ?? ?? 00 00 00") + 11;
                wallmountAnywhere = DalamudApi.SigScanner.ScanText("c6 87 83 01 00 00 00 48 83 c4 ??") + 6;

                HousingModulePtr = DalamudApi.SigScanner.GetStaticAddressFromSig("48 8B 05 ?? ?? ?? ?? 8B 52");
                LayoutWorldPtr = DalamudApi.SigScanner.GetStaticAddressFromSig("48 8B D1 48 8B 0D ?? ?? ?? ?? 48 85 C9 74 0A", 3);

            }
            catch (Exception e)
            {
                DalamudApi.PluginLog.Error(e, "Could not load housing memory!!");
            }
        }

        public static Memory Instance { get; private set; }

        private IntPtr HousingModulePtr { get; }
        private IntPtr LayoutWorldPtr { get; }

        public unsafe HousingModule* HousingModule => HousingModulePtr != IntPtr.Zero ? (HousingModule*)Marshal.ReadIntPtr(HousingModulePtr) : null;
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

            if (!DalamudApi.DataManager.GetExcelSheet<TerritoryType>().TryGetRow(territoryId, out var row)) return null;

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
            if (GetCurrentTerritory() != HousingArea.Indoors) return [];
            if (!GetActiveLayout(out var manager)) return [];
            if (!manager.IndoorAreaData.HasValue) return [];
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
            var objects = new List<HousingGameObject>();

            for (var i = 0; i < 400; i++)
            {

                var oPtr = HousingModule->GetCurrentManager()->Objects[i];
                if (oPtr == 0) continue;

                objects.Add(*(HousingGameObject*)oPtr);
            }
            return objects;
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
            if (!DalamudApi.DataManager.GetExcelSheet<TerritoryType>().TryGetRow(GetTerritoryTypeId(), out var territoryRow))
            {
                LogError($"Invalid territory row: {GetTerritoryTypeId()}");
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

        private static void WriteProtectedBytes(IntPtr addr, byte[] b)
        {
            if (addr == IntPtr.Zero) return;
            VirtualProtect(addr, 1, Protection.PAGE_EXECUTE_READWRITE, out var oldProtection);
            Marshal.Copy(b, 0, addr, b.Length);
            VirtualProtect(addr, 1, oldProtection, out _);
        }

        private static void WriteProtectedBytes(IntPtr addr, byte b)
        {
            if (addr == IntPtr.Zero) return;
            WriteProtectedBytes(addr, [b]);
        }

        private static byte ReadProtectedByte(IntPtr addr)
        {
            byte value = 0;

            if (addr == IntPtr.Zero) return value;

            VirtualProtect(addr, 1, Protection.PAGE_EXECUTE_READWRITE, out var oldProtection);
            value = Marshal.ReadByte(addr);
            VirtualProtect(addr, 1, oldProtection, out _);

            return value;
        }

        public bool GetPlaceAnywhere()
        {
            if (placeAnywhere == IntPtr.Zero)
            {
                LogError("Could not setup memory for placing anywhere");
                return false;
            }

            var value = ReadProtectedByte(placeAnywhere);
            return value != 0;

        }

        /// <summary>
        /// Sets the flag for place anywhere in memory.
        /// </summary>
        /// <param name="state">Boolean state for if you can place anywhere.</param>
        public void SetPlaceAnywhere(bool state)
        {
            if (placeAnywhere == IntPtr.Zero || wallAnywhere == IntPtr.Zero || wallmountAnywhere == IntPtr.Zero)
            {
                LogError("Could not setup memory for placing anywhere");
                return;
            }

            // The byte state from boolean.
            var bstate = (byte)(state ? 1 : 0);

            // Write the bytes for place anywhere.
            WriteProtectedBytes(placeAnywhere, bstate);
            WriteProtectedBytes(wallAnywhere, bstate);
            WriteProtectedBytes(wallmountAnywhere, bstate);
        }

        #region Kernel32

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool VirtualProtect(IntPtr lpAddress, uint dwSize, Protection flNewProtect, out Protection lpflOldProtect);

        public enum Protection
        {
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }

        #endregion
    }
}