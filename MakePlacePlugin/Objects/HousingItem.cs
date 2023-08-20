using Lumina.Excel.GeneratedSheets;
using System;
using System.Numerics;

namespace MakePlacePlugin.Objects
{
    public class HousingItem
    {
        public uint ItemKey;
        public byte Stain;
        public uint MaterialItemKey = 0;
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

        public HousingItem(Item item, HousingGameObject gameObject)
            : this(item, gameObject.color, gameObject.X, gameObject.Y, gameObject.Z, gameObject.rotation)
        {
        }

        public Vector3 GetLocation()
        {
            return new Vector3(X, Y, Z);
        }
    }
}
