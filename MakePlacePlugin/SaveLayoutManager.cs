using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Text.Unicode;

using FFXIVClientStructs.FFXIV.Client.Game.MJI;
using Lumina.Excel.Sheets;
using Lumina.Extensions;
using MakePlacePlugin.Objects;
using static MakePlacePlugin.MakePlacePlugin;

namespace MakePlacePlugin
{

    public class Transform
    {
        public List<float> location { get; set; } = new List<float> { 0, 0, 0 };
        public List<float> rotation { get; set; } = new List<float> { 0, 0, 0, 1 };
        public List<float> scale { get; set; } = new List<float> { 1, 1, 1 };

    }

    public class BasicItem
    {
        public string name { get; set; } = "";
        public uint itemId { get; set; } = 0;
    }

    public class Fixture : BasicItem
    {
        public string level { get; set; } = "";
        public string type { get; set; } = "";

        public Fixture() { }

        public Fixture(string inType)
        {
            type = inType;
        }

        public Fixture(string inType, string inName) : this(inType)
        {
            name = inName;
        }

        public Fixture(string inType, string inName, string inLevel) : this(inType, inName)
        {
            level = inLevel;
        }
    }

    public class Furniture : BasicItem
    {
        public Transform transform { get; set; } = new Transform();

        public Dictionary<string, object> properties { get; set; } = new Dictionary<string, object>();

        public List<Furniture> attachments { get; set; } = new List<Furniture>();

        public Color GetColor()
        {

            if (properties.TryGetValue("color", out object colorObj))
            {
                var color = (string)colorObj;
                return System.Drawing.ColorTranslator.FromHtml("#" + color.Substring(0, 6));
            }

            return Color.Empty;
        }

        public BasicItem GetMaterial()
        {
            if (properties.TryGetValue("material", out object materialObj))
            {
                if (materialObj is JsonElement materialJson)
                {
                    return materialJson.Deserialize<BasicItem>();
                }

            }

            return new BasicItem();
        }

        int ColorDiff(Color c1, Color c2)
        {
            return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R)
                                   + (c1.G - c2.G) * (c1.G - c2.G)
                                   + (c1.B - c2.B) * (c1.B - c2.B));
        }

        public uint GetClosestStain(List<(Color, uint)> colorList)
        {
            var color = GetColor();
            var minDist = 2000;
            uint closestStain = 0;

            foreach (var testTuple in colorList)
            {
                var currentDist = ColorDiff(testTuple.Item1, color);
                if (currentDist < minDist)
                {
                    minDist = currentDist;
                    closestStain = testTuple.Item2;
                }
            }
            return closestStain;
        }
    }

    public class Layout
    {
        public Transform playerTransform { get; set; } = new Transform();

        public string houseSize { get; set; } = "";

        public float interiorScale { get; set; } = 1;

        public List<Fixture> interiorFixture { get; set; } = new List<Fixture>();

        public List<Furniture> interiorFurniture { get; set; } = new List<Furniture>();

        public float exteriorScale { get; set; } = 1;

        public List<Fixture> exteriorFixture { get; set; } = new List<Fixture>();

        public List<Furniture> exteriorFurniture { get; set; } = new List<Furniture>();

        public Dictionary<string, dynamic> properties { get; set; } = new Dictionary<string, dynamic>();

    }

    public class ObjectToInferredTypesConverter : JsonConverter<object>
    {
        public override object Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options) => reader.TokenType switch
            {
                JsonTokenType.True => true,
                JsonTokenType.False => false,
                JsonTokenType.Number when reader.TryGetInt64(out long l) => l,
                JsonTokenType.Number => reader.GetDouble(),
                JsonTokenType.String when reader.TryGetDateTime(out DateTime datetime) => datetime,
                JsonTokenType.String => reader.GetString()!,
                _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
            };

        public override void Write(
            Utf8JsonWriter writer,
            object objectToWrite,
            JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
    }



    public class SaveLayoutManager
    {
        public static Configuration Config;
        public static MakePlacePlugin Plugin;

        public static List<(Color, uint)> ColorList;

        public SaveLayoutManager(MakePlacePlugin plugin, Configuration config)
        {
            Config = config;
            Plugin = plugin;
        }

        public static float layoutScale = 1;


        private static float ParseFloat(string floatString)
        {
            var updatedString = floatString.Replace(",", CultureInfo.InvariantCulture.NumberFormat.NumberDecimalSeparator);

            return float.Parse(updatedString, NumberStyles.Any, CultureInfo.InvariantCulture);
        }

        private float scale(float i)
        {

            return checkZero(i);
        }

        private static float descale(float i)
        {
            return i / layoutScale;
        }

        private float checkZero(float i)
        {
            if (Math.Abs(i) < 0.001) return 0;
            return i;
        }

        List<float> RotationToQuat(float rotation)
        {
            Quaternion q = Quaternion.CreateFromYawPitchRoll(0, 0, rotation);

            return new List<float> { checkZero(q.X), checkZero(q.Y), checkZero(q.Z), checkZero(q.W) };
        }

        static HousingItem ConvertToHousingItem(Furniture furniture)
        {
            var ItemSheet = DalamudApi.DataManager.GetExcelSheet<Item>();
            var itemRow = ItemSheet.FirstOrNull(row => row.Name.ToString().Equals(furniture.name));

            if (itemRow == null) itemRow = ItemSheet.FirstOrNull(row => row.RowId == furniture.itemId);

            if (itemRow == null) return null;

            var r = furniture.transform.rotation;
            var quat = new Quaternion(r[0], r[1], r[2], r[3]);

            var houseItem = new HousingItem(
                itemRow.Value,
                (byte)furniture.GetClosestStain(ColorList),
                descale(furniture.transform.location[0]),
                descale(furniture.transform.location[2]), // switch Y & Z axis
                descale(furniture.transform.location[1]),
                -QuaternionExtensions.ComputeZAngle(quat));


            if (furniture.properties.ContainsKey("material"))
            {
                var material = furniture.GetMaterial();
                houseItem.MaterialItemKey = material.itemId;
            }

            return houseItem;
        }


        static void ImportFurniture(List<HousingItem> itemList, List<Furniture> furnitureList)
        {


            foreach (Furniture furniture in furnitureList)
            {

                var houseItem = ConvertToHousingItem(furniture);
                if (houseItem != null)
                {
                    itemList.Add(houseItem);
                }

                foreach (Furniture child in furniture.attachments)
                {
                    var childItem = ConvertToHousingItem(child);
                    if (childItem != null)
                    {
                        itemList.Add(childItem);
                    }
                }

            }
        }

        public static void ImportLayout(string path)
        {

            string jsonString = File.ReadAllText(path);
            var options = new JsonSerializerOptions();
            options.Converters.Add(new ObjectToInferredTypesConverter());

            Layout layout = JsonSerializer.Deserialize<Layout>(jsonString, options);


            var StainList = DalamudApi.DataManager.GetExcelSheet<Stain>();

            ColorList = new List<(Color, uint)>();

            foreach (var stain in StainList)
            {
                if (stain.Unknown2) // bool for whether the dye can be used for housing
                {
                    ColorList.Add((Color.FromArgb((int)stain.Color), stain.RowId));
                }
            }


            Plugin.InteriorItemList.Clear();
            layoutScale = layout.interiorScale;
            ImportFurniture(Plugin.InteriorItemList, layout.interiorFurniture);

            Plugin.ExteriorItemList.Clear();
            layoutScale = layout.exteriorScale;
            ImportFurniture(Plugin.ExteriorItemList, layout.exteriorFurniture);

            Plugin.Layout = layout;
        }

        public unsafe static void LoadExteriorFixtures()
        {
            var exterior = Plugin.Layout.exteriorFixture;
            exterior.Clear();

            if (!Memory.Instance.GetHousingController(out var controller)) return;

            var mgr = Memory.Instance.HousingModule->GetCurrentManager();

            var customize = controller.Houses(mgr->Plot);

            var housingData = HousingData.Instance;

            var roof = customize.GetPart(ExteriorPartsType.Roof);
            if (roof.FixtureKey != 0 && housingData.IsUnitedExteriorPart(roof.FixtureKey, out var roofItem))
            {
                var fixture = new Fixture();
                fixture.type = Utils.GetExteriorPartDescriptor(ExteriorPartsType.Walls);
                fixture.name = roofItem.Name.ToString();
                fixture.itemId = roofItem.RowId;
                exterior.Add(fixture);

            }
            else
            {
                for (var i = 0; i < HouseCustomize.PartsMax; i++)
                {
                    var type = (ExteriorPartsType)i;
                    var part = customize.GetPart(type);
                    if (!housingData.TryGetItem(part.FixtureKey, out var item)) continue;

                    var fixture = new Fixture();
                    fixture.type = Utils.GetExteriorPartDescriptor(type);
                    fixture.name = item.Name.ToString();
                    fixture.itemId = item.RowId;
                    exterior.Add(fixture);

                }
            }
        }

        public unsafe static void LoadIslandFixtures()
        {
            Plugin.Layout.houseSize = "Island";

            var exterior = Plugin.Layout.exteriorFixture;
            exterior.Clear();

            var manager = MJIManager.Instance();
            var state = manager->IslandState;

            string TerrainMatName(byte id)
            {
                switch (id)
                {
                    case 0:
                        return "Overgrown";
                    case 1:
                        return "Dirt";
                    case 2:
                        return "Stone";
                    case 3:
                        return "Tiled Brick";
                    default:
                        return "";
                }
            }

            exterior.Add(new Fixture("Grounds", TerrainMatName(state.GroundsGlamourId)));
            exterior.Add(new Fixture("Paths", TerrainMatName(state.PathsGlamourId)));
            exterior.Add(new Fixture("Slopes", TerrainMatName(state.SlopesGlamourId)));

            // TODO: Add Cabin Levels when the IslandState struct is fixed

            string ToRoman(byte id)
            {
                switch (id)
                {
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

            var BuildingSheet = DalamudApi.DataManager.GetSubrowExcelSheet<MJIBuilding>();

            var workshop = state.Workshops;
            for (int i = 0; i < 4; i++)
            {
                if (workshop.PlaceId[i] == 0) continue;

                var fixture = new Fixture("Facility");
                fixture.level = "Facility " + ToRoman(workshop.PlaceId[i]);
                fixture.name = BuildingSheet.GetSubrowOrDefault(1, workshop.BuildingLevel[i])?.Name.Value.Text.ToString();

                exterior.Add(fixture);
            }

            var granary = state.Granaries;
            for (int i = 0; i < 2; i++)
            {
                if (granary.PlaceId[i] == 0) continue;
                var fixture = new Fixture("Facility");
                fixture.level = "Facility " + ToRoman(granary.PlaceId[i]);
                fixture.name = BuildingSheet.GetSubrowOrDefault(2, granary.BuildingLevel[i])?.Name.Value.Text.ToString();

                exterior.Add(fixture);
            }

            var LandmarkSheet = DalamudApi.DataManager.GetExcelSheet<MJILandmark>();
            for (int i = 0; i < 5; i++)
            {
                var id = state.LandmarkIds[i];
                if (id == 0) continue;

                var fixture = new Fixture("Landmark");
                fixture.level = "Landmark " + ToRoman((byte)(i + 1));
                fixture.name = LandmarkSheet.GetRowOrDefault(id)?.Name.Value.Text.ToString();
                exterior.Add(fixture);
            }
        }

        public static void LoadInteriorFixtures()
        {
            var layout = Plugin.Layout;
            layout.interiorFixture.Clear();

            for (var i = 0; i < IndoorAreaData.FloorMax; i++)
            {
                var fixtures = Memory.Instance.GetInteriorCommonFixtures(i);
                if (fixtures.Length == 0) continue;

                for (var j = 0; j < IndoorFloorData.PartsMax; j++)
                {
                    if (fixtures[j].FixtureKey == -1 || fixtures[j].FixtureKey == 0) continue;
                    if (!fixtures[j].Item.HasValue) continue;

                    var item = fixtures[j].Item.Value;
                    if (item.RowId == 0) continue;

                    var fixture = new Fixture();
                    fixture.type = Utils.GetInteriorPartDescriptor((InteriorPartsType)j);
                    fixture.level = Utils.GetFloorDescriptor((InteriorFloor)i);

                    fixture.name = item.Name.ExtractText();
                    fixture.itemId = item.RowId;

                    layout.interiorFixture.Add(fixture);
                }
            }

            layout.houseSize = Memory.Instance.GetIndoorHouseSize();

            var territoryId = Memory.Instance.GetTerritoryTypeId();

            if (DalamudApi.DataManager.GetExcelSheet<TerritoryType>().TryGetRow(territoryId, out var row))
            {
                var placeName = row.Name.ToString();

                var district = new Fixture();
                district.type = "District";

                var districtName = placeName.Substring(0, 2);

                switch (districtName)
                {
                    case "s1":
                        district.name = "Mist";
                        break;
                    case "f1":
                        district.name = "Lavender Beds";
                        break;
                    case "w1":
                        district.name = "Goblet";
                        break;
                    case "e1":
                        district.name = "Shirogane";
                        break;
                    case "r1":
                        district.name = "Empyreum";
                        break;
                    case "h1":
                        district.name = "Minimalist";
                        break;
                    default:
                        break;
                }
                layout.interiorFixture.Add(district);
            }

        }

        void RecordFurniture(List<Furniture> furnitureList, List<HousingItem> itemList)
        {
            HousingData Data = HousingData.Instance;
            furnitureList.Clear();
            foreach (HousingItem gameObject in itemList)
            {

                var furniture = new Furniture();

                furniture.name = gameObject.Name;
                furniture.itemId = gameObject.ItemKey;
                furniture.transform.location = new List<float> { scale(gameObject.X), scale(gameObject.Z), scale(gameObject.Y) };
                furniture.transform.rotation = RotationToQuat(-gameObject.Rotate);

                if (gameObject.Stain != 0 && Data.TryGetStain(gameObject.Stain, out var stainColor))
                {

                    var color = Utils.StainToVector4(stainColor.Color);
                    var cr = (int)(color.X * 255);
                    var cg = (int)(color.Y * 255);
                    var cb = (int)(color.Z * 255);
                    var ca = (int)(color.W * 255);

                    furniture.properties.Add("color", $"{cr:X2}{cg:X2}{cb:X2}{ca:X2}");

                }
                else if (gameObject.MaterialItemKey != 0)
                {
                    if (DalamudApi.DataManager.GetExcelSheet<Item>().TryGetRow(gameObject.MaterialItemKey, out var item))
                    {
                        var basicItem = new BasicItem();
                        basicItem.name = item.Name.ToString();
                        basicItem.itemId = gameObject.MaterialItemKey;
                        furniture.properties.Add("material", basicItem);
                    }

                }


                furnitureList.Add(furniture);
            }

        }

        public void ExportLayout()
        {

            if (Directory.Exists(Config.SaveLocation))
            {
                throw new Exception("Save file not specified");
            }

            Layout save = Plugin.Layout;
            save.playerTransform = new Transform();

            save.interiorScale = 1;

            RecordFurniture(save.interiorFurniture, Plugin.InteriorItemList);
            RecordFurniture(save.exteriorFurniture, Plugin.ExteriorItemList);


            var encoderSettings = new TextEncoderSettings();
            encoderSettings.AllowCharacters('\'');
            encoderSettings.AllowRange(UnicodeRanges.BasicLatin);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                Converters = { new ObjectToInferredTypesConverter() }

            };
            string jsonString = JsonSerializer.Serialize(save, options);

            string pattern = @"\s+(-?(?:[0-9]*[.])?[0-9]+(?:E-[0-9]+)?,?)\s*(?=\s[-\d\]])";
            string result = Regex.Replace(jsonString, pattern, " $1");

            File.WriteAllText(Config.SaveLocation, result);


            Log("Finished exporting layout");
        }

    }
}
