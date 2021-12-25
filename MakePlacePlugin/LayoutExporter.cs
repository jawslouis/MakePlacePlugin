using Dalamud.Game.Gui;
using Dalamud.Logging;
using Lumina.Excel.GeneratedSheets;
using MakePlacePlugin.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text.Unicode;
using System.Linq;
using static MakePlacePlugin.MakePlacePlugin;
using System.Drawing;

namespace MakePlacePlugin
{

    class Transform
    {
        public List<float> location { get; set; }
        public List<float> rotation { get; set; }
        public List<float> scale { get; set; } = new List<float> { 1, 1, 1 };

    }



    struct SaveProperty
    {
        public string key { get; set; }
        public string value { get; set; }

        public SaveProperty(string k, string v)
        {
            key = k;
            value = v;
        }
    }

    class Furniture
    {
        public string name { get; set; }

        public Transform transform { get; set; } = new Transform();

        public List<SaveProperty> properties { get; set; } = new List<SaveProperty>();

        public Color GetColor()
        {
            foreach (var prop in properties)
            {
                if (prop.key.Equals("Color"))
                {
                    return System.Drawing.ColorTranslator.FromHtml("#" + prop.value.Substring(0, 6));
                }
            }

            return Color.Empty;
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

    class Layout
    {
        public Transform playerTransform { get; set; } = new Transform();

        public string houseSize { get; set; }

        public List<SaveProperty> fixture { get; set; } = new List<SaveProperty>();
        public List<Furniture> furniture { get; set; } = new List<Furniture>();
    }


    public class LayoutExporter
    {
        public ChatGui chat;

        public LayoutExporter(ChatGui chatGui)
        {
            chat = chatGui;
        }

        public static float layoutScale = 1;


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

        public static List<HousingItem> ImportLayout(string path)
        {

            string jsonString = File.ReadAllText(path);

            Layout layout = JsonSerializer.Deserialize<Layout>(jsonString);

            List<HousingItem> houseItems = new List<HousingItem>();


            var ItemList = Data.GetExcelSheet<Item>();
            var StainList = Data.GetExcelSheet<Stain>();

            var colorList = new List<(Color, uint)>();

            foreach (var stain in StainList)
            {
                var color = Utils.StainToVector4(stain.Color);
                colorList.Add((Color.FromArgb((int)stain.Color), stain.RowId));
            }

            layoutScale = 1;
            foreach (var prop in layout.fixture)
            {
                if (prop.key.Equals("Scale"))
                {
                    layoutScale = float.Parse(prop.value);

                }
            }


            foreach (Furniture furniture in layout.furniture)
            {
                var itemRow = ItemList.FirstOrDefault(row => row.Name.ToString().Equals(furniture.name));

                if (itemRow == null) continue;

                var r = furniture.transform.rotation;
                var quat = new Quaternion(r[0], r[1], r[2], r[3]);

                var houseItem = new HousingItem(
                    itemRow.RowId,
                    (byte)furniture.GetClosestStain(colorList),
                    descale(furniture.transform.location[0]),
                    descale(furniture.transform.location[2]), // switch Y & Z axis
                    descale(furniture.transform.location[1]),
                    -QuaternionExtensions.ComputeZAngle(quat),
                    furniture.name);

                houseItems.Add(houseItem);
            }

            return houseItems;
        }

        public void ExportLayout(List<HousingItem> HousingItemList, Configuration Config)
        {

            Memory Mem = Memory.Instance;

            HousingData Data = HousingData.Instance;

            Layout save = new Layout();
            save.playerTransform.location = new List<float> { 0, 0, 0 };
            save.playerTransform.rotation = RotationToQuat(0);

            if (Data.TryGetLandSetDict(Mem.GetTerritoryTypeId(), out var landSets))
            {

            }


            for (var i = 0; i < IndoorAreaData.FloorMax; i++)
            {
                var fixtures = Mem.GetInteriorCommonFixtures(i);
                if (fixtures.Length == 0) continue;
                var isCurrentFloor = Mem.CurrentFloor() == (InteriorFloor)i;

                for (var j = 0; j < IndoorFloorData.PartsMax; j++)
                {
                    if (fixtures[j].FixtureKey == -1 || fixtures[j].FixtureKey == 0) continue;
                    var fixtureName = Utils.GetInteriorPartDescriptor((InteriorPartsType)j);


                    var prop = new SaveProperty();

                    prop.key = $"{Utils.GetFloorDescriptor((InteriorFloor)i)}:{fixtureName}".Replace(" ", "");
                    prop.value = fixtures[j].Item.Name.ToString();
                    save.fixture.Add(prop);
                }
            }

            var territoryId = Memory.Instance.GetTerritoryTypeId();
            var row = MakePlacePlugin.Data.GetExcelSheet<TerritoryType>().GetRow(territoryId);

            if (row != null)
            {
                var names = row.PlaceName.Value.Name.ToString().Split('-');

                string sizeString = "Apartment";

                switch (names[0].Trim())
                {
                    case "Private Cottage":
                        sizeString = "Small";
                        break;
                    case "Private House":
                        sizeString = "Medium";
                        break;
                    case "Private Mansion":
                        sizeString = "Large";
                        break;
                    default:
                        break;
                }

                save.houseSize = sizeString;

                save.fixture.Add(new SaveProperty("District", names[1].Replace("The", "").Trim()));
            }


            save.fixture.Add(new SaveProperty("Scale", "1"));


            foreach (HousingItem gameObject in HousingItemList)
            {

                var furniture = new Furniture();

                furniture.name = gameObject.Name;

                furniture.transform.location = new List<float> { scale(gameObject.X), scale(gameObject.Z), scale(gameObject.Y) };

                furniture.transform.rotation = RotationToQuat(-gameObject.Rotate);

                if (gameObject.Stain != 0 && Data.TryGetStain(gameObject.Stain, out var stainColor))
                {

                    var color = Utils.StainToVector4(stainColor.Color);
                    var cr = (int)(color.X * 255);
                    var cg = (int)(color.Y * 255);
                    var cb = (int)(color.Z * 255);
                    var ca = (int)(color.W * 255);

                    furniture.properties.Add(new SaveProperty("Color", $"{cr:X2}{cg:X2}{cb:X2}{ca:X2}"));

                }

                save.furniture.Add(furniture);
            }

            var encoderSettings = new TextEncoderSettings();
            encoderSettings.AllowCharacters('\'');
            encoderSettings.AllowRange(UnicodeRanges.BasicLatin);

            var options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(save, options);

            string pattern = @"\s+(-?(?:[0-9]*[.])?[0-9]+(?:E-[0-9]+)?,?)\s*(?=\s)";
            string result = Regex.Replace(jsonString, pattern, " $1");

            File.WriteAllText(Config.SaveLocation, result);

            Log("Finished exporting layout");
        }

    }
}
