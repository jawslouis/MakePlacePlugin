using System.Collections.Generic;
using System.Linq;
using Dalamud.Data;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;

namespace MakePlacePlugin
{
    public class HousingData
    {
        private readonly Dictionary<uint, HousingFurniture> _furnitureDict;
        private readonly Dictionary<uint, Item> _itemDict;
        private readonly Dictionary<uint, Stain> _stainDict;

        private readonly Dictionary<uint, Dictionary<uint, CommonLandSet>> _territoryToLandSetDict;
        private readonly Dictionary<uint, uint> _unitedDict;
        private readonly Dictionary<uint, HousingYardObject> _yardObjectDict;

        private readonly Dictionary<ushort, uint> _wallpaper;
        private readonly Dictionary<ushort, uint> _smallFishprint;
        private readonly Dictionary<ushort, uint> _mediumFishprint;
        private readonly Dictionary<ushort, uint> _largeFishprint;
        private readonly Dictionary<ushort, uint> _extraLargeFishprint;
        private readonly Dictionary<ushort, uint> _painting;


        private static MakePlacePlugin Plugin;

        private HousingData()
        {
            var sheet = DalamudApi.DataManager.GetExcelSheet<HousingLandSet>();
            uint[] terriKeys = { 339, 340, 341, 641 };

            _territoryToLandSetDict = new Dictionary<uint, Dictionary<uint, CommonLandSet>>();

            for (uint i = 0; i < sheet.RowCount && i < terriKeys.Length; i++)
            {

                var row = sheet.GetRow(i);
                var rowDict = new Dictionary<uint, CommonLandSet>();
                for (var j = 0; j < row.LandSets.Length; j++)
                {
                    var cset = CommonLandSet.FromExd(row.LandSets[j], j);
                    rowDict[cset.PlacardId] = cset;
                }

                _territoryToLandSetDict[terriKeys[i]] = rowDict;
            }

            var unitedExteriorSheet = DalamudApi.DataManager.GetExcelSheet<HousingUnitedExterior>();
            _unitedDict = new Dictionary<uint, uint>();
            foreach (var row in unitedExteriorSheet)
                foreach (var item in row.Item)
                    _unitedDict[item.Row] = row.RowId;

            _itemDict = DalamudApi.DataManager.GetExcelSheet<Item>()
                .Where(item => item.AdditionalData != 0 && (item.ItemSearchCategory.Row == 65 || item.ItemSearchCategory.Row == 66))
                .ToDictionary(row => row.AdditionalData, row => row);

            _stainDict = DalamudApi.DataManager.GetExcelSheet<Stain>().ToDictionary(row => row.RowId, row => row);
            _furnitureDict = DalamudApi.DataManager.GetExcelSheet<HousingFurniture>().ToDictionary(row => row.RowId, row => row);
            _yardObjectDict = DalamudApi.DataManager.GetExcelSheet<HousingYardObject>().ToDictionary(row => row.RowId, row => row);

            DalamudApi.PluginLog.Info($"Loaded {_territoryToLandSetDict.Keys.Count} landset rows");
            DalamudApi.PluginLog.Info($"Loaded {_furnitureDict.Keys.Count} furniture");
            DalamudApi.PluginLog.Info($"Loaded {_yardObjectDict.Keys.Count} yard objects");
            DalamudApi.PluginLog.Info($"Loaded {_unitedDict.Keys.Count} united parts");
            DalamudApi.PluginLog.Info($"Loaded {_stainDict.Keys.Count} dyes");
            DalamudApi.PluginLog.Info($"Loaded {_itemDict.Keys.Count} items with AdditionalData");

            _wallpaper = new Dictionary<ushort, uint>();
            _smallFishprint = new Dictionary<ushort, uint>();
            _mediumFishprint = new Dictionary<ushort, uint>();
            _largeFishprint = new Dictionary<ushort, uint>();
            _extraLargeFishprint = new Dictionary<ushort, uint>();
            _painting = new Dictionary<ushort, uint>();

            var materialSheet = DalamudApi.DataManager.GetExcelSheet<VaseFlower>();

            foreach (var row in materialSheet)
            {
                var id = row.RowId;

                if (id < 1000) continue;
                else if (id > 1000 && id < 2000) _painting.TryAdd(row.Unknown0, row.Item.Row);
                else if (id > 2000 && id < 3000) _wallpaper.TryAdd(row.Unknown0, row.Item.Row);
                else if (id > 3000 && id < 4000) _smallFishprint.TryAdd(row.Unknown0, row.Item.Row);
                else if (id > 4000 && id < 5000) _mediumFishprint.TryAdd(row.Unknown0, row.Item.Row);
                else if (id > 5000 && id < 6000) _largeFishprint.TryAdd(row.Unknown0, row.Item.Row);
                else if (id > 6000 && id < 7000) _extraLargeFishprint.TryAdd(row.Unknown0, row.Item.Row);

            }
        }

        public static HousingData Instance { get; private set; }

        public static void Init(MakePlacePlugin plugin)
        {
            Plugin = plugin;
            Instance = new HousingData();
        }

        public bool TryGetYardObject(uint id, out HousingYardObject yardObject)
        {
            return _yardObjectDict.TryGetValue(id, out yardObject);
        }

        public bool TryGetFurniture(uint id, out HousingFurniture furniture)
        {
            return _furnitureDict.TryGetValue(id, out furniture);
        }

        public bool IsUnitedExteriorPart(uint id, out Item item)
        {
            item = null;
            if (!_unitedDict.TryGetValue(id, out var unitedId))
                return false;

            if (!_itemDict.TryGetValue(unitedId, out item))
                return false;
            return true;
        }

        public bool TryGetLandSetDict(uint id, out Dictionary<uint, CommonLandSet> dict)
        {
            return _territoryToLandSetDict.TryGetValue(id, out dict);
        }

        public bool TryGetItem(uint id, out Item item)
        {
            return _itemDict.TryGetValue(id, out item);
        }

        public bool TryGetStain(uint id, out Stain stain)
        {
            return _stainDict.TryGetValue(id, out stain);
        }


        public uint GetMaterialItemKey(uint itemId, ushort material)
        {
            if (material == 0) return 0;

            // Angler Canvas
            if (itemId == 28931) return _smallFishprint.GetValueOrDefault(material);
            else if (itemId == 28932) return _mediumFishprint.GetValueOrDefault(material);
            else if (itemId == 28933) return _largeFishprint.GetValueOrDefault(material);
            else if (itemId == 28934) return _extraLargeFishprint.GetValueOrDefault(material);

            if (itemId >= 16935 && itemId <= 16937)
            {
                // Picture Frame
                return _painting.GetValueOrDefault(material);
            }

            return _wallpaper.GetValueOrDefault(material);

        }
    }
}