// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Transform
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System.Collections.Generic;

namespace MakePlacePlugin; 

public class Transform {
    public List<float> location { get; set; } = new() {
        0.0f,
        0.0f,
        0.0f
    };

    public List<float> rotation { get; set; } = new() {
        0.0f,
        0.0f,
        0.0f,
        1f
    };

    public List<float> scale { get; set; } = new() {
        1f,
        1f,
        1f
    };
}
