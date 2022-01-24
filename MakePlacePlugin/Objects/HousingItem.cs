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

        public HousingItem(uint itemKey, byte stain, float x, float y, float z, float rotate, string name)
        {
            ItemKey = itemKey;
            Stain = stain;
            X = x;
            Y = y;
            Z = z;
            Rotate = rotate;
            Name = name;
        }

    }
}
