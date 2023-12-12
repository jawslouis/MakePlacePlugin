// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Layout
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System.Collections.Generic;

namespace MakePlacePlugin; 

public class Layout {
    public Transform playerTransform { get; set; } = new();

    public string houseSize { get; set; } = "";

    public float interiorScale { get; set; } = 1f;

    public List<Fixture> interiorFixture { get; set; } = new();

    public List<Furniture> interiorFurniture { get; set; } = new();

    public float exteriorScale { get; set; } = 1f;

    public List<Fixture> exteriorFixture { get; set; } = new();

    public List<Furniture> exteriorFurniture { get; set; } = new();

    public Dictionary<string, object> properties { get; set; } = new();
}
