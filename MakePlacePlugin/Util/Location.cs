// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Util.Location
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System.Numerics;

namespace MakePlacePlugin.Util; 

internal struct Location {
    public float x;
    public float y;
    public float z;
    public float rotation;
    public string size;
    public string entranceLayout;

    public Location(float _x, float _y, float _z, float rot, string _size, string layout) {
        this.x = _x;
        this.y = _y;
        this.z = _z;
        this.rotation = rot;
        this.size = _size;
        this.entranceLayout = layout;
    }

    public Vector3 ToVector() {
        return new Vector3(this.x, this.y, this.z);
    }
}
