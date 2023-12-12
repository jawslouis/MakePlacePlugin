// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousePart
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System.Runtime.InteropServices;

namespace MakePlacePlugin; 

[StructLayout(LayoutKind.Explicit, Size = 40)]
public struct HousePart {
    [FieldOffset(0)] public int Category;
    [FieldOffset(4)] private readonly int Unknown1;
    [FieldOffset(8)] public ushort FixtureKey;
    [FieldOffset(10)] public byte Color;
    [FieldOffset(11)] private readonly byte Padding;
    [FieldOffset(12)] private readonly int Unknown2;
    [FieldOffset(16)] private readonly unsafe void* Unknown3;
}
