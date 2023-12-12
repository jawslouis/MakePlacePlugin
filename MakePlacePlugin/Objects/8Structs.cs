// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HouseCustomize
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;

namespace MakePlacePlugin; 

public struct HouseCustomize {
    public const int PartsMax = 8;
    private readonly IntPtr _thisPtr;

    public static HouseCustomize Get(IntPtr address) {
        return new HouseCustomize(address);
    }

    private HouseCustomize(IntPtr thisPtr) {
        this._thisPtr = thisPtr;
    }

    public unsafe int Size => *(int*)this._thisPtr + 16;

    public unsafe HousePart GetPart(int type) {
        return *(HousePart*)(this._thisPtr + (32 + type * 40));
    }

    public unsafe HousePart GetPart(ExteriorPartsType type) {
        return *(HousePart*)(this._thisPtr + (32 + (int)type * 40));
    }
}
