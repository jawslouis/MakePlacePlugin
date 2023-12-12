// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousingItemInfo
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System.Runtime.InteropServices;

namespace MakePlacePlugin; 

[StructLayout(LayoutKind.Explicit, Size = 48)]
public struct HousingItemInfo {
    [FieldOffset(0)] public ushort modelId;
    [FieldOffset(16)] public float X;
    [FieldOffset(20)] public float Y;
    [FieldOffset(24)] public float Z;
    [FieldOffset(32)] public float Rotation;
    [FieldOffset(36)] public uint ObjectIndex;
}
