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

            LandSets = new LandSet[60];

            ushort offset = 0;
            for (int i = 0; i < LandSets.Length; i++)
            {
                LandSets[i].LandRange = parser.ReadOffset<uint>(offset);
                offset += 4;
                LandSets[i].PlacardId = parser.ReadOffset<uint>(offset);
                offset += 4;
                LandSets[i].UnknownRange1 = parser.ReadOffset<uint>(offset);
                offset += 4;
                LandSets[i].InitialPrice = parser.ReadOffset<uint>(offset);
                offset += 4;
                LandSets[i].Size = parser.ReadOffset<byte>(offset);
                offset += 4;
            }
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