// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.IndoorFloorData
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;

namespace MakePlacePlugin; 

public struct IndoorFloorData {
    public const int PartsMax = 5;
    private readonly IntPtr _thisPtr;

    public static IndoorFloorData Get(IntPtr address) {
        return new IndoorFloorData(address);
    }

    private IndoorFloorData(IntPtr thisPtr) {
        this._thisPtr = thisPtr;
    }

    public unsafe int GetPart(InteriorPartsType index) {
        return *(int*)(this._thisPtr + (int)index * 4);
    }

    public unsafe int GetPart(int index) {
        return *(int*)(this._thisPtr + index * 4);
    }
}
