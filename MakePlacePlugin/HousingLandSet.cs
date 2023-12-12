// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.HousingLandSet
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace MakePlacePlugin; 

[Sheet("HousingLandSet")]
public class HousingLandSet : ExcelRow {
    public LandSet[] LandSets;

    public uint UnknownRange { get; private set; }

    public uint UnknownRange2 { get; private set; }

    public virtual void PopulateData(RowParser parser, GameData data, Language language) {
        this.RowId = parser.RowId;
        this.SubRowId = parser.SubRowId;
        this.LandSets = new LandSet[60];
        ushort num1 = 0;
        for (var index = 0; index < this.LandSets.Length; ++index) {
            this.LandSets[index].LandRange = parser.ReadOffset<uint>(num1);
            var num2 = (ushort)(num1 + 4U);
            this.LandSets[index].PlacardId = parser.ReadOffset<uint>(num2);
            var num3 = (ushort)(num2 + 4U);
            this.LandSets[index].UnknownRange1 = parser.ReadOffset<uint>(num3);
            var num4 = (ushort)(num3 + 4U);
            this.LandSets[index].InitialPrice = parser.ReadOffset<uint>(num4);
            var num5 = (ushort)(num4 + 4U);
            this.LandSets[index].Size = parser.ReadOffset<byte>(num5);
            num1 = (ushort)(num5 + 4U);
        }

        this.UnknownRange = parser.ReadColumn<uint>(300);
        this.UnknownRange2 = parser.ReadColumn<uint>(301);
    }

    public struct LandSet {
        public uint LandRange;
        public uint PlacardId;
        public uint UnknownRange1;
        public uint InitialPrice;
        public byte Size;
        private unsafe fixed byte padding[3];
    }
}
