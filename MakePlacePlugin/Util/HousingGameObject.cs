// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousingGameObject
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System.Runtime.InteropServices;

namespace MakePlacePlugin; 

[StructLayout(LayoutKind.Explicit, Size = 464)]
public struct HousingGameObject {
    [FieldOffset(48)] public unsafe fixed byte name[64];
    [FieldOffset(128)] public uint housingRowId;
    [FieldOffset(132)] public uint OwnerID;
    [FieldOffset(176)] public float X;
    [FieldOffset(180)] public float Y;
    [FieldOffset(184)] public float Z;
    [FieldOffset(264)] public unsafe HousingItemStruct* Item;
    [FieldOffset(192)] public float rotation;
    [FieldOffset(424)] public uint housingRowId2;
    [FieldOffset(432)] public byte color;
}
