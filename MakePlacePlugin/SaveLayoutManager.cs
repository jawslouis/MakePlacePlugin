// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.SaveLayoutManager
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using Lumina.Excel.GeneratedSheets;
using MakePlacePlugin.Objects;

namespace MakePlacePlugin; 

public class SaveLayoutManager {
    public static Configuration Config;
    public static MakePlacePlugin Plugin;
    public static List<(Color, uint)> ColorList;
    public static float layoutScale = 1f;

    public SaveLayoutManager(MakePlacePlugin plugin, Configuration config) {
        Config = config;
        Plugin = plugin;
    }

    private static float ParseFloat(string floatString) {
        return float.Parse(floatString.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator),
            NumberStyles.Any, CultureInfo.InvariantCulture);
    }

    private float scale(float i) {
        return this.checkZero(i);
    }

    private static float descale(float i) {
        return i / layoutScale;
    }

    private float checkZero(float i) {
        return Math.Abs(i) < 0.001 ? 0.0f : i;
    }

    private List<float> RotationToQuat(float rotation) {
        var fromYawPitchRoll = Quaternion.CreateFromYawPitchRoll(0.0f, 0.0f, rotation);
        return new List<float> {
            this.checkZero(fromYawPitchRoll.X),
            this.checkZero(fromYawPitchRoll.Y),
            this.checkZero(fromYawPitchRoll.Z),
            this.checkZero(fromYawPitchRoll.W)
        };
    }

    private static HousingItem ConvertToHousingItem(Furniture furniture) {
        var excelSheet = DalamudApi.DataManager.GetExcelSheet<Item>();
        var obj =
            ((IEnumerable<Item>)excelSheet).FirstOrDefault((Func<Item, bool>)(row =>
                row.Name.ToString().Equals(furniture.name))) ??
            excelSheet.FirstOrDefault(row => (int)row.RowId == (int)furniture.itemId);
        if (obj == null)
            return null;
        var rotation = furniture.transform.rotation;
        var q = new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
        var housingItem = new HousingItem(obj, (byte)furniture.GetClosestStain(ColorList),
            descale(furniture.transform.location[0]), descale(furniture.transform.location[2]),
            descale(furniture.transform.location[1]), -q.ComputeZAngle());
        if (furniture.properties.ContainsKey("material")) {
            var material = furniture.GetMaterial();
            housingItem.MaterialItemKey = material.itemId;
        }

        return housingItem;
    }

    private static void ImportFurniture(List<HousingItem> itemList, List<Furniture> furnitureList) {
        foreach (var furniture in furnitureList) {
            var housingItem1 = ConvertToHousingItem(furniture);
            if (housingItem1 != null)
                itemList.Add(housingItem1);
            foreach (var attachment in furniture.attachments) {
                var housingItem2 = ConvertToHousingItem(attachment);
                if (housingItem2 != null)
                    itemList.Add(housingItem2);
            }
        }
    }

    public static void ImportLayout(string path) {
        var json = File.ReadAllText(path);
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Converters.Add(new ObjectToInferredTypesConverter());
        var options = serializerOptions;
        var layout = JsonSerializer.Deserialize<Layout>(json, options);
        var excelSheet = DalamudApi.DataManager.GetExcelSheet<Stain>();
        ColorList = new List<(Color, uint)>();
        foreach (var stain in excelSheet)
            if (stain.Unknown6)
                ColorList.Add((Color.FromArgb((int)stain.Color), stain.RowId));
        Plugin.InteriorItemList.Clear();
        layoutScale = layout.interiorScale;
        ImportFurniture(Plugin.InteriorItemList, layout.interiorFurniture);
        Plugin.ExteriorItemList.Clear();
        layoutScale = layout.exteriorScale;
        ImportFurniture(Plugin.ExteriorItemList, layout.exteriorFurniture);
        Plugin.Layout = layout;
    }

    public static unsafe void LoadExteriorFixtures() {
        var exteriorFixture = Plugin.Layout.exteriorFixture;
        exteriorFixture.Clear();
        HousingController controller;
        if (!Memory.Instance.GetHousingController(out controller))
            return;
        var currentManager = Memory.Instance.HousingModule->GetCurrentManager();
        var houseCustomize = controller.Houses(currentManager->Plot);
        var instance = HousingData.Instance;
        var part1 = houseCustomize.GetPart(ExteriorPartsType.Roof);
        Item obj1;
        if (part1.FixtureKey != 0 && instance.IsUnitedExteriorPart(part1.FixtureKey, out obj1)) {
            var fixture = new Fixture();
            fixture.type = Utils.GetExteriorPartDescriptor(ExteriorPartsType.Walls);
            fixture.name = obj1.Name.ToString();
            fixture.itemId = obj1.RowId;
            exteriorFixture.Add(fixture);
        } else {
            for (var index = 0; index < 8; ++index) {
                var exteriorPartsType = (ExteriorPartsType)index;
                var part2 = houseCustomize.GetPart(exteriorPartsType);
                Item obj2;
                if (instance.TryGetItem(part2.FixtureKey, out obj2)) {
                    var fixture = new Fixture();
                    fixture.type = Utils.GetExteriorPartDescriptor(exteriorPartsType);
                    fixture.name = obj2.Name.ToString();
                    fixture.itemId = obj2.RowId;
                    exteriorFixture.Add(fixture);
                }
            }
        }
    }

    public static unsafe void LoadIslandFixtures() {
        Plugin.Layout.houseSize = "Island";
        var exteriorFixture = Plugin.Layout.exteriorFixture;
        exteriorFixture.Clear();
        var islandState = MJIManager.Instance()->IslandState;
        exteriorFixture.Add(new Fixture("Grounds", TerrainMatName(islandState.GroundsGlamourId)));
        exteriorFixture.Add(new Fixture("Paths", TerrainMatName(islandState.PathsGlamourId)));
        exteriorFixture.Add(new Fixture("Slopes", TerrainMatName(islandState.SlopesGlamourId)));
        var excelSheet1 = DalamudApi.DataManager.GetExcelSheet<MJIBuilding>();
        var workshops = islandState.Workshops;
        for (var index = 0; index < 4; ++index)
            if (workshops.PlaceId[index] != 0) {
                var fixture = new Fixture("Facility");
                fixture.level = "Facility " + ToRoman(workshops.PlaceId[index]);
                fixture.name = excelSheet1.GetRow(1U, workshops.GlamourLevel[index])?.Name.Value.Text.ToString();
                exteriorFixture.Add(fixture);
            }

        var granaries = islandState.Granaries;
        for (var index = 0; index < 4; ++index)
            if (granaries.PlaceId[index] != 0) {
                var fixture = new Fixture("Facility");
                fixture.level = "Facility " + ToRoman(granaries.PlaceId[index]);
                fixture.name = excelSheet1.GetRow(2U, fixture.itemId)?.Name.Value.Text.ToString();
                exteriorFixture.Add(fixture);
            }

        var excelSheet2 = DalamudApi.DataManager.GetExcelSheet<MJILandmark>();
        for (var index = 0; index < 5; ++index) {
            var num = islandState.LandmarkIds[index];
            if (num != 0) {
                var fixture = new Fixture("Landmark");
                fixture.level = "Landmark " + ToRoman((byte)(index + 1));
                fixture.name = excelSheet2.GetRow(num)?.Name.Value.Text.ToString();
                exteriorFixture.Add(fixture);
            }
        }

        static string TerrainMatName(byte id) {
            switch (id) {
                case 0:
                    return "Overgrown";
                case 1:
                    return "Dirt";
                case 2:
                    return "Stone";
                default:
                    return "";
            }
        }

        static string ToRoman(byte id) {
            switch (id) {
                case 1:
                    return "I";
                case 2:
                    return "II";
                case 3:
                    return "III";
                case 4:
                    return "IV";
                case 5:
                    return "V";
                case 6:
                    return "VI";
                default:
                    return "";
            }
        }
    }

    public static void LoadInteriorFixtures() {
        var layout = Plugin.Layout;
        layout.interiorFixture.Clear();
        for (var index = 0; index < 4; ++index) {
            var interiorCommonFixtures = Memory.Instance.GetInteriorCommonFixtures(index);
            if (interiorCommonFixtures.Length != 0)
                for (var partsType = 0; partsType < 5; ++partsType)
                    if (interiorCommonFixtures[partsType].FixtureKey != -1 &&
                        interiorCommonFixtures[partsType].FixtureKey != 0 &&
                        interiorCommonFixtures[partsType].Item != null) {
                        var fixture = new Fixture();
                        fixture.type = Utils.GetInteriorPartDescriptor((InteriorPartsType)partsType);
                        fixture.level = Utils.GetFloorDescriptor((InteriorFloor)index);
                        fixture.name = interiorCommonFixtures[partsType].Item.Name.ToString();
                        fixture.itemId = interiorCommonFixtures[partsType].Item.RowId;
                        layout.interiorFixture.Add(fixture);
                    }
        }

        layout.houseSize = Memory.Instance.GetIndoorHouseSize();
        var territoryTypeId = Memory.Instance.GetTerritoryTypeId();
        var row = DalamudApi.DataManager.GetExcelSheet<TerritoryType>().GetRow(territoryTypeId);
        if (row == null)
            return;
        var str = row.Name.ToString();
        var fixture1 = new Fixture();
        fixture1.type = "District";
        switch (str.Substring(0, 2)) {
            case "s1":
                fixture1.name = "Mist";
                break;
            case "f1":
                fixture1.name = "Lavender Beds";
                break;
            case "w1":
                fixture1.name = "Goblet";
                break;
            case "e1":
                fixture1.name = "Shirogane";
                break;
            case "r1":
                fixture1.name = "Empyreum";
                break;
        }

        layout.interiorFixture.Add(fixture1);
    }

    private void RecordFurniture(List<Furniture> furnitureList, List<HousingItem> itemList) {
        var instance = HousingData.Instance;
        furnitureList.Clear();
        foreach (var housingItem in itemList) {
            var furniture = new Furniture();
            furniture.name = housingItem.Name;
            furniture.itemId = housingItem.ItemKey;
            furniture.transform.location = new List<float> {
                this.scale(housingItem.X),
                this.scale(housingItem.Z),
                this.scale(housingItem.Y)
            };
            furniture.transform.rotation = this.RotationToQuat(-housingItem.Rotate);
            Stain stain;
            if (housingItem.Stain != 0 && instance.TryGetStain(housingItem.Stain, out stain)) {
                var vector4 = Utils.StainToVector4(stain.Color);
                var num1 = (int)(vector4.X * (double)byte.MaxValue);
                var num2 = (int)(vector4.Y * (double)byte.MaxValue);
                var num3 = (int)(vector4.Z * (double)byte.MaxValue);
                var num4 = (int)(vector4.W * (double)byte.MaxValue);
                var properties = furniture.properties;
                var interpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 4);
                interpolatedStringHandler.AppendFormatted(num1, "X2");
                interpolatedStringHandler.AppendFormatted(num2, "X2");
                interpolatedStringHandler.AppendFormatted(num3, "X2");
                interpolatedStringHandler.AppendFormatted(num4, "X2");
                var stringAndClear = interpolatedStringHandler.ToStringAndClear();
                properties.Add("color", stringAndClear);
            } else if (housingItem.MaterialItemKey != 0U) {
                var row = DalamudApi.DataManager.GetExcelSheet<Item>().GetRow(housingItem.MaterialItemKey);
                if (row != null)
                    furniture.properties.Add("material", new BasicItem {
                        name = row.Name.ToString(),
                        itemId = housingItem.MaterialItemKey
                    });
            }

            furnitureList.Add(furniture);
        }
    }

    public void ExportLayout() {
        if (Directory.Exists(Config.SaveLocation))
            throw new Exception("Save file not specified");
        var layout = Plugin.Layout;
        layout.playerTransform = new Transform();
        layout.interiorScale = 1f;
        this.RecordFurniture(layout.interiorFurniture, Plugin.InteriorItemList);
        this.RecordFurniture(layout.exteriorFurniture, Plugin.ExteriorItemList);
        var textEncoderSettings = new TextEncoderSettings();
        textEncoderSettings.AllowCharacters('\'');
        textEncoderSettings.AllowRange(UnicodeRanges.BasicLatin);
        var serializerOptions = new JsonSerializerOptions();
        serializerOptions.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
        serializerOptions.WriteIndented = true;
        serializerOptions.Converters.Add(new ObjectToInferredTypesConverter());
        var options = serializerOptions;
        var contents = Regex.Replace(JsonSerializer.Serialize(layout, options),
            "\\s+(-?(?:[0-9]*[.])?[0-9]+(?:E-[0-9]+)?,?)\\s*(?=\\s[-\\d\\]])", " $1");
        File.WriteAllText(Config.SaveLocation, contents);
        MakePlacePlugin.Log("Finished exporting layout");
    }
}
