// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.IndoorAreaData
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;

namespace MakePlacePlugin; 

public struct IndoorAreaData {
    public const int FloorMax = 4;
    private readonly IntPtr _thisPtr;

    public static IndoorAreaData Get(IntPtr address) {
        return new IndoorAreaData(address);
    }

    private IndoorAreaData(IntPtr thisPtr) {
        this._thisPtr = thisPtr;
    }

    public unsafe IndoorFloorData GetFloor(InteriorFloor index) {
        return IndoorFloorData.Get(new IntPtr((void*)(this._thisPtr + (40 + (int)index * 20))));
    }

    public unsafe IndoorFloorData GetFloor(int index) {
        return IndoorFloorData.Get(new IntPtr((void*)(this._thisPtr + (40 + index * 20))));
    }

    public unsafe float LightLevel => *(float*)(this._thisPtr + 128);
}
