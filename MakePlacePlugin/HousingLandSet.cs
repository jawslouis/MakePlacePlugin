using System.Runtime.InteropServices;
using Lumina;
using Lumina.Data;
using Lumina.Excel;

namespace MakePlacePlugin
{
    [Sheet("HousingLandSet")]
    public class HousingLandSet : ExcelRow
    {
        public LandSet[] LandSets;

        public uint UnknownRange { get; private set; }
        public uint UnknownRange2 { get; private set; }

        public override void PopulateData(RowParser parser, GameData data, Language language)
        {
            RowId = parser.RowId;
            SubRowId = parser.SubRowId;

            LandSets = parser.ReadStructuresAsArray<LandSet>(0, 60);
            UnknownRange = parser.ReadColumn<uint>(300);
            UnknownRange2 = parser.ReadColumn<uint>(301);
        }

        [StructLayout(LayoutKind.Sequential)]
        public unsafe struct LandSet
        {
            public uint LandRange;
            public uint PlacardId;

            public uint UnknownRange1;

            // public uint ExitPopRange;
            public uint InitialPrice;
            public byte Size;
            private fixed byte padding[3];
        }
    }
}