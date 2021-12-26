using Dalamud.Data;
using Dalamud.Game;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.Game.Gui;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiScene;
using Lumina.Excel.GeneratedSheets;
using MakePlacePlugin.Gui;
using MakePlacePlugin.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;

namespace MakePlacePlugin
{
    public class MakePlacePlugin : IDalamudPlugin
    {
        public string Name => "MakePlace Plugin";
        public PluginUi Gui { get; private set; }
        public Configuration Config { get; private set; }

        [PluginService]
        public static CommandManager CommandManager { get; private set; }
        [PluginService]
        public static Framework Framework { get; private set; }

        [PluginService]
        public static DalamudPluginInterface Interface { get; private set; }
        [PluginService]
        public static GameGui GameGui { get; private set; }
        [PluginService]
        public static ChatGui ChatGui { get; private set; }
        [PluginService]
        public static ClientState ClientState { get; private set; }
        [PluginService]
        public static DataManager Data { get; private set; }

        [PluginService]
        public static SigScanner Scanner { get; private set; }

        [PluginService]
        public static TargetManager TargetMgr { get; private set; }


        // Texture dictionary for the housing item icons.
        public readonly Dictionary<ushort, TextureWrap> TextureDictionary = new Dictionary<ushort, TextureWrap>();

        public List<HousingItem> HousingItemList = new List<HousingItem>();

        public static List<HousingItem> ItemsToPlace = new List<HousingItem>();

        public int PreviewTerritory = 0;

        private delegate IntPtr LoadHousingFuncDelegate(IntPtr a1, IntPtr a2);
        private HookWrapper<LoadHousingFuncDelegate> LoadHousingFuncHook;


        private delegate bool UpdateLayoutDelegate(IntPtr a1);
        private HookWrapper<UpdateLayoutDelegate> IsSaveLayoutHook;


        // Function for selecting an item, usually used when clicking on one in game.        
        public delegate void SelectItemDelegate(IntPtr housingStruct, IntPtr item);
        private static HookWrapper<SelectItemDelegate> SelectItemHook;



        public static bool ApplyChange = false;

        public static LayoutExporter HousePrinter;

        public static bool logHousingDetour = false;

        public void Dispose()
        {
            foreach (var t in this.TextureDictionary)
                t.Value?.Dispose();
            TextureDictionary.Clear();

            HookManager.Dispose();

            Config.PlaceAnywhere = false;
            ClientState.TerritoryChanged -= TerritoryChanged;
            CommandManager.RemoveHandler("/makeplace");
            Gui?.Dispose();
            Interface?.Dispose();



        }


        public MakePlacePlugin(
                [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
                [RequiredVersion("1.0")] CommandManager commandManager
            )
        {
            Config = Interface.GetPluginConfig() as Configuration ?? new Configuration();
            Config.Initialize(Interface);
            Config.Save();

            // LoadOffset();
            Initialize();

            CommandManager.AddHandler("/makeplace", new CommandInfo(CommandHandler)
            {
                HelpMessage = "load config window."
            });
            Gui = new PluginUi(this);
            ClientState.TerritoryChanged += TerritoryChanged;


            HousingData.Init(Data, this);
            Memory.Init(Scanner);
            HousePrinter = new LayoutExporter(ChatGui);


        }
        public void Initialize()
        {

            HookManager.Init(Scanner);

            LoadHousingFuncHook = HookManager.Hook<LoadHousingFuncDelegate>("48 8B 41 08 48 85 C0 74 09 48 8D 48 10", LoadHousingFuncDetour);

            IsSaveLayoutHook = HookManager.Hook<UpdateLayoutDelegate>("40 53 48 83 ec 20 48 8b d9 48 8b 0d d8 a4 8c 01 e8 bb 3c b0 ff 33 d2 48 8b c8 e8 41 d6 f2 ff", IsSaveLayoutDetour);

            SelectItemHook = HookManager.Hook<SelectItemDelegate>("E8 ?? ?? ?? ?? 48 8B CE E8 ?? ?? ?? ?? 48 8B 6C 24 40 48 8B CE", SelectItemDetour);

        }


        unsafe static public void SelectItemDetour(IntPtr housing, IntPtr item)
        {
            SelectItemHook.Original(housing, item);
        }


        unsafe static public void SelectItem(IntPtr item)
        {
            SelectItemDetour((IntPtr)Memory.Instance.HousingStructure, item);
        }


        public static bool IsDecorMode()
        {
            var addon = GameGui.GetAddonByName("HousingGoods", 1);

            return addon != IntPtr.Zero;
        }

        public unsafe static bool IsRotateMode()
        {
            return Memory.Instance.HousingStructure->Mode == HousingLayoutMode.Rotate;
        }

        public unsafe void PlaceNextItem()
        {

            if (!IsDecorMode() || !IsRotateMode() || ItemsToPlace.Count == 0)
            {
                return;
            }

            try
            {

                while (ItemsToPlace.Count > 0)
                {
                    var item = ItemsToPlace.First();
                    ItemsToPlace.RemoveAt(0);

                    if (item.ItemStruct == IntPtr.Zero) continue;

                    SetItemPosition(item);

                    if (Config.LoadInterval > 0)
                    {
                        Thread.Sleep(Config.LoadInterval);
                    }

                }



                if (ItemsToPlace.Count == 0)
                {
                    Log("Finished applying layout");
                }

            }
            catch (Exception e)
            {
                LogError($"Error: {e.Message}", e.StackTrace);
            }
        }

        unsafe public static void SetItemPosition(HousingItem rowItem)
        {

            if (!IsDecorMode() || !IsRotateMode())

            {
                LogError("Unable to set position outside of Rotate Layout mode");
                return;
            }

            if (rowItem.ItemStruct == IntPtr.Zero) return;

            Log("Placing " + rowItem.Name);

            var MemInstance = Memory.Instance;

            logHousingDetour = true;
            ApplyChange = true;

            SelectItem(rowItem.ItemStruct);

            Vector3 position = new Vector3(rowItem.X, rowItem.Y, rowItem.Z);
            Vector3 rotation = new Vector3();

            rotation.Y = (float)(rowItem.Rotate * 180 / Math.PI);
            MemInstance.WritePosition(position);
            MemInstance.WriteRotation(rotation);
        }

        public void ApplyLayout()
        {
            Log("Applying layout");
            ItemsToPlace = new List<HousingItem>(Config.HousingItemList);

            var thread = new Thread(PlaceNextItem);
            thread.Start();
        }


        public unsafe void MatchLayout()
        {

            List<HousingGameObject> allObjects;
            Memory Mem = Memory.Instance;
            bool dObjectsLoaded = Mem.TryGetNameSortedHousingGameObjectList(out allObjects);

            List<HousingGameObject> unmatched = new List<HousingGameObject>();

            // first we find perfect match
            foreach (var gameObject in allObjects)
            {

                uint furnitureKey = gameObject.housingRowId;
                var furniture = Data.GetExcelSheet<HousingFurniture>().GetRow(furnitureKey);
                var itemKey = furniture.Item.Value.RowId;

                var houseItem = Config.HousingItemList.FirstOrDefault(item => item.ItemKey == itemKey && item.Stain == gameObject.color && item.ItemStruct == IntPtr.Zero);
                if (houseItem == null)
                {
                    unmatched.Add(gameObject);
                    continue;
                }

                houseItem.ItemStruct = (IntPtr)gameObject.Item;
            }

            // then we match even if the dye doesn't fit
            foreach (var gameObject in unmatched)
            {

                uint furnitureKey = gameObject.housingRowId;
                var furniture = Data.GetExcelSheet<HousingFurniture>().GetRow(furnitureKey);
                var itemKey = furniture.Item.Value.RowId;

                var houseItem = Config.HousingItemList.FirstOrDefault(item => item.ItemKey == itemKey && item.ItemStruct == IntPtr.Zero);
                if (houseItem == null)
                {
                    continue;
                }

                houseItem.ItemStruct = (IntPtr)gameObject.Item;
                houseItem.DyeMatch = false;
            }

        }
        unsafe public void LoadLayout()
        {

            if (Config.HousingItemList.Count > 0)
            {
                Config.HousingItemList.Clear();
            }

            List<HousingGameObject> dObjects;
            Memory Mem = Memory.Instance;
            bool dObjectsLoaded = Mem.TryGetNameSortedHousingGameObjectList(out dObjects);

            HousingItemList.Clear();

            foreach (var gameObject in dObjects)
            {
                uint furnitureKey = gameObject.housingRowId;

                var furniture = Data.GetExcelSheet<HousingFurniture>().GetRow(furnitureKey);
                var item = furniture.Item.Value;
                if (item.RowId == 0) continue;

                byte stain = gameObject.color;
                var rotate = gameObject.rotation;
                var x = gameObject.X;
                var y = gameObject.Y;
                var z = gameObject.Z;

                var housingItem = new HousingItem(
                        item.RowId,
                        stain,
                        x,
                        y,
                        z,
                        rotate,
                        item.Name
                    );

                housingItem.ItemStruct = (IntPtr)gameObject.Item;

                HousingItemList.Add(housingItem);
            }

            Log(String.Format("Loaded {0} furniture items", HousingItemList.Count));

            Config.HousingItemList = HousingItemList.ToList();

            Config.HiddenScreenItemHistory = new List<int>();
            var territoryTypeId = ClientState.TerritoryType;
            Config.LocationId = territoryTypeId;
            Config.Save();
        }


        public bool IsSaveLayoutDetour(IntPtr housingStruct)
        {
            var result = IsSaveLayoutHook.Original(housingStruct);

            if (ApplyChange)
            {
                ApplyChange = false;
                return true;
            }

            return result;
        }


        private IntPtr LoadHousingFuncDetour(IntPtr a1, IntPtr dataPtr)
        {

            HousingItemList.Clear();
            Config.HousingItemList.Clear();
            Config.Save();

            return this.LoadHousingFuncHook.Original(a1, dataPtr);
        }
        private void TerritoryChanged(object sender, ushort e)
        {
            Config.DrawScreen = false;
            Config.Save();
        }
        public void CommandHandler(string command, string arguments)
        {
            var args = arguments.Trim().Replace("\"", string.Empty);

            if (string.IsNullOrEmpty(args) || args.Equals("config", StringComparison.OrdinalIgnoreCase))
            {
                Gui.ConfigWindow.Visible = !Gui.ConfigWindow.Visible;
                return;
            }
        }

        public static void Log(string message, string detail_message = "")
        {
            //if (!Config.PrintMessage) return;
            var msg = $"{message}";
            PluginLog.Log(detail_message == "" ? msg : detail_message);
            ChatGui.Print(msg);
        }
        public static void LogError(string message, string detail_message = "")
        {
            //if (!Config.PrintError) return;
            var msg = $"{message}";
            PluginLog.LogError(detail_message == "" ? msg : detail_message);
            ChatGui.PrintError(msg);
        }

    }

}
