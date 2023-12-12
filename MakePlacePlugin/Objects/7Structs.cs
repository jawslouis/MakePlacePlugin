// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousingController
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;

namespace MakePlacePlugin; 

public struct HousingController {
    public const int HousesMax = 60;
    private readonly IntPtr _thisPtr;

    public static HousingController Get(IntPtr address) {
        return new HousingController(address);
    }

    private HousingController(IntPtr thisPtr) {
        this._thisPtr = thisPtr;
    }

    public unsafe uint AreaType => *(uint*)this._thisPtr + 8U;

    public unsafe HouseCustomize Houses(int index) {
        return HouseCustomize.Get(new IntPtr((void*)(this._thisPtr + (496 + index * 464))));
    }
}
