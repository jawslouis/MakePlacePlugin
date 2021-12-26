using System;
using System.Numerics;
using Dalamud.Logging;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;

namespace MakePlacePlugin
{
    public static class Utils
    {
        public static string GetExteriorPartDescriptor(ExteriorPartsType partsType)
        {
            return partsType switch
            {
                ExteriorPartsType.Roof => "Roof",
                ExteriorPartsType.Walls => "Walls",
                ExteriorPartsType.Windows => "Windows",
                ExteriorPartsType.Door => "Door",
                ExteriorPartsType.RoofOpt => "Roof (opt)",
                ExteriorPartsType.WallOpt => "Wall (opt)",
                ExteriorPartsType.SignOpt => "Signboard (opt)",
                ExteriorPartsType.Fence => "Fence",
                _ => "Unknown"
            };
        }

        public static string GetInteriorPartDescriptor(InteriorPartsType partsType)
        {
            return partsType switch
            {
                InteriorPartsType.Walls => "Wall",
                InteriorPartsType.Windows => "Window",
                InteriorPartsType.Door => "Door",
                InteriorPartsType.Floor => "Floor",
                InteriorPartsType.Light => "Light",
                _ => "Unknown"
            };
        }

        public static string GetFloorDescriptor(InteriorFloor floor)
        {
            return floor switch
            {
                InteriorFloor.Ground => "GroundFloor",
                InteriorFloor.Basement => "Basement",
                InteriorFloor.Upstairs => "UpperFloor",
                InteriorFloor.External => "Main",
                _ => "Unknown"
            };
        }

        public static float DistanceFromPlayer(HousingGameObject obj, Vector3 playerPos)
        {
            return Distance(new Vector3(playerPos.X, playerPos.Y, playerPos.Z), new Vector3(obj.X, obj.Y, obj.Z));
        }

        public static float Distance(Vector3 v1, Vector3 v2)
        {
            var x1 = Math.Pow(v2.X - v1.X, 2);
            var y1 = Math.Pow(v2.Y - v1.Y, 2);
            var z1 = Math.Pow(v2.Z - v1.Z, 2);

            return (float)Math.Sqrt(x1 + y1 + z1);
        }


        public static void StainButton(string id, Stain color, Vector2 size)
        {
            var floatColor = StainToVector4(color.Color);
            ImGui.ColorButton($"##{id}", floatColor, ImGuiColorEditFlags.NoTooltip, size);
        }

        private static bool Collides(Vector2 origin, Vector2 bounds, Vector2 pos)
        {
            return pos.X > origin.X && pos.X < bounds.X &&
                   pos.Y > origin.Y && pos.Y < bounds.Y;
        }

        private static void ColorTooltip(string text, Vector4 color)
        {
            ImGui.BeginTooltip();
            var size = new Vector2(ImGui.GetFontSize() * 4 + ImGui.GetStyle().FramePadding.Y * 2,
                ImGui.GetFontSize() * 4 + ImGui.GetStyle().FramePadding.Y * 2);
            var cr = (int)(color.X * 255);
            var cg = (int)(color.Y * 255);
            var cb = (int)(color.Z * 255);
            var ca = (int)(color.W * 255);
            ImGui.ColorButton("##preview", color, ImGuiColorEditFlags.None, size);
            ImGui.SameLine();
            ImGui.Text(
                $"{text}\n" +
                $"#{cr:X2}{cb:X2}{cg:X2}{ca:X2}\n" +
                $"R: {cr}, G: {cg}, B: {cg}, A: {ca}\n" +
                $"({color.X:F3}, {color.Y:F3}, {color.Z:F3}, {color.W:F3})");
            ImGui.EndTooltip();
        }

        public static Vector4 StainToVector4(uint stainColor)
        {
            var s = 1.0f / 255.0f;

            return new Vector4()
            {
                X = ((stainColor >> 16) & 0xFF) * s,
                Y = ((stainColor >> 8) & 0xFF) * s,
                Z = ((stainColor >> 0) & 0xFF) * s,
                W = ((stainColor >> 24) & 0xFF) * s
            };
        }

    }
}