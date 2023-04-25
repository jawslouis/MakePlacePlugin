using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace MakePlacePlugin.Objects
{
    public class HousingItem
    {
        public uint ItemKey;
        public byte Stain;
        public float X;
        public float Y;
        public float Z;
        public float Rotate;
        public string Name = "";
        public IntPtr ItemStruct = IntPtr.Zero;
        public bool DyeMatch = true;
        public bool CorrectLocation = true;
        public bool CorrectRotation = true;
        public bool IsTableOrWallMounted = false;

        public HousingItem(Item item, byte stain, float x, float y, float z, float rotate)
        {
            ItemKey = item.RowId;
            Name = item.Name;
            IsTableOrWallMounted = item.ItemUICategory.Value.RowId == 78 || item.ItemUICategory.Value.RowId == 79;

            Stain = stain;
            X = x;
            Y = y;
            Z = z;
            Rotate = rotate;
        }

        public Vector3 GetLocation()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
