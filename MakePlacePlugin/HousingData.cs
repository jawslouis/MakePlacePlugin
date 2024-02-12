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
    }
}