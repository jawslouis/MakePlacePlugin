// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.CommonLandSet
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

namespace MakePlacePlugin; 

public struct CommonLandSet {
    public uint LandRange;
    public uint PlacardId;
    public uint UnknownRange1;
    public uint InitialPrice;
    public byte Size;
    public int PlotIndex;

    public static CommonLandSet FromExd(HousingLandSet.LandSet lset, int index) {
        return new CommonLandSet() {
            LandRange = lset.LandRange,
            PlacardId = lset.PlacardId,
            UnknownRange1 = lset.UnknownRange1,
            InitialPrice = lset.InitialPrice,
            Size = lset.Size,
            PlotIndex = index
        };
    }

    public string SizeString() {
        string str;
        switch (this.Size) {
            case 0:
                str = "Small";
                break;
            case 1:
                str = "Medium";
                break;
            case 2:
                str = "Large";
                break;
            default:
                str = "Apartment";
                break;
        }

        return str;
    }
}
