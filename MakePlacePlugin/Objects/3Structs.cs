// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousingModule
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Runtime.InteropServices;

namespace MakePlacePlugin; 

[StructLayout(LayoutKind.Explicit)]
public struct HousingModule {
    [FieldOffset(0)] public unsafe HousingObjectManager* currentTerritory;
    [FieldOffset(8)] public unsafe HousingObjectManager* outdoorTerritory;
    [FieldOffset(16)] public unsafe HousingObjectManager* indoorTerritory;

    public unsafe HousingObjectManager* GetCurrentManager() {
        return (IntPtr)this.outdoorTerritory == IntPtr.Zero ? this.indoorTerritory : this.outdoorTerritory;
    }

    public unsafe bool IsOutdoors() {
        return (IntPtr)this.outdoorTerritory != IntPtr.Zero;
    }

    public unsafe bool IsIndoors() {
        return (IntPtr)this.indoorTerritory != IntPtr.Zero;
    }
}
