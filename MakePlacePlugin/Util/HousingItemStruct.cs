// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousingItemStruct
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System.Numerics;
using System.Runtime.InteropServices;

namespace MakePlacePlugin; 

[StructLayout(LayoutKind.Explicit)]
public struct HousingItemStruct {
    [FieldOffset(80)] public Vector3 Position;
    [FieldOffset(96)] public Quaternion Rotation;
    [FieldOffset(248)] public unsafe ItemMaterialManager* MaterialManager;
}
