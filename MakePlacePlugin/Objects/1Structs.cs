// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.CommonFixture
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using Lumina.Excel.GeneratedSheets;

namespace MakePlacePlugin; 

public struct CommonFixture {
    public bool IsExterior;
    public int FixtureType;
    public int FixtureKey;
    public Stain Stain;
    public Item Item;

    public CommonFixture(
        bool isExterior,
        int fixtureType,
        int fixtureKey,
        Stain stain,
        Item item) {
        this.IsExterior = isExterior;
        this.FixtureType = fixtureType;
        this.FixtureKey = fixtureKey;
        this.Stain = stain;
        this.Item = item;
    }
}
