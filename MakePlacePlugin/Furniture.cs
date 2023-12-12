// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Furniture
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.Json;

namespace MakePlacePlugin; 

public class Furniture : BasicItem {
    public Transform transform { get; set; } = new();

    public Dictionary<string, object> properties { get; set; } = new();

    public List<Furniture> attachments { get; set; } = new();

    public Color GetColor() {
        object obj;
        return this.properties.TryGetValue("color", out obj)
            ? ColorTranslator.FromHtml("#" + ((string)obj).Substring(0, 6))
            : Color.Empty;
    }

    public BasicItem GetMaterial() {
        object obj;
        return this.properties.TryGetValue("material", out obj) && obj is JsonElement element
            ? element.Deserialize<BasicItem>()
            : new BasicItem();
    }

    private int ColorDiff(Color c1, Color c2) {
        return (int)Math.Sqrt((c1.R - c2.R) * (c1.R - c2.R) +
                              (c1.G - c2.G) * (c1.G - c2.G) +
                              (c1.B - c2.B) * (c1.B - c2.B));
    }

    public uint GetClosestStain(List<(Color, uint)> colorList) {
        var color1 = this.GetColor();
        var num1 = 2000;
        uint closestStain = 0;
        foreach (var color2 in colorList) {
            var num2 = this.ColorDiff(color2.Item1, color1);
            if (num2 < num1) {
                num1 = num2;
                closestStain = color2.Item2;
            }
        }

        return closestStain;
    }
}
