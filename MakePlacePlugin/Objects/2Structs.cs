// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousingObjectManager
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Runtime.InteropServices;

namespace MakePlacePlugin; 

[StructLayout(LayoutKind.Explicit)]
public struct HousingObjectManager {
    [FieldOffset(16)] public IntPtr ObjectList;
    [FieldOffset(35200)] public unsafe fixed ulong Objects[400];
    [FieldOffset(38562)] public byte Ward;
    [FieldOffset(38568)] public byte Plot;
    [FieldOffset(38632)] public unsafe HousingGameObject* IndoorActiveObject2;
    [FieldOffset(38640)] public unsafe HousingGameObject* IndoorHoverObject;
    [FieldOffset(38648)] public unsafe HousingGameObject* IndoorActiveObject;
    [FieldOffset(39608)] public unsafe HousingGameObject* OutdoorActiveObject2;
    [FieldOffset(39616)] public unsafe HousingGameObject* OutdoorHoverObject;
    [FieldOffset(39624)] public unsafe HousingGameObject* OutdoorActiveObject;

    public static unsafe HousingItemInfo* GetItemInfo(HousingObjectManager* mgr, int index) {
        return (HousingItemInfo*)((IntPtr)mgr + new IntPtr(16) + 48 * index);
    }
}
