// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Fixture
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

namespace MakePlacePlugin; 

public class Fixture : BasicItem {
    public Fixture() { }

    public Fixture(string inType) {
        this.type = inType;
    }

    public Fixture(string inType, string inName)
        : this(inType) {
        this.name = inName;
    }

    public Fixture(string inType, string inName, string inLevel)
        : this(inType, inName) {
        this.level = inLevel;
    }

    public string level { get; set; } = "";

    public string type { get; set; } = "";
}
