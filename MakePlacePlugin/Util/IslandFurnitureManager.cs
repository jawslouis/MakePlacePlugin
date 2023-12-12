// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.IslandFurnitureManager
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Runtime.InteropServices;

namespace MakePlacePlugin; 

[StructLayout(LayoutKind.Explicit)]
public struct IslandFurnitureManager {
    [FieldOffset(5784)] public IntPtr ObjectList;
    [FieldOffset(5792)] public unsafe fixed ulong Objects[400];
}
