// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Utils
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using Lumina.Excel.GeneratedSheets;
using MakePlacePlugin.Objects;

namespace MakePlacePlugin; 

public static class Utils {
    public static string GetExteriorPartDescriptor(ExteriorPartsType partsType) {
        string exteriorPartDescriptor;
        switch (partsType) {
            case ExteriorPartsType.Roof:
                exteriorPartDescriptor = "Roof";
                break;
            case ExteriorPartsType.Walls:
                exteriorPartDescriptor = "Exterior Wall";
                break;
            case ExteriorPartsType.Windows:
                exteriorPartDescriptor = "Window";
                break;
            case ExteriorPartsType.Door:
                exteriorPartDescriptor = "Door";
                break;
            case ExteriorPartsType.RoofOpt:
                exteriorPartDescriptor = "Roof Decor";
                break;
            case ExteriorPartsType.WallOpt:
                exteriorPartDescriptor = "Exterior Wall Decor";
                break;
            case ExteriorPartsType.SignOpt:
                exteriorPartDescriptor = "Placard";
                break;
            case ExteriorPartsType.Fence:
                exteriorPartDescriptor = "Fence";
                break;
            default:
                exteriorPartDescriptor = "Unknown";
                break;
        }

        return exteriorPartDescriptor;
    }

    public static string GetInteriorPartDescriptor(InteriorPartsType partsType) {
        string interiorPartDescriptor;
        switch (partsType) {
            case InteriorPartsType.Walls:
                interiorPartDescriptor = "Wall";
                break;
            case InteriorPartsType.Windows:
                interiorPartDescriptor = "Window";
                break;
            case InteriorPartsType.Door:
                interiorPartDescriptor = "Door";
                break;
            case InteriorPartsType.Floor:
                interiorPartDescriptor = "Floor";
                break;
            case InteriorPartsType.Light:
                interiorPartDescriptor = "Light";
                break;
            default:
                interiorPartDescriptor = "Unknown";
                break;
        }

        return interiorPartDescriptor;
    }

    public static string GetFloorDescriptor(InteriorFloor floor) {
        string floorDescriptor;
        switch (floor) {
            case InteriorFloor.Ground:
                floorDescriptor = "Ground Floor";
                break;
            case InteriorFloor.Upstairs:
                floorDescriptor = "Upper Floor";
                break;
            case InteriorFloor.Basement:
                floorDescriptor = "Basement";
                break;
            case InteriorFloor.External:
                floorDescriptor = "Main";
                break;
            default:
                floorDescriptor = "Unknown";
                break;
        }

        return floorDescriptor;
    }

    public static float DistanceFromPlayer(HousingGameObject obj, Vector3 playerPos) {
        return Distance(new Vector3(playerPos.X, playerPos.Y, playerPos.Z), new Vector3(obj.X, obj.Y, obj.Z));
    }

    public static double FastDistance(Vector3 v1, Vector3 v2) {
        var num1 = Math.Pow(v2.X - (double)v1.X, 2.0);
        var num2 = Math.Pow(v2.Y - (double)v1.Y, 2.0);
        var num3 = Math.Pow(v2.Z - (double)v1.Z, 2.0);
        var num4 = num2;
        return num1 + num4 + num3;
    }

    public static float Distance(Vector3 v1, Vector3 v2) {
        return (float)Math.Sqrt(FastDistance(v1, v2));
    }

    public static void StainButton(string id, Stain color, Vector2 size) {
        var vector4 = StainToVector4(color.Color);
        ImGui.ColorButton("##" + id, vector4, (ImGuiColorEditFlags)64, size);
    }

    public static Vector4 StainToVector4(uint stainColor) {
        var num = 0.003921569f;
        return new Vector4 {
            X = ((stainColor >> 16) & byte.MaxValue) * num,
            Y = ((stainColor >> 8) & byte.MaxValue) * num,
            Z = (stainColor & byte.MaxValue) * num,
            W = ((stainColor >> 24) & byte.MaxValue) * num
        };
    }

    public static HousingItem GetNearestHousingItem(
        IEnumerable<HousingItem> items,
        Vector3 position) {
        return items
            .OrderBy((Func<HousingItem, double>)(item => FastDistance(position, new Vector3(item.X, item.Y, item.Z))))
            .FirstOrDefault();
    }
}
