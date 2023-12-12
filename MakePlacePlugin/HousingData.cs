// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousingData
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;

namespace MakePlacePlugin; 

public class HousingData {
    private static MakePlacePlugin Plugin;
    private readonly Dictionary<uint, HousingFurniture> _furnitureDict;
    private readonly Dictionary<uint, Item> _itemDict;
    private readonly Dictionary<uint, Stain> _stainDict;
    private readonly Dictionary<uint, Dictionary<uint, CommonLandSet>> _territoryToLandSetDict;
    private readonly Dictionary<uint, uint> _unitedDict;
    private readonly Dictionary<uint, HousingYardObject> _yardObjectDict;

    private HousingData() {
        var excelSheet1 = DalamudApi.DataManager.GetExcelSheet<HousingLandSet>();
        var numArray = new uint[4] {
            339U,
            340U,
            341U,
            641U
        };
        this._territoryToLandSetDict = new Dictionary<uint, Dictionary<uint, CommonLandSet>>();
        for (uint index1 = 0; index1 < ((ExcelSheetImpl)excelSheet1).RowCount && index1 < numArray.Length; ++index1) {
            var row = excelSheet1.GetRow(index1);
            var dictionary = new Dictionary<uint, CommonLandSet>();
            for (var index2 = 0; index2 < row.LandSets.Length; ++index2) {
                var commonLandSet = CommonLandSet.FromExd(row.LandSets[index2], index2);
                dictionary[commonLandSet.PlacardId] = commonLandSet;
            }

            this._territoryToLandSetDict[numArray[(int)index1]] = dictionary;
        }

        var excelSheet2 = DalamudApi.DataManager.GetExcelSheet<HousingUnitedExterior>();
        this._unitedDict = new Dictionary<uint, uint>();
        foreach (var housingUnitedExterior in excelSheet2)
        foreach (LazyRow<HousingExterior> lazyRow in housingUnitedExterior.Item)
            this._unitedDict[lazyRow.Row] = housingUnitedExterior.RowId;
        this._itemDict = ((IEnumerable<Item>)DalamudApi.DataManager.GetExcelSheet<Item>()).Where(
            (Func<Item, bool>)(item => {
                if (item.AdditionalData == 0U)
                    return false;
                return item.ItemSearchCategory.Row == 65U || item.ItemSearchCategory.Row == 66U;
            })).ToDictionary((Func<Item, uint>)(row => row.AdditionalData), (Func<Item, Item>)(row => row));
        this._stainDict =
            ((IEnumerable<Stain>)DalamudApi.DataManager.GetExcelSheet<Stain>()).ToDictionary(
                (Func<Stain, uint>)(row => row.RowId), (Func<Stain, Stain>)(row => row));
        this._furnitureDict =
            ((IEnumerable<HousingFurniture>)DalamudApi.DataManager.GetExcelSheet<HousingFurniture>()).ToDictionary(
                (Func<HousingFurniture, uint>)(row => row.RowId),
                (Func<HousingFurniture, HousingFurniture>)(row => row));
        this._yardObjectDict =
            ((IEnumerable<HousingYardObject>)DalamudApi.DataManager.GetExcelSheet<HousingYardObject>()).ToDictionary(
                (Func<HousingYardObject, uint>)(row => row.RowId),
                (Func<HousingYardObject, HousingYardObject>)(row => row));
        var pluginLog1 = DalamudApi.PluginLog;
        var interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
        interpolatedStringHandler.AppendLiteral("Loaded ");
        interpolatedStringHandler.AppendFormatted(this._territoryToLandSetDict.Keys.Count);
        interpolatedStringHandler.AppendLiteral(" landset rows");
        var stringAndClear1 = interpolatedStringHandler.ToStringAndClear();
        var objArray1 = Array.Empty<object>();
        pluginLog1.Info(stringAndClear1, objArray1);
        var pluginLog2 = DalamudApi.PluginLog;
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(17, 1);
        interpolatedStringHandler.AppendLiteral("Loaded ");
        interpolatedStringHandler.AppendFormatted(this._furnitureDict.Keys.Count);
        interpolatedStringHandler.AppendLiteral(" furniture");
        var stringAndClear2 = interpolatedStringHandler.ToStringAndClear();
        var objArray2 = Array.Empty<object>();
        pluginLog2.Info(stringAndClear2, objArray2);
        var pluginLog3 = DalamudApi.PluginLog;
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
        interpolatedStringHandler.AppendLiteral("Loaded ");
        interpolatedStringHandler.AppendFormatted(this._yardObjectDict.Keys.Count);
        interpolatedStringHandler.AppendLiteral(" yard objects");
        var stringAndClear3 = interpolatedStringHandler.ToStringAndClear();
        var objArray3 = Array.Empty<object>();
        pluginLog3.Error(stringAndClear3, objArray3);
        var pluginLog4 = DalamudApi.PluginLog;
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(20, 1);
        interpolatedStringHandler.AppendLiteral("Loaded ");
        interpolatedStringHandler.AppendFormatted(this._unitedDict.Keys.Count);
        interpolatedStringHandler.AppendLiteral(" united parts");
        var stringAndClear4 = interpolatedStringHandler.ToStringAndClear();
        var objArray4 = Array.Empty<object>();
        pluginLog4.Error(stringAndClear4, objArray4);
        var pluginLog5 = DalamudApi.PluginLog;
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(12, 1);
        interpolatedStringHandler.AppendLiteral("Loaded ");
        interpolatedStringHandler.AppendFormatted(this._stainDict.Keys.Count);
        interpolatedStringHandler.AppendLiteral(" dyes");
        var stringAndClear5 = interpolatedStringHandler.ToStringAndClear();
        var objArray5 = Array.Empty<object>();
        pluginLog5.Error(stringAndClear5, objArray5);
        var pluginLog6 = DalamudApi.PluginLog;
        interpolatedStringHandler = new DefaultInterpolatedStringHandler(33, 1);
        interpolatedStringHandler.AppendLiteral("Loaded ");
        interpolatedStringHandler.AppendFormatted(this._itemDict.Keys.Count);
        interpolatedStringHandler.AppendLiteral(" items with AdditionalData");
        var stringAndClear6 = interpolatedStringHandler.ToStringAndClear();
        var objArray6 = Array.Empty<object>();
        pluginLog6.Error(stringAndClear6, objArray6);
    }

    public static HousingData Instance { get; private set; }

    public static void Init(MakePlacePlugin plugin) {
        Plugin = plugin;
        Instance = new HousingData();
    }

    public bool TryGetYardObject(uint id, out HousingYardObject yardObject) {
        return this._yardObjectDict.TryGetValue(id, out yardObject);
    }

    public bool TryGetFurniture(uint id, out HousingFurniture furniture) {
        return this._furnitureDict.TryGetValue(id, out furniture);
    }

    public bool IsUnitedExteriorPart(uint id, out Item item) {
        item = null;
        uint key;
        return this._unitedDict.TryGetValue(id, out key) && this._itemDict.TryGetValue(key, out item);
    }

    public bool TryGetLandSetDict(uint id, out Dictionary<uint, CommonLandSet> dict) {
        return this._territoryToLandSetDict.TryGetValue(id, out dict);
    }

    public bool TryGetItem(uint id, out Item item) {
        return this._itemDict.TryGetValue(id, out item);
    }

    public bool TryGetStain(uint id, out Stain stain) {
        return this._stainDict.TryGetValue(id, out stain);
    }
}
