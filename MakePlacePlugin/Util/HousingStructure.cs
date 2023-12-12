// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousingStructure
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System.Runtime.InteropServices;

namespace MakePlacePlugin; 

[StructLayout(LayoutKind.Explicit)]
public struct HousingStructure {
    [FieldOffset(0)] public HousingLayoutMode Mode;
    [FieldOffset(4)] public HousingLayoutMode LastMode;
    [FieldOffset(8)] public ItemState State;
    [FieldOffset(16)] public unsafe HousingItemStruct* HoverItem;
    [FieldOffset(24)] public unsafe HousingItemStruct* ActiveItem;
    [FieldOffset(184)] public bool Rotating;
}
