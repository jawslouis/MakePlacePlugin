// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.LayoutManager
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Runtime.InteropServices;

namespace MakePlacePlugin; 

[StructLayout(LayoutKind.Explicit)]
public struct LayoutManager {
    [FieldOffset(32)] public uint TerritoryTypeId;
    [FieldOffset(128)] private readonly IntPtr _housingController;
    [FieldOffset(144)] private readonly IntPtr _indoorAreaData;

    public HousingController? HousingController => this._housingController == IntPtr.Zero
        ? new HousingController?()
        : global::MakePlacePlugin.HousingController.Get(this._housingController);

    public IndoorAreaData? IndoorAreaData => this._indoorAreaData == IntPtr.Zero
        ? new IndoorAreaData?()
        : global::MakePlacePlugin.IndoorAreaData.Get(this._indoorAreaData);
}
