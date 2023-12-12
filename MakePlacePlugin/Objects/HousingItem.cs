// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.Objects.HousingItem
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Numerics;
using Lumina.Excel.GeneratedSheets;

namespace MakePlacePlugin.Objects; 

public class HousingItem {
    public bool CorrectLocation = true;
    public bool CorrectRotation = true;
    public bool DyeMatch = true;
    public bool IsTableOrWallMounted;
    public uint ItemKey;
    public IntPtr ItemStruct = IntPtr.Zero;
    public uint MaterialItemKey;
    public string Name = "";
    public float Rotate;
    public byte Stain;
    public float X;
    public float Y;
    public float Z;

    public HousingItem(Item item, byte stain, float x, float y, float z, float rotate) {
        this.ItemKey = item.RowId;
        this.Name = item.Name;
        this.IsTableOrWallMounted = item.ItemUICategory.Value.RowId == 78U || item.ItemUICategory.Value.RowId == 79U;
        this.Stain = stain;
        this.X = x;
        this.Y = y;
        this.Z = z;
        this.Rotate = rotate;
    }

    public HousingItem(Item item, HousingGameObject gameObject)
        : this(item, gameObject.color, gameObject.X, gameObject.Y, gameObject.Z, gameObject.rotation) { }

    public Vector3 GetLocation() {
        return new Vector3(this.X, this.Y, this.Z);
    }
}
