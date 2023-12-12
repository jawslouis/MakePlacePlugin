// Decompiled with JetBrains decompiler
// Type: MakePlacePlugin.MoveUtil
// Assembly: MakePlacePlugin, Version=3.2.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 33141E60-60A1-49AF-9070-B7E7DF8090BB
// Assembly location: C:\Users\Julian\Downloads\MakePlace\MakePlacePlugin.dll

using System;
using System.Numerics;

namespace MakePlacePlugin; 

internal class MoveUtil {
    private const float Deg2Rad = 0.0174532924f;
    private const float Rad2Deg = 57.29578f;

    public static Quaternion ToQ(Vector3 euler) {
        var num1 = euler.X * (Math.PI / 180.0) * 0.5;
        var num2 = (float)(euler.Y * (Math.PI / 180.0) * 0.5);
        var num3 = (float)(euler.Z * (Math.PI / 180.0) * 0.5);
        var num4 = (float)Math.Sin(num1);
        var num5 = (float)Math.Cos(num1);
        var num6 = (float)Math.Sin(num2);
        var num7 = (float)Math.Cos(num2);
        var num8 = (float)Math.Sin(num3);
        var num9 = (float)Math.Cos(num3);
        Quaternion q;
        q.X = (float)(num7 * (double)num4 * num9 + num6 * (double)num5 * num8);
        q.Y = (float)(num6 * (double)num5 * num9 - num7 * (double)num4 * num8);
        q.Z = (float)(num7 * (double)num5 * num8 - num6 * (double)num4 * num9);
        q.W = (float)(num7 * (double)num5 * num9 + num6 * (double)num4 * num8);
        return q;
    }

    public static Vector3 FromQ(Quaternion q2) {
        var quaternion = new Quaternion(q2.W, q2.Z, q2.X, q2.Y);
        var vector3 = new Vector3 {
            Y = (float)Math.Atan2(2.0 * quaternion.X * quaternion.W + 2.0 * quaternion.Y * quaternion.Z,
                1.0 - 2.0 * (quaternion.Z * (double)quaternion.Z + quaternion.W * (double)quaternion.W)),
            X = (float)Math.Asin(2.0 * (quaternion.X * (double)quaternion.Z - quaternion.W * (double)quaternion.Y)),
            Z = (float)Math.Atan2(2.0 * quaternion.X * quaternion.Y + 2.0 * quaternion.Z * quaternion.W,
                1.0 - 2.0 * (quaternion.Y * (double)quaternion.Y + quaternion.Z * (double)quaternion.Z))
        };
        vector3.X *= 57.29578f;
        vector3.Y *= 57.29578f;
        vector3.Z *= 57.29578f;
        return vector3;
    }

    public static float DistanceFromPlayer(HousingGameObject obj, Vector3 playerPos) {
        return Distance(new Vector3(playerPos.X, playerPos.Z, playerPos.Y), new Vector3(obj.X, obj.Y, obj.Z));
    }

    public static float Distance(Vector3 v1, Vector3 v2) {
        var num1 = Math.Pow(v2.X - (double)v1.X, 2.0);
        var num2 = Math.Pow(v2.Y - (double)v1.Y, 2.0);
        var num3 = Math.Pow(v2.Z - (double)v1.Z, 2.0);
        var num4 = num2;
        return (float)Math.Sqrt(num1 + num4 + num3);
    }
}
